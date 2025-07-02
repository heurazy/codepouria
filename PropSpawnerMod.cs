using System;
using UnityEngine;

// Token: 0x02000229 RID: 553
[Serializable]
public abstract class PropSpawnerMod
{
	// Token: 0x06000E10 RID: 3600
	public abstract void ModifyObject(GameObject spawned, PropSpawner.SpawnData spawnData);
}
