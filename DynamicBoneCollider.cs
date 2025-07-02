using System;
using UnityEngine;

// Token: 0x02000076 RID: 118
[AddComponentMenu("Dynamic Bone/Dynamic Bone Collider")]
public class DynamicBoneCollider : DynamicBoneColliderBase
{
	// Token: 0x0600043B RID: 1083 RVA: 0x00018FE4 File Offset: 0x000171E4
	private void OnValidate()
	{
		this.m_Radius = Mathf.Max(this.m_Radius, 0f);
		this.m_Height = Mathf.Max(this.m_Height, 0f);
	}

	// Token: 0x0600043C RID: 1084 RVA: 0x00019014 File Offset: 0x00017214
	public override bool Collide(ref Vector3 particlePosition, float particleRadius)
	{
		float num = this.m_Radius * Mathf.Abs(base.transform.lossyScale.x);
		float num2 = this.m_Height * 0.5f - this.m_Radius;
		if (num2 <= 0f)
		{
			if (this.m_Bound == DynamicBoneColliderBase.Bound.Outside)
			{
				return DynamicBoneCollider.OutsideSphere(ref particlePosition, particleRadius, base.transform.TransformPoint(this.m_Center), num);
			}
			return DynamicBoneCollider.InsideSphere(ref particlePosition, particleRadius, base.transform.TransformPoint(this.m_Center), num);
		}
		else
		{
			Vector3 center = this.m_Center;
			Vector3 center2 = this.m_Center;
			switch (this.m_Direction)
			{
			case DynamicBoneColliderBase.Direction.X:
				center.x -= num2;
				center2.x += num2;
				break;
			case DynamicBoneColliderBase.Direction.Y:
				center.y -= num2;
				center2.y += num2;
				break;
			case DynamicBoneColliderBase.Direction.Z:
				center.z -= num2;
				center2.z += num2;
				break;
			}
			if (this.m_Bound == DynamicBoneColliderBase.Bound.Outside)
			{
				return DynamicBoneCollider.OutsideCapsule(ref particlePosition, particleRadius, base.transform.TransformPoint(center), base.transform.TransformPoint(center2), num);
			}
			return DynamicBoneCollider.InsideCapsule(ref particlePosition, particleRadius, base.transform.TransformPoint(center), base.transform.TransformPoint(center2), num);
		}
	}

	// Token: 0x0600043D RID: 1085 RVA: 0x00019158 File Offset: 0x00017358
	private static bool OutsideSphere(ref Vector3 particlePosition, float particleRadius, Vector3 sphereCenter, float sphereRadius)
	{
		float num = sphereRadius + particleRadius;
		float num2 = num * num;
		Vector3 vector = particlePosition - sphereCenter;
		float sqrMagnitude = vector.sqrMagnitude;
		if (sqrMagnitude > 0f && sqrMagnitude < num2)
		{
			float num3 = Mathf.Sqrt(sqrMagnitude);
			particlePosition = sphereCenter + vector * (num / num3);
			return true;
		}
		return false;
	}

	// Token: 0x0600043E RID: 1086 RVA: 0x000191B0 File Offset: 0x000173B0
	private static bool InsideSphere(ref Vector3 particlePosition, float particleRadius, Vector3 sphereCenter, float sphereRadius)
	{
		float num = sphereRadius - particleRadius;
		float num2 = num * num;
		Vector3 vector = particlePosition - sphereCenter;
		float sqrMagnitude = vector.sqrMagnitude;
		if (sqrMagnitude > num2)
		{
			float num3 = Mathf.Sqrt(sqrMagnitude);
			particlePosition = sphereCenter + vector * (num / num3);
			return true;
		}
		return false;
	}

