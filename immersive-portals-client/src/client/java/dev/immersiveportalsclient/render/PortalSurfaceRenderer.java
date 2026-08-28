package dev.immersiveportalsclient.render;

import java.util.ArrayList;
import java.util.List;

import com.mojang.blaze3d.vertex.PoseStack;
import com.mojang.blaze3d.vertex.VertexConsumer;
import dev.immersiveportalsclient.state.PortalState;
import net.fabricmc.fabric.api.client.rendering.v1.level.LevelExtractionContext;
import net.fabricmc.fabric.api.client.rendering.v1.level.LevelExtractionEvents;
import net.fabricmc.fabric.api.client.rendering.v1.level.LevelRenderContext;
import net.fabricmc.fabric.api.client.rendering.v1.level.LevelRenderEvents;
import net.minecraft.client.Minecraft;
import net.minecraft.client.multiplayer.ClientLevel;
import net.minecraft.client.renderer.rendertype.RenderTypes;
import net.minecraft.core.BlockPos;
import net.minecraft.resources.ResourceKey;
import net.minecraft.server.MinecraftServer;
import net.minecraft.server.level.ServerLevel;
import net.minecraft.util.ARGB;
import net.minecraft.world.level.Level;
import net.minecraft.world.level.block.Blocks;
import net.minecraft.world.level.block.state.BlockState;
import net.minecraft.world.phys.AABB;
import net.minecraft.world.phys.Vec3;

public final class PortalSurfaceRenderer {
    private enum Plane { X, Z, HORIZONTAL }
    private record PortalCell(int x, int y, int z, Plane plane, PortalState.Kind kind) { }
    private record PortalTile(AABB bounds, int color) { }

    private static final int SCAN_RADIUS = 10;
    private static final int MAX_CELLS = 96;
    private static final int PREVIEW_RESOLUTION = 8;
    private static final int PREVIEW_DEPTH = 28;
    private static final double HALF_THICKNESS = 0.032;
    private static volatile List<PortalTile> tiles = List.of();

    private PortalSurfaceRenderer() { }

    public static void register() {
        LevelExtractionEvents.END_EXTRACTION.register(PortalSurfaceRenderer::extract);
        LevelRenderEvents.BEFORE_TRANSLUCENT_TERRAIN.register(PortalSurfaceRenderer::render);
    }

    private static void extract(LevelExtractionContext context) {
        ClientLevel level = context.level();
        Vec3 camera = context.levelState().cameraRenderState.pos;
        List<PortalCell> portalCells = findPortalCells(level, camera);
        if (portalCells.isEmpty()) {
            tiles = List.of();
            return;
        }

        Minecraft client = Minecraft.getInstance();
        MinecraftServer integratedServer = client.getSingleplayerServer();
        ServerLevel destination = null;
        ResourceKey<Level> currentDimension = level.dimension();

        if (integratedServer != null) {
            PortalState.Kind firstKind = portalCells.getFirst().kind();
            ResourceKey<Level> target = targetDimension(currentDimension, firstKind);
            destination = integratedServer.getLevel(target);
        }

        ArrayList<PortalTile> next = new ArrayList<>();
        for (PortalCell cell : portalCells) {
            appendTiles(next, cell, camera, currentDimension, destination);
        }
        tiles = List.copyOf(next);
    }

    private static List<PortalCell> findPortalCells(ClientLevel level, Vec3 camera) {
        BlockPos center = BlockPos.containing(camera.x, camera.y, camera.z);
        BlockPos.MutableBlockPos pos = new BlockPos.MutableBlockPos();
        ArrayList<PortalCell> next = new ArrayList<>();

        outer:
        for (int y = -SCAN_RADIUS; y <= SCAN_RADIUS; y++) {
            for (int x = -SCAN_RADIUS; x <= SCAN_RADIUS; x++) {
                for (int z = -SCAN_RADIUS; z <= SCAN_RADIUS; z++) {
                    pos.set(center.getX() + x, center.getY() + y, center.getZ() + z);
                    BlockState state = level.getBlockState(pos);
                    PortalState.Kind kind;
                    Plane plane;
                    if (state.is(Blocks.NETHER_PORTAL)) {
                        kind = PortalState.Kind.NETHER;
                        boolean eastWest = level.getBlockState(pos.east()).is(Blocks.NETHER_PORTAL)
                            || level.getBlockState(pos.west()).is(Blocks.NETHER_PORTAL);
                        plane = eastWest ? Plane.X : Plane.Z;
                    } else if (state.is(Blocks.END_PORTAL)) {
                        kind = PortalState.Kind.END;
                        plane = Plane.HORIZONTAL;
                    } else {
                        continue;
                    }

                    next.add(new PortalCell(pos.getX(), pos.getY(), pos.getZ(), plane, kind));
                    if (next.size() >= MAX_CELLS) break outer;
                }
            }
        }
        return next;
    }

