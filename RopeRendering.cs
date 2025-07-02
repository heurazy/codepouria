using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using pworld.Scripts.Extensions;
using Unity.Mathematics;
using UnityEngine;

// Token: 0x02000119 RID: 281
public class RopeRendering : MonoBehaviour
{
	// Token: 0x0600083F RID: 2111 RVA: 0x0002BED5 File Offset: 0x0002A0D5
	private void Awake()
	{
		this.view = base.GetComponentInParent<PhotonView>();
		this.rope = base.GetComponent<Rope>();
		this.lineRenderer = base.GetComponentInChildren<LineRenderer>();
	}

	// Token: 0x06000840 RID: 2112 RVA: 0x0002BEFC File Offset: 0x0002A0FC
	private void LateUpdate()
	{
		if (this.hide)
		{
			return;
		}
		if (this.targetPoints.Count != this.remoteRenderingPoints.Count)
		{
			Debug.LogError("Target points count mismatch");
			return;
		}
		float num = 1f / (float)PhotonNetwork.SerializationRate;
		this.sinceLastPackage += Time.deltaTime;
		float num2 = this.sinceLastPackage / num;
		for (int i = 0; i < this.remoteRenderingPoints.Count; i++)
		{
			Vector3 vector = Vector3.Lerp(this.remoteRenderingPoints[i], this.targetPoints[i], num2 * 0.5f);
			this.remoteRenderingPoints[i] = vector;
		}
		List<float3> list;
		if (!this.view.IsMine)
		{
			list = this.remoteRenderingPoints;
		}
		else
		{
			list = (from transform1 in this.rope.GetRopeSegments()
				select transform1.position).ToList<float3>();
		}
		List<float3> list2 = list;
		List<Vector3> list3 = new List<Vector3>();
		if (this.startPos != null && this.rope.attachmenState == Rope.ATTACHMENT.inSpool)
		{
			list3.Add(this.startPos.position);
		}
		if (list2.Count > 0)
		{
			list3.Add(list2[0]);
		}
		for (int j = list2.Count - 1; j >= 1; j--)
		{
			list3.Add(list2[j]);
		}
		this.lineRenderer.positionCount = list3.Count;
		this.lineRenderer.SetPositions(list3.ToArray());
		if (this.rope.IsActive() && list3.Count > 1)
		{
			this.lineRenderer.enabled = true;
			return;
		}
		this.lineRenderer.enabled = false;
	}

	// Token: 0x06000841 RID: 2113 RVA: 0x0002C0D4 File Offset: 0x0002A2D4
	public void SetData(RopeSyncData data)
	{
		this.sinceLastPackage = 0f;
		this.targetPoints = data.segments.Select((RopeSyncData.SegmentData segmentData) => segmentData.position).ToList<float3>();
		int num = data.segments.Length;
		int count = this.remoteRenderingPoints.Count;
		if (num < count)
		{
			int num2 = count - num;
			for (int i = 0; i < num2; i++)
			{
				this.remoteRenderingPoints.RemoveLast<float3>();
			}
			return;
		}
		if (num > count)
		{
			int num3 = num - count;
			for (int j = 0; j < num3; j++)
			{
				int num4 = count + j;
				this.remoteRenderingPoints.Add(data.segments[num4].position);
			}
		}
	}

	// Token: 0x040007B6 RID: 1974
	public Transform startPos;

	// Token: 0x040007B7 RID: 1975
	public LineRenderer lineRenderer;

	// Token: 0x040007B8 RID: 1976
	private Rope rope;

	// Token: 0x040007B9 RID: 1977
	private PhotonView view;

	// Token: 0x040007BA RID: 1978
	private List<float3> remoteRenderingPoints = new List<float3>();

	// Token: 0x040007BB RID: 1979
	private List<float3> targetPoints = new List<float3>();

	// Token: 0x040007BC RID: 1980
	private float sinceLastPackage;

	// Token: 0x040007BD RID: 1981
	public bool hide = true;
}
