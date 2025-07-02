using System;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x0200020C RID: 524
public class PauseMainMenu : MenuWindow
{
	// Token: 0x170000B1 RID: 177
	// (get) Token: 0x06000D8E RID: 3470 RVA: 0x00044513 File Offset: 0x00042713
	public override bool openOnStart
	{
		get
		{
			return false;
		}
	}

	// Token: 0x170000B2 RID: 178
	// (get) Token: 0x06000D8F RID: 3471 RVA: 0x00044516 File Offset: 0x00042716
	public override bool selectOnOpen
	{
		get
		{
			return true;
		}
	}

	// Token: 0x170000B3 RID: 179
	// (get) Token: 0x06000D90 RID: 3472 RVA: 0x00044519 File Offset: 0x00042719
	public override bool closeOnPause
	{
		get
		{
			return true;
		}
	}

	// Token: 0x170000B4 RID: 180
	// (get) Token: 0x06000D91 RID: 3473 RVA: 0x0004451C File Offset: 0x0004271C
	public override bool closeOnUICancel
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06000D92 RID: 3474 RVA: 0x0004451F File Offset: 0x0004271F
	protected override void Initialize()
	{
		this.backButton.onClick.AddListener(new UnityAction(base.Close));
	}

	// Token: 0x06000D93 RID: 3475 RVA: 0x0004453D File Offset: 0x0004273D
	protected override void OnClose()
	{
		this.mainMenu.Open();
	}

	// Token: 0x04000CA2 RID: 3234
	public MenuWindow mainMenu;

	// Token: 0x04000CA3 RID: 3235
	public Button backButton;
}
