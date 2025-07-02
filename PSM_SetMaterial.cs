using System;
using UnityEngine;

// Token: 0x0200023A RID: 570
public class PSM_SetMaterial : PropSpawnerMod
{
	// Token: 0x06000E32 RID: 3634 RVA: 0x00047740 File Offset: 0x00045940
	public override void ModifyObject(GameObject spawned, PropSpawner.SpawnData spawnData)
	{
		Renderer[] componentsInChildren = spawned.GetComponentsInChildren<Renderer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].sharedMaterial = this.mat;
		}
	}

	// Token: 0x04000D45 RID: 3397
	public Material mat;
}
