using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200009E RID: 158
public class ItemBackpackVisuals : BackpackVisuals
{
	// Token: 0x060005C2 RID: 1474 RVA: 0x00020510 File Offset: 0x0001E710
	private void Awake()
	{
		this.item = base.GetComponent<Item>();
	}

	// Token: 0x060005C3 RID: 1475 RVA: 0x0002051E File Offset: 0x0001E71E
	public override BackpackData GetBackpackData()
	{
		return base.GetComponent<Item>().GetData<BackpackData>(DataEntryKey.BackpackData);
	}

	// Token: 0x060005C4 RID: 1476 RVA: 0x0002052C File Offset: 0x0001E72C
	protected override void PutItemInBackpack(GameObject visual, byte slotID)
	{
		visual.GetComponent<PhotonView>().RPC("PutInBackpackRPC", RpcTarget.All, new object[]
		{
			slotID,
			BackpackReference.GetFromBackpackItem(this.item)
		});
	}

	// Token: 0x040005CE RID: 1486
	private Item item;
}
