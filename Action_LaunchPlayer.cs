using System;

// Token: 0x020000B8 RID: 184
public class Action_LaunchPlayer : ItemAction
{
	// Token: 0x0600061B RID: 1563 RVA: 0x00021615 File Offset: 0x0001F815
	public override void RunAction()
	{
		base.character.AddForce(MainCamera.instance.transform.forward * this.force, 1f, 1f);
	}

	// Token: 0x04000602 RID: 1538
	public float force;
}
