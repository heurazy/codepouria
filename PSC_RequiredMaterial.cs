using System;
using UnityEngine;

// Token: 0x02000243 RID: 579
public class PSC_RequiredMaterial : PropSpawnerConstraint
{
	// Token: 0x06000E43 RID: 3651 RVA: 0x00047A74 File Offset: 0x00045C74
	public override bool CheckConstraint(PropSpawner.SpawnData spawnData)
	{
		MeshRenderer componentInChildren = spawnData.hit.transform.GetComponentInChildren<MeshRenderer>();
		return !(componentInChildren != null) || componentInChildren.sharedMaterial == this.bannedMaterial;
	}

	// Token: 0x04000D52 RID: 3410
	public Material bannedMaterial;
}
