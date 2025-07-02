using System;
using UnityEngine.Audio;
using Zorro.Settings;

// Token: 0x02000130 RID: 304
public class MusicVolumeSetting : VolumeSetting, IExposedSetting
{
	// Token: 0x060008DE RID: 2270 RVA: 0x0002DB8C File Offset: 0x0002BD8C
	public MusicVolumeSetting(AudioMixerGroup mixerGroup)
		: base(mixerGroup)
	{
	}

	// Token: 0x060008DF RID: 2271 RVA: 0x0002DB95 File Offset: 0x0002BD95
	public override string GetParameterName()
	{
		return "MusicVolume";
	}

	// Token: 0x060008E0 RID: 2272 RVA: 0x0002DB9C File Offset: 0x0002BD9C
	public string GetDisplayName()
	{
		return "Music Volume";
	}

	// Token: 0x060008E1 RID: 2273 RVA: 0x0002DBA3 File Offset: 0x0002BDA3
	public string GetCategory()
	{
		return "Audio";
	}
}
