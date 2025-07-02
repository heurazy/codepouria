using System;
using System.Collections.Generic;
using UnityEngine;
using Zorro.Core;

// Token: 0x0200023B RID: 571
public class PSM_SetMaterialOnChild : PropSpawnerMod
{
	// Token: 0x06000E34 RID: 3636 RVA: 0x00047778 File Offset: 0x00045978
	public override void ModifyObject(GameObject spawned, PropSpawner.SpawnData spawnData)
	{
		List<Renderer> rends = new List<Renderer>();
		spawned.transform.FindChildrenRecursive(this.childName).ForEach(delegate(Transform c)
		{
			rends.AddRange(c.GetComponentsInChildren<Renderer>());
		});
		for (int i = 0; i < rends.Count; i++)
		{
			rends[i].sharedMaterial = this.mat;
		}
	}

	// Token: 0x04000D46 RID: 3398
	public string childName;

	// Token: 0x04000D47 RID: 3399
	public Material mat;
}
