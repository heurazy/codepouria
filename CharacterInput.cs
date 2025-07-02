using System;
using UnityEngine;
using UnityEngine.InputSystem;

// Token: 0x0200000C RID: 12
public class CharacterInput : MonoBehaviour
{
	// Token: 0x060000F1 RID: 241 RVA: 0x000077F0 File Offset: 0x000059F0
	public void Init()
	{
		CharacterInput.action_pause = InputSystem.actions.FindAction("Pause", false);
		CharacterInput.action_move = InputSystem.actions.FindAction("Move", false);
		CharacterInput.action_look = InputSystem.actions.FindAction("Look", false);
		CharacterInput.action_jump = InputSystem.actions.FindAction("Jump", false);
		CharacterInput.action_sprint = InputSystem.actions.FindAction("Sprint", false);
		CharacterInput.action_sprintToggle = InputSystem.actions.FindAction("SprintToggle", false);
		CharacterInput.action_interact = InputSystem.actions.FindAction("Interact", false);
		CharacterInput.action_drop = InputSystem.actions.FindAction("Drop", false);
		CharacterInput.action_crouch = InputSystem.actions.FindAction("Crouch", false);
		CharacterInput.action_crouchToggle = InputSystem.actions.FindAction("CrouchToggle", false);
		CharacterInput.action_usePrimary = InputSystem.actions.FindAction("UsePrimary", false);
		CharacterInput.action_useSecondary = InputSystem.actions.FindAction("UseSecondary", false);
		CharacterInput.action_scroll = InputSystem.actions.FindAction("Scroll", false);
		CharacterInput.push_to_talk = InputSystem.actions.FindAction("PushToTalk", false);
		CharacterInput.action_emote = InputSystem.actions.FindAction("Emote", false);
		CharacterInput.action_ping = InputSystem.actions.FindAction("Ping", false);
		for (int i = 0; i < CharacterInput.hotbarActions.Length; i++)
		{
			CharacterInput.hotbarActions[i] = InputSystem.actions.FindAction(string.Format("Hotbar{0}", i + 1), false);
		}
		CharacterInput.action_selectSlotForward = InputSystem.actions.FindAction("SelectSlotForward", false);
		CharacterInput.action_selectSlotBackward = InputSystem.actions.FindAction("SelectSlotBackward", false);
		CharacterInput.action_unselectSlot = InputSystem.actions.FindAction("UnselectSlot", false);
		CharacterInput.action_selectBackpack = InputSystem.actions.FindAction("SelectBackpack", false);
		CharacterInput.action_spectateLeft = InputSystem.actions.FindAction("SpectateLeft", false);
		CharacterInput.action_spectateRight = InputSystem.actions.FindAction("SpectateRight", false);
		CharacterInput.action_scrollButtonLeft = InputSystem.actions.FindAction("ScrollButtonLeft", false);
		CharacterInput.action_scrollButtonRight = InputSystem.actions.FindAction("ScrollButtonRight", false);
	}

	// Token: 0x060000F2 RID: 242 RVA: 0x00007A2B File Offset: 0x00005C2B
	public bool SelectSlotWasPressed(int key)
	{
		return this.HotbarKeyWasPressed(key);
	}

	// Token: 0x060000F3 RID: 243 RVA: 0x00007A34 File Offset: 0x00005C34
	public bool SelectSlotIsPressed(int key)
	{
		return this.HotbarKeyIsPressed(key);
	}

	// Token: 0x060000F4 RID: 244 RVA: 0x00007A3D File Offset: 0x00005C3D
	public bool HotbarKeyWasPressed(int key)
	{
		return key >= 0 && key < CharacterInput.hotbarActions.Length && CharacterInput.hotbarActions[key].WasPressedThisFrame();
	}

	// Token: 0x060000F5 RID: 245 RVA: 0x00007A5B File Offset: 0x00005C5B
	public bool HotbarKeyIsPressed(int key)
	{
		return key >= 0 && key < CharacterInput.hotbarActions.Length && CharacterInput.hotbarActions[key].IsPressed();
	}

