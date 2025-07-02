using System;
using UnityEngine;

// Token: 0x02000201 RID: 513
public class MoveTransform : MonoBehaviour
{
	// Token: 0x06000D4F RID: 3407 RVA: 0x00043216 File Offset: 0x00041416
	private void Update()
	{
		base.transform.position += this.move * Time.deltaTime;
	}

	// Token: 0x04000C77 RID: 3191
	public Vector3 move;
}
