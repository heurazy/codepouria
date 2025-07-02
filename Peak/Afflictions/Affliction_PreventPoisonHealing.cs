using System;
using Zorro.Core.Serizalization;

namespace Peak.Afflictions
{
	// Token: 0x020002E2 RID: 738
	public class Affliction_PreventPoisonHealing : Affliction
	{
		// Token: 0x0600124B RID: 4683 RVA: 0x00059FA8 File Offset: 0x000581A8
		public Affliction_PreventPoisonHealing()
		{
		}

		// Token: 0x0600124C RID: 4684 RVA: 0x00059FB0 File Offset: 0x000581B0
		public Affliction_PreventPoisonHealing(float totalTime)
			: base(totalTime)
		{
		}

		// Token: 0x0600124D RID: 4685 RVA: 0x00059FB9 File Offset: 0x000581B9
		public override Affliction.AfflictionType GetAfflictionType()
		{
			return Affliction.AfflictionType.PreventPoisonHealing;
		}

		// Token: 0x0600124E RID: 4686 RVA: 0x00059FBD File Offset: 0x000581BD
		public override void Serialize(BinarySerializer serializer)
		{
			serializer.WriteFloat(this.totalTime);
		}

		// Token: 0x0600124F RID: 4687 RVA: 0x00059FCB File Offset: 0x000581CB
		public override void Deserialize(BinaryDeserializer serializer)
		{
			this.totalTime = serializer.ReadFloat();
		}

		// Token: 0x06001250 RID: 4688 RVA: 0x00059FD9 File Offset: 0x000581D9
		public override void Stack(Affliction incomingAffliction)
		{
			this.totalTime = incomingAffliction.totalTime;
		}
	}
}
