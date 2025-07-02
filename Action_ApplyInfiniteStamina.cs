using System;
using Peak.Afflictions;
using UnityEngine;

// Token: 0x020000AB RID: 171
public class Action_ApplyInfiniteStamina : ItemAction
{
	// Token: 0x060005F8 RID: 1528 RVA: 0x00021189 File Offset: 0x0001F389
	public override void RunAction()
	{
		Debug.Log("Adding infinite stamina buff");
		base.character.refs.afflictions.AddAffliction(new Affliction_InfiniteStamina(this.buffTime), false);
	}

	// Token: 0x040005F1 RID: 1521
	public float buffTime;

	// Token: 0x040005F2 RID: 1522
	public float drowsyAmount = 0.25f;
}
