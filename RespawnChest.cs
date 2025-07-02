using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000256 RID: 598
public class RespawnChest : Luggage, IInteractible
{
	// Token: 0x06000E7E RID: 3710 RVA: 0x00048C37 File Offset: 0x00046E37
	public override string GetInteractionText()
	{
		if (Character.PlayerIsDeadOrDown())
		{
			return "Revive Scouts";
		}
		return "Open";
	}

	// Token: 0x06000E7F RID: 3711 RVA: 0x00048C4B File Offset: 0x00046E4B
	private void DebugSpawn()
	{
		this.SpawnItems(this.GetSpawnSpots());
	}

	// Token: 0x06000E80 RID: 3712 RVA: 0x00048C5A File Offset: 0x00046E5A
	public override void Interact(Character interactor)
	{
	}

	// Token: 0x06000E81 RID: 3713 RVA: 0x00048C5C File Offset: 0x00046E5C
	public override void Interact_CastFinished(Character interactor)
	{
		base.Interact_CastFinished(interactor);
		GlobalEvents.TriggerRespawnChestOpened(this, interactor);
	}

	// Token: 0x06000E82 RID: 3714 RVA: 0x00048C6C File Offset: 0x00046E6C
	public override List<PhotonView> SpawnItems(List<Transform> spawnSpots)
	{
		List<PhotonView> list = new List<PhotonView>();
		if (!PhotonNetwork.IsMasterClient)
		{
			return list;
		}
		if (Ascents.canReviveDead && Character.PlayerIsDeadOrDown())
		{
			base.photonView.RPC("RemoveSkeletonRPC", RpcTarget.AllBuffered, Array.Empty<object>());
			this.RespawnAllPlayersHere();
		}
		else
		{
			base.SpawnItems(spawnSpots);
		}
		return list;
	}

	// Token: 0x06000E83 RID: 3715 RVA: 0x00048CBD File Offset: 0x00046EBD
	[PunRPC]
	private void RemoveSkeletonRPC()
	{
		this.skeleton.SetActive(false);
	}

	// Token: 0x06000E84 RID: 3716 RVA: 0x00048CCC File Offset: 0x00046ECC
	private void RespawnAllPlayersHere()
	{
		foreach (Character character in Character.AllCharacters)
		{
			if (character.data.dead || character.data.fullyPassedOut)
			{
				character.photonView.RPC("RPCA_ReviveAtPosition", RpcTarget.All, new object[]
				{
					base.transform.position + base.transform.up * 8f,
					true
				});
			}
		}
	}

	// Token: 0x06000E85 RID: 3717 RVA: 0x00048D80 File Offset: 0x00046F80
	public new bool IsInteractible(Character interactor)
	{
		return this.state == Luggage.LuggageState.Closed;
	}

	// Token: 0x04000D78 RID: 3448
	public GameObject skeleton;
}
