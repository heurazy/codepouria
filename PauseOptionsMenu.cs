using System;
using Steamworks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x02000166 RID: 358
public class PauseOptionsMenu : MenuWindow
{
	// Token: 0x17000093 RID: 147
	// (get) Token: 0x06000A34 RID: 2612 RVA: 0x000322C6 File Offset: 0x000304C6
	public override bool openOnStart
	{
		get
		{
			return false;
		}
	}

	// Token: 0x17000094 RID: 148
	// (get) Token: 0x06000A35 RID: 2613 RVA: 0x000322C9 File Offset: 0x000304C9
	public override bool selectOnOpen
	{
		get
		{
			return true;
		}
	}

	// Token: 0x17000095 RID: 149
	// (get) Token: 0x06000A36 RID: 2614 RVA: 0x000322CC File Offset: 0x000304CC
	public override Selectable objectToSelectOnOpen
	{
		get
		{
			return this.resumeButton;
		}
	}

	// Token: 0x17000096 RID: 150
	// (get) Token: 0x06000A37 RID: 2615 RVA: 0x000322D4 File Offset: 0x000304D4
	public override bool closeOnPause
	{
		get
		{
			return true;
		}
	}

	// Token: 0x17000097 RID: 151
	// (get) Token: 0x06000A38 RID: 2616 RVA: 0x000322D7 File Offset: 0x000304D7
	public override bool closeOnUICancel
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06000A39 RID: 2617 RVA: 0x000322DC File Offset: 0x000304DC
	protected override void Initialize()
	{
		this.resumeButton.onClick.AddListener(new UnityAction(base.Close));
		this.badgesButton.onClick.AddListener(new UnityAction(this.OpenBadgeMenu));
		this.optionsButton.onClick.AddListener(new UnityAction(this.OpenSettingsMenu));
		this.inviteButton.onClick.AddListener(new UnityAction(this.InviteFriends));
		this.controlsButton.onClick.AddListener(new UnityAction(this.OpenControlsMenu));
	}

	// Token: 0x06000A3A RID: 2618 RVA: 0x00032378 File Offset: 0x00030578
	public void InviteFriends()
	{
		CSteamID csteamID;
		if (GameHandler.GetService<SteamLobbyHandler>().InSteamLobby(out csteamID))
		{
			SteamFriends.ActivateGameOverlayInviteDialog(csteamID);
		}
	}

	// Token: 0x06000A3B RID: 2619 RVA: 0x00032399 File Offset: 0x00030599
	protected override void OnOpen()
	{
		this.pauseBgCanvas.gameObject.SetActive(true);
	}

	// Token: 0x06000A3C RID: 2620 RVA: 0x000323AC File Offset: 0x000305AC
	protected override void OnClose()
	{
		this.pauseBgCanvas.gameObject.SetActive(false);
	}

	// Token: 0x06000A3D RID: 2621 RVA: 0x000323BF File Offset: 0x000305BF
	public void OpenBadgeMenu()
	{
		this.badgeMenu.Open();
		base.Hide();
	}

	// Token: 0x06000A3E RID: 2622 RVA: 0x000323D2 File Offset: 0x000305D2
	public void OpenSettingsMenu()
	{
		this.settingsMenu.Open();
		base.Hide();
	}

	// Token: 0x06000A3F RID: 2623 RVA: 0x000323E5 File Offset: 0x000305E5
	public void OpenControlsMenu()
	{
		this.controlsMenu.Open();
		base.Hide();
	}

	// Token: 0x04000900 RID: 2304
	public Canvas pauseBgCanvas;

	// Token: 0x04000901 RID: 2305
	public Button resumeButton;

	// Token: 0x04000902 RID: 2306
	public Button inviteButton;

	// Token: 0x04000903 RID: 2307
	public Button badgesButton;

	// Token: 0x04000904 RID: 2308
	public Button optionsButton;

	// Token: 0x04000905 RID: 2309
	public Button controlsButton;

	// Token: 0x04000906 RID: 2310
	public Button leaveButton;

	// Token: 0x04000907 RID: 2311
	public MenuWindow badgeMenu;

	// Token: 0x04000908 RID: 2312
	public MenuWindow settingsMenu;

	// Token: 0x04000909 RID: 2313
	public MenuWindow controlsMenu;
}
