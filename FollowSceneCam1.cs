using System;
using UnityEngine;

// Token: 0x020001D0 RID: 464
[ExecuteInEditMode]
public class FollowSceneCam1 : MonoBehaviour
{
	// Token: 0x06000C64 RID: 3172 RVA: 0x0003DA00 File Offset: 0x0003BC00
	private void OnDrawGizmosSelected()
	{
		if (Camera.current != null)
		{
			base.transform.position = Camera.current.transform.position;
		}
	}
}
