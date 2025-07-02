using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Voice.Unity.UtilityScripts;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Photon.Voice.Unity.Demos.DemoVoiceUI
{
	// Token: 0x020002BE RID: 702
	[RequireComponent(typeof(UnityVoiceClient), typeof(ConnectAndJoin))]
	public class DemoVoiceUI : MonoBehaviour, IInRoomCallbacks, IMatchmakingCallbacks
	{
		// Token: 0x06001111 RID: 4369 RVA: 0x00054830 File Offset: 0x00052A30
		private void Start()
		{
			this.connectAndJoin = base.GetComponent<ConnectAndJoin>();
			this.voiceConnection = base.GetComponent<UnityVoiceClient>();
			this.voiceAudioPreprocessor = this.voiceConnection.PrimaryRecorder.GetComponent<WebRtcAudioDsp>();
			this.compressionGainGameObject = this.agcCompressionGainSlider.transform.parent.gameObject;
			this.compressionGainText = this.compressionGainGameObject.GetComponentInChildren<Text>();
			this.targetLevelGameObject = this.agcTargetLevelSlider.transform.parent.gameObject;
			this.targetLevelText = this.targetLevelGameObject.GetComponentInChildren<Text>();
			this.aecOptionsGameObject = this.aecHighPassToggle.transform.parent.gameObject;
			this.SetDefaults();
			this.InitUiCallbacks();
			this.GetSavedNickname();
			this.voiceConnection.PrimaryRecorder.InputFactory = this.toneInputFactory;
			this.voiceConnection.SpeakerLinked += this.OnSpeakerCreated;
			this.voiceConnection.Client.AddCallbackTarget(this);
		}

		// Token: 0x06001112 RID: 4370 RVA: 0x0005492F File Offset: 0x00052B2F
		protected virtual void SetDefaults()
		{
			this.muteToggle.isOn = !this.defaultTransmitEnabled;
		}

		// Token: 0x06001113 RID: 4371 RVA: 0x00054945 File Offset: 0x00052B45
		private void OnDestroy()
		{
			this.voiceConnection.SpeakerLinked -= this.OnSpeakerCreated;
			this.voiceConnection.Client.RemoveCallbackTarget(this);
		}

		// Token: 0x06001114 RID: 4372 RVA: 0x00054970 File Offset: 0x00052B70
		private void GetSavedNickname()
		{
			string @string = PlayerPrefs.GetString("vNick");
			if (!string.IsNullOrEmpty(@string))
			{
				this.localNicknameText.text = @string;
				this.voiceConnection.Client.NickName = @string;
			}
		}

		// Token: 0x06001115 RID: 4373 RVA: 0x000549B0 File Offset: 0x00052BB0
		protected virtual void OnSpeakerCreated(Speaker speaker)
		{
			speaker.gameObject.transform.SetParent(this.RemoteVoicesPanel, false);
			speaker.GetComponent<RemoteSpeakerUI>().Init(this.voiceConnection);
			speaker.OnRemoteVoiceRemoveAction = (Action<Speaker>)Delegate.Combine(speaker.OnRemoteVoiceRemoveAction, new Action<Speaker>(this.OnRemoteVoiceRemove));
		}

		// Token: 0x06001116 RID: 4374 RVA: 0x00054A07 File Offset: 0x00052C07
		private void OnRemoteVoiceRemove(Speaker speaker)
		{
			if (speaker != null)
			{
				Object.Destroy(speaker.gameObject);
			}
		}

		// Token: 0x06001117 RID: 4375 RVA: 0x00054A20 File Offset: 0x00052C20
		private void ToggleMute(bool isOn)
		{
			this.muteToggle.targetGraphic.enabled = !isOn;
			if (isOn)
			{
				this.voiceConnection.Client.LocalPlayer.Mute();
				return;
			}
			this.voiceConnection.Client.LocalPlayer.Unmute();
		}

		// Token: 0x06001118 RID: 4376 RVA: 0x00054A71 File Offset: 0x00052C71
		protected virtual void ToggleIsRecording(bool isRecording)
		{
			this.voiceConnection.PrimaryRecorder.RecordingEnabled = isRecording;
		}

		// Token: 0x06001119 RID: 4377 RVA: 0x00054A84 File Offset: 0x00052C84
		private void ToggleDebugEcho(bool isOn)
		{
			this.voiceConnection.PrimaryRecorder.DebugEchoMode = isOn;
		}

		// Token: 0x0600111A RID: 4378 RVA: 0x00054A97 File Offset: 0x00052C97
		private void ToggleReliable(bool isOn)
		{
			this.voiceConnection.PrimaryRecorder.ReliableMode = isOn;
		}

		// Token: 0x0600111B RID: 4379 RVA: 0x00054AAA File Offset: 0x00052CAA
		private void ToggleEncryption(bool isOn)
		{
			this.voiceConnection.PrimaryRecorder.Encrypt = isOn;
		}

		// Token: 0x0600111C RID: 4380 RVA: 0x00054ABD File Offset: 0x00052CBD
		private void ToggleAEC(bool isOn)
		{
			this.voiceAudioPreprocessor.AEC = isOn;
			this.aecOptionsGameObject.SetActive(isOn);
			this.voiceConnection.Client.LocalPlayer.SetAEC(isOn);
		}

		// Token: 0x0600111D RID: 4381 RVA: 0x00054AEE File Offset: 0x00052CEE
		private void ToggleNoiseSuppression(bool isOn)
		{
			this.voiceAudioPreprocessor.NoiseSuppression = isOn;
		}

		// Token: 0x0600111E RID: 4382 RVA: 0x00054AFC File Offset: 0x00052CFC
		private void ToggleAGC(bool isOn)
		{
			this.voiceAudioPreprocessor.AGC = isOn;
			this.compressionGainGameObject.SetActive(isOn);
			this.targetLevelGameObject.SetActive(isOn);
			this.voiceConnection.Client.LocalPlayer.SetAGC(isOn, this.voiceAudioPreprocessor.AgcCompressionGain, this.voiceAudioPreprocessor.AgcTargetLevel);
		}

		// Token: 0x0600111F RID: 4383 RVA: 0x00054B5A File Offset: 0x00052D5A
		private void ToggleVAD(bool isOn)
		{
			this.voiceAudioPreprocessor.VAD = isOn;
			this.voiceConnection.Client.LocalPlayer.SetWebRTCVAD(isOn);
		}

		// Token: 0x06001120 RID: 4384 RVA: 0x00054B7F File Offset: 0x00052D7F
		private void ToggleHighPass(bool isOn)
		{
			this.voiceAudioPreprocessor.HighPass = isOn;
		}

		// Token: 0x06001121 RID: 4385 RVA: 0x00054B90 File Offset: 0x00052D90
		private void ToggleDsp(bool isOn)
		{
			this.voiceAudioPreprocessor.enabled = isOn;
			this.voiceConnection.PrimaryRecorder.RestartRecording();
			this.webRtcDspGameObject.SetActive(isOn);
			this.voiceConnection.Client.LocalPlayer.SetWebRTCVAD(this.voiceAudioPreprocessor.VAD);
			this.voiceConnection.Client.LocalPlayer.SetAEC(this.voiceAudioPreprocessor.AEC);
			this.voiceConnection.Client.LocalPlayer.SetAGC(this.voiceAudioPreprocessor.AGC, this.voiceAudioPreprocessor.AgcCompressionGain, this.voiceAudioPreprocessor.AgcTargetLevel);
		}

		// Token: 0x06001122 RID: 4386 RVA: 0x00054C3F File Offset: 0x00052E3F
		private void ToggleAudioClipStreaming(bool isOn)
		{
			if (isOn)
			{
				this.audioToneToggle.SetValue(false);
				this.voiceConnection.PrimaryRecorder.SourceType = Recorder.InputSourceType.AudioClip;
				return;
			}
			if (!this.audioToneToggle.isOn)
			{
				this.microphoneSelector.SwitchToSelectedMic();
			}
		}

		// Token: 0x06001123 RID: 4387 RVA: 0x00054C7C File Offset: 0x00052E7C
		private void ToggleAudioToneFactory(bool isOn)
		{
			if (isOn)
			{
				this.streamAudioClipToggle.SetValue(false);
				this.voiceConnection.PrimaryRecorder.SourceType = Recorder.InputSourceType.Factory;
				this.voiceConnection.PrimaryRecorder.InputFactory = this.toneInputFactory;
				return;
			}
			if (!this.streamAudioClipToggle.isOn)
			{
				this.microphoneSelector.SwitchToSelectedMic();
			}
		}

		// Token: 0x06001124 RID: 4388 RVA: 0x00054CD8 File Offset: 0x00052ED8
		private void TogglePhotonVAD(bool isOn)
		{
			this.voiceConnection.PrimaryRecorder.VoiceDetection = isOn;
			this.voiceConnection.Client.LocalPlayer.SetPhotonVAD(isOn);
		}

		// Token: 0x06001125 RID: 4389 RVA: 0x00054D02 File Offset: 0x00052F02
		private void ToggleAecHighPass(bool isOn)
		{
			this.voiceAudioPreprocessor.AecHighPass = isOn;
			this.voiceConnection.Client.LocalPlayer.SetAEC(isOn);
		}

		// Token: 0x06001126 RID: 4390 RVA: 0x00054D28 File Offset: 0x00052F28
		private void OnAgcCompressionGainChanged(float agcCompressionGain)
		{
			this.voiceAudioPreprocessor.AgcCompressionGain = (int)agcCompressionGain;
			this.compressionGainText.text = "Compression Gain: " + agcCompressionGain;
			this.voiceConnection.Client.LocalPlayer.SetAGC(this.voiceAudioPreprocessor.AGC, (int)agcCompressionGain, this.voiceAudioPreprocessor.AgcTargetLevel);
		}

		// Token: 0x06001127 RID: 4391 RVA: 0x00054D8C File Offset: 0x00052F8C
		private void OnAgcTargetLevelChanged(float agcTargetLevel)
		{
			this.voiceAudioPreprocessor.AgcTargetLevel = (int)agcTargetLevel;
			this.targetLevelText.text = "Target Level: " + agcTargetLevel;
			this.voiceConnection.Client.LocalPlayer.SetAGC(this.voiceAudioPreprocessor.AGC, this.voiceAudioPreprocessor.AgcCompressionGain, (int)agcTargetLevel);
		}

		// Token: 0x06001128 RID: 4392 RVA: 0x00054DF0 File Offset: 0x00052FF0
		private void OnReverseStreamDelayChanged(string newReverseStreamString)
		{
			int num;
			if (int.TryParse(newReverseStreamString, out num) && num > 0)
			{
				this.voiceAudioPreprocessor.ReverseStreamDelayMs = num;
				return;
			}
			this.reverseStreamDelayInputField.text = this.voiceAudioPreprocessor.ReverseStreamDelayMs.ToString();
		}

		// Token: 0x06001129 RID: 4393 RVA: 0x00054E36 File Offset: 0x00053036
		private void OnMicrophoneChanged(Recorder.MicType micType, DeviceInfo deviceInfo)
		{
			this.voiceConnection.Client.LocalPlayer.SetMic(micType);
			this.androidMicSettingGameObject.SetActive(micType == Recorder.MicType.Photon);
		}

		// Token: 0x0600112A RID: 4394 RVA: 0x00054E5E File Offset: 0x0005305E
		private void OnAndroidMicSettingsChanged(bool isOn)
		{
			this.voiceConnection.PrimaryRecorder.SetAndroidNativeMicrophoneSettings(this.androidAecToggle.isOn, this.androidAgcToggle.isOn, this.androidNsToggle.isOn);
		}

		// Token: 0x0600112B RID: 4395 RVA: 0x00054E92 File Offset: 0x00053092
		private void UpdateSyncedNickname(string nickname)
		{
			nickname = nickname.Trim();
			this.voiceConnection.Client.LocalPlayer.NickName = nickname;
			PlayerPrefs.SetString("vNick", nickname);
		}

		// Token: 0x0600112C RID: 4396 RVA: 0x00054EC0 File Offset: 0x000530C0
		private void JoinOrCreateRoom(string roomName)
		{
			if (string.IsNullOrEmpty(roomName))
			{
				this.connectAndJoin.RoomName = string.Empty;
				this.connectAndJoin.RandomRoom = true;
			}
			else
			{
				this.connectAndJoin.RoomName = roomName.Trim();
				this.connectAndJoin.RandomRoom = false;
			}
			if (this.voiceConnection.Client.InRoom)
			{
				this.voiceConnection.Client.OpLeaveRoom(false, false);
				return;
			}
			if (!this.voiceConnection.Client.IsConnected)
			{
				this.voiceConnection.ConnectUsingSettings(null);
			}
		}

		// Token: 0x0600112D RID: 4397 RVA: 0x00054F55 File Offset: 0x00053155
		private void PhotonVoiceCreated(PhotonVoiceCreatedParams p)
		{
			this.InitUiValues();
		}

		// Token: 0x0600112E RID: 4398 RVA: 0x00054F60 File Offset: 0x00053160
		protected virtual void Update()
		{
			this.connectionStatusText.text = this.voiceConnection.Client.State.ToString();
			this.serverStatusText.text = string.Format("{0}/{1}", this.voiceConnection.Client.CloudRegion, this.voiceConnection.Client.CurrentServerAddress);
			if (this.voiceConnection.PrimaryRecorder.IsCurrentlyTransmitting)
			{
				float num = this.voiceConnection.PrimaryRecorder.LevelMeter.CurrentAvgAmp;
				if (num > 1f)
				{
					num /= 32768f;
				}
				if ((double)num > 0.1)
				{
					this.inputWarningText.text = "Input too loud!";
					this.inputWarningText.color = this.warningColor;
				}
				else
				{
					this.inputWarningText.text = string.Empty;
					this.ResetTextColor(this.inputWarningText);
				}
			}
			if (this.voiceConnection.FramesReceivedPerSecond > 0f)
			{
				this.packetLossWarningText.text = string.Format("{0:0.##}% Packet Loss", this.voiceConnection.FramesLostPercent);
				this.packetLossWarningText.color = ((this.voiceConnection.FramesLostPercent > 1f) ? this.warningColor : this.okColor);
			}
			else
			{
				this.packetLossWarningText.text = string.Empty;
				this.ResetTextColor(this.packetLossWarningText);
			}
			this.rttText.text = "RTT:" + this.voiceConnection.Client.LoadBalancingPeer.RoundTripTime;
			this.SetTextColor(this.voiceConnection.Client.LoadBalancingPeer.RoundTripTime, this.rttText, this.rttYellowThreshold, this.rttRedThreshold);
			this.rttVariationText.text = "VAR:" + this.voiceConnection.Client.LoadBalancingPeer.RoundTripTimeVariance;
			this.SetTextColor(this.voiceConnection.Client.LoadBalancingPeer.RoundTripTimeVariance, this.rttVariationText, this.rttVariationYellowThreshold, this.rttVariationRedThreshold);
		}

		// Token: 0x0600112F RID: 4399 RVA: 0x00055187 File Offset: 0x00053387
		private void SetTextColor(int textValue, Text text, int yellowThreshold, int redThreshold)
		{
			if (textValue > redThreshold)
			{
				text.color = this.redColor;
				return;
			}
			if (textValue > yellowThreshold)
			{
				text.color = this.warningColor;
				return;
			}
			text.color = this.okColor;
		}

		// Token: 0x06001130 RID: 4400 RVA: 0x000551B8 File Offset: 0x000533B8
		private void ResetTextColor(Text text)
		{
			text.color = this.defaultColor;
		}

		// Token: 0x06001131 RID: 4401 RVA: 0x000551C8 File Offset: 0x000533C8
		private void InitUiCallbacks()
		{
			this.muteToggle.SetSingleOnValueChangedCallback(new UnityAction<bool>(this.ToggleMute));
			this.debugEchoToggle.SetSingleOnValueChangedCallback(new UnityAction<bool>(this.ToggleDebugEcho));
			this.reliableTransmissionToggle.SetSingleOnValueChangedCallback(new UnityAction<bool>(this.ToggleReliable));
			this.encryptionToggle.SetSingleOnValueChangedCallback(new UnityAction<bool>(this.ToggleEncryption));
			this.streamAudioClipToggle.SetSingleOnValueChangedCallback(new UnityAction<bool>(this.ToggleAudioClipStreaming));
			this.audioToneToggle.SetSingleOnValueChangedCallback(new UnityAction<bool>(this.ToggleAudioToneFactory));
			this.photonVadToggle.SetSingleOnValueChangedCallback(new UnityAction<bool>(this.TogglePhotonVAD));
			this.vadToggle.SetSingleOnValueChangedCallback(new UnityAction<bool>(this.ToggleVAD));
			this.aecToggle.SetSingleOnValueChangedCallback(new UnityAction<bool>(this.ToggleAEC));
			this.agcToggle.SetSingleOnValueChangedCallback(new UnityAction<bool>(this.ToggleAGC));
			this.dspToggle.SetSingleOnValueChangedCallback(new UnityAction<bool>(this.ToggleDsp));
			this.highPassToggle.SetSingleOnValueChangedCallback(new UnityAction<bool>(this.ToggleHighPass));
			this.aecHighPassToggle.SetSingleOnValueChangedCallback(new UnityAction<bool>(this.ToggleAecHighPass));
			this.noiseSuppressionToggle.SetSingleOnValueChangedCallback(new UnityAction<bool>(this.ToggleNoiseSuppression));
			this.agcCompressionGainSlider.SetSingleOnValueChangedCallback(new UnityAction<float>(this.OnAgcCompressionGainChanged));
			this.agcTargetLevelSlider.SetSingleOnValueChangedCallback(new UnityAction<float>(this.OnAgcTargetLevelChanged));
			this.localNicknameText.SetSingleOnEndEditCallback(new UnityAction<string>(this.UpdateSyncedNickname));
			this.roomNameInputField.SetSingleOnEndEditCallback(new UnityAction<string>(this.JoinOrCreateRoom));
			this.reverseStreamDelayInputField.SetSingleOnEndEditCallback(new UnityAction<string>(this.OnReverseStreamDelayChanged));
			this.androidAgcToggle.SetSingleOnValueChangedCallback(new UnityAction<bool>(this.OnAndroidMicSettingsChanged));
			this.androidAecToggle.SetSingleOnValueChangedCallback(new UnityAction<bool>(this.OnAndroidMicSettingsChanged));
			this.androidNsToggle.SetSingleOnValueChangedCallback(new UnityAction<bool>(this.OnAndroidMicSettingsChanged));
		}

		// Token: 0x06001132 RID: 4402 RVA: 0x000553D0 File Offset: 0x000535D0
		private void InitUiValues()
		{
			this.muteToggle.SetValue(this.voiceConnection.Client.LocalPlayer.IsMuted());
			this.debugEchoToggle.SetValue(this.voiceConnection.PrimaryRecorder.DebugEchoMode);
			this.reliableTransmissionToggle.SetValue(this.voiceConnection.PrimaryRecorder.ReliableMode);
			this.encryptionToggle.SetValue(this.voiceConnection.PrimaryRecorder.Encrypt);
			this.streamAudioClipToggle.SetValue(this.voiceConnection.PrimaryRecorder.SourceType == Recorder.InputSourceType.AudioClip);
			this.audioToneToggle.SetValue(this.voiceConnection.PrimaryRecorder.SourceType == Recorder.InputSourceType.Factory && this.voiceConnection.PrimaryRecorder.InputFactory == this.toneInputFactory);
			this.photonVadToggle.SetValue(this.voiceConnection.PrimaryRecorder.VoiceDetection);
			this.androidAgcToggle.SetValue(this.voiceConnection.PrimaryRecorder.AndroidMicrophoneAGC);
			this.androidAecToggle.SetValue(this.voiceConnection.PrimaryRecorder.AndroidMicrophoneAEC);
			this.androidNsToggle.SetValue(this.voiceConnection.PrimaryRecorder.AndroidMicrophoneNS);
			if (this.webRtcDspGameObject != null)
			{
				this.dspToggle.gameObject.SetActive(true);
				this.dspToggle.SetValue(this.voiceAudioPreprocessor.enabled);
				this.webRtcDspGameObject.SetActive(this.dspToggle.isOn);
				this.aecToggle.SetValue(this.voiceAudioPreprocessor.AEC);
				this.aecHighPassToggle.SetValue(this.voiceAudioPreprocessor.AecHighPass);
				this.reverseStreamDelayInputField.text = this.voiceAudioPreprocessor.ReverseStreamDelayMs.ToString();
				this.aecOptionsGameObject.SetActive(this.voiceAudioPreprocessor.AEC);
				this.noiseSuppressionToggle.isOn = this.voiceAudioPreprocessor.NoiseSuppression;
				this.agcToggle.SetValue(this.voiceAudioPreprocessor.AGC);
				this.agcCompressionGainSlider.SetValue((float)this.voiceAudioPreprocessor.AgcCompressionGain);
				this.agcTargetLevelSlider.SetValue((float)this.voiceAudioPreprocessor.AgcTargetLevel);
				this.compressionGainGameObject.SetActive(this.voiceAudioPreprocessor.AGC);
				this.targetLevelGameObject.SetActive(this.voiceAudioPreprocessor.AGC);
				this.vadToggle.SetValue(this.voiceAudioPreprocessor.VAD);
				this.highPassToggle.SetValue(this.voiceAudioPreprocessor.HighPass);
				return;
			}
			this.dspToggle.gameObject.SetActive(false);
		}

		// Token: 0x06001133 RID: 4403 RVA: 0x00055688 File Offset: 0x00053888
		private void SetRoomDebugText()
		{
			string text = string.Empty;
			if (this.voiceConnection.Client.InRoom)
			{
				foreach (Photon.Realtime.Player player in this.voiceConnection.Client.CurrentRoom.Players.Values)
				{
					text += player.ToStringFull();
				}
				this.roomStatusText.text = string.Format("{0} {1}", this.voiceConnection.Client.CurrentRoom.Name, text);
			}
			else
			{
				this.roomStatusText.text = string.Empty;
			}
			this.roomStatusText.text = ((this.voiceConnection.Client.CurrentRoom == null) ? string.Empty : string.Format("{0} {1}", this.voiceConnection.Client.CurrentRoom.Name, text));
		}

		// Token: 0x06001134 RID: 4404 RVA: 0x00055790 File Offset: 0x00053990
		protected virtual void OnActorPropertiesChanged(Photon.Realtime.Player targetPlayer, Hashtable changedProps)
		{
			if (targetPlayer.IsLocal)
			{
				bool flag = targetPlayer.IsMuted();
				this.voiceConnection.PrimaryRecorder.TransmitEnabled = !flag;
				this.muteToggle.SetValue(flag);
			}
			this.SetRoomDebugText();
		}

		// Token: 0x06001135 RID: 4405 RVA: 0x000557D2 File Offset: 0x000539D2
		protected void OnApplicationQuit()
		{
			this.voiceConnection.Client.RemoveCallbackTarget(this);
		}

		// Token: 0x06001136 RID: 4406 RVA: 0x000557E5 File Offset: 0x000539E5
		void IInRoomCallbacks.OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
		{
			this.SetRoomDebugText();
		}

		// Token: 0x06001137 RID: 4407 RVA: 0x000557ED File Offset: 0x000539ED
		void IInRoomCallbacks.OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
		{
			this.SetRoomDebugText();
		}

		// Token: 0x06001138 RID: 4408 RVA: 0x000557F5 File Offset: 0x000539F5
		void IInRoomCallbacks.OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
		{
		}

		// Token: 0x06001139 RID: 4409 RVA: 0x000557F7 File Offset: 0x000539F7
		void IInRoomCallbacks.OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps)
		{
			this.OnActorPropertiesChanged(targetPlayer, changedProps);
		}

		// Token: 0x0600113A RID: 4410 RVA: 0x00055801 File Offset: 0x00053A01
		void IInRoomCallbacks.OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
		{
		}

		// Token: 0x0600113B RID: 4411 RVA: 0x00055803 File Offset: 0x00053A03
		void IMatchmakingCallbacks.OnFriendListUpdate(List<FriendInfo> friendList)
		{
		}

		// Token: 0x0600113C RID: 4412 RVA: 0x00055805 File Offset: 0x00053A05
		void IMatchmakingCallbacks.OnCreatedRoom()
		{
		}

		// Token: 0x0600113D RID: 4413 RVA: 0x00055807 File Offset: 0x00053A07
		void IMatchmakingCallbacks.OnCreateRoomFailed(short returnCode, string message)
		{
		}

		// Token: 0x0600113E RID: 4414 RVA: 0x00055809 File Offset: 0x00053A09
		void IMatchmakingCallbacks.OnJoinedRoom()
		{
			this.SetRoomDebugText();
			this.voiceConnection.Client.LocalPlayer.SetMic(this.voiceConnection.PrimaryRecorder.MicrophoneType);
		}

		// Token: 0x0600113F RID: 4415 RVA: 0x00055837 File Offset: 0x00053A37
		void IMatchmakingCallbacks.OnJoinRoomFailed(short returnCode, string message)
		{
		}

		// Token: 0x06001140 RID: 4416 RVA: 0x00055839 File Offset: 0x00053A39
		void IMatchmakingCallbacks.OnJoinRandomFailed(short returnCode, string message)
		{
		}

		// Token: 0x06001141 RID: 4417 RVA: 0x0005583B File Offset: 0x00053A3B
		void IMatchmakingCallbacks.OnLeftRoom()
		{
			this.SetRoomDebugText();
			this.SetDefaults();
		}

		// Token: 0x04000F9C RID: 3996
		[SerializeField]
		private Text connectionStatusText;

		// Token: 0x04000F9D RID: 3997
		[SerializeField]
		private Text serverStatusText;

		// Token: 0x04000F9E RID: 3998
		[SerializeField]
		private Text roomStatusText;

		// Token: 0x04000F9F RID: 3999
		[SerializeField]
		private Text inputWarningText;

		// Token: 0x04000FA0 RID: 4000
		[SerializeField]
		private Text rttText;

		// Token: 0x04000FA1 RID: 4001
		[SerializeField]
		private Text rttVariationText;

		// Token: 0x04000FA2 RID: 4002
		[SerializeField]
		private Text packetLossWarningText;

		// Token: 0x04000FA3 RID: 4003
		[SerializeField]
		private InputField localNicknameText;

		// Token: 0x04000FA4 RID: 4004
		[SerializeField]
		private Toggle debugEchoToggle;

		// Token: 0x04000FA5 RID: 4005
		[SerializeField]
		private Toggle reliableTransmissionToggle;

		// Token: 0x04000FA6 RID: 4006
		[SerializeField]
		private Toggle encryptionToggle;

		// Token: 0x04000FA7 RID: 4007
		[SerializeField]
		private GameObject webRtcDspGameObject;

		// Token: 0x04000FA8 RID: 4008
		[SerializeField]
		private Toggle aecToggle;

		// Token: 0x04000FA9 RID: 4009
		[SerializeField]
		private Toggle aecHighPassToggle;

		// Token: 0x04000FAA RID: 4010
		[SerializeField]
		private InputField reverseStreamDelayInputField;

		// Token: 0x04000FAB RID: 4011
		[SerializeField]
		private Toggle noiseSuppressionToggle;

		// Token: 0x04000FAC RID: 4012
		[SerializeField]
		private Toggle agcToggle;

		// Token: 0x04000FAD RID: 4013
		[SerializeField]
		private Slider agcCompressionGainSlider;

		// Token: 0x04000FAE RID: 4014
		[SerializeField]
		private Slider agcTargetLevelSlider;

		// Token: 0x04000FAF RID: 4015
		[SerializeField]
		private Toggle vadToggle;

		// Token: 0x04000FB0 RID: 4016
		[SerializeField]
		private Toggle muteToggle;

		// Token: 0x04000FB1 RID: 4017
		[SerializeField]
		private Toggle streamAudioClipToggle;

		// Token: 0x04000FB2 RID: 4018
		[SerializeField]
		private Toggle audioToneToggle;

		// Token: 0x04000FB3 RID: 4019
		[SerializeField]
		private Toggle dspToggle;

		// Token: 0x04000FB4 RID: 4020
		[SerializeField]
		private Toggle highPassToggle;

		// Token: 0x04000FB5 RID: 4021
		[SerializeField]
		private Toggle photonVadToggle;

		// Token: 0x04000FB6 RID: 4022
		[SerializeField]
		private MicrophoneSelector microphoneSelector;

		// Token: 0x04000FB7 RID: 4023
		[SerializeField]
		private GameObject androidMicSettingGameObject;

		// Token: 0x04000FB8 RID: 4024
		[SerializeField]
		private Toggle androidAgcToggle;

		// Token: 0x04000FB9 RID: 4025
		[SerializeField]
		private Toggle androidAecToggle;

		// Token: 0x04000FBA RID: 4026
		[SerializeField]
		private Toggle androidNsToggle;

		// Token: 0x04000FBB RID: 4027
		[SerializeField]
		private bool defaultTransmitEnabled;

		// Token: 0x04000FBC RID: 4028
		[SerializeField]
		private bool fullScreen;

		// Token: 0x04000FBD RID: 4029
		[SerializeField]
		private InputField roomNameInputField;

		// Token: 0x04000FBE RID: 4030
		[SerializeField]
		private int rttYellowThreshold = 100;

		// Token: 0x04000FBF RID: 4031
		[SerializeField]
		private int rttRedThreshold = 160;

		// Token: 0x04000FC0 RID: 4032
		[SerializeField]
		private int rttVariationYellowThreshold = 25;

		// Token: 0x04000FC1 RID: 4033
		[SerializeField]
		private int rttVariationRedThreshold = 50;

		// Token: 0x04000FC2 RID: 4034
		private GameObject compressionGainGameObject;

		// Token: 0x04000FC3 RID: 4035
		private GameObject targetLevelGameObject;

		// Token: 0x04000FC4 RID: 4036
		private Text compressionGainText;

		// Token: 0x04000FC5 RID: 4037
		private Text targetLevelText;

		// Token: 0x04000FC6 RID: 4038
		private GameObject aecOptionsGameObject;

		// Token: 0x04000FC7 RID: 4039
		public Transform RemoteVoicesPanel;

		// Token: 0x04000FC8 RID: 4040
		protected UnityVoiceClient voiceConnection;

		// Token: 0x04000FC9 RID: 4041
		private WebRtcAudioDsp voiceAudioPreprocessor;

		// Token: 0x04000FCA RID: 4042
		private ConnectAndJoin connectAndJoin;

		// Token: 0x04000FCB RID: 4043
		private readonly Color warningColor = new Color(0.9f, 0.5f, 0f, 1f);

		// Token: 0x04000FCC RID: 4044
		private readonly Color okColor = new Color(0f, 0.6f, 0.2f, 1f);

		// Token: 0x04000FCD RID: 4045
		private readonly Color redColor = new Color(1f, 0f, 0f, 1f);

		// Token: 0x04000FCE RID: 4046
		private readonly Color defaultColor = new Color(0f, 0f, 0f, 1f);

		// Token: 0x04000FCF RID: 4047
		private Func<IAudioDesc> toneInputFactory = () => new AudioUtil.ToneAudioReader<float>(null, 440.0, 48000, 2);
	}
}
