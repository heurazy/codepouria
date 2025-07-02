using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Zorro.Core;

// Token: 0x0200000D RID: 13
public class CharacterItems : MonoBehaviourPunCallbacks
{
	// Token: 0x060000FB RID: 251 RVA: 0x00007DEA File Offset: 0x00005FEA
	private IEnumerator SubscribeRoutine(bool subscribe)
	{
		while (!this.character.player)
		{
			yield return null;
		}
		if (subscribe)
		{
			global::Player player = this.character.player;
			player.itemsChangedAction = (Action<ItemSlot[]>)Delegate.Combine(player.itemsChangedAction, new Action<ItemSlot[]>(this.UpdateClimbingSpikeCount));
		}
		else
		{
			global::Player player2 = this.character.player;
			player2.itemsChangedAction = (Action<ItemSlot[]>)Delegate.Remove(player2.itemsChangedAction, new Action<ItemSlot[]>(this.UpdateClimbingSpikeCount));
		}
		yield break;
	}

	// Token: 0x060000FC RID: 252 RVA: 0x00007E00 File Offset: 0x00006000
	private void Awake()
	{
		this.character = base.GetComponent<Character>();
		this.photonView = base.GetComponent<PhotonView>();
		this.currentSelectedSlot = Optionable<byte>.None;
		this.lastSelectedSlot = Optionable<byte>.Some(0);
		base.StartCoroutine(this.SubscribeRoutine(true));
	}

	// Token: 0x060000FD RID: 253 RVA: 0x00007E3F File Offset: 0x0000603F
	private void OnDestroy()
	{
		if (this.character.player)
		{
			global::Player player = this.character.player;
			player.itemsChangedAction = (Action<ItemSlot[]>)Delegate.Remove(player.itemsChangedAction, new Action<ItemSlot[]>(this.UpdateClimbingSpikeCount));
		}
	}

	// Token: 0x060000FE RID: 254 RVA: 0x00007E7F File Offset: 0x0000607F
	private void FixedUpdate()
	{
		if (this.character.data.currentItem)
		{
			this.HoldItem(this.character.data.currentItem);
		}
	}

	// Token: 0x060000FF RID: 255 RVA: 0x00007EB0 File Offset: 0x000060B0
	private void HoldItem(Item item)
	{
		Vector3 vector = this.GetItemHoldPos(item) - item.transform.position;
		item.rig.AddForce(vector * this.holdForce, ForceMode.Acceleration);
		Vector3 itemHoldForward = this.GetItemHoldForward(item);
		Vector3 itemHoldUp = this.GetItemHoldUp(item);
		Vector3 vector2 = Vector3.Cross(item.transform.forward, itemHoldForward).normalized * Vector3.Angle(item.transform.forward, itemHoldForward);
		vector2 += Vector3.Cross(item.transform.up, itemHoldUp).normalized * Vector3.Angle(item.transform.up, itemHoldUp);
		item.rig.AddTorque(vector2 * this.holdTorque, ForceMode.Acceleration);
	}

	// Token: 0x06000100 RID: 256 RVA: 0x00007F7D File Offset: 0x0000617D
	private void Update()
	{
		this.DoSwitching();
		this.DoDropping();
		this.DoUsing();
		this.UpdateClimbingSpikeUse();
	}

	// Token: 0x06000101 RID: 257 RVA: 0x00007F98 File Offset: 0x00006198
	private void DoUsing()
	{
		if (!this.character.data.currentItem)
		{
			return;
		}
		if (this.character.data.passedOut || this.character.data.fullyPassedOut)
		{
			return;
		}
		if (this.character.input.usePrimaryWasPressed && this.character.data.currentItem.CanUsePrimary())
		{
			this.character.data.currentItem.StartUsePrimary();
		}
		if (this.character.input.usePrimaryIsPressed && this.character.data.currentItem.CanUsePrimary())
		{
			this.character.data.currentItem.ContinueUsePrimary();
		}
		if (this.character.input.usePrimaryWasReleased || (this.character.data.currentItem.isUsingPrimary && !this.character.data.currentItem.CanUsePrimary()))
		{
			this.character.data.currentItem.CancelUsePrimary();
		}
		if (!this.character.CanDoInput())
		{
			this.character.data.currentItem.CancelUsePrimary();
		}
		if (this.character.input.useSecondaryIsPressed && this.character.data.currentItem.CanUseSecondary())
		{
			this.character.data.currentItem.StartUseSecondary();
		}
		if (this.character.input.useSecondaryIsPressed && this.character.data.currentItem.CanUseSecondary())
		{
			this.character.data.currentItem.ContinueUseSecondary();
		}
		if (this.character.input.useSecondaryWasReleased || (this.character.data.currentItem.isUsingSecondary && !this.character.data.currentItem.CanUseSecondary()))
		{
			this.character.data.currentItem.CancelUseSecondary();
		}
		if (this.character.input.scrollButtonLeftWasPressed)
		{
			this.character.data.currentItem.ScrollButtonLeft();
		}
		if (this.character.input.scrollButtonRightWasPressed)
		{
			this.character.data.currentItem.ScrollButtonRight();
		}
		if (this.character.input.scrollInput != 0f)
		{
			this.character.data.currentItem.Scroll(this.character.input.scrollInput);
		}
	}

