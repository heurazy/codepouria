using System;
using UnityEngine;

// Token: 0x02000237 RID: 567
public class PSM_RandomScale : PropSpawnerMod
{
	// Token: 0x06000E2C RID: 3628 RVA: 0x00047681 File Offset: 0x00045881
	public override void ModifyObject(GameObject spawned, PropSpawner.SpawnData spawnData)
	{
		spawned.transform.localScale *= Mathf.Lerp(this.minScaleMult, this.maxScaleMult, Mathf.Pow(Random.value, this.randomPow));
	}

	// Token: 0x04000D3F RID: 3391
	public float minScaleMult;

	// Token: 0x04000D40 RID: 3392
	public float maxScaleMult = 2f;

	// Token: 0x04000D41 RID: 3393
	public float randomPow = 1f;
}
