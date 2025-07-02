using System;
using UnityEngine;

// Token: 0x02000250 RID: 592
public class RadiusCheck : CustomSpawnCondition
{
	// Token: 0x06000E70 RID: 3696 RVA: 0x00048A5C File Offset: 0x00046C5C
	public override bool CheckCondition(PropSpawner.SpawnData data)
	{
		LayerMask mask = HelperFunctions.GetMask(this.layerType);
		Collider[] array = Physics.OverlapSphere(base.transform.position, this.radius, mask);
		return array == null || array.Length == 0;
	}

	// Token: 0x04000D6F RID: 3439
	public HelperFunctions.LayerType layerType;

	// Token: 0x04000D70 RID: 3440
	public float radius = 5f;
}