	// Token: 0x06000102 RID: 258 RVA: 0x00008230 File Offset: 0x00006430
	private void DoDropping()
	{
		if (this.character.input.dropWasPressed && this.character.data.currentItem && this.character.data.currentItem.UIData.canDrop)
		{
			this.lastPressedDrop = Time.time;
			this.pressedDrop = true;
		}
		if (this.pressedDrop && this.character.input.dropWasReleased && this.character.data.currentItem && this.currentSelectedSlot.IsSome)
		{
			Vector3 vector = this.character.data.currentItem.transform.position;
			Vector3 vector2 = this.character.data.currentItem.rig.linearVelocity;
			if (this.character.data.currentItem is Backpack)
			{
				vector += MainCamera.instance.transform.forward * 0.5f;
				vector2 = Vector3.zero;
			}
			if (this.throwChargeLevel > 0.1f && base.transform.GetComponent<CharacterAnimations>())
			{
				base.transform.GetComponent<CharacterAnimations>().throwTime = 0.125f;
			}
			ItemSlot itemSlot = this.character.player.GetItemSlot(this.currentSelectedSlot.Value);
			this.photonView.RPC("DropItemRpc", RpcTarget.All, new object[]
			{
				this.throwChargeLevel,
				this.currentSelectedSlot.Value,
				vector,
				vector2,
				this.character.data.currentItem.transform.rotation,
				itemSlot.data
			});
			this.throwChargeLevel = 0f;
			this.EquipSlot(Optionable<byte>.None);
		}
		if (this.pressedDrop && this.character.input.dropIsPressed && Time.time - this.lastPressedDrop > this.delayBeforeThrowCharge)
		{
			this.throwChargeLevel = Mathf.Min(this.throwChargeLevel + 1f / this.throwChargeTime * Time.deltaTime, 1f);
			return;
		}
		this.throwChargeLevel = 0f;
	}

	// Token: 0x06000103 RID: 259 RVA: 0x00008490 File Offset: 0x00006690
	internal void DropAllItems(bool includeBackpack)
	{
		if (!this.character.IsLocal)
		{
			return;
		}
		Transform transform = this.character.GetBodypart(BodypartType.Hip).transform;
		Vector3 vector = transform.position + transform.forward * 0.6f;
		if (this.currentSelectedSlot.IsSome && this.character.data.currentItem)
		{
			ItemSlot itemSlot = this.character.player.GetItemSlot(this.currentSelectedSlot.Value);
			this.photonView.RPC("DropItemRpc", RpcTarget.All, new object[]
			{
				this.throwChargeLevel,
				this.currentSelectedSlot.Value,
				this.character.data.currentItem.transform.position,
				Vector3.zero,
				this.character.data.currentItem.transform.rotation,
				itemSlot.data
			});
			vector += Vector3.up * 0.5f;
		}
		for (int i = (includeBackpack ? 3 : 2); i >= 0; i--)
		{
			this.photonView.RPC("DropItemFromSlotRPC", RpcTarget.All, new object[]
			{
				(byte)i,
				vector
			});
			vector += Vector3.up * 0.5f;
		}
	}

	// Token: 0x06000104 RID: 260 RVA: 0x0000861C File Offset: 0x0000681C
	[PunRPC]
	internal void DropItemFromSlotRPC(byte slotID, Vector3 spawnPosition)
	{
		Debug.Log("Trying to empty slot " + slotID.ToString());
		ItemSlot itemSlot = this.character.player.GetItemSlot(slotID);
		if (!itemSlot.IsEmpty())
		{
			if (PhotonNetwork.IsMasterClient)
			{
				PhotonNetwork.Instantiate("0_Items/" + itemSlot.GetPrefabName(), spawnPosition, Quaternion.identity, 0, null).GetComponent<PhotonView>().RPC("SetItemInstanceDataRPC", RpcTarget.All, new object[] { itemSlot.data });
			}
			this.character.player.EmptySlot(Optionable<byte>.Some(slotID));
		}
	}

	// Token: 0x06000105 RID: 261 RVA: 0x000086B4 File Offset: 0x000068B4
	[PunRPC]
	public void DestroyHeldItemRpc()
	{
		Item currentItem = this.character.data.currentItem;
		if (currentItem == null)
		{
			return;
		}
		this.UnAttatchEquipedItem();
		if (currentItem.photonView.IsMine || (currentItem.photonView.Controller.IsMasterClient && PhotonNetwork.IsMasterClient))
		{
			PhotonNetwork.Destroy(currentItem.gameObject);
		}
	}

