using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

namespace Photon.Chat.Demo
{
	// Token: 0x020002C8 RID: 712
	public class ChatGui : MonoBehaviour, IChatClientListener
	{
		// Token: 0x170000CE RID: 206
		// (get) Token: 0x06001191 RID: 4497 RVA: 0x0005675F File Offset: 0x0005495F
		// (set) Token: 0x06001192 RID: 4498 RVA: 0x00056767 File Offset: 0x00054967
		public string UserName { get; set; }

		// Token: 0x06001193 RID: 4499 RVA: 0x00056770 File Offset: 0x00054970
		public void Start()
		{
			Object.DontDestroyOnLoad(base.gameObject);
			this.UserIdText.text = "";
			this.StateText.text = "";
			this.StateText.gameObject.SetActive(true);
			this.UserIdText.gameObject.SetActive(true);
			this.Title.SetActive(true);
			this.ChatPanel.gameObject.SetActive(false);
			this.ConnectingLabel.SetActive(false);
			if (string.IsNullOrEmpty(this.UserName))
			{
				this.UserName = "user" + (Environment.TickCount % 99).ToString();
			}
			this.chatAppSettings = PhotonNetwork.PhotonServerSettings.AppSettings.GetChatSettings();
			bool flag = !string.IsNullOrEmpty(this.chatAppSettings.AppIdChat);
			this.missingAppIdErrorPanel.SetActive(!flag);
			this.UserIdFormPanel.gameObject.SetActive(flag);
			if (!flag)
			{
				Debug.LogError("You need to set the chat app ID in the PhotonServerSettings file in order to continue.");
			}
		}

		// Token: 0x06001194 RID: 4500 RVA: 0x00056878 File Offset: 0x00054A78
		public void Connect()
		{
			this.UserIdFormPanel.gameObject.SetActive(false);
			this.chatClient = new ChatClient(this, ConnectionProtocol.Udp);
			this.chatClient.UseBackgroundWorkerForSending = true;
			this.chatClient.AuthValues = new AuthenticationValues(this.UserName);
			this.chatClient.ConnectUsingSettings(this.chatAppSettings);
			this.ChannelToggleToInstantiate.gameObject.SetActive(false);
			Debug.Log("Connecting as: " + this.UserName);
			this.ConnectingLabel.SetActive(true);
		}

		// Token: 0x06001195 RID: 4501 RVA: 0x00056909 File Offset: 0x00054B09
		public void OnDestroy()
		{
			if (this.chatClient != null)
			{
				this.chatClient.Disconnect(ChatDisconnectCause.DisconnectByClientLogic);
			}
		}

		// Token: 0x06001196 RID: 4502 RVA: 0x00056920 File Offset: 0x00054B20
		public void OnApplicationQuit()
		{
			if (this.chatClient != null)
			{
				this.chatClient.Disconnect(ChatDisconnectCause.DisconnectByClientLogic);
			}
		}

		// Token: 0x06001197 RID: 4503 RVA: 0x00056938 File Offset: 0x00054B38
		public void Update()
		{
			if (this.chatClient != null)
			{
				this.chatClient.Service();
			}
			if (this.StateText == null)
			{
				Object.Destroy(base.gameObject);
				return;
			}
			this.StateText.gameObject.SetActive(this.ShowState);
		}

		// Token: 0x06001198 RID: 4504 RVA: 0x00056988 File Offset: 0x00054B88
		public void OnEnterSend()
		{
			if (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter))
			{
				this.SendChatMessage(this.InputFieldChat.text);
				this.InputFieldChat.text = "";
			}
		}

		// Token: 0x06001199 RID: 4505 RVA: 0x000569C0 File Offset: 0x00054BC0
		public void OnClickSend()
		{
			if (this.InputFieldChat != null)
			{
				this.SendChatMessage(this.InputFieldChat.text);
				this.InputFieldChat.text = "";
			}
		}

