using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Zorro.Core;

// Token: 0x020001F4 RID: 500
public class LootData : MonoBehaviour
{
	// Token: 0x06000D0A RID: 3338 RVA: 0x00041344 File Offset: 0x0003F544
	public static List<Item> GetAllItemsInPool(SpawnPool pool)
	{
		List<Item> list = new List<Item>();
		LootData.PopulateLootData();
		Dictionary<ushort, int> dictionary;
		if (LootData.AllSpawnWeightData.TryGetValue(pool, out dictionary))
		{
			foreach (KeyValuePair<ushort, int> keyValuePair in dictionary)
			{
				Item item;
				if (ItemDatabase.TryGetItem(keyValuePair.Key, out item))
				{
					list.Add(item);
				}
			}
		}
		return list;
	}

	// Token: 0x06000D0B RID: 3339 RVA: 0x000413C0 File Offset: 0x0003F5C0
	public bool IsValidToSpawn()
	{
		if (this.banInSolo)
		{
			if (PhotonNetwork.OfflineMode)
			{
				return false;
			}
			if (PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.PlayerCount <= 1)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06000D0C RID: 3340 RVA: 0x000413EC File Offset: 0x0003F5EC
	private void PrintOdds()
	{
		LootData.PopulateLootData();
		Item component = base.GetComponent<Item>();
		if (!component)
		{
			Debug.LogError("Loot data only works on Items right now.");
		}
		string text = base.gameObject.name + " appears in pools:\n";
		foreach (KeyValuePair<SpawnPool, Dictionary<ushort, int>> keyValuePair in LootData.AllSpawnWeightData)
		{
			if (keyValuePair.Value.ContainsKey(component.itemID))
			{
				text += string.Format("{0} ({1}%)\n", keyValuePair.Key.ToString(), LootData.GetPercentageOdds(component.itemID, keyValuePair.Key));
			}
		}
		Debug.Log(text);
	}

	// Token: 0x06000D0D RID: 3341 RVA: 0x000414C4 File Offset: 0x0003F6C4
	public static void PrintOdds(SpawnPool pool)
	{
		LootData.PopulateLootData();
		string text = pool.ToString() + " contains items:\n";
		Dictionary<ushort, int> dictionary;
		if (LootData.AllSpawnWeightData.TryGetValue(pool, out dictionary))
		{
			foreach (KeyValuePair<ushort, int> keyValuePair in dictionary)
			{
				Item item;
				if (ItemDatabase.TryGetItem(keyValuePair.Key, out item))
				{
					LootData component = item.GetComponent<LootData>();
					if (component)
					{
						text += string.Format("{0} ({1}% ({2}))\n", item.gameObject.name, LootData.GetPercentageOdds(keyValuePair.Key, pool), component.Rarity.ToString());
					}
					else
					{
						text += string.Format("{0} ({1}%)\n", item.gameObject.name, LootData.GetPercentageOdds(keyValuePair.Key, pool));
					}
				}
			}
		}
		Debug.Log(text);
	}

	// Token: 0x06000D0E RID: 3342 RVA: 0x000415E0 File Offset: 0x0003F7E0
	public static GameObject GetRandomItem(SpawnPool spawnPool)
	{
		if (LootData.AllSpawnWeightData == null)
		{
			LootData.PopulateLootData();
		}
		Dictionary<ushort, int> dictionary;
		if (LootData.AllSpawnWeightData.TryGetValue(spawnPool, out dictionary))
		{
			Item item;
			ItemDatabase.TryGetItem(dictionary.RandomSelection((KeyValuePair<ushort, int> i) => i.Value).Key, out item);
			return item.gameObject;
		}
		return null;
	}

	// Token: 0x06000D0F RID: 3343 RVA: 0x00041648 File Offset: 0x0003F848
	public static List<GameObject> GetRandomItems(SpawnPool spawnPool, int count, bool canRepeat = false)
	{
		if (LootData.AllSpawnWeightData == null)
		{
			LootData.PopulateLootData();
		}
		Dictionary<ushort, int> dictionary;
		if (LootData.AllSpawnWeightData.TryGetValue(spawnPool, out dictionary))
		{
			Dictionary<ushort, int> dictionary2 = new Dictionary<ushort, int>(dictionary);
			List<GameObject> list = new List<GameObject>();
			for (int j = 0; j < count; j++)
			{
				ushort key = dictionary2.RandomSelection((KeyValuePair<ushort, int> i) => i.Value).Key;
				Item item;
				if (ItemDatabase.TryGetItem(key, out item))
				{
					if (!item.IsValidToSpawn())
					{
						Debug.Log(item.gameObject.name + " IS INVALID TO SPAWN");
						dictionary2.Remove(key);
						j--;
					}
					else
					{
						list.Add(item.gameObject);
						if (!canRepeat)
						{
							dictionary2.Remove(key);
						}
					}
				}
			}
			return list;
		}
		return null;
	}

	// Token: 0x06000D10 RID: 3344 RVA: 0x0004171C File Offset: 0x0003F91C
	public static float GetPercentageOdds(ushort itemID, SpawnPool pool)
	{
		if (LootData.AllSpawnWeightData.ContainsKey(pool))
		{
			int num = 0;
			int num2 = 0;
			foreach (KeyValuePair<ushort, int> keyValuePair in LootData.AllSpawnWeightData[pool])
			{
				num += keyValuePair.Value;
				if (keyValuePair.Key == itemID)
				{
					num2 = keyValuePair.Value;
				}
			}
			return (float)Mathf.FloorToInt((float)num2 / (float)num * 1000f) / 10f;
		}
		return 0f;
	}

	// Token: 0x06000D11 RID: 3345 RVA: 0x000417B8 File Offset: 0x0003F9B8
	public static void PopulateLootData()
	{
		LootData.AllSpawnWeightData = new Dictionary<SpawnPool, Dictionary<ushort, int>>();
		Array values = Enum.GetValues(typeof(SpawnPool));
		foreach (KeyValuePair<ushort, Item> keyValuePair in SingletonAsset<ItemDatabase>.Instance.itemLookup)
		{
			LootData component = keyValuePair.Value.GetComponent<LootData>();
			if (component)
			{
				foreach (object obj in values)
				{
					SpawnPool spawnPool = (SpawnPool)obj;
					if (spawnPool != SpawnPool.None && component.spawnLocations.HasFlag(spawnPool))
					{
						int num = LootData.RarityWeights[component.Rarity];
						if (!LootData.AllSpawnWeightData.ContainsKey(spawnPool))
						{
							LootData.AllSpawnWeightData.Add(spawnPool, new Dictionary<ushort, int> { { keyValuePair.Key, num } });
						}
						else
						{
							LootData.AllSpawnWeightData[spawnPool].Add(keyValuePair.Key, num);
						}
					}
				}
			}
		}
	}

	// Token: 0x04000C06 RID: 3078
	public Rarity Rarity;

	// Token: 0x04000C07 RID: 3079
	public SpawnPool spawnLocations;

	// Token: 0x04000C08 RID: 3080
	public List<ItemRarityOverride> rarityOverrides = new List<ItemRarityOverride>();

	// Token: 0x04000C09 RID: 3081
	public bool banInSolo;

	// Token: 0x04000C0A RID: 3082
	public static Dictionary<SpawnPool, Dictionary<ushort, int>> AllSpawnWeightData = null;

	// Token: 0x04000C0B RID: 3083
	public static Dictionary<Rarity, int> RarityWeights = new Dictionary<Rarity, int>
	{
		{
			Rarity.Common,
			100
		},
		{
			Rarity.Uncommon,
			50
		},
		{
			Rarity.Rare,
			30
		},
		{
			Rarity.Epic,
			20
		},
		{
			Rarity.Legendary,
			15
		},
		{
			Rarity.Mythic,
			5
		},
		{
			Rarity.RidiculouslyRare,
			1
		}
	};
}
