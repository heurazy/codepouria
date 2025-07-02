using System;

// Token: 0x02000004 RID: 4
public class BackpackSlot : ItemSlot
{
	// Token: 0x06000017 RID: 23 RVA: 0x0000245D File Offset: 0x0000065D
	public BackpackSlot(byte slotID)
		: base(slotID)
	{
	}

	// Token: 0x06000018 RID: 24 RVA: 0x00002466 File Offset: 0x00000666
	public override void EmptyOut()
	{
		this.hasBackpack = false;
		base.EmptyOut();
	}

	// Token: 0x06000019 RID: 25 RVA: 0x00002475 File Offset: 0x00000675
	public override bool IsEmpty()
	{
		return !this.hasBackpack;
	}

	// Token: 0x0600001A RID: 26 RVA: 0x00002480 File Offset: 0x00000680
	public override string GetPrefabName()
	{
		return "Backpack";
	}

	// Token: 0x04000006 RID: 6
	public bool hasBackpack;
}
