using System;
using UnityEngine;
using Zorro.Core.Serizalization;

namespace Peak.Afflictions
{
	// Token: 0x020002DD RID: 733
	public class Affliction_InfiniteStamina : Affliction
	{
		// Token: 0x06001224 RID: 4644 RVA: 0x000598E3 File Offset: 0x00057AE3
		public Affliction_InfiniteStamina(float totalTime)
			: base(totalTime)
		{
		}

		// Token: 0x06001225 RID: 4645 RVA: 0x000598EC File Offset: 0x00057AEC
		public Affliction_InfiniteStamina()
		{
		}

		// Token: 0x06001226 RID: 4646 RVA: 0x000598F4 File Offset: 0x00057AF4
		public override Affliction.AfflictionType GetAfflictionType()
		{
			return Affliction.AfflictionType.InfiniteStamina;
		}

		// Token: 0x06001227 RID: 4647 RVA: 0x000598F8 File Offset: 0x00057AF8
		public override void Stack(Affliction incomingAffliction)
		{
			Affliction_InfiniteStamina affliction_InfiniteStamina = incomingAffliction as Affliction_InfiniteStamina;
			if (affliction_InfiniteStamina != null)
			{
				this.totalTime = incomingAffliction.totalTime;
				this.timeElapsed = 0f;
				if (this.drowsyAffliction != null)
				{
					this.drowsyAffliction.totalTime += affliction_InfiniteStamina.drowsyAffliction.totalTime;
				}
			}
		}

		// Token: 0x06001228 RID: 4648 RVA: 0x0005994B File Offset: 0x00057B4B
		public override void OnApplied()
		{
			if (this.character.IsLocal)
			{
				GUIManager.instance.StartSugarRush();
			}
		}

		// Token: 0x06001229 RID: 4649 RVA: 0x00059964 File Offset: 0x00057B64
		public override void OnRemoved()
		{
			if (this.character.IsLocal)
			{
				GUIManager.instance.EndSugarRush();
				if (this.drowsyAffliction != null)
				{
					this.character.refs.afflictions.AddAffliction(this.drowsyAffliction, false);
				}
			}
		}

		// Token: 0x0600122A RID: 4650 RVA: 0x000599A4 File Offset: 0x00057BA4
		public override void Serialize(BinarySerializer serializer)
		{
			serializer.WriteFloat(this.totalTime);
			serializer.WriteFloat(this.climbDelay);
			bool flag = this.drowsyAffliction != null;
			serializer.WriteBool(flag);
			if (flag)
			{
				this.drowsyAffliction.Serialize(serializer);
			}
		}

		// Token: 0x0600122B RID: 4651 RVA: 0x000599EC File Offset: 0x00057BEC
		public override void Deserialize(BinaryDeserializer serializer)
		{
			this.totalTime = serializer.ReadFloat();
			this.climbDelay = serializer.ReadFloat();
			this.bonusTime = this.climbDelay;
			if (serializer.ReadBool())
			{
				this.drowsyAffliction = new Affliction_AdjustStatusOverTime();
				this.drowsyAffliction.Deserialize(serializer);
			}
		}

		// Token: 0x0600122C RID: 4652 RVA: 0x00059A3C File Offset: 0x00057C3C
		protected override void UpdateEffect()
		{
			this.character.AddStamina(1f);
			if (this.character.data.isClimbing)
			{
				this.climbDelay = 0f;
				this.bonusTime = 0f;
			}
		}

		// Token: 0x04001065 RID: 4197
		[SerializeReference]
		public Affliction drowsyAffliction;

		// Token: 0x04001066 RID: 4198
		public float climbDelay;
	}
}
