using System;
using UnityEngine;

// Token: 0x0200024B RID: 587
public class PSCP_LineCheck : PropSpawnerConstraintPost
{
	// Token: 0x06000E53 RID: 3667 RVA: 0x00047D60 File Offset: 0x00045F60
	public override bool CheckConstraint(GameObject spawned, PropSpawner.SpawnData spawnData)
	{
		return !HelperFunctions.LineCheck(spawned.transform.TransformPoint(this.localStart), spawned.transform.TransformPoint(this.localEnd), HelperFunctions.LayerType.TerrainMap, 0f, QueryTriggerInteraction.Ignore).transform;
	}

	// Token: 0x04000D5D RID: 3421
	public Vector3 localStart = new Vector3(0f, 0.1f, 0f);

	// Token: 0x04000D5E RID: 3422
	public Vector3 localEnd = new Vector3(0f, 5f, 0f);
}
