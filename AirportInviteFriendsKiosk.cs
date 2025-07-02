using System;
using Steamworks;
using UnityEngine;

// Token: 0x02000039 RID: 57
public class AirportInviteFriendsKiosk : MonoBehaviour, IInteractible
{
	// Token: 0x060002D4 RID: 724 RVA: 0x000128FE File Offset: 0x00010AFE
	public bool IsInteractible(Character interactor)
	{
		return true;
	}

	// Token: 0x1700002A RID: 42
	// (get) Token: 0x060002D5 RID: 725 RVA: 0x00012901 File Offset: 0x00010B01
	// (set) Token: 0x060002D6 RID: 726 RVA: 0x0001292F File Offset: 0x00010B2F
	private MeshRenderer[] meshRenderers
	{
		get
		{
			if (this._mr == null)
			{
				this._mr = base.GetComponentsInChildren<MeshRenderer>();
				MonoBehaviour.print(this._mr.Length);
			}
			return this._mr;
		}
		set
		{
			this._mr = value;
		}
	}

	// Token: 0x060002D7 RID: 727 RVA: 0x00012938 File Offset: 0x00010B38
	public void Awake()
	{
		this.mpb = new MaterialPropertyBlock();
	}

	// Token: 0x060002D8 RID: 728 RVA: 0x00012948 File Offset: 0x00010B48
	public void Interact(Character interactor)
	{
		CSteamID csteamID;
		if (GameHandler.GetService<SteamLobbyHandler>().InSteamLobby(out csteamID))
		{
			Debug.Log("Open Invite Friends UI...");
			SteamFriends.ActivateGameOverlayInviteDialog(csteamID);
		}
	}

	// Token: 0x060002D9 RID: 729 RVA: 0x00012974 File Offset: 0x00010B74
	public void HoverEnter()
	{
		if (this.mpb != null)
		{
			this.mpb.SetFloat(Item.PROPERTY_INTERACTABLE, 1f);
			for (int i = 0; i < this.meshRenderers.Length; i++)
			{
				if (this.meshRenderers[i] != null)
				{
					this.meshRenderers[i].SetPropertyBlock(this.mpb);
				}
			}
		}
	}

	// Token: 0x060002DA RID: 730 RVA: 0x000129D4 File Offset: 0x00010BD4
	public void HoverExit()
	{
		if (this.mpb != null)
		{
			this.mpb.SetFloat(Item.PROPERTY_INTERACTABLE, 0f);
			for (int i = 0; i < this.meshRenderers.Length; i++)
			{
				this.meshRenderers[i].SetPropertyBlock(this.mpb);
			}
		}
	}

	// Token: 0x060002DB RID: 731 RVA: 0x00012A24 File Offset: 0x00010C24
	public Vector3 Center()
	{
		return base.transform.position;
	}

	// Token: 0x060002DC RID: 732 RVA: 0x00012A31 File Offset: 0x00010C31
	public Transform GetTransform()
	{
		return base.transform;
	}

	// Token: 0x060002DD RID: 733 RVA: 0x00012A39 File Offset: 0x00010C39
	public string GetInteractionText()
	{
		return "Invite Friends";
	}

	// Token: 0x060002DE RID: 734 RVA: 0x00012A40 File Offset: 0x00010C40
	public string GetName()
	{
		return "Invite Kiosk";
	}

	// Token: 0x04000377 RID: 887
	private MaterialPropertyBlock mpb;

	// Token: 0x04000378 RID: 888
	private MeshRenderer[] _mr;
}
