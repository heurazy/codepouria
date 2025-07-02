using System;
using System.Collections.Generic;
using System.Linq;
using pworld.Scripts.Extensions;
using UnityEngine;
using UnityEngine.Splines;

namespace Knot
{
	// Token: 0x020002D0 RID: 720
	public class AlignWithSpline : MonoBehaviour
	{
		// Token: 0x170000D0 RID: 208
		// (get) Token: 0x060011CD RID: 4557 RVA: 0x00057780 File Offset: 0x00055980
		public float KnotStepSize
		{
			get
			{
				return this.knotProgressRange * 2f;
			}
		}

		// Token: 0x060011CE RID: 4558 RVA: 0x00057790 File Offset: 0x00055990
		public void DistanceToSpline(Vector3 position, out float closest, out float atSplineProgress)
		{
			position = position.xyo();
			int num = 200;
			float num2 = 1f / (float)num;
			closest = float.MaxValue;
			atSplineProgress = 0f;
			for (int i = 0; i < num; i++)
			{
				float num3 = num2 * (float)i;
				Vector3 vector = this.splineContainer.Spline.EvaluatePosition(num3).PToV3().xyo() - position;
				if (vector.magnitude < closest)
				{
					closest = vector.magnitude;
					atSplineProgress = num3;
				}
			}
		}

		// Token: 0x170000D1 RID: 209
		// (get) Token: 0x060011CF RID: 4559 RVA: 0x0005780D File Offset: 0x00055A0D
		public Vector2 KnotProgressRangeRelation
		{
			get
			{
				return this.knotProgressRangeRelation * this.knotProgressRange;
			}
		}

		// Token: 0x060011D0 RID: 4560 RVA: 0x00057820 File Offset: 0x00055A20
		private void EvaluateKnot(AlignWithSpline.TiedKnot tiedKnot)
		{
			float templateProgress = tiedKnot.knotPoints[0].templateProgress;
			Vector2 vector = this.KnotProgressRangeRelation;
			vector.x += this.knotProgress;
			vector.y += this.knotProgress;
			Vector2 vector2 = vector;
			vector2.x += this.KnotStepSize;
			vector2.y += this.KnotStepSize;
			if (templateProgress > vector.y && templateProgress > vector2.y)
			{
				Debug.LogError("");
			}
			float x = vector.x;
		}

		// Token: 0x060011D1 RID: 4561 RVA: 0x000578C0 File Offset: 0x00055AC0
		private void TieRope2()
		{
			Plane plane = new Plane(Camera.main.transform.forward, this.splineContainer.transform.position);
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			float num;
			if (!plane.Raycast(ray, out num))
			{
				return;
			}
			Vector3 vector = ray.direction * num + ray.origin;
			this.tiedKnot.knotPoints.Add(new AlignWithSpline.TiedKnot.KnotPoint
			{
				position = vector.xyo(),
				templateProgress = this.lastKnotPointProgress,
				inside = false
			});
		}

		// Token: 0x060011D2 RID: 4562 RVA: 0x00057960 File Offset: 0x00055B60
		private void TieRope()
		{
			Plane plane = new Plane(Camera.main.transform.forward, this.splineContainer.transform.position);
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit[] array = Physics.RaycastAll(ray);
			if (array.Length != 0)
			{
				IOrderedEnumerable<RaycastHit> orderedEnumerable = array.OrderBy((RaycastHit h) => Mathf.Abs(h.textureCoord.x - this.lastKnotPointProgress));
				foreach (RaycastHit raycastHit in orderedEnumerable)
				{
					Debug.Log(string.Format("{0} Hit: {1}", Time.frameCount, raycastHit.textureCoord.x));
				}
				RaycastHit raycastHit2 = orderedEnumerable.First<RaycastHit>();
				if (this.tiedKnot.knotPoints.Count > 0)
				{
					List<AlignWithSpline.TiedKnot.KnotPoint> knotPoints = this.tiedKnot.knotPoints;
					if (Vector3.Distance(knotPoints[knotPoints.Count - 1].position, raycastHit2.point) < this.minKnotPointDistance)
					{
						return;
					}
				}
				this.lastKnotPointProgress = raycastHit2.textureCoord.x;
				this.tiedKnot.knotPoints.Add(new AlignWithSpline.TiedKnot.KnotPoint
				{
					position = raycastHit2.point.xyo(),
					templateProgress = this.lastKnotPointProgress,
					inside = true
				});
				Debug.Log(string.Format("Added: {0}", raycastHit2.textureCoord.x));
				return;
			}
			float num;
			if (plane.Raycast(ray, out num))
			{
				Vector3 vector = ray.direction * num + ray.origin;
				this.tiedKnot.knotPoints.Add(new AlignWithSpline.TiedKnot.KnotPoint
				{
					position = vector.xyo(),
					templateProgress = this.lastKnotPointProgress,
					inside = false
				});
			}
		}

		// Token: 0x060011D3 RID: 4563 RVA: 0x00057B44 File Offset: 0x00055D44
		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Mouse0))
			{
				this.tiedKnot = new AlignWithSpline.TiedKnot();
			}
			else if (Input.GetKey(KeyCode.Mouse0))
			{
				this.TieRope();
			}
			Input.GetKeyUp(KeyCode.Mouse0);
		}

		// Token: 0x060011D4 RID: 4564 RVA: 0x00057B7C File Offset: 0x00055D7C
		private void FixedUpdate()
		{
		}

		// Token: 0x04001027 RID: 4135
		public SplineContainer splineContainer;

		// Token: 0x04001028 RID: 4136
		public float knotProgress;

		// Token: 0x04001029 RID: 4137
		public float minKnotPointDistance = 0.001f;

		// Token: 0x0400102A RID: 4138
		public float lastKnotPointProgress;

		// Token: 0x0400102B RID: 4139
		private AlignWithSpline.TiedKnot tiedKnot = new AlignWithSpline.TiedKnot();

		// Token: 0x0400102C RID: 4140
		public TiedKnotVisualizer tiedKnotVisualizer;

		// Token: 0x0400102D RID: 4141
		public float knotProgressRange = 0.025f;

		// Token: 0x0400102E RID: 4142
		public Vector2 knotProgressRangeRelation = new Vector2(-2f, 1f);

		// Token: 0x0400102F RID: 4143
		public float test = -0.3f;

		// Token: 0x020003D1 RID: 977
		public class TiedKnot
		{
			// Token: 0x04001404 RID: 5124
			public List<AlignWithSpline.TiedKnot.KnotPoint> knotPoints = new List<AlignWithSpline.TiedKnot.KnotPoint>();

			// Token: 0x020003E9 RID: 1001
			public class KnotPoint
			{
				// Token: 0x04001452 RID: 5202
				public Vector3 position;

				// Token: 0x04001453 RID: 5203
				public float templateProgress;

				// Token: 0x04001454 RID: 5204
				public bool inside;
			}
		}
	}
}
