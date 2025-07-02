using System;
using UnityEngine;

// Token: 0x0200024C RID: 588
public class PSCP_Custom : PropSpawnerConstraintPost
{
	// Token: 0x06000E55 RID: 3669 RVA: 0x00047DE8 File Offset: 0x00045FE8
	public override bool CheckConstraint(GameObject spawned, PropSpawner.SpawnData spawnData)
	{
		CustomSpawnCondition[] components = spawned.GetComponents<CustomSpawnCondition>();
		for (int i = 0; i < components.Length; i++)
		{
			if (!components[i].CheckCondition(spawnData))
			{
				return false;
			}
		}
		return true;
	}
}
