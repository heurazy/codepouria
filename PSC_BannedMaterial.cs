using System;
using UnityEngine;

// Token: 0x02000242 RID: 578
public class PSC_BannedMaterial : PropSpawnerConstraint
{
	// Token: 0x06000E41 RID: 3649 RVA: 0x00047A30 File Offset: 0x00045C30
	public override bool CheckConstraint(PropSpawner.SpawnData spawnData)
	{
		MeshRenderer componentInChildren = spawnData.hit.transform.GetComponentInChildren<MeshRenderer>();
		return !(componentInChildren != null) || componentInChildren.sharedMaterial != this.bannedMaterial;
	}

	// Token: 0x04000D51 RID: 3409
	public Material bannedMaterial;
}
