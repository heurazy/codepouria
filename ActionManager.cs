using System;
using UnityEngine;

// Token: 0x02000038 RID: 56
public class ActionManager : MonoBehaviour
{
	// Token: 0x060002D1 RID: 721 RVA: 0x00012733 File Offset: 0x00010933
	private void Start()
	{
		if (base.GetComponent<Animator>())
		{
			this.anim = base.GetComponent<Animator>();
		}
	}

	// Token: 0x060002D2 RID: 722 RVA: 0x00012750 File Offset: 0x00010950
	private void Update()
	{
		if (this.anim)
		{
			this.anim.SetBool("Jump Cancel", this.jumpCancel);
			this.anim.SetBool("Attack Cancel", this.attackCancel);
			this.anim.SetBool("Continuable", this.continuable);
			this.anim.SetBool("Fall Cancel", this.fallCancel);
			this.anim.SetBool("Dash Cancel", this.dashCancel);
			this.anim.SetBool("Crouch Cancel", this.crouchCancel);
			this.anim.SetBool("Special State", this.specialState);
			if (this.actionTimer <= 0f)
			{
				this.anim.SetBool("Action", false);
			}
			if (this.actionTimer > 0f)
			{
				this.anim.SetBool("Action", true);
			}
			if (this.edgeCaseTimer <= 0f)
			{
				this.anim.SetBool("Edge Case", false);
			}
			if (this.edgeCaseTimer > 0f)
			{
				this.anim.SetBool("Edge Case", true);
			}
		}
		this.actionTimer -= Time.deltaTime;
		this.edgeCaseTimer -= Time.deltaTime;
		if (this.actionTimer <= 0f)
		{
			this.actionTimer = 0f;
		}
		if (this.edgeCaseTimer <= 0f)
		{
			this.edgeCaseTimer = 0f;
		}
	}

	// Token: 0x0400036D RID: 877
	public float actionTimer;

	// Token: 0x0400036E RID: 878
	public float edgeCaseTimer;

	// Token: 0x0400036F RID: 879
	public Animator anim;

	// Token: 0x04000370 RID: 880
	public bool fallCancel = true;

	// Token: 0x04000371 RID: 881
	public bool jumpCancel = true;

	// Token: 0x04000372 RID: 882
	public bool attackCancel = true;

	// Token: 0x04000373 RID: 883
	public bool dashCancel = true;

	// Token: 0x04000374 RID: 884
	public bool crouchCancel = true;

	// Token: 0x04000375 RID: 885
	public bool continuable;

	// Token: 0x04000376 RID: 886
	public bool specialState;
}
