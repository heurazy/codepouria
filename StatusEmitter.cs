using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x0200027B RID: 635
public class StatusEmitter : MonoBehaviour
{
	// Token: 0x06000F4D RID: 3917 RVA: 0x0004D41C File Offset: 0x0004B61C
	public void Update()
	{
		this.timeSinceLastTick += Time.deltaTime;
		if (this.timeSinceLastTick < this.tickTime)
		{
			return;
		}
		foreach (CharacterAfflictions characterAfflictions in from c in new HashSet<CharacterAfflictions>(from hit in Physics.OverlapSphere(base.transform.position, this.radius + this.outerFade)
				select hit.GetComponentInParent<CharacterAfflictions>())
			where c != null
			where c.character.photonView.IsMine
			select c)
		{
			float num = this.amount;
			if (this.outerFade > 0.01f)
			{
				float num2 = Vector3.Distance(characterAfflictions.character.Center, base.transform.position);
				num *= Mathf.InverseLerp(this.radius + this.outerFade, num2, num2);
			}
			if (num > 0f)
			{
				characterAfflictions.AddStatus(this.statusType, this.amount * this.timeSinceLastTick, false);
			}
			if (num < 0f)
			{
				characterAfflictions.SubtractStatus(this.statusType, Mathf.Abs(this.amount * this.timeSinceLastTick), false);
			}
		}
		this.timeSinceLastTick = 0f;
	}

	// Token: 0x06000F4E RID: 3918 RVA: 0x0004D5B0 File Offset: 0x0004B7B0
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(base.transform.position, this.radius);
		Gizmos.color = new Color(1f, 0f, 0f, 0.2f);
		Gizmos.DrawWireSphere(base.transform.position, this.radius + this.outerFade);
	}

	// Token: 0x06000F4F RID: 3919 RVA: 0x0004D618 File Offset: 0x0004B818
	private void Start()
	{
	}

	// Token: 0x04000E3C RID: 3644
	public CharacterAfflictions.STATUSTYPE statusType;

	// Token: 0x04000E3D RID: 3645
	public float amount;

	// Token: 0x04000E3E RID: 3646
	public float radius = 1f;

	// Token: 0x04000E3F RID: 3647
	public float outerFade;

	// Token: 0x04000E40 RID: 3648
	private float timeSinceLastTick;

	// Token: 0x04000E41 RID: 3649
	private float tickTime = 0.5f;
}
