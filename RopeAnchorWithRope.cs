using System;
using System.Collections;
using System.Runtime.CompilerServices;
using Photon.Pun;
using pworld.Scripts.Extensions;
using UnityEngine;

// Token: 0x02000116 RID: 278
public class RopeAnchorWithRope : MonoBehaviourPunCallbacks
{
	// Token: 0x06000823 RID: 2083 RVA: 0x0002B32D File Offset: 0x0002952D
	public override void OnJoinedRoom()
	{
		base.OnJoinedRoom();
		this.SpawnRope();
	}

	// Token: 0x06000824 RID: 2084 RVA: 0x0002B33C File Offset: 0x0002953C
	public Rope SpawnRope()
	{
		if (!base.photonView.IsMine)
		{
			return null;
		}
		this.ropeInstance = PhotonNetwork.Instantiate(this.ropePrefab.name, this.anchor.anchorPoint.position, this.anchor.anchorPoint.rotation, 0, null);
		this.rope = this.ropeInstance.GetComponent<Rope>();
		this.rope.photonView.RPC("AttachToAnchor_Rpc", RpcTarget.AllBuffered, new object[] { this.anchor.photonView });
		this.rope.Segments = this.ropeSegmentLength;
		base.StartCoroutine(this.<SpawnRope>g__SpoolOut|7_0());
		return this.rope;
	}

	// Token: 0x06000825 RID: 2085 RVA: 0x0002B3EF File Offset: 0x000295EF
	public virtual void Awake()
	{
		this.anchor = base.GetComponent<RopeAnchor>();
	}

	// Token: 0x06000827 RID: 2087 RVA: 0x0002B41B File Offset: 0x0002961B
	[CompilerGenerated]
	private IEnumerator <SpawnRope>g__SpoolOut|7_0()
	{
		float elapsed = 0f;
		while (elapsed < this.spoolOutTime)
		{
			elapsed += Time.deltaTime;
			this.rope.Segments = Mathf.Lerp(0f, this.ropeSegmentLength, (elapsed / this.spoolOutTime).Clamp01());
			yield return null;
		}
		yield break;
	}

	// Token: 0x040007A0 RID: 1952
	public float ropeSegmentLength = 20f;

	// Token: 0x040007A1 RID: 1953
	public float spoolOutTime = 5f;

	// Token: 0x040007A2 RID: 1954
	public GameObject ropePrefab;

	// Token: 0x040007A3 RID: 1955
	public GameObject ropeInstance;

	// Token: 0x040007A4 RID: 1956
	public RopeAnchor anchor;

	// Token: 0x040007A5 RID: 1957
	public Rope rope;
}
