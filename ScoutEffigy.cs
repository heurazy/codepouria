using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Zorro.Core;

// Token: 0x020000E5 RID: 229
public class ScoutEffigy : Constructable
{
	// Token: 0x060006F6 RID: 1782 RVA: 0x00024894 File Offset: 0x00022A94
	protected override void Update()
	{
		if (this.item.holderCharacter)
		{
			if (!Character.PlayerIsDeadOrDown())
			{
				this.item.overrideUsability = Optionable<bool>.Some(false);
			}
			else
			{
				this.item.overrideUsability = Optionable<bool>.None;
			}
		}
		base.Update();
	}

	// Token: 0x060006F7 RID: 1783 RVA: 0x000248E4 File Offset: 0x00022AE4
	public override void FinishConstruction()
	{
		if (!this.constructing)
		{
			return;
		}
		if (this.currentPreview == null)
		{
			return;
		}
		List<Character> list = new List<Character>();
		foreach (Character character in Character.AllCharacters)
		{
			if (character.data.dead || character.data.fullyPassedOut)
			{
				list.Add(character);
			}
		}
		if (list.Count == 0)
		{
			return;
		}
		list.RandomSelection((Character c) => 1).photonView.RPC("RPCA_ReviveAtPosition", RpcTarget.All, new object[]
		{
			this.currentConstructHit.point + Vector3.up * 1f,
			false
		});
	}
}
