using System;
using UnityEngine;

// Token: 0x0200022C RID: 556
public class PSM_BakedVolumeLightModiferIntensity : PropSpawnerMod
{
	// Token: 0x06000E16 RID: 3606 RVA: 0x00047090 File Offset: 0x00045290
	public override void ModifyObject(GameObject spawned, PropSpawner.SpawnData spawnData)
	{
		BakedVolumeLight component = spawned.GetComponent<BakedVolumeLight>();
		if (!component)
		{
			return;
		}
		if (this.customIntensity)
		{
			component.intensity = this.intensity;
		}
		if (this.customColor)
		{
			component.color = this.color;
		}
	}

	// Token: 0x04000D1E RID: 3358
	public bool customColor;

	// Token: 0x04000D1F RID: 3359
	public Color color = new Color(0.86f, 0.56f, 0.04f, 0.87f);

	// Token: 0x04000D20 RID: 3360
	public bool customIntensity;

	// Token: 0x04000D21 RID: 3361
	public float intensity = 0.5f;
}
