package dev.immersiveportalsclient;

import dev.immersiveportalsclient.render.PortalHud;
import dev.immersiveportalsclient.render.PortalRenderGuard;
import dev.immersiveportalsclient.render.PortalSurfaceRenderer;
import dev.immersiveportalsclient.state.PortalState;
import net.fabricmc.api.ClientModInitializer;
import net.fabricmc.fabric.api.client.event.lifecycle.v1.ClientLevelEvents;
import net.fabricmc.fabric.api.client.event.lifecycle.v1.ClientTickEvents;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

public final class ImmersivePortalsClient implements ClientModInitializer {
    public static final String MOD_ID = "immersive_portals_client";
    public static final Logger LOGGER = LoggerFactory.getLogger(MOD_ID);

    @Override
    public void onInitializeClient() {
        ClientTickEvents.END_CLIENT_TICK.register(PortalState::tick);
        ClientLevelEvents.AFTER_CLIENT_LEVEL_CHANGE.register((client, level) -> PortalState.onLevelChanged(level.dimension()));
        PortalHud.register();
        PortalRenderGuard.register();
        PortalSurfaceRenderer.register();
        LOGGER.info("Immersive Portals Client 26.2 initialized (client-only, Nether/End only)");
    }
}