    private static ResourceKey<Level> targetDimension(ResourceKey<Level> current, PortalState.Kind kind) {
        if (kind == PortalState.Kind.NETHER) {
            return Level.NETHER.equals(current) ? Level.OVERWORLD : Level.NETHER;
        }
        if (kind == PortalState.Kind.END) {
            return Level.END.equals(current) ? Level.OVERWORLD : Level.END;
        }
        return current;
    }

    private static void appendTiles(
        List<PortalTile> out,
        PortalCell cell,
        Vec3 camera,
        ResourceKey<Level> currentDimension,
        ServerLevel destination
    ) {
        for (int v = 0; v < PREVIEW_RESOLUTION; v++) {
            for (int u = 0; u < PREVIEW_RESOLUTION; u++) {
                int color = destination == null
                    ? fallbackColor(cell.kind(), u, v)
                    : sampleDestinationColor(destination, currentDimension, cell, camera, u, v);
                out.add(new PortalTile(tileBounds(cell, u, v), color));
            }
        }
    }

    private static int sampleDestinationColor(
        ServerLevel destination,
        ResourceKey<Level> currentDimension,
        PortalCell cell,
        Vec3 camera,
        int u,
        int v
    ) {
        double scale = Level.NETHER.equals(currentDimension) ? 8.0 : 1.0 / 8.0;
        if (cell.kind() == PortalState.Kind.END) scale = 1.0;

        int baseX;
        int baseY = cell.y();
        int baseZ;
        if (cell.kind() == PortalState.Kind.END && !Level.END.equals(currentDimension)) {
            baseX = 100;
            baseY = 50;
            baseZ = 0;
        } else {
            baseX = (int) Math.floor(cell.x() * scale);
            baseZ = (int) Math.floor(cell.z() * scale);
        }

        double across = (u + 0.5) / PREVIEW_RESOLUTION - 0.5;
        double vertical = (v + 0.5) / PREVIEW_RESOLUTION - 0.5;
        int normalSign;
        int sampleX = baseX;
        int sampleY = baseY + (int) Math.floor(vertical * 7.0);
        int sampleZ = baseZ;

        if (cell.plane() == Plane.X) {
            sampleX += (int) Math.floor(across * 9.0);
            normalSign = camera.z < cell.z() + 0.5 ? 1 : -1;
        } else if (cell.plane() == Plane.Z) {
            sampleZ += (int) Math.floor(across * 9.0);
            normalSign = camera.x < cell.x() + 0.5 ? 1 : -1;
        } else {
            sampleX += (int) Math.floor(across * 9.0);
            sampleZ += (int) Math.floor(vertical * 9.0);
            normalSign = -1;
        }

        BlockPos.MutableBlockPos cursor = new BlockPos.MutableBlockPos();
        for (int depth = 1; depth <= PREVIEW_DEPTH; depth++) {
            int x = sampleX;
            int y = sampleY;
            int z = sampleZ;
            if (cell.plane() == Plane.X) z += depth * normalSign;
            else if (cell.plane() == Plane.Z) x += depth * normalSign;
            else y -= depth;

            cursor.set(x, y, z);
            if (!destination.hasChunkAt(cursor)) continue;
            BlockState state = destination.getBlockState(cursor);
            if (!state.isAir()) {
                return shade(colorFor(state, destination.dimension()), depth);
            }
        }

        return backgroundColor(destination.dimension(), u, v);
    }

