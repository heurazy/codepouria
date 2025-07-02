using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

// Token: 0x020000EC RID: 236
[ExecuteInEditMode]
public class LightVolume : MonoBehaviour
{
	// Token: 0x06000709 RID: 1801 RVA: 0x00024ECC File Offset: 0x000230CC
	private void SetShaderVars()
	{
		Shader.SetGlobalFloat("brightness", this.brightness);
		Shader.SetGlobalFloat("ambienceStrength", this.ambienceStrength);
		Shader.SetGlobalFloat("ambienceMin", this.ambienceMin);
		Shader.SetGlobalVector("gridRes", this.gridRes);
		Shader.SetGlobalFloat("raySpacing", this.raySpacing);
		Shader.SetGlobalVector("gridOffset", this.gridOffset);
	}

	// Token: 0x0600070A RID: 1802 RVA: 0x00024F48 File Offset: 0x00023148
	public void SetSize()
	{
		Shader.SetGlobalTexture("_LightMap", null);
		Bounds totalBounds = LightVolume.GetTotalBounds((this.sceneParent == null) ? base.gameObject : this.sceneParent);
		this.gridOffset = totalBounds.center;
		this.gridRes = new Vector3Int(Mathf.CeilToInt((totalBounds.size.x + 3f) / this.raySpacing), Mathf.CeilToInt((totalBounds.size.y + 3f) / this.raySpacing), Mathf.CeilToInt((totalBounds.size.z + 3f) / this.raySpacing));
	}

	// Token: 0x0600070B RID: 1803 RVA: 0x00024FF4 File Offset: 0x000231F4
	private static Bounds GetTotalBounds(GameObject gameObject)
	{
		Bounds bounds = default(Bounds);
		bool flag = true;
		foreach (MeshRenderer meshRenderer in gameObject.GetComponentsInChildren<MeshRenderer>())
		{
			if (flag)
			{
				bounds = meshRenderer.bounds;
			}
			else
			{
				bounds.Encapsulate(meshRenderer.bounds);
			}
			flag = false;
		}
		return bounds;
	}

	// Token: 0x0600070C RID: 1804 RVA: 0x00025044 File Offset: 0x00023244
	private void OnDrawGizmosSelected()
	{
		if (!this.showVolumeGizmos)
		{
			return;
		}
		Gizmos.color = Color.black;
		Gizmos.DrawWireCube(this.gridOffset - Vector3.one * 0.25f, this.gridRes * this.raySpacing);
		Gizmos.color = Color.white;
		Gizmos.DrawWireCube(this.gridOffset + Vector3.one * 0.25f, this.gridRes * this.raySpacing);
	}

	// Token: 0x0600070D RID: 1805 RVA: 0x000250D8 File Offset: 0x000232D8
	private void Awake()
	{
		LightVolume.instance = this;
	}

	// Token: 0x0600070E RID: 1806 RVA: 0x000250E0 File Offset: 0x000232E0
	private void Start()
	{
		this.SetShaderVars();
		Shader.SetGlobalTexture("_LightMap", this.lightMap);
	}

	// Token: 0x1700005E RID: 94
	// (get) Token: 0x0600070F RID: 1807 RVA: 0x000250F8 File Offset: 0x000232F8
	private bool RaytracingShaderNotSupported
	{
		get
		{
			return !SystemInfo.supportsRayTracingShaders;
		}
	}

	// Token: 0x06000710 RID: 1808 RVA: 0x00025104 File Offset: 0x00023304
	public void Bake(Action onComplete = null)
	{
		if (!this.computeShader)
		{
			Debug.LogError("Cannot bake at runtime (serialize the ComputeShader if you want to do this)");
			return;
		}
		if (!this.rayTracingShader)
		{
			Debug.LogError("Cannot bake at runtime (serialize the RayTracingShader if you want to do this)");
			return;
		}
		this.SetSize();
		RenderTexture renderTexture = this.RunBake();
		RenderTexture renderTexture2 = this.RunBlur(renderTexture);
		renderTexture2.name = "LightVolumeRenderTexture";
		this.SetShaderVars();
		Shader.SetGlobalTexture("_LightMap", renderTexture2);
		this.SaveTex(renderTexture2, onComplete);
	}

