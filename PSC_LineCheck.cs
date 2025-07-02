using System;
using UnityEngine;

// Token: 0x02000240 RID: 576
public class PSC_LineCheck : PropSpawnerConstraint
{
	// Token: 0x06000E3D RID: 3645 RVA: 0x00047920 File Offset: 0x00045B20
	public override bool CheckConstraint(PropSpawner.SpawnData spawnData)
	{
		Vector3 vector = spawnData.hit.point + Vector3.Scale(spawnData.spawnerTransform.lossyScale, this.localStart);
		Vector3 vector2 = vector + Vector3.Scale(spawnData.spawnerTransform.localScale, this.localEnd);
		bool flag = !HelperFunctions.LineCheck(vector, vector2, HelperFunctions.LayerType.TerrainMap, 0f, QueryTriggerInteraction.Ignore).transform;
		Debug.DrawLine(vector, vector2, flag ? Color.green : Color.red, 10f);
		return flag;
	}

	// Token: 0x04000D4D RID: 3405
	public Vector3 localStart = new Vector3(0f, 0f, 0f);

	// Token: 0x04000D4E RID: 3406
	public Vector3 localEnd = new Vector3(0f, 5f, 0f);
}
