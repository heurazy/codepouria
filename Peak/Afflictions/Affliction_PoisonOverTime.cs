using System;
using UnityEngine;
using Zorro.Core.Serizalization;

namespace Peak.Afflictions
{
	// Token: 0x020002DC RID: 732
	public class Affliction_PoisonOverTime : Affliction
	{
		// Token: 0x0600121C RID: 4636 RVA: 0x000597C4 File Offset: 0x000579C4
		public override void OnApplied()
		{
			Debug.Log(string.Format("Added poison to character {0} total time: {1} delay: {2} status per second: {3}", new object[]
			{
				this.character.gameObject.name,
				this.totalTime,
				this.delayBeforeEffect,
				this.statusPerSecond
			}));
		}

		// Token: 0x0600121D RID: 4637 RVA: 0x00059823 File Offset: 0x00057A23
		public Affliction_PoisonOverTime(float totalTime, float delay, float statusPerSecond)
			: base(totalTime)
		{
			this.totalTime = totalTime + delay;
			this.delayBeforeEffect = delay;
			this.statusPerSecond = statusPerSecond;
		}

		// Token: 0x0600121E RID: 4638 RVA: 0x00059843 File Offset: 0x00057A43
		public override void Serialize(BinarySerializer serializer)
		{
			serializer.WriteFloat(this.totalTime);
			serializer.WriteFloat(this.delayBeforeEffect);
			serializer.WriteFloat(this.statusPerSecond);
		}

		// Token: 0x0600121F RID: 4639 RVA: 0x00059869 File Offset: 0x00057A69
		public override void Deserialize(BinaryDeserializer serializer)
		{
			this.totalTime = serializer.ReadFloat();
			this.delayBeforeEffect = serializer.ReadFloat();
			this.statusPerSecond = serializer.ReadFloat();
		}

		// Token: 0x06001220 RID: 4640 RVA: 0x0005988F File Offset: 0x00057A8F
		public override void Stack(Affliction incomingAffliction)
		{
			this.totalTime += incomingAffliction.totalTime;
		}

		// Token: 0x06001221 RID: 4641 RVA: 0x000598A4 File Offset: 0x00057AA4
		public Affliction_PoisonOverTime()
		{
		}

		// Token: 0x06001222 RID: 4642 RVA: 0x000598AC File Offset: 0x00057AAC
		public override Affliction.AfflictionType GetAfflictionType()
		{
			return Affliction.AfflictionType.PoisonOverTime;
		}

		// Token: 0x06001223 RID: 4643 RVA: 0x000598AF File Offset: 0x00057AAF
		protected override void UpdateEffect()
		{
			if (this.timeElapsed > this.delayBeforeEffect)
			{
				this.character.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Poison, this.statusPerSecond * Time.deltaTime, false);
			}
		}

		// Token: 0x04001063 RID: 4195
		public float delayBeforeEffect;

		// Token: 0x04001064 RID: 4196
		public float statusPerSecond;
	}
}