	// Token: 0x0600043F RID: 1087 RVA: 0x00019200 File Offset: 0x00017400
	private static bool OutsideCapsule(ref Vector3 particlePosition, float particleRadius, Vector3 capsuleP0, Vector3 capsuleP1, float capsuleRadius)
	{
		float num = capsuleRadius + particleRadius;
		float num2 = num * num;
		Vector3 vector = capsuleP1 - capsuleP0;
		Vector3 vector2 = particlePosition - capsuleP0;
		float num3 = Vector3.Dot(vector2, vector);
		if (num3 <= 0f)
		{
			float sqrMagnitude = vector2.sqrMagnitude;
			if (sqrMagnitude > 0f && sqrMagnitude < num2)
			{
				float num4 = Mathf.Sqrt(sqrMagnitude);
				particlePosition = capsuleP0 + vector2 * (num / num4);
				return true;
			}
		}
		else
		{
			float sqrMagnitude2 = vector.sqrMagnitude;
			if (num3 >= sqrMagnitude2)
			{
				vector2 = particlePosition - capsuleP1;
				float sqrMagnitude3 = vector2.sqrMagnitude;
				if (sqrMagnitude3 > 0f && sqrMagnitude3 < num2)
				{
					float num5 = Mathf.Sqrt(sqrMagnitude3);
					particlePosition = capsuleP1 + vector2 * (num / num5);
					return true;
				}
			}
			else if (sqrMagnitude2 > 0f)
			{
				num3 /= sqrMagnitude2;
				vector2 -= vector * num3;
				float sqrMagnitude4 = vector2.sqrMagnitude;
				if (sqrMagnitude4 > 0f && sqrMagnitude4 < num2)
				{
					float num6 = Mathf.Sqrt(sqrMagnitude4);
					particlePosition += vector2 * ((num - num6) / num6);
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06000440 RID: 1088 RVA: 0x00019338 File Offset: 0x00017538
	private static bool InsideCapsule(ref Vector3 particlePosition, float particleRadius, Vector3 capsuleP0, Vector3 capsuleP1, float capsuleRadius)
	{
		float num = capsuleRadius - particleRadius;
		float num2 = num * num;
		Vector3 vector = capsuleP1 - capsuleP0;
		Vector3 vector2 = particlePosition - capsuleP0;
		float num3 = Vector3.Dot(vector2, vector);
		if (num3 <= 0f)
		{
			float sqrMagnitude = vector2.sqrMagnitude;
			if (sqrMagnitude > num2)
			{
				float num4 = Mathf.Sqrt(sqrMagnitude);
				particlePosition = capsuleP0 + vector2 * (num / num4);
				return true;
			}
		}
		else
		{
			float sqrMagnitude2 = vector.sqrMagnitude;
			if (num3 >= sqrMagnitude2)
			{
				vector2 = particlePosition - capsuleP1;
				float sqrMagnitude3 = vector2.sqrMagnitude;
				if (sqrMagnitude3 > num2)
				{
					float num5 = Mathf.Sqrt(sqrMagnitude3);
					particlePosition = capsuleP1 + vector2 * (num / num5);
					return true;
				}
			}
			else if (sqrMagnitude2 > 0f)
			{
				num3 /= sqrMagnitude2;
				vector2 -= vector * num3;
				float sqrMagnitude4 = vector2.sqrMagnitude;
				if (sqrMagnitude4 > num2)
				{
					float num6 = Mathf.Sqrt(sqrMagnitude4);
					particlePosition += vector2 * ((num - num6) / num6);
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06000441 RID: 1089 RVA: 0x0001944C File Offset: 0x0001764C
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
		float num = this.m_Radius * Mathf.Abs(base.transform.lossyScale.x);
		float num2 = this.m_Height * 0.5f - this.m_Radius;
		if (num2 <= 0f)
		{
			Gizmos.DrawWireSphere(base.transform.TransformPoint(this.m_Center), num);
			return;
		}
		Vector3 center = this.m_Center;
		Vector3 center2 = this.m_Center;
		switch (this.m_Direction)
		{
		case DynamicBoneColliderBase.Direction.X:
			center.x -= num2;
			center2.x += num2;
			break;
		case DynamicBoneColliderBase.Direction.Y:
			center.y -= num2;
			center2.y += num2;
			break;
		case DynamicBoneColliderBase.Direction.Z:
			center.z -= num2;
			center2.z += num2;
			break;
		}
		Gizmos.DrawWireSphere(base.transform.TransformPoint(center), num);
		Gizmos.DrawWireSphere(base.transform.TransformPoint(center2), num);
	}

	// Token: 0x04000496 RID: 1174
	[Tooltip("The radius of the sphere or capsule.")]
	public float m_Radius = 0.5f;

	// Token: 0x04000497 RID: 1175
	[Tooltip("The height of the capsule.")]
	public float m_Height;
}