		// Token: 0x0600119A RID: 4506 RVA: 0x000569F4 File Offset: 0x00054BF4
		private void SendChatMessage(string inputLine)
		{
			if (string.IsNullOrEmpty(inputLine))
			{
				return;
			}
			if ("test".Equals(inputLine))
			{
				if (this.TestLength != this.testBytes.Length)
				{
					this.testBytes = new byte[this.TestLength];
				}
				this.chatClient.SendPrivateMessage(this.chatClient.AuthValues.UserId, this.testBytes, true);
			}
			bool flag = this.chatClient.PrivateChannels.ContainsKey(this.selectedChannelName);
			string text = string.Empty;
			if (flag)
			{
				text = this.selectedChannelName.Split(new char[] { ':' })[1];
			}
			if (inputLine[0].Equals('\\'))
			{
				string[] array = inputLine.Split(new char[] { ' ' }, 2);
				if (array[0].Equals("\\help"))
				{
					this.PostHelpToCurrentChannel();
				}
				if (array[0].Equals("\\state"))
				{
					int num = 0;
					List<string> list = new List<string>();
					list.Add("i am state " + num.ToString());
					string[] array2 = array[1].Split(new char[] { ' ', ',' });
					if (array2.Length != 0)
					{
						num = int.Parse(array2[0]);
					}
					if (array2.Length > 1)
					{
						list.Add(array2[1]);
					}
					this.chatClient.SetOnlineStatus(num, list.ToArray());
					return;
				}
				if ((array[0].Equals("\\subscribe") || array[0].Equals("\\s")) && !string.IsNullOrEmpty(array[1]))
				{
					this.chatClient.Subscribe(array[1].Split(new char[] { ' ', ',' }));
					return;
				}
				if ((array[0].Equals("\\unsubscribe") || array[0].Equals("\\u")) && !string.IsNullOrEmpty(array[1]))
				{
					this.chatClient.Unsubscribe(array[1].Split(new char[] { ' ', ',' }));
					return;
				}
				if (array[0].Equals("\\clear"))
				{
					if (flag)
					{
						this.chatClient.PrivateChannels.Remove(this.selectedChannelName);
						return;
					}
					ChatChannel chatChannel;
					if (this.chatClient.TryGetChannel(this.selectedChannelName, flag, out chatChannel))
					{
						chatChannel.ClearMessages();
						return;
					}
				}
				else if (array[0].Equals("\\msg") && !string.IsNullOrEmpty(array[1]))
				{
					string[] array3 = array[1].Split(new char[] { ' ', ',' }, 2);
					if (array3.Length < 2)
					{
						return;
					}
					string text2 = array3[0];
					string text3 = array3[1];
					this.chatClient.SendPrivateMessage(text2, text3, false);
					return;
				}
				else
				{
					if ((!array[0].Equals("\\join") && !array[0].Equals("\\j")) || string.IsNullOrEmpty(array[1]))
					{
						Debug.Log("The command '" + array[0] + "' is invalid.");
						return;
					}
					string[] array4 = array[1].Split(new char[] { ' ', ',' }, 2);
					if (this.channelToggles.ContainsKey(array4[0]))
					{
						this.ShowChannel(array4[0]);
						return;
					}
					this.chatClient.Subscribe(new string[] { array4[0] });
					return;
				}
			}
			else
			{
				if (flag)
				{
					this.chatClient.SendPrivateMessage(text, inputLine, false);
					return;
				}
				this.chatClient.PublishMessage(this.selectedChannelName, inputLine, false);
			}
		}

		// Token: 0x0600119B RID: 4507 RVA: 0x00056D4C File Offset: 0x00054F4C
		public void PostHelpToCurrentChannel()
		{
			Text currentChannelText = this.CurrentChannelText;
			currentChannelText.text += ChatGui.HelpText;
		}

		// Token: 0x0600119C RID: 4508 RVA: 0x00056D69 File Offset: 0x00054F69
		public void DebugReturn(DebugLevel level, string message)
		{
			if (level == DebugLevel.ERROR)
			{
				Debug.LogError(message);
				return;
			}
			if (level == DebugLevel.WARNING)
			{
				Debug.LogWarning(message);
				return;
			}
			Debug.Log(message);
		}

