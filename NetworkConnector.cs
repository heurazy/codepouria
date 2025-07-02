using System;
using System.Collections;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Photon.Pun;
using Photon.Realtime;
using Steamworks;
using Unity.Multiplayer.Playmode;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zorro.Core;
using Zorro.UI.Modal;

// Token: 0x020000FA RID: 250
public class NetworkConnector : MonoBehaviourPunCallbacks
{
	// Token: 0x06000760 RID: 1888 RVA: 0x00027778 File Offset: 0x00025978
	private void Awake()
	{
		NetworkConnector._instance = this;
	}

	// Token: 0x06000761 RID: 1889 RVA: 0x00027780 File Offset: 0x00025980
	private async void Start()
	{
		Debug.Log("Network Connector is starting in scene: " + SceneManager.GetActiveScene().name);
		ConnectionState state = GameHandler.GetService<ConnectionService>().StateMachine.CurrentState;
		if (state is InRoomState)
		{
			foreach (global::Player player in PlayerHandler.GetAllPlayers())
			{
				player.hasClosedEndScreen = false;
			}
			if (PhotonNetwork.IsMasterClient)
			{
				GameHandler.GetService<SteamLobbyHandler>().SetLobbyData();
			}
			if (this.keepSettingLobbyDataCoroutine == null)
			{
				this.keepSettingLobbyDataCoroutine = base.StartCoroutine(this.KeepSettingLobbyData());
			}
		}
		else
		{
			if (state is DefaultConnectionState)
			{
				string[] mppmTag = CurrentPlayer.ReadOnlyTags();
				if (mppmTag.Contains("Client") && !mppmTag.Contains("CaelansShitComputer"))
				{
					await Awaitable.WaitForSecondsAsync(10f, default(CancellationToken));
				}
				if (mppmTag.Contains("CaelansShitComputer"))
				{
					this.yieldingForCaelan = true;
					Debug.LogError("Waiting for button in NetworkConnector to be clicked...");
					while (this.yieldingForCaelan)
					{
						await Task.Delay(100);
					}
					Debug.LogError("Button clicked.");
				}
				mppmTag = null;
			}
			PhotonNetwork.SerializationRate = 30;
			PhotonNetwork.SendRate = 30;
			if (state is DefaultConnectionState)
			{
				PhotonNetwork.NickName = NetworkConnector.GetUsername();
				Debug.Log("Initialized with name: " + PhotonNetwork.NickName);
				BuildVersion buildVersion = new BuildVersion(Application.version);
				PhotonNetwork.PhotonServerSettings.AppSettings.AppVersion = buildVersion.ToMatchmaking();
				if (CurrentPlayer.ReadOnlyTags().Contains("Client"))
				{
					JoinSpecificRoomState joinSpecificRoomState = state.stateMachine.SwitchState<JoinSpecificRoomState>(false);
					joinSpecificRoomState.RoomName = Environment.MachineName;
					state = joinSpecificRoomState;
				}
				else
				{
					HostState hostState = state.stateMachine.SwitchState<HostState>(false);
					hostState.RoomName = Environment.MachineName;
					state = hostState;
				}
				if (!PhotonNetwork.OfflineMode)
				{
					NetworkConnector.ConnectToPhoton();
				}
			}
			if (PhotonNetwork.NetworkClientState == ClientState.ConnectedToMasterServer)
			{
				this.HandleConnectionState(state);
			}
			else
			{
				PhotonNetwork.NickName = NetworkConnector.GetUsername();
				NetworkConnector.ConnectToPhoton();
			}
		}
	}

	// Token: 0x06000762 RID: 1890 RVA: 0x000277B7 File Offset: 0x000259B7
	public static string GetUsername()
	{
		return SteamFriends.GetPersonaName();
	}

