using System;
using UnityEngine;

// Token: 0x0200025C RID: 604
public class RotationSpring : MonoBehaviour
{
	// Token: 0x06000E96 RID: 3734 RVA: 0x00049448 File Offset: 0x00047648
	private void Update()
	{
		Transform parent = base.transform.parent;
		Vector3 forward = parent.forward;
		Vector3 up = parent.up;
		Vector3 vector = Vector3.Cross(base.transform.forward, forward).normalized * Vector3.Angle(base.transform.forward, forward);
		vector += Vector3.Cross(base.transform.up, up).normalized * Vector3.Angle(base.transform.up, up);
		this.vel = FRILerp.Lerp(this.vel, vector * this.spring, this.drag, true);
		base.transform.Rotate(this.vel * Time.deltaTime, Space.World);
	}

	// Token: 0x06000E97 RID: 3735 RVA: 0x00049513 File Offset: 0x00047713
	public void AddForce(Vector3 force)
	{
		this.vel += force;
	}

	// Token: 0x04000D8F RID: 3471
	public float spring;

	// Token: 0x04000D90 RID: 3472
	public float drag;

	// Token: 0x04000D91 RID: 3473
	private Vector3 vel;
}
