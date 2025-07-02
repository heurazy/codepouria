using System;
using UnityEngine;

// Token: 0x0200022E RID: 558
public class PSM_SetUpRotationToNormal : PropSpawnerMod
{
	// Token: 0x06000E1A RID: 3610 RVA: 0x00047174 File Offset: 0x00045374
	public override void ModifyObject(GameObject spawned, PropSpawner.SpawnData spawnData)
	{
		spawned.transform.rotation = Quaternion.Lerp(spawned.transform.rotation, HelperFunctions.GetRandomRotationWithUp(spawnData.normal), Mathf.Lerp(this.minEffect, this.maxEffect, Mathf.Pow(Random.value, this.randomPow)));
	}

	// Token: 0x04000D25 RID: 3365
	[Range(0f, 1f)]
	public float minEffect;

	// Token: 0x04000D26 RID: 3366
	[Range(0f, 1f)]
	public float maxEffect = 1f;

	// Token: 0x04000D27 RID: 3367
	public float randomPow = 1f;
}