	// Token: 0x06000106 RID: 262 RVA: 0x00008714 File Offset: 0x00006914
	[PunRPC]
	public void DropItemRpc(float throwCharge, byte slotID, Vector3 spawnPos, Vector3 velocity, Quaternion rotation, ItemInstanceData itemInstanceData)
	{
		if (!this.character.data.currentItem)
		{
			return;
		}
		float num = 0f;
		if (throwCharge > 0f)
		{
			num = this.minThrowForce + (this.maxThrowForce - this.minThrowForce) * throwCharge;
		}
		Item currentItem = this.character.data.currentItem;
		this.UnAttatchEquipedItem();
		if (currentItem.photonView.IsMine || (currentItem.photonView.Controller.IsMasterClient && PhotonNetwork.IsMasterClient))
		{
			PhotonNetwork.Destroy(currentItem.gameObject);
		}
		ItemSlot itemSlot = this.character.player.GetItemSlot(slotID);
		if (PhotonNetwork.IsMasterClient)
		{
			Debug.Log(string.Format("Dropping slot: {0}", slotID));
			Vector3 normalized = HelperFunctions.LookToDirection(this.character.data.lookValues, Vector3.forward).normalized;
			PhotonView component = PhotonNetwork.InstantiateItemRoom(itemSlot.GetPrefabName(), spawnPos, rotation).GetComponent<PhotonView>();
			GameUtils.instance.IgnoreCollisions(this.character.gameObject, component.gameObject, 0.5f);
			Rigidbody component2 = component.GetComponent<Rigidbody>();
			component.RPC("SetKinematicRPC", RpcTarget.AllBuffered, new object[]
			{
				false,
				component.transform.position,
				component.transform.rotation
			});
			component2.linearVelocity = velocity + normalized * num * 0.5f;
			component2.angularVelocity = Vector3.Cross(normalized, Vector3.up) * num * 0.5f;
			component.RPC("SetItemInstanceDataRPC", RpcTarget.All, new object[] { itemInstanceData });
			Debug.Log(string.Format("Setting force: {0} m/s", component2.linearVelocity.magnitude));
		}
		this.character.player.EmptySlot(Optionable<byte>.Some(slotID));
		this.pressedDrop = false;
	}

	// Token: 0x06000107 RID: 263 RVA: 0x00008920 File Offset: 0x00006B20
	[PunRPC]
	public void OnPickupAccepted(byte slotID)
	{
		if (slotID != 3)
		{
			if (!this.character.data.isClimbingAnything)
			{
				this.character.refs.items.EquipSlot(Optionable<byte>.Some(slotID));
			}
		}
		else if (this.character.data.carriedPlayer != null)
		{
			this.character.refs.carriying.Drop(this.character.data.carriedPlayer);
		}
		this.RefreshAllCharacterCarryWeight();
	}

	// Token: 0x06000108 RID: 264 RVA: 0x000089A3 File Offset: 0x00006BA3
	public void RefreshAllCharacterCarryWeight()
	{
		this.photonView.RPC("RefreshAllCharacterCarryWeightRPC", RpcTarget.All, Array.Empty<object>());
	}

	// Token: 0x06000109 RID: 265 RVA: 0x000089BC File Offset: 0x00006BBC
	[PunRPC]
	public void RefreshAllCharacterCarryWeightRPC()
	{
		Debug.Log("Starting weight update.");
		List<Character> allPlayerCharacters = PlayerHandler.GetAllPlayerCharacters();
		for (int i = 0; i < allPlayerCharacters.Count; i++)
		{
			Debug.Log("Updating weight for " + allPlayerCharacters[i].gameObject.name + "...");
			allPlayerCharacters[i].refs.afflictions.UpdateWeight();
		}
	}

	// Token: 0x0600010A RID: 266 RVA: 0x00008A28 File Offset: 0x00006C28
	public void EquipSlot(Optionable<byte> slotID)
	{
		CharacterItems.<>c__DisplayClass30_0 CS$<>8__locals1 = new CharacterItems.<>c__DisplayClass30_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.slotID = slotID;
		this.lastEquippedSlotTime = Time.time;
		CS$<>8__locals1.waitForFrames = false;
		if (CS$<>8__locals1.slotID.IsSome)
		{
			this.lastSelectedSlot = CS$<>8__locals1.slotID;
		}
		if (this.photonView.IsMine && this.character.data.currentItem != null)
		{
			this.character.data.currentItem.CancelUsePrimary();
			this.character.data.currentItem.CancelUseSecondary();
			if (!this.character.data.currentItem.UIData.canPocket || (this.currentSelectedSlot.IsSome && this.currentSelectedSlot.Value == 250 && !this.character.player.GetItemSlot(this.currentSelectedSlot.Value).IsEmpty()))
			{
				Vector3 vector = this.character.data.currentItem.transform.position + Vector3.down * 0.2f;
				Vector3 linearVelocity = this.character.data.currentItem.rig.linearVelocity;
				CS$<>8__locals1.waitForFrames = true;
				ItemSlot itemSlot = this.character.player.GetItemSlot(this.currentSelectedSlot.Value);
				this.photonView.RPC("DropItemRpc", RpcTarget.All, new object[]
				{
					this.throwChargeLevel,
					this.currentSelectedSlot.Value,
					vector,
					linearVelocity,
					this.character.data.currentItem.transform.rotation,
					itemSlot.data
				});
			}
		}
		base.StartCoroutine(CS$<>8__locals1.<EquipSlot>g__TheRest|0());
	}

