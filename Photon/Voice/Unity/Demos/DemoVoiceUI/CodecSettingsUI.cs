using System;
using System.Collections.Generic;
using POpusCodec.Enums;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Photon.Voice.Unity.Demos.DemoVoiceUI
{
	// Token: 0x020002BD RID: 701
	public class CodecSettingsUI : MonoBehaviour
	{
		// Token: 0x06001107 RID: 4359 RVA: 0x000544DC File Offset: 0x000526DC
		private void Awake()
		{
			this.frameDurationDropdown.ClearOptions();
			this.frameDurationDropdown.AddOptions(CodecSettingsUI.frameDurationOptions);
			this.InitFrameDuration();
			this.frameDurationDropdown.SetSingleOnValueChangedCallback(new UnityAction<int>(this.OnFrameDurationChanged));
			this.samplingRateDropdown.ClearOptions();
			this.samplingRateDropdown.AddOptions(CodecSettingsUI.samplingRateOptions);
			this.InitSamplingRate();
			this.samplingRateDropdown.SetSingleOnValueChangedCallback(new UnityAction<int>(this.OnSamplingRateChanged));
			this.bitrateInputField.SetSingleOnValueChangedCallback(new UnityAction<string>(this.OnBitrateChanged));
			this.InitBitrate();
		}

		// Token: 0x06001108 RID: 4360 RVA: 0x00054576 File Offset: 0x00052776
		private void Update()
		{
			this.InitFrameDuration();
			this.InitSamplingRate();
			this.InitBitrate();
		}

		// Token: 0x06001109 RID: 4361 RVA: 0x0005458C File Offset: 0x0005278C
		private void OnBitrateChanged(string newBitrateString)
		{
			int num;
			if (int.TryParse(newBitrateString, out num))
			{
				this.recorder.Bitrate = num;
			}
		}

		// Token: 0x0600110A RID: 4362 RVA: 0x000545B0 File Offset: 0x000527B0
		private void OnFrameDurationChanged(int index)
		{
			OpusCodec.FrameDuration frameDuration = this.recorder.FrameDuration;
			switch (index)
			{
			case 0:
				frameDuration = OpusCodec.FrameDuration.Frame2dot5ms;
				break;
			case 1:
				frameDuration = OpusCodec.FrameDuration.Frame5ms;
				break;
			case 2:
				frameDuration = OpusCodec.FrameDuration.Frame10ms;
				break;
			case 3:
				frameDuration = OpusCodec.FrameDuration.Frame20ms;
				break;
			case 4:
				frameDuration = OpusCodec.FrameDuration.Frame40ms;
				break;
			case 5:
				frameDuration = OpusCodec.FrameDuration.Frame60ms;
				break;
			}
			this.recorder.FrameDuration = frameDuration;
		}

		// Token: 0x0600110B RID: 4363 RVA: 0x00054624 File Offset: 0x00052824
		private void OnSamplingRateChanged(int index)
		{
			SamplingRate samplingRate = this.recorder.SamplingRate;
			switch (index)
			{
			case 0:
				samplingRate = SamplingRate.Sampling08000;
				break;
			case 1:
				samplingRate = SamplingRate.Sampling12000;
				break;
			case 2:
				samplingRate = SamplingRate.Sampling16000;
				break;
			case 3:
				samplingRate = SamplingRate.Sampling24000;
				break;
			case 4:
				samplingRate = SamplingRate.Sampling48000;
				break;
			}
			this.recorder.SamplingRate = samplingRate;
		}

		// Token: 0x0600110C RID: 4364 RVA: 0x0005468C File Offset: 0x0005288C
		private void InitFrameDuration()
		{
			int num = 0;
			OpusCodec.FrameDuration frameDuration = this.recorder.FrameDuration;
			if (frameDuration <= OpusCodec.FrameDuration.Frame10ms)
			{
				if (frameDuration != OpusCodec.FrameDuration.Frame5ms)
				{
					if (frameDuration == OpusCodec.FrameDuration.Frame10ms)
					{
						num = 2;
					}
				}
				else
				{
					num = 1;
				}
			}
			else if (frameDuration != OpusCodec.FrameDuration.Frame20ms)
			{
				if (frameDuration != OpusCodec.FrameDuration.Frame40ms)
				{
					if (frameDuration == OpusCodec.FrameDuration.Frame60ms)
					{
						num = 5;
					}
				}
				else
				{
					num = 4;
				}
			}
			else
			{
				num = 3;
			}
			this.frameDurationDropdown.value = num;
		}

		// Token: 0x0600110D RID: 4365 RVA: 0x000546FC File Offset: 0x000528FC
		private void InitSamplingRate()
		{
			int num = 0;
			SamplingRate samplingRate = this.recorder.SamplingRate;
			if (samplingRate <= SamplingRate.Sampling16000)
			{
				if (samplingRate != SamplingRate.Sampling12000)
				{
					if (samplingRate == SamplingRate.Sampling16000)
					{
						num = 2;
					}
				}
				else
				{
					num = 1;
				}
			}
			else if (samplingRate != SamplingRate.Sampling24000)
			{
				if (samplingRate == SamplingRate.Sampling48000)
				{
					num = 4;
				}
			}
			else
			{
				num = 3;
			}
			this.samplingRateDropdown.value = num;
		}

		// Token: 0x0600110E RID: 4366 RVA: 0x00054760 File Offset: 0x00052960
		private void InitBitrate()
		{
			this.bitrateInputField.text = this.recorder.Bitrate.ToString();
		}

		// Token: 0x04000F96 RID: 3990
		[SerializeField]
		private Dropdown frameDurationDropdown;

		// Token: 0x04000F97 RID: 3991
		[SerializeField]
		private Dropdown samplingRateDropdown;

		// Token: 0x04000F98 RID: 3992
		[SerializeField]
		private InputField bitrateInputField;

		// Token: 0x04000F99 RID: 3993
		[SerializeField]
		private Recorder recorder;

		// Token: 0x04000F9A RID: 3994
		private static readonly List<string> frameDurationOptions = new List<string> { "2.5ms", "5ms", "10ms", "20ms", "40ms", "60ms" };

		// Token: 0x04000F9B RID: 3995
		private static readonly List<string> samplingRateOptions = new List<string> { "8kHz", "12kHz", "16kHz", "24kHz", "48kHz" };
	}
}
