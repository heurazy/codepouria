using System;
using System.Collections.Generic;
using Knot;
using pworld.Scripts.Extensions;
using UnityEngine;

// Token: 0x020001E6 RID: 486
public class KnotTemplateBoss : MonoBehaviour
{
	// Token: 0x06000CD0 RID: 3280 RVA: 0x0003FF15 File Offset: 0x0003E115
	private void Awake()
	{
		KnotTemplateBoss.me = this;
	}

	// Token: 0x170000AE RID: 174
	// (get) Token: 0x06000CD1 RID: 3281 RVA: 0x0003FF1D File Offset: 0x0003E11D
	// (set) Token: 0x06000CD2 RID: 3282 RVA: 0x0003FF25 File Offset: 0x0003E125
	public LinkedListNode<KnotTemplate> Current
	{
		get
		{
			return this.current;
		}
		set
		{
			this.displayRoot.KillAllChildren(true, false, false);
			this.current = value;
			Object.Instantiate<KnotTemplate>(this.current.Value, this.displayRoot);
		}
	}

	// Token: 0x06000CD3 RID: 3283 RVA: 0x0003FF53 File Offset: 0x0003E153
	private void Start()
	{
		this.templates = new LinkedList<KnotTemplate>(this.startTemplates);
		this.Current = this.templates.First;
	}

	// Token: 0x06000CD4 RID: 3284 RVA: 0x0003FF78 File Offset: 0x0003E178
	public void Next()
	{
		this.Current = ((this.current.Next != null) ? this.Current.Next : this.templates.First);
		Object.FindFirstObjectByType<KnotMaker>().Clear();
		Object.FindFirstObjectByType<KnotUnmaker>().grabbing = false;
		Object.FindFirstObjectByType<TiedKnotVisualizer>().Clear();
	}

	// Token: 0x06000CD5 RID: 3285 RVA: 0x0003FFD0 File Offset: 0x0003E1D0
	public void Previous()
	{
		this.Current = ((this.Current.Previous != null) ? this.current.Previous : this.templates.Last);
		Object.FindFirstObjectByType<KnotMaker>().Clear();
		Object.FindFirstObjectByType<KnotUnmaker>().grabbing = false;
		Object.FindFirstObjectByType<TiedKnotVisualizer>().Clear();
	}

	// Token: 0x06000CD6 RID: 3286 RVA: 0x00040027 File Offset: 0x0003E227
	private void Update()
	{
	}

	// Token: 0x04000BCB RID: 3019
	public Transform displayRoot;

	// Token: 0x04000BCC RID: 3020
	public List<KnotTemplate> startTemplates = new List<KnotTemplate>();

	// Token: 0x04000BCD RID: 3021
	public LinkedList<KnotTemplate> templates = new LinkedList<KnotTemplate>();

	// Token: 0x04000BCE RID: 3022
	private LinkedListNode<KnotTemplate> current;

	// Token: 0x04000BCF RID: 3023
	public static KnotTemplateBoss me;
}
