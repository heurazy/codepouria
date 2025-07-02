using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Photon.Voice.Unity.Demos
{
	// Token: 0x020002BA RID: 698
	public class BackgroundMusicController : MonoBehaviour
	{
		// Token: 0x060010EF RID: 4335 RVA: 0x00054210 File Offset: 0x00052410
		private void Awake()
		{
			this.volumeSlider.minValue = 0f;
			this.volumeSlider.maxValue = 1f;
			this.volumeSlider.SetSingleOnValueChangedCallback(new UnityAction<float>(this.OnVolumeChanged));
			this.volumeSlider.value = this.initialVolume;
			this.OnVolumeChanged(this.initialVolume);
		}

		// Token: 0x060010F0 RID: 4336 RVA: 0x00054271 File Offset: 0x00052471
		private void OnVolumeChanged(float newValue)
		{
			this.audioSource.volume = newValue;
		}

		// Token: 0x04000F8E RID: 3982
		[SerializeField]
		private Text volumeText;

		// Token: 0x04000F8F RID: 3983
		[SerializeField]
		private Slider volumeSlider;

		// Token: 0x04000F90 RID: 3984
		[SerializeField]
		private AudioSource audioSource;

		// Token: 0x04000F91 RID: 3985
		[SerializeField]
		private float initialVolume = 0.125f;
	}
}
