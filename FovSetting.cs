using System;
using Unity.Mathematics;
using Zorro.Settings;

// Token: 0x02000126 RID: 294
public class FovSetting : FloatSetting, IExposedSetting
{
	// Token: 0x06000893 RID: 2195 RVA: 0x0002D6C4 File Offset: 0x0002B8C4
	public override void ApplyValue()
	{
	}

	// Token: 0x06000894 RID: 2196 RVA: 0x0002D6C6 File Offset: 0x0002B8C6
	protected override float GetDefaultValue()
	{
		return 70f;
	}

	// Token: 0x06000895 RID: 2197 RVA: 0x0002D6CD File Offset: 0x0002B8CD
	protected override float2 GetMinMaxValue()
	{
		return new float2(60f, 100f);
	}

	// Token: 0x06000896 RID: 2198 RVA: 0x0002D6DE File Offset: 0x0002B8DE
	public string GetDisplayName()
	{
		return "Field of view";
	}

	// Token: 0x06000897 RID: 2199 RVA: 0x0002D6E5 File Offset: 0x0002B8E5
	public string GetCategory()
	{
		return "General";
	}
}
