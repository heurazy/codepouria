using System;

// Token: 0x020000BC RID: 188
public class Action_OverrideCamera : ItemAction
{
	// Token: 0x06000624 RID: 1572 RVA: 0x0002185C File Offset: 0x0001FA5C
	public override void RunAction()
	{
		MainCamera.instance.SetCameraOverride(this.cameraOverride);
	}

	// Token: 0x04000609 RID: 1545
	public CameraOverride cameraOverride;
}
