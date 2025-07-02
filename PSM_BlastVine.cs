using System;
using UnityEngine;

// Token: 0x02000239 RID: 569
public class PSM_BlastVine : PropSpawnerMod
{
	// Token: 0x06000E30 RID: 3632 RVA: 0x0004772B File Offset: 0x0004592B
	public override void ModifyObject(GameObject spawned, PropSpawner.SpawnData spawnData)
	{
		spawned.GetComponent<VinePlane>().Blast();
	}
}
