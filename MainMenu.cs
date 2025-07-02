using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zorro.Core;

// Token: 0x020001FE RID: 510
public class MainMenu : MenuWindow
{
	// Token: 0x170000AF RID: 175
	// (get) Token: 0x06000D34 RID: 3380 RVA: 0x0004295D File Offset: 0x00040B5D
	public override Selectable objectToSelectOnOpen
	{
		get
		{
			return this.playWithFriendsButton;
		}
	}

	// Token: 0x06000D35 RID: 3381 RVA: 0x00042968 File Offset: 0x00040B68
	protected override void Initialize()
	{
		AudioLevels.Reset();
		this.playSoloButton.onClick.AddListener(new UnityAction(this.PlaySoloClicked));
		this.optionsButton.onClick.AddListener(new UnityAction(this.OpenSettings));
		this.creditsButton.onClick.AddListener(new UnityAction(this.ToggleCredits));
		this.quitButton.onClick.AddListener(new UnityAction(this.Quit));
		this.discordButton.onClick.AddListener(new UnityAction(this.OpenDiscord));
		this.landfallButton.onClick.AddListener(new UnityAction(this.OpenLandfallWebsite));
		this.aggrocrabButton.onClick.AddListener(new UnityAction(this.OpenAggrocrabWebsite));
	}

	// Token: 0x06000D36 RID: 3382 RVA: 0x00042A3E File Offset: 0x00040C3E
	protected override void Update()
	{
		if (this.settingsMenu.isOpen)
		{
			this.credits.SetActive(false);
		}
	}

	// Token: 0x06000D37 RID: 3383 RVA: 0x00042A59 File Offset: 0x00040C59
	private void OpenSettings()
	{
		EventSystem.current.SetSelectedGameObject(null);
		this.settingsMenu.Open();
	}

	// Token: 0x06000D38 RID: 3384 RVA: 0x00042A71 File Offset: 0x00040C71
	public void ToggleCredits()
	{
		EventSystem.current.SetSelectedGameObject(null);
		this.credits.SetActive(!this.credits.activeSelf);
		if (this.credits.activeSelf)
		{
			this.RandomizeMainGuys();
		}
	}

	// Token: 0x06000D39 RID: 3385 RVA: 0x00042AAA File Offset: 0x00040CAA
	public void OpenDiscord()
	{
		EventSystem.current.SetSelectedGameObject(null);
		Application.OpenURL("https://discord.gg/peakgame");
	}

	// Token: 0x06000D3A RID: 3386 RVA: 0x00042AC1 File Offset: 0x00040CC1
	public void OpenLandfallWebsite()
	{
		EventSystem.current.SetSelectedGameObject(null);
		Application.OpenURL("https://landfall.se/");
	}

	// Token: 0x06000D3B RID: 3387 RVA: 0x00042AD8 File Offset: 0x00040CD8
	public void OpenAggrocrabWebsite()
	{
		EventSystem.current.SetSelectedGameObject(null);
		Application.OpenURL("https://aggrocrab.com/");
	}

	// Token: 0x06000D3C RID: 3388 RVA: 0x00042AF0 File Offset: 0x00040CF0
	public void RandomizeMainGuys()
	{
		Transform transform = this.mainGuysHolder;
		List<Transform> list = new List<Transform>();
		for (int i = 0; i < transform.childCount; i++)
		{
			list.Add(transform.GetChild(i));
		}
		for (int j = list.Count - 1; j > 0; j--)
		{
			int num = Random.Range(0, j + 1);
			List<Transform> list2 = list;
			int num2 = j;
			List<Transform> list3 = list;
			int num3 = num;
			Transform transform2 = list[num];
			Transform transform3 = list[j];
			list2[num2] = transform2;
			list3[num3] = transform3;
		}
		for (int k = 0; k < list.Count; k++)
		{
			list[k].SetSiblingIndex(k);
		}
	}

	// Token: 0x06000D3D RID: 3389 RVA: 0x00042BA2 File Offset: 0x00040DA2
	public void Quit()
	{
		Application.Quit();
	}

	// Token: 0x06000D3E RID: 3390 RVA: 0x00042BAC File Offset: 0x00040DAC
	private void GoToAirport()
	{
		RetrievableResourceSingleton<LoadingScreenHandler>.Instance.Load(LoadingScreen.LoadingScreenType.Basic, null, new IEnumerator[] { RetrievableResourceSingleton<LoadingScreenHandler>.Instance.LoadSceneProcess("Airport", false, true, 3f) });
	}

	// Token: 0x06000D3F RID: 3391 RVA: 0x00042BE4 File Offset: 0x00040DE4
	private void PlaySoloClicked()
	{
		RetrievableResourceSingleton<LoadingScreenHandler>.Instance.Load(LoadingScreen.LoadingScreenType.Basic, null, new IEnumerator[] { this.StartOfflineModeRoutine() });
	}

	// Token: 0x06000D40 RID: 3392 RVA: 0x00042C01 File Offset: 0x00040E01
	private IEnumerator StartOfflineModeRoutine()
	{
		PhotonNetwork.IsMessageQueueRunning = true;
		GameHandler.AddStatus<IsDisconnectingForOfflineMode>(new IsDisconnectingForOfflineMode());
		PhotonNetwork.Disconnect();
		while (PhotonNetwork.IsConnected)
		{
			Debug.Log("We are still connected.. waiting for disconnect");
			yield return null;
		}
		PhotonNetwork.OfflineMode = true;
		GameHandler.ClearStatus<IsDisconnectingForOfflineMode>();
		yield return RetrievableResourceSingleton<LoadingScreenHandler>.Instance.LoadSceneProcess("Airport", false, true, 3f);
		yield break;
	}

	// Token: 0x04000C58 RID: 3160
	public GameObject credits;

	// Token: 0x04000C59 RID: 3161
	public Transform mainGuysHolder;

	// Token: 0x04000C5A RID: 3162
	public Button playWithFriendsButton;

	// Token: 0x04000C5B RID: 3163
	public Button playSoloButton;

	// Token: 0x04000C5C RID: 3164
	public Button optionsButton;

	// Token: 0x04000C5D RID: 3165
	public Button creditsButton;

	// Token: 0x04000C5E RID: 3166
	public Button quitButton;

	// Token: 0x04000C5F RID: 3167
	public Button discordButton;

	// Token: 0x04000C60 RID: 3168
	public Button landfallButton;

	// Token: 0x04000C61 RID: 3169
	public Button aggrocrabButton;

	// Token: 0x04000C62 RID: 3170
	public MenuWindow settingsMenu;
}