	// Token: 0x06000711 RID: 1809 RVA: 0x0002517C File Offset: 0x0002337C
	private RenderTexture RunBake()
	{
		this.rayTracingShader.SetVector("gridRadius", new Vector3((float)this.gridRes.x, (float)this.gridRes.y, (float)this.gridRes.z) * (this.raySpacing / 2f));
		this.rayTracingShader.SetVector("gridOffset", this.gridOffset);
		this.rayTracingShader.SetVector("skyColor", this.skyColor);
		this.rayTracingShader.SetInt("rayCount", this.rayCount);
		ComputeBuffer computeBuffer;
		int num = this.BuildLights(out computeBuffer);
		IDisposable disposable;
		this.BuildMeshes(out disposable, num);
		RenderTexture renderTexture = LightVolume.Create3DTexture(FilterMode.Bilinear, RenderTextureFormat.ARGBHalf, this.gridRes);
		this.rayTracingShader.SetTexture("lightMap", renderTexture);
		for (int i = 0; i < num + 1; i++)
		{
			this.rayTracingShader.SetInt("doLightIndex", i);
			this.rayTracingShader.Dispatch("RaygenShader", this.gridRes.x, this.gridRes.y, this.gridRes.z, null);
		}
		computeBuffer.Dispose();
		disposable.Dispose();
		return renderTexture;
	}

	// Token: 0x06000712 RID: 1810 RVA: 0x000252B8 File Offset: 0x000234B8
	private static RenderTexture Create3DTexture(FilterMode filterMode, RenderTextureFormat format, Vector3Int resolution)
	{
		RenderTexture renderTexture = new RenderTexture(resolution.x, resolution.y, 0);
		renderTexture.enableRandomWrite = true;
		renderTexture.format = format;
		renderTexture.dimension = TextureDimension.Tex3D;
		renderTexture.volumeDepth = resolution.z;
		renderTexture.wrapMode = TextureWrapMode.Clamp;
		renderTexture.filterMode = filterMode;
		renderTexture.hideFlags = HideFlags.DontSave;
		if (!renderTexture.Create())
		{
			throw new Exception("Failed to create texture");
		}
		return renderTexture;
	}

	// Token: 0x06000713 RID: 1811 RVA: 0x00025324 File Offset: 0x00023524
	private int BuildLights(out ComputeBuffer toDispose)
	{
		List<LightVolume.GpuLight> list = new List<LightVolume.GpuLight>();
		GameObject gameObject = ((this.sceneParent == null) ? base.gameObject : this.sceneParent);
		if (this.allLightsFound == null)
		{
			this.allLightsFound = new List<BakedVolumeLight>();
		}
		this.allLightsFound.Clear();
		foreach (BakedVolumeLight bakedVolumeLight in gameObject.GetComponentsInChildren<BakedVolumeLight>())
		{
			this.allLightsFound.Add(bakedVolumeLight);
			Vector3 vector = new Vector3(bakedVolumeLight.color.r, bakedVolumeLight.color.g, bakedVolumeLight.color.b);
			BakedVolumeLight.LightModes mode = bakedVolumeLight.mode;
			float num;
			if (mode != BakedVolumeLight.LightModes.Point)
			{
				if (mode != BakedVolumeLight.LightModes.Spot)
				{
					throw new Exception();
				}
				num = bakedVolumeLight.coneSize * 0.017453292f;
			}
			else
			{
				num = 0f;
			}
			float num2 = num;
			list.Add(new LightVolume.GpuLight
			{
				Position = bakedVolumeLight.transform.position,
				ConeSize = num2,
				Direction = bakedVolumeLight.transform.forward,
				Radius = bakedVolumeLight.radius,
				Color = vector * bakedVolumeLight.intensity,
				Falloff = bakedVolumeLight.falloff,
				ConeFalloff = bakedVolumeLight.coneFalloff
			});
		}
		int count = list.Count;
		if (count == 0)
		{
			list.Add(default(LightVolume.GpuLight));
		}
		ComputeBuffer computeBuffer = new ComputeBuffer(list.Count, 52);
		computeBuffer.SetData<LightVolume.GpuLight>(list);
		this.rayTracingShader.SetBuffer("lightBuffer", computeBuffer);
		this.rayTracingShader.SetInt("lightBufferLength", count);
		toDispose = computeBuffer;
		return count;
	}

