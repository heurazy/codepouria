using System;
using UnityEngine;

// Token: 0x02000187 RID: 391
public class AnimatorValues : MonoBehaviour
{
	// Token: 0x06000ACC RID: 2764 RVA: 0x00035045 File Offset: 0x00033245
	private void Start()
	{
		this.anim = base.GetComponent<Animator>();
		this.cD = base.GetComponentInParent<CharacterData>();
		this.cI = base.GetComponentInParent<CharacterInput>();
	}

	// Token: 0x06000ACD RID: 2765 RVA: 0x0003506C File Offset: 0x0003326C
	private void Update()
	{
		this.anim.SetFloat("Input X", this.cI.movementInput.x);
		this.anim.SetFloat("Input Y", this.cI.movementInput.y);
		this.anim.SetBool("Is Grounded", this.cD.isGrounded);
		if (this.cI.sprintIsPressed)
		{
			this.anim.SetFloat("Sprint", 1f, 0.125f, Time.deltaTime);
		}
		if (!this.cI.sprintIsPressed)
		{
			this.anim.SetFloat("Sprint", 0f, 0.125f, Time.deltaTime);
		}
		this.anim.SetFloat("Velocity Y", this.cD.avarageVelocity.y);
	}

	// Token: 0x040009D2 RID: 2514
	private Animator anim;

	// Token: 0x040009D3 RID: 2515
	private CharacterData cD;

	// Token: 0x040009D4 RID: 2516
	private CharacterInput cI;
}