    private static int colorFor(BlockState state, ResourceKey<Level> dimension) {
        if (state.is(Blocks.LAVA)) return ARGB.colorFromFloat(1.0f, 1.0f, 0.22f, 0.02f);
        if (state.is(Blocks.NETHERRACK)) return ARGB.colorFromFloat(1.0f, 0.42f, 0.12f, 0.10f);
        if (state.is(Blocks.NETHER_BRICKS)) return ARGB.colorFromFloat(1.0f, 0.25f, 0.05f, 0.06f);
        if (state.is(Blocks.BLACKSTONE) || state.is(Blocks.BASALT)) return ARGB.colorFromFloat(1.0f, 0.16f, 0.14f, 0.16f);
        if (state.is(Blocks.SOUL_SAND) || state.is(Blocks.SOUL_SOIL)) return ARGB.colorFromFloat(1.0f, 0.28f, 0.22f, 0.16f);
        if (state.is(Blocks.CRIMSON_NYLIUM)) return ARGB.colorFromFloat(1.0f, 0.46f, 0.08f, 0.16f);
        if (state.is(Blocks.WARPED_NYLIUM)) return ARGB.colorFromFloat(1.0f, 0.08f, 0.38f, 0.34f);
        if (state.is(Blocks.END_STONE)) return ARGB.colorFromFloat(1.0f, 0.80f, 0.82f, 0.52f);
        if (state.is(Blocks.OBSIDIAN)) return ARGB.colorFromFloat(1.0f, 0.09f, 0.05f, 0.16f);
        if (state.is(Blocks.WATER)) return ARGB.colorFromFloat(1.0f, 0.12f, 0.30f, 0.78f);
        if (state.is(Blocks.GRASS_BLOCK)) return ARGB.colorFromFloat(1.0f, 0.30f, 0.58f, 0.18f);
        if (state.is(Blocks.DIRT)) return ARGB.colorFromFloat(1.0f, 0.40f, 0.26f, 0.15f);
        if (state.is(Blocks.STONE) || state.is(Blocks.COBBLESTONE)) return ARGB.colorFromFloat(1.0f, 0.46f, 0.46f, 0.46f);
        if (state.is(Blocks.SNOW_BLOCK) || state.is(Blocks.SNOW)) return ARGB.colorFromFloat(1.0f, 0.90f, 0.96f, 1.0f);
        if (state.is(Blocks.OAK_LEAVES) || state.is(Blocks.SPRUCE_LEAVES) || state.is(Blocks.BIRCH_LEAVES)) {
            return ARGB.colorFromFloat(1.0f, 0.16f, 0.45f, 0.12f);
        }
        if (Level.NETHER.equals(dimension)) return ARGB.colorFromFloat(1.0f, 0.38f, 0.13f, 0.10f);
        if (Level.END.equals(dimension)) return ARGB.colorFromFloat(1.0f, 0.67f, 0.68f, 0.42f);
        return ARGB.colorFromFloat(1.0f, 0.50f, 0.50f, 0.50f);
    }

    private static int shade(int color, int depth) {
        float factor = Math.max(0.42f, 1.0f - depth / 42.0f);
        int r = (int) (((color >> 16) & 0xFF) * factor);
        int g = (int) (((color >> 8) & 0xFF) * factor);
        int b = (int) ((color & 0xFF) * factor);
        return 0xFF000000 | (r << 16) | (g << 8) | b;
    }

    private static int backgroundColor(ResourceKey<Level> dimension, int u, int v) {
        float variation = ((u * 17 + v * 31) & 7) / 90.0f;
        if (Level.NETHER.equals(dimension)) {
            return ARGB.colorFromFloat(1.0f, 0.16f + variation, 0.025f, 0.02f);
        }
        if (Level.END.equals(dimension)) {
            boolean star = ((u * 13 + v * 29) % 19) == 0;
            return star ? 0xFFD9D7FF : 0xFF08070D;
        }
        return ARGB.colorFromFloat(1.0f, 0.38f + variation, 0.64f + variation, 0.92f);
    }

    private static int fallbackColor(PortalState.Kind kind, int u, int v) {
        float pulse = ((u + v) & 1) == 0 ? 1.0f : 0.72f;
        if (kind == PortalState.Kind.END) {
            return ARGB.colorFromFloat(1.0f, 0.025f * pulse, 0.02f * pulse, 0.05f * pulse);
        }
        return ARGB.colorFromFloat(1.0f, 0.36f * pulse, 0.035f, 0.55f * pulse);
    }

