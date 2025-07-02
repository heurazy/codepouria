using System;
using TMPro;
using UnityEngine;
using Zorro.ControllerSupport;
using Zorro.Core;

// Token: 0x0200009D RID: 157
[CreateAssetMenu(fileName = "InputSpriteData", menuName = "Scouts/Input Sprite Data")]
public class InputSpriteData : SingletonAsset<InputSpriteData>
{
	// Token: 0x060005C0 RID: 1472 RVA: 0x0002034C File Offset: 0x0001E54C
	public static string GetSpriteTag(InputSpriteData.InputAction action, InputScheme scheme)
	{
		if (scheme == InputScheme.KeyboardMouse)
		{
			switch (action)
			{
			case InputSpriteData.InputAction.Interact:
			case InputSpriteData.InputAction.HoldInteract:
				return "<sprite=14 tint=1>";
			case InputSpriteData.InputAction.UsePrimary:
				return "<sprite=109 tint=1>";
			case InputSpriteData.InputAction.UseSecondary:
				return "<sprite=110 tint=1>";
			case InputSpriteData.InputAction.Scroll:
				return "<sprite=112 tint=1>";
			case InputSpriteData.InputAction.Throw:
			case InputSpriteData.InputAction.Drop:
				return "<sprite=26 tint=1>";
			case InputSpriteData.InputAction.Slot1:
				return "<sprite=1 tint=1>";
			case InputSpriteData.InputAction.Slot2:
				return "<sprite=2 tint=1>";
			case InputSpriteData.InputAction.Slot3:
				return "<sprite=3 tint=1>";
			case InputSpriteData.InputAction.Slot4:
				return "<sprite=4 tint=1>";
			case InputSpriteData.InputAction.SpectateLeft:
				return "<sprite=10 tint=1>";
			case InputSpriteData.InputAction.SpectateRight:
				return "<sprite=13 tint=1>";
			case InputSpriteData.InputAction.Move:
				return "<sprite=115 tint=1>";
			case InputSpriteData.InputAction.Aim:
				return "<sprite=108 tint=1>";
			case InputSpriteData.InputAction.Sprint:
				return "<sprite=51 tint=1>";
			case InputSpriteData.InputAction.Jump:
				return "<sprite=69 tint=1>";
			case InputSpriteData.InputAction.Crouch:
				return "<sprite=49 tint=1>";
			case InputSpriteData.InputAction.Ping:
				return "<sprite=111 tint=1>";
			case InputSpriteData.InputAction.Emote:
				return "<sprite=27 tint=1>";
			}
		}
		else if (scheme == InputScheme.Gamepad)
		{
			switch (action)
			{
			case InputSpriteData.InputAction.Interact:
			case InputSpriteData.InputAction.HoldInteract:
				return "<sprite=2 tint=1>";
			case InputSpriteData.InputAction.UsePrimary:
				return "<sprite=7 tint=1>";
			case InputSpriteData.InputAction.UseSecondary:
				return "<sprite=6 tint=1>";
			case InputSpriteData.InputAction.Scroll:
				return "<sprite=4 tint=1><sprite=5 tint=1>";
			case InputSpriteData.InputAction.Throw:
			case InputSpriteData.InputAction.Drop:
				return "<sprite=3 tint=1>";
			case InputSpriteData.InputAction.SpectateLeft:
				return "<sprite=14 tint=1>";
			case InputSpriteData.InputAction.SpectateRight:
				return "<sprite=15 tint=1>";
			case InputSpriteData.InputAction.Move:
				return "<sprite=16 tint=1>";
			case InputSpriteData.InputAction.Aim:
				return "<sprite=17 tint=1>";
			case InputSpriteData.InputAction.Sprint:
				return "<sprite=10 tint=1>";
			case InputSpriteData.InputAction.Jump:
				return "<sprite=0 tint=1>";
			case InputSpriteData.InputAction.Crouch:
				return "<sprite=1 tint=1>";
			case InputSpriteData.InputAction.Ping:
				return "<sprite=11 tint=1>";
			case InputSpriteData.InputAction.SlotLeft:
				return "<sprite=14 tint=1>";
			case InputSpriteData.InputAction.SlotRight:
				return "<sprite=15 tint=1>";
			case InputSpriteData.InputAction.DeselectSlot:
				return "<sprite=13 tint=1>";
			case InputSpriteData.InputAction.Emote:
				return "<sprite=12 tint=1>";
			}
		}
		return "";
	}

	// Token: 0x040005C9 RID: 1481
	public TMP_SpriteAsset keyboardSprites;

	// Token: 0x040005CA RID: 1482
	public TMP_SpriteAsset xboxSprites;

	// Token: 0x040005CB RID: 1483
	public TMP_SpriteAsset switchSprites;

	// Token: 0x040005CC RID: 1484
	public TMP_SpriteAsset ps5Sprites;

	// Token: 0x040005CD RID: 1485
	public TMP_SpriteAsset ps4Sprites;

	// Token: 0x02000322 RID: 802
	public enum InputAction
	{
		// Token: 0x04001173 RID: 4467
		Interact,
		// Token: 0x04001174 RID: 4468
		HoldInteract,
		// Token: 0x04001175 RID: 4469
		UsePrimary,
		// Token: 0x04001176 RID: 4470
		UseSecondary,
		// Token: 0x04001177 RID: 4471
		Scroll,
		// Token: 0x04001178 RID: 4472
		Throw,
		// Token: 0x04001179 RID: 4473
		Drop,
		// Token: 0x0400117A RID: 4474
		Slot1,
		// Token: 0x0400117B RID: 4475
		Slot2,
		// Token: 0x0400117C RID: 4476
		Slot3,
		// Token: 0x0400117D RID: 4477
		Slot4,
		// Token: 0x0400117E RID: 4478
		SpectateLeft,
		// Token: 0x0400117F RID: 4479
		SpectateRight,
		// Token: 0x04001180 RID: 4480
		Move,
		// Token: 0x04001181 RID: 4481
		Aim,
		// Token: 0x04001182 RID: 4482
		Sprint,
		// Token: 0x04001183 RID: 4483
		Jump,
		// Token: 0x04001184 RID: 4484
		Crouch,
		// Token: 0x04001185 RID: 4485
		Ping,
		// Token: 0x04001186 RID: 4486
		SlotLeft,
		// Token: 0x04001187 RID: 4487
		SlotRight,
		// Token: 0x04001188 RID: 4488
		DeselectSlot,
		// Token: 0x04001189 RID: 4489
		Emote
	}
}
