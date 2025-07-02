using System;
using System.Collections.Generic;
using UnityEngine;
using Zorro.Core.Serizalization;

namespace Peak.Afflictions
{
	// Token: 0x020002E1 RID: 737
	public class Affliction_Chaos : Affliction
	{
		// Token: 0x06001244 RID: 4676 RVA: 0x00059D6A File Offset: 0x00057F6A
		public Affliction_Chaos()
		{
		}

		// Token: 0x06001245 RID: 4677 RVA: 0x00059D72 File Offset: 0x00057F72
		public Affliction_Chaos(float statusAmountAverage, float statusAmountStandardDeviation, float averageBonusStamina, float standardDeviationBonusStamina)
		{
			this.statusAmountAverage = statusAmountAverage;
			this.statusAmountStandardDeviation = statusAmountStandardDeviation;
			this.averageBonusStamina = averageBonusStamina;
			this.standardDeviationBonusStamina = standardDeviationBonusStamina;
		}

		// Token: 0x06001246 RID: 4678 RVA: 0x00059D98 File Offset: 0x00057F98
		public override void OnApplied()
		{
			if (this.character.IsLocal)
			{
				List<CharacterAfflictions.STATUSTYPE> list = new List<CharacterAfflictions.STATUSTYPE>
				{
					CharacterAfflictions.STATUSTYPE.Cold,
					CharacterAfflictions.STATUSTYPE.Hot,
					CharacterAfflictions.STATUSTYPE.Poison,
					CharacterAfflictions.STATUSTYPE.Drowsy,
					CharacterAfflictions.STATUSTYPE.Injury,
					CharacterAfflictions.STATUSTYPE.Hunger
				};
				this.character.refs.afflictions.ClearAllStatus(false);
				float num = Mathf.Clamp(Util.GenerateNormalDistribution(this.statusAmountAverage, this.statusAmountStandardDeviation), 0f, 1f);
				Debug.Log(string.Format("total status: {0}", num));
				float num2 = num;
				while (num2 > 0.05f && list.Count != 0)
				{
					float num3;
					if (list.Count == 1)
					{
						num3 = num2;
					}
					else
					{
						num3 = num * Util.GenerateNormalDistribution(0.3f, 0.5f);
					}
					Debug.Log(string.Format("Next status: {0}", num3));
					num3 = Mathf.Min(num3, num2);
					if (num3 >= 0.025f)
					{
						int num4 = Random.Range(0, list.Count);
						CharacterAfflictions.STATUSTYPE statustype = list[num4];
						this.character.refs.afflictions.AddStatus(statustype, num3, false);
						list.RemoveAt(num4);
						if (statustype == CharacterAfflictions.STATUSTYPE.Hot)
						{
							list.Remove(CharacterAfflictions.STATUSTYPE.Cold);
						}
						else if (statustype == CharacterAfflictions.STATUSTYPE.Cold)
						{
							list.Remove(CharacterAfflictions.STATUSTYPE.Hot);
						}
						num2 -= num3;
					}
				}
				float num5 = Mathf.Clamp(Util.GenerateNormalDistribution(this.averageBonusStamina, this.standardDeviationBonusStamina), 0f, 1f);
				this.character.SetExtraStamina(num5);
				this.character.refs.afflictions.RemoveAffliction(this, false);
			}
		}

		// Token: 0x06001247 RID: 4679 RVA: 0x00059F3F File Offset: 0x0005813F
		public override Affliction.AfflictionType GetAfflictionType()
		{
			return Affliction.AfflictionType.Chaos;
		}

		// Token: 0x06001248 RID: 4680 RVA: 0x00059F42 File Offset: 0x00058142
		public override void Stack(Affliction incomingAffliction)
		{
		}

		// Token: 0x06001249 RID: 4681 RVA: 0x00059F44 File Offset: 0x00058144
		public override void Serialize(BinarySerializer serializer)
		{
			serializer.WriteFloat(this.statusAmountAverage);
			serializer.WriteFloat(this.statusAmountStandardDeviation);
			serializer.WriteFloat(this.averageBonusStamina);
			serializer.WriteFloat(this.standardDeviationBonusStamina);
		}

		// Token: 0x0600124A RID: 4682 RVA: 0x00059F76 File Offset: 0x00058176
		public override void Deserialize(BinaryDeserializer serializer)
		{
			this.statusAmountAverage = serializer.ReadFloat();
			this.statusAmountStandardDeviation = serializer.ReadFloat();
			this.averageBonusStamina = serializer.ReadFloat();
			this.standardDeviationBonusStamina = serializer.ReadFloat();
		}

		// Token: 0x0400106C RID: 4204
		public float statusAmountAverage;

		// Token: 0x0400106D RID: 4205
		public float statusAmountStandardDeviation;

		// Token: 0x0400106E RID: 4206
		public float averageBonusStamina;

		// Token: 0x0400106F RID: 4207
		public float standardDeviationBonusStamina;
	}
}
