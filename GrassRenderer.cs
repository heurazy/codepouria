using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using Zorro.Core.Compute;

// Token: 0x02000096 RID: 150
[ExecuteInEditMode]
public class GrassRenderer : MonoBehaviour
{
	// Token: 0x06000527 RID: 1319 RVA: 0x0001D5E8 File Offset: 0x0001B7E8
	private void OnEnable()
	{
		ComputeBuffer geometryBuffer = this.GeometryBuffer;
		if (geometryBuffer != null)
		{
			geometryBuffer.Dispose();
		}
		ComputeBuffer argumentsBuffer = this.ArgumentsBuffer;
		if (argumentsBuffer != null)
		{
			argumentsBuffer.Dispose();
		}
		ComputeBuffer grassPointsBuffer = this.GrassPointsBuffer;
		if (grassPointsBuffer != null)
		{
			grassPointsBuffer.Dispose();
		}
		this.DataProvider = base.GetComponent<GrassDataProvider>();
	}

	// Token: 0x06000528 RID: 1320 RVA: 0x0001D634 File Offset: 0x0001B834
	private void OnDisable()
	{
		ComputeBuffer geometryBuffer = this.GeometryBuffer;
		if (geometryBuffer != null)
		{
			geometryBuffer.Dispose();
		}
		ComputeBuffer argumentsBuffer = this.ArgumentsBuffer;
		if (argumentsBuffer != null)
		{
			argumentsBuffer.Dispose();
		}
		ComputeBuffer grassPointsBuffer = this.GrassPointsBuffer;
		if (grassPointsBuffer != null)
		{
			grassPointsBuffer.Dispose();
		}
		this.GeometryBuffer = null;
		this.ArgumentsBuffer = null;
		this.GrassPointsBuffer = null;
	}

	// Token: 0x06000529 RID: 1321 RVA: 0x0001D689 File Offset: 0x0001B889
	private void Update()
	{
	}

	// Token: 0x0600052A RID: 1322 RVA: 0x0001D68C File Offset: 0x0001B88C
	private void Render()
	{
		if (!this.DataProvider)
		{
			this.DataProvider = base.GetComponent<GrassDataProvider>();
		}
		if (this.GrassPointsBuffer == null || this.DataProvider.IsDirty())
		{
			ComputeBuffer grassPointsBuffer = this.GrassPointsBuffer;
			if (grassPointsBuffer != null)
			{
				grassPointsBuffer.Dispose();
			}
			this.GrassPointsBuffer = this.DataProvider.GetData();
		}
		Camera camera = null;
		if (Application.isPlaying)
		{
			camera = MainCamera.instance.cam;
		}
		if (!GrassChunking.ShouldDrawChunk(GrassChunking.GetChunkFromPosition(camera.transform.position), this.CurrentChunk))
		{
			return;
		}
		if (this.GeometryBuffer == null)
		{
			this.GeometryBuffer = new ComputeBuffer(10000, 148, ComputeBufferType.Append);
			this.ArgumentsBuffer = new ComputeBuffer(1, 16, ComputeBufferType.DrawIndirect);
		}
		this.GeometryBuffer.SetCounterValue(0U);
		this.ArgumentsBuffer.SetData(this.argsBufferReset);
		this.grassComputeShader.SetBuffer(this.grassGeometryKernel.kernelID, "GeometryBuffer", this.GeometryBuffer);
		this.grassComputeShader.SetBuffer(this.grassGeometryKernel.kernelID, "IndirectArgsBuffer", this.ArgumentsBuffer);
		this.grassComputeShader.SetBuffer(this.grassGeometryKernel.kernelID, "GrassPoints", this.GrassPointsBuffer);
		this.grassComputeShader.SetFloat("_Time", Time.realtimeSinceStartup);
		this.grassComputeShader.SetVector("_CameraWSPos", camera.transform.position);
		MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
		materialPropertyBlock.SetBuffer("GeometryBuffer", this.GeometryBuffer);
		uint num;
		uint num2;
		uint num3;
		this.grassComputeShader.GetKernelThreadGroupSizes(this.grassGeometryKernel.kernelID, out num, out num2, out num3);
		this.grassGeometryKernel.Dispatch(new int3(this.GrassPointsBuffer.count, 1, 1));
		Graphics.DrawProceduralIndirect(this.m_grassRenderMaterial, new Bounds(base.transform.position, Vector3.one * 500f), MeshTopology.Triangles, this.ArgumentsBuffer, 0, null, materialPropertyBlock, ShadowCastingMode.Off, true, 0);
	}

	// Token: 0x0600052B RID: 1323 RVA: 0x0001D88D File Offset: 0x0001BA8D
	public GrassRenderer()
	{
		int[] array = new int[4];
		array[1] = 1;
		this.argsBufferReset = array;
		base..ctor();
	}

	// Token: 0x04000542 RID: 1346
	public int3 CurrentChunk = int3.zero;

	// Token: 0x04000543 RID: 1347
	public ComputeShader grassComputeShader;

	// Token: 0x04000544 RID: 1348
	private ComputeKernel grassGeometryKernel;

	// Token: 0x04000545 RID: 1349
	private ComputeBuffer GeometryBuffer;

	// Token: 0x04000546 RID: 1350
	private ComputeBuffer ArgumentsBuffer;

	// Token: 0x04000547 RID: 1351
	private ComputeBuffer GrassPointsBuffer;

	// Token: 0x04000548 RID: 1352
	private const int MAX_GRASS = 10000;

	// Token: 0x04000549 RID: 1353
	private const int DRAW_STRIDE = 148;

	// Token: 0x0400054A RID: 1354
	private const int INDIRECT_DRAW_ARGS_STIDE = 16;

	// Token: 0x0400054B RID: 1355
	private int[] argsBufferReset;

	// Token: 0x0400054C RID: 1356
	public Material m_grassRenderMaterial;

	// Token: 0x0400054D RID: 1357
	private GrassDataProvider DataProvider;
}
