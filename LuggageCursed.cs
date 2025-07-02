using System;
using UnityEngine;

// Token: 0x020000DE RID: 222
public class LuggageCursed : Luggage
{
	// Token: 0x060006D0 RID: 1744 RVA: 0x000239CC File Offset: 0x00021BCC
	public override void Interact_CastFinished(Character interactor)
	{
		if (!interactor.IsLocal)
		{
			return;
		}
		float num = (float)Random.Range(this.minCurse, this.maxCurse + 1) * 0.025f;
		if (num > 0f)
		{
			interactor.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Curse, num, false);
		}
		interactor.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Injury, this.injuryAmt, false);
		base.Interact_CastFinished(interactor);
	}

	// Token: 0x04000663 RID: 1635
	public int minCurse;

	// Token: 0x04000664 RID: 1636
	public int maxCurse;

	// Token: 0x04000665 RID: 1637
	public float injuryAmt;
}
