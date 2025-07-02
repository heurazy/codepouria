using System;
using Photon.Pun;
using Photon.Realtime;
using Zorro.Core;
using Zorro.PhotonUtility;

// Token: 0x020000F1 RID: 241
public class MapReconnectHandler : MonoBehaviourPunCallbacks
{
	// Token: 0x06000738 RID: 1848 RVA: 0x00026358 File Offset: 0x00024558
	public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
	{
		base.OnPlayerEnteredRoom(newPlayer);
		if (!newPlayer.IsLocal && PhotonNetwork.IsMasterClient)
		{
			RaiseEventOptions @default = RaiseEventOptions.Default;
			@default.TargetActors = new int[] { newPlayer.ActorNumber };
			CustomCommands<CustomCommandType>.SendPackage(new SyncMapHandlerDebugCommandPackage(Singleton<MapHandler>.Instance.GetCurrentSegment(), new int[] { newPlayer.ActorNumber }), @default);
		}
	}
}
