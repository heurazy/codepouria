using System;
using UnityEngine;

// Token: 0x020001B1 RID: 433
public class CursedSkullVFX : ItemVFX
{
	// Token: 0x06000BF4 RID: 3060 RVA: 0x0003BE97 File Offset: 0x0003A097
	protected override void Start()
	{
		base.Start();
		this.curseParticles.Play();
		this.animator.enabled = true;
	}

	// Token: 0x04000AD6 RID: 2774
	public ParticleSystem curseParticles;

	// Token: 0x04000AD7 RID: 2775
	public Animator animator;
}
