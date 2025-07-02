using System;
using Unity.Mathematics;
using UnityEngine;

// Token: 0x020000F3 RID: 243
[DefaultExecutionOrder(99999)]
public class Mirror : MonoBehaviour
{
	// Token: 0x0600073C RID: 1852 RVA: 0x000263FC File Offset: 0x000245FC
	private void Start()
	{
		Vector2 quadSize = this.getQuadSize();
		this.mirrorWidth = quadSize.x;
		this.mirrorHeight = quadSize.y;
	}

	// Token: 0x0600073D RID: 1853 RVA: 0x00026428 File Offset: 0x00024628
	private Vector2 getQuadSize()
	{
		Vector2 vector = default(Vector2);
		Renderer component = this.mirrorTransform.GetComponent<Renderer>();
		vector.x = Mathf.Abs(component.bounds.size.z);
		vector.y = component.bounds.size.y;
		return vector;
	}

	// Token: 0x0600073E RID: 1854 RVA: 0x00026484 File Offset: 0x00024684
	private void LateUpdate()
	{
		if (this.player == null && Character.localCharacter != null)
		{
			this.player = Character.localCharacter;
		}
		if (this.player == null)
		{
			return;
		}
		if (Camera.main != null && !this.isInitialized)
		{
			this.mainCam = Camera.main;
			this.mirrorCamera.depth -= 1f;
			this.mirrorCamera.targetTexture = this.renderTexture;
			this.isInitialized = true;
		}
		this.mainCam.transform.position - this.mirrorTransform.position;
		if (this.mirrorCamera == null)
		{
			return;
		}
		Vector3 up = this.mirrorTransform.up;
		Vector3 position = this.mirrorTransform.position;
		Vector3 vector = this.mainCam.transform.position - position;
		Vector3 vector2 = Vector3.Reflect(vector, up);
		this.depth = vector.x;
		if (this.useCameraTransform)
		{
			this.mirrorCamera.transform.position = position + vector2 + this.mirrorTransform.forward * this.verticalOffset;
		}
		Vector3 vector3 = Vector3.Reflect(this.mainCam.transform.up, up);
		Quaternion quaternion = Quaternion.LookRotation(Vector3.Reflect(this.mainCam.transform.forward, up), vector3);
		if (this.useCameraRot)
		{
			this.mirrorCamera.transform.rotation = quaternion;
		}
		this.mirrorCamera.projectionMatrix = Mirror.MirrorProjectionMatrix(this.mirrorCamera, this.mirrorCamera.farClipPlane, this.nearplaneOffset, this.mirrorTransform.position, this.mirrorTransform.up, this.mirrorWidth, this.mirrorHeight);
	}

	// Token: 0x0600073F RID: 1855 RVA: 0x0002665C File Offset: 0x0002485C
	private void OnPreRender()
	{
		GL.invertCulling = true;
	}

	// Token: 0x06000740 RID: 1856 RVA: 0x00026664 File Offset: 0x00024864
	private void OnPostRender()
	{
		GL.invertCulling = false;
	}

	// Token: 0x06000741 RID: 1857 RVA: 0x0002666C File Offset: 0x0002486C
	public void OnPreCull()
	{
	}

