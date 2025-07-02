using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Unity.Multiplayer.Playmode;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zorro.Core;

// Token: 0x020000F5 RID: 245
public class CharacterSpawner : MonoBehaviourPunCallbacks
{
	// Token: 0x06000748 RID: 1864 RVA: 0x00026A84 File Offset: 0x00024C84
	private void Start()
	{
		if (PhotonNetwork.InRoom)
		{
			base.StartCoroutine(this.SpawnLocalPlayer());
		}
	}

	// Token: 0x06000749 RID: 1865 RVA: 0x00026A9A File Offset: 0x00024C9A
	public override void OnJoinedRoom()
	{
		base.StartCoroutine(this.SpawnLocalPlayer());
	}

	// Token: 0x0600074A RID: 1866 RVA: 0x00026AA9 File Offset: 0x00024CA9
	private IEnumerator SpawnLocalPlayer()
	{
		yield return new WaitForEndOfFrame();
		if (this.hasSpawnedPlayer)
		{
			yield break;
		}
		Vector3 vector = Vector3.zero;
		Quaternion quaternion = Quaternion.identity;
		int index = PhotonNetwork.LocalPlayer.ActorNumber % SpawnPoint.allSpawnPoints.Count;
		SpawnPoint spawnPoint = SpawnPoint.allSpawnPoints.FirstOrDefault((SpawnPoint s) => s.index == index);
		if (spawnPoint == null)
		{
			spawnPoint = SpawnPoint.allSpawnPoints[0];
		}
		this.hasSpawnedPlayer = true;
		if (spawnPoint != null)
		{
			vector = spawnPoint.transform.position;
			quaternion = spawnPoint.transform.rotation;
			Debug.Log(string.Format("Setting player{0} to spawn point {1}", PhotonNetwork.LocalPlayer.ActorNumber, spawnPoint.index));
		}
		else
		{
			Debug.LogError("No Spawn Point, make on in the scene!");
		}
		bool flag = SceneManager.GetActiveScene().name == "Airport";
		SceneSwitchingStatus sceneSwitchingStatus;
		if (!GameHandler.TryGetStatus<SceneSwitchingStatus>(out sceneSwitchingStatus) && !flag)
		{
			if (RoomProperties.me.IsReconnecting() && !RoomProperties.me.GetReconnectPosition(out vector))
			{
			}
		}
		else
		{
			GameHandler.ClearStatus<SceneSwitchingStatus>();
		}
		IEnumerable<string> enumerable = CurrentPlayer.ReadOnlyTags();
		if (Singleton<MapHandler>.Instance != null && Singleton<MapHandler>.Instance.GetCurrentSegment() != Segment.Beach)
		{
			Segment currentSegment = Singleton<MapHandler>.Instance.GetCurrentSegment();
			vector = Singleton<MapHandler>.Instance.segments[(int)currentSegment].reconnectSpawnPos.position;
		}
		if (!enumerable.Contains("NoCharacter"))
		{
			if (Character.localCharacter == null)
			{
				Debug.Log("Spawning local character.");
				Character component = PhotonNetwork.Instantiate("Character", vector, quaternion, 0, null).GetComponent<Character>();
				component.data.spawnPoint = spawnPoint.transform;
				if (spawnPoint.startPassedOut)
				{
					component.StartPassedOutOnTheBeach();
				}
			}
			else
			{
				Debug.Log("Moving local character to warp point.");
				Character.localCharacter.photonView.RPC("WarpPlayerRPC", RpcTarget.All, new object[] { vector, false });
				Character.localCharacter.data.spawnPoint = spawnPoint.transform;
			}
		}
		if (Player.localPlayer == null)
		{
			PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity, 0, null);
		}
		if (!flag)
		{
			RoomProperties.me.Reconnect();
		}
		else
		{
			RoomProperties.me.Clear();
		}
		yield break;
	}

	// Token: 0x040006E0 RID: 1760
	public Item[] itemsToSpawnWith;

	// Token: 0x040006E1 RID: 1761
	private bool hasSpawnedPlayer;
}
