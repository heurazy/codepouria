using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000144 RID: 324
public class TempleConfig : MonoBehaviourPunCallbacks
{
	// Token: 0x06000944 RID: 2372 RVA: 0x0002EDC2 File Offset: 0x0002CFC2
	private void Awake()
	{
		this.view = base.GetComponent<PhotonView>();
	}

	// Token: 0x06000945 RID: 2373 RVA: 0x0002EDD0 File Offset: 0x0002CFD0
	private void Start()
	{
		for (int i = 0; i < this.columns.Count; i++)
		{
			this.positions.Add(this.columns[i].transform.position);
		}
	}

	// Token: 0x06000946 RID: 2374 RVA: 0x0002EE14 File Offset: 0x0002D014
	public override void OnJoinedRoom()
	{
		base.OnJoinedRoom();
		if (this.view.IsMine)
		{
			this.view.RPC("CreateTemple_RPC", RpcTarget.AllBuffered, new object[] { (int)DateTime.Now.Ticks });
		}
	}

	// Token: 0x06000947 RID: 2375 RVA: 0x0002EE64 File Offset: 0x0002D064
	[PunRPC]
	public void CreateTemple_RPC(int seed)
	{
		Debug.Log("Set Seed");
		Random.InitState(seed);
		List<GameObject> list = this.columns;
		list = list.OrderBy((GameObject x) => Random.value).ToList<GameObject>();
		for (int i = 0; i < list.Count; i++)
		{
			list[i].transform.position = this.positions[i];
			this.columns[i].transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, (float)((int)(Random.value * 4f) * 90)));
		}
		for (int j = 0; j < this.arrowShooters.Length; j++)
		{
			if (Random.value < this.arrowShooterChance)
			{
				this.arrowShooters[j].SetActive(true);
			}
			else
			{
				this.arrowShooters[j].SetActive(false);
			}
		}
	}

	// Token: 0x06000948 RID: 2376 RVA: 0x0002EF59 File Offset: 0x0002D159
	private void Update()
	{
	}

	// Token: 0x0400083C RID: 2108
	private PhotonView view;

	// Token: 0x0400083D RID: 2109
	[Range(0f, 1f)]
	public float arrowShooterChance;

	// Token: 0x0400083E RID: 2110
	public List<GameObject> columns;

	// Token: 0x0400083F RID: 2111
	private List<Vector3> positions = new List<Vector3>();

	// Token: 0x04000840 RID: 2112
	public GameObject[] arrowShooters;
}
