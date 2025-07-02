using System;
using System.Collections.Generic;
using Photon.Voice.Unity.UtilityScripts;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Photon.Voice.Unity.Demos.DemoVoiceUI
{
	// Token: 0x020002C1 RID: 705
	public class MicrophoneSelector : VoiceComponent
	{
		// Token: 0x06001145 RID: 4421 RVA: 0x0005594C File Offset: 0x00053B4C
		protected override void Awake()
		{
			base.Awake();
			this.unityMicEnum = new AudioInEnumerator(base.Logger);
			this.photonMicEnum = Platform.CreateAudioInEnumerator(base.Logger);
			this.photonMicEnum.OnReady = delegate
			{
				this.SetupMicDropdown();
				this.SetCurrentValue();
			};
			this.refreshButton.GetComponentInChildren<Button>().onClick.AddListener(new UnityAction(this.RefreshMicrophones));
			this.fillArea = this.micLevelSlider.fillRect.GetComponent<Image>();
			this.defaultFillColor = this.fillArea.color;
		}

		// Token: 0x06001146 RID: 4422 RVA: 0x000559E0 File Offset: 0x00053BE0
		private void Update()
		{
			if (this.recorder != null)
			{
				this.micLevelSlider.value = this.recorder.LevelMeter.CurrentPeakAmp;
				this.fillArea.color = (this.recorder.IsCurrentlyTransmitting ? this.speakingFillColor : this.defaultFillColor);
			}
		}

		// Token: 0x06001147 RID: 4423 RVA: 0x00055A3C File Offset: 0x00053C3C
		private void OnEnable()
		{
			MicrophonePermission.MicrophonePermissionCallback += this.OnMicrophonePermissionCallback;
		}

		// Token: 0x06001148 RID: 4424 RVA: 0x00055A4F File Offset: 0x00053C4F
		private void OnMicrophonePermissionCallback(bool granted)
		{
			this.RefreshMicrophones();
		}

		// Token: 0x06001149 RID: 4425 RVA: 0x00055A57 File Offset: 0x00053C57
		private void OnDisable()
		{
			MicrophonePermission.MicrophonePermissionCallback -= this.OnMicrophonePermissionCallback;
		}

		// Token: 0x0600114A RID: 4426 RVA: 0x00055A6C File Offset: 0x00053C6C
		private void SetupMicDropdown()
		{
			this.micDropdown.ClearOptions();
			this.micOptions = new List<MicRef>();
			List<string> list = new List<string>();
			this.micOptions.Add(new MicRef(MicType.Unity, DeviceInfo.Default));
			list.Add(string.Format("[Unity]\u00a0[Default]", Array.Empty<object>()));
			foreach (DeviceInfo deviceInfo in this.unityMicEnum)
			{
				this.micOptions.Add(new MicRef(MicType.Unity, deviceInfo));
				list.Add(string.Format("[Unity]\u00a0{0}", deviceInfo));
			}
			this.micOptions.Add(new MicRef(MicType.Photon, DeviceInfo.Default));
			list.Add(string.Format("[Photon]\u00a0[Default]", Array.Empty<object>()));
			foreach (DeviceInfo deviceInfo2 in this.photonMicEnum)
			{
				this.micOptions.Add(new MicRef(MicType.Photon, deviceInfo2));
				list.Add(string.Format("[Photon]\u00a0{0}", deviceInfo2));
			}
			this.micDropdown.AddOptions(list);
			this.micDropdown.onValueChanged.RemoveAllListeners();
			this.micDropdown.onValueChanged.AddListener(delegate(int x)
			{
				this.SwitchToSelectedMic();
			});
		}

		// Token: 0x0600114B RID: 4427 RVA: 0x00055BE4 File Offset: 0x00053DE4
		public void SwitchToSelectedMic()
		{
			MicRef micRef = this.micOptions[this.micDropdown.value];
			MicType micType = micRef.MicType;
			if (micType != MicType.Unity)
			{
				if (micType == MicType.Photon)
				{
					this.recorder.SourceType = Recorder.InputSourceType.Microphone;
					this.recorder.MicrophoneType = Recorder.MicType.Photon;
					this.recorder.MicrophoneDevice = micRef.Device;
				}
			}
			else
			{
				this.recorder.SourceType = Recorder.InputSourceType.Microphone;
				this.recorder.MicrophoneType = Recorder.MicType.Unity;
				this.recorder.MicrophoneDevice = micRef.Device;
			}
			MicrophoneSelector.MicrophoneSelectorEvent microphoneSelectorEvent = this.onValueChanged;
			if (microphoneSelectorEvent == null)
			{
				return;
			}
			microphoneSelectorEvent.Invoke(micRef.MicType, micRef.Device);
		}

		// Token: 0x0600114C RID: 4428 RVA: 0x00055C88 File Offset: 0x00053E88
		private void SetCurrentValue()
		{
			if (this.micOptions == null)
			{
				Debug.LogWarning("micOptions list is null");
				return;
			}
			this.micDropdown.gameObject.SetActive(true);
			this.refreshButton.SetActive(true);
			for (int i = 0; i < this.micOptions.Count; i++)
			{
				MicRef micRef = this.micOptions[i];
				if ((micRef.MicType == MicType.Unity && this.recorder.SourceType == Recorder.InputSourceType.Microphone && this.recorder.MicrophoneType == Recorder.MicType.Unity) || (micRef.MicType == MicType.Photon && this.recorder.SourceType == Recorder.InputSourceType.Microphone && this.recorder.MicrophoneType == Recorder.MicType.Photon))
				{
					this.micDropdown.value = i;
					return;
				}
			}
		}

		// Token: 0x0600114D RID: 4429 RVA: 0x00055D3B File Offset: 0x00053F3B
		public void RefreshMicrophones()
		{
			this.unityMicEnum.Refresh();
			this.photonMicEnum.Refresh();
		}

		// Token: 0x0600114E RID: 4430 RVA: 0x00055D53 File Offset: 0x00053F53
		private void PhotonVoiceCreated()
		{
			this.RefreshMicrophones();
		}

		// Token: 0x04000FD6 RID: 4054
		public MicrophoneSelector.MicrophoneSelectorEvent onValueChanged = new MicrophoneSelector.MicrophoneSelectorEvent();

		// Token: 0x04000FD7 RID: 4055
		private List<MicRef> micOptions;

		// Token: 0x04000FD8 RID: 4056
		[SerializeField]
		private Dropdown micDropdown;

		// Token: 0x04000FD9 RID: 4057
		[SerializeField]
		private Slider micLevelSlider;

		// Token: 0x04000FDA RID: 4058
		[SerializeField]
		private Recorder recorder;

		// Token: 0x04000FDB RID: 4059
		[SerializeField]
		[FormerlySerializedAs("RefreshButton")]
		private GameObject refreshButton;

		// Token: 0x04000FDC RID: 4060
		private Image fillArea;

		// Token: 0x04000FDD RID: 4061
		private Color defaultFillColor = Color.white;

		// Token: 0x04000FDE RID: 4062
		private Color speakingFillColor = Color.green;

		// Token: 0x04000FDF RID: 4063
		private IDeviceEnumerator unityMicEnum;

		// Token: 0x04000FE0 RID: 4064
		private IDeviceEnumerator photonMicEnum;

		// Token: 0x020003D0 RID: 976
		public class MicrophoneSelectorEvent : UnityEvent<MicType, DeviceInfo>
		{
		}
	}
}
