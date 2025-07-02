using System;
using TMPro;
using UnityEngine;
using Zorro.ControllerSupport;
using Zorro.Core;

// Token: 0x02000158 RID: 344
public class InputIcon : MonoBehaviour
{
	// Token: 0x060009D2 RID: 2514 RVA: 0x00030E9F File Offset: 0x0002F09F
	private void Awake()
	{
		this.text = base.GetComponent<TMP_Text>();
	}

	// Token: 0x060009D3 RID: 2515 RVA: 0x00030EAD File Offset: 0x0002F0AD
	private void OnEnable()
	{
		InputHandler instance = RetrievableResourceSingleton<InputHandler>.Instance;
		instance.InputSchemeChanged = (Action<InputScheme>)Delegate.Combine(instance.InputSchemeChanged, new Action<InputScheme>(this.OnDeviceChange));
		this.OnDeviceChange(InputHandler.GetCurrentUsedInputScheme());
	}

	// Token: 0x060009D4 RID: 2516 RVA: 0x00030EE0 File Offset: 0x0002F0E0
	private void OnDisable()
	{
		InputHandler instance = RetrievableResourceSingleton<InputHandler>.Instance;
		instance.InputSchemeChanged = (Action<InputScheme>)Delegate.Remove(instance.InputSchemeChanged, new Action<InputScheme>(this.OnDeviceChange));
	}

	// Token: 0x060009D5 RID: 2517 RVA: 0x00030F08 File Offset: 0x0002F108
	private void OnDeviceChange(InputScheme scheme)
	{
		if (scheme == InputScheme.KeyboardMouse)
		{
			this.text.spriteAsset = this.keyboardSprites;
		}
		else if (scheme == InputScheme.Gamepad)
		{
			GamepadType gamepadType = InputHandler.GetGamepadType();
			if (gamepadType == GamepadType.Xbox)
			{
				this.text.spriteAsset = this.xboxSprites;
			}
			else if (gamepadType == GamepadType.Dualshock)
			{
				this.text.spriteAsset = this.ps4Sprites;
			}
			else if (gamepadType == GamepadType.Dualsense)
			{
				this.text.spriteAsset = this.ps4Sprites;
			}
			else if (gamepadType == GamepadType.SteamDeck)
			{
				this.text.spriteAsset = this.xboxSprites;
			}
			else if (gamepadType == GamepadType.Unkown)
			{
				this.text.spriteAsset = this.xboxSprites;
			}
		}
		this.SetText(scheme);
	}

	// Token: 0x060009D6 RID: 2518 RVA: 0x00030FAC File Offset: 0x0002F1AC
	private void SetText(InputScheme scheme)
	{
		if (scheme == InputScheme.Gamepad)
		{
			this.text.enabled = !this.disableIfController;
		}
		else if (scheme == InputScheme.KeyboardMouse)
		{
			this.text.enabled = !this.disableIfKeyboard;
		}
		string spriteTag = InputSpriteData.GetSpriteTag(this.action, scheme);
		if (!string.IsNullOrEmpty(spriteTag))
		{
			this.text.text = spriteTag;
		}
		if (scheme == InputScheme.Gamepad)
		{
			this.hold.SetActive(this.action == InputSpriteData.InputAction.Throw || this.action == InputSpriteData.InputAction.HoldInteract);
		}
	}

	// Token: 0x040008BC RID: 2236
	private TMP_Text text;

	// Token: 0x040008BD RID: 2237
	public GameObject hold;

	// Token: 0x040008BE RID: 2238
	public InputSpriteData.InputAction action;

	// Token: 0x040008BF RID: 2239
	public TMP_SpriteAsset keyboardSprites;

	// Token: 0x040008C0 RID: 2240
	public TMP_SpriteAsset xboxSprites;

	// Token: 0x040008C1 RID: 2241
	public TMP_SpriteAsset switchSprites;

	// Token: 0x040008C2 RID: 2242
	public TMP_SpriteAsset ps5Sprites;

	// Token: 0x040008C3 RID: 2243
	public TMP_SpriteAsset ps4Sprites;

	// Token: 0x040008C4 RID: 2244
	public bool disableIfController;

	// Token: 0x040008C5 RID: 2245
	public bool disableIfKeyboard;
}
