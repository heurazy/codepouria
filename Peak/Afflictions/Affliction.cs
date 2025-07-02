using System;
using Unity.Collections;
using UnityEngine;
using Zorro.Core.Serizalization;

namespace Peak.Afflictions
{
	// Token: 0x020002D8 RID: 728
	[Serializable]
	public abstract class Affliction
	{
		// Token: 0x060011FB RID: 4603 RVA: 0x00059181 File Offset: 0x00057381
		public Affliction()
		{
		}

		// Token: 0x060011FC RID: 4604 RVA: 0x00059189 File Offset: 0x00057389
		public Affliction(float totalTime)
		{
			this.totalTime = totalTime;
		}

		// Token: 0x060011FD RID: 4605 RVA: 0x00059198 File Offset: 0x00057398
		public static Affliction CreateBlankAffliction(Affliction.AfflictionType afflictionType)
		{
			switch (afflictionType)
			{
			case Affliction.AfflictionType.PoisonOverTime:
				return new Affliction_PoisonOverTime();
			case Affliction.AfflictionType.InfiniteStamina:
				return new Affliction_InfiniteStamina();
			case Affliction.AfflictionType.FasterBoi:
				return new Affliction_FasterBoi();
			case Affliction.AfflictionType.Exhausted:
				return new Affliction_Exhaustion();
			case Affliction.AfflictionType.Glowing:
				return new Affliction_Glowing();
			case Affliction.AfflictionType.AdjustStatusOverTime:
				return new Affliction_AdjustStatusOverTime();
			case Affliction.AfflictionType.Chaos:
				return new Affliction_Chaos();
			case Affliction.AfflictionType.AdjustStatus:
				return new Affliction_AdjustStatus();
			case Affliction.AfflictionType.ClearAllStatus:
				return new Affliction_ClearAllStatus();
			case Affliction.AfflictionType.PreventPoisonHealing:
				return new Affliction_PreventPoisonHealing();
			default:
				return null;
			}
		}

		// Token: 0x060011FE RID: 4606
		public abstract Affliction.AfflictionType GetAfflictionType();

		// Token: 0x060011FF RID: 4607 RVA: 0x00059212 File Offset: 0x00057412
		public virtual void OnApplied()
		{
		}

		// Token: 0x06001200 RID: 4608 RVA: 0x00059214 File Offset: 0x00057414
		public virtual void OnRemoved()
		{
		}

		// Token: 0x06001201 RID: 4609
		public abstract void Stack(Affliction incomingAffliction);

		// Token: 0x06001202 RID: 4610 RVA: 0x00059218 File Offset: 0x00057418
		public virtual bool Tick()
		{
			if (this.bonusTime > 0f)
			{
				this.bonusTime -= Time.deltaTime;
			}
			else
			{
				this.timeElapsed += Time.deltaTime;
			}
			if (this.timeElapsed >= this.totalTime)
			{
				return true;
			}
			this.UpdateEffect();
			return false;
		}

		// Token: 0x06001203 RID: 4611 RVA: 0x0005926F File Offset: 0x0005746F
		protected virtual void UpdateEffect()
		{
		}

		// Token: 0x06001204 RID: 4612
		public abstract void Serialize(BinarySerializer serializer);

		// Token: 0x06001205 RID: 4613
		public abstract void Deserialize(BinaryDeserializer serializer);

		// Token: 0x06001206 RID: 4614 RVA: 0x00059274 File Offset: 0x00057474
		public Affliction Copy()
		{
			BinarySerializer binarySerializer = new BinarySerializer(100, Allocator.Temp);
			Affliction affliction = Affliction.CreateBlankAffliction(this.GetAfflictionType());
			this.Serialize(binarySerializer);
			BinaryDeserializer binaryDeserializer = new BinaryDeserializer(binarySerializer);
			affliction.Deserialize(binaryDeserializer);
			binarySerializer.Dispose();
			binaryDeserializer.Dispose();
			return affliction;
		}

		// Token: 0x04001057 RID: 4183
		public float timeElapsed;

		// Token: 0x04001058 RID: 4184
		public float totalTime;

		// Token: 0x04001059 RID: 4185
		protected float bonusTime;

		// Token: 0x0400105A RID: 4186
		[HideInInspector]
		public Character character;

		// Token: 0x020003D7 RID: 983
		public enum AfflictionType
		{
			// Token: 0x04001412 RID: 5138
			PoisonOverTime,
			// Token: 0x04001413 RID: 5139
			InfiniteStamina,
			// Token: 0x04001414 RID: 5140
			FasterBoi,
			// Token: 0x04001415 RID: 5141
			Exhausted,
			// Token: 0x04001416 RID: 5142
			Glowing,
			// Token: 0x04001417 RID: 5143
			AdjustStatusOverTime,
			// Token: 0x04001418 RID: 5144
			Chaos,
			// Token: 0x04001419 RID: 5145
			AdjustStatus,
			// Token: 0x0400141A RID: 5146
			ClearAllStatus,
			// Token: 0x0400141B RID: 5147
			PreventPoisonHealing
		}
	}
}
