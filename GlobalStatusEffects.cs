using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001D5 RID: 469
public class GlobalStatusEffects : MonoBehaviour
{
	// Token: 0x06000C81 RID: 3201 RVA: 0x0003E237 File Offset: 0x0003C437
	private void Start()
	{
	}

	// Token: 0x06000C82 RID: 3202 RVA: 0x0003E23C File Offset: 0x0003C43C
	private void Update()
	{
		foreach (GlobalStatusEffects.Effect effect in this.effects)
		{
			foreach (Character character in PlayerHandler.GetAllPlayerCharacters())
			{
				character.refs.afflictions.AddStatus(effect.type, effect.amount / effect.inTime * Time.deltaTime, false);
			}
		}
	}

	// Token: 0x04000B7B RID: 2939
	public List<GlobalStatusEffects.Effect> effects = new List<GlobalStatusEffects.Effect>();

	// Token: 0x0200038B RID: 907
	[Serializable]
	public class Effect
	{
		// Token: 0x04001319 RID: 4889
		public CharacterAfflictions.STATUSTYPE type;

		// Token: 0x0400131A RID: 4890
		public float amount;

		// Token: 0x0400131B RID: 4891
		public float inTime = 60f;
	}
}
