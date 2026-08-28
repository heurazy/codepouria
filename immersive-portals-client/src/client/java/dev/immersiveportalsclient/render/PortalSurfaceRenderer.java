package dev.immersiveportalsclient.render;

import java.util.ArrayList;
import java.util.List;
import java.util.concurrent.atomic.AtomicBoolean;

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
import net.minecraft.world.level.Level;
import net.minecraft.world.level.block.Blocks;
import net.minecraft.world.level.block.state.BlockState;
import net.minecraft.world.phys.AABB;
import net.minecraft.world.phys.Vec3;

public final class PortalSurfaceRenderer {
    private enum Plane { X, Z, HORIZONTAL }

    private record PortalCell(int x, int y, int z, Plane plane, PortalState.Kind kind) { }

    private record PortalGroup(
        int minX, int minY, int minZ,
        int maxX, int maxY, int maxZ,
        Plane plane, PortalState.Kind kind
    ) {
        int widthBlocks() {
            return plane == Plane.X ? maxX - minX + 1 :
                plane == Plane.Z ? maxZ - minZ + 1 :
                maxX - minX + 1;
        }

        int heightBlocks() {
            return plane == Plane.HORIZONTAL ? maxZ - minZ + 1 : maxY - minY + 1;
        }

        double centerX() { return (minX + maxX + 1.0) * 0.5; }
        double centerY() { return (minY + maxY + 1.0) * 0.5; }
        double centerZ() { return (minZ + maxZ + 1.0) * 0.5; }
    }

    private record PortalTile(AABB bounds, int color) { }
    private record Anchor(double x, double y, double z) { }

    private static final int SCAN_RADIUS = 12;
    private static final int MAX_CELLS = 128;
    private static final int PIXELS_PER_BLOCK = 10;
    private static final int RAY_STEPS = 128;
    private static final double RAY_STEP = 0.42;
    private static final double HALF_THICKNESS = 0.018;
    private static final long REBUILD_INTERVAL_TICKS = 8;

    private static final AtomicBoolean BUILD_QUEUED = new AtomicBoolean();
    private static volatile List<PortalTile> tiles = List.of();
    private static long lastScheduledTick = Long.MIN_VALUE;

    private PortalSurfaceRenderer() { }

    public static void register() {
        LevelExtractionEvents.END_EXTRACTION.register(PortalSurfaceRenderer::extract);
        LevelRenderEvents.BEFORE_TRANSLUCENT_TERRAIN.register(PortalSurfaceRenderer::render);
    }

