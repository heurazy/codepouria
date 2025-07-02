using System;
using Photon.Pun;

// Token: 0x020000C0 RID: 192
public class Action_ReduceUses : ItemAction
{
	// Token: 0x0600062C RID: 1580 RVA: 0x000218B5 File Offset: 0x0001FAB5
	public override void RunAction()
	{
		this.item.photonView.RPC("ReduceUsesRPC", RpcTarget.All, Array.Empty<object>());
	}

	// Token: 0x0600062D RID: 1581 RVA: 0x000218D4 File Offset: 0x0001FAD4
	[PunRPC]
	public void ReduceUsesRPC()
	{
		OptionableIntItemData data = this.item.GetData<OptionableIntItemData>(DataEntryKey.ItemUses);
		if (data.HasData && data.Value > 0)
		{
			data.Value--;
			if (this.item.totalUses > 0)
			{
				this.item.SetUseRemainingPercentage((float)data.Value / (float)this.item.totalUses);
			}
			if (data.Value == 0 && this.consumeOnFullyUsed && base.character && base.character.IsLocal && base.character.data.currentItem == this.item)
			{
				this.item.StartCoroutine(this.item.ConsumeDelayed(false));
			}
		}
	}

	// Token: 0x0400060D RID: 1549
	public bool consumeOnFullyUsed;
}
