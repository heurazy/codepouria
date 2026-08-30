package dev.immersiveportalsclient.render;

import java.lang.reflect.Field;
import java.util.ArrayList;
import java.util.List;

import com.mojang.blaze3d.vertex.PoseStack;
import com.mojang.blaze3d.vertex.VertexConsumer;
import net.fabricmc.fabric.api.client.rendering.v1.level.LevelExtractionContext;
import net.fabricmc.fabric.api.client.rendering.v1.level.LevelExtractionEvents;
import net.fabricmc.fabric.api.client.rendering.v1.level.LevelRenderContext;
import net.fabricmc.fabric.api.client.rendering.v1.level.LevelRenderEvents;
import net.minecraft.client.multiplayer.ClientLevel;
import net.minecraft.client.renderer.rendertype.RenderTypes;
import net.minecraft.core.BlockPos;
import net.minecraft.world.level.block.Blocks;
import net.minecraft.world.phys.AABB;
import net.minecraft.world.phys.Vec3;

/**
 * Safety layer for the client-side portal preview.
 *
 * It fixes the alpha.4 scheduler bootstrap and keeps a dark portal surface
 * behind the generated preview so hiding the vanilla texture can never leave
 * a literal transparent hole in the world.
 */
public final class PortalRenderGuard {
    private enum Plane { X, Z, HORIZONTAL }
    private record Backdrop(AABB bounds, int color) { }

    private static final int SCAN_RADIUS = 12;
    private static final double BACK_OFFSET = 0.060;
    private static final double HALF_THICKNESS = 0.010;
    private static volatile List<Backdrop> backdrops = List.of();

    private PortalRenderGuard() { }

    public static void register() {
        primePreviewScheduler();
        LevelExtractionEvents.END_EXTRACTION.register(PortalRenderGuard::extract);
        LevelRenderEvents.BEFORE_TRANSLUCENT_TERRAIN.register(PortalRenderGuard::render);
    }

    private static void primePreviewScheduler() {
        try {
            Field field = PortalSurfaceRenderer.class.getDeclaredField("lastScheduledTick");
            field.setAccessible(true);
            if (field.getLong(null) == Long.MIN_VALUE) {
                field.setLong(null, -8L);
            }
        } catch (ReflectiveOperationException ignored) {
            // If the implementation changes, the backdrop still prevents a hole.
        }
    }

    private static void extract(LevelExtractionContext context) {
        ClientLevel level = context.level();
        Vec3 camera = context.levelState().cameraRenderState.pos;
        BlockPos center = BlockPos.containing(camera.x, camera.y, camera.z);
        BlockPos.MutableBlockPos pos = new BlockPos.MutableBlockPos();
        ArrayList<Backdrop> next = new ArrayList<>();

        for (int y = -SCAN_RADIUS; y <= SCAN_RADIUS; y++) {
            for (int x = -SCAN_RADIUS; x <= SCAN_RADIUS; x++) {
                for (int z = -SCAN_RADIUS; z <= SCAN_RADIUS; z++) {
                    pos.set(center.getX() + x, center.getY() + y, center.getZ() + z);
                    var state = level.getBlockState(pos);

                    if (state.is(Blocks.NETHER_PORTAL)) {
                        boolean eastWest = level.getBlockState(pos.east()).is(Blocks.NETHER_PORTAL)
                            || level.getBlockState(pos.west()).is(Blocks.NETHER_PORTAL);
                        Plane plane = eastWest ? Plane.X : Plane.Z;
                        next.add(new Backdrop(bounds(pos, plane, camera), 0xFF190B2B));
                    } else if (state.is(Blocks.END_PORTAL)) {
                        next.add(new Backdrop(bounds(pos, Plane.HORIZONTAL, camera), 0xFF07060D));
                    }
                }
            }
        }

        backdrops = List.copyOf(next);
    }

    private static AABB bounds(BlockPos pos, Plane plane, Vec3 camera) {
        double x = pos.getX();
        double y = pos.getY();
        double z = pos.getZ();

        return switch (plane) {
            case X -> {
                double centerZ = z + 0.5;
                double offset = camera.z < centerZ ? BACK_OFFSET : -BACK_OFFSET;
                double planeZ = centerZ + offset;
                yield new AABB(x, y, planeZ - HALF_THICKNESS, x + 1.0, y + 1.0, planeZ + HALF_THICKNESS);
            }
            case Z -> {
                double centerX = x + 0.5;
                double offset = camera.x < centerX ? BACK_OFFSET : -BACK_OFFSET;
                double planeX = centerX + offset;
                yield new AABB(planeX - HALF_THICKNESS, y, z, planeX + HALF_THICKNESS, y + 1.0, z + 1.0);
            }
            case HORIZONTAL -> {
                double centerY = y + 0.75;
                double offset = camera.y > centerY ? -BACK_OFFSET : BACK_OFFSET;
                double planeY = centerY + offset;
                yield new AABB(x, planeY - HALF_THICKNESS, z, x + 1.0, planeY + HALF_THICKNESS, z + 1.0);
            }
        };
    }

    private static void render(LevelRenderContext context) {
        List<Backdrop> frame = backdrops;
        if (frame.isEmpty()) return;

        Vec3 camera = context.levelState().cameraRenderState.pos;
        PoseStack poseStack = context.poseStack();
        poseStack.pushPose();
        poseStack.translate(-camera.x, -camera.y, -camera.z);
        context.submitNodeCollector().submitCustomGeometry(
            poseStack,
            RenderTypes.debugFilledBox(),
            (pose, buffer) -> draw(pose, buffer, frame)
        );
        poseStack.popPose();
    }

    private static void draw(PoseStack.Pose pose, VertexConsumer v, List<Backdrop> frame) {
        for (Backdrop backdrop : frame) {
            AABB b = backdrop.bounds();
            int c = backdrop.color();

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
    }
}
