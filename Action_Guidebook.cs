using System;
using Zorro.Core;

// Token: 0x020000B5 RID: 181
public class Action_Guidebook : ItemAction
{
	// Token: 0x0600060F RID: 1551 RVA: 0x0002141D File Offset: 0x0001F61D
	private void Awake()
	{
		this.guidebook = base.GetComponent<Guidebook>();
	}

	// Token: 0x06000610 RID: 1552 RVA: 0x0002142B File Offset: 0x0001F62B
	public override void RunAction()
	{
		this.guidebook.ToggleGuidebook();
		if (this.isSinglePage)
		{
			Singleton<AchievementManager>.Instance.TriggerSeenGuidebookPage(this.singlePageIndex);
		}
	}

	// Token: 0x040005FB RID: 1531
	private Guidebook guidebook;

	// Token: 0x040005FC RID: 1532
	public bool isSinglePage;

	// Token: 0x040005FD RID: 1533
	public int singlePageIndex;
}
