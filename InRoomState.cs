using System;
using UnityEngine;
using Zorro.PhotonUtility;

// Token: 0x02000067 RID: 103
public class InRoomState : ConnectionState
{
	// Token: 0x060003EC RID: 1004 RVA: 0x00016E60 File Offset: 0x00015060
	public override void Enter()
	{
		base.Enter();
		this.verifiedLobby = null;
		this.hasLoadedCustomization = false;
		GameHandler.RestartService<PersistentPlayerDataService>(new PersistentPlayerDataService());
		Debug.Log("Restarting PersistentPlayerDataService");
		CommandListener commandListener = CustomCommands<CustomCommandType>.SpawnCommandListener<CommandListener>();
		commandListener.RegisterPackage<SyncPersistentPlayerDataPackage>(new SyncPersistentPlayerDataPackage());
		commandListener.RegisterPackage<SyncMapHandlerDebugCommandPackage>(new SyncMapHandlerDebugCommandPackage());
		GameHandler.ClearAllStatuses();
	}

	// Token: 0x04000448 RID: 1096
	public bool hasLoadedCustomization;

	// Token: 0x04000449 RID: 1097
	public string verifiedLobby;
}
