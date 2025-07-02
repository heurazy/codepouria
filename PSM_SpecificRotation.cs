using System;
using UnityEngine;

// Token: 0x02000238 RID: 568
public class PSM_SpecificRotation : PropSpawnerMod
{
	// Token: 0x06000E2E RID: 3630 RVA: 0x000476D8 File Offset: 0x000458D8
	public override void ModifyObject(GameObject spawned, PropSpawner.SpawnData spawnData)
	{
		Vector3 vector = this.eulerAngles;
		if (this.random > 0f)
		{
			vector = Vector3.Lerp(vector, this.eulerAnglesRandom, Random.value * this.random);
		}
		spawned.transform.rotation = Quaternion.Euler(vector);
	}

	// Token: 0x04000D42 RID: 3394
	public Vector3 eulerAngles;

	// Token: 0x04000D43 RID: 3395
	[Range(0f, 1f)]
	public float random;

	// Token: 0x04000D44 RID: 3396
	public Vector3 eulerAnglesRandom;
}
