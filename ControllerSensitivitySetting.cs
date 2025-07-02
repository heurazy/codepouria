using System;
using Unity.Mathematics;
using Zorro.Settings;

// Token: 0x02000125 RID: 293
public class ControllerSensitivitySetting : FloatSetting, IExposedSetting
{
	// Token: 0x0600088D RID: 2189 RVA: 0x0002D694 File Offset: 0x0002B894
	public override void ApplyValue()
	{
	}

	// Token: 0x0600088E RID: 2190 RVA: 0x0002D696 File Offset: 0x0002B896
	protected override float GetDefaultValue()
	{
		return 2f;
	}

	// Token: 0x0600088F RID: 2191 RVA: 0x0002D69D File Offset: 0x0002B89D
	protected override float2 GetMinMaxValue()
	{
		return new float2(0.1f, 5f);
	}

	// Token: 0x06000890 RID: 2192 RVA: 0x0002D6AE File Offset: 0x0002B8AE
	public string GetDisplayName()
	{
		return "Controller Sensitivity";
	}

	// Token: 0x06000891 RID: 2193 RVA: 0x0002D6B5 File Offset: 0x0002B8B5
	public string GetCategory()
	{
		return "General";
	}
}
