using System;
using ExitGames.Client.Photon;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Photon.Voice.Unity.Demos.DemoVoiceUI
{
	// Token: 0x020002C3 RID: 707
	public class RemoteSpeakerUI : MonoBehaviour, IInRoomCallbacks
	{
		// Token: 0x170000CD RID: 205
		// (get) Token: 0x06001164 RID: 4452 RVA: 0x00056040 File Offset: 0x00054240
		protected Photon.Realtime.Player Actor
		{
			get
			{
				if (this.loadBalancingClient == null || this.loadBalancingClient.CurrentRoom == null)
				{
					return null;
				}
				return this.loadBalancingClient.CurrentRoom.GetPlayer(this.speaker.RemoteVoice.PlayerId, false);
			}
		}

		// Token: 0x06001165 RID: 4453 RVA: 0x0005607C File Offset: 0x0005427C
		protected virtual void Start()
		{
			this.speaker = base.GetComponent<Speaker>();
			this.audioSource = base.GetComponent<AudioSource>();
			this.playDelayInputField.text = this.speaker.PlayDelay.ToString();
			this.playDelayInputField.SetSingleOnEndEditCallback(new UnityAction<string>(this.OnPlayDelayChanged));
			this.SetNickname();
			this.SetMutedState();
			this.SetProperties();
			this.volumeSlider.minValue = 0f;
			this.volumeSlider.maxValue = 1f;
			this.volumeSlider.SetSingleOnValueChangedCallback(new UnityAction<float>(this.OnVolumeChanged));
			this.volumeSlider.value = 1f;
			this.OnVolumeChanged(1f);
		}

		// Token: 0x06001166 RID: 4454 RVA: 0x0005613A File Offset: 0x0005433A
		private void OnVolumeChanged(float newValue)
		{
			this.audioSource.volume = newValue;
		}

		// Token: 0x06001167 RID: 4455 RVA: 0x00056148 File Offset: 0x00054348
		private void OnPlayDelayChanged(string str)
		{
			int num;
			if (int.TryParse(str, out num))
			{
				this.speaker.PlayDelay = num;
				return;
			}
			Debug.LogErrorFormat("Failed to parse {0}", new object[] { str });
		}

		// Token: 0x06001168 RID: 4456 RVA: 0x00056180 File Offset: 0x00054380
		private void Update()
		{
			this.remoteIsTalking.enabled = this.speaker.IsPlaying;
			if (this.speaker.IsPlaying)
			{
				int lag = this.speaker.Lag;
				this.smoothedLag = (lag + this.smoothedLag * 99) / 100;
				this.bufferLagText.text = string.Concat(new object[] { "Buffer Lag: ", this.smoothedLag, "/", lag });
				return;
			}
			this.bufferLagText.text = "Buffer Lag: " + this.smoothedLag + "/-";
		}

		// Token: 0x06001169 RID: 4457 RVA: 0x00056233 File Offset: 0x00054433
		private void OnDestroy()
		{
			if (this.loadBalancingClient != null)
			{
				this.loadBalancingClient.RemoveCallbackTarget(this);
			}
		}

		// Token: 0x0600116A RID: 4458 RVA: 0x0005624C File Offset: 0x0005444C
		private void SetNickname()
		{
			string text = this.speaker.name;
			if (this.Actor != null)
			{
				text = this.Actor.NickName;
				if (string.IsNullOrEmpty(text))
				{
					text = "user " + this.Actor.ActorNumber;
				}
			}
			this.nameText.text = text;
		}

		// Token: 0x0600116B RID: 4459 RVA: 0x000562A8 File Offset: 0x000544A8
		private void SetMutedState()
		{
			this.SetMutedState(this.Actor.IsMuted());
		}

		// Token: 0x0600116C RID: 4460 RVA: 0x000562BC File Offset: 0x000544BC
		private void SetProperties()
		{
			this.photonVad.enabled = this.Actor.HasPhotonVAD();
			this.webrtcVad.enabled = this.Actor.HasWebRTCVAD();
			this.aec.enabled = this.Actor.HasAEC();
			this.agc.enabled = this.Actor.HasAGC();
			this.agc.text = "AGC Gain: " + this.Actor.GetAGCGain().ToString() + " Level: " + this.Actor.GetAGCLevel().ToString();
			Recorder.MicType? micType = this.Actor.GetMic();
			this.mic.enabled = micType != null;
			Text text = this.mic;
			string text2;
			if (micType == null)
			{
				text2 = "";
			}
			else
			{
				Recorder.MicType? micType2 = micType;
				Recorder.MicType micType3 = Recorder.MicType.Unity;
				text2 = (((micType2.GetValueOrDefault() == micType3) & (micType2 != null)) ? "Unity MIC" : "Photon MIC");
			}
			text.text = text2;
		}

		// Token: 0x0600116D RID: 4461 RVA: 0x000563BE File Offset: 0x000545BE
		protected virtual void SetMutedState(bool isMuted)
		{
			this.remoteIsMuting.enabled = isMuted;
		}

		// Token: 0x0600116E RID: 4462 RVA: 0x000563CC File Offset: 0x000545CC
		protected virtual void OnActorPropertiesChanged(Photon.Realtime.Player targetPlayer, Hashtable changedProps)
		{
			if (this.speaker != null && this.speaker.RemoteVoice != null && targetPlayer.ActorNumber == this.speaker.RemoteVoice.PlayerId)
			{
				this.SetMutedState();
				this.SetNickname();
				this.SetProperties();
			}
		}

		// Token: 0x0600116F RID: 4463 RVA: 0x0005641E File Offset: 0x0005461E
		public virtual void Init(VoiceConnection vC)
		{
			this.voiceConnection = vC;
			this.loadBalancingClient = this.voiceConnection.Client;
			this.loadBalancingClient.AddCallbackTarget(this);
		}

		// Token: 0x06001170 RID: 4464 RVA: 0x00056444 File Offset: 0x00054644
		void IInRoomCallbacks.OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
		{
		}

		// Token: 0x06001171 RID: 4465 RVA: 0x00056446 File Offset: 0x00054646
		void IInRoomCallbacks.OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
		{
		}

		// Token: 0x06001172 RID: 4466 RVA: 0x00056448 File Offset: 0x00054648
		void IInRoomCallbacks.OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
		{
		}

		// Token: 0x06001173 RID: 4467 RVA: 0x0005644A File Offset: 0x0005464A
		void IInRoomCallbacks.OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps)
		{
			this.OnActorPropertiesChanged(targetPlayer, changedProps);
		}

		// Token: 0x06001174 RID: 4468 RVA: 0x00056454 File Offset: 0x00054654
		void IInRoomCallbacks.OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
		{
		}

		// Token: 0x04000FE7 RID: 4071
		[SerializeField]
		private Text nameText;

		// Token: 0x04000FE8 RID: 4072
		[SerializeField]
		protected Image remoteIsMuting;

		// Token: 0x04000FE9 RID: 4073
		[SerializeField]
		private Image remoteIsTalking;

		// Token: 0x04000FEA RID: 4074
		[SerializeField]
		private InputField playDelayInputField;

		// Token: 0x04000FEB RID: 4075
		[SerializeField]
		private Text bufferLagText;

		// Token: 0x04000FEC RID: 4076
		[SerializeField]
		private Slider volumeSlider;

		// Token: 0x04000FED RID: 4077
		[SerializeField]
		private Text photonVad;

		// Token: 0x04000FEE RID: 4078
		[SerializeField]
		private Text webrtcVad;

		// Token: 0x04000FEF RID: 4079
		[SerializeField]
		private Text aec;

		// Token: 0x04000FF0 RID: 4080
		[SerializeField]
		private Text agc;

		// Token: 0x04000FF1 RID: 4081
		[SerializeField]
		private Text mic;

		// Token: 0x04000FF2 RID: 4082
		protected Speaker speaker;

		// Token: 0x04000FF3 RID: 4083
		private AudioSource audioSource;

		// Token: 0x04000FF4 RID: 4084
		protected VoiceConnection voiceConnection;

		// Token: 0x04000FF5 RID: 4085
		protected LoadBalancingClient loadBalancingClient;

		// Token: 0x04000FF6 RID: 4086
		private int smoothedLag;
	}
}
