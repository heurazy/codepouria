using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

// Token: 0x0200013E RID: 318
public class SingleBufferFeature : ScriptableRendererFeature
{
	// Token: 0x0600092A RID: 2346 RVA: 0x0002E6C4 File Offset: 0x0002C8C4
	public override void Create()
	{
		this.m_ScriptablePass = new SingleBufferFeature.CustomRenderPass(this.settings, base.name);
		this.m_ScriptablePass.renderPassEvent = this.settings._event;
	}

	// Token: 0x0600092B RID: 2347 RVA: 0x0002E6F4 File Offset: 0x0002C8F4
	public unsafe override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
	{
		CameraType cameraType = *renderingData.cameraData.cameraType;
		if (cameraType == CameraType.Preview)
		{
			return;
		}
		if (!this.settings.showInSceneView && cameraType == CameraType.SceneView)
		{
			return;
		}
		renderer.EnqueuePass(this.m_ScriptablePass);
	}

	// Token: 0x0600092C RID: 2348 RVA: 0x0002E731 File Offset: 0x0002C931
	protected override void Dispose(bool disposing)
	{
		this.m_ScriptablePass.Dispose();
	}

	// Token: 0x0400082C RID: 2092
	public SingleBufferFeature.Settings settings = new SingleBufferFeature.Settings();

	// Token: 0x0400082D RID: 2093
	private SingleBufferFeature.CustomRenderPass m_ScriptablePass;

	// Token: 0x02000364 RID: 868
	public class CustomRenderPass : ScriptableRenderPass
	{
		// Token: 0x060013AE RID: 5038 RVA: 0x0005D4E4 File Offset: 0x0005B6E4
		public CustomRenderPass(SingleBufferFeature.Settings settings, string name)
		{
			this.settings = settings;
			this.filteringSettings = new FilteringSettings(new RenderQueueRange?(RenderQueueRange.transparent), settings.layerMask, uint.MaxValue, 0);
			this.shaderTagsList.Add(new ShaderTagId("SRPDefaultUnlit"));
			this.shaderTagsList.Add(new ShaderTagId("UniversalForward"));
			this.shaderTagsList.Add(new ShaderTagId("UniversalForwardOnly"));
			this._profilingSampler = new ProfilingSampler(name);
		}

		// Token: 0x060013AF RID: 5039 RVA: 0x0005D578 File Offset: 0x0005B778
		public unsafe override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
		{
			RenderTextureDescriptor renderTextureDescriptor = *renderingData.cameraData.cameraTargetDescriptor;
			renderTextureDescriptor.depthBufferBits = 0;
			RenderingUtils.ReAllocateIfNeeded(ref this.rtTempColor, in renderTextureDescriptor, FilterMode.Point, TextureWrapMode.Repeat, false, 1, 0f, "_TemporaryColorTexture");
			if (this.settings.colorTargetDestinationID != "")
			{
				RenderingUtils.ReAllocateIfNeeded(ref this.rtCustomColor, in renderTextureDescriptor, FilterMode.Point, TextureWrapMode.Repeat, false, 1, 0f, this.settings.colorTargetDestinationID);
			}
			else
			{
				this.rtCustomColor = renderingData.cameraData.renderer->cameraColorTargetHandle;
			}
			RTHandle cameraDepthTargetHandle = renderingData.cameraData.renderer->cameraDepthTargetHandle;
			base.ConfigureTarget(this.rtCustomColor, cameraDepthTargetHandle);
			base.ConfigureClear(ClearFlag.Color, new Color(0f, 0f, 0f, 0f));
		}

		// Token: 0x060013B0 RID: 5040 RVA: 0x0005D64C File Offset: 0x0005B84C
		public unsafe override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
		{
			CommandBuffer commandBuffer = CommandBufferPool.Get();
			using (new ProfilingScope(commandBuffer, this._profilingSampler))
			{
				context.ExecuteCommandBuffer(commandBuffer);
				commandBuffer.Clear();
				SortingCriteria sortingCriteria = SortingCriteria.CommonTransparent;
				DrawingSettings drawingSettings = base.CreateDrawingSettings(this.shaderTagsList, ref renderingData, sortingCriteria);
				if (this.settings.overrideMaterial != null)
				{
					drawingSettings.overrideMaterialPassIndex = this.settings.overrideMaterialPass;
					drawingSettings.overrideMaterial = this.settings.overrideMaterial;
				}
				context.DrawRenderers(*renderingData.cullResults, ref drawingSettings, ref this.filteringSettings);
				if (this.settings.colorTargetDestinationID != "")
				{
					commandBuffer.SetGlobalTexture(this.settings.colorTargetDestinationID, this.rtCustomColor);
				}
				if (this.settings.blitMaterial != null)
				{
					RTHandle cameraColorTargetHandle = renderingData.cameraData.renderer->cameraColorTargetHandle;
					if (cameraColorTargetHandle != null && this.rtTempColor != null)
					{
						Blitter.BlitCameraTexture(commandBuffer, cameraColorTargetHandle, this.rtTempColor, this.settings.blitMaterial, 0);
						Blitter.BlitCameraTexture(commandBuffer, this.rtTempColor, cameraColorTargetHandle, 0f, false);
					}
				}
			}
			context.ExecuteCommandBuffer(commandBuffer);
			commandBuffer.Clear();
			CommandBufferPool.Release(commandBuffer);
		}

		// Token: 0x060013B1 RID: 5041 RVA: 0x0005D7B4 File Offset: 0x0005B9B4
		public override void OnCameraCleanup(CommandBuffer cmd)
		{
		}

		// Token: 0x060013B2 RID: 5042 RVA: 0x0005D7B6 File Offset: 0x0005B9B6
		public void Dispose()
		{
			if (this.settings.colorTargetDestinationID != "")
			{
				RTHandle rthandle = this.rtCustomColor;
				if (rthandle != null)
				{
					rthandle.Release();
				}
			}
			RTHandle rthandle2 = this.rtTempColor;
			if (rthandle2 == null)
			{
				return;
			}
			rthandle2.Release();
		}

		// Token: 0x0400127A RID: 4730
		private SingleBufferFeature.Settings settings;

		// Token: 0x0400127B RID: 4731
		private FilteringSettings filteringSettings;

		// Token: 0x0400127C RID: 4732
		private ProfilingSampler _profilingSampler;

		// Token: 0x0400127D RID: 4733
		private List<ShaderTagId> shaderTagsList = new List<ShaderTagId>();

		// Token: 0x0400127E RID: 4734
		private RTHandle rtCustomColor;

		// Token: 0x0400127F RID: 4735
		private RTHandle rtTempColor;
	}

	// Token: 0x02000365 RID: 869
	[Serializable]
	public class Settings
	{
		// Token: 0x04001280 RID: 4736
		public bool showInSceneView = true;

		// Token: 0x04001281 RID: 4737
		public RenderPassEvent _event = RenderPassEvent.AfterRenderingOpaques;

		// Token: 0x04001282 RID: 4738
		[Header("Draw Renderers Settings")]
		public LayerMask layerMask = 1;

		// Token: 0x04001283 RID: 4739
		public Material overrideMaterial;

		// Token: 0x04001284 RID: 4740
		public int overrideMaterialPass;

		// Token: 0x04001285 RID: 4741
		public string colorTargetDestinationID = "";

		// Token: 0x04001286 RID: 4742
		[Header("Blit Settings")]
		public Material blitMaterial;
	}
}
