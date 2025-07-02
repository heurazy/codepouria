using System;
using System.Collections;
using Photon.Pun;
using Steamworks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zorro.Core;
using Zorro.UI.Modal;

// Token: 0x0200006D RID: 109
public class SteamLobbyHandler : GameService<SteamLobbyHandler>
{
	// Token: 0x060003FD RID: 1021 RVA: 0x000171D8 File Offset: 0x000153D8
	public SteamLobbyHandler()
	{
		Debug.Log("Steam Lobby Handler initialized");
		Callback<LobbyCreated_t>.Create(new Callback<LobbyCreated_t>.DispatchDelegate(this.OnLobbyCreated));
		Callback<GameLobbyJoinRequested_t>.Create(new Callback<GameLobbyJoinRequested_t>.DispatchDelegate(this.OnLobbyJoinRequested));
		Callback<LobbyEnter_t>.Create(new Callback<LobbyEnter_t>.DispatchDelegate(this.OnLobbyEnter));
		Callback<LobbyDataUpdate_t>.Create(new Callback<LobbyDataUpdate_t>.DispatchDelegate(this.OnLobbyDataUpdate));
		this.m_currentLobby = CSteamID.Nil;
	}

	// Token: 0x060003FE RID: 1022 RVA: 0x00017254 File Offset: 0x00015454
	private void OnLobbyEnter(LobbyEnter_t param)
	{
		if (this.m_isHosting)
		{
			this.m_isHosting = false;
			return;
		}
		if (param.m_EChatRoomEnterResponse != 1U)
		{
			this.m_currentLobby = CSteamID.Nil;
			return;
		}
		this.m_currentLobby = new CSteamID(param.m_ulSteamIDLobby);
		Debug.Log("Entered Steam Lobby: " + this.m_currentLobby.ToString());
		string lobbyData = SteamMatchmaking.GetLobbyData(this.m_currentLobby, "PhotonRegion");
		string text = SteamMatchmaking.GetLobbyData(this.m_currentLobby, "CurrentScene");
		if (!string.IsNullOrEmpty(lobbyData))
		{
			this.tryingToFetchLobbyDataAttempts = Optionable<int>.None;
			if (string.IsNullOrEmpty(text))
			{
				text = "Airport";
				Debug.LogError("Failed to get scene to load, defaulting to airport");
			}
			JoinSpecificRoomState joinSpecificRoomState = GameHandler.GetService<ConnectionService>().StateMachine.SwitchState<JoinSpecificRoomState>(false);
			joinSpecificRoomState.RoomName = param.m_ulSteamIDLobby.ToString();
			joinSpecificRoomState.RegionToJoin = lobbyData;
			RetrievableResourceSingleton<LoadingScreenHandler>.Instance.Load(LoadingScreen.LoadingScreenType.Basic, null, new IEnumerator[] { RetrievableResourceSingleton<LoadingScreenHandler>.Instance.LoadSceneProcess(text, false, true, 3f) });
			return;
		}
		if (this.tryingToFetchLobbyDataAttempts.IsNone)
		{
			this.tryingToFetchLobbyDataAttempts = Optionable<int>.Some(1);
		}
		else
		{
			this.tryingToFetchLobbyDataAttempts = Optionable<int>.Some(this.tryingToFetchLobbyDataAttempts.Value + 1);
		}
		Debug.LogError(string.Format("Failed to get lobby region, attempts: {0}", this.tryingToFetchLobbyDataAttempts.Value));
		if (this.tryingToFetchLobbyDataAttempts.Value < 5)
		{
			this.LeaveLobby();
			this.TryJoinLobby(new CSteamID(param.m_ulSteamIDLobby));
			return;
		}
		Debug.LogError("Failed to fetch steam lobby");
		this.LeaveLobby();
		Modal.OpenModal(new DefaultHeaderModalOption("Joining failed", "Steam lobby doesn't seem valid"), new ModalButtonsOption(new ModalButtonsOption.Option[]
		{
			new ModalButtonsOption.Option("Okay", null)
		}), null);
	}