	// Token: 0x06000763 RID: 1891 RVA: 0x000277BE File Offset: 0x000259BE
	private IEnumerator KeepSettingLobbyData()
	{
		int index = 100;
		while (PhotonNetwork.InRoom)
		{
			if (GameHandler.GetService<SteamLobbyHandler>().InSteamLobby() && PhotonNetwork.InRoom)
			{
				string name = PhotonNetwork.CurrentRoom.Name;
				ulong num;
				if (index > 5 && ulong.TryParse(name, out num))
				{
					index = 0;
					CloudAPI.VerifyLobby(num, delegate(string s)
					{
						InRoomState inRoomState = GameHandler.GetService<ConnectionService>().StateMachine.CurrentState as InRoomState;
						if (inRoomState != null)
						{
							inRoomState.verifiedLobby = s;
						}
					});
				}
				if (PhotonNetwork.IsMasterClient)
				{
					GameHandler.GetService<SteamLobbyHandler>().SetLobbyData();
					Debug.Log("IS master, is updating lobby data");
				}
			}
			int num2 = index;
			index = num2 + 1;
			yield return new WaitForSecondsRealtime(100f);
		}
		yield break;
	}

	// Token: 0x06000764 RID: 1892 RVA: 0x000277C6 File Offset: 0x000259C6
	private void EndConnectionYield()
	{
		this.yieldingForCaelan = false;
	}

	// Token: 0x06000765 RID: 1893 RVA: 0x000277D0 File Offset: 0x000259D0
	private void HandleConnectionState(ConnectionState state)
	{
		HostState hostState = state as HostState;
		if (hostState != null)
		{
			RoomOptions roomOptions = NetworkConnector.HostRoomOptions();
			PhotonNetwork.CreateRoom(hostState.RoomName, roomOptions, null, null);
		}
		JoinSpecificRoomState joinSpecificRoomState = state as JoinSpecificRoomState;
		if (joinSpecificRoomState != null)
		{
			Debug.Log(string.Concat(new string[]
			{
				"$Connecting to specific region: ",
				joinSpecificRoomState.RegionToJoin,
				" with app ID ",
				PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime,
				". Is currently connected to: ",
				PhotonNetwork.CloudRegion
			}));
			if (PhotonNetwork.CloudRegion != joinSpecificRoomState.RegionToJoin && !string.IsNullOrEmpty(joinSpecificRoomState.RegionToJoin))
			{
				Debug.Log("Disconnecting and reconnecting to specfic region: " + joinSpecificRoomState.RegionToJoin);
				PhotonNetwork.Disconnect();
				PhotonNetwork.ConnectToRegion(joinSpecificRoomState.RegionToJoin);
				return;
			}
			Debug.Log("Joining specific room: " + joinSpecificRoomState.RoomName);
			PhotonNetwork.JoinRoom(joinSpecificRoomState.RoomName, null);
		}
	}

	// Token: 0x06000766 RID: 1894 RVA: 0x000278BC File Offset: 0x00025ABC
	public override void OnConnectedToMaster()
	{
		ConnectionState currentState = GameHandler.GetService<ConnectionService>().StateMachine.CurrentState;
		Debug.Log("Connected to Photon Master Server... region: " + PhotonNetwork.CloudRegion);
		this.HandleConnectionState(currentState);
	}

	// Token: 0x06000767 RID: 1895 RVA: 0x000278F4 File Offset: 0x00025AF4
	public override void OnLeftRoom()
	{
		base.OnLeftRoom();
		SceneManager.LoadScene(NetworkConnector.rejoinScene);
		NetworkConnector.rejoinScene = "Title";
	}

	// Token: 0x06000768 RID: 1896 RVA: 0x00027910 File Offset: 0x00025B10
	public override void OnCreatedRoom()
	{
		base.OnCreatedRoom();
		GameHandler.GetService<ConnectionService>().StateMachine.SwitchState<InRoomState>(false);
		SteamLobbyHandler service = GameHandler.GetService<SteamLobbyHandler>();
		if (service.InSteamLobby())
		{
			if (PhotonNetwork.IsMasterClient)
			{
				service.SetLobbyData();
			}
			if (this.keepSettingLobbyDataCoroutine == null)
			{
				this.keepSettingLobbyDataCoroutine = base.StartCoroutine(this.KeepSettingLobbyData());
			}
		}
	}

	// Token: 0x06000769 RID: 1897 RVA: 0x00027969 File Offset: 0x00025B69
	public override void OnCreateRoomFailed(short returnCode, string message)
	{
		base.OnCreateRoomFailed(returnCode, message);
		Debug.LogError(string.Format("Failed to create Photon Room, code: {0}, message: {1}", returnCode, message));
	}

