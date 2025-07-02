using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200028F RID: 655
public class TriggerRelay : MonoBehaviour
{
	// Token: 0x06000FAD RID: 4013 RVA: 0x0004F66B File Offset: 0x0004D86B
	private void Awake()
	{
		this.view = base.GetComponent<PhotonView>();
	}

	// Token: 0x06000FAE RID: 4014 RVA: 0x0004F679 File Offset: 0x0004D879
	[PunRPC]
	public void RPCA_Trigger(int childID)
	{
		base.transform.GetChild(childID).GetComponent<TriggerEvent>().Trigger();
	}

	// Token: 0x06000FAF RID: 4015 RVA: 0x0004F691 File Offset: 0x0004D891
	[PunRPC]
	public void RPCA_TriggerWithTarget(int childID, int targetID)
	{
		base.transform.GetChild(childID).GetComponent<SlipperyJellyfish>().Trigger(targetID);
	}

	// Token: 0x04000EB7 RID: 3767
	internal PhotonView view;
}