	// Token: 0x060003FF RID: 1023 RVA: 0x00017414 File Offset: 0x00015614
	private void OnLobbyJoinRequested(GameLobbyJoinRequested_t param)
	{
		Debug.Log(string.Format("On Lobby Join Requested: {0} by {1}", param.m_steamIDLobby, param.m_steamIDFriend));
		if (SteamMatchmaking.RequestLobbyData(param.m_steamIDLobby))
		{
			this.m_currentlyFetchingGameVersion = Optionable<CSteamID>.Some(param.m_steamIDLobby);
			return;
		}
		Modal.OpenModal(new DefaultHeaderModalOption("Failed to join lobby", "Failed to fetch lobby data"), new ModalButtonsOption(new ModalButtonsOption.Option[]
		{
			new ModalButtonsOption.Option("Okay", null)
		}), null);
	}

	// Token: 0x06000400 RID: 1024 RVA: 0x00017494 File Offset: 0x00015694
	private void OnLobbyDataUpdate(LobbyDataUpdate_t param)
	{
		if (param.m_bSuccess == 1)
		{
			if (this.m_currentlyFetchingGameVersion.IsSome && this.m_currentlyFetchingGameVersion.Value.m_SteamID == param.m_ulSteamIDLobby)
			{
				string lobbyData = SteamMatchmaking.GetLobbyData(this.m_currentlyFetchingGameVersion.Value, "PeakVersion");
				if (lobbyData == new BuildVersion(Application.version).ToMatchmaking())
				{
					if (PhotonNetwork.InRoom)
					{
						Debug.LogError("Not joining invite because your already in a room...");
						return;
					}
					this.JoinLobby(this.m_currentlyFetchingGameVersion.Value);
				}
				else
				{
					Debug.LogError("Game version mismatch: " + lobbyData);
					Modal.OpenModal(new DefaultHeaderModalOption("Game version mismatch", string.Concat(new string[]
					{
						"Host has different game version: [",
						lobbyData,
						"] while you have [",
						new BuildVersion(Application.version).ToMatchmaking(),
						"]"
					})), new ModalButtonsOption(new ModalButtonsOption.Option[]
					{
						new ModalButtonsOption.Option("Okay", null)
					}), null);
				}
			}
		}
		else
		{
			Debug.LogError("Failed to fetch lobby data");
			Modal.OpenModal(new DefaultHeaderModalOption("Failed to find lobby", "This invite might be out of date"), new ModalButtonsOption(new ModalButtonsOption.Option[]
			{
				new ModalButtonsOption.Option("Okay", null)
			}), null);
		}
		if (this.m_currentlyFetchingGameVersion.IsSome)
		{
			this.m_currentlyFetchingGameVersion = Optionable<CSteamID>.None;
		}
	}

	// Token: 0x06000401 RID: 1025 RVA: 0x000175F7 File Offset: 0x000157F7
	private void JoinLobby(CSteamID lobbyID)
	{
		this.LeaveLobby();
		Debug.Log(string.Format("Joining lobby: {0}", lobbyID));
		SteamMatchmaking.JoinLobby(lobbyID);
	}

	// Token: 0x06000402 RID: 1026 RVA: 0x0001761C File Offset: 0x0001581C
	public void TryJoinLobby(CSteamID lobbyID)
	{
		if (SteamMatchmaking.RequestLobbyData(lobbyID))
		{
			this.m_currentlyFetchingGameVersion = Optionable<CSteamID>.Some(lobbyID);
			return;
		}
		Modal.OpenModal(new DefaultHeaderModalOption("Failed to join lobby", "Failed to fetch lobby data"), new ModalButtonsOption(new ModalButtonsOption.Option[]
		{
			new ModalButtonsOption.Option("Okay", null)
		}), null);
	}

