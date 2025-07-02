using System;

// Token: 0x020000B2 RID: 178
public class Action_Die : ItemAction
{
	// Token: 0x06000609 RID: 1545 RVA: 0x000213CE File Offset: 0x0001F5CE
	public override void RunAction()
	{
		base.character.Invoke("DieInstantly", 0.02f);
	}
}
