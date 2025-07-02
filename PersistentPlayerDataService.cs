using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Zorro.PhotonUtility;

// Token: 0x0200006B RID: 107
public class PersistentPlayerDataService : GameService<PersistentPlayerDataService>, IDisposable
{
	// Token: 0x060003F3 RID: 1011 RVA: 0x00016EFC File Offset: 0x000150FC
	public PersistentPlayerDataService()
	{
		this.syncPersistentPlayerDataHandle = CustomCommands<CustomCommandType>.RegisterListener<SyncPersistentPlayerDataPackage>(new Action<SyncPersistentPlayerDataPackage>(this.OnSyncReceived));
	}

	// Token: 0x060003F4 RID: 1012 RVA: 0x00016F31 File Offset: 0x00015131
	public void Dispose()
	{
		CustomCommands<CustomCommandType>.UnregisterListener(this.syncPersistentPlayerDataHandle);
	}

	// Token: 0x060003F5 RID: 1013 RVA: 0x00016F40 File Offset: 0x00015140
	private void OnSyncReceived(SyncPersistentPlayerDataPackage package)
	{
		Debug.Log("On Sync Received!");
		this.PersistentPlayerDatas[package.ActorNumber] = package.Data;
		if (this.OnChangeActions.ContainsKey(package.ActorNumber))
		{
			this.OnChangeActions[package.ActorNumber](package.Data);
		}
	}

	// Token: 0x060003F6 RID: 1014 RVA: 0x00016F9D File Offset: 0x0001519D
	public PersistentPlayerData GetPlayerData(Photon.Realtime.Player player)
	{
		return this.GetPlayerData(player.ActorNumber);
	}

	// Token: 0x060003F7 RID: 1015 RVA: 0x00016FAC File Offset: 0x000151AC
	public PersistentPlayerData GetPlayerData(int actorNumber)
	{
		if (!this.PersistentPlayerDatas.ContainsKey(actorNumber))
		{
			this.PersistentPlayerDatas[actorNumber] = new PersistentPlayerData();
			Debug.Log(string.Format("Initializing player data for player: {0}", actorNumber));
		}
		return this.PersistentPlayerDatas[actorNumber];
	}

	// Token: 0x060003F8 RID: 1016 RVA: 0x00016FFC File Offset: 0x000151FC
	public void SetPlayerData(Photon.Realtime.Player player, PersistentPlayerData playerData)
	{
		this.PersistentPlayerDatas[player.ActorNumber] = playerData;
		Debug.Log("Setting Player Data for: " + player.NickName);
		if (this.OnChangeActions.ContainsKey(player.ActorNumber))
		{
			Action<PersistentPlayerData> action = this.OnChangeActions[player.ActorNumber];
			if (action != null)
			{
				action(playerData);
			}
		}
		CustomCommands<CustomCommandType>.SendPackage(new SyncPersistentPlayerDataPackage
		{
			Data = playerData,
			ActorNumber = player.ActorNumber
		}, ReceiverGroup.Others);
	}

	// Token: 0x060003F9 RID: 1017 RVA: 0x00017080 File Offset: 0x00015280
	public void SubscribeToPlayerDataChange(Photon.Realtime.Player player, Action<PersistentPlayerData> onChange)
	{
		if (!this.OnChangeActions.ContainsKey(player.ActorNumber))
		{
			this.OnChangeActions[player.ActorNumber] = onChange;
			return;
		}
		Dictionary<int, Action<PersistentPlayerData>> onChangeActions = this.OnChangeActions;
		int actorNumber = player.ActorNumber;
		onChangeActions[actorNumber] = (Action<PersistentPlayerData>)Delegate.Combine(onChangeActions[actorNumber], onChange);
	}

	// Token: 0x060003FA RID: 1018 RVA: 0x000170DC File Offset: 0x000152DC
	public void UnsubscribeToPlayerDataChange(Photon.Realtime.Player player, Action<PersistentPlayerData> onChange)
	{
		if (this.OnChangeActions.ContainsKey(player.ActorNumber))
		{
			Dictionary<int, Action<PersistentPlayerData>> onChangeActions = this.OnChangeActions;
			int actorNumber = player.ActorNumber;
			onChangeActions[actorNumber] = (Action<PersistentPlayerData>)Delegate.Remove(onChangeActions[actorNumber], onChange);
		}
	}

	// Token: 0x060003FB RID: 1019 RVA: 0x00017124 File Offset: 0x00015324
	public void SyncToPlayer(Photon.Realtime.Player newPlayer)
	{
		foreach (KeyValuePair<int, PersistentPlayerData> keyValuePair in this.PersistentPlayerDatas)
		{
			int num;
			PersistentPlayerData persistentPlayerData;
			keyValuePair.Deconstruct(out num, out persistentPlayerData);
			int num2 = num;
			PersistentPlayerData persistentPlayerData2 = persistentPlayerData;
			Photon.Realtime.Player player;
			if (PhotonNetwork.TryGetPlayer(num2, out player) && !player.IsInactive)
			{
				RaiseEventOptions raiseEventOptions = new RaiseEventOptions
				{
					TargetActors = new int[] { newPlayer.ActorNumber }
				};
				CustomCommands<CustomCommandType>.SendPackage(new SyncPersistentPlayerDataPackage
				{
					Data = persistentPlayerData2,
					ActorNumber = num2
				}, raiseEventOptions);
			}
		}
	}

	// Token: 0x0400044D RID: 1101
	private Dictionary<int, PersistentPlayerData> PersistentPlayerDatas = new Dictionary<int, PersistentPlayerData>();

	// Token: 0x0400044E RID: 1102
	private Dictionary<int, Action<PersistentPlayerData>> OnChangeActions = new Dictionary<int, Action<PersistentPlayerData>>();

	// Token: 0x0400044F RID: 1103
	private ListenerHandle syncPersistentPlayerDataHandle;
}