	// Token: 0x06000403 RID: 1027 RVA: 0x0001766C File Offset: 0x0001586C
	private void OnLobbyCreated(LobbyCreated_t param)
	{
		this.m_isHosting = true;
		if (param.m_eResult != EResult.k_EResultOK)
		{
			Modal.OpenModal(new DefaultHeaderModalOption("Failed to create lobby", string.Format("{0}", param.m_eResult)), new ModalButtonsOption(new ModalButtonsOption.Option[]
			{
				new ModalButtonsOption.Option("Okay", null)
			}), null);
			return;
		}
		Debug.Log(string.Format("Lobby Created: {0}", param.m_ulSteamIDLobby));
		this.m_currentLobby = new CSteamID(param.m_ulSteamIDLobby);
		if (!SteamMatchmaking.SetLobbyData(this.m_currentLobby, "PeakVersion", new BuildVersion(Application.version).ToMatchmaking()))
		{
			Debug.LogError("Failed to assign game version to lobby");
		}
		GameHandler.GetService<ConnectionService>().StateMachine.SwitchState<HostState>(false).RoomName = param.m_ulSteamIDLobby.ToString();
		RetrievableResourceSingleton<LoadingScreenHandler>.Instance.Load(LoadingScreen.LoadingScreenType.Basic, null, new IEnumerator[] { RetrievableResourceSingleton<LoadingScreenHandler>.Instance.LoadSceneProcess("Airport", false, true, 3f) });
	}

	// Token: 0x06000404 RID: 1028 RVA: 0x00017770 File Offset: 0x00015970
	public void SetLobbyData()
	{
		if (this.m_currentLobby == CSteamID.Nil)
		{
			Debug.LogError("Failed to set lobby data, no lobby joined...");
			return;
		}
		if (!PhotonNetwork.InRoom)
		{
			Debug.LogError("Failed to set Lobby data. not in a photon room");
			return;
		}
		if (SteamMatchmaking.SetLobbyData(this.m_currentLobby, "PhotonRegion", PhotonNetwork.CloudRegion))
		{
			Debug.Log("Set Photon Region to steam lobby data: " + PhotonNetwork.CloudRegion);
		}
		else
		{
			Debug.LogError("Failed to set lobby data, returned not okay...");
		}
		string name = SceneManager.GetActiveScene().name;
		if (SteamMatchmaking.SetLobbyData(this.m_currentLobby, "CurrentScene", name))
		{
			Debug.Log("Set current scene to: " + name);
			return;
		}
		Debug.LogError("Failed to set lobby data, returned not okay...");
	}

	// Token: 0x06000405 RID: 1029 RVA: 0x00017820 File Offset: 0x00015A20
	public void LeaveLobby()
	{
		if (this.m_currentLobby != CSteamID.Nil)
		{
			string text = "Leaving current lobby: ";
			CSteamID currentLobby = this.m_currentLobby;
			Debug.Log(text + currentLobby.ToString());
			SteamMatchmaking.LeaveLobby(this.m_currentLobby);
			this.m_currentLobby = CSteamID.Nil;
			return;
		}
		Debug.Log("Can't leave current lobby because not in a lobby");
	}

	// Token: 0x06000406 RID: 1030 RVA: 0x00017883 File Offset: 0x00015A83
	public bool InSteamLobby()
	{
		return this.m_currentLobby != CSteamID.Nil;
	}

	// Token: 0x06000407 RID: 1031 RVA: 0x00017895 File Offset: 0x00015A95
	public bool InSteamLobby(out CSteamID lobbyID)
	{
		lobbyID = this.m_currentLobby;
		return this.m_currentLobby != CSteamID.Nil;
	}

	// Token: 0x04000450 RID: 1104
	private const string PHOTON_REGION_KEY = "PhotonRegion";

	// Token: 0x04000451 RID: 1105
	private const string GAME_VERSION_KEY = "PeakVersion";

	// Token: 0x04000452 RID: 1106
	private const string CURRENT_SCENE_KEY = "CurrentScene";

	// Token: 0x04000453 RID: 1107
	private bool m_isHosting;

	// Token: 0x04000454 RID: 1108
	private CSteamID m_currentLobby;

	// Token: 0x04000455 RID: 1109
	private Optionable<CSteamID> m_currentlyFetchingGameVersion;

	// Token: 0x04000456 RID: 1110
	private Optionable<int> tryingToFetchLobbyDataAttempts = Optionable<int>.None;
}
