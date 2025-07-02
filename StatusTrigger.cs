using System;
using Peak.Afflictions;
using UnityEngine;

// Token: 0x0200027D RID: 637
public class StatusTrigger : MonoBehaviour
{
	// Token: 0x06000F54 RID: 3924 RVA: 0x0004D7CF File Offset: 0x0004B9CF
	private void Update()
	{
		this.counter += Time.deltaTime;
	}

	// Token: 0x06000F55 RID: 3925 RVA: 0x0004D7E4 File Offset: 0x0004B9E4
	private void OnTriggerEnter(Collider other)
	{
		Character componentInParent = other.GetComponentInParent<Character>();
		if (componentInParent == null)
		{
			return;
		}
		if (!componentInParent.IsLocal)
		{
			return;
		}
		if (this.counter < this.cooldown)
		{
			return;
		}
		this.counter = 0f;
		if (this.addStatus)
		{
			componentInParent.refs.afflictions.AddStatus(this.statusType, this.statusAmount, false);
		}
		if (this.poisonOverTime)
		{
			componentInParent.refs.afflictions.AddAffliction(new Affliction_PoisonOverTime(this.poisonOverTimeDuration, this.poisonOverTimeDelay, this.poisonOverTimeAmountPerSecond), false);
		}
	}

	// Token: 0x04000E4B RID: 3659
	public float cooldown = 1f;

	// Token: 0x04000E4C RID: 3660
	public bool addStatus;

	// Token: 0x04000E4D RID: 3661
	public CharacterAfflictions.STATUSTYPE statusType;

	// Token: 0x04000E4E RID: 3662
	public float statusAmount = 0.05f;

	// Token: 0x04000E4F RID: 3663
	public bool poisonOverTime;

	// Token: 0x04000E50 RID: 3664
	public float poisonOverTimeDuration = 5f;

	// Token: 0x04000E51 RID: 3665
	public float poisonOverTimeDelay = 1f;

	// Token: 0x04000E52 RID: 3666
	public float poisonOverTimeAmountPerSecond = 0.01f;

	// Token: 0x04000E53 RID: 3667
	private float counter;
}
