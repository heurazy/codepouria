using System;
using System.Collections.Generic;
using System.Linq;
using pworld.Scripts.Extensions;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace Knot
{
	// Token: 0x020002D5 RID: 725
	public class TiedKnotVisualizer : MonoBehaviour
	{
		// Token: 0x060011F0 RID: 4592 RVA: 0x000589A2 File Offset: 0x00056BA2
		private void Awake()
		{
			this.lr = base.GetComponent<LineRenderer>();
		}

		// Token: 0x060011F1 RID: 4593 RVA: 0x000589B0 File Offset: 0x00056BB0
		public void Refresh()
		{
			this.Visualize(this.knot);
		}

		// Token: 0x060011F2 RID: 4594 RVA: 0x000589C0 File Offset: 0x00056BC0
		public void Go()
		{
			foreach (TiedKnotVisualizer.KnotPart knotPart in this.knot)
			{
				Debug.Log(string.Format("Quality: {0}, Position: {1}", knotPart.quality, knotPart.position));
			}
		}

		// Token: 0x060011F3 RID: 4595 RVA: 0x00058A34 File Offset: 0x00056C34
		public void Visualize(List<TiedKnotVisualizer.KnotPart> knot)
		{
			this.knot = knot;
			List<Vector3> list = knot.Select((TiedKnotVisualizer.KnotPart knotPoint) => knotPoint.position).ToList<Vector3>();
			if (!this.splineIt)
			{
				this.lr.positionCount = list.Count;
				this.lr.SetPositions(list.ToArray());
				return;
			}
			Spline spline = new Spline();
			spline.Knots = list.Select((Vector3 knotPoint) => new BezierKnot(knotPoint)).ToArray<BezierKnot>();
			List<Vector3> list2 = new List<Vector3>();
			float num = 1f / (float)this.count;
			for (int i = 0; i < this.count; i++)
			{
				float num2 = num * (float)i;
				float3 @float = spline.EvaluatePosition(num2);
				list2.Add(@float.PToV3());
			}
			this.lr.positionCount = this.count;
			this.lr.SetPositions(list2.ToArray());
		}

		// Token: 0x060011F4 RID: 4596 RVA: 0x00058B3D File Offset: 0x00056D3D
		private void Start()
		{
		}

		// Token: 0x060011F5 RID: 4597 RVA: 0x00058B3F File Offset: 0x00056D3F
		private void Update()
		{
		}

		// Token: 0x060011F6 RID: 4598 RVA: 0x00058B41 File Offset: 0x00056D41
		public void Clear()
		{
			this.knot.Clear();
			this.Refresh();
		}

		// Token: 0x04001051 RID: 4177
		private LineRenderer lr;

		// Token: 0x04001052 RID: 4178
		public int count;

		// Token: 0x04001053 RID: 4179
		public bool splineIt;

		// Token: 0x04001054 RID: 4180
		public List<TiedKnotVisualizer.KnotPart> knot = new List<TiedKnotVisualizer.KnotPart>();

		// Token: 0x020003D5 RID: 981
		public struct KnotPart
		{
			// Token: 0x0600152C RID: 5420 RVA: 0x000614CA File Offset: 0x0005F6CA
			public KnotPart(bool quality, Vector3 position, int part)
			{
				this.quality = quality;
				this.position = position;
				this.part = part;
			}

			// Token: 0x0400140B RID: 5131
			public bool quality;

			// Token: 0x0400140C RID: 5132
			public Vector3 position;

			// Token: 0x0400140D RID: 5133
			public int part;
		}
	}
}
