using System;
using Unity.Mathematics;
using UnityEngine;
using Zorro.Settings;

// Token: 0x02000127 RID: 295
public class FPSCapSetting : FloatSetting, IExposedSetting
{
	// Token: 0x06000899 RID: 2201 RVA: 0x0002D6F4 File Offset: 0x0002B8F4
	public override void ApplyValue()
	{
		Application.targetFrameRate = Mathf.RoundToInt(base.Value);
	}

	// Token: 0x0600089A RID: 2202 RVA: 0x0002D706 File Offset: 0x0002B906
	public string GetDisplayName()
	{
		return "Max Framerate";
	}

	// Token: 0x0600089B RID: 2203 RVA: 0x0002D70D File Offset: 0x0002B90D
	public string GetCategory()
	{
		return "Graphics";
	}

	// Token: 0x0600089C RID: 2204 RVA: 0x0002D714 File Offset: 0x0002B914
	protected override float GetDefaultValue()
	{
		return 400f;
	}

	// Token: 0x0600089D RID: 2205 RVA: 0x0002D71B File Offset: 0x0002B91B
	protected override float2 GetMinMaxValue()
	{
		return new float2(30f, 600f);
	}
}
