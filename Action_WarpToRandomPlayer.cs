using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x020000C6 RID: 198
public class Action_WarpToRandomPlayer : ItemAction
{
	// Token: 0x0600063C RID: 1596 RVA: 0x00021BCC File Offset: 0x0001FDCC
	public override void RunAction()
	{
		for (int i = 0; i < this.warpSFX.Length; i++)
		{
			this.warpSFX[i].Play(default(Vector3));
		}
		List<Character> list = new List<Character>();
		foreach (Character character in Character.AllCharacters)
		{
			if (!(character == base.character) && !character.data.dead && Vector3.Distance(base.character.Center, character.Center) > this.minimumDistance)
			{
				list.Add(character);
			}
		}
		if (list.Count == 0 && this.restoreUsesOnFailure)
		{
			this.item.photonView.RPC("IncreaseUsesRPC", RpcTarget.All, Array.Empty<object>());
			return;
		}
		Vector3 center = list.RandomSelection((Character c) => 1).Center;
		base.character.photonView.RPC("WarpPlayerRPC", RpcTarget.All, new object[] { center, true });
	}

	// Token: 0x0600063D RID: 1597 RVA: 0x00021D14 File Offset: 0x0001FF14
	[PunRPC]
	public void IncreaseUsesRPC()
	{
		OptionableIntItemData data = this.item.GetData<OptionableIntItemData>(DataEntryKey.ItemUses);
		if (data.HasData && data.Value != -1)
		{
			data.Value++;
			if (this.item.totalUses > 0)
			{
				this.item.SetUseRemainingPercentage((float)data.Value / (float)this.item.totalUses);
			}
		}
	}

	// Token: 0x04000615 RID: 1557
	public float minimumDistance = 12f;

	// Token: 0x04000616 RID: 1558
	public bool restoreUsesOnFailure = true;

	// Token: 0x04000617 RID: 1559
	public SFX_Instance[] warpSFX;
}
