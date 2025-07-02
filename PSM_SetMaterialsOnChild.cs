using System;
using System.Collections.Generic;
using UnityEngine;
using Zorro.Core;

// Token: 0x0200023C RID: 572
public class PSM_SetMaterialsOnChild : PropSpawnerMod
{
	// Token: 0x06000E36 RID: 3638 RVA: 0x000477F0 File Offset: 0x000459F0
	public override void ModifyObject(GameObject spawned, PropSpawner.SpawnData spawnData)
	{
		List<Renderer> rends = new List<Renderer>();
		spawned.transform.FindChildrenRecursive(this.childName).ForEach(delegate(Transform c)
		{
			rends.AddRange(c.GetComponentsInChildren<Renderer>());
		});
		for (int i = 0; i < rends.Count; i++)
		{
			Material[] sharedMaterials = rends[i].sharedMaterials;
			for (int j = 0; j < sharedMaterials.Length; j++)
			{
				foreach (MatAndID matAndID in this.edits)
				{
					if (matAndID.id == j)
					{
						sharedMaterials[j] = matAndID.mat;
					}
				}
			}
			rends[i].sharedMaterials = sharedMaterials;
		}
	}

	// Token: 0x04000D48 RID: 3400
	public string childName;

	// Token: 0x04000D49 RID: 3401
	public MatAndID[] edits;
}
