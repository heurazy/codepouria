using System;
using System.Runtime.CompilerServices;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Zorro.Core;
using Zorro.Core.Serizalization;

// Token: 0x02000021 RID: 33
public class Player : MonoBehaviourPunCallbacks
{
	// Token: 0x17000027 RID: 39
	// (get) Token: 0x0600022D RID: 557 RVA: 0x0000FA51 File Offset: 0x0000DC51
	public Character character
	{
		get
		{
			return PlayerHandler.GetPlayerCharacter(this.view.Owner);
		}
	}

	// Token: 0x0600022E RID: 558 RVA: 0x0000FA64 File Offset: 0x0000DC64
	private void Awake()
	{
		this.view = base.GetComponent<PhotonView>();
		byte b = 0;
		while ((int)b < this.itemSlots.Length)
		{
			this.itemSlots[(int)b] = new ItemSlot(b);
			b += 1;
		}
		this.tempFullSlot = new TemporaryItemSlot(250);
		this.backpackSlot = new BackpackSlot(3);
		if (this.view != null)
		{
			PlayerHandler.RegisterPlayer(this);
			if (this.view.IsMine)
			{
				global::Player.localPlayer = this;
			}
		}
		base.gameObject.name = "Player: " + this.view.Owner.NickName;
	}

	// Token: 0x0600022F RID: 559 RVA: 0x0000FB08 File Offset: 0x0000DD08
	public bool AddItem(ushort itemID, ItemInstanceData instanceData, out ItemSlot slot)
	{
		global::Player.<>c__DisplayClass13_0 CS$<>8__locals1;
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.instanceData = instanceData;
		if (CS$<>8__locals1.instanceData == null)
		{
			CS$<>8__locals1.instanceData = new ItemInstanceData(Guid.NewGuid());
			ItemInstanceDataHandler.AddInstanceData(CS$<>8__locals1.instanceData);
		}
		if (!PhotonNetwork.IsMasterClient)
		{
			Debug.LogError("Only Master Client can add items!");
			slot = null;
			return false;
		}
		if (!ItemDatabase.TryGetItem(itemID, out CS$<>8__locals1.ItemPrefab))
		{
			Debug.LogError(string.Format("Failed to get item from item ID: {0}", itemID));
			slot = null;
			return false;
		}
		slot = this.<AddItem>g__AddToSlot|13_0(ref CS$<>8__locals1);
		if (slot == null)
		{
			Debug.LogError(string.Concat(new string[]
			{
				"Failed adding ",
				CS$<>8__locals1.ItemPrefab.name,
				" to ",
				base.name,
				"'s inventory, no slots available!"
			}));
			return false;
		}
		Debug.Log(string.Format("Granting {0}: {1} and added to slot: {2}", base.name, CS$<>8__locals1.ItemPrefab.name, slot.itemSlotID));
		byte[] array = IBinarySerializable.ToManagedArray<InventorySyncData>(new InventorySyncData(this.itemSlots, this.backpackSlot, this.tempFullSlot));
		this.view.RPC("SyncInventoryRPC", RpcTarget.Others, new object[] { array, false });
		return true;
	}

	// Token: 0x06000230 RID: 560 RVA: 0x0000FC48 File Offset: 0x0000DE48
	[PunRPC]
	public void SyncInventoryRPC(byte[] data, bool forceSync)
	{
		if (!forceSync && PhotonNetwork.IsMasterClient)
		{
			Debug.LogError("SyncInventoryRPC should not sync to Master client. They are the boss");
			return;
		}
		InventorySyncData fromManagedArray = IBinarySerializable.GetFromManagedArray<InventorySyncData>(data);
		byte b = 0;
		while ((int)b < this.itemSlots.Length)
		{
			Item item;
			this.itemSlots[(int)b].prefab = (ItemDatabase.TryGetItem(fromManagedArray.slots[(int)b].ItemID, out item) ? item : null);
			this.itemSlots[(int)b].data = fromManagedArray.slots[(int)b].Data;
			Debug.Log(string.Format("Sync Inventory on {0} is setting slot: {1} to {2}", base.name, b, this.itemSlots[(int)b].ToString()));
			b += 1;
		}
		Debug.Log(string.Format("Sync Inventory on {0} is setting backpack: {1}", base.name, fromManagedArray.hasBackpack));
		this.backpackSlot.hasBackpack = fromManagedArray.hasBackpack;
		this.backpackSlot.data = fromManagedArray.backpackInstanceData;
		Item item2;
		this.tempFullSlot.prefab = (ItemDatabase.TryGetItem(fromManagedArray.tempSlot.ItemID, out item2) ? item2 : null);
		this.tempFullSlot.data = fromManagedArray.tempSlot.Data;
		if (this.view.IsMine)
		{
			this.character.refs.items.RefreshAllCharacterCarryWeightRPC();
		}
	}

	// Token: 0x06000231 RID: 561 RVA: 0x0000FD98 File Offset: 0x0000DF98
	[PunRPC]
	public void RPCRemoveItemFromSlot(byte slotID)
	{
		if (!PhotonNetwork.IsMasterClient)
		{
			Debug.LogError("Only Master Client can remove items!");
			return;
		}
		this.GetItemSlot(slotID).EmptyOut();
		InventorySyncData inventorySyncData = new InventorySyncData(this.itemSlots, this.backpackSlot, this.tempFullSlot);
		this.view.RPC("SyncInventoryRPC", RpcTarget.Others, new object[]
		{
			IBinarySerializable.ToManagedArray<InventorySyncData>(inventorySyncData),
			false
		});
	}

