using System;
using System.Collections.Generic;
using UnityEngine;
using Zorro.Core;

// Token: 0x02000148 RID: 328
[CreateAssetMenu(fileName = "AscentData", menuName = "Scriptable Objects/AscentData")]
public class AscentData : SingletonAsset<AscentData>
{
	// Token: 0x04000850 RID: 2128
	public List<AscentData.AscentInstanceData> ascents;

	// Token: 0x02000369 RID: 873
	[Serializable]
	public class AscentInstanceData
	{
		// Token: 0x04001290 RID: 4752
		public string title;

		// Token: 0x04001291 RID: 4753
		public string titleReward;

		// Token: 0x04001292 RID: 4754
		public string description;

		// Token: 0x04001293 RID: 4755
		public Color color;

		// Token: 0x04001294 RID: 4756
		public Sprite sashSprite;
	}
}