	// Token: 0x06000714 RID: 1812 RVA: 0x000254D8 File Offset: 0x000236D8
	private void BuildMeshes(out IDisposable toDispose, int lightCountForDebug)
	{
		int value = this.occluderMask.value;
		GameObject gameObject = ((this.sceneParent == null) ? base.gameObject : this.sceneParent);
		if (this.allMeshRenderersFound == null)
		{
			this.allMeshRenderersFound = new List<MeshRenderer>();
		}
		this.allMeshRenderersFound.Clear();
		RayTracingAccelerationStructure rayTracingAccelerationStructure = new RayTracingAccelerationStructure();
		uint num = 0U;
		int num2 = 0;
		foreach (MeshRenderer meshRenderer in gameObject.GetComponentsInChildren<MeshRenderer>())
		{
			if (((1 << meshRenderer.gameObject.layer) & value) != 0 || meshRenderer.GetComponent<LightingCollider>())
			{
				this.allMeshRenderersFound.Add(meshRenderer);
				Mesh sharedMesh = meshRenderer.GetComponent<MeshFilter>().sharedMesh;
				if (!(sharedMesh == null))
				{
					int subMeshCount = sharedMesh.subMeshCount;
					for (int j = 0; j < subMeshCount; j++)
					{
						num += sharedMesh.GetIndexCount(j);
					}
					num2 += sharedMesh.vertexCount;
					RayTracingSubMeshFlags[] array = new RayTracingSubMeshFlags[subMeshCount];
					for (int k = 0; k < array.Length; k++)
					{
						array[k] = RayTracingSubMeshFlags.Enabled | RayTracingSubMeshFlags.ClosestHitOnly;
					}
					rayTracingAccelerationStructure.AddInstance(meshRenderer, array, true, false, 255U, uint.MaxValue);
				}
			}
		}
		rayTracingAccelerationStructure.Build();
		Debug.Log(string.Format("Light Volume Baker found: {0} lights, {1} meshes, {2} indices, {3} vertices", new object[]
		{
			lightCountForDebug,
			this.allMeshRenderersFound.Count,
			num,
			num2
		}));
		this.rayTracingShader.SetAccelerationStructure("g_SceneAccelStruct", rayTracingAccelerationStructure);
		toDispose = rayTracingAccelerationStructure;
	}

	// Token: 0x06000715 RID: 1813 RVA: 0x0002566C File Offset: 0x0002386C
	private RenderTexture RunBlur(RenderTexture inputTex)
	{
		if (this.blurRadius <= 0)
		{
			return inputTex;
		}
		this.computeShader.SetInt("blurRadius", this.blurRadius);
		Vector3Int vector3Int = new Vector3Int(inputTex.width, inputTex.height, inputTex.volumeDepth);
		RenderTexture renderTexture = LightVolume.Create3DTexture(inputTex.filterMode, inputTex.format, vector3Int);
		for (int i = 0; i < 3; i++)
		{
			this.computeShader.SetTexture(1, "blurInputLightMap", inputTex);
			this.computeShader.SetTexture(1, "lightMap", renderTexture);
			this.computeShader.SetInt("blurAxis", i);
			uint num = 4U;
			uint num2 = 4U;
			uint num3 = 4U;
			long num4 = ((long)vector3Int.x + (long)((ulong)num) - 1L) / (long)((ulong)num);
			long num5 = ((long)vector3Int.y + (long)((ulong)num2) - 1L) / (long)((ulong)num2);
			long num6 = ((long)vector3Int.z + (long)((ulong)num3) - 1L) / (long)((ulong)num3);
			this.computeShader.Dispatch(1, (int)num4, (int)num5, (int)num6);
			RenderTexture renderTexture2 = inputTex;
			RenderTexture renderTexture3 = renderTexture;
			renderTexture = renderTexture2;
			inputTex = renderTexture3;
		}
		Object.DestroyImmediate(renderTexture);
		return inputTex;
	}

	// Token: 0x06000716 RID: 1814 RVA: 0x00025778 File Offset: 0x00023978
	private void SaveTex(RenderTexture renderTexture, Action onComplete = null)
	{
		AsyncGPUReadbackRequest asyncGPUReadbackRequest = AsyncGPUReadback.Request(renderTexture, 0, null);
		asyncGPUReadbackRequest.WaitForCompletion();
		byte[] array = new byte[asyncGPUReadbackRequest.layerDataSize * asyncGPUReadbackRequest.layerCount];
		for (int i = 0; i < asyncGPUReadbackRequest.layerCount; i++)
		{
			NativeArray<byte>.Copy(asyncGPUReadbackRequest.GetData<byte>(i), 0, array, i * asyncGPUReadbackRequest.layerDataSize, asyncGPUReadbackRequest.layerDataSize);
		}
		if (!this.lightMap || this.lightMap.width != renderTexture.width || this.lightMap.height != renderTexture.height || this.lightMap.depth != renderTexture.volumeDepth || this.lightMap.graphicsFormat != renderTexture.graphicsFormat)
		{
			if (this.lightMap)
			{
				Object.DestroyImmediate(this.lightMap);
			}
			this.lightMap = new Texture3D(renderTexture.width, renderTexture.height, renderTexture.volumeDepth, renderTexture.graphicsFormat, TextureCreationFlags.None);
		}
		this.lightMap.name = "LightVolumeBakeTexture";
		this.lightMap.wrapMode = renderTexture.wrapMode;
		this.lightMap.filterMode = renderTexture.filterMode;
		this.lightMap.SetPixelData<byte>(array, 0, 0);
		this.lightMap.Apply();
		Shader.SetGlobalTexture("_LightMap", this.lightMap);
		if (onComplete != null)
		{
			onComplete();
		}
		Object.DestroyImmediate(renderTexture);
	}

