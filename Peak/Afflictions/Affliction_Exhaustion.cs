using System;
using UnityEngine;
using Zorro.Core.Serizalization;

namespace Peak.Afflictions
{
	// Token: 0x020002D9 RID: 729
	public class Affliction_Exhaustion : Affliction
	{
		// Token: 0x06001207 RID: 4615 RVA: 0x000592B6 File Offset: 0x000574B6
		public override Affliction.AfflictionType GetAfflictionType()
		{
			return Affliction.AfflictionType.Exhausted;
		}

		// Token: 0x06001208 RID: 4616 RVA: 0x000592BC File Offset: 0x000574BC
		protected override void UpdateEffect()
		{
			base.UpdateEffect();
			float num = this.drainAmount / this.totalTime * Time.deltaTime;
			this.character.UseStamina(num, true);
			Debug.Log(string.Format("Exhausterd: {0}", num));
		}

		// Token: 0x06001209 RID: 4617 RVA: 0x00059306 File Offset: 0x00057506
		public override void Stack(Affliction incomingAffliction)
		{
			this.totalTime = Mathf.Max(this.timeElapsed, incomingAffliction.totalTime);
		}

		// Token: 0x0600120A RID: 4618 RVA: 0x0005931F File Offset: 0x0005751F
		public override void Serialize(BinarySerializer serializer)
		{
			serializer.WriteFloat(this.totalTime);
			serializer.WriteFloat(this.drainAmount);
		}

		// Token: 0x0600120B RID: 4619 RVA: 0x00059339 File Offset: 0x00057539
		public override void Deserialize(BinaryDeserializer serializer)
		{
			this.totalTime = serializer.ReadFloat();
			this.drainAmount = serializer.ReadFloat();
		}

		// Token: 0x0400105B RID: 4187
		public float drainAmount;
	}
}
