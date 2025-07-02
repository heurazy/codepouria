using System;
using UnityEngine;

// Token: 0x02000241 RID: 577
public class PSC_Normal : PropSpawnerConstraint
{
	// Token: 0x06000E3F RID: 3647 RVA: 0x000479E8 File Offset: 0x00045BE8
	public override bool CheckConstraint(PropSpawner.SpawnData spawnData)
	{
		float num = Vector3.Angle(Vector3.up, spawnData.normal);
		return num < this.maxAngle && num > this.minAngle;
	}

	// Token: 0x04000D4F RID: 3407
	public float minAngle;

	// Token: 0x04000D50 RID: 3408
	public float maxAngle = 50f;
}
