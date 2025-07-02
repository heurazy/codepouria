using System;
using Photon.Pun;
using UnityEngine;
using Zorro.Core.Serizalization;

// Token: 0x020000C7 RID: 199
public struct BackpackReference : IBinarySerializable
{
	// Token: 0x0600063F RID: 1599 RVA: 0x00021D94 File Offset: 0x0001FF94
	public static BackpackReference GetFromBackpackItem(Item item)
	{
		return new BackpackReference
		{
			type = BackpackReference.BackpackType.Item,
			view = item.GetComponent<PhotonView>(),
			locationTransform = item.transform
		};
	}

	// Token: 0x06000640 RID: 1600 RVA: 0x00021DCC File Offset: 0x0001FFCC
	public static BackpackReference GetFromEquippedBackpack(Character character)
	{
		return new BackpackReference
		{
			type = BackpackReference.BackpackType.Equipped,
			view = character.GetComponent<PhotonView>(),
			locationTransform = character.GetBodypart(BodypartType.Torso).transform
		};
	}

	// Token: 0x06000641 RID: 1601 RVA: 0x00021E0A File Offset: 0x0002000A
	public BackpackVisuals GetVisuals()
	{
		if (this.type == BackpackReference.BackpackType.Item)
		{
			return this.view.GetComponent<ItemBackpackVisuals>();
		}
		return this.view.GetComponent<CharacterBackpackHandler>().backpackVisuals;
	}

	// Token: 0x06000642 RID: 1602 RVA: 0x00021E30 File Offset: 0x00020030
	public void Serialize(BinarySerializer serializer)
	{
		serializer.WriteByte((byte)this.type);
		serializer.WriteInt(this.view.ViewID);
	}

	// Token: 0x06000643 RID: 1603 RVA: 0x00021E4F File Offset: 0x0002004F
	public void Deserialize(BinaryDeserializer deserializer)
	{
		this.type = (BackpackReference.BackpackType)deserializer.ReadByte();
		this.view = PhotonView.Find(deserializer.ReadInt());
	}

	// Token: 0x06000644 RID: 1604 RVA: 0x00021E6E File Offset: 0x0002006E
	public ItemInstanceData GetItemInstanceData()
	{
		if (this.type == BackpackReference.BackpackType.Item)
		{
			return this.view.GetComponent<Item>().data;
		}
		return this.view.GetComponent<Character>().player.backpackSlot.data;
	}

	// Token: 0x06000645 RID: 1605 RVA: 0x00021EA4 File Offset: 0x000200A4
	public BackpackData GetData()
	{
		if (this.type == BackpackReference.BackpackType.Item)
		{
			return this.view.GetComponent<Item>().GetData<BackpackData>(DataEntryKey.BackpackData);
		}
		BackpackData backpackData;
		if (!this.view.GetComponent<Character>().player.backpackSlot.data.TryGetDataEntry<BackpackData>(DataEntryKey.BackpackData, out backpackData))
		{
			backpackData = this.view.GetComponent<Character>().player.backpackSlot.data.RegisterNewEntry<BackpackData>(DataEntryKey.BackpackData);
		}
		return backpackData;
	}

	// Token: 0x06000646 RID: 1606 RVA: 0x00021F11 File Offset: 0x00020111
	public bool IsOnMyBack()
	{
		return this.type != BackpackReference.BackpackType.Item && this.view.IsMine;
	}

	// Token: 0x06000647 RID: 1607 RVA: 0x00021F28 File Offset: 0x00020128
	public bool TryGetBackpackItem(out Backpack backpack)
	{
		if (this.type == BackpackReference.BackpackType.Item)
		{
			backpack = this.view.GetComponent<Backpack>();
			return true;
		}
		backpack = null;
		return false;
	}

	// Token: 0x04000618 RID: 1560
	public BackpackReference.BackpackType type;

	// Token: 0x04000619 RID: 1561
	public PhotonView view;

	// Token: 0x0400061A RID: 1562
	public Transform locationTransform;

	// Token: 0x02000328 RID: 808
	public enum BackpackType : byte
	{
		// Token: 0x0400119E RID: 4510
		Item,
		// Token: 0x0400119F RID: 4511
		Equipped
	}
}
