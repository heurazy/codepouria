using System;
using Photon.Pun;
using Steamworks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zorro.UI;

// Token: 0x0200015C RID: 348
public class MainMenuMainPage : UIPage
{
	// Token: 0x060009E9 RID: 2537 RVA: 0x0003183C File Offset: 0x0002FA3C
	private void Start()
	{
		this.m_playButton.onClick.AddListener(new UnityAction(this.PlayClicked));
		NetworkConnector.ConnectToPhoton();
		PhotonNetwork.AddCallbackTarget(this);
		PhotonNetwork.NickName = NetworkConnector.GetUsername();
		Debug.Log("Initialized with name: " + PhotonNetwork.NickName);
		GameHandler.RestartService<PlayerHandler>(new PlayerHandler());
		Debug.Log("Restarting Player Handler Service...");
	}

	// Token: 0x060009EA RID: 2538 RVA: 0x000318A3 File Offset: 0x0002FAA3
	private void SettingsClicked()
	{
		this.pageHandler.TransistionToPage<MainMenuSettingsPage>(new SetActivePageTransistion());
	}

	// Token: 0x060009EB RID: 2539 RVA: 0x000318B6 File Offset: 0x0002FAB6
	private void OnDestroy()
	{
		PhotonNetwork.RemoveCallbackTarget(this);
	}

	// Token: 0x060009EC RID: 2540 RVA: 0x000318BE File Offset: 0x0002FABE
	private void PlayClicked()
	{
		SteamMatchmaking.CreateLobby((GameHandler.Instance.SettingsHandler.GetSetting<LobbyTypeSetting>().Value == LobbyTypeSetting.LobbyType.Friends) ? ELobbyType.k_ELobbyTypeFriendsOnly : ELobbyType.k_ELobbyTypePrivate, NetworkConnector.MAX_PLAYERS);
	}

	// Token: 0x060009ED RID: 2541 RVA: 0x000318E5 File Offset: 0x0002FAE5
	private void Update()
	{
		this.m_playButton.gameObject.SetActive(!PhotonNetwork.OfflineMode);
	}

	// Token: 0x040008E4 RID: 2276
	[SerializeField]
	private Button m_playButton;

	// Token: 0x040008E5 RID: 2277
	[SerializeField]
	private Button m_playSoloButton;

	// Token: 0x040008E6 RID: 2278
	[SerializeField]
	private Button m_settingsButton;
}
