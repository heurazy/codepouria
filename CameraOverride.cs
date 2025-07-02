using System;
using UnityEngine;

// Token: 0x0200019E RID: 414
public class CameraOverride : MonoBehaviour
{
	// Token: 0x06000B63 RID: 2915 RVA: 0x00038375 File Offset: 0x00036575
	public void DoOverride()
	{
		MainCamera.instance.SetCameraOverride(this);
	}

	// Token: 0x04000A74 RID: 2676
	public float fov = 35f;
}
