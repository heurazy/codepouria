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
import net.minecraft.client.multiplayer.ClientLevel;
import net.minecraft.client.renderer.rendertype.RenderTypes;
import net.minecraft.core.BlockPos;
import net.minecraft.util.ARGB;
import net.minecraft.world.level.block.Blocks;
import net.minecraft.world.phys.AABB;
import net.minecraft.world.phys.Vec3;

public final class PortalSurfaceRenderer {
    private enum Plane { X, Z, HORIZONTAL }
    private record PortalCell(int x, int y, int z, Plane plane, PortalState.Kind kind, float phase) { }
    private static final int SCAN_RADIUS = 10;
    private static final int MAX_CELLS = 384;
    private static final double INSET = 0.035;
    private static final double HALF_THICKNESS = 0.018;
    private static List<PortalCell> cells = List.of();
    private PortalSurfaceRenderer() { }

    public static void register() {
        LevelExtractionEvents.END_EXTRACTION.register(PortalSurfaceRenderer::extract);
        LevelRenderEvents.BEFORE_TRANSLUCENT_TERRAIN.register(PortalSurfaceRenderer::render);
    }

    private static void extract(LevelExtractionContext context) {
        ClientLevel level = context.level();
        Vec3 camera = context.levelState().cameraRenderState.pos;
        BlockPos center = BlockPos.containing(camera.x, camera.y, camera.z);
        BlockPos.MutableBlockPos pos = new BlockPos.MutableBlockPos();
        ArrayList<PortalCell> next = new ArrayList<>();
        long gameTime = level.getGameTime();
        outer:
        for (int y = -SCAN_RADIUS; y <= SCAN_RADIUS; y++) for (int x = -SCAN_RADIUS; x <= SCAN_RADIUS; x++) for (int z = -SCAN_RADIUS; z <= SCAN_RADIUS; z++) {
            pos.set(center.getX() + x, center.getY() + y, center.getZ() + z);
            var state = level.getBlockState(pos);
            PortalState.Kind kind;
            Plane plane;
            if (state.is(Blocks.NETHER_PORTAL)) {
                kind = PortalState.Kind.NETHER;
                boolean eastWest = level.getBlockState(pos.east()).is(Blocks.NETHER_PORTAL) || level.getBlockState(pos.west()).is(Blocks.NETHER_PORTAL);
                plane = eastWest ? Plane.X : Plane.Z;
            } else if (state.is(Blocks.END_PORTAL)) {
                kind = PortalState.Kind.END;
                plane = Plane.HORIZONTAL;
            } else continue;
            float phase = (float) ((gameTime * 0.045 + pos.getX() * 0.17 + pos.getY() * 0.11 + pos.getZ() * 0.13) % 1.0);
            next.add(new PortalCell(pos.getX(), pos.getY(), pos.getZ(), plane, kind, phase));
            if (next.size() >= MAX_CELLS) break outer;
        }
        cells = List.copyOf(next);
    }

    private static void render(LevelRenderContext context) {
        List<PortalCell> frameCells = cells;
        if (frameCells.isEmpty()) return;
        Vec3 camera = context.levelState().cameraRenderState.pos;
        PoseStack poseStack = context.poseStack();
        poseStack.pushPose();
        poseStack.translate(-camera.x, -camera.y, -camera.z);
        context.submitNodeCollector().submitCustomGeometry(poseStack, RenderTypes.debugFilledBox(), (pose, buffer) -> drawPortalCells(pose, buffer, frameCells));
        poseStack.popPose();
    }

    private static void drawPortalCells(PoseStack.Pose pose, VertexConsumer buffer, List<PortalCell> frameCells) {
        for (PortalCell cell : frameCells) {
            float pulse = 0.72f + 0.28f * (float) Math.sin(cell.phase * Math.PI * 2.0);
            int color = cell.kind == PortalState.Kind.END ? ARGB.colorFromFloat(0.78f, 0.055f, 0.045f, 0.085f) : ARGB.colorFromFloat(0.48f, 0.34f * pulse, 0.055f, 0.62f * pulse);
            drawFilledBox(pose, buffer, portalBounds(cell), color);
        }
    }

    private static AABB portalBounds(PortalCell cell) {
        double x = cell.x, y = cell.y, z = cell.z;
        return switch (cell.plane) {
            case X -> new AABB(x + INSET, y + INSET, z + 0.5 - HALF_THICKNESS, x + 1.0 - INSET, y + 1.0 - INSET, z + 0.5 + HALF_THICKNESS);
            case Z -> new AABB(x + 0.5 - HALF_THICKNESS, y + INSET, z + INSET, x + 0.5 + HALF_THICKNESS, y + 1.0 - INSET, z + 1.0 - INSET);
            case HORIZONTAL -> new AABB(x + INSET, y + 0.75 - HALF_THICKNESS, z + INSET, x + 1.0 - INSET, y + 0.75 + HALF_THICKNESS, z + 1.0 - INSET);
        };
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
