using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;
using Zorro.Core;

// Token: 0x0200008E RID: 142
public class GameOverHandler : Singleton<GameOverHandler>
{
	// Token: 0x060004E8 RID: 1256 RVA: 0x0001C5E0 File Offset: 0x0001A7E0
	protected override void Awake()
	{
		base.Awake();
		this.view = base.GetComponent<PhotonView>();
	}

	// Token: 0x060004E9 RID: 1257 RVA: 0x0001C5F4 File Offset: 0x0001A7F4
	public void LocalPlayerHasClosedEndScreen()
	{
		this.view.RPC("PlayerHasClosedEndScreen", RpcTarget.All, new object[] { PhotonNetwork.LocalPlayer.ActorNumber });
	}

	// Token: 0x060004EA RID: 1258 RVA: 0x0001C620 File Offset: 0x0001A820
	[PunRPC]
	public void PlayerHasClosedEndScreen(int actorNumber)
	{
		Player player;
		if (!PlayerHandler.TryGetPlayer(actorNumber, out player))
		{
			Debug.LogError(string.Format("Player not found: {0}", actorNumber));
			return;
		}
		player.hasClosedEndScreen = true;
		Debug.Log(string.Format("{0} Player has closed end screen", player));
	}

	// Token: 0x060004EB RID: 1259 RVA: 0x0001C664 File Offset: 0x0001A864
	public void LoadAirport()
	{
		this.view.RPC("LoadAirportMaster", RpcTarget.MasterClient, Array.Empty<object>());
	}

	// Token: 0x060004EC RID: 1260 RVA: 0x0001C67C File Offset: 0x0001A87C
	[PunRPC]
	public void LoadAirportMaster()
	{
		this.view.RPC("BeginIslandLoadRPC", RpcTarget.All, Array.Empty<object>());
	}

	// Token: 0x060004ED RID: 1261 RVA: 0x0001C694 File Offset: 0x0001A894
	[PunRPC]
	public void BeginIslandLoadRPC()
	{
		Debug.Log("Load Island RPC..");
		SceneSwitchingStatus sceneSwitchingStatus;
		if (GameHandler.TryGetStatus<SceneSwitchingStatus>(out sceneSwitchingStatus))
		{
			Debug.Log("Already loading... ");
			return;
		}
		GameHandler.AddStatus<SceneSwitchingStatus>(new SceneSwitchingStatus());
		RetrievableResourceSingleton<LoadingScreenHandler>.Instance.Load(LoadingScreen.LoadingScreenType.Basic, null, new IEnumerator[] { RetrievableResourceSingleton<LoadingScreenHandler>.Instance.LoadSceneProcess("Airport", true, true, 0f) });
	}

	// Token: 0x060004EE RID: 1262 RVA: 0x0001C6F4 File Offset: 0x0001A8F4
	public void ForceEveryPlayerDoneWithEndScreen()
	{
		this.view.RPC("ForceEveryPlayerDoneWithEndScreenRPC", RpcTarget.All, Array.Empty<object>());
	}

	// Token: 0x060004EF RID: 1263 RVA: 0x0001C70C File Offset: 0x0001A90C
	[PunRPC]
	public void ForceEveryPlayerDoneWithEndScreenRPC()
	{
		Debug.Log("Force every player closed end screen");
		foreach (Player player in PlayerHandler.GetAllPlayers())
		{
			player.hasClosedEndScreen = true;
		}
	}

	// Token: 0x04000526 RID: 1318
	private PhotonView view;
}
