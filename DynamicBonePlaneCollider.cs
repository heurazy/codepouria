using System;
using UnityEngine;

// Token: 0x02000078 RID: 120
[AddComponentMenu("Dynamic Bone/Dynamic Bone Plane Collider")]
public class DynamicBonePlaneCollider : DynamicBoneColliderBase
{
	// Token: 0x06000445 RID: 1093 RVA: 0x0001959C File Offset: 0x0001779C
	private void OnValidate()
	{
	}

	// Token: 0x06000446 RID: 1094 RVA: 0x000195A0 File Offset: 0x000177A0
	public override bool Collide(ref Vector3 particlePosition, float particleRadius)
	{
		Vector3 vector = Vector3.up;
		switch (this.m_Direction)
		{
		case DynamicBoneColliderBase.Direction.X:
			vector = base.transform.right;
			break;
		case DynamicBoneColliderBase.Direction.Y:
			vector = base.transform.up;
			break;
		case DynamicBoneColliderBase.Direction.Z:
			vector = base.transform.forward;
			break;
		}
		Vector3 vector2 = base.transform.TransformPoint(this.m_Center);
		Plane plane = new Plane(vector, vector2);
		float distanceToPoint = plane.GetDistanceToPoint(particlePosition);
		if (this.m_Bound == DynamicBoneColliderBase.Bound.Outside)
		{
			if (distanceToPoint < 0f)
			{
				particlePosition -= vector * distanceToPoint;
				return true;
			}
		}
		else if (distanceToPoint > 0f)
		{
			particlePosition -= vector * distanceToPoint;
			return true;
		}
		return false;
	}

	// Token: 0x06000447 RID: 1095 RVA: 0x00019670 File Offset: 0x00017870
	private void OnDrawGizmosSelected()
	{
		if (!base.enabled)
		{
			return;
		}
		if (this.m_Bound == DynamicBoneColliderBase.Bound.Outside)
		{
			Gizmos.color = Color.yellow;
		}
		else
		{
			Gizmos.color = Color.magenta;
		}
		Vector3 vector = Vector3.up;
		switch (this.m_Direction)
		{
		case DynamicBoneColliderBase.Direction.X:
			vector = base.transform.right;
			break;
		case DynamicBoneColliderBase.Direction.Y:
			vector = base.transform.up;
			break;
		case DynamicBoneColliderBase.Direction.Z:
			vector = base.transform.forward;
			break;
		}
		Vector3 vector2 = base.transform.TransformPoint(this.m_Center);
		Gizmos.DrawLine(vector2, vector2 + vector);
	}
}
