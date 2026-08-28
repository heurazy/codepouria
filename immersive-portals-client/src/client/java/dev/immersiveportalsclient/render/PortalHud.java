package dev.immersiveportalsclient.render;

import dev.immersiveportalsclient.ImmersivePortalsClient;
import dev.immersiveportalsclient.state.PortalState;
import net.minecraft.resources.Identifier;
import net.minecraft.util.Mth;
import net.fabricmc.fabric.api.client.rendering.v1.hud.HudElementRegistry;

public final class PortalHud {
    private PortalHud() { }
    public static void register() {
        HudElementRegistry.addLast(
            Identifier.fromNamespaceAndPath(ImmersivePortalsClient.MOD_ID, "portal_transition"),
            (graphics, deltaTracker) -> {
                float strength = PortalState.visualStrength();
                if (strength < 0.015f) return;
                int width = graphics.guiWidth();
                int height = graphics.guiHeight();
                int alpha = Mth.clamp((int) (strength * (PortalState.isTransitioning() ? 145 : 72)), 0, 180);
                PortalState.Kind kind = PortalState.nearbyKind();
                int rgb = kind == PortalState.Kind.END ? 0x15131F : 0x35105A;
                int transparent = (Math.max(0, alpha / 4) << 24) | rgb;
                int opaqueEdge = (alpha << 24) | rgb;
                int edge = Math.max(8, Math.min(width, height) / 10);
                graphics.fillGradient(0, 0, width, edge, opaqueEdge, transparent);
                graphics.fillGradient(0, height - edge, width, height, transparent, opaqueEdge);
                graphics.fill(0, edge, edge, height - edge, transparent);
                graphics.fill(width - edge, edge, width, height - edge, transparent);
            }
        );
    }
}
