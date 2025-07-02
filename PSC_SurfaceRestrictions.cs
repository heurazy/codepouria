using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000248 RID: 584
public class PSC_SurfaceRestrictions : PropSpawnerConstraint
{
	// Token: 0x06000E4D RID: 3661 RVA: 0x00047C64 File Offset: 0x00045E64
	public override bool CheckConstraint(PropSpawner.SpawnData spawnData)
	{
		if ((this.effectedLayers.value & (1 << spawnData.hit.transform.gameObject.layer)) != 0)
		{
			for (int i = 0; i < this.whitelistedTagWords.Count; i++)
			{
				if (spawnData.hit.transform.tag.ToUpper().Contains(this.whitelistedTagWords[i].ToUpper()))
				{
					return true;
				}
			}
			return false;
		}
		return true;
	}

	// Token: 0x04000D5A RID: 3418
	public LayerMask effectedLayers;

	// Token: 0x04000D5B RID: 3419
	public List<string> whitelistedTagWords = new List<string>();
}