	// Token: 0x0600010B RID: 267 RVA: 0x00008C1E File Offset: 0x00006E1E
	[PunRPC]
	public void EquipRemoteSlot(byte slotID)
	{
		this.EquipSlot(Optionable<byte>.Some(slotID));
	}

	// Token: 0x0600010C RID: 268 RVA: 0x00008C2C File Offset: 0x00006E2C
	[PunRPC]
	public void EquipSlotRpc(int slotID, int objectViewID)
	{
		if (!this.photonView.IsMine)
		{
			if (slotID == -1)
			{
				if (this.currentSelectedSlot.IsSome)
				{
					this.lastSelectedSlot = this.currentSelectedSlot;
				}
				this.currentSelectedSlot = Optionable<byte>.None;
			}
			else
			{
				this.currentSelectedSlot = Optionable<byte>.Some((byte)slotID);
			}
		}
		PhotonView photonView = null;
		if (objectViewID != -1)
		{
			photonView = PhotonNetwork.GetPhotonView(objectViewID);
			Debug.Log(string.Format("{0} is equipping {1} in slot {2}", this.character.gameObject.name, photonView.name, slotID));
		}
		else
		{
			Debug.Log(string.Format("{0} is equipping nothing in slot {1}", this.character.gameObject.name, slotID));
		}
		Item item;
		if (photonView != null)
		{
			item = this.Equip(photonView.GetComponent<Item>());
		}
		else
		{
			item = this.Equip(null);
		}
		if (this.photonView.IsMine && item != null)
		{
			item.OnStash();
			Debug.Log(this.character.gameObject.name + " destroying " + item.gameObject.name);
			PhotonNetwork.Destroy(item.GetComponent<PhotonView>());
		}
		if (this.character.player.itemsChangedAction != null)
		{
			this.character.player.itemsChangedAction(this.character.player.itemSlots);
		}
		Action action = this.onSlotEquipped;
		if (action == null)
		{
			return;
		}
		action();
	}

	// Token: 0x0600010D RID: 269 RVA: 0x00008D98 File Offset: 0x00006F98
	public Item Equip(Item item)
	{
		CharacterItems.<>c__DisplayClass34_0 CS$<>8__locals1 = new CharacterItems.<>c__DisplayClass34_0();
		CS$<>8__locals1.item = item;
		CS$<>8__locals1.<>4__this = this;
		Item currentItem = this.character.data.currentItem;
		this.pressedDrop = false;
		if (this.character.data.currentItem)
		{
			this.UnAttatchEquipedItem();
		}
		if (CS$<>8__locals1.item == null)
		{
			return currentItem;
		}
		this.character.data.currentItem = CS$<>8__locals1.item;
		CS$<>8__locals1.item.holderCharacter = this.character;
		CS$<>8__locals1.item.SetState(ItemState.Held, this.character);
		Debug.Log(string.Format("Equping {0} and starting attach coroutine..", CS$<>8__locals1.item));
		base.StartCoroutine(CS$<>8__locals1.<Equip>g__IWait|0());
		return currentItem;
	}

	// Token: 0x0600010E RID: 270 RVA: 0x00008E5C File Offset: 0x0000705C
	private void AttachItem(Item item)
	{
		this.character.GetBodypartRig(BodypartType.Hand_R).transform.position = this.GetItemPosRightWorld(item);
		this.character.GetBodypartRig(BodypartType.Hand_L).transform.position = this.GetItemPosLeftWorld(item);
		this.character.GetBodypartRig(BodypartType.Hand_R).transform.rotation = this.GetItemRotRightWorld(item);
		this.character.GetBodypartRig(BodypartType.Hand_L).transform.rotation = this.GetItemRotLeftWorld(item);
		Debug.Log("Attatching Fixed Joint to grab " + item.name);
		this.character.GetBodypartRig(BodypartType.Hand_R).gameObject.AddComponent<FixedJoint>().connectedBody = item.rig;
		this.character.GetBodypartRig(BodypartType.Hand_L).gameObject.AddComponent<FixedJoint>().connectedBody = item.rig;
	}

	// Token: 0x0600010F RID: 271 RVA: 0x00008F37 File Offset: 0x00007137
	private void UnAttachItem()
	{
		Object.Destroy(this.character.GetBodypartRig(BodypartType.Hand_R).gameObject.GetComponent<FixedJoint>());
		Object.Destroy(this.character.GetBodypartRig(BodypartType.Hand_L).gameObject.GetComponent<FixedJoint>());
	}

