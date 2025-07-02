using System;
using System.Collections.Generic;
using Steamworks;
using UnityEngine;
using UnityEngine.Localization;
using Zorro.Settings;

// Token: 0x0200012C RID: 300
public class LodQuality : EnumSetting<LodQuality.Quality>, IExposedSetting
{
	// Token: 0x060008BF RID: 2239 RVA: 0x0002D88C File Offset: 0x0002BA8C
	public override void ApplyValue()
	{
		QualitySettings.lodBias = this.GetBias(base.Value);
	}

	// Token: 0x060008C0 RID: 2240 RVA: 0x0002D89F File Offset: 0x0002BA9F
	private float GetBias(LodQuality.Quality value)
	{
		if (value == LodQuality.Quality.High)
		{
			return 1f;
		}
		if (value == LodQuality.Quality.Medium)
		{
			return 0.85f;
		}
		return 0.75f;
	}

	// Token: 0x060008C1 RID: 2241 RVA: 0x0002D8BA File Offset: 0x0002BABA
	protected override LodQuality.Quality GetDefaultValue()
	{
		if (SteamUtils.IsSteamRunningOnSteamDeck())
		{
			return LodQuality.Quality.Low;
		}
		return LodQuality.Quality.Medium;
	}

	// Token: 0x060008C2 RID: 2242 RVA: 0x0002D8C6 File Offset: 0x0002BAC6
	public override List<LocalizedString> GetLocalizedChoices()
	{
		return null;
	}

	// Token: 0x060008C3 RID: 2243 RVA: 0x0002D8C9 File Offset: 0x0002BAC9
	public string GetDisplayName()
	{
		return "World Quality";
	}

	// Token: 0x060008C4 RID: 2244 RVA: 0x0002D8D0 File Offset: 0x0002BAD0
	public string GetCategory()
	{
		return "Graphics";
	}

	// Token: 0x02000357 RID: 855
	public enum Quality
	{
		// Token: 0x04001242 RID: 4674
		Low,
		// Token: 0x04001243 RID: 4675
		Medium,
		// Token: 0x04001244 RID: 4676
		High
	}
}
