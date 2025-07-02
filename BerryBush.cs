using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200007C RID: 124
public class BerryBush : Spawner
{
	// Token: 0x06000461 RID: 1121 RVA: 0x000199E8 File Offset: 0x00017BE8
	public override List<PhotonView> SpawnItems(List<Transform> spawnSpots)
	{
		List<PhotonView> list = new List<PhotonView>();
		if (!PhotonNetwork.IsMasterClient)
		{
			return list;
		}
		List<Transform> list2 = new List<Transform>(spawnSpots);
		GameObject randomItem = LootData.GetRandomItem(this.spawnPool);
		float num = Random.value;
		num = Mathf.Pow(num, this.randomPow);
		int num2 = Mathf.RoundToInt(Mathf.Lerp(this.possibleBerries.x, this.possibleBerries.y, num));
		int num3 = 0;
		while (num3 < spawnSpots.Count && num3 < num2)
		{
			int num4 = Random.Range(0, list2.Count);
			if (!(randomItem == null))
			{
				Item component = PhotonNetwork.InstantiateItemRoom(randomItem.name, list2[num4].position, Quaternion.identity).GetComponent<Item>();
				list.Add(component.GetComponent<PhotonView>());
				if (this.spawnUpTowardsTarget)
				{
					component.transform.up = (this.spawnUpTowardsTarget.position - component.transform.position).normalized;
					component.transform.Rotate(Vector3.up, Random.Range(0f, 360f), Space.Self);
				}
				if (component != null)
				{
					component.GetComponent<PhotonView>().RPC("SetKinematicRPC", RpcTarget.AllBuffered, new object[]
					{
						true,
						component.transform.position,
						component.transform.rotation
					});
				}
				list2.RemoveAt(num4);
			}
			num3++;
		}
		return list;
	}

	// Token: 0x0400049F RID: 1183
	public Vector2 possibleBerries;

	// Token: 0x040004A0 RID: 1184
	public float randomPow = 1f;
}