    private static void extract(LevelExtractionContext context) {
        ClientLevel level = context.level();
        Vec3 camera = context.levelState().cameraRenderState.pos;
        List<PortalCell> cells = findPortalCells(level, camera);

        if (cells.isEmpty()) {
            tiles = List.of();
            return;
        }

        List<PortalGroup> groups = groupPortalCells(cells);
        Minecraft client = Minecraft.getInstance();
        MinecraftServer integratedServer = client.getSingleplayerServer();

        if (integratedServer == null) {
            tiles = buildFallback(groups);
            return;
        }

        long gameTime = level.getGameTime();
        if (gameTime - lastScheduledTick < REBUILD_INTERVAL_TICKS) return;
        if (!BUILD_QUEUED.compareAndSet(false, true)) return;

        lastScheduledTick = gameTime;
        ResourceKey<Level> currentDimension = level.dimension();
        List<PortalGroup> immutableGroups = List.copyOf(groups);
        Vec3 immutableCamera = new Vec3(camera.x, camera.y, camera.z);

        integratedServer.execute(() -> {
            try {
                ArrayList<PortalTile> next = new ArrayList<>();
                for (PortalGroup group : immutableGroups) {
                    ResourceKey<Level> targetKey = targetDimension(currentDimension, group.kind());
                    ServerLevel destination = integratedServer.getLevel(targetKey);
                    if (destination == null) {
                        appendFallback(next, group);
                        continue;
                    }
                    appendPerspectivePreview(next, group, immutableCamera, currentDimension, destination);
                }
                tiles = List.copyOf(next);
            } catch (Throwable ignored) {
                tiles = buildFallback(immutableGroups);
            } finally {
                BUILD_QUEUED.set(false);
            }
        });
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

    private static List<PortalGroup> groupPortalCells(List<PortalCell> cells) {
        boolean[] visited = new boolean[cells.size()];
        ArrayList<PortalGroup> groups = new ArrayList<>();

        for (int start = 0; start < cells.size(); start++) {
            if (visited[start]) continue;
            PortalCell seed = cells.get(start);
            ArrayList<Integer> queue = new ArrayList<>();
            queue.add(start);
            visited[start] = true;

            int minX = seed.x(), minY = seed.y(), minZ = seed.z();
            int maxX = seed.x(), maxY = seed.y(), maxZ = seed.z();

            for (int qi = 0; qi < queue.size(); qi++) {
                PortalCell a = cells.get(queue.get(qi));
                minX = Math.min(minX, a.x());
                minY = Math.min(minY, a.y());
                minZ = Math.min(minZ, a.z());
                maxX = Math.max(maxX, a.x());
                maxY = Math.max(maxY, a.y());
                maxZ = Math.max(maxZ, a.z());

                for (int j = 0; j < cells.size(); j++) {
                    if (visited[j]) continue;
                    PortalCell b = cells.get(j);
                    if (b.kind() != seed.kind() || b.plane() != seed.plane()) continue;

                    int manhattan = Math.abs(a.x() - b.x())
                        + Math.abs(a.y() - b.y())
                        + Math.abs(a.z() - b.z());

                    if (manhattan == 1) {
                        visited[j] = true;
                        queue.add(j);
                    }
                }
            }

            groups.add(new PortalGroup(
                minX, minY, minZ,
                maxX, maxY, maxZ,
                seed.plane(), seed.kind()
            ));
        }

        return groups;
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

    private static void appendPerspectivePreview(
        List<PortalTile> out,
        PortalGroup group,
        Vec3 camera,
        ResourceKey<Level> currentDimension,
        ServerLevel destination
    ) {
        double scale = 1.0;
        if (group.kind() == PortalState.Kind.NETHER) {
            scale = Level.NETHER.equals(currentDimension) ? 8.0 : 1.0 / 8.0;
        }

        double expectedX = group.centerX() * scale;
        double expectedY = group.centerY();
        double expectedZ = group.centerZ() * scale;

        if (group.kind() == PortalState.Kind.END && !Level.END.equals(currentDimension)) {
            expectedX = 100.0;
            expectedY = 50.0;
            expectedZ = 0.0;
        }

        forceDestinationArea(destination, expectedX, expectedZ);
        Anchor anchor = findDestinationAnchor(destination, group.kind(), expectedX, expectedY, expectedZ);

        double cameraAcross;
        double cameraVertical = camera.y - group.centerY();

        if (group.plane() == Plane.X) cameraAcross = camera.x - group.centerX();
        else if (group.plane() == Plane.Z) cameraAcross = camera.z - group.centerZ();
        else cameraAcross = camera.x - group.centerX();

        int forwardSign;
        if (group.plane() == Plane.X) {
            forwardSign = camera.z < group.centerZ() ? 1 : -1;
        } else if (group.plane() == Plane.Z) {
            forwardSign = camera.x < group.centerX() ? 1 : -1;
        } else {
            forwardSign = -1;
        }

        int pixelWidth = Math.max(1, group.widthBlocks() * PIXELS_PER_BLOCK);
        int pixelHeight = Math.max(1, group.heightBlocks() * PIXELS_PER_BLOCK);
        double aspect = (double) pixelWidth / pixelHeight;
        double tanHalfFov = Math.tan(Math.toRadians(68.0) * 0.5);

        double originX = anchor.x();
        double originY = anchor.y() + 1.62 + cameraVertical * 0.28;
        double originZ = anchor.z();

        if (group.plane() == Plane.X) originX += cameraAcross * 0.55;
        else if (group.plane() == Plane.Z) originZ += cameraAcross * 0.55;
        else {
            originX += cameraAcross * 0.45;
            originZ += (camera.z - group.centerZ()) * 0.45;
        }

        for (int py = 0; py < pixelHeight; py++) {
            for (int px = 0; px < pixelWidth; px++) {
                double ndcX = ((px + 0.5) / pixelWidth) * 2.0 - 1.0;
                double ndcY = 1.0 - ((py + 0.5) / pixelHeight) * 2.0;

                double right = ndcX * aspect * tanHalfFov;
                double up = ndcY * tanHalfFov;

                double dirX;
                double dirY = up;
                double dirZ;

                if (group.plane() == Plane.X) {
                    dirX = right;
                    dirZ = forwardSign;
                } else if (group.plane() == Plane.Z) {
                    dirX = forwardSign;
                    dirZ = right;
                } else {
                    dirX = right;
                    dirY = -1.0;
                    dirZ = up;
                }

                double invLen = 1.0 / Math.sqrt(dirX * dirX + dirY * dirY + dirZ * dirZ);
                dirX *= invLen;
                dirY *= invLen;
                dirZ *= invLen;

                int color = traceColor(
                    destination,
                    originX, originY, originZ,
                    dirX, dirY, dirZ,
                    px, py
                );

                out.add(new PortalTile(pixelBounds(group, px, py, pixelWidth, pixelHeight), color));
            }
        }
    }

    private static void forceDestinationArea(ServerLevel destination, double x, double z) {
        int centerChunkX = ((int) Math.floor(x)) >> 4;
        int centerChunkZ = ((int) Math.floor(z)) >> 4;

        for (int dz = -2; dz <= 2; dz++) {
            for (int dx = -2; dx <= 2; dx++) {
                destination.getChunk(centerChunkX + dx, centerChunkZ + dz);
            }
        }
    }

    private static Anchor findDestinationAnchor(
        ServerLevel destination,
        PortalState.Kind kind,
        double expectedX,
        double expectedY,
        double expectedZ
    ) {
        int ex = (int) Math.floor(expectedX);
        int ez = (int) Math.floor(expectedZ);
        int minY = Level.NETHER.equals(destination.dimension()) ? 8 : 5;
        int maxY = Level.NETHER.equals(destination.dimension()) ? 118 : 250;
        int ey = clamp((int) Math.floor(expectedY), minY + 2, maxY - 3);

        BlockPos.MutableBlockPos p = new BlockPos.MutableBlockPos();

        for (int radius = 0; radius <= 14; radius++) {
            for (int dz = -radius; dz <= radius; dz++) {
                for (int dx = -radius; dx <= radius; dx++) {
                    if (radius > 0 && Math.max(Math.abs(dx), Math.abs(dz)) != radius) continue;

                    int x = ex + dx;
                    int z = ez + dz;
                    for (int y = minY; y <= maxY; y++) {
                        p.set(x, y, z);
                        BlockState state = destination.getBlockState(p);
                        if ((kind == PortalState.Kind.NETHER && state.is(Blocks.NETHER_PORTAL))
                            || (kind == PortalState.Kind.END && state.is(Blocks.END_PORTAL))) {
                            return new Anchor(x + 0.5, y + 0.15, z + 0.5);
                        }
                    }
                }
            }
        }

        for (int radius = 0; radius <= 20; radius++) {
            for (int dz = -radius; dz <= radius; dz++) {
                for (int dx = -radius; dx <= radius; dx++) {
                    if (radius > 0 && Math.max(Math.abs(dx), Math.abs(dz)) != radius) continue;

                    int x = ex + dx;
                    int z = ez + dz;

                    for (int offset = 0; offset <= 44; offset++) {
                        int yA = ey + offset;
                        if (yA <= maxY && isOpenStandingSpot(destination, p, x, yA, z)) {
                            return new Anchor(x + 0.5, yA, z + 0.5);
                        }

                        if (offset != 0) {
                            int yB = ey - offset;
                            if (yB >= minY && isOpenStandingSpot(destination, p, x, yB, z)) {
                                return new Anchor(x + 0.5, yB, z + 0.5);
                            }
                        }
                    }
                }
            }
        }

        return new Anchor(expectedX + 0.5, ey, expectedZ + 0.5);
    }

    private static boolean isOpenStandingSpot(
        ServerLevel level,
        BlockPos.MutableBlockPos p,
        int x, int y, int z
    ) {
        p.set(x, y - 1, z);
        BlockState floor = level.getBlockState(p);
        if (floor.isAir() || floor.is(Blocks.LAVA) || floor.is(Blocks.WATER)) return false;

        p.set(x, y, z);
        if (!isTransparentForPreview(level.getBlockState(p))) return false;

        p.set(x, y + 1, z);
        return isTransparentForPreview(level.getBlockState(p));
    }

    private static int traceColor(
        ServerLevel level,
        double ox, double oy, double oz,
        double dx, double dy, double dz,
        int px, int py
    ) {
        BlockPos.MutableBlockPos p = new BlockPos.MutableBlockPos();

        for (int step = 2; step < RAY_STEPS; step++) {
            double distance = step * RAY_STEP;
            int x = (int) Math.floor(ox + dx * distance);
            int y = (int) Math.floor(oy + dy * distance);
            int z = (int) Math.floor(oz + dz * distance);

            p.set(x, y, z);
            if (!level.hasChunkAt(p)) break;

            BlockState state = level.getBlockState(p);
            if (isTransparentForPreview(state)) continue;

            int color = colorFor(state, level.dimension());
            return shade(color, distance, dy);
        }

        return backgroundColor(level.dimension(), px, py, dy);
    }

    private static boolean isTransparentForPreview(BlockState state) {
        return state.isAir()
            || state.is(Blocks.NETHER_PORTAL)
            || state.is(Blocks.END_PORTAL)
            || state.is(Blocks.FIRE)
            || state.is(Blocks.SOUL_FIRE);
    }

    private static int colorFor(BlockState state, ResourceKey<Level> dimension) {
        if (state.is(Blocks.LAVA)) return 0xFFFF5A08;
        if (state.is(Blocks.GLOWSTONE)) return 0xFFFFD86A;
        if (state.is(Blocks.SHROOMLIGHT)) return 0xFFFFB45E;
        if (state.is(Blocks.MAGMA_BLOCK)) return 0xFFB93A13;
        if (state.is(Blocks.NETHERRACK)) return 0xFF7A2420;
        if (state.is(Blocks.NETHER_BRICKS) || state.is(Blocks.RED_NETHER_BRICKS)) return 0xFF351016;
        if (state.is(Blocks.BLACKSTONE) || state.is(Blocks.BASALT) || state.is(Blocks.POLISHED_BASALT)) return 0xFF29262D;
        if (state.is(Blocks.SOUL_SAND) || state.is(Blocks.SOUL_SOIL)) return 0xFF554235;
        if (state.is(Blocks.CRIMSON_NYLIUM)) return 0xFF8B1732;
        if (state.is(Blocks.WARPED_NYLIUM)) return 0xFF15665F;
        if (state.is(Blocks.END_STONE)) return 0xFFD9D99A;
        if (state.is(Blocks.OBSIDIAN) || state.is(Blocks.CRYING_OBSIDIAN)) return 0xFF241333;
        if (state.is(Blocks.WATER)) return 0xFF2859C7;
        if (state.is(Blocks.GRASS_BLOCK)) return 0xFF5B8E35;
        if (state.is(Blocks.DIRT) || state.is(Blocks.COARSE_DIRT)) return 0xFF765339;
        if (state.is(Blocks.STONE) || state.is(Blocks.COBBLESTONE) || state.is(Blocks.DEEPSLATE)) return 0xFF777777;
        if (state.is(Blocks.SNOW_BLOCK) || state.is(Blocks.SNOW)) return 0xFFEAF5FF;
        if (state.is(Blocks.SAND)) return 0xFFD7C47B;
        if (state.is(Blocks.GRAVEL)) return 0xFF77716C;
        if (state.is(Blocks.OAK_LEAVES) || state.is(Blocks.SPRUCE_LEAVES) || state.is(Blocks.BIRCH_LEAVES)) return 0xFF3E7D35;

        if (Level.NETHER.equals(dimension)) return 0xFF63302B;
        if (Level.END.equals(dimension)) return 0xFFAAA873;
        return 0xFF7E7E7E;
    }

    private static int shade(int color, double distance, double rayY) {
        double fog = Math.max(0.28, 1.0 - distance / 70.0);
        double face = 0.88 + Math.max(-0.12, Math.min(0.12, -rayY * 0.16));
        double factor = fog * face;

        int r = clamp((int) (((color >> 16) & 0xFF) * factor), 0, 255);
        int g = clamp((int) (((color >> 8) & 0xFF) * factor), 0, 255);
        int b = clamp((int) ((color & 0xFF) * factor), 0, 255);
        return 0xFF000000 | (r << 16) | (g << 8) | b;
    }

    private static int backgroundColor(ResourceKey<Level> dimension, int px, int py, double rayY) {
        int noise = (px * 31 + py * 17) & 7;

        if (Level.NETHER.equals(dimension)) {
            int r = 34 + noise * 2 + (rayY > 0.25 ? 6 : 0);
            return 0xFF000000 | (r << 16) | (5 << 8) | 4;
        }

        if (Level.END.equals(dimension)) {
            boolean star = ((px * 13 + py * 29) % 47) == 0;
            return star ? 0xFFE3E0FF : 0xFF08070D;
        }

        int r = 76 + noise;
        int g = 132 + noise;
        int b = 196 + noise;
        return 0xFF000000 | (r << 16) | (g << 8) | b;
    }

    private static List<PortalTile> buildFallback(List<PortalGroup> groups) {
        ArrayList<PortalTile> out = new ArrayList<>();
        for (PortalGroup group : groups) appendFallback(out, group);
        return List.copyOf(out);
    }

    private static void appendFallback(List<PortalTile> out, PortalGroup group) {
        int pixelWidth = Math.max(1, group.widthBlocks() * 4);
        int pixelHeight = Math.max(1, group.heightBlocks() * 4);

        for (int py = 0; py < pixelHeight; py++) {
            for (int px = 0; px < pixelWidth; px++) {
                int color;
                if (group.kind() == PortalState.Kind.END) {
                    color = ((px * 11 + py * 23) % 29 == 0) ? 0xFFDCD8FF : 0xFF090611;
                } else {
                    int pulse = ((px + py) & 1) == 0 ? 0x42 : 0x32;
                    color = 0xFF000000 | (pulse << 16) | (8 << 8) | 0x5A;
                }
                out.add(new PortalTile(pixelBounds(group, px, py, pixelWidth, pixelHeight), color));
            }
        }
    }

    private static AABB pixelBounds(
        PortalGroup group,
        int px, int py,
        int pixelWidth, int pixelHeight
    ) {
        double u0 = (double) px / pixelWidth;
        double u1 = (double) (px + 1) / pixelWidth;
        double top0 = (double) py / pixelHeight;
        double top1 = (double) (py + 1) / pixelHeight;

        if (group.plane() == Plane.X) {
            double x0 = group.minX() + u0 * group.widthBlocks();
            double x1 = group.minX() + u1 * group.widthBlocks();
            double y1 = group.maxY() + 1.0 - top0 * group.heightBlocks();
            double y0 = group.maxY() + 1.0 - top1 * group.heightBlocks();
            double z = group.minZ() + 0.5;
            return new AABB(x0, y0, z - HALF_THICKNESS, x1, y1, z + HALF_THICKNESS);
        }

        if (group.plane() == Plane.Z) {
            double z0 = group.minZ() + u0 * group.widthBlocks();
            double z1 = group.minZ() + u1 * group.widthBlocks();
            double y1 = group.maxY() + 1.0 - top0 * group.heightBlocks();
            double y0 = group.maxY() + 1.0 - top1 * group.heightBlocks();
            double x = group.minX() + 0.5;
            return new AABB(x - HALF_THICKNESS, y0, z0, x + HALF_THICKNESS, y1, z1);
        }

        double x0 = group.minX() + u0 * group.widthBlocks();
        double x1 = group.minX() + u1 * group.widthBlocks();
        double z0 = group.minZ() + top0 * group.heightBlocks();
        double z1 = group.minZ() + top1 * group.heightBlocks();
        double y = group.minY() + 0.75;
        return new AABB(x0, y - HALF_THICKNESS, z0, x1, y + HALF_THICKNESS, z1);
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
        for (PortalTile tile : frameTiles) {
            drawFilledBox(pose, buffer, tile.bounds(), tile.color());
        }
    }

    private static void drawFilledBox(PoseStack.Pose pose, VertexConsumer v, AABB b, int c) {
        v.addVertex(pose,(float)b.minX,(float)b.minY,(float)b.minZ).setColor(c);
        v.addVertex(pose,(float)b.maxX,(float)b.minY,(float)b.minZ).setColor(c);
        v.addVertex(pose,(float)b.maxX,(float)b.maxY,(float)b.minZ).setColor(c);
        v.addVertex(pose,(float)b.minX,(float)b.maxY,(float)b.minZ).setColor(c);

        v.addVertex(pose,(float)b.maxX,(float)b.minY,(float)b.maxZ).setColor(c);
        v.addVertex(pose,(float)b.minX,(float)b.minY,(float)b.maxZ).setColor(c);
        v.addVertex(pose,(float)b.minX,(float)b.maxY,(float)b.maxZ).setColor(c);
        v.addVertex(pose,(float)b.maxX,(float)b.maxY,(float)b.maxZ).setColor(c);

        v.addVertex(pose,(float)b.minX,(float)b.minY,(float)b.maxZ).setColor(c);
        v.addVertex(pose,(float)b.minX,(float)b.minY,(float)b.minZ).setColor(c);
        v.addVertex(pose,(float)b.minX,(float)b.maxY,(float)b.minZ).setColor(c);
        v.addVertex(pose,(float)b.minX,(float)b.maxY,(float)b.maxZ).setColor(c);

        v.addVertex(pose,(float)b.maxX,(float)b.minY,(float)b.minZ).setColor(c);
        v.addVertex(pose,(float)b.maxX,(float)b.minY,(float)b.maxZ).setColor(c);
        v.addVertex(pose,(float)b.maxX,(float)b.maxY,(float)b.maxZ).setColor(c);
        v.addVertex(pose,(float)b.maxX,(float)b.maxY,(float)b.minZ).setColor(c);

        v.addVertex(pose,(float)b.minX,(float)b.maxY,(float)b.minZ).setColor(c);
        v.addVertex(pose,(float)b.maxX,(float)b.maxY,(float)b.minZ).setColor(c);
        v.addVertex(pose,(float)b.maxX,(float)b.maxY,(float)b.maxZ).setColor(c);
        v.addVertex(pose,(float)b.minX,(float)b.maxY,(float)b.maxZ).setColor(c);

        v.addVertex(pose,(float)b.minX,(float)b.minY,(float)b.maxZ).setColor(c);
        v.addVertex(pose,(float)b.maxX,(float)b.minY,(float)b.maxZ).setColor(c);
        v.addVertex(pose,(float)b.maxX,(float)b.minY,(float)b.minZ).setColor(c);
        v.addVertex(pose,(float)b.minX,(float)b.minY,(float)b.minZ).setColor(c);
    }

    private static int clamp(int value, int min, int max) {
        return Math.max(min, Math.min(max, value));
    }
}
