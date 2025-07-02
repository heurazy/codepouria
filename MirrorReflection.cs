using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000189 RID: 393
[ExecuteInEditMode]
public class MirrorReflection : MonoBehaviour
{
	// Token: 0x06000AD5 RID: 2773 RVA: 0x00035488 File Offset: 0x00033688
	public void OnWillRenderObject()
	{
		Renderer component = base.GetComponent<Renderer>();
		if (!base.enabled || !component || !component.sharedMaterial || !component.enabled)
		{
			return;
		}
		Camera current = Camera.current;
		if (!current)
		{
			return;
		}
		if (MirrorReflection.s_InsideRendering)
		{
			return;
		}
		MirrorReflection.s_InsideRendering = true;
		Camera camera;
		this.CreateMirrorObjects(current, out camera);
		Vector3 position = base.transform.position;
		Vector3 up = base.transform.up;
		int pixelLightCount = QualitySettings.pixelLightCount;
		if (this.m_DisablePixelLights)
		{
			QualitySettings.pixelLightCount = 0;
		}
		this.UpdateCameraModes(current, camera);
		float num = -Vector3.Dot(up, position) - this.m_ClipPlaneOffset;
		Vector4 vector = new Vector4(up.x, up.y, up.z, num);
		Matrix4x4 zero = Matrix4x4.zero;
		MirrorReflection.CalculateReflectionMatrix(ref zero, vector);
		Vector3 position2 = current.transform.position;
		Vector3 vector2 = zero.MultiplyPoint(position2);
		camera.worldToCameraMatrix = current.worldToCameraMatrix * zero;
		Vector4 vector3 = this.CameraSpacePlane(camera, position, up, 1f);
		Matrix4x4 matrix4x = current.CalculateObliqueMatrix(vector3);
		camera.projectionMatrix = matrix4x;
		camera.cullingMask = -17 & this.m_ReflectLayers.value;
		camera.targetTexture = this.m_ReflectionTexture;
		GL.invertCulling = true;
		camera.transform.position = vector2;
		Vector3 eulerAngles = current.transform.eulerAngles;
		camera.transform.eulerAngles = new Vector3(0f, eulerAngles.y, eulerAngles.z);
		camera.Render();
		camera.transform.position = position2;
		GL.invertCulling = false;
		Shader.SetGlobalTexture("_ReflectionTex", this.m_ReflectionTexture);
		if (this.m_DisablePixelLights)
		{
			QualitySettings.pixelLightCount = pixelLightCount;
		}
		MirrorReflection.s_InsideRendering = false;
	}

	// Token: 0x06000AD6 RID: 2774 RVA: 0x00035650 File Offset: 0x00033850
	private void OnDisable()
	{
		if (this.m_ReflectionTexture)
		{
			Object.DestroyImmediate(this.m_ReflectionTexture);
			this.m_ReflectionTexture = null;
		}
		foreach (object obj in this.m_ReflectionCameras)
		{
			Object.DestroyImmediate(((Camera)((DictionaryEntry)obj).Value).gameObject);
		}
		this.m_ReflectionCameras.Clear();
	}

	// Token: 0x06000AD7 RID: 2775 RVA: 0x000356E4 File Offset: 0x000338E4
	private void UpdateCameraModes(Camera src, Camera dest)
	{
		if (dest == null)
		{
			return;
		}
		dest.clearFlags = src.clearFlags;
		dest.backgroundColor = src.backgroundColor;
		if (src.clearFlags == CameraClearFlags.Skybox)
		{
			Skybox skybox = src.GetComponent(typeof(Skybox)) as Skybox;
			Skybox skybox2 = dest.GetComponent(typeof(Skybox)) as Skybox;
			if (!skybox || !skybox.material)
			{
				skybox2.enabled = false;
			}
			else
			{
				skybox2.enabled = true;
				skybox2.material = skybox.material;
			}
		}
		dest.farClipPlane = src.farClipPlane;
		dest.nearClipPlane = src.nearClipPlane;
		dest.orthographic = src.orthographic;
		dest.fieldOfView = src.fieldOfView;
		dest.aspect = src.aspect;
		dest.orthographicSize = src.orthographicSize;
	}

