using System;
using Photon.Pun;

// Token: 0x02000102 RID: 258
public abstract class OnNetworkStart : MonoBehaviourPunCallbacks
{
	// Token: 0x0600079D RID: 1949 RVA: 0x000288D6 File Offset: 0x00026AD6
	private void Start()
	{
		this.TryRunningNetworkStart();
	}

	// Token: 0x0600079E RID: 1950 RVA: 0x000288DE File Offset: 0x00026ADE
	public override void OnJoinedRoom()
	{
		base.OnJoinedRoom();
		this.TryRunningNetworkStart();
	}

	// Token: 0x0600079F RID: 1951 RVA: 0x000288EC File Offset: 0x00026AEC
	private void TryRunningNetworkStart()
	{
		if (this.hasRunNetworkStart)
		{
			return;
		}
		if (PhotonNetwork.InRoom)
		{
			this.NetworkStart();
			this.hasRunNetworkStart = true;
		}
	}

	// Token: 0x060007A0 RID: 1952
	public abstract void NetworkStart();

	// Token: 0x0400071A RID: 1818
	private bool hasRunNetworkStart;
}
