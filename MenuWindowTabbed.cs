using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000164 RID: 356
public class MenuWindowTabbed : MenuWindow
{
	// Token: 0x1700008E RID: 142
	// (get) Token: 0x06000A26 RID: 2598 RVA: 0x000320D0 File Offset: 0x000302D0
	public virtual int startOnTab
	{
		get
		{
			return 0;
		}
	}

	// Token: 0x06000A27 RID: 2599 RVA: 0x000320D3 File Offset: 0x000302D3
	internal override void Open()
	{
		this.InitTabs();
		base.Open();
		this.SelectTab(this.startOnTab);
	}

	// Token: 0x06000A28 RID: 2600 RVA: 0x000320ED File Offset: 0x000302ED
	protected virtual void InitTabs()
	{
	}

	// Token: 0x06000A29 RID: 2601 RVA: 0x000320F0 File Offset: 0x000302F0
	public void SelectTab(int index)
	{
		if (this.tabs.Count <= index || index < 0)
		{
			Debug.LogError(string.Format("{0} tried to select out of range tab: {1}", base.gameObject.name, index));
			return;
		}
		for (int i = 0; i < this.tabs.Count; i++)
		{
			if (i == index)
			{
				this.tabs[i].Open();
			}
			else
			{
				this.tabs[i].Close();
			}
		}
		this.currentTab = index;
	}

	// Token: 0x06000A2A RID: 2602 RVA: 0x00032178 File Offset: 0x00030378
	public void SelectNextTab(bool forward)
	{
		this.currentTab += (forward ? 1 : (-1));
		if (this.currentTab >= this.tabs.Count)
		{
			this.currentTab = 0;
		}
		else if (this.currentTab < 0)
		{
			this.currentTab = this.tabs.Count - 1;
		}
		this.SelectTab(this.currentTab);
	}

	// Token: 0x040008F8 RID: 2296
	protected List<MenuWindow> tabs = new List<MenuWindow>();

	// Token: 0x040008F9 RID: 2297
	private int currentTab;
}
