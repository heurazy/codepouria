using System;
using System.Collections.Generic;
using Steamworks;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Zorro.Settings;

// Token: 0x02000132 RID: 306
public class RenderScaleSetting : EnumSetting<RenderScaleSetting.RenderScaleQuality>, IExposedSetting
{
	// Token: 0x060008E9 RID: 2281 RVA: 0x0002DBE8 File Offset: 0x0002BDE8
	public override void ApplyValue()
	{
		UniversalRenderPipelineAsset universalRenderPipelineAsset = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
		if (universalRenderPipelineAsset != null)
		{
			universalRenderPipelineAsset.renderScale = this.GetRenderScale(base.Value);
			Debug.Log(string.Format("Set Render Scale: {0}", universalRenderPipelineAsset.renderScale));
			if (base.Value == RenderScaleSetting.RenderScaleQuality.Native)
			{
				universalRenderPipelineAsset.upscalingFilter = UpscalingFilterSelection.Linear;
				return;
			}
			universalRenderPipelineAsset.upscalingFilter = UpscalingFilterSelection.STP;
		}
	}

	// Token: 0x060008EA RID: 2282 RVA: 0x0002DC48 File Offset: 0x0002BE48
	public float GetRenderScale(RenderScaleSetting.RenderScaleQuality quality)
	{
		float num;
		switch (quality)
		{
		case RenderScaleSetting.RenderScaleQuality.Native:
			num = 1f;
			break;
		case RenderScaleSetting.RenderScaleQuality.High:
			num = 0.8f;
			break;
		case RenderScaleSetting.RenderScaleQuality.Medium:
			num = 0.6f;
			break;
		case RenderScaleSetting.RenderScaleQuality.Low:
			num = 0.4f;
			break;
		default:
			throw new ArgumentOutOfRangeException("quality", quality, null);
		}
		return num;
	}

	// Token: 0x060008EB RID: 2283 RVA: 0x0002DCA0 File Offset: 0x0002BEA0
	protected override RenderScaleSetting.RenderScaleQuality GetDefaultValue()
	{
		if (SteamUtils.IsSteamRunningOnSteamDeck())
		{
			return RenderScaleSetting.RenderScaleQuality.Medium;
		}
		return RenderScaleSetting.RenderScaleQuality.High;
	}

	// Token: 0x060008EC RID: 2284 RVA: 0x0002DCAC File Offset: 0x0002BEAC
	public override List<LocalizedString> GetLocalizedChoices()
	{
		return null;
	}

	// Token: 0x060008ED RID: 2285 RVA: 0x0002DCAF File Offset: 0x0002BEAF
	public string GetDisplayName()
	{
		return "Render Scale";
	}

	// Token: 0x060008EE RID: 2286 RVA: 0x0002DCB6 File Offset: 0x0002BEB6
	public string GetCategory()
	{
		return "Graphics";
	}

	// Token: 0x0200035C RID: 860
	public enum RenderScaleQuality
	{
		// Token: 0x0400124F RID: 4687
		Native,
		// Token: 0x04001250 RID: 4688
		High,
		// Token: 0x04001251 RID: 4689
		Medium,
		// Token: 0x04001252 RID: 4690
		Low
	}
}