		// Token: 0x0600119D RID: 4509 RVA: 0x00056D88 File Offset: 0x00054F88
		public void OnConnected()
		{
			if (this.ChannelsToJoinOnConnect != null && this.ChannelsToJoinOnConnect.Length != 0)
			{
				this.chatClient.Subscribe(this.ChannelsToJoinOnConnect, this.HistoryLengthToFetch);
			}
			this.ConnectingLabel.SetActive(false);
			this.UserIdText.text = "Connected as " + this.UserName;
			this.ChatPanel.gameObject.SetActive(true);
			if (this.FriendsList != null && this.FriendsList.Length != 0)
			{
				this.chatClient.AddFriends(this.FriendsList);
				foreach (string text in this.FriendsList)
				{
					if (this.FriendListUiItemtoInstantiate != null && text != this.UserName)
					{
						this.InstantiateFriendButton(text);
					}
				}
			}
			if (this.FriendListUiItemtoInstantiate != null)
			{
				this.FriendListUiItemtoInstantiate.SetActive(false);
			}
			this.chatClient.SetOnlineStatus(2);
		}

		// Token: 0x0600119E RID: 4510 RVA: 0x00056E7C File Offset: 0x0005507C
		public void OnDisconnected()
		{
			Debug.Log("OnDisconnected()");
			this.ConnectingLabel.SetActive(false);
		}

		// Token: 0x0600119F RID: 4511 RVA: 0x00056E94 File Offset: 0x00055094
		public void OnChatStateChange(ChatState state)
		{
			this.StateText.text = state.ToString();
		}

		// Token: 0x060011A0 RID: 4512 RVA: 0x00056EB0 File Offset: 0x000550B0
		public void OnSubscribed(string[] channels, bool[] results)
		{
			foreach (string text in channels)
			{
				this.chatClient.PublishMessage(text, "says 'hi'.", false);
				if (this.ChannelToggleToInstantiate != null)
				{
					this.InstantiateChannelButton(text);
				}
			}
			Debug.Log("OnSubscribed: " + string.Join(", ", channels));
			this.ShowChannel(channels[0]);
		}

		// Token: 0x060011A1 RID: 4513 RVA: 0x00056F1C File Offset: 0x0005511C
		public void OnSubscribed(string channel, string[] users, Dictionary<object, object> properties)
		{
			Debug.LogFormat("OnSubscribed: {0}, users.Count: {1} Channel-props: {2}.", new object[]
			{
				channel,
				users.Length,
				properties.ToStringFull()
			});
		}

		// Token: 0x060011A2 RID: 4514 RVA: 0x00056F48 File Offset: 0x00055148
		private void InstantiateChannelButton(string channelName)
		{
			if (this.channelToggles.ContainsKey(channelName))
			{
				Debug.Log("Skipping creation for an existing channel toggle.");
				return;
			}
			Toggle toggle = Object.Instantiate<Toggle>(this.ChannelToggleToInstantiate);
			toggle.gameObject.SetActive(true);
			toggle.GetComponentInChildren<ChannelSelector>().SetChannel(channelName);
			toggle.transform.SetParent(this.ChannelToggleToInstantiate.transform.parent, false);
			this.channelToggles.Add(channelName, toggle);
		}

