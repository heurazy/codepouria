using System;
using UnityEngine;

// Token: 0x0200022F RID: 559
public class PSM_SetForwardRotationToNormal : PropSpawnerMod
{
	// Token: 0x06000E1C RID: 3612 RVA: 0x000471E8 File Offset: 0x000453E8
	public override void ModifyObject(GameObject spawned, PropSpawner.SpawnData spawnData)
	{
		spawned.transform.rotation = Quaternion.Lerp(spawned.transform.rotation, Quaternion.LookRotation(spawnData.normal), Mathf.Lerp(this.minEffect, this.maxEffect, Mathf.Pow(Random.value, this.randomPow)));
	}

	// Token: 0x04000D28 RID: 3368
	[Range(0f, 1f)]
	public float minEffect;

	// Token: 0x04000D29 RID: 3369
	[Range(0f, 1f)]
	public float maxEffect = 1f;

	// Token: 0x04000D2A RID: 3370
	public float randomPow = 1f;
}
