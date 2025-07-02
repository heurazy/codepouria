using System;
using UnityEngine;

// Token: 0x020001BC RID: 444
public class DestroyObject : MonoBehaviour
{
	// Token: 0x06000C1A RID: 3098 RVA: 0x0003C873 File Offset: 0x0003AA73
	public void DestroySelf()
	{
		Object.Destroy(base.gameObject);
	}
}