	// Token: 0x06000110 RID: 272 RVA: 0x00008F70 File Offset: 0x00007170
	private Quaternion GetItemHoldRotation(Item item)
	{
		return Quaternion.LookRotation(this.GetItemHoldForward(item), this.GetItemHoldUp(item));
	}

	// Token: 0x06000111 RID: 273 RVA: 0x00008F85 File Offset: 0x00007185
	private Vector3 GetItemHoldUp(Item item)
	{
		return this.character.data.lookDirection_Up;
	}

	// Token: 0x06000112 RID: 274 RVA: 0x00008F97 File Offset: 0x00007197
	private Vector3 GetItemHoldForward(Item item)
	{
		return this.character.data.lookDirection;
	}

	// Token: 0x06000113 RID: 275 RVA: 0x00008FAC File Offset: 0x000071AC
	public Vector3 GetItemHoldPos(Item item)
	{
		Vector3 vector = this.character.refs.animationItemTransform.position - this.character.refs.animationHipTransform.position;
		return this.character.refs.hip.transform.position + vector;
	}

	// Token: 0x06000114 RID: 276 RVA: 0x00009009 File Offset: 0x00007209
	public void UnAttatchEquipedItem()
	{
		this.UnAttachItem();
		this.character.data.currentItem = null;
	}

	// Token: 0x17000013 RID: 19
	// (get) Token: 0x06000115 RID: 277 RVA: 0x00009022 File Offset: 0x00007222
	private float equippedSlotCooldown
	{
		get
		{
			if (this.timesSwitchedRecently >= 3)
			{
				return 0.25f;
			}
			return 0f;
		}
	}

	// Token: 0x06000116 RID: 278 RVA: 0x00009038 File Offset: 0x00007238
	private void DoSwitching()
	{
		if (this.timesSwitchedRecently > 0 && this.lastSwitched + 0.4f < Time.time)
		{
			this.timesSwitchedRecently = 0;
		}
		if (this.character.IsLocal && this.character.CanDoInput() && Time.time > this.lastEquippedSlotTime + this.equippedSlotCooldown)
		{
			if (this.character.data.isClimbing || this.character.data.isRopeClimbing)
			{
				return;
			}
			if (this.character.input.selectSlotForwardWasPressed)
			{
				bool flag = !this.character.player.GetItemSlot(3).IsEmpty();
				byte b = decimal.ToByte((int)(this.lastSelectedSlot.Value + 1));
				if (!this.character.player.itemSlots.WithinRange((int)b) && (b != 3 || !flag))
				{
					b = 0;
				}
				this.lastSwitched = Time.time;
				this.timesSwitchedRecently++;
				this.EquipSlot(Optionable<byte>.Some(b));
			}
			else if (this.character.input.selectSlotBackwardWasPressed)
			{
				bool flag2 = !this.character.player.GetItemSlot(3).IsEmpty();
				int num = (int)(this.lastSelectedSlot.Value - 1);
				if (num < 0)
				{
					if (flag2)
					{
						num = (int)decimal.ToByte(3m);
					}
					else
					{
						num = this.character.player.itemSlots.Length - 1;
					}
				}
				this.lastSwitched = Time.time;
				this.timesSwitchedRecently++;
				this.EquipSlot(Optionable<byte>.Some(decimal.ToByte(num)));
			}
			else if (this.character.input.unselectSlotWasPressed)
			{
				if (this.currentSelectedSlot.IsSome)
				{
					this.lastSwitched = Time.time;
					this.timesSwitchedRecently++;
					this.EquipSlot(Optionable<byte>.None);
				}
				else
				{
					this.lastSwitched = Time.time;
					this.timesSwitchedRecently++;
					this.EquipSlot(this.lastSelectedSlot);
				}
			}
			for (byte b2 = 0; b2 <= 3; b2 += 1)
			{
				if (this.character.input.SelectSlotWasPressed((int)b2))
				{
					if (!this.character.player.itemSlots.WithinRange((int)b2) && b2 != 3)
					{
						this.lastSwitched = Time.time;
						this.timesSwitchedRecently++;
						this.EquipSlot(Optionable<byte>.None);
					}
					else if (this.currentSelectedSlot.IsSome && this.currentSelectedSlot.Value == b2)
					{
						this.lastSwitched = Time.time;
						this.timesSwitchedRecently++;
						this.EquipSlot(Optionable<byte>.None);
					}
					else
					{
						this.lastSwitched = Time.time;
						this.timesSwitchedRecently++;
						this.EquipSlot(Optionable<byte>.Some(b2));
					}
				}
			}
		}
	}

	// Token: 0x06000117 RID: 279 RVA: 0x0000932F File Offset: 0x0000752F
	internal void AddGravity(Vector3 gravity)
	{
		this.character.data.currentItem.rig.AddForce(gravity, ForceMode.Acceleration);
	}

	// Token: 0x06000118 RID: 280 RVA: 0x0000934D File Offset: 0x0000754D
	internal void AddMovementForce(float movementForce)
	{
		this.character.data.currentItem.rig.AddForce(movementForce * this.character.data.worldMovementInput_Grounded, ForceMode.Acceleration);
	}

