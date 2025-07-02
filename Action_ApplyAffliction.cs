using System;
using Peak.Afflictions;
using UnityEngine;

// Token: 0x020000A9 RID: 169
public class Action_ApplyAffliction : ItemAction
{
	// Token: 0x060005F3 RID: 1523 RVA: 0x00021144 File Offset: 0x0001F344
	public override void RunAction()
	{
		if (this.affliction == null)
		{
			Debug.LogError("Your affliction is null bro");
			return;
		}
		base.character.refs.afflictions.AddAffliction(this.affliction, false);
	}

	// Token: 0x040005F0 RID: 1520
	[SerializeReference]
	public Affliction affliction;
}
