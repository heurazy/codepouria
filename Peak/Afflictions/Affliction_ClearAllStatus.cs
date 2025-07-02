using System;
using Zorro.Core.Serizalization;

namespace Peak.Afflictions
{
	// Token: 0x020002DF RID: 735
	public class Affliction_ClearAllStatus : Affliction
	{
		// Token: 0x06001234 RID: 4660 RVA: 0x00059B73 File Offset: 0x00057D73
		public Affliction_ClearAllStatus()
		{
		}

		// Token: 0x06001235 RID: 4661 RVA: 0x00059B7B File Offset: 0x00057D7B
		public Affliction_ClearAllStatus(bool excludeCurse, float totalTime)
			: base(totalTime)
		{
			this.excludeCurse = excludeCurse;
		}

		// Token: 0x06001236 RID: 4662 RVA: 0x00059B8B File Offset: 0x00057D8B
		public override Affliction.AfflictionType GetAfflictionType()
		{
			return Affliction.AfflictionType.ClearAllStatus;
		}

		// Token: 0x06001237 RID: 4663 RVA: 0x00059B8E File Offset: 0x00057D8E
		public override void Stack(Affliction incomingAffliction)
		{
			this.OnApplied();
		}

		// Token: 0x06001238 RID: 4664 RVA: 0x00059B96 File Offset: 0x00057D96
		public override void OnApplied()
		{
			if (this.character.IsLocal)
			{
				this.character.refs.afflictions.ClearAllStatus(this.excludeCurse);
			}
		}

		// Token: 0x06001239 RID: 4665 RVA: 0x00059BC0 File Offset: 0x00057DC0
		public override void Serialize(BinarySerializer serializer)
		{
			serializer.WriteBool(this.excludeCurse);
		}

		// Token: 0x0600123A RID: 4666 RVA: 0x00059BCE File Offset: 0x00057DCE
		public override void Deserialize(BinaryDeserializer serializer)
		{
			this.excludeCurse = serializer.ReadBool();
		}

		// Token: 0x04001069 RID: 4201
		public bool excludeCurse;
	}
}
