using System;

// Token: 0x020000B4 RID: 180
public class Action_GiveExtraStamina : ItemAction
{
	// Token: 0x0600060D RID: 1549 RVA: 0x00021402 File Offset: 0x0001F602
	public override void RunAction()
	{
		base.character.AddExtraStamina(this.amount);
	}

	// Token: 0x040005FA RID: 1530
	public float amount;
}
