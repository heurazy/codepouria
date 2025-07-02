using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zorro.UI;

// Token: 0x0200016B RID: 363
public class PauseMenuMainPage : UIPage
{
	// Token: 0x06000A52 RID: 2642 RVA: 0x0003262E File Offset: 0x0003082E
	private void Start()
	{
		this.m_quitButton.onClick.AddListener(new UnityAction(this.OnQuitClicked));
		this.m_settingsButton.onClick.AddListener(new UnityAction(this.OnSettingsClicked));
	}

	// Token: 0x06000A53 RID: 2643 RVA: 0x00032668 File Offset: 0x00030868
	private void OnSettingsClicked()
	{
		this.pageHandler.TransistionToPage<PauseMenuSettingsMenuPage>();
	}

	// Token: 0x06000A54 RID: 2644 RVA: 0x00032676 File Offset: 0x00030876
	private void OnQuitClicked()
	{
		GameHandler.GetService<SteamLobbyHandler>().LeaveLobby();
		PhotonNetwork.Disconnect();
		Debug.Log("Leaving Photon room and returning to main menu");
	}

	// Token: 0x04000919 RID: 2329
	[SerializeField]
	private Button m_quitButton;

	// Token: 0x0400091A RID: 2330
	[SerializeField]
	private Button m_settingsButton;
}
