using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x020001AA RID: 426
public class ClimbHandle : MonoBehaviour, IInteractible
{
	// Token: 0x06000BD8 RID: 3032 RVA: 0x0003B87E File Offset: 0x00039A7E
	private void Start()
	{
		this.view = base.GetComponent<PhotonView>();
	}

	// Token: 0x06000BD9 RID: 3033 RVA: 0x0003B88C File Offset: 0x00039A8C
	public Vector3 Center()
	{
		return base.transform.position;
	}

	// Token: 0x06000BDA RID: 3034 RVA: 0x0003B899 File Offset: 0x00039A99
	public string GetInteractionText()
	{
		return "Grab";
	}

	// Token: 0x06000BDB RID: 3035 RVA: 0x0003B8A0 File Offset: 0x00039AA0
	public string GetName()
	{
		return "piton";
	}

	// Token: 0x06000BDC RID: 3036 RVA: 0x0003B8A7 File Offset: 0x00039AA7
	public Transform GetTransform()
	{
		return base.transform;
	}

	// Token: 0x06000BDD RID: 3037 RVA: 0x0003B8AF File Offset: 0x00039AAF
	public void HoverEnter()
	{
	}

	// Token: 0x06000BDE RID: 3038 RVA: 0x0003B8B1 File Offset: 0x00039AB1
	public void HoverExit()
	{
	}

	// Token: 0x06000BDF RID: 3039 RVA: 0x0003B8B3 File Offset: 0x00039AB3
	public void Interact(Character interactor)
	{
		if (this.hanger)
		{
			return;
		}
		this.view.RPC("RPCA_Hang", RpcTarget.All, new object[] { interactor.photonView });
	}

	// Token: 0x06000BE0 RID: 3040 RVA: 0x0003B8E4 File Offset: 0x00039AE4
	[PunRPC]
	public void RPCA_Hang(PhotonView view)
	{
		if (view == null)
		{
			return;
		}
		Character component = view.GetComponent<Character>();
		if (component == null)
		{
			return;
		}
		this.hanger = component;
		component.refs.climbing.StartHang(this);
		Action<Character> action = this.onHangStart;
		if (action == null)
		{
			return;
		}
		action(component);
	}

	// Token: 0x06000BE1 RID: 3041 RVA: 0x0003B938 File Offset: 0x00039B38
	[PunRPC]
	public void RPCA_UnHang(PhotonView view)
	{
		this.hanger = null;
		if (view == null)
		{
			return;
		}
		Character component = view.GetComponent<Character>();
		if (component == null)
		{
			return;
		}
		component.data.currentClimbHandle = null;
		Action action = this.onHangStop;
		if (action == null)
		{
			return;
		}
		action();
	}

	// Token: 0x06000BE2 RID: 3042 RVA: 0x0003B983 File Offset: 0x00039B83
	public bool IsInteractible(Character interactor)
	{
		return this.hanger == null;
	}

	// Token: 0x06000BE3 RID: 3043 RVA: 0x0003B991 File Offset: 0x00039B91
	internal void Break()
	{
		if (this.hanger != null)
		{
			this.hanger.refs.climbing.CancelHandle(true);
		}
		base.gameObject.SetActive(false);
	}

	// Token: 0x04000AAF RID: 2735
	public Character hanger;

	// Token: 0x04000AB0 RID: 2736
	internal PhotonView view;

	// Token: 0x04000AB1 RID: 2737
	public Action<Character> onHangStart;

	// Token: 0x04000AB2 RID: 2738
	public Action onHangStop;
}
