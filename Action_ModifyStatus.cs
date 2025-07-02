using System;
using UnityEngine;
using Zorro.Core;

// Token: 0x020000BA RID: 186
public class Action_ModifyStatus : ItemAction
{
	// Token: 0x06000620 RID: 1568 RVA: 0x00021674 File Offset: 0x0001F874
	public override void RunAction()
	{
		bool passedOut = base.character.data.passedOut;
		if (this.changeAmount < 0f)
		{
			if (this.statusType == CharacterAfflictions.STATUSTYPE.Poison)
			{
				base.character.refs.afflictions.ClearPoisonAfflictions();
				int num = Mathf.RoundToInt(Mathf.Min(base.character.refs.afflictions.GetCurrentStatus(CharacterAfflictions.STATUSTYPE.Poison), Mathf.Abs(this.changeAmount)) * 100f);
				Character character;
				if (this.item.TryGetFeeder(out character))
				{
					GameUtils.instance.IncrementFriendPoisonHealing(num, character.photonView.Owner);
				}
				else
				{
					Singleton<AchievementManager>.Instance.IncrementSteamStat(STEAMSTATTYPE.PoisonHealed, num);
				}
			}
			Character character2;
			if (this.statusType == CharacterAfflictions.STATUSTYPE.Injury && this.item.TryGetFeeder(out character2))
			{
				int num2 = Mathf.RoundToInt(Mathf.Min(base.character.refs.afflictions.GetCurrentStatus(CharacterAfflictions.STATUSTYPE.Injury), Mathf.Abs(this.changeAmount)) * 100f);
				GameUtils.instance.IncrementFriendHealing(num2, character2.photonView.Owner);
			}
			base.character.refs.afflictions.SubtractStatus(this.statusType, Mathf.Abs(this.changeAmount), false);
		}
		else
		{
			base.character.refs.afflictions.AddStatus(this.statusType, Mathf.Abs(this.changeAmount), false);
		}
		float statusSum = base.character.refs.afflictions.statusSum;
		if (passedOut && statusSum <= 1f)
		{
			Debug.Log("LIFE WAS SAVED");
			Character character3;
			if (this.item.TryGetFeeder(out character3))
			{
				GameUtils.instance.ThrowEmergencyPreparednessAchievement(character3.photonView.Owner);
			}
		}
	}

	// Token: 0x04000604 RID: 1540
	public CharacterAfflictions.STATUSTYPE statusType;

	// Token: 0x04000605 RID: 1541
	public float changeAmount;
}
