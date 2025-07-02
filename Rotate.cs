using System;
using UnityEngine;

// Token: 0x0200025B RID: 603
public class Rotate : MonoBehaviour
{
	// Token: 0x06000E94 RID: 3732 RVA: 0x0004941D File Offset: 0x0004761D
	private void Update()
	{
		this.tf.transform.Rotate(this.rotation * Time.deltaTime);
	}

	// Token: 0x04000D8D RID: 3469
	public Transform tf;

	// Token: 0x04000D8E RID: 3470
	public Vector3 rotation;
}
