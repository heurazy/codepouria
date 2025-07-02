using System;
using UnityEngine;
using Zorro.Core.Serizalization;

namespace Peak.Afflictions
{
	// Token: 0x020002DA RID: 730
	public class Affliction_Glowing : Affliction
	{
		// Token: 0x0600120D RID: 4621 RVA: 0x0005935B File Offset: 0x0005755B
		public override Affliction.AfflictionType GetAfflictionType()
		{
			return Affliction.AfflictionType.Glowing;
		}

		// Token: 0x0600120E RID: 4622 RVA: 0x00059360 File Offset: 0x00057560
		public override void OnApplied()
		{
			base.OnApplied();
			Material material = this.character.refs.mainRenderer.materials[0];
			float @float = material.GetFloat("_Glow");
			Debug.Log(string.Format("Appling Glow to character {0}, amount {1}", this.character.gameObject.name, @float));
			material.SetFloat("_Glow", @float + 1f);
			this.pointLightInstance = Object.Instantiate<GameObject>(this.pointLightPref, this.character.GetBodypart(BodypartType.Head).transform);
			this.pointLightInstance.transform.localPosition = Vector3.zero;
		}

		// Token: 0x0600120F RID: 4623 RVA: 0x00059404 File Offset: 0x00057604
		public override void OnRemoved()
		{
			base.OnRemoved();
			Material material = this.character.refs.mainRenderer.materials[0];
			float @float = material.GetFloat("_Glow");
			Debug.Log(string.Format("Removing Glow from character {0}, amount {1}", this.character.gameObject.name, @float));
			material.SetFloat("_Glow", @float - 1f);
			Object.DestroyImmediate(this.pointLightInstance);
		}

		// Token: 0x06001210 RID: 4624 RVA: 0x0005947B File Offset: 0x0005767B
		public override void Stack(Affliction incomingAffliction)
		{
			this.totalTime = Mathf.Max(this.totalTime, incomingAffliction.totalTime);
		}

		// Token: 0x06001211 RID: 4625 RVA: 0x00059494 File Offset: 0x00057694
		public override void Serialize(BinarySerializer serializer)
		{
			serializer.WriteFloat(this.totalTime);
		}

		// Token: 0x06001212 RID: 4626 RVA: 0x000594A2 File Offset: 0x000576A2
		public override void Deserialize(BinaryDeserializer serializer)
		{
			this.totalTime = serializer.ReadFloat();
		}

		// Token: 0x0400105C RID: 4188
		public GameObject pointLightPref;

		// Token: 0x0400105D RID: 4189
		private GameObject pointLightInstance;
	}
}
