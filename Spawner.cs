using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000088 RID: 136
public class Spawner : OnNetworkStart
{
	// Token: 0x1700004E RID: 78
	// (get) Token: 0x060004B6 RID: 1206 RVA: 0x0001B735 File Offset: 0x00019935
	protected bool isWeightedSpawnPoints
	{
		get
		{
			return this.spawnPointMode == Spawner.SpawnPointMode.WeightedLists;
		}
	}

	// Token: 0x1700004F RID: 79
	// (get) Token: 0x060004B7 RID: 1207 RVA: 0x0001B740 File Offset: 0x00019940
	private bool isSpawnPool
	{
		get
		{
			return this.spawnMode == Spawner.SpawnMode.SpawnPool;
		}
	}

	// Token: 0x17000050 RID: 80
	// (get) Token: 0x060004B8 RID: 1208 RVA: 0x0001B74B File Offset: 0x0001994B
	private bool isSingleItem
	{
		get
		{
			return this.spawnMode == Spawner.SpawnMode.SingleItem;
		}
	}

	// Token: 0x17000051 RID: 81
	// (get) Token: 0x060004B9 RID: 1209 RVA: 0x0001B756 File Offset: 0x00019956
	private bool isHeightBasedSpawnPool
	{
		get
		{
			return this.spawnMode == Spawner.SpawnMode.HeightBasedSpawnPools;
		}
	}

	// Token: 0x17000052 RID: 82
	// (get) Token: 0x060004BA RID: 1210 RVA: 0x0001B761 File Offset: 0x00019961
	public bool hasSpawnList
	{
		get
		{
			return this.isSpawnPool && this.spawns != null && this.spawnPool == SpawnPool.None;
		}
	}

	// Token: 0x060004BB RID: 1211 RVA: 0x0001B784 File Offset: 0x00019984
	public override void NetworkStart()
	{
	}

	// Token: 0x060004BC RID: 1212 RVA: 0x0001B788 File Offset: 0x00019988
	public List<PhotonView> TrySpawnItems()
	{
		List<PhotonView> list = new List<PhotonView>();
		if (!PhotonNetwork.IsMasterClient)
		{
			return list;
		}
		if (!this.spawnOnStart)
		{
			return list;
		}
		if (Random.Range(0f, 1f) <= this.baseSpawnChance)
		{
			list.AddRange(this.SpawnItems(this.GetSpawnSpots()));
		}
		return list;
	}

	// Token: 0x060004BD RID: 1213 RVA: 0x0001B7D8 File Offset: 0x000199D8
	protected virtual List<Transform> GetSpawnSpots()
	{
		Spawner.SpawnPointMode spawnPointMode = this.spawnPointMode;
		if (spawnPointMode == Spawner.SpawnPointMode.SingleList)
		{
			return this.spawnSpots;
		}
		if (spawnPointMode != Spawner.SpawnPointMode.WeightedLists)
		{
			return new List<Transform>();
		}
		return this.weightedSpawnSpots.RandomSelection((Spawner.WeightedSpawnPointEntry w) => w.weight).spawnSpots;
	}

	// Token: 0x060004BE RID: 1214 RVA: 0x0001B834 File Offset: 0x00019A34
	public virtual List<PhotonView> SpawnItems(List<Transform> spawnSpots)
	{
		List<PhotonView> list = new List<PhotonView>();
		if (!PhotonNetwork.IsMasterClient)
		{
			return list;
		}
		if (spawnSpots.Count == 0)
		{
			return list;
		}
		List<GameObject> objectsToSpawn = this.GetObjectsToSpawn(spawnSpots.Count, this.canRepeatSpawns);
		int num = 0;
		while (num < spawnSpots.Count && num < spawnSpots.Count)
		{
			if (!(objectsToSpawn[num] == null))
			{
				Item component = PhotonNetwork.InstantiateItemRoom(objectsToSpawn[num].name, spawnSpots[num].position, spawnSpots[num].rotation).GetComponent<Item>();
				list.Add(component.GetComponent<PhotonView>());
				if (this.spawnUpTowardsTarget)
				{
					component.transform.up = (this.spawnUpTowardsTarget.position - component.transform.position).normalized;
				}
				if (this.centerItemsVisually)
				{
					Vector3 vector = spawnSpots[num].position - component.Center();
					component.transform.position += vector;
				}
				component.ForceSyncForFrames();
				if (component != null)
				{
					component.GetComponent<PhotonView>().RPC("SetKinematicRPC", RpcTarget.AllBuffered, new object[]
					{
						true,
						component.transform.position,
						component.transform.rotation
					});
				}
			}
			num++;
		}
		return list;
	}