	// Token: 0x06000119 RID: 281 RVA: 0x00009380 File Offset: 0x00007580
	internal void AddDrag(float drag, float factor = 1f)
	{
		drag = Mathf.Lerp(1f, drag, factor);
		this.character.data.currentItem.rig.linearVelocity *= drag;
		this.character.data.currentItem.rig.angularVelocity *= drag;
	}

	// Token: 0x0600011A RID: 282 RVA: 0x000093E7 File Offset: 0x000075E7
	internal Vector3 GetItemPosRightWorld(Item item)
	{
		return item.transform.Find("Hand_R").position;
	}

	// Token: 0x0600011B RID: 283 RVA: 0x000093FE File Offset: 0x000075FE
	internal Vector3 GetItemPosLeftWorld(Item item)
	{
		return item.transform.Find("Hand_L").position;
	}

	// Token: 0x0600011C RID: 284 RVA: 0x00009415 File Offset: 0x00007615
	internal Quaternion GetItemRotRightWorld(Item item)
	{
		return item.transform.Find("Hand_R").rotation;
	}

	// Token: 0x0600011D RID: 285 RVA: 0x0000942C File Offset: 0x0000762C
	internal Quaternion GetItemRotLeftWorld(Item item)
	{
		return item.transform.Find("Hand_L").rotation;
	}

	// Token: 0x0600011E RID: 286 RVA: 0x00009444 File Offset: 0x00007644
	internal Vector3 GetItemPosRight(Item item)
	{
		Vector3 localPosition = item.transform.Find("Hand_R").localPosition;
		return this.character.refs.animationItemTransform.TransformPoint(localPosition);
	}

	// Token: 0x0600011F RID: 287 RVA: 0x00009480 File Offset: 0x00007680
	internal Quaternion GetItemRotRight(Item item)
	{
		Transform transform = item.transform.Find("Hand_R");
		Vector3 vector = item.transform.InverseTransformDirection(transform.forward);
		Vector3 vector2 = item.transform.InverseTransformDirection(transform.up);
		Vector3 vector3 = this.character.refs.animationItemTransform.TransformDirection(vector);
		Vector3 vector4 = this.character.refs.animationItemTransform.TransformDirection(vector2);
		return Quaternion.LookRotation(vector3, vector4);
	}

	// Token: 0x06000120 RID: 288 RVA: 0x000094F8 File Offset: 0x000076F8
	internal Quaternion GetItemRotLeft(Item item)
	{
		Transform transform = item.transform.Find("Hand_L");
		Vector3 vector = item.transform.InverseTransformDirection(transform.forward);
		Vector3 vector2 = item.transform.InverseTransformDirection(transform.up);
		Vector3 vector3 = this.character.refs.animationItemTransform.TransformDirection(vector);
		Vector3 vector4 = this.character.refs.animationItemTransform.TransformDirection(vector2);
		return Quaternion.LookRotation(vector3, vector4);
	}

	// Token: 0x06000121 RID: 289 RVA: 0x00009570 File Offset: 0x00007770
	internal Vector3 GetItemPosLeft(Item item)
	{
		Vector3 vector = HelperFunctions.MultiplyVectors(item.transform.Find("Hand_L").localPosition, item.transform.lossyScale);
		return this.character.refs.animationItemTransform.TransformPoint(vector);
	}

	// Token: 0x06000122 RID: 290 RVA: 0x000095BC File Offset: 0x000077BC
	public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
	{
		base.OnPlayerEnteredRoom(newPlayer);
		if (PhotonNetwork.IsMasterClient && this.character.data.currentItem != null)
		{
			Debug.Log("Setting " + base.gameObject.name + " to hold " + this.character.data.currentItem.name);
			this.photonView.RPC("RPC_InitHoldingItem", newPlayer, new object[] { this.character.data.currentItem.GetComponent<PhotonView>() });
		}
	}

	// Token: 0x06000123 RID: 291 RVA: 0x00009652 File Offset: 0x00007852
	[PunRPC]
	public void RPC_InitHoldingItem(PhotonView item)
	{
		Debug.Log("Init holding item: " + item.name);
		this.Equip(item.GetComponent<Item>());
	}

	// Token: 0x06000124 RID: 292 RVA: 0x00009678 File Offset: 0x00007878
	public void UpdateClimbingSpikeCount(ItemSlot[] slots)
	{
		int num = 0;
		this.currentClimbingSpikeComponent = null;
		this.currentClimbingSpikeItemSlot = null;
		foreach (ItemSlot itemSlot in slots)
		{
			if (itemSlot != null && itemSlot.prefab != null)
			{
				ClimbingSpikeComponent component = itemSlot.prefab.GetComponent<ClimbingSpikeComponent>();
				IntItemData intItemData;
				if (component != null && (!itemSlot.data.TryGetDataEntry<IntItemData>(DataEntryKey.CookedAmount, out intItemData) || intItemData.Value <= 0))
				{
					num++;
					if (this.currentClimbingSpikeComponent == null)
					{
						this.currentClimbingSpikeComponent = component;
						this.currentClimbingSpikeItemSlot = itemSlot;
					}
				}
			}
		}
		this.character.data.climbingSpikeCount = num;
	}

