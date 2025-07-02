using System;
using UnityEngine;

// Token: 0x020001AB RID: 427
public class ClimbModifierSurface : MonoBehaviour
{
	// Token: 0x06000BE5 RID: 3045 RVA: 0x0003B9CC File Offset: 0x00039BCC
	public void OnClimb(Character character)
	{
		if (!this.applyStatus)
		{
			return;
		}
		if (!character.IsLocal)
		{
			return;
		}
		if (Time.time < this.lastTriggerTime + this.statusCooldown)
		{
			return;
		}
		character.refs.afflictions.AddStatus(this.statusType, this.statusAmount, false);
		this.lastTriggerTime = Time.time;
	}

	// Token: 0x06000BE6 RID: 3046 RVA: 0x0003BA29 File Offset: 0x00039C29
	public void OnClimbEnter()
	{
	}

	// Token: 0x06000BE7 RID: 3047 RVA: 0x0003BA2B File Offset: 0x00039C2B
	public void OnClimbExit()
	{
	}

	// Token: 0x04000AB3 RID: 2739
	public bool onlySlideDown;

	// Token: 0x04000AB4 RID: 2740
	public float speedMultiplier = 1f;

	// Token: 0x04000AB5 RID: 2741
	public float staminaUsageMultiplier = 1f;

	// Token: 0x04000AB6 RID: 2742
	public bool applyStatus;

	// Token: 0x04000AB7 RID: 2743
	public CharacterAfflictions.STATUSTYPE statusType;

	// Token: 0x04000AB8 RID: 2744
	public float statusAmount = 0.5f;

	// Token: 0x04000AB9 RID: 2745
	public float statusCooldown = 0.5f;

	// Token: 0x04000ABA RID: 2746
	private float lastTriggerTime;
}