	// Token: 0x06000742 RID: 1858 RVA: 0x00026670 File Offset: 0x00024870
	public static Matrix4x4 MirrorProjectionMatrix(Camera cam, float far, float near, Vector3 mirrorCenter, Vector3 mirrorForward, float mirrorWidth, float mirrorHeight)
	{
		Transform transform = cam.transform;
		Vector3 vector = -Vector3.Cross(mirrorForward, Vector3.up).normalized;
		Vector3 vector2 = transform.InverseTransformPoint(mirrorCenter + -vector * (mirrorWidth / 2f));
		Vector3 vector3 = transform.InverseTransformPoint(mirrorCenter + vector * (mirrorWidth / 2f));
		Vector3 vector4 = transform.InverseTransformPoint(mirrorCenter + Vector3.up * (mirrorHeight / 2f));
		Vector3 vector5 = transform.InverseTransformPoint(mirrorCenter + Vector3.down * (mirrorHeight / 2f));
		Vector3 normalized = vector2.normalized;
		Vector3 normalized2 = vector3.normalized;
		Vector3 normalized3 = vector4.normalized;
		Vector3 normalized4 = vector5.normalized;
		Plane plane = new Plane(Vector3.forward, Vector3.forward * near);
		float num;
		float num2;
		float num3;
		float num4;
		if (plane.Raycast(new Ray(Vector3.zero, normalized), out num) && plane.Raycast(new Ray(Vector3.zero, normalized2), out num2) && plane.Raycast(new Ray(Vector3.zero, normalized3), out num3) && plane.Raycast(new Ray(Vector3.zero, normalized4), out num4))
		{
			float x = (normalized * num).x;
			float x2 = (normalized2 * num2).x;
			float y = (normalized3 * num3).y;
			float y2 = (normalized4 * num4).y;
			return float4x4.PerspectiveOffCenter(x, x2, y2, y, near, far);
		}
		Debug.LogWarning("Couldn't intersect with near plane raycasting to the mirrors corners?");
		return Matrix4x4.identity;
	}

	// Token: 0x06000743 RID: 1859 RVA: 0x00026811 File Offset: 0x00024A11
	public void Update()
	{
	}

	// Token: 0x06000744 RID: 1860 RVA: 0x00026814 File Offset: 0x00024A14
	private static Matrix4x4 PerspectiveOffCenter(float left, float right, float bottom, float top, float near, float far)
	{
		float num = 2f * near / (right - left);
		float num2 = 2f * near / (top - bottom);
		float num3 = (right + left) / (right - left);
		float num4 = (top + bottom) / (top - bottom);
		float num5 = -(far + near) / (far - near);
		float num6 = -(2f * far * near) / (far - near);
		float num7 = -1f;
		Matrix4x4 matrix4x = default(Matrix4x4);
		matrix4x[0, 0] = num;
		matrix4x[0, 1] = 0f;
		matrix4x[0, 2] = num3;
		matrix4x[0, 3] = 0f;
		matrix4x[1, 0] = 0f;
		matrix4x[1, 1] = num2;
		matrix4x[1, 2] = num4;
		matrix4x[1, 3] = 0f;
		matrix4x[2, 0] = 0f;
		matrix4x[2, 1] = 0f;
		matrix4x[2, 2] = num5;
		matrix4x[2, 3] = num6;
		matrix4x[3, 0] = 0f;
		matrix4x[3, 1] = 0f;
		matrix4x[3, 2] = num7;
		matrix4x[3, 3] = 0f;
		return matrix4x;
	}

	// Token: 0x040006CC RID: 1740
	public Camera mirrorCamera;

	// Token: 0x040006CD RID: 1741
	public Transform mirrorTransform;

	// Token: 0x040006CE RID: 1742
	private Character player;

	// Token: 0x040006CF RID: 1743
	private Camera mainCam;

	// Token: 0x040006D0 RID: 1744
	public RenderTexture renderTexture;

	// Token: 0x040006D1 RID: 1745
	private BoxCollider box;

	// Token: 0x040006D2 RID: 1746
	public bool useCameraTransform;

	// Token: 0x040006D3 RID: 1747
	public bool useCameraRot;

	// Token: 0x040006D4 RID: 1748
	public float offsetScale;

	// Token: 0x040006D5 RID: 1749
	public float mirrorCamDistance;

	// Token: 0x040006D6 RID: 1750
	public float verticalOffset;

	// Token: 0x040006D7 RID: 1751
	public float left = -0.2f;

	// Token: 0x040006D8 RID: 1752
	public float right = 0.2f;

	// Token: 0x040006D9 RID: 1753
	public float top = 0.2f;

	// Token: 0x040006DA RID: 1754
	public float bottom = -0.2f;

	// Token: 0x040006DB RID: 1755
	public bool isInitialized;

	// Token: 0x040006DC RID: 1756
	public float mirrorWidth;

	// Token: 0x040006DD RID: 1757
	public float mirrorHeight;

	// Token: 0x040006DE RID: 1758
	public float nearplaneOffset;

	// Token: 0x040006DF RID: 1759
	private float depth;
}
