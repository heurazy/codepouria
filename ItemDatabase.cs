using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Zorro.Core;
using Zorro.Core.CLI;

// Token: 0x020000DC RID: 220
[ConsoleClassCustomizer("Item")]
[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Scouts/ItemDatabase")]
public class ItemDatabase : ObjectDatabaseAsset<ItemDatabase, Item>
{
	// Token: 0x060006B0 RID: 1712 RVA: 0x00023596 File Offset: 0x00021796
	public override void OnLoaded()
	{
		base.OnLoaded();
	}

	// Token: 0x060006B1 RID: 1713 RVA: 0x0002359E File Offset: 0x0002179E
	public void LoadItems()
	{
	}

	// Token: 0x060006B2 RID: 1714 RVA: 0x000235A0 File Offset: 0x000217A0
	[ContextMenu("Reload entire database")]
	public void ReloadAllItems()
	{
	}

	// Token: 0x060006B3 RID: 1715 RVA: 0x000235A4 File Offset: 0x000217A4
	private ushort GetAvailableID()
	{
		for (ushort num = 0; num < 65535; num += 1)
		{
			if (!this.itemLookup.ContainsKey(num))
			{
				return num;
			}
		}
		return 0;
	}

	// Token: 0x060006B4 RID: 1716 RVA: 0x000235D4 File Offset: 0x000217D4
	private bool ItemExistsInDatabase(Item item, out ushort itemID)
	{
		foreach (ushort num in this.itemLookup.Keys)
		{
			if (this.itemLookup[num].Equals(item))
			{
				itemID = num;
				return true;
			}
		}
		itemID = 0;
		return false;
	}

	// Token: 0x060006B5 RID: 1717 RVA: 0x00023648 File Offset: 0x00021848
	[ConsoleCommand]
	public static void Add(Item item)
	{
		if (MainCamera.instance == null)
		{
			return;
		}
		if (!PhotonNetwork.IsConnected)
		{
			return;
		}
		Transform transform = MainCamera.instance.transform;
		RaycastHit raycastHit;
		if (!Physics.Raycast(transform.position, transform.forward, out raycastHit))
		{
			raycastHit = default(RaycastHit);
		}
		ItemDatabase.Add(item, raycastHit.point + raycastHit.normal);
	}

	// Token: 0x060006B6 RID: 1718 RVA: 0x000236AC File Offset: 0x000218AC
	public static void Add(Item item, Vector3 point)
	{
		if (!PhotonNetwork.IsConnected)
		{
			return;
		}
		Debug.Log(string.Format("Spawn item: {0} at {1}", item, point));
		PhotonNetwork.Instantiate("0_Items/" + item.name, point, Quaternion.identity, 0, null).GetComponent<Item>().RequestPickup(Character.localCharacter.GetComponent<PhotonView>());
	}

	// Token: 0x060006B7 RID: 1719 RVA: 0x00023708 File Offset: 0x00021908
	public static bool TryGetItem(ushort itemID, out Item item)
	{
		return SingletonAsset<ItemDatabase>.Instance.itemLookup.TryGetValue(itemID, out item);
	}

	// Token: 0x0400065A RID: 1626
	public Dictionary<ushort, Item> itemLookup = new Dictionary<ushort, Item>();
}
