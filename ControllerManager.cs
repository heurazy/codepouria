using System;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

// Token: 0x02000059 RID: 89
public class ControllerManager
{
	// Token: 0x060003BB RID: 955 RVA: 0x00016871 File Offset: 0x00014A71
	public void Init()
	{
		InputSystem.onDeviceChange += this.OnDeviceChange;
		this.UpdateGamepadUsage();
	}

	// Token: 0x060003BC RID: 956 RVA: 0x0001688A File Offset: 0x00014A8A
	public void Destroy()
	{
		InputSystem.onDeviceChange -= this.OnDeviceChange;
	}

	// Token: 0x060003BD RID: 957 RVA: 0x0001689D File Offset: 0x00014A9D
	private void OnDeviceChange(InputDevice device, InputDeviceChange change)
	{
		this.UpdateGamepadUsage();
	}

	// Token: 0x060003BE RID: 958 RVA: 0x000168A8 File Offset: 0x00014AA8
	private void UpdateGamepadUsage()
	{
		using (ReadOnlyArray<InputDevice>.Enumerator enumerator = InputSystem.devices.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current is Gamepad)
				{
					this.gamepadAttached = true;
					return;
				}
			}
		}
		this.gamepadAttached = false;
	}

	// Token: 0x04000430 RID: 1072
	public bool gamepadAttached;
}
