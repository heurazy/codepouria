using System;
using System.Collections.Generic;
using Photon.Pun;
using Unity.Mathematics;
using UnityEngine;

// Token: 0x02000118 RID: 280
public class RopeClimbingAPI : MonoBehaviour
{
	// Token: 0x06000835 RID: 2101 RVA: 0x0002BC6B File Offset: 0x00029E6B
	private void Awake()
	{
		this.rope = base.GetComponent<Rope>();
		this.photonView = base.GetComponentInParent<PhotonView>();
	}

	// Token: 0x06000836 RID: 2102 RVA: 0x0002BC85 File Offset: 0x00029E85
	public float GetMove()
	{
		return -1f * (1f / this.rope.GetTotalLength());
	}

	// Token: 0x06000837 RID: 2103 RVA: 0x0002BC9E File Offset: 0x00029E9E
	public float GetPercentFromSegmentIndex(int segmentIndex)
	{
		return (float)segmentIndex / ((float)this.rope.SegmentCount - 1f);
	}

	// Token: 0x06000838 RID: 2104 RVA: 0x0002BCB8 File Offset: 0x00029EB8
	public float GetAngleAtPercent(float percent)
	{
		Transform segmentFromPercent = this.GetSegmentFromPercent(percent);
		Debug.DrawLine(segmentFromPercent.transform.position, segmentFromPercent.transform.position + segmentFromPercent.up, Color.red);
		return segmentFromPercent.GetComponent<RopeSegment>().GetAngle();
	}

	// Token: 0x06000839 RID: 2105 RVA: 0x0002BD04 File Offset: 0x00029F04
	public Matrix4x4 GetSegmentMatrixFromPercent(float percent)
	{
		int num = Mathf.RoundToInt(Mathf.Lerp(0f, (float)(this.rope.SegmentCount - 1), percent));
		Transform transform = this.rope.GetRopeSegments()[num];
		return Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
	}

	// Token: 0x0600083A RID: 2106 RVA: 0x0002BD58 File Offset: 0x00029F58
	public Vector3 GetUp(float ropePercent)
	{
		Transform segmentFromPercent = this.GetSegmentFromPercent(ropePercent);
		Vector3 vector = segmentFromPercent.up;
		if (Vector3.Angle(Vector3.up, segmentFromPercent.up) > 90f)
		{
			vector *= -1f;
		}
		return vector;
	}

	// Token: 0x0600083B RID: 2107 RVA: 0x0002BD98 File Offset: 0x00029F98
	public float UpMult(float percent)
	{
		return (float)((Vector3.Angle(Vector3.up, this.GetSegmentFromPercent(percent).up) < 90f) ? (-1) : 1);
	}

	// Token: 0x0600083C RID: 2108 RVA: 0x0002BDBC File Offset: 0x00029FBC
	public Vector3 GetPosition(float percent)
	{
		percent = Mathf.Clamp01(percent);
		float num = percent * (float)(this.rope.SegmentCount - 1);
		int num2 = Mathf.FloorToInt(num);
		int num3 = num2;
		if (num2 == 0)
		{
			num2 = 1;
		}
		if (percent < 1f)
		{
			num3 = num2 + 1;
		}
		float num4 = num - (float)num2;
		List<Transform> ropeSegments = this.rope.GetRopeSegments();
		num2 = math.clamp(num2, 0, ropeSegments.Count - 1);
		num3 = math.clamp(num3, num2, ropeSegments.Count - 1);
		return Vector3.Lerp(ropeSegments[num2].position, ropeSegments[num3].position, num4);
	}

	// Token: 0x0600083D RID: 2109 RVA: 0x0002BE4C File Offset: 0x0002A04C
	public Transform GetSegmentFromPercent(float percent)
	{
		percent = Mathf.Clamp01(percent);
		float num = percent * (float)(this.rope.SegmentCount - 1);
		int num2 = Mathf.FloorToInt(num);
		int num3 = num2;
		if (num2 == 0)
		{
			num2 = 1;
		}
		if (percent < 1f)
		{
			num3 = num2 + 1;
		}
		float num4 = num - (float)num2;
		List<Transform> ropeSegments = this.rope.GetRopeSegments();
		num2 = math.clamp(num2, 0, ropeSegments.Count - 1);
		num3 = math.clamp(num3, num2, ropeSegments.Count - 1);
		return ropeSegments[(num4 > 0.5f) ? num3 : num2];
	}

	// Token: 0x040007B4 RID: 1972
	private Rope rope;

	// Token: 0x040007B5 RID: 1973
	private PhotonView photonView;
}
