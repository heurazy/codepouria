using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200022B RID: 555
public class PSM_AddPlayerCountBasedDespawner : PropSpawnerMod
{
	// Token: 0x06000E14 RID: 3604 RVA: 0x00047044 File Offset: 0x00045244
	public override void ModifyObject(GameObject spawned, PropSpawner.SpawnData spawnData)
	{
		if (spawned.GetComponent<PhotonView>())
		{
			DestroyBasedOnPlayerCount destroyBasedOnPlayerCount = spawned.AddComponent<DestroyBasedOnPlayerCount>();
			if (this.onePerPlayer)
			{
				destroyBasedOnPlayerCount.destroyIfPlayerCountIsLessThan = spawnData.spawnCount + 1;
				return;
			}
			destroyBasedOnPlayerCount.destroyIfPlayerCountIsLessThan = this.destroyAllIfLessThan;
		}
	}

	// Token: 0x04000D1C RID: 3356
	public bool onePerPlayer;

	// Token: 0x04000D1D RID: 3357
	public int destroyAllIfLessThan;
}
