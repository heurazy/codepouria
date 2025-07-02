using System;
using UnityEngine;

// Token: 0x02000233 RID: 563
public class PSM_RayDirectionOffset : PropSpawnerMod
{
	// Token: 0x06000E24 RID: 3620 RVA: 0x0004742C File Offset: 0x0004562C
	public override void ModifyObject(GameObject spawned, PropSpawner.SpawnData spawnData)
	{
		spawned.transform.position += spawnData.rayDir * Mathf.Lerp(this.minOffset, this.maxOffset, Mathf.Pow(Random.value, this.randomPow));
	}

	// Token: 0x04000D32 RID: 3378
	public float minOffset;

	// Token: 0x04000D33 RID: 3379
	public float maxOffset = 5f;

	// Token: 0x04000D34 RID: 3380
	public float randomPow = 1f;
}
