using System;
using UnityEngine;

// Token: 0x02000246 RID: 582
public class PSC_SameTypeDistance : PropSpawnerConstraint
{
	// Token: 0x06000E49 RID: 3657 RVA: 0x00047B18 File Offset: 0x00045D18
	public override bool CheckConstraint(PropSpawner.SpawnData spawnData)
	{
		int childCount = spawnData.spawnerTransform.childCount;
		for (int i = 0; i < childCount; i++)
		{
			Vector3 vector = spawnData.pos - spawnData.spawnerTransform.GetChild(i).position;
			vector.x /= this.axisMultipliers.x;
			vector.y /= this.axisMultipliers.y;
			vector.z /= this.axisMultipliers.z;
			if (vector.magnitude < this.minDistance)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x04000D56 RID: 3414
	public float minDistance = 5f;

	// Token: 0x04000D57 RID: 3415
	public Vector3 axisMultipliers = Vector3.one;
}