	// Token: 0x060000F6 RID: 246 RVA: 0x00007A7C File Offset: 0x00005C7C
	internal void Sample(bool playerMovementActive)
	{
		this.ResetInput();
		this.pauseWasPressed = CharacterInput.action_pause.WasPressedThisFrame();
		this.interactWasPressed = CharacterInput.action_interact.WasPressedThisFrame();
		this.interactIsPressed = CharacterInput.action_interact.IsPressed();
		this.interactWasReleased = CharacterInput.action_interact.WasReleasedThisFrame();
		this.emoteIsPressed = CharacterInput.action_emote.IsPressed();
		if (playerMovementActive)
		{
			this.movementInput = CharacterInput.action_move.ReadValue<Vector2>();
			this.sprintWasPressed = CharacterInput.action_sprint.WasPressedThisFrame();
			this.sprintIsPressed = CharacterInput.action_sprint.IsPressed();
			this.sprintToggleIsPressed = CharacterInput.action_sprintToggle.IsPressed();
			this.sprintToggleWasPressed = CharacterInput.action_sprintToggle.WasPressedThisFrame();
			this.jumpWasPressed = CharacterInput.action_jump.WasPressedThisFrame();
			this.jumpIsPressed = CharacterInput.action_jump.IsPressed();
			this.dropWasPressed = CharacterInput.action_drop.WasPressedThisFrame();
			this.dropIsPressed = CharacterInput.action_drop.IsPressed();
			this.dropWasReleased = CharacterInput.action_drop.WasReleasedThisFrame();
			this.lookInput = CharacterInput.action_look.ReadValue<Vector2>();
			this.scrollInput = CharacterInput.action_scroll.ReadValue<float>();
			this.usePrimaryWasPressed = CharacterInput.action_usePrimary.WasPressedThisFrame();
			this.usePrimaryIsPressed = CharacterInput.action_usePrimary.IsPressed();
			this.usePrimaryWasReleased = CharacterInput.action_usePrimary.WasReleasedThisFrame();
			this.useSecondaryWasPressed = CharacterInput.action_useSecondary.WasPressedThisFrame();
			this.useSecondaryIsPressed = CharacterInput.action_useSecondary.IsPressed();
			this.useSecondaryWasReleased = CharacterInput.action_useSecondary.WasReleasedThisFrame();
			this.crouchWasPressed = CharacterInput.action_crouch.WasPressedThisFrame();
			this.crouchIsPressed = CharacterInput.action_crouch.IsPressed();
			this.crouchToggleWasPressed = CharacterInput.action_crouchToggle.WasPressedThisFrame();
			this.scrolledUp = this.scrollInput > 0f;
			this.scrolledDown = this.scrollInput < 0f;
			this.spectateLeftWasPressed = CharacterInput.action_spectateLeft.WasPressedThisFrame();
			this.spectateRightWasPressed = CharacterInput.action_spectateRight.WasPressedThisFrame();
			this.selectBackpackWasPressed = CharacterInput.action_selectBackpack.WasPerformedThisFrame();
			this.scrollButtonLeftWasPressed = CharacterInput.action_scrollButtonLeft.WasPressedThisFrame();
			this.scrollButtonRightWasPressed = CharacterInput.action_scrollButtonRight.WasPressedThisFrame();
			this.pingWasPressed = CharacterInput.action_ping.WasPressedThisFrame();
			this.pushToTalkPressed = CharacterInput.push_to_talk.IsPressed();
			this.unselectSlotWasPressed = CharacterInput.action_unselectSlot.WasPressedThisFrame();
		}
		this.selectSlotForwardWasPressed = CharacterInput.action_selectSlotForward.WasPressedThisFrame();
		this.selectSlotBackwardWasPressed = CharacterInput.action_selectSlotBackward.WasPressedThisFrame();
		this.unselectSlotWasPressed = CharacterInput.action_unselectSlot.WasPressedThisFrame();
	}

	// Token: 0x060000F7 RID: 247 RVA: 0x00007D0B File Offset: 0x00005F0B
	internal void SampleAlways()
	{
	}

	// Token: 0x060000F8 RID: 248 RVA: 0x00007D10 File Offset: 0x00005F10
	internal void ResetInput()
	{
		this.lookInput = Vector2.zero;
		this.movementInput = Vector2.zero;
		this.sprintIsPressed = false;
		this.jumpWasPressed = false;
		this.jumpIsPressed = false;
		this.useSecondaryIsPressed = false;
		this.useSecondaryWasPressed = false;
		this.useSecondaryWasReleased = false;
		this.usePrimaryWasPressed = false;
		this.usePrimaryIsPressed = false;
		this.usePrimaryWasReleased = false;
		this.interactWasPressed = false;
		this.interactIsPressed = false;
		this.interactWasReleased = false;
		this.dropWasPressed = false;
		this.dropIsPressed = false;
		this.dropWasReleased = false;
		this.scrolledUp = false;
		this.scrolledDown = false;
		this.sprintWasPressed = false;
		this.sprintToggleWasPressed = false;
		this.crouchWasPressed = false;
		this.crouchToggleWasPressed = false;
		this.crouchIsPressed = false;
		this.emoteIsPressed = false;
	}

	// Token: 0x040000D7 RID: 215
	public static InputAction action_move;

	// Token: 0x040000D8 RID: 216
	public static InputAction action_look;

	// Token: 0x040000D9 RID: 217
	public static InputAction action_jump;

	// Token: 0x040000DA RID: 218
	public static InputAction action_sprint;

	// Token: 0x040000DB RID: 219
	public static InputAction action_sprintToggle;

	// Token: 0x040000DC RID: 220
	public static InputAction action_interact;

