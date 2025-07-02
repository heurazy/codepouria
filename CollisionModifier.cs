using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001AE RID: 430
public class CollisionModifier : MonoBehaviour
{
	// Token: 0x06000BED RID: 3053 RVA: 0x0003BBD8 File Offset: 0x00039DD8
	public void Collide(Character character, ContactPoint contactPoint)
	{
		CollisionModifier.<>c__DisplayClass9_0 CS$<>8__locals1 = new CollisionModifier.<>c__DisplayClass9_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.character = character;
		Action<Character, CollisionModifier> action = this.onCollide;
		if (action != null)
		{
			action(CS$<>8__locals1.character, this);
		}
		if (!this.applyEffects)
		{
			return;
		}
		if (this.characterList.Contains(CS$<>8__locals1.character))
		{
			return;
		}
		foreach (CollisionMod collisionMod in this.additionalMods)
		{
			CS$<>8__locals1.character.refs.afflictions.AddStatus(collisionMod.statusType, collisionMod.amount, false);
			CS$<>8__locals1.character.AddForce((CS$<>8__locals1.character.Center - contactPoint.point).normalized * collisionMod.knockback, 1f, 1f);
		}
		CS$<>8__locals1.character.refs.afflictions.AddStatus(this.statusType, this.damage, false);
		CS$<>8__locals1.character.AddForce(Vector3.Lerp((CS$<>8__locals1.character.Center - contactPoint.point).normalized, base.transform.forward, this.knockbackTowardsFwdVector) * this.knockback, 1f, 1f);
		base.StartCoroutine(CS$<>8__locals1.<Collide>g__IHoldPlayer|0());
	}

	// Token: 0x04000AC6 RID: 2758
	private List<Character> characterList = new List<Character>();

	// Token: 0x04000AC7 RID: 2759
	public bool applyEffects = true;

	// Token: 0x04000AC8 RID: 2760
	public CharacterAfflictions.STATUSTYPE statusType;

	// Token: 0x04000AC9 RID: 2761
	public float damage = 0.15f;

	// Token: 0x04000ACA RID: 2762
	public float cooldown = 1f;

	// Token: 0x04000ACB RID: 2763
	public float knockback = 20f;

	// Token: 0x04000ACC RID: 2764
	public float knockbackTowardsFwdVector;

	// Token: 0x04000ACD RID: 2765
	public List<CollisionMod> additionalMods = new List<CollisionMod>();

	// Token: 0x04000ACE RID: 2766
	public Action<Character, CollisionModifier> onCollide;
}
