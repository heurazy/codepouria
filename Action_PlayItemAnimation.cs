using System;
using UnityEngine;

// Token: 0x020000BF RID: 191
public class Action_PlayItemAnimation : ItemAction
{
	// Token: 0x0600062A RID: 1578 RVA: 0x00021894 File Offset: 0x0001FA94
	public override void RunAction()
	{
		this.anim.Play(this.animationName, 0, 0f);
	}

	// Token: 0x0400060B RID: 1547
	public Animator anim;

	// Token: 0x0400060C RID: 1548
	public string animationName;
}
