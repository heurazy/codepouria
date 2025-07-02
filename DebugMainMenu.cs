using System;
using System.Linq;
using TMPro;
using Unity.Multiplayer.Playmode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Token: 0x02000072 RID: 114
public class DebugMainMenu : MonoBehaviour
{
	// Token: 0x06000417 RID: 1047 RVA: 0x00017B3C File Offset: 0x00015D3C
	private void Start()
	{
		this.m_matchmakeButton.onClick.AddListener(new UnityAction(this.MatchmakeClicked));
		this.m_debugJoinButton.onClick.AddListener(new UnityAction(this.DebugJoinClicked));
		this.m_debugCreateButton.onClick.AddListener(new UnityAction(this.DebugCreateClicked));
		this.m_debugRejoinButton.onClick.AddListener(new UnityAction(this.DebugRejoinClicked));
		if (this.debugJoinOnAwake)
		{
			this.DebugHaxxClicked();
		}
	}

	// Token: 0x06000418 RID: 1048 RVA: 0x00017BC7 File Offset: 0x00015DC7
	private void DebugRejoinClicked()
	{
		Debug.Log("Rejoining...");
		GameHandler.GetService<ConnectionService>();
		SceneManager.LoadScene("WilIsland");
	}

	// Token: 0x06000419 RID: 1049 RVA: 0x00017BE3 File Offset: 0x00015DE3
	private void DebugCreateClicked()
	{
		GameHandler.GetService<ConnectionService>().StateMachine.SwitchState<HostState>(false).RoomName = "THEPETHEN";
		SceneManager.LoadScene("WilIsland");
	}

	// Token: 0x0600041A RID: 1050 RVA: 0x00017C09 File Offset: 0x00015E09
	private void DebugJoinClicked()
	{
		GameHandler.GetService<ConnectionService>().StateMachine.SwitchState<JoinSpecificRoomState>(false).RoomName = "THEPETHEN";
		SceneManager.LoadScene("WilIsland");
	}

	// Token: 0x0600041B RID: 1051 RVA: 0x00017C30 File Offset: 0x00015E30
	private void DebugHaxxClicked()
	{
		ConnectionService service = GameHandler.GetService<ConnectionService>();
		if (CurrentPlayer.ReadOnlyTags().Contains("Client") || !DebugMainMenu.first)
		{
			service.StateMachine.SwitchState<JoinSpecificRoomState>(false).RoomName = "THEPETHEN";
		}
		else
		{
			service.StateMachine.SwitchState<HostState>(false).RoomName = "THEPETHEN";
		}
		DebugMainMenu.first = false;
		SceneManager.LoadScene("WilIsland");
	}

	// Token: 0x0600041C RID: 1052 RVA: 0x00017C9C File Offset: 0x00015E9C
	private void MatchmakeClicked()
	{
		if (string.IsNullOrEmpty(this.m_usernameField.text))
		{
			Debug.LogError("Failed to get username field...");
			return;
		}
		if (string.IsNullOrEmpty(this.m_roomField.text))
		{
			Debug.LogError("Failed to get room name field...");
			return;
		}
		JoinSpecificRoomState joinSpecificRoomState = GameHandler.GetService<ConnectionService>().StateMachine.SwitchState<JoinSpecificRoomState>(false);
		joinSpecificRoomState.RoomName = this.m_roomField.text.ToLower();
		joinSpecificRoomState.RegionToJoin = "eu";
		SceneManager.LoadScene("WilIsland");
	}

	// Token: 0x0400046C RID: 1132
	[SerializeField]
	private Button m_matchmakeButton;

	// Token: 0x0400046D RID: 1133
	[SerializeField]
	private Button m_debugJoinButton;

	// Token: 0x0400046E RID: 1134
	[SerializeField]
	private Button m_debugCreateButton;

	// Token: 0x0400046F RID: 1135
	[SerializeField]
	private Button m_debugRejoinButton;

	// Token: 0x04000470 RID: 1136
	[SerializeField]
	private TMP_InputField m_usernameField;

	// Token: 0x04000471 RID: 1137
	[SerializeField]
	private TMP_InputField m_roomField;

	// Token: 0x04000472 RID: 1138
	public bool debugJoinOnAwake = true;

	// Token: 0x04000473 RID: 1139
	private static bool first = true;
}
