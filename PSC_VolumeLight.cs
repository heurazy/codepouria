using System;
using UnityEngine;

// Token: 0x02000249 RID: 585
public class PSC_VolumeLight : PropSpawnerConstraint
{
	// Token: 0x06000E4F RID: 3663 RVA: 0x00047CF4 File Offset: 0x00045EF4
	public override bool CheckConstraint(PropSpawner.SpawnData spawnData)
	{
		Color color = LightVolume.Instance().SamplePosition(spawnData.pos);
		return color.a > this.minMax.x && color.a < this.minMax.y;
	}

	// Token: 0x04000D5C RID: 3420
	public Vector2 minMax = new Vector2(0f, 0.5f);
}
