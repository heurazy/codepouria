using System;
using Zorro.Core.Serizalization;

// Token: 0x020000F9 RID: 249
public struct InventorySyncData : IBinarySerializable
{
	// Token: 0x0600075D RID: 1885 RVA: 0x00027538 File Offset: 0x00025738
	public InventorySyncData(ItemSlot[] itemSlots, BackpackSlot backpackSlot, ItemSlot tempSlot)
	{
		this.slotCount = (byte)itemSlots.Length;
		this.slots = new InventorySyncData.SlotData[itemSlots.Length];
		InventorySyncData.SlotData slotData;
		for (int i = 0; i < (int)this.slotCount; i++)
		{
			ushort num = ((itemSlots[i].prefab == null) ? ushort.MaxValue : itemSlots[i].prefab.itemID);
			InventorySyncData.SlotData[] array = this.slots;
			int num2 = i;
			slotData = new InventorySyncData.SlotData
			{
				ItemID = num,
				Data = itemSlots[i].data
			};
			array[num2] = slotData;
		}
		slotData = new InventorySyncData.SlotData
		{
			ItemID = ((tempSlot.prefab == null) ? ushort.MaxValue : tempSlot.prefab.itemID),
			Data = tempSlot.data
		};
		this.tempSlot = slotData;
		this.hasBackpack = !backpackSlot.IsEmpty();
		this.backpackInstanceData = backpackSlot.data;
	}

	// Token: 0x0600075E RID: 1886 RVA: 0x00027620 File Offset: 0x00025820
	public void Serialize(BinarySerializer serializer)
	{
		serializer.WriteByte(this.slotCount);
		for (int i = 0; i < (int)this.slotCount; i++)
		{
			this.slots[i].Serialize(serializer);
		}
		this.tempSlot.Serialize(serializer);
		serializer.WriteBool(this.hasBackpack);
		if (this.hasBackpack)
		{
			if (this.backpackInstanceData == null)
			{
				this.backpackInstanceData = new ItemInstanceData(Guid.NewGuid());
				ItemInstanceDataHandler.AddInstanceData(this.backpackInstanceData);
			}
			serializer.WriteGuid(this.backpackInstanceData.guid);
			this.backpackInstanceData.Serialize(serializer);
		}
	}

	// Token: 0x0600075F RID: 1887 RVA: 0x000276BC File Offset: 0x000258BC
	public void Deserialize(BinaryDeserializer deserializer)
	{
		this.slotCount = deserializer.ReadByte();
		this.slots = new InventorySyncData.SlotData[(int)this.slotCount];
		this.tempSlot = default(InventorySyncData.SlotData);
		for (int i = 0; i < (int)this.slotCount; i++)
		{
			InventorySyncData.SlotData slotData = default(InventorySyncData.SlotData);
			slotData.Deserialize(deserializer);
			this.slots[i] = slotData;
		}
		this.tempSlot.Deserialize(deserializer);
		this.hasBackpack = deserializer.ReadBool();
		if (this.hasBackpack)
		{
			Guid guid = deserializer.ReadGuid();
			if (!ItemInstanceDataHandler.TryGetInstanceData(guid, out this.backpackInstanceData))
			{
				this.backpackInstanceData = new ItemInstanceData(guid);
				ItemInstanceDataHandler.AddInstanceData(this.backpackInstanceData);
			}
			this.backpackInstanceData.Deserialize(deserializer);
		}
	}

	// Token: 0x040006F4 RID: 1780
	public byte slotCount;

	// Token: 0x040006F5 RID: 1781
	public InventorySyncData.SlotData[] slots;

	// Token: 0x040006F6 RID: 1782
	public InventorySyncData.SlotData tempSlot;

	// Token: 0x040006F7 RID: 1783
	public bool hasBackpack;

	// Token: 0x040006F8 RID: 1784
	public ItemInstanceData backpackInstanceData;

	// Token: 0x0200033F RID: 831
	public struct SlotData : IBinarySerializable
	{
		// Token: 0x06001349 RID: 4937 RVA: 0x0005C4D9 File Offset: 0x0005A6D9
		public void Serialize(BinarySerializer serializer)
		{
			serializer.WriteUshort(this.ItemID);
			if (this.ItemID != 65535)
			{
				serializer.WriteGuid(this.Data.guid);
				this.Data.Serialize(serializer);
			}
		}

		// Token: 0x0600134A RID: 4938 RVA: 0x0005C514 File Offset: 0x0005A714
		public void Deserialize(BinaryDeserializer deserializer)
		{
			this.ItemID = deserializer.ReadUShort();
			if (this.ItemID != 65535)
			{
				Guid guid = deserializer.ReadGuid();
				if (!ItemInstanceDataHandler.TryGetInstanceData(guid, out this.Data))
				{
					this.Data = new ItemInstanceData(guid);
					ItemInstanceDataHandler.AddInstanceData(this.Data);
				}
				this.Data.Deserialize(deserializer);
			}
		}

		// Token: 0x040011FD RID: 4605
		public ushort ItemID;

		// Token: 0x040011FE RID: 4606
		public ItemInstanceData Data;
	}
}
