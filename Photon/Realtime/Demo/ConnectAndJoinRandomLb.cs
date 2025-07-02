using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;
using UnityEngine.UI;

namespace Photon.Realtime.Demo
{
	// Token: 0x020002C4 RID: 708
	public class ConnectAndJoinRandomLb : MonoBehaviour, IConnectionCallbacks, IMatchmakingCallbacks, ILobbyCallbacks
	{
		// Token: 0x06001176 RID: 4470 RVA: 0x00056460 File Offset: 0x00054660
		public void Start()
		{
			this.lbc = new LoadBalancingClient(ConnectionProtocol.Udp);
			this.lbc.AddCallbackTarget(this);
			if (!this.lbc.ConnectUsingSettings(this.appSettings))
			{
				Debug.LogError("Error while connecting");
			}
			this.ch = base.gameObject.GetComponent<ConnectionHandler>();
			if (this.ch != null)
			{
				this.ch.Client = this.lbc;
				this.ch.StartFallbackSendAckThread();
			}
		}

		// Token: 0x06001177 RID: 4471 RVA: 0x000564E0 File Offset: 0x000546E0
		public void Update()
		{
			LoadBalancingClient loadBalancingClient = this.lbc;
			if (loadBalancingClient != null)
			{
				loadBalancingClient.Service();
				Text stateUiText = this.StateUiText;
				string text = loadBalancingClient.State.ToString();
				if (stateUiText != null && !stateUiText.text.Equals(text))
				{
					stateUiText.text = "State: " + text;
				}
			}
		}

		// Token: 0x06001178 RID: 4472 RVA: 0x00056541 File Offset: 0x00054741
		public void OnConnected()
		{
		}

		// Token: 0x06001179 RID: 4473 RVA: 0x00056543 File Offset: 0x00054743
		public void OnConnectedToMaster()
		{
			Debug.Log("OnConnectedToMaster");
			this.lbc.OpJoinRandomRoom(null);
		}

		// Token: 0x0600117A RID: 4474 RVA: 0x0005655C File Offset: 0x0005475C
		public void OnDisconnected(DisconnectCause cause)
		{
			Debug.Log("OnDisconnected(" + cause.ToString() + ")");
		}

		// Token: 0x0600117B RID: 4475 RVA: 0x0005657F File Offset: 0x0005477F
		public void OnCustomAuthenticationResponse(Dictionary<string, object> data)
		{
		}

		// Token: 0x0600117C RID: 4476 RVA: 0x00056581 File Offset: 0x00054781
		public void OnCustomAuthenticationFailed(string debugMessage)
		{
		}

		// Token: 0x0600117D RID: 4477 RVA: 0x00056583 File Offset: 0x00054783
		public void OnRegionListReceived(RegionHandler regionHandler)
		{
			Debug.Log("OnRegionListReceived");
			regionHandler.PingMinimumOfRegions(new Action<RegionHandler>(this.OnRegionPingCompleted), null);
		}

		// Token: 0x0600117E RID: 4478 RVA: 0x000565A3 File Offset: 0x000547A3
		public void OnRoomListUpdate(List<RoomInfo> roomList)
		{
		}

		// Token: 0x0600117F RID: 4479 RVA: 0x000565A5 File Offset: 0x000547A5
		public void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics)
		{
		}

		// Token: 0x06001180 RID: 4480 RVA: 0x000565A7 File Offset: 0x000547A7
		public void OnJoinedLobby()
		{
		}

		// Token: 0x06001181 RID: 4481 RVA: 0x000565A9 File Offset: 0x000547A9
		public void OnLeftLobby()
		{
		}

		// Token: 0x06001182 RID: 4482 RVA: 0x000565AB File Offset: 0x000547AB
		public void OnFriendListUpdate(List<FriendInfo> friendList)
		{
		}

		// Token: 0x06001183 RID: 4483 RVA: 0x000565AD File Offset: 0x000547AD
		public void OnCreatedRoom()
		{
		}

		// Token: 0x06001184 RID: 4484 RVA: 0x000565AF File Offset: 0x000547AF
		public void OnCreateRoomFailed(short returnCode, string message)
		{
		}

		// Token: 0x06001185 RID: 4485 RVA: 0x000565B1 File Offset: 0x000547B1
		public void OnJoinedRoom()
		{
			Debug.Log("OnJoinedRoom");
		}

		// Token: 0x06001186 RID: 4486 RVA: 0x000565BD File Offset: 0x000547BD
		public void OnJoinRoomFailed(short returnCode, string message)
		{
		}

		// Token: 0x06001187 RID: 4487 RVA: 0x000565BF File Offset: 0x000547BF
		public void OnJoinRandomFailed(short returnCode, string message)
		{
			Debug.Log("OnJoinRandomFailed");
			this.lbc.OpCreateRoom(new EnterRoomParams());
		}

		// Token: 0x06001188 RID: 4488 RVA: 0x000565DC File Offset: 0x000547DC
		public void OnLeftRoom()
		{
		}

		// Token: 0x06001189 RID: 4489 RVA: 0x000565E0 File Offset: 0x000547E0
		private void OnRegionPingCompleted(RegionHandler regionHandler)
		{
			string text = "OnRegionPingCompleted ";
			Region bestRegion = regionHandler.BestRegion;
			Debug.Log(text + ((bestRegion != null) ? bestRegion.ToString() : null));
			Debug.Log("RegionPingSummary: " + regionHandler.SummaryToCache);
			this.lbc.ConnectToRegionMaster(regionHandler.BestRegion.Code);
		}

		// Token: 0x04000FF7 RID: 4087
		[SerializeField]
		private AppSettings appSettings = new AppSettings();

		// Token: 0x04000FF8 RID: 4088
		private LoadBalancingClient lbc;

		// Token: 0x04000FF9 RID: 4089
		private ConnectionHandler ch;

		// Token: 0x04000FFA RID: 4090
		public Text StateUiText;
	}
}
