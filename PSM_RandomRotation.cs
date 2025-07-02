using System;
using UnityEngine;

// Token: 0x0200022D RID: 557
public class PSM_RandomRotation : PropSpawnerMod
{
	// Token: 0x06000E18 RID: 3608 RVA: 0x00047108 File Offset: 0x00045308
	public override void ModifyObject(GameObject spawned, PropSpawner.SpawnData spawnData)
	{
		spawned.transform.rotation = Quaternion.Lerp(spawned.transform.rotation, Random.rotation, Mathf.Lerp(this.minRotation, this.maxRotation, Mathf.Pow(Random.value, this.randomPow)));
	}

	// Token: 0x04000D22 RID: 3362
	[Range(0f, 1f)]
	public float minRotation;

	// Token: 0x04000D23 RID: 3363
	[Range(0f, 1f)]
	public float maxRotation = 0.5f;

	// Token: 0x04000D24 RID: 3364
	public float randomPow = 1f;
}
