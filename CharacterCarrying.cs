using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Zorro.Core;

// Token: 0x020001A2 RID: 418
public class CharacterCarrying : MonoBehaviour
{
	// Token: 0x06000B6F RID: 2927 RVA: 0x0003859C File Offset: 0x0003679C
	private void Start()
	{
		this.character = base.GetComponent<Character>();
	}

	// Token: 0x06000B70 RID: 2928 RVA: 0x000385AC File Offset: 0x000367AC
	private void FixedUpdate()
	{
		if (this.character.data.isCarried && this.character.data.carrier == null)
		{
			this.CarrierGone();
		}
		if (this.character.data.carrier)
		{
			this.GetCarried();
		}
	}

	// Token: 0x06000B71 RID: 2929 RVA: 0x00038608 File Offset: 0x00036808
	private void Update()
	{
		if (this.character.data.carriedPlayer && (this.character.data.carriedPlayer.data.dead || !this.character.data.carriedPlayer.data.fullyPassedOut || this.character.input.selectBackpackWasPressed || this.character.data.fullyPassedOut || this.character.data.dead) && this.character.refs.view.IsMine)
		{
			this.Drop(this.character.data.carriedPlayer);
		}
	}

	// Token: 0x06000B72 RID: 2930 RVA: 0x000386CC File Offset: 0x000368CC
	private void ToggleCarryPhysics(bool setCarried)
	{
		this.character.refs.ragdoll.ToggleCollision(!setCarried);
		this.character.refs.animations.SetBool("IsCarried", setCarried);
		Debug.Log("SetIsCarried: " + setCarried.ToString());
	}

	// Token: 0x06000B73 RID: 2931 RVA: 0x00038724 File Offset: 0x00036924
	private void GetCarried()
	{
		Vector3 vector = Vector3.ClampMagnitude(this.character.data.carrier.refs.carryPosRef.position + this.character.data.carrier.data.avarageVelocity * 0.06f - this.character.Center, 1f);
		this.character.AddForce(vector * 500f, 1f, 1f);
		this.character.refs.movement.ApplyExtraDrag(0.5f, true);
		this.character.data.sinceGrounded = 0f;
	}

	// Token: 0x06000B74 RID: 2932 RVA: 0x000387E4 File Offset: 0x000369E4
	internal void StartCarry(Character target)
	{
		this.character.refs.items.EquipSlot(Optionable<byte>.None);
		this.character.photonView.RPC("RPCA_StartCarry", RpcTarget.All, new object[] { target.photonView });
	}

	// Token: 0x06000B75 RID: 2933 RVA: 0x00038830 File Offset: 0x00036A30
	[PunRPC]
	public void RPCA_StartCarry(PhotonView targetView)
	{
		Character component = targetView.GetComponent<Character>();
		BackpackSlot backpackSlot = this.character.player.backpackSlot;
		if (!backpackSlot.IsEmpty())
		{
			if (PhotonNetwork.IsMasterClient)
			{
				Debug.Log(string.Format("{0} is starting to carry {1} but has backpack, dropping backpack", this.character, component));
				PhotonNetwork.InstantiateItemRoom(backpackSlot.GetPrefabName(), component.GetBodypart(BodypartType.Torso).transform.position, Quaternion.identity).GetComponent<PhotonView>().RPC("SetItemInstanceDataRPC", RpcTarget.All, new object[] { backpackSlot.data });
			}
			backpackSlot.EmptyOut();
		}
		else if (this.character.data.carriedPlayer != null)
		{
			this.character.refs.carriying.Drop(this.character.data.carriedPlayer);
			return;
		}
		component.refs.carriying.ToggleCarryPhysics(true);
		component.data.isCarried = true;
		this.character.data.carriedPlayer = component;
		component.data.carrier = this.character;
		List<Character> allPlayerCharacters = PlayerHandler.GetAllPlayerCharacters();
		for (int i = 0; i < allPlayerCharacters.Count; i++)
		{
			Debug.Log("Updating weight for " + allPlayerCharacters[i].gameObject.name + "...");
			allPlayerCharacters[i].refs.afflictions.UpdateWeight();
		}
	}

	// Token: 0x06000B76 RID: 2934 RVA: 0x00038991 File Offset: 0x00036B91
	internal void Drop(Character target)
	{
		this.character.photonView.RPC("RPCA_Drop", RpcTarget.All, new object[] { target.photonView });
	}

	// Token: 0x06000B77 RID: 2935 RVA: 0x000389B8 File Offset: 0x00036BB8
	[PunRPC]
	public void RPCA_Drop(PhotonView targetView)
	{
		Character component = targetView.GetComponent<Character>();
		component.refs.carriying.ToggleCarryPhysics(false);
		component.data.isCarried = false;
		component.data.carrier = null;
		this.character.data.carriedPlayer = null;
		List<Character> allPlayerCharacters = PlayerHandler.GetAllPlayerCharacters();
		for (int i = 0; i < allPlayerCharacters.Count; i++)
		{
			Debug.Log("Updating weight for " + allPlayerCharacters[i].gameObject.name + "...");
			allPlayerCharacters[i].refs.afflictions.UpdateWeight();
		}
	}

	// Token: 0x06000B78 RID: 2936 RVA: 0x00038A56 File Offset: 0x00036C56
	private void CarrierGone()
	{
		this.character.refs.carriying.ToggleCarryPhysics(false);
	}

	// Token: 0x04000A80 RID: 2688
	private Character character;
}
