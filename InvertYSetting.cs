using System;
using System.Collections.Generic;
using UnityEngine.Localization;
using Zorro.Settings;

// Token: 0x0200012A RID: 298
public class InvertYSetting : OffOnSetting, IExposedSetting
{
	// Token: 0x060008B1 RID: 2225 RVA: 0x0002D829 File Offset: 0x0002BA29
	public override void ApplyValue()
	{
	}

	// Token: 0x060008B2 RID: 2226 RVA: 0x0002D82B File Offset: 0x0002BA2B
	protected override OffOnMode GetDefaultValue()
	{
		return OffOnMode.OFF;
	}

	// Token: 0x060008B3 RID: 2227 RVA: 0x0002D82E File Offset: 0x0002BA2E
	public override List<LocalizedString> GetLocalizedChoices()
	{
		return null;
	}

	// Token: 0x060008B4 RID: 2228 RVA: 0x0002D831 File Offset: 0x0002BA31
	public string GetDisplayName()
	{
		return "Invert Y";
	}

	// Token: 0x060008B5 RID: 2229 RVA: 0x0002D838 File Offset: 0x0002BA38
	public string GetCategory()
	{
		return "General";
	}
}
