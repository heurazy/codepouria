using System;
using UnityEngine;

// Token: 0x020002A0 RID: 672
public class WaterFallConfig : CustomSpawnCondition
{
	// Token: 0x06001000 RID: 4096 RVA: 0x00051308 File Offset: 0x0004F508
	public override bool CheckCondition(PropSpawner.SpawnData data)
	{
		RaycastHit raycastHit = HelperFunctions.LineCheck(this.rayStart.position, this.rayEnd.position, HelperFunctions.LayerType.TerrainMap, 0f, QueryTriggerInteraction.Ignore);
		if (raycastHit.transform)
		{
			this.endRock.transform.position = raycastHit.point;
			MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
			this.mesh.GetPropertyBlock(materialPropertyBlock);
			materialPropertyBlock.SetFloat("_WorldPositionY", raycastHit.point.y);
			this.mesh.SetPropertyBlock(materialPropertyBlock);
		}
		return true;
	}

	// Token: 0x04000F0B RID: 3851
	public MeshRenderer mesh;

	// Token: 0x04000F0C RID: 3852
	public Transform endRock;

	// Token: 0x04000F0D RID: 3853
	public Transform rayStart;

	// Token: 0x04000F0E RID: 3854
	public Transform rayEnd;
}
