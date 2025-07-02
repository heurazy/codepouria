using System;
using UnityEngine;

// Token: 0x0200019F RID: 415
public class CameraOverride_Binoculars : CameraOverride
{
	// Token: 0x06000B65 RID: 2917 RVA: 0x00038395 File Offset: 0x00036595
	private void Start()
	{
		this.lerpedFOV = this.fov;
	}

	// Token: 0x06000B66 RID: 2918 RVA: 0x000383A4 File Offset: 0x000365A4
	private void Update()
	{
		if (Character.localCharacter == null)
		{
			return;
		}
		base.transform.rotation = Quaternion.LookRotation(Character.localCharacter.data.lookDirection);
		this.fov = Mathf.Lerp(this.fov, this.lerpedFOV, Time.deltaTime * 5f);
	}

	// Token: 0x06000B67 RID: 2919 RVA: 0x00038400 File Offset: 0x00036600
	public void AdjustFOV(float value)
	{
		this.lerpedFOV += value;
		this.lerpedFOV = Mathf.Clamp(this.lerpedFOV, this.minFov, this.maxFov);
	}

	// Token: 0x04000A75 RID: 2677
	public float minFov;

	// Token: 0x04000A76 RID: 2678
	public float maxFov;

	// Token: 0x04000A77 RID: 2679
	public float fovChangeRate;

	// Token: 0x04000A78 RID: 2680
	public float lerpedFOV;
}
