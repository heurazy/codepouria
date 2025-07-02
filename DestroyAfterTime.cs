using System;
using UnityEngine;

// Token: 0x020001BA RID: 442
public class DestroyAfterTime : MonoBehaviour
{
	// Token: 0x06000C16 RID: 3094 RVA: 0x0003C836 File Offset: 0x0003AA36
	private void Start()
	{
		Object.Destroy(base.gameObject, this.time);
	}

	// Token: 0x04000B17 RID: 2839
	public float time = 3f;
}
