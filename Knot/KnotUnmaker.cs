using System;
using System.Collections.Generic;
using pworld.Scripts.Extensions;
using UnityEngine;

namespace Knot
{
	// Token: 0x020002D4 RID: 724
	public class KnotUnmaker : MonoBehaviour
	{
		// Token: 0x060011ED RID: 4589 RVA: 0x0005854C File Offset: 0x0005674C
		public void Update()
		{
			Vector3 vector;
			this.MouseToPlaneRaycast(out vector);
			if (this.visualizer.knot == null || this.visualizer.knot.Count == 0)
			{
				this.grabbing = false;
				this.elapsed = 0f;
				return;
			}
			if (Input.GetKeyDown(KeyCode.Mouse1))
			{
				List<TiedKnotVisualizer.KnotPart> knot = this.visualizer.knot;
				float num = Vector3.Distance(knot[knot.Count - 1].position, vector);
				Debug.Log(string.Format("Try Erase Grab {0} < {1}", num, this.grabDistance));
				if (num < this.grabDistance)
				{
					Debug.Log("Grabbing");
					this.grabbing = true;
				}
			}
			if (Input.GetKey(KeyCode.Mouse1) && this.grabbing)
			{
				if (this.visualizer.knot.Count < 2)
				{
					this.visualizer.knot.Clear();
					this.grabbing = false;
					return;
				}
				Vector3 vector2 = vector.xyo();
				List<TiedKnotVisualizer.KnotPart> knot2 = this.visualizer.knot;
				Vector3 vector3 = vector2 - knot2[knot2.Count - 1].position.xyo();
				Vector3 vector4 = Vector3.zero;
				int num2 = Mathf.Min(10, this.visualizer.knot.Count) - 1;
				for (int i = 0; i < num2; i++)
				{
					Vector3 vector5 = vector4;
					List<TiedKnotVisualizer.KnotPart> knot3 = this.visualizer.knot;
					int num3 = i + 2;
					Vector3 vector6 = knot3[knot3.Count - num3].position.xyo();
					List<TiedKnotVisualizer.KnotPart> knot4 = this.visualizer.knot;
					vector4 = vector5 + (vector6 - knot4[knot4.Count - 1].position.xyo());
				}
				vector4 /= (float)num2;
				float num4 = Vector3.Angle(vector3, vector4);
				string text = "try Erasing, angle: {0}, Erase Speed {1}";
				object obj = num4;
				List<TiedKnotVisualizer.KnotPart> knot5 = this.visualizer.knot;
				Debug.Log(string.Format(text, obj, knot5[knot5.Count - 1].quality));
				float num5 = Mathf.InverseLerp(0f, this.dragAngle, num4);
				this.lineColor = Color.Lerp(this.good, this.bad, num5);
				if (num4 < this.dragAngle)
				{
					float num6 = this.eraseSpeed * (1f - Mathf.InverseLerp(0f, this.dragAngle, num4));
					float num7 = num6;
					float num8 = this.minDistToDrag;
					float num9 = this.maxDistToDrag;
					List<TiedKnotVisualizer.KnotPart> knot6 = this.visualizer.knot;
					num6 = num7 * Mathf.InverseLerp(num8, num9, Vector3.Distance(knot6[knot6.Count - 1].position, vector));
					float num10 = this.elapsed;
					float deltaTime = Time.deltaTime;
					List<TiedKnotVisualizer.KnotPart> knot7 = this.visualizer.knot;
					this.elapsed = num10 + deltaTime * (knot7[knot7.Count - 1].quality ? num6 : (num6 * 0.1f));
					List<TiedKnotVisualizer.KnotPart> knot8 = this.visualizer.knot;
					Vector3 position = knot8[knot8.Count - 2].position;
					List<TiedKnotVisualizer.KnotPart> knot9 = this.visualizer.knot;
					float num11 = Vector3.Distance(position, knot9[knot9.Count - 1].position);
					if (this.elapsed > num11)
					{
						Debug.Log(string.Format("Removing endKnot, knotDist: {0}", num11));
						this.visualizer.knot.RemoveAt(this.visualizer.knot.Count - 1);
						this.visualizer.Refresh();
						this.elapsed -= num11;
					}
				}
			}
			if (Input.GetKeyUp(KeyCode.Mouse1))
			{
				Debug.Log("Stop Grabbing");
				this.grabbing = false;
			}
		}

		// Token: 0x060011EE RID: 4590 RVA: 0x000588D4 File Offset: 0x00056AD4
		public bool MouseToPlaneRaycast(out Vector3 position)
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			Plane plane = new Plane(Camera.main.transform.forward, (KnotTemplateBoss.me != null) ? KnotTemplateBoss.me.displayRoot.position : Vector3.zero);
			float num;
			if (plane.Raycast(ray, out num))
			{
				position = ray.direction * num + ray.origin;
				return true;
			}
			position = Vector3.zero;
			return false;
		}

		// Token: 0x04001046 RID: 4166
		public TiedKnotVisualizer visualizer;

		// Token: 0x04001047 RID: 4167
		public float grabDistance = 0.05f;

		// Token: 0x04001048 RID: 4168
		public float dragAngle = 45f;

		// Token: 0x04001049 RID: 4169
		public float eraseSpeed = 0.1f;

		// Token: 0x0400104A RID: 4170
		private float elapsed;

		// Token: 0x0400104B RID: 4171
		public bool grabbing;

		// Token: 0x0400104C RID: 4172
		public Color lineColor;

		// Token: 0x0400104D RID: 4173
		public Color good;

		// Token: 0x0400104E RID: 4174
		public Color bad;

		// Token: 0x0400104F RID: 4175
		public float minDistToDrag = 1f;

		// Token: 0x04001050 RID: 4176
		public float maxDistToDrag = 10f;
	}
}
