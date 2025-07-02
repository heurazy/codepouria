using System;
using UnityEngine;

// Token: 0x02000050 RID: 80
[ExecuteInEditMode]
[DefaultExecutionOrder(100000)]
public class CameraQuad : MonoBehaviour
{
	// Token: 0x06000381 RID: 897 RVA: 0x00015344 File Offset: 0x00013544
	private void LateUpdate()
	{
		if (!this.cam)
		{
			this.cam = Camera.main;
		}
		float num = this.cam.nearClipPlane + this.distance;
		Vector3 vector = this.cam.ViewportToWorldPoint(new Vector3(0f, 0f, num));
		Vector3 vector2 = this.cam.ViewportToWorldPoint(new Vector3(0f, 1f, num));
		Vector3 vector3 = this.cam.ViewportToWorldPoint(new Vector3(1f, 0f, num));
		this.cam.ViewportToWorldPoint(new Vector3(1f, 1f, num));
		float num2 = Vector3.Distance(vector, vector2);
		float num3 = Vector3.Distance(vector, vector3);
		base.transform.localScale = new Vector3(num3, num2, 1f);
		base.transform.position = this.cam.transform.position + this.cam.transform.forward * num;
		base.transform.rotation = this.cam.transform.rotation;
	}

	// Token: 0x04000407 RID: 1031
	public float distance = 0.01f;

	// Token: 0x04000408 RID: 1032
	private Camera cam;
}
