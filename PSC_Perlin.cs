using System;
using UnityEngine;

// Token: 0x02000247 RID: 583
public class PSC_Perlin : PropSpawnerConstraint
{
	// Token: 0x06000E4B RID: 3659 RVA: 0x00047BCC File Offset: 0x00045DCC
	public override bool CheckConstraint(PropSpawner.SpawnData spawnData)
	{
		float num = Mathf.PerlinNoise((spawnData.pos.x + 500f) * this.perlinSize * 0.1f, (spawnData.pos.z + 500f) * this.perlinSize * 0.1f);
		return num > this.minMax.x && num < this.minMax.y;
	}

	// Token: 0x04000D58 RID: 3416
	public float perlinSize = 10f;

	// Token: 0x04000D59 RID: 3417
	public Vector2 minMax = new Vector2(0f, 0.5f);
}
