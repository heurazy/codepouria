using System;
using UnityEngine;

// Token: 0x02000203 RID: 515
public class MultipleGroundPoints : CustomSpawnCondition
{
	// Token: 0x06000D57 RID: 3415 RVA: 0x0004341C File Offset: 0x0004161C
	public override bool CheckCondition(PropSpawner.SpawnData data)
	{
		Transform transform = base.transform.Find("GroundPoints");
		for (int i = 0; i < transform.childCount; i++)
		{
			Transform child = transform.GetChild(i);
			RaycastHit raycastHit = HelperFunctions.LineCheck(child.position, child.position + Vector3.down * this.checkRange, this.layerType, 0f, QueryTriggerInteraction.Ignore);
			if (!raycastHit.transform)
			{
				return false;
			}
			if (Vector3.Angle(Vector3.up, raycastHit.normal) > this.maxAngle)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x04000C7D RID: 3197
	public HelperFunctions.LayerType layerType;

	// Token: 0x04000C7E RID: 3198
	public float maxAngle = 30f;

	// Token: 0x04000C7F RID: 3199
	public float checkRange = 5f;
}
