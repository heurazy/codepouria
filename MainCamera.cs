using System;
using UnityEngine;

// Token: 0x020001FD RID: 509
public class MainCamera : MonoBehaviour
{
	// Token: 0x06000D2F RID: 3375 RVA: 0x000428DA File Offset: 0x00040ADA
	private void Awake()
	{
		this.cam = base.GetComponent<Camera>();
		MainCamera.instance = this;
	}

	// Token: 0x06000D30 RID: 3376 RVA: 0x000428EE File Offset: 0x00040AEE
	public void SetCameraOverride(CameraOverride setOverride)
	{
		this.camOverride = setOverride;
		this.sinceOverride = 0;
	}

	// Token: 0x06000D31 RID: 3377 RVA: 0x000428FE File Offset: 0x00040AFE
	private void Update()
	{
		AudioListener.volume = Mathf.Lerp(AudioListener.volume, 1f, 0.1f * Time.deltaTime);
	}

	// Token: 0x06000D32 RID: 3378 RVA: 0x0004291F File Offset: 0x00040B1F
	private void LateUpdate()
	{
		if (Player.localPlayer == null)
		{
			return;
		}
		if (this.sinceOverride > 1)
		{
			this.camOverride = null;
		}
		this.sinceOverride++;
	}

	// Token: 0x04000C54 RID: 3156
	public static MainCamera instance;

	// Token: 0x04000C55 RID: 3157
	internal Camera cam;

	// Token: 0x04000C56 RID: 3158
	internal CameraOverride camOverride;

	// Token: 0x04000C57 RID: 3159
	private int sinceOverride = 10;
}
