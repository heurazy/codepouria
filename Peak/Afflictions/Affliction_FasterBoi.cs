using System;
using UnityEngine;
using Zorro.Core.Serizalization;

namespace Peak.Afflictions
{
	// Token: 0x020002DB RID: 731
	public class Affliction_FasterBoi : Affliction
	{
		// Token: 0x06001214 RID: 4628 RVA: 0x000594B8 File Offset: 0x000576B8
		public override Affliction.AfflictionType GetAfflictionType()
		{
			return Affliction.AfflictionType.FasterBoi;
		}

		// Token: 0x06001215 RID: 4629 RVA: 0x000594BB File Offset: 0x000576BB
		public override void Stack(Affliction incomingAffliction)
		{
			this.totalTime = Mathf.Max(this.totalTime, incomingAffliction.totalTime);
		}

		// Token: 0x06001216 RID: 4630 RVA: 0x000594D4 File Offset: 0x000576D4
		protected override void UpdateEffect()
		{
			if (this.character.data.isClimbing)
			{
				this.climbDelay = 0f;
				this.bonusTime = 0f;
			}
		}

		// Token: 0x06001217 RID: 4631 RVA: 0x00059500 File Offset: 0x00057700
		public override void OnApplied()
		{
			base.OnApplied();
			this.character.refs.movement.movementModifier += this.moveSpeedMod;
			this.character.refs.climbing.climbSpeedMod += this.climbSpeedMod;
			this.character.refs.ropeHandling.climbSpeedMod += this.climbSpeedMod;
			this.character.refs.vineClimbing.climbSpeedMod += this.climbSpeedMod;
			if (this.character.IsLocal)
			{
				GUIManager.instance.StartEnergyDrink();
			}
			this.cachedDrowsy = this.character.refs.afflictions.GetCurrentStatus(CharacterAfflictions.STATUSTYPE.Drowsy);
			this.character.refs.afflictions.SubtractStatus(CharacterAfflictions.STATUSTYPE.Drowsy, 2f, false);
			this.bonusTime = this.climbDelay;
		}

		// Token: 0x06001218 RID: 4632 RVA: 0x000595F8 File Offset: 0x000577F8
		public override void OnRemoved()
		{
			base.OnRemoved();
			this.character.refs.movement.movementModifier -= this.moveSpeedMod;
			this.character.refs.climbing.climbSpeedMod -= this.climbSpeedMod;
			this.character.refs.ropeHandling.climbSpeedMod -= this.climbSpeedMod;
			this.character.refs.vineClimbing.climbSpeedMod -= this.climbSpeedMod;
			this.character.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Drowsy, this.cachedDrowsy + this.drowsyOnEnd, false);
			if (this.character.IsLocal)
			{
				GUIManager.instance.EndEnergyDrink();
			}
		}

		// Token: 0x06001219 RID: 4633 RVA: 0x000596D0 File Offset: 0x000578D0
		public override void Serialize(BinarySerializer serializer)
		{
			serializer.WriteFloat(this.totalTime);
			serializer.WriteFloat(this.moveSpeedMod);
			serializer.WriteFloat(this.climbSpeedMod);
			serializer.WriteFloat(this.drowsyOnEnd);
			serializer.WriteFloat(this.cachedDrowsy);
			serializer.WriteFloat(this.climbDelay);
		}

		// Token: 0x0600121A RID: 4634 RVA: 0x00059728 File Offset: 0x00057928
		public override void Deserialize(BinaryDeserializer serializer)
		{
			this.totalTime = serializer.ReadFloat();
			this.moveSpeedMod = serializer.ReadFloat();
			this.climbSpeedMod = serializer.ReadFloat();
			this.drowsyOnEnd = serializer.ReadFloat();
			this.cachedDrowsy = serializer.ReadFloat();
			this.climbDelay = serializer.ReadFloat();
			this.bonusTime = this.climbDelay;
			Debug.Log("Bonus time set to " + this.bonusTime.ToString());
		}

		// Token: 0x0400105E RID: 4190
		public float moveSpeedMod = 1f;

		// Token: 0x0400105F RID: 4191
		public float climbSpeedMod = 1f;

		// Token: 0x04001060 RID: 4192
		public float drowsyOnEnd;

		// Token: 0x04001061 RID: 4193
		private float cachedDrowsy;

		// Token: 0x04001062 RID: 4194
		public float climbDelay;
	}
}
