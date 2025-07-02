using System;

// Token: 0x020000C1 RID: 193
public class Action_RestoreHunger : ItemAction
{
	// Token: 0x0600062F RID: 1583 RVA: 0x000219A7 File Offset: 0x0001FBA7
	public override void RunAction()
	{
		base.character.refs.afflictions.SubtractStatus(CharacterAfflictions.STATUSTYPE.Hunger, this.restorationAmount, false);
	}

	// Token: 0x0400060E RID: 1550
	public float restorationAmount;
}
