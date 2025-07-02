using System;
using System.Linq;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ExitGames.Demos.DemoPunVoice
{
	// Token: 0x020002B9 RID: 697
	public class VoiceDemoUI : MonoBehaviour
	{
		// Token: 0x170000CC RID: 204
		// (get) Token: 0x060010DB RID: 4315 RVA: 0x00053675 File Offset: 0x00051875
		// (set) Token: 0x060010DC RID: 4316 RVA: 0x00053680 File Offset: 0x00051880
		public bool DebugMode
		{
			get
			{
				return this.debugMode;
			}
			set
			{
				this.debugMode = value;
				this.debugGO.SetActive(this.debugMode);
				this.voiceDebugText.text = string.Empty;
				if (this.debugMode)
				{
					this.previousDebugLevel = this.punVoiceClient.Client.LoadBalancingPeer.DebugOut;
					this.punVoiceClient.Client.LoadBalancingPeer.DebugOut = DebugLevel.ALL;
				}
				else
				{
					this.punVoiceClient.Client.LoadBalancingPeer.DebugOut = this.previousDebugLevel;
				}
				if (VoiceDemoUI.DebugToggled != null)
				{
					VoiceDemoUI.DebugToggled(this.debugMode);
				}
			}
		}

		// Token: 0x14000006 RID: 6
		// (add) Token: 0x060010DD RID: 4317 RVA: 0x00053724 File Offset: 0x00051924
		// (remove) Token: 0x060010DE RID: 4318 RVA: 0x00053758 File Offset: 0x00051958
		public static event VoiceDemoUI.OnDebugToggle DebugToggled;

		// Token: 0x060010DF RID: 4319 RVA: 0x0005378B File Offset: 0x0005198B
		private void Awake()
		{
			this.punVoiceClient = PunVoiceClient.Instance;
			Debug.LogWarning("VoiceDemoUI selected a punVoiceClient.Instance", this.punVoiceClient);
		}

		// Token: 0x060010E0 RID: 4320 RVA: 0x000537A8 File Offset: 0x000519A8
		private void OnDestroy()
		{
			ChangePOV.CameraChanged -= this.OnCameraChanged;
			BetterToggle.ToggleValueChanged -= this.BetterToggle_ToggleValueChanged;
			CharacterInstantiation.CharacterInstantiated -= this.CharacterInstantiation_CharacterInstantiated;
			this.punVoiceClient.Client.StateChanged -= this.VoiceClientStateChanged;
			PhotonNetwork.NetworkingClient.StateChanged -= this.PunClientStateChanged;
		}

		// Token: 0x060010E1 RID: 4321 RVA: 0x0005381C File Offset: 0x00051A1C
		private void CharacterInstantiation_CharacterInstantiated(GameObject character)
		{
			PhotonVoiceView component = character.GetComponent<PhotonVoiceView>();
			if (component != null)
			{
				this.recorder = component;
			}
		}

		// Token: 0x060010E2 RID: 4322 RVA: 0x00053840 File Offset: 0x00051A40
		private void InitToggles(Toggle[] toggles)
		{
			if (toggles == null)
			{
				return;
			}
			foreach (Toggle toggle in toggles)
			{
				string name = toggle.name;
				if (!(name == "Mute"))
				{
					if (!(name == "VoiceDetection"))
					{
						if (!(name == "DebugVoice"))
						{
							if (!(name == "Transmit"))
							{
								if (!(name == "DebugEcho"))
								{
									if (name == "AutoConnectAndJoin")
									{
										toggle.isOn = this.punVoiceClient.AutoConnectAndJoin;
									}
								}
								else if (this.recorder != null && this.recorder.RecorderInUse != null)
								{
									toggle.isOn = this.recorder.RecorderInUse.DebugEchoMode;
								}
							}
							else if (this.recorder != null && this.recorder.RecorderInUse != null)
							{
								toggle.isOn = this.recorder.RecorderInUse.TransmitEnabled;
							}
						}
						else
						{
							toggle.isOn = this.DebugMode;
						}
					}
					else if (this.recorder != null && this.recorder.RecorderInUse != null)
					{
						toggle.isOn = this.recorder.RecorderInUse.VoiceDetection;
					}
				}
				else
				{
					toggle.isOn = AudioListener.volume <= 0.001f;
				}
			}
		}

		// Token: 0x060010E3 RID: 4323 RVA: 0x000539C0 File Offset: 0x00051BC0
		private void BetterToggle_ToggleValueChanged(Toggle toggle)
		{
			string name = toggle.name;
			if (!(name == "Mute"))
			{
				if (!(name == "Transmit"))
				{
					if (!(name == "VoiceDetection"))
					{
						if (!(name == "DebugEcho"))
						{
							if (name == "DebugVoice")
							{
								this.DebugMode = toggle.isOn;
								return;
							}
							if (!(name == "AutoConnectAndJoin"))
							{
								return;
							}
							this.punVoiceClient.AutoConnectAndJoin = toggle.isOn;
						}
						else if (this.recorder.RecorderInUse)
						{
							this.recorder.RecorderInUse.DebugEchoMode = toggle.isOn;
							return;
						}
					}
					else if (this.recorder.RecorderInUse)
					{
						this.recorder.RecorderInUse.VoiceDetection = toggle.isOn;
						return;
					}
				}
				else if (this.recorder.RecorderInUse)
				{
					this.recorder.RecorderInUse.TransmitEnabled = toggle.isOn;
					return;
				}
				return;
			}
			if (toggle.isOn)
			{
				this.volumeBeforeMute = AudioListener.volume;
				AudioListener.volume = 0f;
				return;
			}
			AudioListener.volume = this.volumeBeforeMute;
			this.volumeBeforeMute = 0f;
		}

		// Token: 0x060010E4 RID: 4324 RVA: 0x00053B00 File Offset: 0x00051D00
		private void OnCameraChanged(Camera newCamera)
		{
			this.canvas.worldCamera = newCamera;
		}

		// Token: 0x060010E5 RID: 4325 RVA: 0x00053B10 File Offset: 0x00051D10
		private void Start()
		{
			ChangePOV.CameraChanged += this.OnCameraChanged;
			BetterToggle.ToggleValueChanged += this.BetterToggle_ToggleValueChanged;
			CharacterInstantiation.CharacterInstantiated += this.CharacterInstantiation_CharacterInstantiated;
			this.punVoiceClient.Client.StateChanged += this.VoiceClientStateChanged;
			PhotonNetwork.NetworkingClient.StateChanged += this.PunClientStateChanged;
			this.canvas = base.GetComponentInChildren<Canvas>();
			if (this.punSwitch != null)
			{
				this.punSwitchText = this.punSwitch.GetComponentInChildren<Text>();
				this.punSwitch.onClick.AddListener(new UnityAction(this.PunSwitchOnClick));
			}
			if (this.voiceSwitch != null)
			{
				this.voiceSwitchText = this.voiceSwitch.GetComponentInChildren<Text>();
				this.voiceSwitch.onClick.AddListener(new UnityAction(this.VoiceSwitchOnClick));
			}
			if (this.calibrateButton != null)
			{
				this.calibrateButton.onClick.AddListener(new UnityAction(this.CalibrateButtonOnClick));
				this.calibrateText = this.calibrateButton.GetComponentInChildren<Text>();
			}
			if (this.punState != null)
			{
				this.debugGO = this.punState.transform.parent.gameObject;
			}
			this.volumeBeforeMute = AudioListener.volume;
			this.previousDebugLevel = this.punVoiceClient.Client.LoadBalancingPeer.DebugOut;
			if (this.globalSettings != null)
			{
				this.globalSettings.SetActive(true);
				this.InitToggles(this.globalSettings.GetComponentsInChildren<Toggle>());
			}
			if (this.devicesInfoText != null)
			{
				using (AudioInEnumerator audioInEnumerator = new AudioInEnumerator(this.punVoiceClient.Logger))
				{
					using (IDeviceEnumerator deviceEnumerator = Platform.CreateAudioInEnumerator(this.punVoiceClient.Logger))
					{
						if (audioInEnumerator.Count<DeviceInfo>() + deviceEnumerator.Count<DeviceInfo>() == 0)
						{
							this.devicesInfoText.enabled = true;
							this.devicesInfoText.color = Color.red;
							this.devicesInfoText.text = "No microphone device detected!";
						}
						else
						{
							this.devicesInfoText.text = "Mic Unity: " + string.Join(", ", audioInEnumerator.Select((DeviceInfo x) => x.ToString()));
							Text text = this.devicesInfoText;
							text.text = text.text + "\nMic Photon: " + string.Join(", ", deviceEnumerator.Select((DeviceInfo x) => x.ToString()));
						}
					}
				}
			}
			this.VoiceClientStateChanged(ClientState.PeerCreated, this.punVoiceClient.ClientState);
			this.PunClientStateChanged(ClientState.PeerCreated, PhotonNetwork.NetworkingClient.State);
		}

		// Token: 0x060010E6 RID: 4326 RVA: 0x00053E10 File Offset: 0x00052010
		private void PunSwitchOnClick()
		{
			if (PhotonNetwork.NetworkClientState == ClientState.Joined)
			{
				PhotonNetwork.Disconnect();
				return;
			}
			if (PhotonNetwork.NetworkClientState == ClientState.Disconnected || PhotonNetwork.NetworkClientState == ClientState.PeerCreated)
			{
				PhotonNetwork.ConnectUsingSettings();
			}
		}

		// Token: 0x060010E7 RID: 4327 RVA: 0x00053E38 File Offset: 0x00052038
		private void VoiceSwitchOnClick()
		{
			if (this.punVoiceClient.ClientState == ClientState.Joined)
			{
				this.punVoiceClient.Disconnect();
				return;
			}
			if (this.punVoiceClient.ClientState == ClientState.PeerCreated || this.punVoiceClient.ClientState == ClientState.Disconnected)
			{
				this.punVoiceClient.ConnectAndJoinRoom();
			}
		}

		// Token: 0x060010E8 RID: 4328 RVA: 0x00053E88 File Offset: 0x00052088
		private void CalibrateButtonOnClick()
		{
			if (this.recorder.RecorderInUse && !this.recorder.RecorderInUse.VoiceDetectorCalibrating)
			{
				this.recorder.RecorderInUse.VoiceDetectorCalibrate(this.calibrationMilliSeconds, null);
			}
		}

		// Token: 0x060010E9 RID: 4329 RVA: 0x00053EC8 File Offset: 0x000520C8
		private void Update()
		{
			if (this.recorder != null && this.recorder.RecorderInUse != null && this.recorder.RecorderInUse.LevelMeter != null)
			{
				this.voiceDebugText.text = string.Format("Amp: avg. {0:0.000000}, peak {1:0.000000}", this.recorder.RecorderInUse.LevelMeter.CurrentAvgAmp, this.recorder.RecorderInUse.LevelMeter.CurrentPeakAmp);
			}
		}

		// Token: 0x060010EA RID: 4330 RVA: 0x00053F54 File Offset: 0x00052154
		private void PunClientStateChanged(ClientState fromState, ClientState toState)
		{
			this.punState.text = string.Format("PUN: {0}", toState);
			if (toState != ClientState.PeerCreated)
			{
				if (toState == ClientState.Joined)
				{
					this.punSwitch.interactable = true;
					this.punSwitchText.text = "PUN Disconnect";
					goto IL_0080;
				}
				if (toState != ClientState.Disconnected)
				{
					this.punSwitch.interactable = false;
					this.punSwitchText.text = "PUN busy";
					goto IL_0080;
				}
			}
			this.punSwitch.interactable = true;
			this.punSwitchText.text = "PUN Connect";
			IL_0080:
			this.UpdateUiBasedOnVoiceState(this.punVoiceClient.ClientState);
		}

		// Token: 0x060010EB RID: 4331 RVA: 0x00053FF2 File Offset: 0x000521F2
		private void VoiceClientStateChanged(ClientState fromState, ClientState toState)
		{
			this.UpdateUiBasedOnVoiceState(toState);
		}

		// Token: 0x060010EC RID: 4332 RVA: 0x00053FFC File Offset: 0x000521FC
		private void UpdateUiBasedOnVoiceState(ClientState voiceClientState)
		{
			this.voiceState.text = string.Format("PhotonVoice: {0}", voiceClientState);
			if (voiceClientState != ClientState.PeerCreated)
			{
				if (voiceClientState != ClientState.Joined)
				{
					if (voiceClientState != ClientState.Disconnected)
					{
						this.voiceSwitch.interactable = false;
						this.voiceSwitchText.text = "Voice busy";
						return;
					}
				}
				else
				{
					this.voiceSwitch.interactable = true;
					this.inGameSettings.SetActive(true);
					this.voiceSwitchText.text = "Voice Disconnect";
					this.InitToggles(this.inGameSettings.GetComponentsInChildren<Toggle>());
					if (this.recorder != null && this.recorder.RecorderInUse != null)
					{
						this.calibrateButton.interactable = !this.recorder.RecorderInUse.VoiceDetectorCalibrating;
						this.calibrateText.text = (this.recorder.RecorderInUse.VoiceDetectorCalibrating ? "Calibrating" : string.Format("Calibrate ({0}s)", this.calibrationMilliSeconds / 1000));
						return;
					}
					this.calibrateButton.interactable = false;
					this.calibrateText.text = "Unavailable";
					return;
				}
			}
			if (PhotonNetwork.InRoom)
			{
				this.voiceSwitch.interactable = true;
				this.voiceSwitchText.text = "Voice Connect";
				this.voiceDebugText.text = string.Empty;
			}
			else
			{
				this.voiceSwitch.interactable = false;
				this.voiceSwitchText.text = "Voice N/A";
				this.voiceDebugText.text = string.Empty;
			}
			this.calibrateButton.interactable = false;
			this.voiceSwitchText.text = "Voice Connect";
			this.calibrateText.text = "Unavailable";
			this.inGameSettings.SetActive(false);
		}

		// Token: 0x060010ED RID: 4333 RVA: 0x000541C7 File Offset: 0x000523C7
		protected void OnApplicationQuit()
		{
			this.punVoiceClient.Client.StateChanged -= this.VoiceClientStateChanged;
			PhotonNetwork.NetworkingClient.StateChanged -= this.PunClientStateChanged;
		}

		// Token: 0x04000F79 RID: 3961
		[SerializeField]
		private Text punState;

		// Token: 0x04000F7A RID: 3962
		[SerializeField]
		private Text voiceState;

		// Token: 0x04000F7B RID: 3963
		private PunVoiceClient punVoiceClient;

		// Token: 0x04000F7C RID: 3964
		private Canvas canvas;

		// Token: 0x04000F7D RID: 3965
		[SerializeField]
		private Button punSwitch;

		// Token: 0x04000F7E RID: 3966
		private Text punSwitchText;

		// Token: 0x04000F7F RID: 3967
		[SerializeField]
		private Button voiceSwitch;

		// Token: 0x04000F80 RID: 3968
		private Text voiceSwitchText;

		// Token: 0x04000F81 RID: 3969
		[SerializeField]
		private Button calibrateButton;

		// Token: 0x04000F82 RID: 3970
		private Text calibrateText;

		// Token: 0x04000F83 RID: 3971
		[SerializeField]
		private Text voiceDebugText;

		// Token: 0x04000F84 RID: 3972
		private PhotonVoiceView recorder;

		// Token: 0x04000F85 RID: 3973
		[SerializeField]
		private GameObject inGameSettings;

		// Token: 0x04000F86 RID: 3974
		[SerializeField]
		private GameObject globalSettings;

		// Token: 0x04000F87 RID: 3975
		[SerializeField]
		private Text devicesInfoText;

		// Token: 0x04000F88 RID: 3976
		private GameObject debugGO;

		// Token: 0x04000F89 RID: 3977
		private bool debugMode;

		// Token: 0x04000F8A RID: 3978
		private float volumeBeforeMute;

		// Token: 0x04000F8B RID: 3979
		private DebugLevel previousDebugLevel;

		// Token: 0x04000F8D RID: 3981
		[SerializeField]
		private int calibrationMilliSeconds = 2000;

		// Token: 0x020003CD RID: 973
		// (Invoke) Token: 0x0600151A RID: 5402
		public delegate void OnDebugToggle(bool debugMode);
	}
}
