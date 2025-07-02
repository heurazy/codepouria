using System;

// Token: 0x020000B0 RID: 176
public class Action_Consume : ItemAction
{
	// Token: 0x06000604 RID: 1540 RVA: 0x0002134F File Offset: 0x0001F54F
	public override void RunAction()
	{
		if (base.character)
		{
			this.item.StartCoroutine(this.item.ConsumeDelayed(false));
		}
	}
}
