using System;
using UnityEngine;

// Token: 0x02000269 RID: 617
public class setright : MonoBehaviour
{
	// Token: 0x06000EE1 RID: 3809 RVA: 0x0004AD1B File Offset: 0x00048F1B
	private void Start()
	{
	}

	// Token: 0x06000EE2 RID: 3810 RVA: 0x0004AD1D File Offset: 0x00048F1D
	private void Update()
	{
	}

	// Token: 0x06000EE3 RID: 3811 RVA: 0x0004AD1F File Offset: 0x00048F1F
	public void go()
	{
		base.transform.right = this.right;
		base.transform.up = this.up;
	}

	// Token: 0x04000DBB RID: 3515
	public Vector3 right;

	// Token: 0x04000DBC RID: 3516
	public Vector3 up;
}
