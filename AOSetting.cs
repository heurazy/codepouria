using System;
using System.Collections.Generic;
using Steamworks;
using UnityEngine.Localization;
using Zorro.Settings;

// Token: 0x02000124 RID: 292
public class AOSetting : OffOnSetting, IExposedSetting
{
	// Token: 0x06000887 RID: 2183 RVA: 0x0002D66D File Offset: 0x0002B86D
	public override void ApplyValue()
	{
	}

	// Token: 0x06000888 RID: 2184 RVA: 0x0002D66F File Offset: 0x0002B86F
	protected override OffOnMode GetDefaultValue()
	{
		if (SteamUtils.IsSteamRunningOnSteamDeck())
		{
			return OffOnMode.OFF;
		}
		return OffOnMode.ON;
	}

	// Token: 0x06000889 RID: 2185 RVA: 0x0002D67B File Offset: 0x0002B87B
	public override List<LocalizedString> GetLocalizedChoices()
	{
		return null;
	}

	// Token: 0x0600088A RID: 2186 RVA: 0x0002D67E File Offset: 0x0002B87E
	public string GetDisplayName()
	{
		return "Ambient Occlusion";
	}

	// Token: 0x0600088B RID: 2187 RVA: 0x0002D685 File Offset: 0x0002B885
	public string GetCategory()
	{
		return "Graphics";
	}
}
