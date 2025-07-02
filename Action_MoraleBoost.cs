using System;

// Token: 0x020000BB RID: 187
public class Action_MoraleBoost : ItemAction
{
	// Token: 0x06000622 RID: 1570 RVA: 0x0002182D File Offset: 0x0001FA2D
	public override void RunAction()
	{
		MoraleBoost.SpawnMoraleBoost(base.transform.position, this.boostRadius, this.baselineStaminaBoost, this.staminaBoostPerAdditionalScout, true, 1);
	}

	// Token: 0x04000606 RID: 1542
	public float boostRadius;

	// Token: 0x04000607 RID: 1543
	public float baselineStaminaBoost;

	// Token: 0x04000608 RID: 1544
	public float staminaBoostPerAdditionalScout;
}
