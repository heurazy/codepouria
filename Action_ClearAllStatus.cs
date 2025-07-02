using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000AF RID: 175
public class Action_ClearAllStatus : ItemAction
{
	// Token: 0x06000602 RID: 1538 RVA: 0x000212D0 File Offset: 0x0001F4D0
	public override void RunAction()
	{
		int num = Enum.GetNames(typeof(CharacterAfflictions.STATUSTYPE)).Length;
		for (int i = 0; i < num; i++)
		{
			CharacterAfflictions.STATUSTYPE statustype = (CharacterAfflictions.STATUSTYPE)i;
			if ((!this.excludeCurse || statustype != CharacterAfflictions.STATUSTYPE.Curse) && !this.otherExclusions.Contains(statustype))
			{
				base.character.refs.afflictions.SubtractStatus(statustype, (float)Mathf.Abs(5), false);
			}
		}
	}

	// Token: 0x040005F6 RID: 1526
	public bool excludeCurse = true;

	// Token: 0x040005F7 RID: 1527
	public List<CharacterAfflictions.STATUSTYPE> otherExclusions = new List<CharacterAfflictions.STATUSTYPE>();
}
