using System;
using UnityEngine.Audio;
using Zorro.Settings;

// Token: 0x0200012D RID: 301
public class MasterVolumeSetting : VolumeSetting, IExposedSetting
{
	// Token: 0x060008C6 RID: 2246 RVA: 0x0002D8DF File Offset: 0x0002BADF
	public MasterVolumeSetting(AudioMixerGroup mixerGroup)
		: base(mixerGroup)
	{
	}

	// Token: 0x060008C7 RID: 2247 RVA: 0x0002D8E8 File Offset: 0x0002BAE8
	public override string GetParameterName()
	{
		return "MasterVolume";
	}

	// Token: 0x060008C8 RID: 2248 RVA: 0x0002D8EF File Offset: 0x0002BAEF
	public string GetDisplayName()
	{
		return "Master Volume";
	}

	// Token: 0x060008C9 RID: 2249 RVA: 0x0002D8F6 File Offset: 0x0002BAF6
	public string GetCategory()
	{
		return "Audio";
	}
}
