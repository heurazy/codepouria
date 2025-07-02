using System;
using UnityEngine;

// Token: 0x0200002E RID: 46
public struct RigidbodySyncData
{
	// Token: 0x06000294 RID: 660 RVA: 0x000117E3 File Offset: 0x0000F9E3
	public RigidbodySyncData(Rigidbody rig)
	{
		this.position = rig.position;
		this.rotation = rig.rotation;
	}

	// Token: 0x04000315 RID: 789
	public Vector3 position;

	// Token: 0x04000316 RID: 790
	public Quaternion rotation;
}