		// Token: 0x060011A3 RID: 4515 RVA: 0x00056FBC File Offset: 0x000551BC
		private void InstantiateFriendButton(string friendId)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(this.FriendListUiItemtoInstantiate);
			gameObject.gameObject.SetActive(true);
			FriendItem component = gameObject.GetComponent<FriendItem>();
			component.FriendId = friendId;
			gameObject.transform.SetParent(this.FriendListUiItemtoInstantiate.transform.parent, false);
			this.friendListItemLUT[friendId] = component;
		}

		// Token: 0x060011A4 RID: 4516 RVA: 0x00057018 File Offset: 0x00055218
		public void OnUnsubscribed(string[] channels)
		{
			foreach (string text in channels)
			{
				if (this.channelToggles.ContainsKey(text))
				{
					Object.Destroy(this.channelToggles[text].gameObject);
					this.channelToggles.Remove(text);
					Debug.Log("Unsubscribed from channel '" + text + "'.");
					if (text == this.selectedChannelName && this.channelToggles.Count > 0)
					{
						IEnumerator<KeyValuePair<string, Toggle>> enumerator = this.channelToggles.GetEnumerator();
						enumerator.MoveNext();
						KeyValuePair<string, Toggle> keyValuePair = enumerator.Current;
						this.ShowChannel(keyValuePair.Key);
						keyValuePair = enumerator.Current;
						keyValuePair.Value.isOn = true;
					}
				}
				else
				{
					Debug.Log("Can't unsubscribe from channel '" + text + "' because you are currently not subscribed to it.");
				}
			}
		}

		// Token: 0x060011A5 RID: 4517 RVA: 0x000570FD File Offset: 0x000552FD
		public void OnGetMessages(string channelName, string[] senders, object[] messages)
		{
			if (channelName.Equals(this.selectedChannelName))
			{
				this.ShowChannel(this.selectedChannelName);
			}
		}

		// Token: 0x060011A6 RID: 4518 RVA: 0x0005711C File Offset: 0x0005531C
		public void OnPrivateMessage(string sender, object message, string channelName)
		{
			this.InstantiateChannelButton(channelName);
			byte[] array = message as byte[];
			if (array != null)
			{
				Debug.Log("Message with byte[].Length: " + array.Length.ToString());
			}
			if (this.selectedChannelName.Equals(channelName))
			{
				this.ShowChannel(channelName);
			}
		}

		// Token: 0x060011A7 RID: 4519 RVA: 0x0005716C File Offset: 0x0005536C
		public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
		{
			Debug.LogWarning("status: " + string.Format("{0} is {1}. Msg:{2}", user, status, message));
			if (this.friendListItemLUT.ContainsKey(user))
			{
				FriendItem friendItem = this.friendListItemLUT[user];
				if (friendItem != null)
				{
					friendItem.OnFriendStatusUpdate(status, gotMessage, message);
				}
			}
		}

		// Token: 0x060011A8 RID: 4520 RVA: 0x000571C9 File Offset: 0x000553C9
		public void OnUserSubscribed(string channel, string user)
		{
			Debug.LogFormat("OnUserSubscribed: channel=\"{0}\" userId=\"{1}\"", new object[] { channel, user });
		}

		// Token: 0x060011A9 RID: 4521 RVA: 0x000571E3 File Offset: 0x000553E3
		public void OnUserUnsubscribed(string channel, string user)
		{
			Debug.LogFormat("OnUserUnsubscribed: channel=\"{0}\" userId=\"{1}\"", new object[] { channel, user });
		}

		// Token: 0x060011AA RID: 4522 RVA: 0x000571FD File Offset: 0x000553FD
		public void OnChannelPropertiesChanged(string channel, string userId, Dictionary<object, object> properties)
		{
			Debug.LogFormat("OnChannelPropertiesChanged: {0} by {1}. Props: {2}.", new object[]
			{
				channel,
				userId,
				properties.ToStringFull()
			});
		}

		// Token: 0x060011AB RID: 4523 RVA: 0x00057220 File Offset: 0x00055420
		public void OnUserPropertiesChanged(string channel, string targetUserId, string senderUserId, Dictionary<object, object> properties)
		{
			Debug.LogFormat("OnUserPropertiesChanged: (channel:{0} user:{1}) by {2}. Props: {3}.", new object[]
			{
				channel,
				targetUserId,
				senderUserId,
				properties.ToStringFull()
			});
		}

		// Token: 0x060011AC RID: 4524 RVA: 0x00057248 File Offset: 0x00055448
		public void OnErrorInfo(string channel, string error, object data)
		{
			Debug.LogFormat("OnErrorInfo for channel {0}. Error: {1} Data: {2}", new object[] { channel, error, data });
		}

		// Token: 0x060011AD RID: 4525 RVA: 0x00057268 File Offset: 0x00055468
		public void AddMessageToSelectedChannel(string msg)
		{
			ChatChannel chatChannel = null;
			if (!this.chatClient.TryGetChannel(this.selectedChannelName, out chatChannel))
			{
				Debug.Log("AddMessageToSelectedChannel failed to find channel: " + this.selectedChannelName);
				return;
			}
			if (chatChannel != null)
			{
				chatChannel.Add("Bot", msg, 0);
			}
		}

		// Token: 0x060011AE RID: 4526 RVA: 0x000572B4 File Offset: 0x000554B4
		public void ShowChannel(string channelName)
		{
			if (string.IsNullOrEmpty(channelName))
			{
				return;
			}
			ChatChannel chatChannel = null;
			if (!this.chatClient.TryGetChannel(channelName, out chatChannel))
			{
				Debug.Log("ShowChannel failed to find channel: " + channelName);
				return;
			}
			this.selectedChannelName = channelName;
			this.CurrentChannelText.text = chatChannel.ToStringMessages();
			Debug.Log("ShowChannel: " + this.selectedChannelName);
			foreach (KeyValuePair<string, Toggle> keyValuePair in this.channelToggles)
			{
				keyValuePair.Value.isOn = keyValuePair.Key == channelName;
			}
		}

		// Token: 0x060011AF RID: 4527 RVA: 0x00057378 File Offset: 0x00055578
		public void OpenDashboard()
		{
			Application.OpenURL("https://dashboard.photonengine.com");
		}

		// Token: 0x04000FFE RID: 4094
		public string[] ChannelsToJoinOnConnect;

		// Token: 0x04000FFF RID: 4095
		public string[] FriendsList;

		// Token: 0x04001000 RID: 4096
		public int HistoryLengthToFetch;

		// Token: 0x04001002 RID: 4098
		private string selectedChannelName;

		// Token: 0x04001003 RID: 4099
		public ChatClient chatClient;

		// Token: 0x04001004 RID: 4100
		protected internal ChatAppSettings chatAppSettings;

		// Token: 0x04001005 RID: 4101
		public GameObject missingAppIdErrorPanel;

		// Token: 0x04001006 RID: 4102
		public GameObject ConnectingLabel;

		// Token: 0x04001007 RID: 4103
		public RectTransform ChatPanel;

		// Token: 0x04001008 RID: 4104
		public GameObject UserIdFormPanel;

		// Token: 0x04001009 RID: 4105
		public InputField InputFieldChat;

		// Token: 0x0400100A RID: 4106
		public Text CurrentChannelText;

		// Token: 0x0400100B RID: 4107
		public Toggle ChannelToggleToInstantiate;

		// Token: 0x0400100C RID: 4108
		public GameObject FriendListUiItemtoInstantiate;

		// Token: 0x0400100D RID: 4109
		private readonly Dictionary<string, Toggle> channelToggles = new Dictionary<string, Toggle>();

		// Token: 0x0400100E RID: 4110
		private readonly Dictionary<string, FriendItem> friendListItemLUT = new Dictionary<string, FriendItem>();

		// Token: 0x0400100F RID: 4111
		public bool ShowState = true;

		// Token: 0x04001010 RID: 4112
		public GameObject Title;

		// Token: 0x04001011 RID: 4113
		public Text StateText;

		// Token: 0x04001012 RID: 4114
		public Text UserIdText;

		// Token: 0x04001013 RID: 4115
		private static string HelpText = "\n    -- HELP --\nTo subscribe to channel(s) (channelnames are case sensitive) :  \n\t<color=#E07B00>\\subscribe</color> <color=green><list of channelnames></color>\n\tor\n\t<color=#E07B00>\\s</color> <color=green><list of channelnames></color>\n\nTo leave channel(s):\n\t<color=#E07B00>\\unsubscribe</color> <color=green><list of channelnames></color>\n\tor\n\t<color=#E07B00>\\u</color> <color=green><list of channelnames></color>\n\nTo switch the active channel\n\t<color=#E07B00>\\join</color> <color=green><channelname></color>\n\tor\n\t<color=#E07B00>\\j</color> <color=green><channelname></color>\n\nTo send a private message: (username are case sensitive)\n\t\\<color=#E07B00>msg</color> <color=green><username></color> <color=green><message></color>\n\nTo change status:\n\t\\<color=#E07B00>state</color> <color=green><stateIndex></color> <color=green><message></color>\n<color=green>0</color> = Offline <color=green>1</color> = Invisible <color=green>2</color> = Online <color=green>3</color> = Away \n<color=green>4</color> = Do not disturb <color=green>5</color> = Looking For Group <color=green>6</color> = Playing\n\nTo clear the current chat tab (private chats get closed):\n\t<color=#E07B00>\\clear</color>";

		// Token: 0x04001014 RID: 4116
		public int TestLength = 2048;

		// Token: 0x04001015 RID: 4117
		private byte[] testBytes = new byte[2048];
	}
}
