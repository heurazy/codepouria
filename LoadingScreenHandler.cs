using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zorro.Core;

// Token: 0x020000ED RID: 237
public class LoadingScreenHandler : RetrievableResourceSingleton<LoadingScreenHandler>
{
	// Token: 0x1700005F RID: 95
	// (get) Token: 0x0600071B RID: 1819 RVA: 0x00025A94 File Offset: 0x00023C94
	// (set) Token: 0x0600071C RID: 1820 RVA: 0x00025A9B File Offset: 0x00023C9B
	public static bool loading { get; private set; }

	// Token: 0x0600071D RID: 1821 RVA: 0x00025AA3 File Offset: 0x00023CA3
	private void Awake()
	{
		this.loadingScreens = new Dictionary<LoadingScreen.LoadingScreenType, LoadingScreen>
		{
			{
				LoadingScreen.LoadingScreenType.Basic,
				this.loadingScreenPrefabBasic
			},
			{
				LoadingScreen.LoadingScreenType.Plane,
				this.loadingScreenPrefabPlane
			}
		};
		Object.DontDestroyOnLoad(this);
	}

	// Token: 0x0600071E RID: 1822 RVA: 0x00025AD0 File Offset: 0x00023CD0
	public LoadingScreen GetLoadingScreenPrefab(LoadingScreen.LoadingScreenType type)
	{
		return this.loadingScreens[type];
	}

	// Token: 0x0600071F RID: 1823 RVA: 0x00025ADE File Offset: 0x00023CDE
	public void Load(LoadingScreen.LoadingScreenType type, Action runAfter, params IEnumerator[] processes)
	{
		GameHandler.ClearStatus<EndScreenStatus>();
		if (!LoadingScreenHandler.loading)
		{
			base.StartCoroutine(this.LoadingRoutine(type, runAfter, processes));
			return;
		}
		Debug.LogError("Tried to load while already loading! If this happens a lot it's likely an issue!");
	}

	// Token: 0x06000720 RID: 1824 RVA: 0x00025B07 File Offset: 0x00023D07
	private IEnumerator LoadingRoutine(LoadingScreen.LoadingScreenType type, Action runAfter, params IEnumerator[] processes)
	{
		LoadingScreen loadingScreen = Object.Instantiate<LoadingScreen>(this.GetLoadingScreenPrefab(type), Vector3.zero, Quaternion.identity);
		LoadingScreenHandler.loading = true;
		yield return base.StartCoroutine(loadingScreen.LoadingRoutine(runAfter, processes));
		LoadingScreenHandler.loading = false;
		yield break;
	}

	// Token: 0x06000721 RID: 1825 RVA: 0x00025B2B File Offset: 0x00023D2B
	internal IEnumerator LoadSceneProcess(string sceneName, bool networked, bool yieldForCharacterSpawn = false, float extraYieldTimeOnEnd = 3f)
	{
		if (networked)
		{
			yield return this.LoadSceneProcessNetworked(sceneName, yieldForCharacterSpawn, extraYieldTimeOnEnd);
		}
		else
		{
			yield return this.LoadSceneProcessOffline(sceneName, yieldForCharacterSpawn, extraYieldTimeOnEnd);
		}
		yield break;
	}

	// Token: 0x06000722 RID: 1826 RVA: 0x00025B57 File Offset: 0x00023D57
	private IEnumerator LoadSceneProcessNetworked(string sceneName, bool yieldForCharacterSpawn, float extraYieldTimeOnEnd)
	{
		PhotonNetwork.LoadLevel(sceneName);
		float timeout = 5f;
		while ((timeout > 0f && PhotonNetwork.LevelLoadingProgress == 0f) || PhotonNetwork.LevelLoadingProgress >= 1f)
		{
			timeout -= Time.unscaledDeltaTime;
			yield return null;
		}
		if (DayNightManager.instance != null)
		{
			DayNightManager.instance.specialDayIntensity = 0f;
		}
		while (PhotonNetwork.LevelLoadingProgress < 1f)
		{
			Debug.Log("Waiting for level loading");
			yield return null;
		}
		while (PhotonNetwork.NetworkClientState == ClientState.ConnectingToGameServer)
		{
			Debug.Log("Waiting while connecting...");
			yield return null;
		}
		if (yieldForCharacterSpawn)
		{
			while (!Character.localCharacter && PhotonNetwork.InRoom)
			{
				Debug.Log("Connected and waiting for player to be spawned");
				yield return null;
			}
		}
		yield return new WaitForSecondsRealtime(extraYieldTimeOnEnd);
		yield break;
	}

	// Token: 0x06000723 RID: 1827 RVA: 0x00025B74 File Offset: 0x00023D74
	private IEnumerator LoadSceneProcessOffline(string sceneName, bool yieldForCharacterSpawn, float extraYieldTimeOnEnd)
	{
		AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
		while (!operation.isDone)
		{
			Debug.Log("Waiting for scene loading...");
			yield return null;
		}
		while (PhotonNetwork.NetworkClientState == ClientState.ConnectingToGameServer)
		{
			Debug.Log("Waiting while connecting...");
			yield return null;
		}
		if (yieldForCharacterSpawn)
		{
			while (!Character.localCharacter && PhotonNetwork.InRoom)
			{
				Debug.Log("Connected and waiting for player to be spawned");
				yield return null;
			}
		}
		yield return new WaitForSecondsRealtime(extraYieldTimeOnEnd);
		yield break;
	}

	// Token: 0x040006B1 RID: 1713
	public LoadingScreen loadingScreenPrefabBasic;

	// Token: 0x040006B2 RID: 1714
	public LoadingScreen loadingScreenPrefabPlane;

	// Token: 0x040006B4 RID: 1716
	private Dictionary<LoadingScreen.LoadingScreenType, LoadingScreen> loadingScreens;
}
