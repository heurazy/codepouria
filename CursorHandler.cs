using System;
using UnityEngine;
using Zorro.ControllerSupport;
using Zorro.Core;
using Zorro.Core.CLI;
using Zorro.UI.Modal;

// Token: 0x020001B3 RID: 435
public class CursorHandler : Singleton<CursorHandler>
{
	// Token: 0x06000BF9 RID: 3065 RVA: 0x0003BF28 File Offset: 0x0003A128
	private void Update()
	{
		bool flag = InputHandler.GetCurrentUsedInputScheme() == InputScheme.KeyboardMouse && (DebugUIHandler.IsOpen || (GUIManager.instance != null && (GUIManager.instance.windowShowingCursor || GUIManager.instance.wheelActive)));
		if (!flag && Modal.IsOpen)
		{
			flag = true;
		}
		if (!flag && !this.isMenuScene)
		{
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
			return;
		}
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
	}

	// Token: 0x04000ADB RID: 2779
	public bool isMenuScene;
}
