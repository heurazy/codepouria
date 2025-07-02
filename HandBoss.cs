using System;
using System.Collections.Generic;
using Knot;
using pworld.Scripts.Extensions;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001E5 RID: 485
public class HandBoss : MonoBehaviour
{
	// Token: 0x06000CCC RID: 3276 RVA: 0x0003FD4F File Offset: 0x0003DF4F
	private void Start()
	{
		Cursor.visible = false;
	}

	// Token: 0x06000CCD RID: 3277 RVA: 0x0003FD57 File Offset: 0x0003DF57
	private void DisableAll()
	{
		this.grabMaking.SetActive(false);
		this.grabUnmaking.SetActive(false);
		this.idle.SetActive(false);
		this.lr.gameObject.SetActive(false);
	}

	// Token: 0x06000CCE RID: 3278 RVA: 0x0003FD90 File Offset: 0x0003DF90
	private void Update()
	{
		this.DisableAll();
		base.transform.position = Input.mousePosition;
		if (this.knotMaker.grabbedRope)
		{
			this.grabMaking.SetActive(true);
			return;
		}
		if (this.knotUnmaker.grabbing)
		{
			this.lr.gameObject.SetActive(true);
			LineRenderer lineRenderer = this.lr;
			int num = 0;
			List<TiedKnotVisualizer.KnotPart> knot = this.knotUnmaker.visualizer.knot;
			Vector3 position = knot[knot.Count - 1].position;
			List<TiedKnotVisualizer.KnotPart> knot2 = this.knotUnmaker.visualizer.knot;
			lineRenderer.SetPosition(num, position.xyn(knot2[knot2.Count - 1].position.z - 1f));
			this.lr.startColor = this.knotUnmaker.lineColor;
			this.lr.endColor = this.knotUnmaker.lineColor;
			this.grabUnmaking.SetActive(true);
			Camera main = Camera.main;
			List<TiedKnotVisualizer.KnotPart> knot3 = this.knotUnmaker.visualizer.knot;
			main.WorldToScreenPoint(knot3[knot3.Count - 1].position);
			LineRenderer lineRenderer2 = this.lr;
			int num2 = 1;
			Vector3 vector = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			List<TiedKnotVisualizer.KnotPart> knot4 = this.knotUnmaker.visualizer.knot;
			lineRenderer2.SetPosition(num2, vector.xyn(knot4[knot4.Count - 1].position.z - 1f));
			return;
		}
		this.idle.SetActive(true);
	}

	// Token: 0x04000BC4 RID: 3012
	public GameObject grabMaking;

	// Token: 0x04000BC5 RID: 3013
	public GameObject grabUnmaking;

	// Token: 0x04000BC6 RID: 3014
	public GameObject idle;

	// Token: 0x04000BC7 RID: 3015
	public Image handImage;

	// Token: 0x04000BC8 RID: 3016
	public KnotMaker knotMaker;

	// Token: 0x04000BC9 RID: 3017
	public KnotUnmaker knotUnmaker;

	// Token: 0x04000BCA RID: 3018
	public LineRenderer lr;
}
