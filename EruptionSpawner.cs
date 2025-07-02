using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x020001C4 RID: 452
public class EruptionSpawner : MonoBehaviour
{
	// Token: 0x06000C31 RID: 3121 RVA: 0x0003CCCD File Offset: 0x0003AECD
	private void Start()
	{
		this.min = base.transform.GetChild(0);
		this.max = base.transform.GetChild(1);
		this.photonView = base.GetComponent<PhotonView>();
	}

	// Token: 0x06000C32 RID: 3122 RVA: 0x0003CD00 File Offset: 0x0003AF00
	private void Update()
	{
		if (!PhotonNetwork.IsMasterClient)
		{
			return;
		}
		if (!HelperFunctions.AnyPlayerInZRange(this.min.position.z, this.max.position.z))
		{
			return;
		}
		this.counter -= Time.deltaTime;
		if (this.counter < 0f)
		{
			this.counter = Random.Range(-5f, 15f);
			Vector3 position = base.transform.position;
			position.x += Random.Range(-155f, 155f);
			position.z += Random.Range(-140f, 140f);
			this.photonView.RPC("RPCA_SpawnEruption", RpcTarget.All, new object[] { position });
		}
	}

	// Token: 0x06000C33 RID: 3123 RVA: 0x0003CDD1 File Offset: 0x0003AFD1
	[PunRPC]
	public void RPCA_SpawnEruption(Vector3 position)
	{
		Object.Instantiate<GameObject>(this.eruption, position, Quaternion.LookRotation(Vector3.up));
	}

	// Token: 0x04000B2A RID: 2858
	private float counter = 10f;

	// Token: 0x04000B2B RID: 2859
	public GameObject eruption;

	// Token: 0x04000B2C RID: 2860
	private PhotonView photonView;

	// Token: 0x04000B2D RID: 2861
	private Transform min;

	// Token: 0x04000B2E RID: 2862
	private Transform max;
}
