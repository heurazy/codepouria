using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000289 RID: 649
public abstract class Transition : MonoBehaviour
{
	// Token: 0x06000F99 RID: 3993
	public abstract IEnumerator TransitionIn(float speed = 1f);

	// Token: 0x06000F9A RID: 3994
	public abstract IEnumerator TransitionOut(float speed = 1f);

	// Token: 0x04000E9E RID: 3742
	public TransitionType transitionType;
}