	// Token: 0x06000125 RID: 293 RVA: 0x00009718 File Offset: 0x00007918
	private bool WithinClimbingSpikePreviewRange()
	{
		if (this.currentClimbingSpikePreview)
		{
			float num = (this.character.data.isClimbingAnything ? this.currentClimbingSpikeComponent.climbingSpikePreviewDisableDistance : this.currentClimbingSpikeComponent.climbingSpikePreviewDisableDistanceGrounded);
			return Vector3.Distance(MainCamera.instance.transform.position, this.currentClimbingSpikePreview.transform.position) <= num;
		}
		return false;
	}

	// Token: 0x17000014 RID: 20
	// (get) Token: 0x06000126 RID: 294 RVA: 0x00009789 File Offset: 0x00007989
	public float climbingSpikeCastProgress
	{
		get
		{
			if (this.currentClimbingSpikeItemSlot == null)
			{
				return 0f;
			}
			if (this.currentClimbingSpikeItemSlot.prefab == null)
			{
				return 0f;
			}
			return this.climbingSpikeTick / this.currentClimbingSpikeItemSlot.prefab.usingTimePrimary;
		}
	}

	// Token: 0x06000127 RID: 295 RVA: 0x000097CC File Offset: 0x000079CC
	private void UpdateClimbingSpikeUse()
	{
		if (this.character.data.climbingSpikeCount <= 0 || this.currentClimbingSpikeItemSlot == null)
		{
			this.CancelClimbingSpike();
			return;
		}
		if (this.climbingSpikeTick > 0f)
		{
			if (!this.WithinClimbingSpikePreviewRange())
			{
				this.CancelClimbingSpike();
				return;
			}
			if ((this.spikingWithPrimary && !this.character.input.usePrimaryIsPressed) || (this.spikingWithSecondary && !this.character.input.useSecondaryIsPressed))
			{
				this.CancelClimbingSpike();
				return;
			}
			this.climbingSpikeTick += Time.deltaTime;
			if (this.climbingSpikeTick >= this.currentClimbingSpikeItemSlot.prefab.usingTimePrimary)
			{
				this.HammerClimbingSpike(this.climbingSpikeHit);
				this.CancelClimbingSpike();
			}
			return;
		}
		else
		{
			if (!this.RaycastClimbingSpikeStart())
			{
				this.climbingSpikeTick = 0f;
				return;
			}
			if (this.climbingSpikeTick == 0f)
			{
				if (this.character.input.usePrimaryIsPressed && !this.character.data.isClimbingAnything && this.climbingSpikeSelected)
				{
					this.spikingWithPrimary = true;
					this.spikingWithSecondary = false;
					this.climbingSpikeTick += Time.deltaTime;
					this.InstantiateClimbingSpikePreview(this.climbingSpikeHit);
					return;
				}
				if (this.character.input.useSecondaryIsPressed && (this.climbingSpikeSelected || this.character.data.isClimbingAnything))
				{
					this.spikingWithPrimary = false;
					this.spikingWithSecondary = true;
					this.climbingSpikeTick += Time.deltaTime;
					this.InstantiateClimbingSpikePreview(this.climbingSpikeHit);
				}
			}
			return;
		}
	}

	// Token: 0x17000015 RID: 21
	// (get) Token: 0x06000128 RID: 296 RVA: 0x00009967 File Offset: 0x00007B67
	private bool climbingSpikeSelected
	{
		get
		{
			return this.currentClimbingSpikeItemSlot != null && this.currentSelectedSlot.IsSome && this.currentSelectedSlot.Value == this.currentClimbingSpikeItemSlot.itemSlotID;
		}
	}

	// Token: 0x06000129 RID: 297 RVA: 0x00009998 File Offset: 0x00007B98
	private void CancelClimbingSpike()
	{
		if (this.currentClimbingSpikePreview)
		{
			Debug.Log("Cancelling climbing spike");
			Object.Destroy(this.currentClimbingSpikePreview);
		}
		this.climbingSpikeTick = 0f;
	}

	// Token: 0x0600012A RID: 298 RVA: 0x000099C8 File Offset: 0x00007BC8
	private void InstantiateClimbingSpikePreview(RaycastHit hit)
	{
		if (!this.currentClimbingSpikePreview && this.currentClimbingSpikeComponent != null)
		{
			this.currentClimbingSpikePreview = Object.Instantiate<GameObject>(this.currentClimbingSpikeComponent.climbingSpikePreviewPrefab);
		}
		if (this.currentClimbingSpikePreview)
		{
			this.currentClimbingSpikePreview.transform.position = this.climbingSpikeHit.point;
			this.currentClimbingSpikePreview.transform.rotation = Quaternion.LookRotation(-this.climbingSpikeHit.normal, Vector3.up);
		}
	}

