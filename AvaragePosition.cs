using System;
using UnityEngine;

// Token: 0x0200018B RID: 395
public class AvaragePosition : MonoBehaviour
{
	// Token: 0x06000ADF RID: 2783 RVA: 0x00035C14 File Offset: 0x00033E14
	private void Update()
	{
		base.transform.position = (this.p1.position + this.p2.position) / 2f;
	}

	// Token: 0x040009EC RID: 2540
	public Transform p1;

	// Token: 0x040009ED RID: 2541
	public Transform p2;
}
