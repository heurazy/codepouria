using System;
using pworld.Scripts.Extensions;
using UnityEngine;

// Token: 0x02000112 RID: 274
public class BrokenRopeCheck : CustomSpawnCondition
{
	// Token: 0x060007F9 RID: 2041 RVA: 0x0002A42C File Offset: 0x0002862C
	public override bool CheckCondition(PropSpawner.SpawnData data)
	{
		this.lastData = data;
		float num = this.estimatedMaxRopeLength / 40f;
		if (data == null)
		{
			data = this.lastData;
		}
		this.lastData = data;
		base.transform.rotation = ExtQuaternion.FromUpAndRightPrioUp(Vector3.down, data.hit.normal);
		Debug.Log(string.Format("Anchor {0}", this.anchor));
		if (this.anchor.anchorPoint == null)
		{
			this.anchor.anchorPoint = this.anchor.transform.Find("AnchorPoint");
		}
		Debug.Log(string.Format("anchorPoint {0}", this.anchor.anchorPoint));
		RaycastHit raycastHit;
		bool flag = new Ray(this.anchor.anchorPoint.transform.position, Vector3.down).Raycast(out raycastHit, HelperFunctions.LayerType.TerrainMap.ToLayerMask(), 10f);
		float num2 = Vector3.Distance(raycastHit.point, base.transform.position);
		if (num2 > num * 39f || !flag)
		{
			this.ropeAnchorWithRope.ropeSegmentLength = 39f;
		}
		else
		{
			this.ropeAnchorWithRope.ropeSegmentLength = num2 / num - 1f;
		}
		return true;
	}

	// Token: 0x04000779 RID: 1913
	[SerializeField]
	private RopeAnchor anchor;

	// Token: 0x0400077A RID: 1914
	[SerializeField]
	private RopeAnchorWithRope ropeAnchorWithRope;

	// Token: 0x0400077B RID: 1915
	private PropSpawner.SpawnData lastData;

	// Token: 0x0400077C RID: 1916
	public float estimatedMaxRopeLength = 17f;
}