	// Token: 0x0600076A RID: 1898 RVA: 0x0002798C File Offset: 0x00025B8C
	public override void OnDisconnected(DisconnectCause cause)
	{
		base.OnDisconnected(cause);
		if (PhotonNetwork.OfflineMode)
		{
			return;
		}
		if (cause == DisconnectCause.DisconnectByClientLogic)
		{
			return;
		}
		Debug.LogError(string.Format("Disconnected from Photon Server: {0}", cause));
		GameHandler.GetService<ConnectionService>().StateMachine.SwitchState<DefaultConnectionState>(false);
		HeaderModalOption headerModalOption = new DefaultHeaderModalOption("Disconnected from Photon", string.Format("You disconnected from photon, reason: {0}", cause));
		ModalButtonsOption.Option[] array = new ModalButtonsOption.Option[1];
		array[0] = new ModalButtonsOption.Option("Okay", delegate
		{
			SceneManager.LoadScene("Title");
		});
		Modal.OpenModal(headerModalOption, new ModalButtonsOption(array), null);
	}

	// Token: 0x0600076B RID: 1899 RVA: 0x00027A2C File Offset: 0x00025C2C
	public override void OnJoinRoomFailed(short returnCode, string message)
	{
		base.OnJoinRoomFailed(returnCode, message);
		Debug.LogError(string.Format("Failed to join Photon Room, code: {0}, message: {1}", returnCode, message));
		GameHandler.GetService<ConnectionService>().StateMachine.SwitchState<DefaultConnectionState>(false);
		HeaderModalOption headerModalOption = new DefaultHeaderModalOption("Failed to find Photon Room", "Could not find the photon room, host could have left just before you joined");
		ModalButtonsOption.Option[] array = new ModalButtonsOption.Option[1];
		array[0] = new ModalButtonsOption.Option("Okay", delegate
		{
			SceneManager.LoadScene("Title");
		});
		Modal.OpenModal(headerModalOption, new ModalButtonsOption(array), null);
	}

	// Token: 0x0600076C RID: 1900 RVA: 0x00027AB4 File Offset: 0x00025CB4
	public override void OnJoinRandomFailed(short returnCode, string message)
	{
		base.OnJoinRandomFailed(returnCode, message);
		Debug.LogError(string.Format("Failed to join Random Photon Room, code: {0}, message: {1}", returnCode, message));
	}

	// Token: 0x0600076D RID: 1901 RVA: 0x00027AD4 File Offset: 0x00025CD4
	public override void OnJoinedRoom()
	{
		if (Character.localCharacter != null)
		{
			Debug.Log(string.Format("On Joined Photon Room. UserId:{0}, rejoined: {1}", Character.localCharacter.photonView.Owner.UserId, Character.localCharacter.photonView.Owner.HasRejoined));
		}
		else
		{
			Debug.Log("On Joined Photon Room. No Character");
		}
		GameHandler.GetService<ConnectionService>().StateMachine.SwitchState<InRoomState>(false);
	}

	// Token: 0x0600076E RID: 1902 RVA: 0x00027B48 File Offset: 0x00025D48
	public static void ConnectToPhoton()
	{
		BuildVersion buildVersion = new BuildVersion(Application.version);
		PhotonNetwork.AutomaticallySyncScene = true;
		PhotonNetwork.GameVersion = buildVersion.ToString();
		PhotonNetwork.PhotonServerSettings.AppSettings.AppVersion = buildVersion.ToMatchmaking();
		PhotonNetwork.ConnectUsingSettings();
		Debug.Log("Photon Start" + PhotonNetwork.NetworkClientState.ToString() + " using app version: " + buildVersion.ToMatchmaking());
	}

	// Token: 0x0600076F RID: 1903 RVA: 0x00027BC3 File Offset: 0x00025DC3
	public static RoomOptions HostRoomOptions()
	{
		return new RoomOptions
		{
			IsVisible = false,
			MaxPlayers = NetworkConnector.MAX_PLAYERS + 1
		};
	}

	// Token: 0x040006F9 RID: 1785
	public static int MAX_PLAYERS = 4;

	// Token: 0x040006FA RID: 1786
	private static NetworkConnector _instance;

	// Token: 0x040006FB RID: 1787
	private Coroutine keepSettingLobbyDataCoroutine;

	// Token: 0x040006FC RID: 1788
	private bool yieldingForCaelan;

	// Token: 0x040006FD RID: 1789
	private static string rejoinScene = "Title";
}
