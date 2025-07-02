using System;
using UnityEngine;
using Zorro.Core.Serizalization;

namespace Peak.Afflictions
{
	// Token: 0x020002E0 RID: 736
	public class Affliction_AdjustStatusOverTime : Affliction
	{
		// Token: 0x0600123B RID: 4667 RVA: 0x00059BDC File Offset: 0x00057DDC
		public Affliction_AdjustStatusOverTime()
		{
		}

		// Token: 0x0600123C RID: 4668 RVA: 0x00059BE4 File Offset: 0x00057DE4
		public Affliction_AdjustStatusOverTime(CharacterAfflictions.STATUSTYPE statusType, float statusPerSecond, float totalTime)
			: base(totalTime)
		{
			this.statusType = statusType;
			this.statusPerSecond = statusPerSecond;
		}

		// Token: 0x0600123D RID: 4669 RVA: 0x00059BFB File Offset: 0x00057DFB
		public override Affliction.AfflictionType GetAfflictionType()
		{
			return Affliction.AfflictionType.AdjustStatusOverTime;
		}

		// Token: 0x0600123E RID: 4670 RVA: 0x00059C00 File Offset: 0x00057E00
		public override void Stack(Affliction incomingAffliction)
		{
			this.totalTime += incomingAffliction.totalTime;
			Affliction_AdjustStatusOverTime affliction_AdjustStatusOverTime = incomingAffliction as Affliction_AdjustStatusOverTime;
			if (affliction_AdjustStatusOverTime != null)
			{
				this.statusPerSecond = Mathf.Max(affliction_AdjustStatusOverTime.statusPerSecond, this.statusPerSecond);
			}
		}

		// Token: 0x0600123F RID: 4671 RVA: 0x00059C41 File Offset: 0x00057E41
		public override void OnApplied()
		{
			if (this.character.IsLocal && this.statusType == CharacterAfflictions.STATUSTYPE.Cold && this.statusPerSecond < 0f)
			{
				GUIManager.instance.StartHeat();
			}
		}

		// Token: 0x06001240 RID: 4672 RVA: 0x00059C70 File Offset: 0x00057E70
		public override void OnRemoved()
		{
			if (this.character.IsLocal && this.statusType == CharacterAfflictions.STATUSTYPE.Cold && this.statusPerSecond < 0f)
			{
				GUIManager.instance.EndHeat();
			}
		}

		// Token: 0x06001241 RID: 4673 RVA: 0x00059CA0 File Offset: 0x00057EA0
		protected override void UpdateEffect()
		{
			if (this.statusPerSecond < 0f)
			{
				this.character.refs.afflictions.SubtractStatus(this.statusType, Mathf.Abs(this.statusPerSecond) * Time.deltaTime, false);
				return;
			}
			if (this.statusPerSecond > 0f)
			{
				this.character.refs.afflictions.AddStatus(this.statusType, this.statusPerSecond * Time.deltaTime, false);
			}
		}

		// Token: 0x06001242 RID: 4674 RVA: 0x00059D1E File Offset: 0x00057F1E
		public override void Serialize(BinarySerializer serializer)
		{
			serializer.WriteInt((int)this.statusType);
			serializer.WriteFloat(this.statusPerSecond);
			serializer.WriteFloat(this.totalTime);
		}

		// Token: 0x06001243 RID: 4675 RVA: 0x00059D44 File Offset: 0x00057F44
		public override void Deserialize(BinaryDeserializer serializer)
		{
			this.statusType = (CharacterAfflictions.STATUSTYPE)serializer.ReadInt();
			this.statusPerSecond = serializer.ReadFloat();
			this.totalTime = serializer.ReadFloat();
		}

		// Token: 0x0400106A RID: 4202
		public CharacterAfflictions.STATUSTYPE statusType;

		// Token: 0x0400106B RID: 4203
		public float statusPerSecond;
	}
}
