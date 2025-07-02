using System;
using UnityEngine;

// Token: 0x02000232 RID: 562
public class PSM_NormalOffset : PropSpawnerMod
{
	// Token: 0x06000E22 RID: 3618 RVA: 0x000473BC File Offset: 0x000455BC
	public override void ModifyObject(GameObject spawned, PropSpawner.SpawnData spawnData)
	{
		spawned.transform.position += spawnData.normal * Mathf.Lerp(this.minOffset, this.maxOffset, Mathf.Pow(Random.value, this.randomPow));
	}

	// Token: 0x04000D2F RID: 3375
	public float minOffset;

	// Token: 0x04000D30 RID: 3376
	public float maxOffset = 2f;

	// Token: 0x04000D31 RID: 3377
	public float randomPow = 1f;
}
