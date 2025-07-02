using System;
using UnityEngine;

// Token: 0x020001BD RID: 445
public class DisableIKBehavior : StateMachineBehaviour
{
	// Token: 0x06000C1C RID: 3100 RVA: 0x0003C888 File Offset: 0x0003AA88
	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		animator.GetComponentInParent<Character>().data.overrideIKForSeconds = 0.1f;
	}
}
