using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000026 RID: 38
[Serializable]
public class RigPart
{
	// Token: 0x0400023A RID: 570
	[HideInInspector]
	public bool justCreated;

	// Token: 0x0400023B RID: 571
	public BodypartType partType;

	// Token: 0x0400023C RID: 572
	public float mass = 10f;

	// Token: 0x0400023D RID: 573
	public float spring = 10f;

	// Token: 0x0400023E RID: 574
	public Transform transform;

	// Token: 0x0400023F RID: 575
	public List<RigCreatorColliderData> colliders = new List<RigCreatorColliderData>();

	// Token: 0x04000240 RID: 576
	public RigCreatorRigidbody rigHandler;

	// Token: 0x04000241 RID: 577
	public Rigidbody rig;

	// Token: 0x04000242 RID: 578
	public ConfigurableJoint joint;

	// Token: 0x04000243 RID: 579
	public RigCreatorJoint jointHandler;
}
