using System;
using Unity.Mathematics;
using Zorro.Settings;

// Token: 0x0200012F RID: 303
public class MouseSensitivitySetting : FloatSetting, IExposedSetting
{
	// Token: 0x060008D8 RID: 2264 RVA: 0x0002DB5C File Offset: 0x0002BD5C
	public override void ApplyValue()
	{
	}

	// Token: 0x060008D9 RID: 2265 RVA: 0x0002DB5E File Offset: 0x0002BD5E
	protected override float GetDefaultValue()
	{
		return 2f;
	}

	// Token: 0x060008DA RID: 2266 RVA: 0x0002DB65 File Offset: 0x0002BD65
	protected override float2 GetMinMaxValue()
	{
		return new float2(0.1f, 5f);
	}

	// Token: 0x060008DB RID: 2267 RVA: 0x0002DB76 File Offset: 0x0002BD76
	public string GetDisplayName()
	{
		return "Mouse Sensitivity";
	}

	// Token: 0x060008DC RID: 2268 RVA: 0x0002DB7D File Offset: 0x0002BD7D
	public string GetCategory()
	{
		return "General";
	}
}
