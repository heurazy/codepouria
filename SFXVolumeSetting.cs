using System;
using UnityEngine.Audio;
using Zorro.Settings;

// Token: 0x02000133 RID: 307
public class SFXVolumeSetting : VolumeSetting, IExposedSetting
{
	// Token: 0x060008F0 RID: 2288 RVA: 0x0002DCC5 File Offset: 0x0002BEC5
	public SFXVolumeSetting(AudioMixerGroup mixerGroup)
		: base(mixerGroup)
	{
	}

	// Token: 0x060008F1 RID: 2289 RVA: 0x0002DCCE File Offset: 0x0002BECE
	public override string GetParameterName()
	{
		return "SFXVolume";
	}

	// Token: 0x060008F2 RID: 2290 RVA: 0x0002DCD5 File Offset: 0x0002BED5
	public string GetDisplayName()
	{
		return "SFX Volume";
	}

	// Token: 0x060008F3 RID: 2291 RVA: 0x0002DCDC File Offset: 0x0002BEDC
	public string GetCategory()
	{
		return "Audio";
	}
}
