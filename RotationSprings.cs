using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200025D RID: 605
public class RotationSprings : MonoBehaviour
{
	// Token: 0x06000E99 RID: 3737 RVA: 0x00049530 File Offset: 0x00047730
	private void Update()
	{
		Transform parent = base.transform.parent;
		Vector3 forward = parent.forward;
		Vector3 up = parent.up;
		Vector3 zero = Vector3.zero;
		for (int i = 0; i < this.springs.Count; i++)
		{
			this.springs[i].DoUpdate(forward, up);
		}
	}

	// Token: 0x06000E9A RID: 3738 RVA: 0x00049584 File Offset: 0x00047784
	public void AddForce(Vector3 force, float spring, float drag)
	{
		RotationSprings.RotationSpringInstance rotationSpringInstance = new RotationSprings.RotationSpringInstance();
		rotationSpringInstance.spring = spring;
		rotationSpringInstance.drag = drag;
		rotationSpringInstance.forward = base.transform.parent.forward;
		rotationSpringInstance.up = base.transform.parent.up;
	}

	// Token: 0x04000D92 RID: 3474
	public List<RotationSprings.RotationSpringInstance> springs = new List<RotationSprings.RotationSpringInstance>();

	// Token: 0x020003AE RID: 942
	[Serializable]
	public class RotationSpringInstance
	{
		// Token: 0x060014B1 RID: 5297 RVA: 0x000605C4 File Offset: 0x0005E7C4
		public void DoUpdate(Vector3 targetForward, Vector3 targetUp)
		{
			Vector3 vector = Vector3.Cross(this.forward, targetForward) * Vector3.Angle(this.forward, targetForward);
			vector += Vector3.Cross(this.up, targetUp) * Vector3.Angle(this.up, targetUp);
			this.vel = FRILerp.Lerp(this.vel, vector * this.spring, this.drag, true);
			this.forward = Quaternion.AngleAxis(Time.deltaTime * this.vel.magnitude, this.vel) * this.forward;
			this.up = Quaternion.AngleAxis(Time.deltaTime * this.vel.magnitude, this.vel) * this.up;
		}

		// Token: 0x0400138A RID: 5002
		public float spring;

		// Token: 0x0400138B RID: 5003
		public float drag;

		// Token: 0x0400138C RID: 5004
		public Vector3 vel;

		// Token: 0x0400138D RID: 5005
		public Vector3 forward;

		// Token: 0x0400138E RID: 5006
		public Vector3 up;
	}
}
