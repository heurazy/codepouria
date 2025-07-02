using System;
using System.Linq;
using pworld.Scripts.Extensions;
using UnityEngine;

// Token: 0x020001D9 RID: 473
public class IcicleCheck : CustomSpawnCondition
{
	// Token: 0x06000C8D RID: 3213 RVA: 0x0003E5F4 File Offset: 0x0003C7F4
	public override bool CheckCondition(PropSpawner.SpawnData data)
	{
		PropSpawner comp = base.GetComponentInParent<PropSpawner>();
		base.transform.localScale = this.minMaxScale.PRndRange().xxx();
		Vector3 vector = this.boxCollider.transform.TransformPoint(this.boxCollider.center);
		Vector3 vector2 = Vector3.Scale(this.boxCollider.transform.lossyScale, this.boxCollider.size) / 2f;
		if (!this.LineCheck())
		{
			return false;
		}
		Collider[] array = (from c in Physics.OverlapBox(vector, vector2, this.boxCollider.transform.rotation)
			where c.GetComponentInParent<PropSpawner>() != comp
			select c).ToArray<Collider>();
		foreach (Collider collider in array)
		{
			Debug.DrawLine(vector, collider.transform.position, Color.red);
		}
		base.transform.position += Vector2.Scale(base.transform.lossyScale, this.minMax).PRndRange().oxo();
		return array.Length == 0;
	}

	// Token: 0x06000C8E RID: 3214 RVA: 0x0003E724 File Offset: 0x0003C924
	public bool LineCheck()
	{
		Vector3 vector = base.transform.TransformPoint(this.localStart);
		Vector3 vector2 = base.transform.TransformPoint(this.localEnd);
		bool flag = !HelperFunctions.LineCheck(vector, vector2, HelperFunctions.LayerType.TerrainMap, 0f, QueryTriggerInteraction.Ignore).transform;
		Debug.DrawLine(vector, vector2, flag ? Color.green : Color.red, 10f);
		return flag;
	}

	// Token: 0x04000B89 RID: 2953
	public BoxCollider boxCollider;

	// Token: 0x04000B8A RID: 2954
	public Vector2 minMax;

	// Token: 0x04000B8B RID: 2955
	public Vector2 minMaxScale = new Vector2(1f, 1f);

	// Token: 0x04000B8C RID: 2956
	public Vector3 localStart = new Vector3(0f, 0f, 0f);

	// Token: 0x04000B8D RID: 2957
	public Vector3 localEnd = new Vector3(0f, 5f, 0f);
}
