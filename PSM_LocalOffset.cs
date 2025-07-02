using System;
using UnityEngine;

// Token: 0x02000231 RID: 561
public class PSM_LocalOffset : PropSpawnerMod
{
	// Token: 0x06000E20 RID: 3616 RVA: 0x0004728C File Offset: 0x0004548C
	public override void ModifyObject(GameObject spawned, PropSpawner.SpawnData spawnData)
	{
		Vector3 vector = Vector3.zero;
		vector += spawned.transform.right * Mathf.Lerp(-this.offset.x, this.offset.x, Random.value) * Mathf.Pow(Random.value, this.randomPow);
		vector += spawned.transform.up * Mathf.Lerp(-this.offset.y, this.offset.y, Random.value) * Mathf.Pow(Random.value, this.randomPow);
		vector += spawned.transform.forward * Mathf.Lerp(-this.offset.z, this.offset.z, Random.value) * Mathf.Pow(Random.value, this.randomPow);
		spawned.transform.position += vector;
	}

	// Token: 0x04000D2B RID: 3371
	public Vector3 offset;

	// Token: 0x04000D2C RID: 3372
	[Range(0f, 1f)]
	public float minEffect;

	// Token: 0x04000D2D RID: 3373
	[Range(0f, 1f)]
	public float maxEffect = 1f;

	// Token: 0x04000D2E RID: 3374
	public float randomPow = 1f;
}