	// Token: 0x060004BF RID: 1215 RVA: 0x0001B9A8 File Offset: 0x00019BA8
	private SpawnPool GetSpawnPool()
	{
		if (this.isHeightBasedSpawnPool)
		{
			for (int i = this.heightBasedSpawnPools.Count - 1; i >= 0; i--)
			{
				Spawner.HeightBasedSpawnListEntry heightBasedSpawnListEntry = this.heightBasedSpawnPools[i];
				if (i == 0 || base.transform.position.y > heightBasedSpawnListEntry.minimumHeight)
				{
					return heightBasedSpawnListEntry.spawnPool;
				}
			}
		}
		return this.spawnPool;
	}

	// Token: 0x060004C0 RID: 1216 RVA: 0x0001BA0C File Offset: 0x00019C0C
	private List<GameObject> GetObjectsToSpawn(int spawnCount, bool canRepeat = false)
	{
		if (this.isSingleItem)
		{
			List<GameObject> list = new List<GameObject>();
			for (int i = 0; i < spawnCount; i++)
			{
				list.Add(this.spawnedObjectPrefab);
			}
			return list;
		}
		if (this.isSpawnPool)
		{
			return LootData.GetRandomItems(this.spawnPool, spawnCount, canRepeat);
		}
		if (this.isHeightBasedSpawnPool)
		{
			for (int j = this.heightBasedSpawnPools.Count - 1; j >= 0; j--)
			{
				Spawner.HeightBasedSpawnListEntry heightBasedSpawnListEntry = this.heightBasedSpawnPools[j];
				if (j == 0 || base.transform.position.y > heightBasedSpawnListEntry.minimumHeight)
				{
					return LootData.GetRandomItems(heightBasedSpawnListEntry.spawnPool, spawnCount, canRepeat);
				}
			}
		}
		List<GameObject> list2 = new List<GameObject>();
		for (int k = 0; k < spawnCount; k++)
		{
			list2.Add(null);
		}
		return list2;
	}

	// Token: 0x060004C1 RID: 1217 RVA: 0x0001BAD0 File Offset: 0x00019CD0
	private void FindOutdatedSpawners()
	{
		bool flag = false;
		Spawner[] array = Object.FindObjectsOfType<Spawner>();
		string text = "";
		foreach (Spawner spawner in array)
		{
			if (spawner.hasSpawnList)
			{
				text = text + "Found outdated spawner: " + spawner.gameObject.name + "\n";
				flag = true;
			}
		}
		if (!flag)
		{
			Debug.Log("NO OUTDATED SPAWNERS! YIPPEEEE");
			return;
		}
		Debug.Log(text);
	}

	// Token: 0x060004C2 RID: 1218 RVA: 0x0001BB3C File Offset: 0x00019D3C
	[ContextMenu("Test Weighted Spawn Points")]
	private void TestWeightedSpawnPoints()
	{
		Dictionary<int, int> dictionary = new Dictionary<int, int>();
		int num = 1000;
		for (int i = 0; i < num; i++)
		{
			Spawner.WeightedSpawnPointEntry weightedSpawnPointEntry = this.weightedSpawnSpots.RandomSelection((Spawner.WeightedSpawnPointEntry w) => w.weight);
			int num2 = this.weightedSpawnSpots.IndexOf(weightedSpawnPointEntry);
			if (dictionary.ContainsKey(num2))
			{
				Dictionary<int, int> dictionary2 = dictionary;
				int num3 = num2;
				int num4 = dictionary2[num3];
				dictionary2[num3] = num4 + 1;
			}
			else
			{
				dictionary.Add(num2, 1);
			}
		}
		string text = string.Format("Test spawned {0} times.\n", num);
		foreach (int num5 in dictionary.Keys)
		{
			text += string.Format("Chose #{0} {1} times. ({2}%)\n", num5, dictionary[num5], (float)dictionary[num5] / (float)num * 100f);
		}
		Debug.Log(text);
	}

