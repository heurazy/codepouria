using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000083 RID: 131
public class GroundPlaceSpawner : Spawner
{
	// Token: 0x0600049A RID: 1178 RVA: 0x0001AA48 File Offset: 0x00018C48
	public override List<PhotonView> SpawnItems(List<Transform> spawnSpots)
	{
		List<PhotonView> list = new List<PhotonView>();
		if (!PhotonNetwork.IsMasterClient)
		{
			return list;
		}
		List<Transform> list2 = new List<Transform>(spawnSpots);
		GameObject gameObject = this.spawns.GetSpawns(1, true)[0];
		int num = Random.Range(Mathf.FloorToInt(this.possibleItems.x), Mathf.FloorToInt(this.possibleItems.y + 1f));
		int num2 = 0;
		while (num2 < spawnSpots.Count && num2 < num)
		{
			int num3 = Random.Range(0, list2.Count);
			RaycastHit raycastHit;
			if (Physics.Raycast(list2[num3].position, -base.transform.up, out raycastHit, 100f, HelperFunctions.GetMask(HelperFunctions.LayerType.TerrainMap)))
			{
				Item component = PhotonNetwork.InstantiateItemRoom(gameObject.name, raycastHit.point, HelperFunctions.GetRandomRotationWithUp(raycastHit.normal)).GetComponent<Item>();
				list.Add(component.GetComponent<PhotonView>());
				component.SetKinematicNetworked(true, component.transform.position, component.transform.rotation);
			}
			list2.RemoveAt(num3);
			num2++;
		}
		return list;
	}

	// Token: 0x040004CF RID: 1231
	[FormerlySerializedAs("possibleBerries")]
	public Vector2 possibleItems;
}
