using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x020000DD RID: 221
public class Luggage : Spawner, IInteractibleConstant, IInteractible
{
	// Token: 0x1700005B RID: 91
	// (get) Token: 0x060006B9 RID: 1721 RVA: 0x0002372E File Offset: 0x0002192E
	// (set) Token: 0x060006BA RID: 1722 RVA: 0x0002374A File Offset: 0x0002194A
	private MeshRenderer[] meshRenderers
	{
		get
		{
			if (this._mr == null)
			{
				this._mr = base.GetComponentsInChildren<MeshRenderer>();
			}
			return this._mr;
		}
		set
		{
			this._mr = value;
		}
	}

	// Token: 0x060006BB RID: 1723 RVA: 0x00023753 File Offset: 0x00021953
	private void Awake()
	{
		this.photonView = base.GetComponent<PhotonView>();
		this.anim = base.GetComponent<Animator>();
		this.mpb = new MaterialPropertyBlock();
		Luggage.ALL_LUGGAGE.Add(this);
	}

	// Token: 0x060006BC RID: 1724 RVA: 0x00023783 File Offset: 0x00021983
	public virtual void Interact(Character interactor)
	{
		this.anim.Play("Luggage_Unclasp");
	}

	// Token: 0x060006BD RID: 1725 RVA: 0x00023795 File Offset: 0x00021995
	[PunRPC]
	protected void OpenLuggageRPC(bool spawnItems)
	{
		if (this.state == Luggage.LuggageState.Closed)
		{
			this.anim.Play("Luggage_Open");
			Luggage.ALL_LUGGAGE.Remove(this);
			this.state = Luggage.LuggageState.Open;
			if (spawnItems)
			{
				base.StartCoroutine(this.<OpenLuggageRPC>g__SpawnItemRoutine|14_0());
			}
		}
	}

	// Token: 0x060006BE RID: 1726 RVA: 0x000237D2 File Offset: 0x000219D2
	private void OnDestroy()
	{
		if (Luggage.ALL_LUGGAGE.Contains(this))
		{
			Luggage.ALL_LUGGAGE.Remove(this);
		}
	}

	// Token: 0x060006BF RID: 1727 RVA: 0x000237F0 File Offset: 0x000219F0
	public Vector3 Center()
	{
		return HelperFunctions.GetTotalBounds(this.meshRenderers).center;
	}

	// Token: 0x060006C0 RID: 1728 RVA: 0x00023810 File Offset: 0x00021A10
	public Transform GetTransform()
	{
		return base.transform;
	}

	// Token: 0x060006C1 RID: 1729 RVA: 0x00023818 File Offset: 0x00021A18
	public virtual string GetInteractionText()
	{
		return "open";
	}

	// Token: 0x060006C2 RID: 1730 RVA: 0x0002381F File Offset: 0x00021A1F
	public string GetName()
	{
		return this.displayName;
	}

	// Token: 0x060006C3 RID: 1731 RVA: 0x00023827 File Offset: 0x00021A27
	public bool IsInteractible(Character interactor)
	{
		return this.state == Luggage.LuggageState.Closed;
	}

	// Token: 0x060006C4 RID: 1732 RVA: 0x00023834 File Offset: 0x00021A34
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

	// Token: 0x060006C5 RID: 1733 RVA: 0x00023894 File Offset: 0x00021A94
	public void HoverExit()
	{
		if (this.mpb != null)
		{
			this.mpb.SetFloat(Item.PROPERTY_INTERACTABLE, 0f);
			for (int i = 0; i < this.meshRenderers.Length; i++)
			{
				if (this.meshRenderers[i] != null)
				{
					this.meshRenderers[i].SetPropertyBlock(this.mpb);
				}
			}
		}
	}

	// Token: 0x060006C6 RID: 1734 RVA: 0x000238F4 File Offset: 0x00021AF4
	public void ReleaseInteract(Character interactor)
	{
	}

	// Token: 0x060006C7 RID: 1735 RVA: 0x000238F6 File Offset: 0x00021AF6
	public bool IsConstantlyInteractable(Character interactor)
	{
		return this.state == Luggage.LuggageState.Closed;
	}

	// Token: 0x060006C8 RID: 1736 RVA: 0x00023901 File Offset: 0x00021B01
	public float GetInteractTime(Character interactor)
	{
		return this.timeToOpen;
	}

	// Token: 0x060006C9 RID: 1737 RVA: 0x0002390C File Offset: 0x00021B0C
	public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
	{
		base.OnPlayerEnteredRoom(newPlayer);
		if (newPlayer.ActorNumber != PhotonNetwork.LocalPlayer.ActorNumber && PhotonNetwork.IsMasterClient && this.state == Luggage.LuggageState.Open)
		{
			this.photonView.RPC("OpenLuggageRPC", RpcTarget.All, new object[] { false });
		}
	}

	// Token: 0x060006CA RID: 1738 RVA: 0x00023962 File Offset: 0x00021B62
	public virtual void Interact_CastFinished(Character interactor)
	{
		if (this.state == Luggage.LuggageState.Closed)
		{
			this.photonView.RPC("OpenLuggageRPC", RpcTarget.All, new object[] { true });
			GlobalEvents.TriggerLuggageOpened(this, interactor);
		}
	}

	// Token: 0x060006CB RID: 1739 RVA: 0x00023993 File Offset: 0x00021B93
	public void CancelCast(Character interactor)
	{
		this.anim.SetTrigger("Reclasp");
	}

	// Token: 0x1700005C RID: 92
	// (get) Token: 0x060006CC RID: 1740 RVA: 0x000239A5 File Offset: 0x00021BA5
	public bool holdOnFinish
	{
		get
		{
			return false;
		}
	}

	// Token: 0x060006CF RID: 1743 RVA: 0x000239BC File Offset: 0x00021BBC
	[CompilerGenerated]
	private IEnumerator <OpenLuggageRPC>g__SpawnItemRoutine|14_0()
	{
		yield return new WaitForSeconds(0.1f);
		this.SpawnItems(this.GetSpawnSpots());
		yield break;
	}

	// Token: 0x0400065B RID: 1627
	public string displayName;

	// Token: 0x0400065C RID: 1628
	private Animator anim;

	// Token: 0x0400065D RID: 1629
	[SerializeField]
	protected Luggage.LuggageState state;

	// Token: 0x0400065E RID: 1630
	private new PhotonView photonView;

	// Token: 0x0400065F RID: 1631
	public float timeToOpen;

	// Token: 0x04000660 RID: 1632
	private MaterialPropertyBlock mpb;

	// Token: 0x04000661 RID: 1633
	public static List<Luggage> ALL_LUGGAGE = new List<Luggage>();

	// Token: 0x04000662 RID: 1634
	private MeshRenderer[] _mr;

	// Token: 0x0200032C RID: 812
	public enum LuggageState
	{
		// Token: 0x040011AA RID: 4522
		Closed,
		// Token: 0x040011AB RID: 4523
		Open
	}
}