	// Token: 0x060004C3 RID: 1219 RVA: 0x0001BC60 File Offset: 0x00019E60
	public void DebugSpawnRates()
	{
		SpawnPool spawnPool = this.GetSpawnPool();
		if (spawnPool != SpawnPool.None)
		{
			LootData.PrintOdds(spawnPool);
		}
	}

	// Token: 0x17000053 RID: 83
	// (get) Token: 0x060004C4 RID: 1220 RVA: 0x0001BC80 File Offset: 0x00019E80
	private bool hasMultipleFlagsSet
	{
		get
		{
			int num = 0;
			foreach (object obj in Enum.GetValues(typeof(SpawnPool)))
			{
				SpawnPool spawnPool = (SpawnPool)obj;
				if (spawnPool != SpawnPool.None && this.spawnPool.HasFlag(spawnPool))
				{
					if (num >= 1)
					{
						return true;
					}
					num++;
				}
			}
			return false;
		}
	}

	// Token: 0x060004C6 RID: 1222 RVA: 0x0001BD26 File Offset: 0x00019F26
	[CompilerGenerated]
	private IEnumerator <NetworkStart>g__WaitAFrame|26_0()
	{
		yield return null;
		this.TrySpawnItems();
		yield break;
	}

	// Token: 0x040004F3 RID: 1267
	public Spawner.SpawnMode spawnMode = Spawner.SpawnMode.SpawnPool;

	// Token: 0x040004F4 RID: 1268
	[FormerlySerializedAs("spawnCountMode")]
	public Spawner.SpawnPointMode spawnPointMode;

	// Token: 0x040004F5 RID: 1269
	[Range(0f, 1f)]
	public float baseSpawnChance;

	// Token: 0x040004F6 RID: 1270
	public GameObject spawnedObjectPrefab;

	// Token: 0x040004F7 RID: 1271
	public SpawnList spawns;

	// Token: 0x040004F8 RID: 1272
	public SpawnPool spawnPool;

	// Token: 0x040004F9 RID: 1273
	public bool canRepeatSpawns;

	// Token: 0x040004FA RID: 1274
	public List<Transform> spawnSpots;

	// Token: 0x040004FB RID: 1275
	public List<Spawner.WeightedSpawnPointEntry> weightedSpawnSpots = new List<Spawner.WeightedSpawnPointEntry>();

	// Token: 0x040004FC RID: 1276
	public Transform spawnUpTowardsTarget;

	// Token: 0x040004FD RID: 1277
	public bool spawnAwayFromUpTarget;

	// Token: 0x040004FE RID: 1278
	public bool centerItemsVisually;

	// Token: 0x040004FF RID: 1279
	public bool spawnOnStart;

	// Token: 0x04000500 RID: 1280
	public List<Spawner.HeightBasedSpawnListEntry> heightBasedSpawnPools;

	// Token: 0x0200030E RID: 782
	public enum SpawnMode
	{
		// Token: 0x04001139 RID: 4409
		SingleItem,
		// Token: 0x0400113A RID: 4410
		SpawnPool,
		// Token: 0x0400113B RID: 4411
		HeightBasedSpawnPools,
		// Token: 0x0400113C RID: 4412
		Guidebook
	}

	// Token: 0x0200030F RID: 783
	public enum SpawnPointMode
	{
		// Token: 0x0400113E RID: 4414
		SingleList,
		// Token: 0x0400113F RID: 4415
		WeightedLists
	}

	// Token: 0x02000310 RID: 784
	[Serializable]
	public class HeightBasedSpawnListEntry
	{
		// Token: 0x04001140 RID: 4416
		public SpawnPool spawnPool;

		// Token: 0x04001141 RID: 4417
		public float minimumHeight;
	}

	// Token: 0x02000311 RID: 785
	[Serializable]
	public class WeightedSpawnPointEntry
	{
		// Token: 0x04001142 RID: 4418
		public List<Transform> spawnSpots;

		// Token: 0x04001143 RID: 4419
		public int weight;

		// Token: 0x04001144 RID: 4420
		[SerializeField]
		internal float percentageOdds;
	}
}
