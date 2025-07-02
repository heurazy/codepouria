using System;
using UnityEngine;

// Token: 0x02000252 RID: 594
public class RandomAnimatorSpeed : MonoBehaviour
{
	// Token: 0x06000E74 RID: 3700 RVA: 0x00048ACF File Offset: 0x00046CCF
	private void Start()
	{
		this.anim = base.GetComponent<Animator>();
		this.anim.speed = Random.Range(this.minSpeed, this.maxSpeed);
	}

	// Token: 0x04000D72 RID: 3442
	private Animator anim;

	// Token: 0x04000D73 RID: 3443
	public float minSpeed = 0.5f;

	// Token: 0x04000D74 RID: 3444
	public float maxSpeed = 2f;
}
