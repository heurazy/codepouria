using System;

// Token: 0x020000B9 RID: 185
public class Action_LightLantern : ItemAction
{
	// Token: 0x0600061D RID: 1565 RVA: 0x0002164E File Offset: 0x0001F84E
	private void Awake()
	{
		this.lantern = base.GetComponent<Lantern>();
	}

	// Token: 0x0600061E RID: 1566 RVA: 0x0002165C File Offset: 0x0001F85C
	public override void RunAction()
	{
		this.lantern.ToggleLantern();
	}

	// Token: 0x04000603 RID: 1539
	private Lantern lantern;
}
