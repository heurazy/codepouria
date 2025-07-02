using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x020000E6 RID: 230
public class SpawnList : MonoBehaviour
{
	// Token: 0x060006F9 RID: 1785 RVA: 0x000249EC File Offset: 0x00022BEC
	private void RefreshPercentageOdds()
	{
		int num = 0;
		foreach (SpawnEntry spawnEntry in this.items)
		{
			num += spawnEntry.weight;
		}
		foreach (SpawnEntry spawnEntry2 in this.items)
		{
			spawnEntry2.percentageOdds = (float)spawnEntry2.weight / (float)num;
			spawnEntry2.percentageOdds = (float)Mathf.FloorToInt(spawnEntry2.percentageOdds * 1000f) / 10f;
		}
	}

	// Token: 0x060006FA RID: 1786 RVA: 0x00024AAC File Offset: 0x00022CAC
	private void SortByWeight()
	{
		this.items = this.items.OrderByDescending((SpawnEntry item) => item.weight).ToList<SpawnEntry>();
	}

	// Token: 0x060006FB RID: 1787 RVA: 0x00024AE3 File Offset: 0x00022CE3
	public GameObject GetSingleSpawn()
	{
		return this.items.RandomSelection((SpawnEntry i) => i.weight).prefab;
	}

	// Token: 0x060006FC RID: 1788 RVA: 0x00024B14 File Offset: 0x00022D14
	public List<GameObject> GetSpawns(int count, bool canRepeat = true)
	{
		List<SpawnEntry> list = new List<SpawnEntry>(this.items);
		List<GameObject> list2 = new List<GameObject>();
		for (int j = 0; j < count; j++)
		{
			SpawnEntry spawnEntry = list.RandomSelection((SpawnEntry i) => i.weight);
			if (spawnEntry != null)
			{
				list2.Add(spawnEntry.prefab);
				if (!canRepeat)
				{
					if (list.Count <= 1)
					{
						list = new List<SpawnEntry>(this.items);
					}
					list.Remove(spawnEntry);
				}
			}
			else
			{
				list2.Add(null);
			}
		}
		return list2;
	}

	// Token: 0x060006FD RID: 1789 RVA: 0x00024BA0 File Offset: 0x00022DA0
	private void FindReferencesInScene()
	{
		Spawner[] array = Object.FindObjectsOfType<Spawner>();
		bool flag = false;
		foreach (Spawner spawner in array)
		{
			if (spawner.spawns.gameObject.name == base.gameObject.name)
			{
				Debug.Log("Found " + base.gameObject.name + " on " + spawner.gameObject.name);
				flag = true;
			}
		}
		if (!flag)
		{
			Debug.Log(base.gameObject.name + " not present in scene.");
		}
	}

	// Token: 0x0400068A RID: 1674
	public List<SpawnEntry> items;
}
