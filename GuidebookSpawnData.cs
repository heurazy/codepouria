using System;
using UnityEngine;
using Zorro.Core;

// Token: 0x02000016 RID: 22
public class GuidebookSpawnData : MonoBehaviour
{
	// Token: 0x0600019A RID: 410 RVA: 0x0000D0AC File Offset: 0x0000B2AC
	public bool CanSpawnRightNow()
	{
		int maxProgressPointReached = Singleton<MountainProgressHandler>.Instance.maxProgressPointReached;
		if (maxProgressPointReached == 0)
		{
			return this.canSpawnInShore;
		}
		if (maxProgressPointReached == 1)
		{
			return this.canSpawnInTropics;
		}
		if (maxProgressPointReached == 2)
		{
			return this.canSpawnInAlpine;
		}
		return this.canSpawnInCaldera;
	}

	// Token: 0x040001A7 RID: 423
	public bool canSpawnInShore;

	// Token: 0x040001A8 RID: 424
	public bool canSpawnInTropics;

	// Token: 0x040001A9 RID: 425
	public bool canSpawnInAlpine;

	// Token: 0x040001AA RID: 426
	public bool canSpawnInCaldera;
}
