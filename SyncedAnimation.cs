using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000283 RID: 643
public class SyncedAnimation : MonoBehaviour
{
	// Token: 0x06000F70 RID: 3952 RVA: 0x0004E403 File Offset: 0x0004C603
	private void Start()
	{
		this.view = base.GetComponent<PhotonView>();
		this.anim = base.GetComponent<Animator>();
	}

	// Token: 0x06000F71 RID: 3953 RVA: 0x0004E420 File Offset: 0x0004C620
	private void Update()
	{
		if (PhotonNetwork.IsMasterClient)
		{
			this.syncCounter += Time.deltaTime;
			if (this.syncCounter > 5f)
			{
				this.view.RPC("RPCA_SyncAnim", RpcTarget.All, new object[] { this.anim.GetCurrentAnimatorStateInfo(0).normalizedTime % 1f });
				this.syncCounter = 0f;
			}
		}
	}

	// Token: 0x06000F72 RID: 3954 RVA: 0x0004E498 File Offset: 0x0004C698
	[PunRPC]
	public void RPCA_SyncAnim(float syncTime)
	{
		this.anim.Play(this.anim.GetCurrentAnimatorStateInfo(0).shortNameHash, 0, syncTime);
	}

	// Token: 0x04000E74 RID: 3700
	private PhotonView view;

	// Token: 0x04000E75 RID: 3701
	private Animator anim;

	// Token: 0x04000E76 RID: 3702
	private float syncCounter;
}
