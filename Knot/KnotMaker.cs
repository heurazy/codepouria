using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using pworld.Scripts.Extensions;
using TMPro;
using UnityEngine;

namespace Knot
{
	// Token: 0x020002D1 RID: 721
	public class KnotMaker : MonoBehaviour
	{
		// Token: 0x060011D7 RID: 4567 RVA: 0x00057BF0 File Offset: 0x00055DF0
		private void Update()
		{
			this.scoreText.text = this.score.ToString();
			if (Input.GetKey(KeyCode.Escape))
			{
				this.Clear();
			}
			if (Input.GetKeyDown(KeyCode.Mouse0) && this.TryGrab())
			{
				this.grabbedRope = true;
				this.TieKnotFillToPoint(Input.mousePosition);
			}
			if (this.grabbedRope)
			{
				this.TieKnotFillToPoint(Input.mousePosition);
				Vector3 vector;
				if (this.MouseToPlaneRaycast(out vector, Input.mousePosition))
				{
					this.visualizer.knot.Add(new TiedKnotVisualizer.KnotPart(false, vector.xyo(), -1));
					this.visualizer.Refresh();
					this.visualizer.knot.RemoveLast<TiedKnotVisualizer.KnotPart>();
				}
			}
			else
			{
				this.visualizer.Refresh();
			}
			if (Input.GetKeyUp(KeyCode.Mouse0))
			{
				this.grabbedRope = false;
			}
		}

		// Token: 0x060011D8 RID: 4568 RVA: 0x00057CC4 File Offset: 0x00055EC4
		private bool TryGrab()
		{
			RaycastHit[] array = Physics.SphereCastAll(Camera.main.ScreenPointToRay(Input.mousePosition), this.width);
			if (this.visualizer.knot.Count == 0)
			{
				return array.Any((RaycastHit hit) => hit.transform.GetSiblingIndex() < this.maxPartJumpAllowed);
			}
			Vector3 vector;
			if (this.visualizer.knot.Count > 0 && this.MouseToPlaneRaycast(out vector, Input.mousePosition))
			{
				Vector3 vector2 = vector.xyo();
				List<TiedKnotVisualizer.KnotPart> knot = this.visualizer.knot;
				float num = Vector3.Distance(vector2, knot[knot.Count - 1].position.xyo());
				Debug.Log(string.Format("distance: {0}", num));
				if (num < this.grabDistance)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060011D9 RID: 4569 RVA: 0x00057D84 File Offset: 0x00055F84
		public bool MouseToPlaneRaycast(out Vector3 position, Vector3 mousePosition)
		{
			Ray ray = Camera.main.ScreenPointToRay(mousePosition);
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

		// Token: 0x060011DA RID: 4570 RVA: 0x00057E0F File Offset: 0x0005600F
		public void Clear()
		{
			this.score = 0f;
			this.grabbedRope = false;
			this.visualizer.knot.Clear();
		}

		// Token: 0x060011DB RID: 4571 RVA: 0x00057E34 File Offset: 0x00056034
		private void TieKnotFillToPoint(Vector3 mousePosition)
		{
			if (this.visualizer.knot.Count == 0)
			{
				this.TieKnot(mousePosition);
				return;
			}
			Camera main = Camera.main;
			List<TiedKnotVisualizer.KnotPart> knot = this.visualizer.knot;
			Vector3 vector = main.WorldToScreenPoint(knot[knot.Count - 1].position);
			int num = Mathf.FloorToInt(Vector3.Distance(vector, mousePosition) / this.minKnotSpacing);
			num = Mathf.Min(num, 100);
			Vector3 normalized = (vector - mousePosition).normalized;
			for (int i = 0; i < num; i++)
			{
				this.TieKnot(vector + -normalized * (this.minKnotSpacing * (float)(i + 1)));
			}
		}

		// Token: 0x060011DC RID: 4572 RVA: 0x00057EE4 File Offset: 0x000560E4
		private void TieKnot(Vector3 mousePosition)
		{
			KnotMaker.<>c__DisplayClass13_0 CS$<>8__locals1;
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.mousePosition = mousePosition;
			RaycastHit[] array = Physics.SphereCastAll(Camera.main.ScreenPointToRay(CS$<>8__locals1.mousePosition), this.width);
			if (array.Length != 0)
			{
				int templateProgress = 0;
				if (this.visualizer.count > 0)
				{
					if (this.visualizer.knot.Any((TiedKnotVisualizer.KnotPart knot) => knot.part != -1))
					{
						templateProgress = this.visualizer.knot.Last((TiedKnotVisualizer.KnotPart knot) => knot.part != -1).part;
					}
				}
				int num = (from hit in array.ToList<RaycastHit>()
					orderby Mathf.Abs(hit.transform.GetSiblingIndex() - (templateProgress + 1))
					select hit).First<RaycastHit>().collider.transform.GetSiblingIndex();
				int num2 = templateProgress - 1;
				int num3 = templateProgress + this.maxPartJumpAllowed;
				bool flag = true;
				if (num > templateProgress && num < num3)
				{
					templateProgress = num;
				}
				if (num <= num2)
				{
					flag = false;
					num = -1;
					this.score -= 1f;
				}
				if (num >= num3)
				{
					flag = false;
					num = -1;
					this.score -= 1f;
				}
				this.<TieKnot>g__AddKnotPositionAtMousePosition|13_0(flag, num, ref CS$<>8__locals1);
				return;
			}
			this.score -= 1f;
			this.<TieKnot>g__AddKnotPositionAtMousePosition|13_0(false, -1, ref CS$<>8__locals1);
		}

		// Token: 0x060011DF RID: 4575 RVA: 0x000580B0 File Offset: 0x000562B0
		[CompilerGenerated]
		private void <TieKnot>g__AddKnotPositionAtMousePosition|13_0(bool quality, int hitPart, ref KnotMaker.<>c__DisplayClass13_0 A_3)
		{
			Vector3 vector;
			if (this.MouseToPlaneRaycast(out vector, A_3.mousePosition))
			{
				this.visualizer.knot.Add(new TiedKnotVisualizer.KnotPart(quality, vector.xyo(), hitPart));
				Debug.Log(string.Format("Quality: {0}, Position: {1}", quality, vector.xyo()));
			}
		}

		// Token: 0x04001030 RID: 4144
		public TiedKnotVisualizer visualizer;

		// Token: 0x04001031 RID: 4145
		public TextMeshProUGUI scoreText;

		// Token: 0x04001032 RID: 4146
		public float score;

		// Token: 0x04001033 RID: 4147
		public float minKnotSpacing = 0.01f;

		// Token: 0x04001034 RID: 4148
		public int maxPartJumpAllowed = 10;

		// Token: 0x04001035 RID: 4149
		public float width = 0.07f;

		// Token: 0x04001036 RID: 4150
		[SerializeField]
		private float grabDistance;

		// Token: 0x04001037 RID: 4151
		public bool grabbedRope;
	}
}
