using System;
using UnityEngine;

// Token: 0x02000264 RID: 612
public class SetAnimatorBool : MonoBehaviour
{
	// Token: 0x06000ED5 RID: 3797 RVA: 0x0004AC1C File Offset: 0x00048E1C
	private void Update()
	{
		base.GetComponent<Animator>().SetBool(this.param, this.on);
	}

	// Token: 0x04000DB3 RID: 3507
	public string param = "Enabled";

	// Token: 0x04000DB4 RID: 3508
	public bool on;
}
