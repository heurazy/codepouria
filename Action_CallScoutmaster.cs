using System;

// Token: 0x020000AE RID: 174
public class Action_CallScoutmaster : ItemAction
{
	// Token: 0x06000600 RID: 1536 RVA: 0x00021298 File Offset: 0x0001F498
	public override void RunAction()
	{
		Scoutmaster scoutmaster;
		if (Scoutmaster.GetPrimaryScoutmaster(out scoutmaster))
		{
			scoutmaster.SetCurrentTarget(this.item.holderCharacter, this.forcedChaseTime);
		}
	}

	// Token: 0x040005F5 RID: 1525
	public float forcedChaseTime;
}
