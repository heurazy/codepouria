using System;
using UnityEngine;

// Token: 0x02000077 RID: 119
public class DynamicBoneColliderBase : MonoBehaviour
{
	// Token: 0x06000443 RID: 1091 RVA: 0x0001957F File Offset: 0x0001777F
	public virtual bool Collide(ref Vector3 particlePosition, float particleRadius)
	{
		return false;
	}

	// Token: 0x04000498 RID: 1176
	[Tooltip("The axis of the capsule's height.")]
	public DynamicBoneColliderBase.Direction m_Direction = DynamicBoneColliderBase.Direction.Y;

	// Token: 0x04000499 RID: 1177
	[Tooltip("The center of the sphere or capsule, in the object's local space.")]
	public Vector3 m_Center = Vector3.zero;

	// Token: 0x0400049A RID: 1178
	[Tooltip("Constrain bones to outside bound or inside bound.")]
	public DynamicBoneColliderBase.Bound m_Bound;

	// Token: 0x02000307 RID: 775
	public enum Direction
	{
		// Token: 0x04001122 RID: 4386
		X,
		// Token: 0x04001123 RID: 4387
		Y,
		// Token: 0x04001124 RID: 4388
		Z
	}

	// Token: 0x02000308 RID: 776
	public enum Bound
	{
		// Token: 0x04001126 RID: 4390
		Outside,
		// Token: 0x04001127 RID: 4391
		Inside
	}
}
