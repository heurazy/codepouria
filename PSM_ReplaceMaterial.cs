using System;
using UnityEngine;

// Token: 0x02000236 RID: 566
public class PSM_ReplaceMaterial : PropSpawnerMod
{
	// Token: 0x06000E2A RID: 3626 RVA: 0x00047618 File Offset: 0x00045818
	public override void ModifyObject(GameObject spawned, PropSpawner.SpawnData spawnData)
	{
		foreach (Renderer renderer in spawned.GetComponentsInChildren<Renderer>())
		{
			Material[] sharedMaterials = renderer.sharedMaterials;
			for (int j = 0; j < sharedMaterials.Length; j++)
			{
				if (sharedMaterials[j] == this.replaceThis)
				{
					sharedMaterials[j] = this.withThis;
				}
			}
			renderer.sharedMaterials = sharedMaterials;
		}
	}

	// Token: 0x04000D3D RID: 3389
	public Material replaceThis;

	// Token: 0x04000D3E RID: 3390
	public Material withThis;
}
