using System;
using System.Collections.Generic;
using Steamworks;
using UnityEngine.Localization;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Zorro.Settings;

// Token: 0x02000134 RID: 308
public class ShadowDistanceSettings : EnumSetting<ShadowDistanceSettings.ShadowDistanceQuality>, IExposedSetting
{
	// Token: 0x060008F4 RID: 2292 RVA: 0x0002DCE4 File Offset: 0x0002BEE4
	public override void ApplyValue()
	{
		UniversalRenderPipelineAsset universalRenderPipelineAsset = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
		if (universalRenderPipelineAsset != null)
		{
			switch (base.Value)
			{
			case ShadowDistanceSettings.ShadowDistanceQuality.High:
				universalRenderPipelineAsset.shadowDistance = 200f;
				universalRenderPipelineAsset.shadowCascadeCount = 2;
				return;
			case ShadowDistanceSettings.ShadowDistanceQuality.Medium:
				universalRenderPipelineAsset.shadowDistance = 150f;
				universalRenderPipelineAsset.shadowCascadeCount = 2;
				return;
			case ShadowDistanceSettings.ShadowDistanceQuality.Low:
				universalRenderPipelineAsset.shadowDistance = 75f;
				universalRenderPipelineAsset.shadowCascadeCount = 1;
				return;
			case ShadowDistanceSettings.ShadowDistanceQuality.Off:
				universalRenderPipelineAsset.shadowDistance = 0f;
				universalRenderPipelineAsset.shadowCascadeCount = 1;
				break;
			default:
				return;
			}
		}
	}

	// Token: 0x060008F5 RID: 2293 RVA: 0x0002DD68 File Offset: 0x0002BF68
	protected override ShadowDistanceSettings.ShadowDistanceQuality GetDefaultValue()
	{
		if (SteamUtils.IsSteamRunningOnSteamDeck())
		{
			return ShadowDistanceSettings.ShadowDistanceQuality.Low;
		}
		return ShadowDistanceSettings.ShadowDistanceQuality.Medium;
	}

	// Token: 0x060008F6 RID: 2294 RVA: 0x0002DD74 File Offset: 0x0002BF74
	public override List<LocalizedString> GetLocalizedChoices()
	{
		return null;
	}

	// Token: 0x060008F7 RID: 2295 RVA: 0x0002DD77 File Offset: 0x0002BF77
	public string GetDisplayName()
	{
		return "Shadow Distance";
	}

	// Token: 0x060008F8 RID: 2296 RVA: 0x0002DD7E File Offset: 0x0002BF7E
	public string GetCategory()
	{
		return "Graphics";
	}

	// Token: 0x0200035D RID: 861
	public enum ShadowDistanceQuality
	{
		// Token: 0x04001254 RID: 4692
		High,
		// Token: 0x04001255 RID: 4693
		Medium,
		// Token: 0x04001256 RID: 4694
		Low,
		// Token: 0x04001257 RID: 4695
		Off
	}
}
