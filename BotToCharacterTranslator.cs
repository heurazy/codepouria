using System;
using UnityEngine;

// Token: 0x0200004D RID: 77
public class BotToCharacterTranslator : MonoBehaviour
{
	// Token: 0x06000375 RID: 885 RVA: 0x00015124 File Offset: 0x00013324
	private void Awake()
	{
		this.character = base.GetComponentInParent<Character>();
		this.bot = base.GetComponentInParent<Bot>();
	}

	// Token: 0x06000376 RID: 886 RVA: 0x00015140 File Offset: 0x00013340
	private void Update()
	{
		this.character.input.movementInput = this.bot.MovementInput;
		this.character.input.sprintIsPressed = this.bot.IsSprinting;
		this.character.data.lookValues = HelperFunctions.DirectionToLook(this.bot.LookDirection);
	}

	// Token: 0x04000400 RID: 1024
	private Character character;

	// Token: 0x04000401 RID: 1025
	private Bot bot;
}