	// Token: 0x06000AD8 RID: 2776 RVA: 0x000357C4 File Offset: 0x000339C4
	private void CreateMirrorObjects(Camera currentCamera, out Camera reflectionCamera)
	{
		reflectionCamera = null;
		if (!this.m_ReflectionTexture || this.m_OldReflectionTextureSize != this.m_TextureSize)
		{
			if (this.m_ReflectionTexture)
			{
				Object.DestroyImmediate(this.m_ReflectionTexture);
			}
			this.m_ReflectionTexture = new RenderTexture(this.m_TextureSize, this.m_TextureSize, 16);
			this.m_ReflectionTexture.name = "__MirrorReflection" + base.GetInstanceID().ToString();
			this.m_ReflectionTexture.isPowerOfTwo = true;
			this.m_ReflectionTexture.hideFlags = HideFlags.DontSave;
			this.m_OldReflectionTextureSize = this.m_TextureSize;
		}
		reflectionCamera = this.m_ReflectionCameras[currentCamera] as Camera;
		if (!reflectionCamera)
		{
			GameObject gameObject = new GameObject("Mirror Refl Camera id" + base.GetInstanceID().ToString() + " for " + currentCamera.GetInstanceID().ToString(), new Type[]
			{
				typeof(Camera),
				typeof(Skybox)
			});
			reflectionCamera = gameObject.GetComponent<Camera>();
			reflectionCamera.enabled = false;
			reflectionCamera.transform.position = base.transform.position;
			reflectionCamera.transform.rotation = base.transform.rotation;
			gameObject.hideFlags = HideFlags.HideAndDontSave;
			this.m_ReflectionCameras[currentCamera] = reflectionCamera;
		}
	}

	// Token: 0x06000AD9 RID: 2777 RVA: 0x0003592C File Offset: 0x00033B2C
	private static float sgn(float a)
	{
		if (a > 0f)
		{
			return 1f;
		}
		if (a < 0f)
		{
			return -1f;
		}
		return 0f;
	}

	// Token: 0x06000ADA RID: 2778 RVA: 0x00035950 File Offset: 0x00033B50
	private Vector4 CameraSpacePlane(Camera cam, Vector3 pos, Vector3 normal, float sideSign)
	{
		Vector3 vector = pos + normal * this.m_ClipPlaneOffset;
		Matrix4x4 worldToCameraMatrix = cam.worldToCameraMatrix;
		Vector3 vector2 = worldToCameraMatrix.MultiplyPoint(vector);
		Vector3 vector3 = worldToCameraMatrix.MultiplyVector(normal).normalized * sideSign;
		return new Vector4(vector3.x, vector3.y, vector3.z, -Vector3.Dot(vector2, vector3));
	}

	// Token: 0x06000ADB RID: 2779 RVA: 0x000359B8 File Offset: 0x00033BB8
	private static void CalculateReflectionMatrix(ref Matrix4x4 reflectionMat, Vector4 plane)
	{
		reflectionMat.m00 = 1f - 2f * plane[0] * plane[0];
		reflectionMat.m01 = -2f * plane[0] * plane[1];
		reflectionMat.m02 = -2f * plane[0] * plane[2];
		reflectionMat.m03 = -2f * plane[3] * plane[0];
		reflectionMat.m10 = -2f * plane[1] * plane[0];
		reflectionMat.m11 = 1f - 2f * plane[1] * plane[1];
		reflectionMat.m12 = -2f * plane[1] * plane[2];
		reflectionMat.m13 = -2f * plane[3] * plane[1];
		reflectionMat.m20 = -2f * plane[2] * plane[0];
		reflectionMat.m21 = -2f * plane[2] * plane[1];
		reflectionMat.m22 = 1f - 2f * plane[2] * plane[2];
		reflectionMat.m23 = -2f * plane[3] * plane[2];
		reflectionMat.m30 = 0f;
		reflectionMat.m31 = 0f;
		reflectionMat.m32 = 0f;
		reflectionMat.m33 = 1f;
	}

	// Token: 0x040009E1 RID: 2529
	public bool m_DisablePixelLights = true;

	// Token: 0x040009E2 RID: 2530
	public int m_TextureSize = 256;

	// Token: 0x040009E3 RID: 2531
	public float m_ClipPlaneOffset = 0.07f;

	// Token: 0x040009E4 RID: 2532
	public LayerMask m_ReflectLayers = -1;

	// Token: 0x040009E5 RID: 2533
	private Hashtable m_ReflectionCameras = new Hashtable();

	// Token: 0x040009E6 RID: 2534
	private RenderTexture m_ReflectionTexture;

	// Token: 0x040009E7 RID: 2535
	private int m_OldReflectionTextureSize;

	// Token: 0x040009E8 RID: 2536
	private static bool s_InsideRendering;
}
