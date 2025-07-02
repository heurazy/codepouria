using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000212 RID: 530
[RequireComponent(typeof(PhotonView))]
public class ChangeName : MonoBehaviour
{
	// Token: 0x06000DAE RID: 3502 RVA: 0x00044ED8 File Offset: 0x000430D8
	private void Start()
	{
		PhotonView component = base.GetComponent<PhotonView>();
		base.name = string.Format("ActorNumber {0}", component.OwnerActorNr);
	}
}
