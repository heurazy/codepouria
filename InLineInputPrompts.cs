using System;
using TMPro;
using UnityEngine;
using Zorro.ControllerSupport;
using Zorro.Core;

// Token: 0x02000157 RID: 343
[RequireComponent(typeof(TMP_Text))]
public class InLineInputPrompts : MonoBehaviour
{
	// Token: 0x060009CB RID: 2507 RVA: 0x00030C94 File Offset: 0x0002EE94
	private void Awake()
	{
		this.text = base.GetComponent<TMP_Text>();
		this.originalText = this.text.text;
	}

	// Token: 0x060009CC RID: 2508 RVA: 0x00030CB3 File Offset: 0x0002EEB3
	private void OnEnable()
	{
		InputHandler instance = RetrievableResourceSingleton<InputHandler>.Instance;
		instance.InputSchemeChanged = (Action<InputScheme>)Delegate.Combine(instance.InputSchemeChanged, new Action<InputScheme>(this.OnDeviceChange));
		this.OnDeviceChange(InputHandler.GetCurrentUsedInputScheme());
	}

	// Token: 0x060009CD RID: 2509 RVA: 0x00030CE6 File Offset: 0x0002EEE6
	private void OnDisable()
	{
		InputHandler instance = RetrievableResourceSingleton<InputHandler>.Instance;
		instance.InputSchemeChanged = (Action<InputScheme>)Delegate.Remove(instance.InputSchemeChanged, new Action<InputScheme>(this.OnDeviceChange));
	}

	// Token: 0x060009CE RID: 2510 RVA: 0x00030D0E File Offset: 0x0002EF0E
	private void OnDeviceChange(InputScheme scheme)
	{
		this.UpdateText(scheme);
		this.UpdateSprites(scheme);
	}

	// Token: 0x060009CF RID: 2511 RVA: 0x00030D20 File Offset: 0x0002EF20
	private void UpdateText(InputScheme scheme)
	{
		string text = this.originalText;
		if (text.Contains("[") && text.Contains("]"))
		{
			foreach (object obj in Enum.GetValues(typeof(InputSpriteData.InputAction)))
			{
				if (text.Contains(obj.ToString()))
				{
					string spriteTag = InputSpriteData.GetSpriteTag((InputSpriteData.InputAction)obj, scheme);
					if (!string.IsNullOrEmpty(spriteTag))
					{
						string text2 = string.Format("[{0}]", obj);
						text = text.Replace(text2, spriteTag);
					}
				}
			}
		}
		this.text.text = text;
	}

	// Token: 0x060009D0 RID: 2512 RVA: 0x00030DE4 File Offset: 0x0002EFE4
	private void UpdateSprites(InputScheme scheme)
	{
		if (scheme == InputScheme.KeyboardMouse)
		{
			this.text.spriteAsset = SingletonAsset<InputSpriteData>.Instance.keyboardSprites;
			return;
		}
		if (scheme == InputScheme.Gamepad)
		{
			GamepadType gamepadType = InputHandler.GetGamepadType();
			if (gamepadType == GamepadType.Xbox)
			{
				this.text.spriteAsset = SingletonAsset<InputSpriteData>.Instance.xboxSprites;
				return;
			}
			if (gamepadType == GamepadType.Dualshock)
			{
				this.text.spriteAsset = SingletonAsset<InputSpriteData>.Instance.ps4Sprites;
				return;
			}
			if (gamepadType == GamepadType.Dualsense)
			{
				this.text.spriteAsset = SingletonAsset<InputSpriteData>.Instance.ps4Sprites;
				return;
			}
			if (gamepadType == GamepadType.SteamDeck)
			{
				this.text.spriteAsset = SingletonAsset<InputSpriteData>.Instance.xboxSprites;
				return;
			}
			if (gamepadType == GamepadType.Unkown)
			{
				this.text.spriteAsset = SingletonAsset<InputSpriteData>.Instance.xboxSprites;
			}
		}
	}

	// Token: 0x040008BA RID: 2234
	private TMP_Text text;

	// Token: 0x040008BB RID: 2235
	private string originalText;
}