    private static AABB tileBounds(PortalCell cell, int u, int v) {
        double a0 = (double) u / PREVIEW_RESOLUTION;
        double a1 = (double) (u + 1) / PREVIEW_RESOLUTION;
        double b0 = (double) v / PREVIEW_RESOLUTION;
        double b1 = (double) (v + 1) / PREVIEW_RESOLUTION;
        double x = cell.x();
        double y = cell.y();
        double z = cell.z();

        return switch (cell.plane()) {
            case X -> new AABB(x + a0, y + b0, z + 0.5 - HALF_THICKNESS, x + a1, y + b1, z + 0.5 + HALF_THICKNESS);
            case Z -> new AABB(x + 0.5 - HALF_THICKNESS, y + b0, z + a0, x + 0.5 + HALF_THICKNESS, y + b1, z + a1);
            case HORIZONTAL -> new AABB(x + a0, y + 0.75 - HALF_THICKNESS, z + b0, x + a1, y + 0.75 + HALF_THICKNESS, z + b1);
        };
    }

    private static void render(LevelRenderContext context) {
        List<PortalTile> frameTiles = tiles;
        if (frameTiles.isEmpty()) return;

        Vec3 camera = context.levelState().cameraRenderState.pos;
        PoseStack poseStack = context.poseStack();
        poseStack.pushPose();
        poseStack.translate(-camera.x, -camera.y, -camera.z);
        context.submitNodeCollector().submitCustomGeometry(
            poseStack,
            RenderTypes.debugFilledBox(),
            (pose, buffer) -> drawTiles(pose, buffer, frameTiles)
        );
        poseStack.popPose();
    }

    private static void drawTiles(PoseStack.Pose pose, VertexConsumer buffer, List<PortalTile> frameTiles) {
        for (PortalTile tile : frameTiles) drawFilledBox(pose, buffer, tile.bounds(), tile.color());
    }

    private static void drawFilledBox(PoseStack.Pose pose, VertexConsumer v, AABB b, int c) {
        v.addVertex(pose,(float)b.minX,(float)b.minY,(float)b.minZ).setColor(c); v.addVertex(pose,(float)b.maxX,(float)b.minY,(float)b.minZ).setColor(c); v.addVertex(pose,(float)b.maxX,(float)b.maxY,(float)b.minZ).setColor(c); v.addVertex(pose,(float)b.minX,(float)b.maxY,(float)b.minZ).setColor(c);
        v.addVertex(pose,(float)b.maxX,(float)b.minY,(float)b.maxZ).setColor(c); v.addVertex(pose,(float)b.minX,(float)b.minY,(float)b.maxZ).setColor(c); v.addVertex(pose,(float)b.minX,(float)b.maxY,(float)b.maxZ).setColor(c); v.addVertex(pose,(float)b.maxX,(float)b.maxY,(float)b.maxZ).setColor(c);
        v.addVertex(pose,(float)b.minX,(float)b.minY,(float)b.maxZ).setColor(c); v.addVertex(pose,(float)b.minX,(float)b.minY,(float)b.minZ).setColor(c); v.addVertex(pose,(float)b.minX,(float)b.maxY,(float)b.minZ).setColor(c); v.addVertex(pose,(float)b.minX,(float)b.maxY,(float)b.maxZ).setColor(c);
        v.addVertex(pose,(float)b.maxX,(float)b.minY,(float)b.minZ).setColor(c); v.addVertex(pose,(float)b.maxX,(float)b.minY,(float)b.maxZ).setColor(c); v.addVertex(pose,(float)b.maxX,(float)b.maxY,(float)b.maxZ).setColor(c); v.addVertex(pose,(float)b.maxX,(float)b.maxY,(float)b.minZ).setColor(c);
        v.addVertex(pose,(float)b.minX,(float)b.maxY,(float)b.minZ).setColor(c); v.addVertex(pose,(float)b.maxX,(float)b.maxY,(float)b.minZ).setColor(c); v.addVertex(pose,(float)b.maxX,(float)b.maxY,(float)b.maxZ).setColor(c); v.addVertex(pose,(float)b.minX,(float)b.maxY,(float)b.maxZ).setColor(c);
        v.addVertex(pose,(float)b.minX,(float)b.minY,(float)b.maxZ).setColor(c); v.addVertex(pose,(float)b.maxX,(float)b.minY,(float)b.maxZ).setColor(c); v.addVertex(pose,(float)b.maxX,(float)b.minY,(float)b.minZ).setColor(c); v.addVertex(pose,(float)b.minX,(float)b.minY,(float)b.minZ).setColor(c);
    }
}
