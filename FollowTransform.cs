using System;
using UnityEngine;

// Token: 0x020001D1 RID: 465
public class FollowTransform : MonoBehaviour
{
	// Token: 0x06000C66 RID: 3174 RVA: 0x0003DA31 File Offset: 0x0003BC31
	private void LateUpdate()
	{
		if (this.t)
		{
			base.transform.position = this.t.position;
		}
	}

	// Token: 0x04000B64 RID: 2916
	public Transform t;
}
