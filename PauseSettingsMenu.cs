using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x02000167 RID: 359
public class PauseSettingsMenu : MenuWindow
{
	// Token: 0x17000098 RID: 152
	// (get) Token: 0x06000A41 RID: 2625 RVA: 0x00032400 File Offset: 0x00030600
	public override bool openOnStart
	{
		get
		{
			return false;
		}
	}

	// Token: 0x17000099 RID: 153
	// (get) Token: 0x06000A42 RID: 2626 RVA: 0x00032403 File Offset: 0x00030603
	public override bool selectOnOpen
	{
		get
		{
			return true;
		}
	}

	// Token: 0x1700009A RID: 154
	// (get) Token: 0x06000A43 RID: 2627 RVA: 0x00032406 File Offset: 0x00030606
	public override bool closeOnPause
	{
		get
		{
			return true;
		}
	}

	// Token: 0x1700009B RID: 155
	// (get) Token: 0x06000A44 RID: 2628 RVA: 0x00032409 File Offset: 0x00030609
	public override bool closeOnUICancel
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06000A45 RID: 2629 RVA: 0x0003240C File Offset: 0x0003060C
	protected override void Initialize()
	{
		this.backButton.onClick.AddListener(new UnityAction(base.Close));
	}

	// Token: 0x06000A46 RID: 2630 RVA: 0x0003242A File Offset: 0x0003062A
	protected override void OnOpen()
	{
		this.pauseBgCanvas.gameObject.SetActive(true);
	}

	// Token: 0x06000A47 RID: 2631 RVA: 0x0003243D File Offset: 0x0003063D
	protected override void OnClose()
	{
		this.pauseBgCanvas.gameObject.SetActive(false);
		this.optionsMenu.Open();
	}

	// Token: 0x0400090A RID: 2314
	public Canvas pauseBgCanvas;

	// Token: 0x0400090B RID: 2315
	public MenuWindow optionsMenu;

	// Token: 0x0400090C RID: 2316
	public Button backButton;
}
