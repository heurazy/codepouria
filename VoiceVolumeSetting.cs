using System;
using UnityEngine.Audio;
using Zorro.Settings;

// Token: 0x02000135 RID: 309
public class VoiceVolumeSetting : VolumeSetting, IExposedSetting
{
	// Token: 0x060008FA RID: 2298 RVA: 0x0002DD8D File Offset: 0x0002BF8D
	public VoiceVolumeSetting(AudioMixerGroup mixerGroup)
		: base(mixerGroup)
	{
	}

	// Token: 0x060008FB RID: 2299 RVA: 0x0002DD96 File Offset: 0x0002BF96
	public override string GetParameterName()
	{
		return "VoiceVolume";
	}

	// Token: 0x060008FC RID: 2300 RVA: 0x0002DD9D File Offset: 0x0002BF9D
	public string GetDisplayName()
	{
		return "Voices Volume";
	}

	// Token: 0x060008FD RID: 2301 RVA: 0x0002DDA4 File Offset: 0x0002BFA4
	public string GetCategory()
	{
		return "Audio";
	}
}
