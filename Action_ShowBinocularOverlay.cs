using System;

// Token: 0x020000C2 RID: 194
public class Action_ShowBinocularOverlay : ItemAction
{
	// Token: 0x06000631 RID: 1585 RVA: 0x000219CE File Offset: 0x0001FBCE
	public override void RunAction()
	{
		GUIManager.instance.EnableBinocularOverlay();
	}
}
