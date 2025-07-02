using System;
using Photon.Voice.PUN;
using UnityEngine;
using UnityEngine.UI;

namespace ExitGames.Demos.DemoPunVoice
{
	// Token: 0x020002B6 RID: 694
	[RequireComponent(typeof(Canvas))]
	public class Highlighter : MonoBehaviour
	{
		// Token: 0x060010CC RID: 4300 RVA: 0x000533A1 File Offset: 0x000515A1
		private void OnEnable()
		{
			ChangePOV.CameraChanged += this.ChangePOV_CameraChanged;
			VoiceDemoUI.DebugToggled += this.VoiceDemoUI_DebugToggled;
		}

		// Token: 0x060010CD RID: 4301 RVA: 0x000533C5 File Offset: 0x000515C5
		private void OnDisable()
		{
			ChangePOV.CameraChanged -= this.ChangePOV_CameraChanged;
			VoiceDemoUI.DebugToggled -= this.VoiceDemoUI_DebugToggled;
		}

		// Token: 0x060010CE RID: 4302 RVA: 0x000533E9 File Offset: 0x000515E9
		private void VoiceDemoUI_DebugToggled(bool debugMode)
		{
			this.showSpeakerLag = debugMode;
		}

		// Token: 0x060010CF RID: 4303 RVA: 0x000533F2 File Offset: 0x000515F2
		private void ChangePOV_CameraChanged(Camera camera)
		{
			this.canvas.worldCamera = camera;
		}

		// Token: 0x060010D0 RID: 4304 RVA: 0x00053400 File Offset: 0x00051600
		private void Awake()
		{
			this.canvas = base.GetComponent<Canvas>();
			if (this.canvas != null && this.canvas.worldCamera == null)
			{
				this.canvas.worldCamera = Camera.main;
			}
			this.photonVoiceView = base.GetComponentInParent<PhotonVoiceView>();
		}

		// Token: 0x060010D1 RID: 4305 RVA: 0x00053458 File Offset: 0x00051658
		private void Update()
		{
			this.recorderSprite.enabled = this.photonVoiceView.IsRecording;
			this.speakerSprite.enabled = this.photonVoiceView.IsSpeaking;
			this.bufferLagText.enabled = this.showSpeakerLag && this.photonVoiceView.IsSpeaking;
			if (this.bufferLagText.enabled)
			{
				this.bufferLagText.text = string.Format("{0}", this.photonVoiceView.SpeakerInUse.Lag);
			}
		}

		// Token: 0x060010D2 RID: 4306 RVA: 0x000534EC File Offset: 0x000516EC
		private void LateUpdate()
		{
			if (this.canvas == null || this.canvas.worldCamera == null)
			{
				return;
			}
			base.transform.rotation = Quaternion.Euler(0f, this.canvas.worldCamera.transform.eulerAngles.y, 0f);
		}

		// Token: 0x04000F70 RID: 3952
		private Canvas canvas;

		// Token: 0x04000F71 RID: 3953
		private PhotonVoiceView photonVoiceView;

		// Token: 0x04000F72 RID: 3954
		[SerializeField]
		private Image recorderSprite;

		// Token: 0x04000F73 RID: 3955
		[SerializeField]
		private Image speakerSprite;

		// Token: 0x04000F74 RID: 3956
		[SerializeField]
		private Text bufferLagText;

		// Token: 0x04000F75 RID: 3957
		private bool showSpeakerLag;
	}
}
