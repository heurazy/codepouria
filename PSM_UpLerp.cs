using System;
using UnityEngine;

// Token: 0x02000235 RID: 565
public class PSM_UpLerp : PropSpawnerMod
{
	// Token: 0x06000E28 RID: 3624 RVA: 0x00047594 File Offset: 0x00045794
	public override void ModifyObject(GameObject spawned, PropSpawner.SpawnData spawnData)
	{
		float num = Mathf.Pow(Random.value, this.randomPow);
		float num2 = Mathf.Lerp(this.minUpLerp, this.maxUpLerp, num);
		Vector3 vector = spawned.transform.up;
		vector = Vector3.Lerp(vector, Vector3.up, num2);
		spawned.transform.rotation = HelperFunctions.GetRotationWithUp(spawned.transform.forward, vector);
	}

	// Token: 0x04000D3A RID: 3386
	[Range(0f, 1f)]
	public float minUpLerp;

	// Token: 0x04000D3B RID: 3387
	[Range(0f, 1f)]
	public float maxUpLerp = 1f;

	// Token: 0x04000D3C RID: 3388
	public float randomPow = 1f;
}
