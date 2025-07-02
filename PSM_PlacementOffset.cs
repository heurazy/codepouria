using System;
using UnityEngine;

// Token: 0x02000234 RID: 564
public class PSM_PlacementOffset : PropSpawnerMod
{
	// Token: 0x06000E26 RID: 3622 RVA: 0x0004749C File Offset: 0x0004569C
	public override void ModifyObject(GameObject spawned, PropSpawner.SpawnData spawnData)
	{
		float num = Mathf.Lerp(this.minHeight.x, this.maxHeight.x, spawnData.placement.x);
		float num2 = Mathf.Lerp(this.minHeight.y, this.maxHeight.y, spawnData.placement.y);
		spawned.transform.position += Vector3.right * (num + num2) * this.xMult;
		spawned.transform.position += Vector3.up * (num + num2) * this.yMult;
		spawned.transform.position += Vector3.forward * (num + num2) * this.zMult;
	}

	// Token: 0x04000D35 RID: 3381
	public float xMult;

	// Token: 0x04000D36 RID: 3382
	public float yMult = 1f;

	// Token: 0x04000D37 RID: 3383
	public float zMult;

	// Token: 0x04000D38 RID: 3384
	public Vector2 minHeight;

	// Token: 0x04000D39 RID: 3385
	public Vector2 maxHeight;
}