	// Token: 0x06000717 RID: 1815 RVA: 0x000258DA File Offset: 0x00023ADA
	public static LightVolume Instance()
	{
		if (LightVolume.instance == null)
		{
			LightVolume.instance = Object.FindAnyObjectByType<LightVolume>();
		}
		return LightVolume.instance;
	}

	// Token: 0x06000718 RID: 1816 RVA: 0x000258F8 File Offset: 0x00023AF8
	internal Color SamplePosition(Vector3 worldPos)
	{
		worldPos -= this.gridOffset;
		worldPos += this.raySpacing * this.gridRes * 0.5f;
		worldPos.x /= this.raySpacing;
		worldPos.y /= this.raySpacing;
		worldPos.z /= this.raySpacing;
		return this.lightMap.GetPixel((int)worldPos.x, (int)worldPos.y, (int)worldPos.z);
	}

	// Token: 0x06000719 RID: 1817 RVA: 0x00025990 File Offset: 0x00023B90
	public float SamplePositionAlpha(Vector3 worldPos)
	{
		worldPos -= this.gridOffset;
		worldPos += this.raySpacing * this.gridRes * 0.5f;
		worldPos.x /= this.raySpacing;
		worldPos.y /= this.raySpacing;
		worldPos.z /= this.raySpacing;
		return this.lightMap.GetPixel((int)worldPos.x, (int)worldPos.y, (int)worldPos.z).a;
	}

	// Token: 0x0400069F RID: 1695
	public bool showVolumeGizmos = true;

	// Token: 0x040006A0 RID: 1696
	public float brightness = 1f;

	// Token: 0x040006A1 RID: 1697
	public float ambienceStrength = 1f;

	// Token: 0x040006A2 RID: 1698
	public float ambienceMin = 0.05f;

	// Token: 0x040006A3 RID: 1699
	public Color skyColor = Color.white;

	// Token: 0x040006A4 RID: 1700
	public Vector3Int gridRes;

	// Token: 0x040006A5 RID: 1701
	public Vector3 gridOffset;

	// Token: 0x040006A6 RID: 1702
	public int rayCount = 128;

	// Token: 0x040006A7 RID: 1703
	public float raySpacing = 1.5f;

	// Token: 0x040006A8 RID: 1704
	[Tooltip("Colliders matching this mask will be used for light tracing, colliders not matching will be ignored")]
	public LayerMask occluderMask = -1;

	// Token: 0x040006A9 RID: 1705
	[Tooltip("Radius (in texels) for how much to box blur the output texture")]
	public int blurRadius;

	// Token: 0x040006AA RID: 1706
	public GameObject sceneParent;

	// Token: 0x040006AB RID: 1707
	public ComputeShader computeShader;

	// Token: 0x040006AC RID: 1708
	public RayTracingShader rayTracingShader;

	// Token: 0x040006AD RID: 1709
	public Texture3D lightMap;

	// Token: 0x040006AE RID: 1710
	public List<BakedVolumeLight> allLightsFound;

	// Token: 0x040006AF RID: 1711
	public List<MeshRenderer> allMeshRenderersFound;

	// Token: 0x040006B0 RID: 1712
	internal static LightVolume instance;

	// Token: 0x02000332 RID: 818
	private struct GpuLight
	{
		// Token: 0x040011BA RID: 4538
		public Vector3 Position;

		// Token: 0x040011BB RID: 4539
		public float ConeSize;

		// Token: 0x040011BC RID: 4540
		public Vector3 Direction;

		// Token: 0x040011BD RID: 4541
		public float Radius;

		// Token: 0x040011BE RID: 4542
		public Vector3 Color;

		// Token: 0x040011BF RID: 4543
		public float Falloff;

		// Token: 0x040011C0 RID: 4544
		public float ConeFalloff;
	}
}
