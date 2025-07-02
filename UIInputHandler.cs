using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Zorro.ControllerSupport;
using Zorro.Core;

// Token: 0x0200017A RID: 378
[DefaultExecutionOrder(-1000)]
public class UIInputHandler : Singleton<UIInputHandler>
{
	// Token: 0x1700009E RID: 158
	// (get) Token: 0x06000A94 RID: 2708 RVA: 0x00033928 File Offset: 0x00031B28
	// (set) Token: 0x06000A95 RID: 2709 RVA: 0x00033930 File Offset: 0x00031B30
	public Vector2 wheelNavigationVector { get; private set; }

	// Token: 0x06000A96 RID: 2710 RVA: 0x0003393C File Offset: 0x00031B3C
	public void Initialize()
	{
		UIInputHandler.action_confirm = InputSystem.actions.FindAction("UIConfirm", false);
		UIInputHandler.action_cancel = InputSystem.actions.FindAction("UICancel", false);
		UIInputHandler.action_tabLeft = InputSystem.actions.FindAction("UITabLeft", false);
		UIInputHandler.action_tabRight = InputSystem.actions.FindAction("UITabRight", false);
		UIInputHandler.action_navigateWheel = InputSystem.actions.FindAction("NavigateWheel", false);
		InputHandler instance = RetrievableResourceSingleton<InputHandler>.Instance;
		instance.InputSchemeChanged = (Action<InputScheme>)Delegate.Combine(instance.InputSchemeChanged, new Action<InputScheme>(this.OnInputSchemeChanged));
	}

	// Token: 0x06000A97 RID: 2711 RVA: 0x000339D8 File Offset: 0x00031BD8
	public override void OnDestroy()
	{
		base.OnDestroy();
		InputHandler instance = RetrievableResourceSingleton<InputHandler>.Instance;
		instance.InputSchemeChanged = (Action<InputScheme>)Delegate.Remove(instance.InputSchemeChanged, new Action<InputScheme>(this.OnInputSchemeChanged));
	}

	// Token: 0x06000A98 RID: 2712 RVA: 0x00033A06 File Offset: 0x00031C06
	private void Update()
	{
		this.Sample();
		if (InputHandler.GetCurrentUsedInputScheme() == InputScheme.Gamepad)
		{
			this.UpdateGamepadUse();
			return;
		}
		if (InputHandler.GetCurrentUsedInputScheme() == InputScheme.KeyboardMouse)
		{
			this.UpdateMouseUse();
		}
	}

	// Token: 0x06000A99 RID: 2713 RVA: 0x00033A2C File Offset: 0x00031C2C
	private void Sample()
	{
		this.confirmWasPressed = UIInputHandler.action_confirm.WasPressedThisFrame();
		this.cancelWasPressed = UIInputHandler.action_cancel.WasPressedThisFrame();
		this.tabLeftWasPressed = UIInputHandler.action_tabLeft.WasPressedThisFrame();
		this.tabRightWasPressed = UIInputHandler.action_tabRight.WasPressedThisFrame();
		this.wheelNavigationVector = UIInputHandler.action_navigateWheel.ReadValue<Vector2>();
	}

	// Token: 0x06000A9A RID: 2714 RVA: 0x00033A89 File Offset: 0x00031C89
	private void UpdateGamepadUse()
	{
		if (EventSystem.current.currentSelectedGameObject == null)
		{
			EventSystem.current.SetSelectedGameObject(UIInputHandler.previouslySelectedControllerElement);
			return;
		}
		UIInputHandler.previouslySelectedControllerElement = EventSystem.current.currentSelectedGameObject;
	}

	// Token: 0x06000A9B RID: 2715 RVA: 0x00033ABC File Offset: 0x00031CBC
	private void UpdateMouseUse()
	{
	}

	// Token: 0x06000A9C RID: 2716 RVA: 0x00033ABE File Offset: 0x00031CBE
	private void OnInputSchemeChanged(InputScheme scheme)
	{
	}

	// Token: 0x06000A9D RID: 2717 RVA: 0x00033AC0 File Offset: 0x00031CC0
	public static void SetSelectedObject(GameObject obj)
	{
		UIInputHandler.previouslySelectedControllerElement = obj;
		if (InputHandler.GetCurrentUsedInputScheme() == InputScheme.Gamepad)
		{
			EventSystem.current.SetSelectedGameObject(obj);
		}
	}

	// Token: 0x06000A9E RID: 2718 RVA: 0x00033ADB File Offset: 0x00031CDB
	private void Deselect()
	{
		EventSystem.current.SetSelectedGameObject(null);
	}

	// Token: 0x06000A9F RID: 2719 RVA: 0x00033AE8 File Offset: 0x00031CE8
	private void SelectPrevious()
	{
		EventSystem.current.SetSelectedGameObject(UIInputHandler.previouslySelectedControllerElement);
	}

	// Token: 0x04000979 RID: 2425
	public static InputAction action_confirm;

	// Token: 0x0400097A RID: 2426
	public static InputAction action_cancel;

	// Token: 0x0400097B RID: 2427
	public static InputAction action_tabLeft;

	// Token: 0x0400097C RID: 2428
	public static InputAction action_tabRight;

	// Token: 0x0400097D RID: 2429
	public static InputAction action_navigateWheel;

	// Token: 0x0400097E RID: 2430
	public bool confirmWasPressed;

	// Token: 0x0400097F RID: 2431
	public bool cancelWasPressed;

	// Token: 0x04000980 RID: 2432
	public bool tabLeftWasPressed;

	// Token: 0x04000981 RID: 2433
	public bool tabRightWasPressed;

	// Token: 0x04000983 RID: 2435
	internal static GameObject previouslySelectedControllerElement;
}
