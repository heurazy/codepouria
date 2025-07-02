using System;
using UnityEngine.Serialization;

// Token: 0x020001F8 RID: 504
[Serializable]
public class ItemRarityOverride
{
	// Token: 0x04000C2E RID: 3118
	public Rarity Rarity;

	// Token: 0x04000C2F RID: 3119
	[FormerlySerializedAs("spawnType")]
	public SpawnPool spawnPool;
}
