using System;
using UnityEngine;
using UnityEngine.Audio;

// Token: 0x0200010F RID: 271
public class ReverbMix : MonoBehaviour
{
	// Token: 0x060007EC RID: 2028 RVA: 0x00029EC0 File Offset: 0x000280C0
	private void Start()
	{
		this.audioMixerGroup.audioMixer.GetFloat("EffectsStrength", out this.startReverbStrength);
		this.audioMixerGroup.audioMixer.SetFloat("EffectsStrength", this.reverbStrength);
	}

	// Token: 0x060007ED RID: 2029 RVA: 0x00029EFA File Offset: 0x000280FA
	private void Update()
	{
	}

	// Token: 0x060007EE RID: 2030 RVA: 0x00029EFC File Offset: 0x000280FC
	private void OnDisable()
	{
		this.audioMixerGroup.audioMixer.SetFloat("EffectsStrength", this.startReverbStrength);
	}

	// Token: 0x04000766 RID: 1894
	public AudioMixerGroup audioMixerGroup;

	// Token: 0x04000767 RID: 1895
	private float startReverbStrength;

	// Token: 0x04000768 RID: 1896
	public float reverbStrength;
}
