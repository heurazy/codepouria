using System;
using UnityEngine;

// Token: 0x0200022A RID: 554
public class Campfire_Set_Segment : PropSpawnerMod
{
	// Token: 0x06000E12 RID: 3602 RVA: 0x00047026 File Offset: 0x00045226
	public override void ModifyObject(GameObject spawned, PropSpawner.SpawnData spawnData)
	{
		spawned.GetComponentInChildren<Campfire>().advanceToSegment = this.Segment;
	}

	// Token: 0x04000D1B RID: 3355
	public Segment Segment;
}
