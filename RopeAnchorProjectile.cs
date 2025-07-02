using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x02000115 RID: 277
public class RopeAnchorProjectile : MonoBehaviourPunCallbacks
{
	// Token: 0x0600081F RID: 2079 RVA: 0x0002B208 File Offset: 0x00029408
	public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
	{
		base.OnPlayerEnteredRoom(newPlayer);
		if (PhotonNetwork.IsMasterClient && this.shot)
		{
			this.photonView.RPC("GetShot", newPlayer, new object[] { this.lastShotTo, this.lastShotTravelTime, this.lastShotRopeLength, this.lastShotFlyingRotation });
		}
	}

	// Token: 0x06000820 RID: 2080 RVA: 0x0002B27A File Offset: 0x0002947A
	private void Awake()
	{
		this.photonView = base.GetComponent<PhotonView>();
	}

	// Token: 0x06000821 RID: 2081 RVA: 0x0002B288 File Offset: 0x00029488
	[PunRPC]
	public void GetShot(Vector3 to, float travelTime, float ropeLength, Vector3 flyingRotation)
	{
		RopeAnchorProjectile.<>c__DisplayClass10_0 CS$<>8__locals1 = new RopeAnchorProjectile.<>c__DisplayClass10_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.flyingRotation = flyingRotation;
		CS$<>8__locals1.to = to;
		CS$<>8__locals1.travelTime = travelTime;
		CS$<>8__locals1.ropeLength = ropeLength;
		this.lastShotTo = CS$<>8__locals1.to;
		this.lastShotTravelTime = CS$<>8__locals1.travelTime;
		this.lastShotRopeLength = CS$<>8__locals1.ropeLength;
		this.lastShotFlyingRotation = CS$<>8__locals1.flyingRotation;
		this.shot = true;
		this.startRotation = base.transform.rotation;
		this.startPosition = base.transform.position;
		base.StartCoroutine(CS$<>8__locals1.<GetShot>g__SpawnRopeRoutine|0());
	}

	// Token: 0x04000798 RID: 1944
	public new PhotonView photonView;

	// Token: 0x04000799 RID: 1945
	public bool shot;

	// Token: 0x0400079A RID: 1946
	private Vector3 startPosition;

	// Token: 0x0400079B RID: 1947
	private Quaternion startRotation;

	// Token: 0x0400079C RID: 1948
	private Vector3 lastShotTo;

	// Token: 0x0400079D RID: 1949
	private float lastShotTravelTime;

	// Token: 0x0400079E RID: 1950
	private float lastShotRopeLength;

	// Token: 0x0400079F RID: 1951
	private Vector3 lastShotFlyingRotation;
}
