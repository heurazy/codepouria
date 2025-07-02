using System;
using UnityEngine;

// Token: 0x02000255 RID: 597
public class RemoveObject : MonoBehaviour
{
	// Token: 0x06000E7C RID: 3708 RVA: 0x00048C22 File Offset: 0x00046E22
	public void DestroySelf()
	{
		Object.Destroy(base.gameObject);
	}
}
