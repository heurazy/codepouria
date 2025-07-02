using System;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;
using Zorro.Core;

// Token: 0x02000165 RID: 357
public class PauseBadgesMenu : MenuWindow
{
	// Token: 0x1700008F RID: 143
	// (get) Token: 0x06000A2C RID: 2604 RVA: 0x000321F0 File Offset: 0x000303F0
	public override bool openOnStart
	{
		get
		{
			return false;
		}
	}

	// Token: 0x17000090 RID: 144
	// (get) Token: 0x06000A2D RID: 2605 RVA: 0x000321F3 File Offset: 0x000303F3
	public override bool selectOnOpen
	{
		get
		{
			return true;
		}
	}

	// Token: 0x17000091 RID: 145
	// (get) Token: 0x06000A2E RID: 2606 RVA: 0x000321F6 File Offset: 0x000303F6
	public override Selectable objectToSelectOnOpen
	{
		get
		{
			return this.backButton;
		}
	}

	// Token: 0x17000092 RID: 146
	// (get) Token: 0x06000A2F RID: 2607 RVA: 0x000321FE File Offset: 0x000303FE
	public override bool closeOnUICancel
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06000A30 RID: 2608 RVA: 0x00032201 File Offset: 0x00030401
	protected override void Initialize()
	{
		this.backButton.onClick.AddListener(new UnityAction(base.Close));
	}

	// Token: 0x06000A31 RID: 2609 RVA: 0x00032220 File Offset: 0x00030420
	protected override void OnOpen()
	{
		int num = 0;
		int num2;
		if (Singleton<AchievementManager>.Instance.GetSteamStatInt(STEAMSTATTYPE.TimesPeaked, out num2))
		{
			num = num2;
		}
		this.peaksSummitedText.text = "PEAKS SUMMITED: " + num.ToString();
		this.scoutTitleText.text = this.ascentData.ascents[Singleton<AchievementManager>.Instance.GetMaxAscent()].titleReward;
		this.badgeSashImage.color = this.ascentData.ascents[Singleton<AchievementManager>.Instance.GetMaxAscent()].color;
	}

	// Token: 0x06000A32 RID: 2610 RVA: 0x000322B1 File Offset: 0x000304B1
	protected override void OnClose()
	{
		this.optionsMenu.Open();
	}

	// Token: 0x040008FA RID: 2298
	public MenuWindow optionsMenu;

	// Token: 0x040008FB RID: 2299
	public Button backButton;

	// Token: 0x040008FC RID: 2300
	public Image badgeSashImage;

	// Token: 0x040008FD RID: 2301
	public TMP_Text scoutTitleText;

	// Token: 0x040008FE RID: 2302
	public AscentData ascentData;

	// Token: 0x040008FF RID: 2303
	public TMP_Text peaksSummitedText;
}
