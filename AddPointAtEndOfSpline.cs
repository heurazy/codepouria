using System;
using System.Collections.Generic;
using System.Linq;
using pworld.Scripts.Extensions;
using UnityEngine;
using UnityEngine.Splines;

// Token: 0x020001E4 RID: 484
public class AddPointAtEndOfSpline : MonoBehaviour
{
	// Token: 0x06000CC7 RID: 3271 RVA: 0x0003FC50 File Offset: 0x0003DE50
	public void SetAllZ(float v)
	{
		SplineContainer component = base.GetComponent<SplineContainer>();
		List<BezierKnot> list = component.Spline.Knots.ToList<BezierKnot>();
		for (int i = 0; i < list.Count; i++)
		{
			BezierKnot bezierKnot = list[i];
			bezierKnot.Position = bezierKnot.Position.xyn(v);
			list[i] = bezierKnot;
		}
		component.Spline.Knots = list;
	}

	// Token: 0x06000CC8 RID: 3272 RVA: 0x0003FCB8 File Offset: 0x0003DEB8
	private void GO()
	{
		SplineContainer component = base.GetComponent<SplineContainer>();
		BezierKnot bezierKnot = component.Spline.Knots.Last<BezierKnot>();
		List<BezierKnot> list = component.Spline.Knots.ToList<BezierKnot>();
		BezierKnot bezierKnot2 = list[list.Count - 2];
		component.Spline.Add(bezierKnot.Position.PToV3() + (bezierKnot.Position.PToV3() - bezierKnot2.Position.PToV3()).normalized, TangentMode.AutoSmooth);
		PExt.SaveObj(component);
	}

	// Token: 0x06000CC9 RID: 3273 RVA: 0x0003FD43 File Offset: 0x0003DF43
	private void Start()
	{
	}

	// Token: 0x06000CCA RID: 3274 RVA: 0x0003FD45 File Offset: 0x0003DF45
	private void Update()
	{
	}
}
