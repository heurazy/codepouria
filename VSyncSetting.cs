using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using Zorro.Settings;

// Token: 0x02000136 RID: 310
public class VSyncSetting : EnumSetting<VSyncSetting.VSyncMode>, IExposedSetting
{
	// Token: 0x060008FE RID: 2302 RVA: 0x0002DDAB File Offset: 0x0002BFAB
	public override void ApplyValue()
	{
		QualitySettings.vSyncCount = (int)base.Value;
		Application.targetFrameRate = 0;
	}

	// Token: 0x060008FF RID: 2303 RVA: 0x0002DDBE File Offset: 0x0002BFBE
	protected override VSyncSetting.VSyncMode GetDefaultValue()
	{
		return (VSyncSetting.VSyncMode)QualitySettings.vSyncCount;
	}

	// Token: 0x06000900 RID: 2304 RVA: 0x0002DDC5 File Offset: 0x0002BFC5
	public override List<LocalizedString> GetLocalizedChoices()
	{
		return null;
	}

	// Token: 0x06000901 RID: 2305 RVA: 0x0002DDC8 File Offset: 0x0002BFC8
	public string GetDisplayName()
	{
		return "Vsync";
	}

	// Token: 0x06000902 RID: 2306 RVA: 0x0002DDCF File Offset: 0x0002BFCF
	public string GetCategory()
	{
		return "Graphics";
	}

	// Token: 0x0200035E RID: 862
	public enum VSyncMode
	{
		// Token: 0x04001259 RID: 4697
		None,
		// Token: 0x0400125A RID: 4698
		Enabled
	}
}
