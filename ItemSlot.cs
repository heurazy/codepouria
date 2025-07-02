using System;
using UnityEngine;

// Token: 0x0200001D RID: 29
[Serializable]
public class ItemSlot
{
	// Token: 0x06000204 RID: 516 RVA: 0x0000ED50 File Offset: 0x0000CF50
	public ItemSlot(byte slotID)
	{
		this.itemSlotID = slotID;
	}

	// Token: 0x06000205 RID: 517 RVA: 0x0000ED5F File Offset: 0x0000CF5F
	public virtual bool IsEmpty()
	{
		return this.prefab == null;
	}

	// Token: 0x06000206 RID: 518 RVA: 0x0000ED6D File Offset: 0x0000CF6D
	public void SetItem(Item itemPrefab, ItemInstanceData itemData)
	{
		this.data = itemData;
		this.prefab = itemPrefab;
		Debug.Log(string.Format("Item Slot ({0}) is now: {1}", this.itemSlotID, itemPrefab.name));
	}

	// Token: 0x06000207 RID: 519 RVA: 0x0000ED9D File Offset: 0x0000CF9D
	public virtual void EmptyOut()
	{
		this.prefab = null;
		Debug.Log(string.Format("Emptied Slot: {0}", this.itemSlotID));
	}

	// Token: 0x06000208 RID: 520 RVA: 0x0000EDC0 File Offset: 0x0000CFC0
	public override string ToString()
	{
		string text = ((this.prefab == null) ? "null" : this.prefab.name);
		return string.Format("Slot ({0}): {1}", this.itemSlotID, text);
	}

	// Token: 0x06000209 RID: 521 RVA: 0x0000EE04 File Offset: 0x0000D004
	public virtual string GetPrefabName()
	{
		return this.prefab.gameObject.name;
	}

	// Token: 0x040001FB RID: 507
	public Item prefab;

	// Token: 0x040001FC RID: 508
	public ItemInstanceData data;

	// Token: 0x040001FD RID: 509
	public byte itemSlotID;
}
