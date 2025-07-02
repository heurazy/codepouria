using System;
using Peak.Afflictions;

// Token: 0x020000B7 RID: 183
public class Action_InflictPoison : ItemAction
{
	// Token: 0x06000619 RID: 1561 RVA: 0x000215DE File Offset: 0x0001F7DE
	public override void RunAction()
	{
		base.character.refs.afflictions.AddAffliction(new Affliction_PoisonOverTime(this.inflictionTime, this.delay, this.poisonPerSecond), false);
	}

	// Token: 0x040005FF RID: 1535
	public float inflictionTime;

	// Token: 0x04000600 RID: 1536
	public float poisonPerSecond;

	// Token: 0x04000601 RID: 1537
	public float delay;
}
