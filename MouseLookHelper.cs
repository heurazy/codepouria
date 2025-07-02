using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

// Token: 0x02000215 RID: 533
[Serializable]
public class MouseLookHelper
{
	// Token: 0x06000DBC RID: 3516 RVA: 0x000453B3 File Offset: 0x000435B3
	public void Init(Transform character, Transform camera)
	{
		this.m_CharacterTargetRot = character.localRotation;
		this.m_CameraTargetRot = camera.localRotation;
	}

	// Token: 0x06000DBD RID: 3517 RVA: 0x000453D0 File Offset: 0x000435D0
	public void LookRotation(Transform character, Transform camera)
	{
		float num = CrossPlatformInputManager.GetAxis("Mouse X") * this.XSensitivity;
		float num2 = CrossPlatformInputManager.GetAxis("Mouse Y") * this.YSensitivity;
		this.m_CharacterTargetRot *= Quaternion.Euler(0f, num, 0f);
		this.m_CameraTargetRot *= Quaternion.Euler(-num2, 0f, 0f);
		if (this.clampVerticalRotation)
		{
			this.m_CameraTargetRot = this.ClampRotationAroundXAxis(this.m_CameraTargetRot);
		}
		if (this.smooth)
		{
			character.localRotation = Quaternion.Slerp(character.localRotation, this.m_CharacterTargetRot, this.smoothTime * Time.deltaTime);
			camera.localRotation = Quaternion.Slerp(camera.localRotation, this.m_CameraTargetRot, this.smoothTime * Time.deltaTime);
			return;
		}
		character.localRotation = this.m_CharacterTargetRot;
		camera.localRotation = this.m_CameraTargetRot;
	}

	// Token: 0x06000DBE RID: 3518 RVA: 0x000454C8 File Offset: 0x000436C8
	private Quaternion ClampRotationAroundXAxis(Quaternion q)
	{
		q.x /= q.w;
		q.y /= q.w;
		q.z /= q.w;
		q.w = 1f;
		float num = 114.59156f * Mathf.Atan(q.x);
		num = Mathf.Clamp(num, this.MinimumX, this.MaximumX);
		q.x = Mathf.Tan(0.008726646f * num);
		return q;
	}

	// Token: 0x04000CCB RID: 3275
	public float XSensitivity = 2f;

	// Token: 0x04000CCC RID: 3276
	public float YSensitivity = 2f;

	// Token: 0x04000CCD RID: 3277
	public bool clampVerticalRotation = true;

	// Token: 0x04000CCE RID: 3278
	public float MinimumX = -90f;

	// Token: 0x04000CCF RID: 3279
	public float MaximumX = 90f;

	// Token: 0x04000CD0 RID: 3280
	public bool smooth;

	// Token: 0x04000CD1 RID: 3281
	public float smoothTime = 5f;

	// Token: 0x04000CD2 RID: 3282
	private Quaternion m_CharacterTargetRot;

	// Token: 0x04000CD3 RID: 3283
	private Quaternion m_CameraTargetRot;
}
