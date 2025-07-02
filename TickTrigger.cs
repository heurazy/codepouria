using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000287 RID: 647
public class TickTrigger : MonoBehaviour
{
	// Token: 0x06000F93 RID: 3987 RVA: 0x0004F238 File Offset: 0x0004D438
	private void Start()
	{
		if (Random.value > this.tickChance)
		{
			Object.Destroy(base.gameObject);
			return;
		}
	}

	// Token: 0x06000F94 RID: 3988 RVA: 0x0004F254 File Offset: 0x0004D454
	private void OnTriggerEnter(Collider other)
	{
		Character componentInParent = other.GetComponentInParent<Character>();
		if (componentInParent && componentInParent.IsLocal)
		{
			PhotonNetwork.Instantiate("BugfixOnYou", Vector3.zero, Quaternion.identity, 0, null).GetComponent<PhotonView>().RPC("AttachBug", RpcTarget.All, new object[] { componentInParent.photonView.ViewID });
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x04000E99 RID: 3737
	public float tickChance = 0.01f;
}
