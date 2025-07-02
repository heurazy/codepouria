using System;
using System.Collections.Generic;
using UnityEngine.Localization;
using Zorro.Settings;

// Token: 0x02000129 RID: 297
public class InvertXSetting : OffOnSetting, IExposedSetting
{
	// Token: 0x060008AB RID: 2219 RVA: 0x0002D80B File Offset: 0x0002BA0B
	public override void ApplyValue()
	{
	}

	// Token: 0x060008AC RID: 2220 RVA: 0x0002D80D File Offset: 0x0002BA0D
	protected override OffOnMode GetDefaultValue()
	{
		return OffOnMode.OFF;
	}

	// Token: 0x060008AD RID: 2221 RVA: 0x0002D810 File Offset: 0x0002BA10
	public override List<LocalizedString> GetLocalizedChoices()
	{
		return null;
	}

	// Token: 0x060008AE RID: 2222 RVA: 0x0002D813 File Offset: 0x0002BA13
	public string GetDisplayName()
	{
		return "Invert Y";
	}

	// Token: 0x060008AF RID: 2223 RVA: 0x0002D81A File Offset: 0x0002BA1A
	public string GetCategory()
	{
		return "General";
	}
}
