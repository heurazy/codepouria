using System;
using UnityEngine;
using Zorro.Core.Serizalization;

namespace Peak.Afflictions
{
	// Token: 0x020002DE RID: 734
	public class Affliction_AdjustStatus : Affliction
	{
		// Token: 0x0600122D RID: 4653 RVA: 0x00059A76 File Offset: 0x00057C76
		public Affliction_AdjustStatus()
		{
		}

		// Token: 0x0600122E RID: 4654 RVA: 0x00059A7E File Offset: 0x00057C7E
		public Affliction_AdjustStatus(CharacterAfflictions.STATUSTYPE statusType, float statusAmount, float totalTime)
			: base(totalTime)
		{
			this.statusType = statusType;
			this.statusAmount = statusAmount;
		}

		// Token: 0x0600122F RID: 4655 RVA: 0x00059A95 File Offset: 0x00057C95
		public override Affliction.AfflictionType GetAfflictionType()
		{
			return Affliction.AfflictionType.AdjustStatus;
		}

		// Token: 0x06001230 RID: 4656 RVA: 0x00059A98 File Offset: 0x00057C98
		public override void Stack(Affliction incomingAffliction)
		{
			this.OnApplied();
		}

		// Token: 0x06001231 RID: 4657 RVA: 0x00059AA0 File Offset: 0x00057CA0
		public override void OnApplied()
		{
			if (this.character.IsLocal)
			{
				this.character.refs.afflictions.AdjustStatus(this.statusType, this.statusAmount, false);
			}
		}

		// Token: 0x06001232 RID: 4658 RVA: 0x00059AD4 File Offset: 0x00057CD4
		public override void Serialize(BinarySerializer serializer)
		{
			Debug.Log("Serializing int");
			serializer.WriteInt((int)this.statusType);
			Debug.Log("Serializing float");
			serializer.WriteFloat(this.statusAmount);
			Debug.Log("Serializing float");
			serializer.WriteFloat(this.totalTime);
		}

		// Token: 0x06001233 RID: 4659 RVA: 0x00059B24 File Offset: 0x00057D24
		public override void Deserialize(BinaryDeserializer serializer)
		{
			Debug.Log("Deserializing int");
			this.statusType = (CharacterAfflictions.STATUSTYPE)serializer.ReadInt();
			Debug.Log("Deserializing float");
			this.statusAmount = serializer.ReadFloat();
			Debug.Log("Deserializing float");
			this.totalTime = serializer.ReadFloat();
		}

		// Token: 0x04001067 RID: 4199
		public CharacterAfflictions.STATUSTYPE statusType;

		// Token: 0x04001068 RID: 4200
		public float statusAmount;
	}
}
