using System;

// Token: 0x02000244 RID: 580
public class PSC_Height : PropSpawnerConstraint
{
	// Token: 0x06000E45 RID: 3653 RVA: 0x00047AB6 File Offset: 0x00045CB6
	public override bool CheckConstraint(PropSpawner.SpawnData spawnData)
	{
		return spawnData.pos.y > this.minHeight && spawnData.pos.y < this.maxHeight;
	}

	// Token: 0x04000D53 RID: 3411
	public float maxHeight = 10000f;

	// Token: 0x04000D54 RID: 3412
	public float minHeight = -10000f;
}
