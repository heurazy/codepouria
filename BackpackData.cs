using System;
using UnityEngine;
using Zorro.Core.Serizalization;

// Token: 0x020000CF RID: 207
public class BackpackData : DataEntryValue
{
	// Token: 0x0600066F RID: 1647 RVA: 0x00022AB8 File Offset: 0x00020CB8
	public override void Init()
	{
		base.Init();
		byte b = 0;
		while ((int)b < this.itemSlots.Length)
		{
			this.itemSlots[(int)b] = new ItemSlot(b);
			b += 1;
		}
	}

	// Token: 0x06000670 RID: 1648 RVA: 0x00022AF0 File Offset: 0x00020CF0
	public override void SerializeValue(BinarySerializer serializer)
	{
		InventorySyncData inventorySyncData = new InventorySyncData(this.itemSlots, new BackpackSlot(4)
		{
			data = new ItemInstanceData(Guid.Empty)
		}, new TemporaryItemSlot(250));
		inventorySyncData.Serialize(serializer);
	}

	// Token: 0x06000671 RID: 1649 RVA: 0x00022B34 File Offset: 0x00020D34
	public override void DeserializeValue(BinaryDeserializer deserializer)
	{
		InventorySyncData inventorySyncData = default(InventorySyncData);
		inventorySyncData.Deserialize(deserializer);
		for (byte b = 0; b < 4; b += 1)
		{
			if (this.itemSlots[(int)b] == null)
			{
				this.itemSlots[(int)b] = new ItemSlot(b);
			}
			Item item;
			this.itemSlots[(int)b].prefab = (ItemDatabase.TryGetItem(inventorySyncData.slots[(int)b].ItemID, out item) ? item : null);
			this.itemSlots[(int)b].data = inventorySyncData.slots[(int)b].Data;
			Debug.Log(string.Format("Sync Back Inventory is setting slot: {0} to {1}", b, this.itemSlots[(int)b].ToString()));
		}
	}

	// Token: 0x06000672 RID: 1650 RVA: 0x00022BE8 File Offset: 0x00020DE8
	public void AddItem(Item prefab, ItemInstanceData data, byte backpackSlotID)
	{
		if (data == null)
		{
			Debug.Log("DATA IS NULL??");
			data = new ItemInstanceData(Guid.NewGuid());
			ItemInstanceDataHandler.AddInstanceData(data);
		}
		if ((int)backpackSlotID < this.itemSlots.Length && this.itemSlots[(int)backpackSlotID].IsEmpty())
		{
			Debug.Log(string.Format("Added item: {0} to slot {1}", prefab.gameObject.name, backpackSlotID));
			this.itemSlots[(int)backpackSlotID].prefab = prefab;
			this.itemSlots[(int)backpackSlotID].data = data;
			return;
		}
	}

	// Token: 0x06000673 RID: 1651 RVA: 0x00022C6C File Offset: 0x00020E6C
	public bool HasFreeSlot()
	{
		for (int i = 0; i < this.itemSlots.Length; i++)
		{
			if (this.itemSlots[i].IsEmpty())
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000674 RID: 1652 RVA: 0x00022CA0 File Offset: 0x00020EA0
	public int FilledSlotCount()
	{
		int num = this.itemSlots.Length;
		for (int i = 0; i < this.itemSlots.Length; i++)
		{
			if (this.itemSlots[i].IsEmpty())
			{
				num--;
			}
		}
		return num;
	}

	// Token: 0x06000675 RID: 1653 RVA: 0x00022CE0 File Offset: 0x00020EE0
	public override string ToString()
	{
		string text = "";
		foreach (ItemSlot itemSlot in this.itemSlots)
		{
			text = text + itemSlot.ToString() + ", " + Environment.NewLine;
		}
		return text;
	}

	// Token: 0x0400063B RID: 1595
	public const int slots = 4;

	// Token: 0x0400063C RID: 1596
	public ItemSlot[] itemSlots = new ItemSlot[4];
}
