using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace ExitGames.Demos.DemoPunVoice
{
	// Token: 0x020002B4 RID: 692
	public class CharacterInstantiation : MonoBehaviourPunCallbacks, IOnEventCallback
	{
		// Token: 0x14000005 RID: 5
		// (add) Token: 0x060010BE RID: 4286 RVA: 0x00052DF4 File Offset: 0x00050FF4
		// (remove) Token: 0x060010BF RID: 4287 RVA: 0x00052E28 File Offset: 0x00051028
		public static event CharacterInstantiation.OnCharacterInstantiated CharacterInstantiated;

		// Token: 0x060010C0 RID: 4288 RVA: 0x00052E5C File Offset: 0x0005105C
		public override void OnJoinedRoom()
		{
			if (!this.AutoSpawn)
			{
				return;
			}
			if (this.PrefabsToInstantiate != null)
			{
				int num = PhotonNetwork.LocalPlayer.ActorNumber;
				if (num < 1)
				{
					num = 1;
				}
				int num2 = (num - 1) % this.PrefabsToInstantiate.Length;
				Vector3 vector;
				Quaternion quaternion;
				this.GetSpawnPoint(out vector, out quaternion);
				Camera.main.transform.position += vector;
				if (this.manualInstantiation)
				{
					this.ManualInstantiation(num2, vector, quaternion);
					return;
				}
				GameObject gameObject = this.PrefabsToInstantiate[num2];
				gameObject = PhotonNetwork.Instantiate(gameObject.name, vector, quaternion, 0, null);
				if (CharacterInstantiation.CharacterInstantiated != null)
				{
					CharacterInstantiation.CharacterInstantiated(gameObject);
				}
			}
		}

		// Token: 0x060010C1 RID: 4289 RVA: 0x00052F04 File Offset: 0x00051104
		private void ManualInstantiation(int index, Vector3 position, Quaternion rotation)
		{
			GameObject gameObject = this.PrefabsToInstantiate[index];
			GameObject gameObject2;
			if (this.differentPrefabs)
			{
				gameObject2 = Object.Instantiate<GameObject>(Resources.Load(string.Format("{0}{1}", gameObject.name, this.localPrefabSuffix)) as GameObject, position, rotation);
			}
			else
			{
				gameObject2 = Object.Instantiate<GameObject>(gameObject, position, rotation);
			}
			PhotonView component = gameObject2.GetComponent<PhotonView>();
			if (PhotonNetwork.AllocateViewID(component))
			{
				object[] array = new object[]
				{
					index,
					gameObject2.transform.position,
					gameObject2.transform.rotation,
					component.ViewID
				};
				RaiseEventOptions raiseEventOptions = new RaiseEventOptions
				{
					Receivers = ReceiverGroup.Others,
					CachingOption = EventCaching.AddToRoomCache
				};
				PhotonNetwork.RaiseEvent(this.manualInstantiationEventCode, array, raiseEventOptions, SendOptions.SendReliable);
				if (CharacterInstantiation.CharacterInstantiated != null)
				{
					CharacterInstantiation.CharacterInstantiated(gameObject2);
					return;
				}
			}
			else
			{
				Debug.LogError("Failed to allocate a ViewId.");
				Object.Destroy(gameObject2);
			}
		}

		// Token: 0x060010C2 RID: 4290 RVA: 0x00052FF8 File Offset: 0x000511F8
		public void OnEvent(EventData photonEvent)
		{
			if (photonEvent.Code == this.manualInstantiationEventCode)
			{
				object[] array = photonEvent.CustomData as object[];
				int num = (int)array[0];
				GameObject gameObject = this.PrefabsToInstantiate[num];
				Vector3 vector = (Vector3)array[1];
				Quaternion quaternion = (Quaternion)array[2];
				GameObject gameObject2;
				if (this.differentPrefabs)
				{
					gameObject2 = Object.Instantiate<GameObject>(Resources.Load(string.Format("{0}{1}", gameObject.name, this.remotePrefabSuffix)) as GameObject, vector, quaternion);
				}
				else
				{
					gameObject2 = Object.Instantiate<GameObject>(gameObject, vector, Quaternion.identity);
				}
				gameObject2.GetComponent<PhotonView>().ViewID = (int)array[3];
			}
		}

		// Token: 0x060010C3 RID: 4291 RVA: 0x000530A0 File Offset: 0x000512A0
		protected virtual void GetSpawnPoint(out Vector3 spawnPos, out Quaternion spawnRot)
		{
			Transform spawnPoint = this.GetSpawnPoint();
			if (spawnPoint != null)
			{
				spawnPos = spawnPoint.position;
				spawnRot = spawnPoint.rotation;
			}
			else
			{
				spawnPos = new Vector3(0f, 0f, 0f);
				spawnRot = new Quaternion(0f, 0f, 0f, 1f);
			}
			if (this.UseRandomOffset)
			{
				Debug.Log("Set Seed");
				Random.InitState((int)(Time.time * 10000f));
				Vector3 vector = Random.insideUnitSphere;
				vector.y = 0f;
				vector = vector.normalized;
				spawnPos += this.PositionOffset * vector;
			}
		}

		// Token: 0x060010C4 RID: 4292 RVA: 0x0005316C File Offset: 0x0005136C
		protected virtual Transform GetSpawnPoint()
		{
			if (this.SpawnPoints == null || this.SpawnPoints.Count == 0)
			{
				return null;
			}
			switch (this.Sequence)
			{
			case CharacterInstantiation.SpawnSequence.Connection:
			{
				int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
				return this.SpawnPoints[(actorNumber == -1) ? 0 : (actorNumber % this.SpawnPoints.Count)];
			}
			case CharacterInstantiation.SpawnSequence.Random:
				return this.SpawnPoints[Random.Range(0, this.SpawnPoints.Count)];
			case CharacterInstantiation.SpawnSequence.RoundRobin:
				this.lastUsedSpawnPointIndex++;
				if (this.lastUsedSpawnPointIndex >= this.SpawnPoints.Count)
				{
					this.lastUsedSpawnPointIndex = 0;
				}
				return this.SpawnPoints[this.lastUsedSpawnPointIndex];
			default:
				return null;
			}
		}

		// Token: 0x04000F5F RID: 3935
		public Transform SpawnPosition;

		// Token: 0x04000F60 RID: 3936
		public float PositionOffset = 2f;

		// Token: 0x04000F61 RID: 3937
		public GameObject[] PrefabsToInstantiate;

		// Token: 0x04000F62 RID: 3938
		public List<Transform> SpawnPoints;

		// Token: 0x04000F63 RID: 3939
		public bool AutoSpawn = true;

		// Token: 0x04000F64 RID: 3940
		public bool UseRandomOffset = true;

		// Token: 0x04000F65 RID: 3941
		public CharacterInstantiation.SpawnSequence Sequence;

		// Token: 0x04000F67 RID: 3943
		[SerializeField]
		private byte manualInstantiationEventCode = 1;

		// Token: 0x04000F68 RID: 3944
		protected int lastUsedSpawnPointIndex = -1;

		// Token: 0x04000F69 RID: 3945
		[SerializeField]
		private bool manualInstantiation;

		// Token: 0x04000F6A RID: 3946
		[SerializeField]
		private bool differentPrefabs;

		// Token: 0x04000F6B RID: 3947
		[SerializeField]
		private string localPrefabSuffix;

		// Token: 0x04000F6C RID: 3948
		[SerializeField]
		private string remotePrefabSuffix;

		// Token: 0x020003CB RID: 971
		public enum SpawnSequence
		{
			// Token: 0x040013FC RID: 5116
			Connection,
			// Token: 0x040013FD RID: 5117
			Random,
			// Token: 0x040013FE RID: 5118
			RoundRobin
		}

		// Token: 0x020003CC RID: 972
		// (Invoke) Token: 0x06001516 RID: 5398
		public delegate void OnCharacterInstantiated(GameObject character);
	}
}
