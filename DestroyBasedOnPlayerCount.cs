using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;

// Token: 0x020001BB RID: 443
public class DestroyBasedOnPlayerCount : MonoBehaviourPun
{
	// Token: 0x06000C18 RID: 3096 RVA: 0x0003C85C File Offset: 0x0003AA5C
	private IEnumerator Start()
	{
		while (!PhotonNetwork.InRoom)
		{
			yield return null;
		}
		if (!PhotonNetwork.IsMasterClient)
		{
			yield break;
		}
		if (PhotonNetwork.PlayerList.Length < this.destroyIfPlayerCountIsLessThan)
		{
			Debug.Log(string.Format("Item was told to destroy if player count <{0} and it is {1}", this.destroyIfPlayerCountIsLessThan, PhotonNetwork.PlayerList.Length));
			PhotonNetwork.Destroy(base.photonView);
		}
		yield break;
	}

	// Token: 0x04000B18 RID: 2840
	public int destroyIfPlayerCountIsLessThan;
}
