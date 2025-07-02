using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200007D RID: 125
public class BerryVine : Spawner
{
	// Token: 0x06000463 RID: 1123 RVA: 0x00019B88 File Offset: 0x00017D88
	protected override List<Transform> GetSpawnSpots()
	{
		Collider[] componentsInChildren = base.GetComponentsInChildren<Collider>();
		List<Transform> list = new List<Transform>();
		for (int i = 1; i < componentsInChildren.Length - 1; i++)
		{
			list.Add(componentsInChildren[i].transform);
		}
		return list;
	}

	// Token: 0x06000464 RID: 1124 RVA: 0x00019BC4 File Offset: 0x00017DC4
	public override List<PhotonView> SpawnItems(List<Transform> spawnSpots)
	{
		List<PhotonView> list = new List<PhotonView>();
		if (!PhotonNetwork.IsMasterClient)
		{
			return list;
		}
		List<Transform> list2 = new List<Transform>(spawnSpots);
		GameObject gameObject = this.spawns.GetSpawns(1, true)[0];
		float num = Random.value;
		num = Mathf.Pow(num, this.randomPow);
		int num2 = Mathf.RoundToInt(Mathf.Lerp(this.possibleBerries.x, this.possibleBerries.y, num));
		int num3 = 0;
		while (num3 < spawnSpots.Count && num3 < num2)
		{
			int num4 = Random.Range(0, list2.Count);
			Item component = PhotonNetwork.InstantiateItemRoom(gameObject.name, list2[num4].position, Quaternion.identity).GetComponent<Item>();
			list.Add(component.GetComponent<PhotonView>());
			if (this.spawnUpTowardsTarget)
			{
				component.transform.up = (this.spawnUpTowardsTarget.position - component.transform.position).normalized;
			}
			component.transform.rotation = Quaternion.Euler(0f, (float)Random.Range(0, 360), 0f);
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
			num3++;
		}
		return list;
	}

	// Token: 0x040004A1 RID: 1185
	public Vector2 possibleBerries;

	// Token: 0x040004A2 RID: 1186
	public float randomPow = 1f;
}