	// Token: 0x0600012B RID: 299 RVA: 0x00009A58 File Offset: 0x00007C58
	public bool RaycastClimbingSpikeStart()
	{
		float num = (this.character.data.isClimbingAnything ? this.currentClimbingSpikeComponent.climbingSpikeStartDistance : this.currentClimbingSpikeComponent.climbingSpikeStartDistanceGrounded);
		return Physics.Raycast(MainCamera.instance.transform.position, MainCamera.instance.transform.forward, out this.climbingSpikeHit, num, HelperFunctions.GetMask(HelperFunctions.LayerType.TerrainMap));
	}

	// Token: 0x0600012C RID: 300 RVA: 0x00009AC8 File Offset: 0x00007CC8
	private void HammerClimbingSpike(RaycastHit hit)
	{
		if (this.currentClimbingSpikeComponent != null && PhotonNetwork.Instantiate("0_Items/" + this.currentClimbingSpikeComponent.hammeredVersionPrefab.gameObject.name, hit.point, Quaternion.LookRotation(-hit.normal, Vector3.up), 0, null) != null)
		{
			if (this.currentClimbingSpikeItemSlot != null)
			{
				ItemSlot itemSlot = this.currentClimbingSpikeItemSlot;
				this.currentClimbingSpikeItemSlot = null;
				this.currentClimbingSpikeComponent = null;
				this.character.player.EmptySlot(Optionable<byte>.Some(itemSlot.itemSlotID));
				if (this.character.data.currentItem != null)
				{
					this.EquipSlot(Optionable<byte>.None);
				}
				this.UpdateClimbingSpikeCount(this.character.player.itemSlots);
				this.character.data.lastConsumedItem = Time.time;
			}
			Singleton<AchievementManager>.Instance.IncrementSteamStat(STEAMSTATTYPE.PitonsPlaced, 1);
			GameUtils.instance.IncrementPermanentItemsPlaced();
		}
	}

	// Token: 0x0600012D RID: 301 RVA: 0x00009BD0 File Offset: 0x00007DD0
	internal void SpawnItemInHand(string objName)
	{
		this.photonView.RPC("RPC_SpawnItemInHandMaster", RpcTarget.MasterClient, new object[] { objName });
	}

	// Token: 0x0600012E RID: 302 RVA: 0x00009BF0 File Offset: 0x00007DF0
	[PunRPC]
	private void RPC_SpawnItemInHandMaster(string objName)
	{
		PhotonNetwork.Instantiate("0_Items/" + objName, this.character.Center + Vector3.up * 3f, Quaternion.identity, 0, null).GetComponent<Item>().Interact(this.character);
	}

	// Token: 0x04000116 RID: 278
	public SFX_Instance cookSfx;

	// Token: 0x04000117 RID: 279
	public float holdForce;

	// Token: 0x04000118 RID: 280
	public float holdTorque;

	// Token: 0x04000119 RID: 281
	public float throwChargeTime;

	// Token: 0x0400011A RID: 282
	public float minThrowForce;

	// Token: 0x0400011B RID: 283
	public float maxThrowForce;

	// Token: 0x0400011C RID: 284
	public float delayBeforeThrowCharge;

	// Token: 0x0400011D RID: 285
	[NonSerialized]
	public Optionable<byte> currentSelectedSlot;

	// Token: 0x0400011E RID: 286
	[NonSerialized]
	public Optionable<byte> lastSelectedSlot;

	// Token: 0x0400011F RID: 287
	private Character character;

	// Token: 0x04000120 RID: 288
	private new PhotonView photonView;

	// Token: 0x04000121 RID: 289
	private float lastEquippedSlotTime;

	// Token: 0x04000122 RID: 290
	[HideInInspector]
	public float throwChargeLevel;

	// Token: 0x04000123 RID: 291
	private float lastPressedDrop;

	// Token: 0x04000124 RID: 292
	private bool pressedDrop;

	// Token: 0x04000125 RID: 293
	public Action onSlotEquipped;

	// Token: 0x04000126 RID: 294
	public const int MAX_SLOT = 3;

	// Token: 0x04000127 RID: 295
	private float lastSwitched;

	// Token: 0x04000128 RID: 296
	private int timesSwitchedRecently;

	// Token: 0x04000129 RID: 297
	private float climbingSpikeTick;

	// Token: 0x0400012A RID: 298
	private bool readyToSpike = true;

	// Token: 0x0400012B RID: 299
	private bool spikingWithPrimary;

	// Token: 0x0400012C RID: 300
	private bool spikingWithSecondary;

	// Token: 0x0400012D RID: 301
	private ItemSlot currentClimbingSpikeItemSlot;

	// Token: 0x0400012E RID: 302
	private ClimbingSpikeComponent currentClimbingSpikeComponent;

	// Token: 0x0400012F RID: 303
	private GameObject currentClimbingSpikePreview;

	// Token: 0x04000130 RID: 304
	private RaycastHit climbingSpikeHit;
}