	// Token: 0x06000232 RID: 562 RVA: 0x0000FE08 File Offset: 0x0000E008
	public void EmptySlot(Optionable<byte> slot)
	{
		if (slot.IsNone)
		{
			Debug.LogError("Can't empty none slot");
			return;
		}
		byte value = slot.Value;
		this.GetItemSlot(value).EmptyOut();
		if (PhotonNetwork.IsMasterClient)
		{
			InventorySyncData inventorySyncData = new InventorySyncData(this.itemSlots, this.backpackSlot, this.tempFullSlot);
			this.view.RPC("SyncInventoryRPC", RpcTarget.Others, new object[]
			{
				IBinarySerializable.ToManagedArray<InventorySyncData>(inventorySyncData),
				false
			});
			return;
		}
		this.view.RPC("RPCRemoveItemFromSlot", RpcTarget.MasterClient, new object[] { value });
	}

	// Token: 0x06000233 RID: 563 RVA: 0x0000FEA8 File Offset: 0x0000E0A8
	public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
	{
		base.OnPlayerEnteredRoom(newPlayer);
		InventorySyncData inventorySyncData = new InventorySyncData(this.itemSlots, this.backpackSlot, this.tempFullSlot);
		this.view.RPC("SyncInventoryRPC", newPlayer, new object[]
		{
			IBinarySerializable.ToManagedArray<InventorySyncData>(inventorySyncData),
			false
		});
	}

	// Token: 0x06000234 RID: 564 RVA: 0x0000FEFE File Offset: 0x0000E0FE
	[PunRPC]
	public void RPC_SetInventory(byte[] newInventory)
	{
	}

	// Token: 0x06000235 RID: 565 RVA: 0x0000FF00 File Offset: 0x0000E100
	public ItemSlot GetItemSlot(byte slotID)
	{
		if (slotID == 3)
		{
			return this.backpackSlot;
		}
		if (slotID == 250)
		{
			return this.tempFullSlot;
		}
		return this.itemSlots[(int)slotID];
	}

	// Token: 0x06000236 RID: 566 RVA: 0x0000FF24 File Offset: 0x0000E124
	public bool HasEmptySlot(ushort itemID)
	{
		Item item;
		if (!ItemDatabase.TryGetItem(itemID, out item))
		{
			Debug.LogError(string.Format("Failed to get item from item ID: {0}", itemID));
			return false;
		}
		if (item is Backpack)
		{
			return this.backpackSlot.IsEmpty();
		}
		ItemSlot[] array = this.itemSlots;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].IsEmpty())
			{
				return true;
			}
		}
		return this.tempFullSlot.IsEmpty();
	}

	// Token: 0x06000237 RID: 567 RVA: 0x0000FF92 File Offset: 0x0000E192
	[ContextMenu("Debug Print Player ID")]
	private void DebugPrintPlayerID()
	{
		Debug.Log(base.photonView.Owner.ActorNumber);
	}

	// Token: 0x06000238 RID: 568 RVA: 0x0000FFB0 File Offset: 0x0000E1B0
	public bool HasInAnySlot(ushort itemID, out byte slotID)
	{
		foreach (ItemSlot itemSlot in this.itemSlots)
		{
			if (!itemSlot.IsEmpty() && itemSlot.prefab.itemID == itemID)
			{
				slotID = itemSlot.itemSlotID;
				return true;
			}
		}
		slotID = 0;
		return false;
	}

	// Token: 0x0600023A RID: 570 RVA: 0x00010010 File Offset: 0x0000E210
	[CompilerGenerated]
	private ItemSlot <AddItem>g__AddToSlot|13_0(ref global::Player.<>c__DisplayClass13_0 A_1)
	{
		if (A_1.ItemPrefab is Backpack)
		{
			if (this.backpackSlot.IsEmpty())
			{
				this.backpackSlot.hasBackpack = true;
				this.backpackSlot.data = A_1.instanceData;
				return this.backpackSlot;
			}
			return null;
		}
		else
		{
			for (int i = 0; i < this.itemSlots.Length; i++)
			{
				if (this.itemSlots[i].IsEmpty())
				{
					this.itemSlots[i].SetItem(A_1.ItemPrefab, A_1.instanceData);
					return this.itemSlots[i];
				}
			}
			if (this.tempFullSlot.IsEmpty() && !this.character.data.isClimbingAnything)
			{
				this.tempFullSlot.SetItem(A_1.ItemPrefab, A_1.instanceData);
				return this.tempFullSlot;
			}
			return null;
		}
	}

	// Token: 0x0400021A RID: 538
	public const int BACKPACKSLOTINDEX = 3;

	// Token: 0x0400021B RID: 539
	public ItemSlot[] itemSlots = new ItemSlot[3];

	// Token: 0x0400021C RID: 540
	public ItemSlot tempFullSlot;

	// Token: 0x0400021D RID: 541
	public BackpackSlot backpackSlot;

	// Token: 0x0400021E RID: 542
	public Action<int> hotbarSelectionChanged;

	// Token: 0x0400021F RID: 543
	public Action<ItemSlot[]> itemsChangedAction;

	// Token: 0x04000220 RID: 544
	public static global::Player localPlayer;

	// Token: 0x04000221 RID: 545
	public bool hasClosedEndScreen;

	// Token: 0x04000222 RID: 546
	public bool doneWithCutscene;

	// Token: 0x04000223 RID: 547
	private PhotonView view;
}
