using System;

// Token: 0x020000B3 RID: 179
public class Action_Flare : ItemAction
{
	// Token: 0x0600060B RID: 1547 RVA: 0x000213ED File Offset: 0x0001F5ED
	public override void RunAction()
	{
		this.flare.LightFlare();
	}

	// Token: 0x040005F9 RID: 1529
	public Flare flare;
}
