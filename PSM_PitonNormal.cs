using System;
using UnityEngine;

// Token: 0x02000230 RID: 560
public class PSM_PitonNormal : PropSpawnerMod
{
	// Token: 0x06000E1E RID: 3614 RVA: 0x0004725A File Offset: 0x0004545A
	public override void ModifyObject(GameObject spawned, PropSpawner.SpawnData spawnData)
	{
		spawned.transform.rotation = Quaternion.LookRotation(-spawnData.hit.normal, Vector3.up);
	}
}
