using System;
using UnityEngine;

// Token: 0x0200023E RID: 574
public class PSM_SetRandomMaterial : PropSpawnerMod
{
	// Token: 0x06000E39 RID: 3641 RVA: 0x000478CC File Offset: 0x00045ACC
	public override void ModifyObject(GameObject spawned, PropSpawner.SpawnData spawnData)
	{
		Renderer[] componentsInChildren = spawned.GetComponentsInChildren<Renderer>();
		Material material = this.mats[Random.Range(0, this.mats.Length)];
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].sharedMaterial = material;
		}
	}

	// Token: 0x04000D4C RID: 3404
	public Material[] mats;
}
