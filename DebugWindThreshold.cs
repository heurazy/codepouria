using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000080 RID: 128
public class DebugWindThreshold : MonoBehaviour
{
	// Token: 0x0600048A RID: 1162 RVA: 0x0001A770 File Offset: 0x00018970
	public void GenerateMap()
	{
		this.ClearMap();
		this.min = this.zone.bounds.min;
		this.max = this.zone.bounds.max;
		Vector3 vector = this.min;
		while (vector.z < this.max.z)
		{
			while (vector.y < this.max.y)
			{
				while (vector.x < this.max.x)
				{
					this.nodes.Add(new DebugWindThreshold.WindNode(vector));
					vector.x += this.nodeSpacing;
				}
				vector.y += this.nodeSpacing;
				vector.x = this.min.x;
			}
			vector.z += this.nodeSpacing;
			vector.y = this.min.y;
			vector.x = this.min.x;
		}
	}

	// Token: 0x0600048B RID: 1163 RVA: 0x0001A874 File Offset: 0x00018A74
	public void ClearMap()
	{
		this.nodes.Clear();
	}

	// Token: 0x0600048C RID: 1164 RVA: 0x0001A884 File Offset: 0x00018A84
	private void OnDrawGizmosSelected()
	{
		for (int i = 0; i < this.nodes.Count; i++)
		{
			float num;
			if (this.nodes[i].wind > this.lowerThreshold + this.thresholdMargin)
			{
				num = 1f;
			}
			else if (this.nodes[i].wind < this.lowerThreshold)
			{
				num = 0f;
			}
			else
			{
				num = Util.RangeLerp(0f, 1f, this.lowerThreshold, this.lowerThreshold + this.thresholdMargin, this.nodes[i].wind, true, null);
			}
			this.nodes[i].DrawGizmo_HeatMap(num);
		}
	}

	// Token: 0x040004C3 RID: 1219
	[Range(0f, 1f)]
	public float lowerThreshold;

	// Token: 0x040004C4 RID: 1220
	[Range(0f, 1f)]
	public float thresholdMargin;

	// Token: 0x040004C5 RID: 1221
	public Collider zone;

	// Token: 0x040004C6 RID: 1222
	public float nodeSpacing = 5f;

	// Token: 0x040004C7 RID: 1223
	public const float MIN_NODE_SPACING = 2f;

	// Token: 0x040004C8 RID: 1224
	public List<DebugWindThreshold.WindNode> nodes = new List<DebugWindThreshold.WindNode>();

	// Token: 0x040004C9 RID: 1225
	public Vector3 min;

	// Token: 0x040004CA RID: 1226
	public Vector3 max;

	// Token: 0x0200030A RID: 778
	[Serializable]
	public class WindNode
	{
		// Token: 0x060012AE RID: 4782 RVA: 0x0005AC19 File Offset: 0x00058E19
		public WindNode(Vector3 position)
		{
			this.position = position;
			this.wind = LightVolume.Instance().SamplePositionAlpha(position);
		}

		// Token: 0x060012AF RID: 4783 RVA: 0x0005AC3C File Offset: 0x00058E3C
		public void DrawGizmo_HeatMap(float amt)
		{
			Color color;
			if (amt == 1f)
			{
				color = Color.red;
			}
			else if (amt == 0f)
			{
				color = Color.green;
			}
			else
			{
				color = Color.Lerp(Color.yellow, Color.red, amt);
			}
			color.a = 0.5f;
			Gizmos.color = color;
			Gizmos.DrawSphere(this.position, 1f);
		}

		// Token: 0x0400112C RID: 4396
		public float wind;

		// Token: 0x0400112D RID: 4397
		public Vector3 position;
	}
}
