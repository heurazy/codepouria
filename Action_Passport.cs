using System;

// Token: 0x020000BD RID: 189
public class Action_Passport : ItemAction
{
	// Token: 0x06000626 RID: 1574 RVA: 0x00021876 File Offset: 0x0001FA76
	public override void RunAction()
	{
		PassportManager.instance.ToggleOpen();
	}
}
