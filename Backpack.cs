using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Rendering;
using Zorro.Core;
using Zorro.Core.CLI;

// Token: 0x02000003 RID: 3
public class Backpack : Item
{
	// Token: 0x06000003 RID: 3 RVA: 0x00002131 File Offset: 0x00000331
	public override void Interact(Character interactor)
	{
		GUIManager.instance.OpenBackpackWheel(BackpackReference.GetFromBackpackItem(this));
	}

	// Token: 0x06000004 RID: 4 RVA: 0x00002143 File Offset: 0x00000343
	protected override void Update()
	{
		base.Update();
		this.groundMesh.gameObject.SetActive(this.itemState == ItemState.Ground);
		this.heldMesh.gameObject.SetActive(this.itemState > ItemState.Ground);
	}

	// Token: 0x06000005 RID: 5 RVA: 0x0000217D File Offset: 0x0000037D
	public void ReleaseInteract(Character interactor)
	{
	}

	// Token: 0x06000006 RID: 6 RVA: 0x0000217F File Offset: 0x0000037F
	public void Wear(Character interactor)
	{
		base.Interact(interactor);
	}

	// Token: 0x06000007 RID: 7 RVA: 0x00002188 File Offset: 0x00000388
	private void DisableVisuals()
	{
		this.mainRenderer.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
	}

	// Token: 0x06000008 RID: 8 RVA: 0x00002196 File Offset: 0x00000396
	private void EnableVisuals()
	{
		this.mainRenderer.shadowCastingMode = ShadowCastingMode.On;
	}

	// Token: 0x06000009 RID: 9 RVA: 0x000021A4 File Offset: 0x000003A4
	public void Stash(Character interactor, byte backpackSlotID)
	{
		if (!interactor.data.currentItem)
		{
			return;
		}
		if (!this.HasSpace())
		{
			return;
		}
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
		this.view.RPC("RPCAddItemToBackpack", RpcTarget.All, new object[]
		{
			interactor.player.GetComponent<PhotonView>(),
			items.currentSelectedSlot.Value,
			backpackSlotID
		});
		interactor.player.EmptySlot(items.currentSelectedSlot);
		if (items.currentSelectedSlot.IsSome && items.currentSelectedSlot.Value == 250)
		{
			interactor.photonView.RPC("DestroyHeldItemRpc", RpcTarget.All, Array.Empty<object>());
			return;
		}
		items.EquipSlot(Optionable<byte>.None);
	}

	// Token: 0x0600000A RID: 10 RVA: 0x000022C0 File Offset: 0x000004C0
	[PunRPC]
	public void RPCAddItemToBackpack(PhotonView playerView, byte slotID, byte backpackSlotID)
	{
		BackpackData data = base.GetData<BackpackData>(DataEntryKey.BackpackData);
		ItemSlot itemSlot = playerView.GetComponent<Player>().GetItemSlot(slotID);
		data.AddItem(itemSlot.prefab, itemSlot.data, backpackSlotID);
		if (PhotonNetwork.IsMasterClient)
		{
			base.GetComponent<BackpackVisuals>().RefreshVisuals();
		}
	}

	// Token: 0x0600000B RID: 11 RVA: 0x00002305 File Offset: 0x00000505
	private void OnDestroy()
	{
		base.GetComponent<BackpackVisuals>().RemoveVisuals();
	}

	// Token: 0x0600000C RID: 12 RVA: 0x00002312 File Offset: 0x00000512
	private bool HasSpace()
	{
		return base.GetData<BackpackData>(DataEntryKey.BackpackData).HasFreeSlot();
	}

	// Token: 0x0600000D RID: 13 RVA: 0x00002320 File Offset: 0x00000520
	public int FilledSlotCount()
	{
		return base.GetData<BackpackData>(DataEntryKey.BackpackData).FilledSlotCount();
	}

	// Token: 0x0600000E RID: 14 RVA: 0x0000232E File Offset: 0x0000052E
	public override string GetInteractionText()
	{
		return "open";
	}

	// Token: 0x0600000F RID: 15 RVA: 0x00002335 File Offset: 0x00000535
	public override void OnInstanceDataRecieved()
	{
		base.OnInstanceDataRecieved();
		base.GetComponent<BackpackVisuals>().RefreshVisuals();
	}

	// Token: 0x06000010 RID: 16 RVA: 0x00002348 File Offset: 0x00000548
	[ConsoleCommand]
	public static void PrintBackpacks()
	{
		foreach (Backpack backpack in Object.FindObjectsByType<Backpack>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID))
		{
			List<ItemSlot> list = backpack.GetData<BackpackData>(DataEntryKey.BackpackData).itemSlots.Where((ItemSlot slot) => !slot.IsEmpty()).ToList<ItemSlot>();
			Debug.Log(string.Format("Backpack: {0}, Full Slots: {1}", backpack.GetInstanceID(), list.Count));
			foreach (ItemSlot itemSlot in list)
			{
				Debug.Log(string.Format("Slot: {0}, data entries: {1}", itemSlot.GetPrefabName(), itemSlot.data.data.Count));
			}
		}
	}

	// Token: 0x06000011 RID: 17 RVA: 0x00002438 File Offset: 0x00000638
	public bool IsConstantlyInteractable(Character interactor)
	{
		return false;
	}

	// Token: 0x06000012 RID: 18 RVA: 0x0000243B File Offset: 0x0000063B
	public float GetInteractTime(Character interactor)
	{
		return this.openRadialMenuTime;
	}

	// Token: 0x06000013 RID: 19 RVA: 0x00002443 File Offset: 0x00000643
	public void Interact_CastFinished(Character interactor)
	{
	}

	// Token: 0x06000014 RID: 20 RVA: 0x00002445 File Offset: 0x00000645
	public void CancelCast(Character interactor)
	{
	}

	// Token: 0x17000001 RID: 1
	// (get) Token: 0x06000015 RID: 21 RVA: 0x00002447 File Offset: 0x00000647
	public bool holdOnFinish
	{
		get
		{
			return false;
		}
	}

	// Token: 0x04000002 RID: 2
	public Transform[] backpackSlots;

	// Token: 0x04000003 RID: 3
	public float openRadialMenuTime = 0.25f;

	// Token: 0x04000004 RID: 4
	public GameObject groundMesh;

	// Token: 0x04000005 RID: 5
	public GameObject heldMesh;
}
