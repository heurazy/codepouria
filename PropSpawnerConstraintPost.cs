using System;
using UnityEngine;

// Token: 0x0200024A RID: 586
[Serializable]
public abstract class PropSpawnerConstraintPost
{
	// Token: 0x06000E51 RID: 3665
	public abstract bool CheckConstraint(GameObject spawned, PropSpawner.SpawnData spawnData);
}