	// Token: 0x040000DD RID: 221
	public static InputAction action_drop;

	// Token: 0x040000DE RID: 222
	public static InputAction action_crouch;

	// Token: 0x040000DF RID: 223
	public static InputAction action_crouchToggle;

	// Token: 0x040000E0 RID: 224
	public static InputAction action_usePrimary;

	// Token: 0x040000E1 RID: 225
	public static InputAction action_useSecondary;

	// Token: 0x040000E2 RID: 226
	public static InputAction action_scroll;

	// Token: 0x040000E3 RID: 227
	public static InputAction action_emote;

	// Token: 0x040000E4 RID: 228
	public static InputAction action_ping;

	// Token: 0x040000E5 RID: 229
	public static InputAction action_pause;

	// Token: 0x040000E6 RID: 230
	public static InputAction action_spectateLeft;

	// Token: 0x040000E7 RID: 231
	public static InputAction action_spectateRight;

	// Token: 0x040000E8 RID: 232
	public static InputAction action_scrollButtonLeft;

	// Token: 0x040000E9 RID: 233
	public static InputAction action_scrollButtonRight;

	// Token: 0x040000EA RID: 234
	public static InputAction action_selectSlotForward;

	// Token: 0x040000EB RID: 235
	public static InputAction action_selectSlotBackward;

	// Token: 0x040000EC RID: 236
	public static InputAction action_unselectSlot;

	// Token: 0x040000ED RID: 237
	public static InputAction action_selectBackpack;

	// Token: 0x040000EE RID: 238
	public static InputAction[] hotbarActions = new InputAction[9];

	// Token: 0x040000EF RID: 239
	public static InputAction push_to_talk;

	// Token: 0x040000F0 RID: 240
	public Vector2 movementInput;

	// Token: 0x040000F1 RID: 241
	public Vector2 lookInput;

	// Token: 0x040000F2 RID: 242
	public float scrollInput;

	// Token: 0x040000F3 RID: 243
	public bool crouchIsPressed;

	// Token: 0x040000F4 RID: 244
	public bool crouchWasPressed;

	// Token: 0x040000F5 RID: 245
	public bool crouchToggleWasPressed;

	// Token: 0x040000F6 RID: 246
	public bool sprintIsPressed;

	// Token: 0x040000F7 RID: 247
	public bool sprintToggleIsPressed;

	// Token: 0x040000F8 RID: 248
	public bool sprintWasPressed;

	// Token: 0x040000F9 RID: 249
	public bool sprintToggleWasPressed;

	// Token: 0x040000FA RID: 250
	public bool pauseWasPressed;

	// Token: 0x040000FB RID: 251
	public bool jumpWasPressed;

	// Token: 0x040000FC RID: 252
	public bool jumpIsPressed;

	// Token: 0x040000FD RID: 253
	public bool interactWasPressed;

	// Token: 0x040000FE RID: 254
	public bool interactIsPressed;

	// Token: 0x040000FF RID: 255
	public bool interactWasReleased;

	// Token: 0x04000100 RID: 256
	public bool dropWasPressed;

	// Token: 0x04000101 RID: 257
	public bool dropIsPressed;

	// Token: 0x04000102 RID: 258
	public bool dropWasReleased;

	// Token: 0x04000103 RID: 259
	public bool usePrimaryWasPressed;

	// Token: 0x04000104 RID: 260
	public bool usePrimaryIsPressed;

	// Token: 0x04000105 RID: 261
	public bool usePrimaryWasReleased;

	// Token: 0x04000106 RID: 262
	public bool useSecondaryWasPressed;

	// Token: 0x04000107 RID: 263
	public bool useSecondaryIsPressed;

	// Token: 0x04000108 RID: 264
	public bool useSecondaryWasReleased;

	// Token: 0x04000109 RID: 265
	public bool pingWasPressed;

	// Token: 0x0400010A RID: 266
	public bool selectSlotForwardWasPressed;

	// Token: 0x0400010B RID: 267
	public bool selectSlotBackwardWasPressed;

	// Token: 0x0400010C RID: 268
	public bool unselectSlotWasPressed;

	// Token: 0x0400010D RID: 269
	public bool selectBackpackWasPressed;

	// Token: 0x0400010E RID: 270
	public bool scrollButtonLeftWasPressed;

	// Token: 0x0400010F RID: 271
	public bool scrollButtonRightWasPressed;

	// Token: 0x04000110 RID: 272
	public bool emoteIsPressed;

	// Token: 0x04000111 RID: 273
	public bool scrolledUp;

	// Token: 0x04000112 RID: 274
	public bool scrolledDown;

	// Token: 0x04000113 RID: 275
	public bool spectateLeftWasPressed;

	// Token: 0x04000114 RID: 276
	public bool spectateRightWasPressed;

	// Token: 0x04000115 RID: 277
	public bool pushToTalkPressed;
}
