using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x0200010E RID: 270
public class ReadyUpToTrigger : MonoBehaviourPunCallbacks
{
	// Token: 0x060007E7 RID: 2023 RVA: 0x00029E19 File Offset: 0x00028019
	public override void OnJoinedRoom()
	{
		this.readyUpStatusDict.Clear();
		this.PopulatePlayerDict();
	}

	// Token: 0x060007E8 RID: 2024 RVA: 0x00029E2C File Offset: 0x0002802C
	public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
	{
		this.PopulatePlayerDict();
	}

	// Token: 0x060007E9 RID: 2025 RVA: 0x00029E34 File Offset: 0x00028034
	public override void OnPlayerLeftRoom(Photon.Realtime.Player leavingPlayer)
	{
		this.readyUpStatusDict.Remove(leavingPlayer);
		Debug.Log("Removing player from ready-up list: " + leavingPlayer.NickName);
	}

	// Token: 0x060007EA RID: 2026 RVA: 0x00029E58 File Offset: 0x00028058
	private void PopulatePlayerDict()
	{
		foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList)
		{
			if (!this.readyUpStatusDict.ContainsKey(player))
			{
				this.readyUpStatusDict.Add(player, false);
				Debug.Log("Adding player to ready-up list: " + player.NickName);
			}
		}
	}

	// Token: 0x04000765 RID: 1893
	public Dictionary<Photon.Realtime.Player, bool> readyUpStatusDict = new Dictionary<Photon.Realtime.Player, bool>();
}
