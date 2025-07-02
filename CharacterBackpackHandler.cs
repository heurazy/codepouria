using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;
using Zorro.Core;

// Token: 0x02000053 RID: 83
public class CharacterBackpackHandler : MonoBehaviour
{
	// Token: 0x06000389 RID: 905 RVA: 0x00015598 File Offset: 0x00013798
	private void Awake()
	{
		this.character = base.GetComponent<Character>();
		this.characterItems = base.GetComponent<CharacterItems>();
		this.photonView = base.GetComponent<PhotonView>();
	}

	// Token: 0x0600038A RID: 906 RVA: 0x000155C0 File Offset: 0x000137C0
	private void LateUpdate()
	{
		bool flag = !this.character.player.backpackSlot.IsEmpty();
		bool flag2 = this.characterItems.currentSelectedSlot.IsSome && this.characterItems.currentSelectedSlot.Value == 3;
		bool flag3 = flag && !flag2;
		bool flag4 = flag3;
		if (this.character.photonView.IsMine && !MainCameraMovement.IsSpectating)
		{
			flag4 = false;
		}
		this.backpack.SetActive(flag4);
		if (flag3)
		{
			if (!this.t)
			{
				for (int i = 0; i < this.wearSFX.Length; i++)
				{
					this.wearSFX[i].Play(this.character.refs.hip.transform.position);
				}
			}
			this.t = true;
		}
		else
		{
			this.t = false;
		}
		if (PhotonNetwork.IsMasterClient)
		{
			if (!this.lastShow && flag3)
			{
				base.StartCoroutine(this.RefreshVisualsDelayed());
			}
			else if (this.lastShow && !flag3)
			{
				this.backpackVisuals.RemoveVisuals();
			}
		}
		this.lastShow = flag3;
	}

	// Token: 0x0600038B RID: 907 RVA: 0x000156D6 File Offset: 0x000138D6
	private IEnumerator RefreshVisualsDelayed()
	{
		yield return null;
		this.backpackVisuals.RefreshVisuals();
		yield break;
	}

	// Token: 0x0600038C RID: 908 RVA: 0x000156E8 File Offset: 0x000138E8
	public void StashInBackpack(Character interactor, byte backpackSlotID)
	{
		CharacterItems items = interactor.refs.items;
		if (items.currentSelectedSlot.IsNone)
		{
			Debug.LogError("Need item slot selected to stash item in backpack!");
			return;
		}
		ItemSlot itemSlot = interactor.player.GetItemSlot(items.currentSelectedSlot.Value);
		if (itemSlot.IsEmpty())
		{
			Debug.LogError(string.Format("Item slot {0} is empty!", itemSlot.itemSlotID));
			return;
		}
		this.photonView.RPC("RPCAddItemToCharacterBackpack", RpcTarget.All, new object[]
		{
			interactor.player.GetComponent<PhotonView>(),
			items.currentSelectedSlot.Value,
			backpackSlotID
		});
		interactor.player.EmptySlot(items.currentSelectedSlot);
		items.EquipSlot(Optionable<byte>.None);
	}

	// Token: 0x0600038D RID: 909 RVA: 0x000157B0 File Offset: 0x000139B0
	[PunRPC]
	public void RPCAddItemToCharacterBackpack(PhotonView playerView, byte inventorySlotID, byte backpackSlotID)
	{
		BackpackData backpackData;
		if (!this.character.player.backpackSlot.data.TryGetDataEntry<BackpackData>(DataEntryKey.BackpackData, out backpackData))
		{
			backpackData = this.character.player.backpackSlot.data.RegisterNewEntry<BackpackData>(DataEntryKey.BackpackData);
		}
		ItemSlot itemSlot = playerView.GetComponent<Player>().GetItemSlot(inventorySlotID);
		backpackData.AddItem(itemSlot.prefab, itemSlot.data, backpackSlotID);
		if (PhotonNetwork.IsMasterClient)
		{
			this.backpackVisuals.RefreshVisuals();
		}
		if (this.character.IsLocal)
		{
			this.character.refs.afflictions.UpdateWeight();
		}
	}

	// Token: 0x0400040D RID: 1037
	private Character character;

	// Token: 0x0400040E RID: 1038
	private CharacterItems characterItems;

	// Token: 0x0400040F RID: 1039
	private PhotonView photonView;

	// Token: 0x04000410 RID: 1040
	public BackpackOnBackVisuals backpackVisuals;

	// Token: 0x04000411 RID: 1041
	public GameObject backpack;

	// Token: 0x04000412 RID: 1042
	private bool lastShow;

	// Token: 0x04000413 RID: 1043
	public SFX_Instance[] wearSFX;

	// Token: 0x04000414 RID: 1044
	private bool t;
}
