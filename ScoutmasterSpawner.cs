using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x020000FF RID: 255
public class ScoutmasterSpawner : MonoBehaviourPunCallbacks
{
	// Token: 0x06000791 RID: 1937 RVA: 0x000285C6 File Offset: 0x000267C6
	private void Awake()
	{
		if (PhotonNetwork.InRoom)
		{
			this.SpawnScoutmaster();
		}
	}

	// Token: 0x06000792 RID: 1938 RVA: 0x000285D5 File Offset: 0x000267D5
	public override void OnJoinedRoom()
	{
		this.SpawnScoutmaster();
	}

	// Token: 0x06000793 RID: 1939 RVA: 0x000285E0 File Offset: 0x000267E0
	private void SpawnScoutmaster()
	{
		if (!PhotonNetwork.IsMasterClient)
		{
			return;
		}
		Debug.Log("SPAWN SCOUTMASTER");
		PhotonNetwork.InstantiateRoomObject("Character_Scoutmaster", base.transform.position, base.transform.rotation, 0, null).GetComponent<Character>().data.spawnPoint = base.transform;
	}
}
