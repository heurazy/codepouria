using System;
using UnityEngine;

// Token: 0x020001B5 RID: 437
public abstract class CustomSpawnCondition : MonoBehaviour
{
	// Token: 0x06000BFC RID: 3068
	public abstract bool CheckCondition(PropSpawner.SpawnData data);
}
