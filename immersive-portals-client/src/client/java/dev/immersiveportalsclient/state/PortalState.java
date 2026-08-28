package dev.immersiveportalsclient.state;

import net.minecraft.client.Minecraft;
import net.minecraft.resources.ResourceKey;
import net.minecraft.world.level.Level;
import net.minecraft.world.level.block.Blocks;
import net.minecraft.core.BlockPos;

public final class PortalState {
    public enum Kind { NONE, NETHER, END }
    private static Kind nearbyKind = Kind.NONE;
    private static float immersion;
    private static int levelChangeFlashTicks;
    private static ResourceKey<Level> dimension;
    private PortalState() { }

    public static void tick(Minecraft client) {
        if (client.player == null || client.level == null) {
            nearbyKind = Kind.NONE;
            immersion = Math.max(0.0f, immersion - 0.12f);
            levelChangeFlashTicks = 0;
            return;
        }
        dimension = client.level.dimension();
        nearbyKind = detectPortal(client);
        float target = nearbyKind == Kind.NONE ? 0.0f : 1.0f;
        float speed = target > immersion ? 0.16f : 0.08f;
        immersion += (target - immersion) * speed;
        if (levelChangeFlashTicks > 0) levelChangeFlashTicks--;
    }

    public static void onLevelChanged(ResourceKey<Level> newDimension) {
        dimension = newDimension;
        levelChangeFlashTicks = 12;
        immersion = Math.max(immersion, 0.65f);
    }

    private static Kind detectPortal(Minecraft client) {
        BlockPos origin = client.player.blockPosition();
        BlockPos.MutableBlockPos cursor = new BlockPos.MutableBlockPos();
        for (int y = -1; y <= 2; y++) for (int x = -1; x <= 1; x++) for (int z = -1; z <= 1; z++) {
            cursor.set(origin.getX() + x, origin.getY() + y, origin.getZ() + z);
            var state = client.level.getBlockState(cursor);
            if (state.is(Blocks.NETHER_PORTAL)) return Kind.NETHER;
            if (state.is(Blocks.END_PORTAL)) return Kind.END;
        }
        return Kind.NONE;
    }

    public static Kind nearbyKind() { return nearbyKind; }
    public static float visualStrength() {
        float flash = levelChangeFlashTicks / 12.0f;
        return Math.min(1.0f, Math.max(immersion, flash));
    }
    public static boolean isTransitioning() { return levelChangeFlashTicks > 0; }
    public static ResourceKey<Level> currentDimension() { return dimension; }
    public static ResourceKey<Level> visualTargetDimension(Kind kind) {
        if (kind == Kind.END) return Level.END.equals(dimension) ? Level.OVERWORLD : Level.END;
        if (kind == Kind.NETHER) return Level.NETHER.equals(dimension) ? Level.OVERWORLD : Level.NETHER;
        return dimension;
    }
}
