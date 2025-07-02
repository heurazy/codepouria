using System;
using UnityEngine;
using Zorro.ControllerSupport;
using Zorro.Core;

// Token: 0x02000177 RID: 375
public class UIWheel : MonoBehaviour
{
	// Token: 0x06000A89 RID: 2697 RVA: 0x000337BB File Offset: 0x000319BB
	protected virtual Vector2 GetCursorOrigin()
	{
		return new Vector2(base.transform.position.x, base.transform.position.y);
	}

	// Token: 0x06000A8A RID: 2698 RVA: 0x000337E2 File Offset: 0x000319E2
	protected virtual void Update()
	{
		if (InputHandler.GetCurrentUsedInputScheme() == InputScheme.Gamepad)
		{
			this.TestGamepadInput();
		}
	}

	// Token: 0x06000A8B RID: 2699 RVA: 0x000337F4 File Offset: 0x000319F4
	protected void TestGamepadInput()
	{
		Vector2 wheelNavigationVector = Singleton<UIInputHandler>.Instance.wheelNavigationVector;
		this.TestSelectSliceGamepad(wheelNavigationVector);
	}

	// Token: 0x06000A8C RID: 2700 RVA: 0x00033813 File Offset: 0x00031A13
	protected virtual void TestSelectSliceGamepad(Vector2 gamepadVector)
	{
	}

	// Token: 0x0400096F RID: 2415
	public float maxCursorDistance;
}
