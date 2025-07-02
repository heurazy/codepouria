using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200027C RID: 636
public class StatusField : MonoBehaviour
{
	// Token: 0x06000F51 RID: 3921 RVA: 0x0004D638 File Offset: 0x0004B838
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(base.transform.position, this.radius);
	}

	// Token: 0x06000F52 RID: 3922 RVA: 0x0004D65C File Offset: 0x0004B85C
	public void Update()
	{
		if (!Character.localCharacter || Vector3.Distance(Character.localCharacter.Center, base.transform.position) > this.radius)
		{
			this.inflicting = false;
			return;
		}
		if (this.doNotApplyIfStatusesMaxed && Character.localCharacter.refs.afflictions.statusSum >= 1f)
		{
			this.inflicting = false;
			return;
		}
		Character.localCharacter.refs.afflictions.AdjustStatus(this.statusType, this.statusAmountPerSecond * Time.deltaTime, false);
		foreach (StatusField.StatusFieldStatus statusFieldStatus in this.additionalStatuses)
		{
			Character.localCharacter.refs.afflictions.AdjustStatus(statusFieldStatus.statusType, statusFieldStatus.statusAmountPerSecond * Time.deltaTime, false);
		}
		if (!this.inflicting && this.statusAmountOnEntry != 0f && Time.time - this.lastEnteredTime > this.entryCooldown)
		{
			Character.localCharacter.refs.afflictions.AdjustStatus(this.statusType, this.statusAmountOnEntry, false);
			this.lastEnteredTime = Time.time;
		}
		this.inflicting = true;
	}

	// Token: 0x04000E42 RID: 3650
	public CharacterAfflictions.STATUSTYPE statusType;

	// Token: 0x04000E43 RID: 3651
	public float statusAmountPerSecond;

	// Token: 0x04000E44 RID: 3652
	public float statusAmountOnEntry;

	// Token: 0x04000E45 RID: 3653
	public float radius;

	// Token: 0x04000E46 RID: 3654
	private float lastEnteredTime;

	// Token: 0x04000E47 RID: 3655
	public float entryCooldown = 1f;

	// Token: 0x04000E48 RID: 3656
	public bool doNotApplyIfStatusesMaxed;

	// Token: 0x04000E49 RID: 3657
	public List<StatusField.StatusFieldStatus> additionalStatuses;

	// Token: 0x04000E4A RID: 3658
	private bool inflicting;

	// Token: 0x020003B6 RID: 950
	[Serializable]
	public class StatusFieldStatus
	{
		// Token: 0x040013A8 RID: 5032
		public CharacterAfflictions.STATUSTYPE statusType;

		// Token: 0x040013A9 RID: 5033
		public float statusAmountPerSecond;
	}
}
