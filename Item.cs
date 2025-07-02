using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using pworld.Scripts.Extensions;
using Sirenix.Utilities;
using Unity.Collections;
using UnityEngine;
using Zorro.ControllerSupport;
using Zorro.Core;
using Zorro.Core.Serizalization;

// Token: 0x0200001B RID: 27
public class Item : MonoBehaviourPunCallbacks, IInteractible
{
	// Token: 0x1700001B RID: 27
	// (get) Token: 0x060001B4 RID: 436 RVA: 0x0000D6FB File Offset: 0x0000B8FB
	public int CarryWeight
	{
		get
		{
			return this.carryWeight + Ascents.itemWeightModifier;
		}
	}

	// Token: 0x1700001C RID: 28
	// (get) Token: 0x060001B5 RID: 437 RVA: 0x0000D709 File Offset: 0x0000B909
	// (set) Token: 0x060001B6 RID: 438 RVA: 0x0000D711 File Offset: 0x0000B911
	public bool isUsingPrimary { get; private set; }

	// Token: 0x1700001D RID: 29
	// (get) Token: 0x060001B7 RID: 439 RVA: 0x0000D71A File Offset: 0x0000B91A
	// (set) Token: 0x060001B8 RID: 440 RVA: 0x0000D722 File Offset: 0x0000B922
	public ItemCooking cooking { get; private set; }

	// Token: 0x060001B9 RID: 441 RVA: 0x0000D72C File Offset: 0x0000B92C
	private void Awake()
	{
		this.view = base.GetComponent<PhotonView>();
		this.cooking = base.gameObject.GetOrAddComponent<ItemCooking>();
		this.AddPhysics();
		this.GetItemActions();
		this.AddPropertyBlock();
		this.particles = base.GetComponent<ItemParticles>();
		if (!this.particles)
		{
			this.particles = base.gameObject.AddComponent<ItemParticles>();
		}
		this.itemComponents = base.GetComponents<ItemComponent>();
		this.physicsSyncer = base.GetComponent<ItemPhysicsSyncer>();
	}

	// Token: 0x060001BA RID: 442 RVA: 0x0000D7AC File Offset: 0x0000B9AC
	private void Start()
	{
		if (!this.HasData(DataEntryKey.ItemUses))
		{
			OptionableIntItemData optionableIntItemData = this.GetData<OptionableIntItemData>(DataEntryKey.ItemUses);
			optionableIntItemData.HasData = this.totalUses != -1;
			optionableIntItemData.Value = this.totalUses;
			if (this.totalUses > 0)
			{
				this.SetUseRemainingPercentage(1f);
			}
		}
		if (!this.rig.isKinematic)
		{
			this.WasActive();
		}
		this.packLayer = 1 << LayerMask.NameToLayer("Exclude Collisions");
	}

	// Token: 0x060001BB RID: 443 RVA: 0x0000D824 File Offset: 0x0000BA24
	public string GetItemName(ItemInstanceData data = null)
	{
		int num = 0;
		IntItemData intItemData;
		if (data == null)
		{
			num = this.GetData<IntItemData>(DataEntryKey.CookedAmount).Value;
		}
		else if (data.TryGetDataEntry<IntItemData>(DataEntryKey.CookedAmount, out intItemData))
		{
			num = intItemData.Value;
		}
		string text;
		if (num < 4)
		{
			switch (num)
			{
			case 1:
				text = "Cooked " + this.UIData.itemName;
				break;
			case 2:
				text = "Well-done " + this.UIData.itemName;
				break;
			case 3:
				text = "Burnt " + this.UIData.itemName;
				break;
			default:
				text = this.UIData.itemName;
				break;
			}
		}
		else
		{
			text = "Incinerated " + this.UIData.itemName;
		}
		return text;
	}

	// Token: 0x060001BC RID: 444 RVA: 0x0000D8DE File Offset: 0x0000BADE
	private void AddPropertyBlock()
	{
		this.mpb = new MaterialPropertyBlock();
		this.mainRenderer = base.GetComponentInChildren<MeshRenderer>();
		this.mainRenderer.GetPropertyBlock(this.mpb);
	}

	// Token: 0x060001BD RID: 445 RVA: 0x0000D908 File Offset: 0x0000BB08
	private void GetItemActions()
	{
		this.itemActions = base.GetComponentsInChildren<ItemActionBase>();
	}

	// Token: 0x060001BE RID: 446 RVA: 0x0000D918 File Offset: 0x0000BB18
	private void AddPhysics()
	{
		this.rig = base.gameObject.GetOrAddComponent<Rigidbody>();
		this.rig.mass = this.mass;
		this.centerOfMass = this.rig.centerOfMass;
		this.rig.interpolation = RigidbodyInterpolation.Interpolate;
		this.rig.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
		this.colliders = base.GetComponentsInChildren<Collider>();
	}

	// Token: 0x060001BF RID: 447 RVA: 0x0000D97C File Offset: 0x0000BB7C
	protected virtual void Update()
	{
		if (this.itemState == ItemState.InBackpack)
		{
			if (this.backpackSlotTransform == null || !this.backpackSlotTransform.UnityObjectExists<Transform>())
			{
				base.transform.position = new Vector3(0f, -500f, 0f);
			}
			else
			{
				base.transform.position = this.backpackSlotTransform.position - this.backpackSlotTransform.rotation * this.centerOfMass * 0.5f;
				base.transform.rotation = this.backpackSlotTransform.rotation;
			}
		}
		else if (this.itemState == ItemState.Ground && PhotonNetwork.IsMasterClient)
		{
			if (base.transform.position.y < -2000f)
			{
				PhotonNetwork.Destroy(base.gameObject);
			}
		}
		else if (this.itemState == ItemState.Held)
		{
			this.WasActive();
		}
		this.UpdateEntryInActiveList();
		this.UpdateCollisionDetectionMode();
	}

	// Token: 0x060001C0 RID: 448 RVA: 0x0000DA75 File Offset: 0x0000BC75
	[PunRPC]
	public void TeleportRPC(Vector3 pos)
	{
		base.transform.position = pos;
	}

	// Token: 0x060001C1 RID: 449 RVA: 0x0000DA83 File Offset: 0x0000BC83
	private void UpdateCollisionDetectionMode()
	{
		if (this.itemState == ItemState.Ground)
		{
			this.rig.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
			return;
		}
		this.rig.collisionDetectionMode = CollisionDetectionMode.Discrete;
	}

	// Token: 0x060001C2 RID: 450 RVA: 0x0000DAA8 File Offset: 0x0000BCA8
	public virtual void Interact(Character interactor)
	{
		if (!interactor.player.HasEmptySlot(this.itemID))
		{
			return;
		}
		base.gameObject.SetActive(false);
		this.view.RPC("RequestPickup", RpcTarget.MasterClient, new object[] { interactor.GetComponent<PhotonView>() });
		Debug.Log("Picking up " + base.gameObject.name);
		ItemBackpackVisuals itemBackpackVisuals;
		if (base.TryGetComponent<ItemBackpackVisuals>(out itemBackpackVisuals))
		{
			itemBackpackVisuals.RemoveVisuals();
		}
		GlobalEvents.TriggerItemRequested(this, interactor);
	}

	// Token: 0x060001C3 RID: 451 RVA: 0x0000DB26 File Offset: 0x0000BD26
	[PunRPC]
	public void DenyPickupRPC()
	{
		base.gameObject.SetActive(true);
		this.SetKinematicNetworked(false, base.transform.position, base.transform.rotation);
	}

	// Token: 0x060001C4 RID: 452 RVA: 0x0000DB54 File Offset: 0x0000BD54
	[PunRPC]
	public void RequestPickup(PhotonView characterView)
	{
		Character component = characterView.GetComponent<Character>();
		ItemSlot itemSlot;
		bool flag = component.player.AddItem(this.itemID, this.data, out itemSlot);
		if (this.itemState == ItemState.InBackpack)
		{
			if (this.backpackReference.IsSome)
			{
				if (flag)
				{
					ValueTuple<byte, BackpackReference> value = this.backpackReference.Value;
					byte item = value.Item1;
					BackpackReference item2 = value.Item2;
					item2.GetData().itemSlots[(int)item].EmptyOut();
					if (item2.type == BackpackReference.BackpackType.Item)
					{
						item2.view.RPC("SetItemInstanceDataRPC", RpcTarget.Others, new object[] { item2.GetItemInstanceData() });
					}
					else
					{
						Character component2 = item2.view.GetComponent<Character>();
						ItemSlot[] itemSlots = component2.player.itemSlots;
						BackpackSlot backpackSlot = component2.player.backpackSlot;
						byte[] array = IBinarySerializable.ToManagedArray<InventorySyncData>(new InventorySyncData(itemSlots, backpackSlot, component2.player.tempFullSlot));
						component2.player.photonView.RPC("SyncInventoryRPC", RpcTarget.Others, new object[] { array, false });
					}
					component.refs.view.RPC("OnPickupAccepted", component.player.photonView.Owner, new object[] { itemSlot.itemSlotID });
					item2.GetVisuals().RefreshVisuals();
					return;
				}
				this.view.RPC("DenyPickupRPC", component.player.photonView.Owner, Array.Empty<object>());
			}
			return;
		}
		if (flag)
		{
			component.refs.view.RPC("OnPickupAccepted", component.player.photonView.Owner, new object[] { itemSlot.itemSlotID });
			PhotonNetwork.Destroy(this.view);
			return;
		}
		this.view.RPC("DenyPickupRPC", component.player.photonView.Owner, Array.Empty<object>());
	}

	// Token: 0x060001C5 RID: 453 RVA: 0x0000DD44 File Offset: 0x0000BF44
	public Vector3 Center()
	{
		if (!this.mainRenderer.UnityObjectExists<Renderer>())
		{
			return base.transform.position;
		}
		return this.mainRenderer.bounds.center;
	}

	// Token: 0x060001C6 RID: 454 RVA: 0x0000DD7D File Offset: 0x0000BF7D
	public Transform GetTransform()
	{
		return base.transform;
	}

	// Token: 0x060001C7 RID: 455 RVA: 0x0000DD85 File Offset: 0x0000BF85
	public virtual string GetInteractionText()
	{
		return "pick up";
	}

	// Token: 0x060001C8 RID: 456 RVA: 0x0000DD8C File Offset: 0x0000BF8C
	public string GetName()
	{
		return this.UIData.itemName;
	}

	// Token: 0x060001C9 RID: 457 RVA: 0x0000DD99 File Offset: 0x0000BF99
	public virtual bool IsInteractible(Character interactor)
	{
		return this.itemState != ItemState.Held && this.itemState != ItemState.InBackpack;
	}

	// Token: 0x060001CA RID: 458 RVA: 0x0000DDB4 File Offset: 0x0000BFB4
	internal void Move(Vector3 position, Quaternion rotation)
	{
		base.transform.position = position;
		base.transform.rotation = rotation;
		this.rig.position = position;
		this.rig.rotation = rotation;
		this.rig.linearVelocity *= 0f;
		this.rig.angularVelocity *= 0f;
	}

	// Token: 0x1700001E RID: 30
	// (get) Token: 0x060001CB RID: 459 RVA: 0x0000DE27 File Offset: 0x0000C027
	// (set) Token: 0x060001CC RID: 460 RVA: 0x0000DE43 File Offset: 0x0000C043
	public Character holderCharacter
	{
		get
		{
			if (this.overrideHolderCharacter)
			{
				return this.overrideHolderCharacter;
			}
			return this._holderCharacter;
		}
		set
		{
			if (value != null)
			{
				this.lastHolderCharacter = value;
			}
			this._holderCharacter = value;
		}
	}

	// Token: 0x1700001F RID: 31
	// (get) Token: 0x060001CD RID: 461 RVA: 0x0000DE5C File Offset: 0x0000C05C
	public Character trueHolderCharacter
	{
		get
		{
			return this._holderCharacter;
		}
	}

	// Token: 0x060001CE RID: 462 RVA: 0x0000DE64 File Offset: 0x0000C064
	private void SetColliders(bool enabled, bool isTrigger, bool excludeLayer = false)
	{
		for (int i = 0; i < this.colliders.Length; i++)
		{
			this.colliders[i].enabled = enabled;
			this.colliders[i].isTrigger = isTrigger;
		}
		if (excludeLayer)
		{
			this.rig.excludeLayers = 1 << LayerMask.NameToLayer("Default");
			return;
		}
		this.rig.excludeLayers = 0;
	}

	// Token: 0x060001CF RID: 463 RVA: 0x0000DED4 File Offset: 0x0000C0D4
	internal void SetState(ItemState setState, Character character = null)
	{
		Debug.Log(string.Format("Setting Item State: {0}", setState));
		this.itemState = setState;
		Action<ItemState> onStateChange = this.OnStateChange;
		if (onStateChange != null)
		{
			onStateChange(setState);
		}
		if (setState == ItemState.InBackpack)
		{
			this.holderCharacter = null;
			this.rig.useGravity = false;
			this.rig.isKinematic = true;
			this.rig.interpolation = RigidbodyInterpolation.None;
			this.SetColliders(true, true, false);
			base.transform.localScale = Vector3.one * 0.5f;
			return;
		}
		if (setState == ItemState.Ground)
		{
			this.holderCharacter = null;
			this.rig.useGravity = true;
			this.rig.isKinematic = false;
			this.rig.interpolation = RigidbodyInterpolation.Interpolate;
			this.centerOfMass = this.rig.centerOfMass;
			if (this is Backpack)
			{
				this.wearerCharacter = null;
			}
			this.SetColliders(true, false, false);
			base.transform.localScale = Vector3.one;
			return;
		}
		if (setState == ItemState.Held)
		{
			this.holderCharacter = character;
			this.rig.useGravity = false;
			this.rig.isKinematic = false;
			this.rig.interpolation = RigidbodyInterpolation.Interpolate;
			if (this is Backpack)
			{
				this.wearerCharacter = null;
			}
			if (character != null && PhotonNetwork.IsMasterClient)
			{
				base.photonView.TransferOwnership(character.GetComponent<PhotonView>().Owner);
			}
			this.SetColliders(true, false, true);
			base.transform.localScale = Vector3.one;
		}
	}

	// Token: 0x060001D0 RID: 464 RVA: 0x0000E047 File Offset: 0x0000C247
	private void HideRenderers()
	{
		base.GetComponentsInChildren<MeshRenderer>().ForEach(delegate(MeshRenderer meshRenderer)
		{
			meshRenderer.enabled = false;
		});
	}

	// Token: 0x17000020 RID: 32
	// (get) Token: 0x060001D1 RID: 465 RVA: 0x0000E074 File Offset: 0x0000C274
	// (set) Token: 0x060001D2 RID: 466 RVA: 0x0000E07C File Offset: 0x0000C27C
	public bool isUsingSecondary { get; private set; }

	// Token: 0x17000021 RID: 33
	// (get) Token: 0x060001D3 RID: 467 RVA: 0x0000E085 File Offset: 0x0000C285
	// (set) Token: 0x060001D4 RID: 468 RVA: 0x0000E08D File Offset: 0x0000C28D
	public float castProgress { get; private set; }

	// Token: 0x17000022 RID: 34
	// (get) Token: 0x060001D5 RID: 469 RVA: 0x0000E096 File Offset: 0x0000C296
	public bool shouldShowCastProgress
	{
		get
		{
			return (this.showUseProgress && this.castProgress > 0f && !this.finishedCast) || this.overrideForceProgress;
		}
	}

	// Token: 0x060001D6 RID: 470 RVA: 0x0000E0C0 File Offset: 0x0000C2C0
	public virtual bool CanUsePrimary()
	{
		if (!this.overrideUsability.IsNone)
		{
			return this.overrideUsability.Value;
		}
		OptionableIntItemData optionableIntItemData = this.GetData<OptionableIntItemData>(DataEntryKey.ItemUses);
		return !optionableIntItemData.HasData || optionableIntItemData.Value == -1 || optionableIntItemData.Value > 0;
	}

	// Token: 0x060001D7 RID: 471 RVA: 0x0000E10C File Offset: 0x0000C30C
	public virtual bool CanUseSecondary()
	{
		bool flag = true;
		OptionableIntItemData optionableIntItemData = this.GetData<OptionableIntItemData>(DataEntryKey.ItemUses);
		if (optionableIntItemData.HasData)
		{
			flag = optionableIntItemData.Value == -1 || optionableIntItemData.Value > 0;
		}
		if (!flag)
		{
			return false;
		}
		if (this.canUseOnFriend)
		{
			if (Interaction.instance.hasValidTargetCharacter)
			{
				return true;
			}
		}
		else if (this.UIData.hasSecondInteract)
		{
			return true;
		}
		return false;
	}

	// Token: 0x060001D8 RID: 472 RVA: 0x0000E16C File Offset: 0x0000C36C
	public void StartUsePrimary()
	{
		if (this.isUsingSecondary)
		{
			this.CancelUseSecondary();
		}
		this.isUsingPrimary = true;
		this.castProgress = 0f;
		this.finishedCast = false;
		if (this.OnPrimaryStarted != null)
		{
			this.OnPrimaryStarted();
		}
	}

	// Token: 0x060001D9 RID: 473 RVA: 0x0000E1A8 File Offset: 0x0000C3A8
	public void ContinueUsePrimary()
	{
		if (this.isUsingSecondary)
		{
			this.CancelUseSecondary();
		}
		if (this.isUsingPrimary)
		{
			if (this.usingTimePrimary > 0f)
			{
				this.castProgress += 1f / this.usingTimePrimary * Time.deltaTime;
				if (this.castProgress >= 1f)
				{
					if (this.OnPrimaryHeld != null)
					{
						this.OnPrimaryHeld();
					}
					if (!this.finishedCast)
					{
						this.FinishCastPrimary();
						return;
					}
				}
			}
			else
			{
				if (!this.finishedCast)
				{
					this.FinishCastPrimary();
				}
				if (this.OnPrimaryHeld != null)
				{
					this.OnPrimaryHeld();
				}
			}
		}
	}

	// Token: 0x060001DA RID: 474 RVA: 0x0000E248 File Offset: 0x0000C448
	protected virtual void FinishCastPrimary()
	{
		if (base.GetComponent<ItemUseFeedback>())
		{
			this.holderCharacter.refs.animator.SetBool(base.GetComponent<ItemUseFeedback>().useAnimation, false);
			if (base.GetComponent<ItemUseFeedback>().sfxUsed)
			{
				base.GetComponent<ItemUseFeedback>().sfxUsed.Play(base.transform.position);
			}
		}
		this.finishedCast = true;
		this.castProgress = 0f;
		if (this.OnPrimaryFinishedCast != null)
		{
			this.OnPrimaryFinishedCast();
		}
	}

	// Token: 0x060001DB RID: 475 RVA: 0x0000E2D8 File Offset: 0x0000C4D8
	public void CancelUsePrimary()
	{
		this.isUsingPrimary = false;
		this.castProgress = 0f;
		this.finishedCast = false;
		if (this.OnPrimaryCancelled != null)
		{
			this.OnPrimaryCancelled();
		}
		if (global::Player.localPlayer == null)
		{
			Debug.LogError("Player.localPlayer is null, cannot play movement animation");
			return;
		}
		if (global::Player.localPlayer.character == null)
		{
			Debug.LogError("Player.localPlayer.character is null, cannot play movement animation");
			return;
		}
		if (global::Player.localPlayer.character.refs == null)
		{
			Debug.LogError("Player.localPlayer.character.refs is null, cannot play movement animation");
			return;
		}
		if (global::Player.localPlayer.character.refs.animations == null)
		{
			Debug.LogError("Player.localPlayer.character.refs.animations is null, cannot play movement animation");
			return;
		}
		global::Player.localPlayer.character.refs.animations.PlaySpecificAnimation("Movement");
	}

	// Token: 0x060001DC RID: 476 RVA: 0x0000E3A7 File Offset: 0x0000C5A7
	public void ScrollButtonLeft()
	{
		if (this.OnScrollButtonLeft != null)
		{
			this.OnScrollButtonLeft();
		}
	}

	// Token: 0x060001DD RID: 477 RVA: 0x0000E3BC File Offset: 0x0000C5BC
	public void ScrollButtonRight()
	{
		if (this.OnScrollButtonRight != null)
		{
			this.OnScrollButtonRight();
		}
	}

	// Token: 0x060001DE RID: 478 RVA: 0x0000E3D1 File Offset: 0x0000C5D1
	public void Scroll(float value)
	{
		if (this.OnScrolled != null)
		{
			this.OnScrolled(value);
		}
		if (InputHandler.GetCurrentUsedInputScheme() == InputScheme.KeyboardMouse && this.OnScrolledMouseOnly != null)
		{
			this.OnScrolledMouseOnly(value);
		}
	}

	// Token: 0x060001DF RID: 479 RVA: 0x0000E404 File Offset: 0x0000C604
	public void StartUseSecondary()
	{
		if (this.isUsingPrimary)
		{
			return;
		}
		if (this.isUsingSecondary)
		{
			return;
		}
		this.isUsingSecondary = true;
		this.castProgress = 0f;
		this.finishedCast = false;
		if (this.holderCharacter && this.canUseOnFriend && Interaction.instance.hasValidTargetCharacter)
		{
			base.photonView.RPC("SendFeedDataRPC", RpcTarget.All, new object[]
			{
				this.holderCharacter.photonView.ViewID,
				Interaction.instance.bestCharacter.character.photonView.ViewID,
				(int)this.itemID,
				this.totalSecondaryUsingTime
			});
		}
		if (this.OnSecondaryStarted != null)
		{
			this.OnSecondaryStarted();
		}
	}

	// Token: 0x060001E0 RID: 480 RVA: 0x0000E4DF File Offset: 0x0000C6DF
	[PunRPC]
	internal void SendFeedDataRPC(int giverID, int recieverID, int itemID, float totalUsingTime)
	{
		GameUtils.instance.StartFeed(giverID, recieverID, (ushort)itemID, totalUsingTime);
	}

	// Token: 0x060001E1 RID: 481 RVA: 0x0000E4F1 File Offset: 0x0000C6F1
	[PunRPC]
	internal void RemoveFeedDataRPC(int giverID)
	{
		GameUtils.instance.EndFeed(giverID);
	}

	// Token: 0x17000023 RID: 35
	// (get) Token: 0x060001E2 RID: 482 RVA: 0x0000E4FE File Offset: 0x0000C6FE
	public float totalSecondaryUsingTime
	{
		get
		{
			if (!this.canUseOnFriend)
			{
				return this.usingTimePrimary;
			}
			return this.usingTimePrimary * 0.7f;
		}
	}

	// Token: 0x060001E3 RID: 483 RVA: 0x0000E51C File Offset: 0x0000C71C
	public void ContinueUseSecondary()
	{
		if (this.isUsingPrimary)
		{
			return;
		}
		if (this.isUsingSecondary)
		{
			if (this.usingTimePrimary > 0f)
			{
				this.castProgress += 1f / this.totalSecondaryUsingTime * Time.deltaTime;
				if (this.castProgress >= 1f)
				{
					if (this.OnSecondaryHeld != null)
					{
						this.OnSecondaryHeld();
					}
					if (!this.finishedCast)
					{
						this.FinishCastSecondary();
						return;
					}
				}
			}
			else if (this.OnSecondaryHeld != null)
			{
				this.OnSecondaryHeld();
			}
		}
	}

	// Token: 0x060001E4 RID: 484 RVA: 0x0000E5A8 File Offset: 0x0000C7A8
	public void FinishCastSecondary()
	{
		this.finishedCast = true;
		this.castProgress = 0f;
		if (this.canUseOnFriend && Interaction.instance.hasValidTargetCharacter)
		{
			if (this.holderCharacter)
			{
				this.holderCharacter.data.lastConsumedItem = Time.time;
				base.photonView.RPC("RemoveFeedDataRPC", RpcTarget.All, new object[] { this.holderCharacter.photonView.ViewID });
			}
			Interaction.instance.bestCharacter.character.FeedItem(this);
			base.photonView.RPC("RemoveFeedDataRPC", RpcTarget.All, new object[] { (int)this.itemID });
			return;
		}
		if (this.OnSecondaryFinishedCast != null)
		{
			this.OnSecondaryFinishedCast();
		}
	}

	// Token: 0x060001E5 RID: 485 RVA: 0x0000E680 File Offset: 0x0000C880
	public void CancelUseSecondary()
	{
		this.isUsingSecondary = false;
		this.castProgress = 0f;
		this.finishedCast = false;
		if (this.OnSecondaryCancelled != null)
		{
			this.OnSecondaryCancelled();
		}
		global::Player.localPlayer.character.refs.animations.PlaySpecificAnimation("Movement");
		if (this.lastHolderCharacter)
		{
			base.photonView.RPC("RemoveFeedDataRPC", RpcTarget.All, new object[] { this.lastHolderCharacter.photonView.ViewID });
		}
	}

	// Token: 0x060001E6 RID: 486 RVA: 0x0000E713 File Offset: 0x0000C913
	public IEnumerator ConsumeDelayed(bool ignoreActions = false)
	{
		if (!ignoreActions && this.OnConsumed != null)
		{
			this.OnConsumed();
		}
		yield return null;
		base.photonView.RPC("Consume", RpcTarget.All, Array.Empty<object>());
		yield break;
	}

	// Token: 0x060001E7 RID: 487 RVA: 0x0000E72C File Offset: 0x0000C92C
	[PunRPC]
	public void Consume()
	{
		if (this.holderCharacter != null)
		{
			string name = this.holderCharacter.gameObject.name;
		}
		if (this.holderCharacter && this.holderCharacter.data.currentItem == this)
		{
			Optionable<byte> currentSelectedSlot = this.holderCharacter.refs.items.currentSelectedSlot;
			this.holderCharacter.refs.animator.SetBool("Consumed Item", true);
			GlobalEvents.TriggerItemConsumed(this, this.holderCharacter);
			if (this.holderCharacter.IsLocal)
			{
				if (currentSelectedSlot.IsSome)
				{
					this.holderCharacter.player.EmptySlot(currentSelectedSlot);
					this.holderCharacter.refs.items.EquipSlot(currentSelectedSlot);
				}
				else
				{
					Debug.LogError("No Item Selected locally but still consuming?? THIS IS BAD. CALL ZORRO");
				}
			}
			this.holderCharacter.data.lastConsumedItem = Time.time;
		}
		base.gameObject.SetActive(false);
	}

	// Token: 0x060001E8 RID: 488 RVA: 0x0000E82A File Offset: 0x0000CA2A
	public virtual void OnStash()
	{
		Action action = this.onStashAction;
		if (action != null)
		{
			action();
		}
		this.CancelUsePrimary();
		this.CancelUseSecondary();
	}

	// Token: 0x060001E9 RID: 489 RVA: 0x0000E84C File Offset: 0x0000CA4C
	[ContextMenu("Add Default Food Scripts")]
	public void AddDefaultFoodScripts()
	{
		this.usingTimePrimary = 1.2f;
		Action_PlayAnimation action_PlayAnimation = base.gameObject.AddComponent<Action_PlayAnimation>();
		action_PlayAnimation.OnPressed = true;
		action_PlayAnimation.animationName = "PlayerEat";
		Action_ModifyStatus action_ModifyStatus = base.gameObject.AddComponent<Action_ModifyStatus>();
		action_ModifyStatus.OnCastFinished = true;
		action_ModifyStatus.statusType = CharacterAfflictions.STATUSTYPE.Hunger;
		action_ModifyStatus.changeAmount = -0.1f;
		base.gameObject.AddComponent<Action_Consume>().OnCastFinished = true;
	}

	// Token: 0x060001EA RID: 490 RVA: 0x0000E8B4 File Offset: 0x0000CAB4
	public void HoverEnter()
	{
		this.mpb.SetFloat(Item.PROPERTY_INTERACTABLE, 1f);
		base.GetComponentInChildren<MeshRenderer>().SetPropertyBlock(this.mpb);
	}

	// Token: 0x060001EB RID: 491 RVA: 0x0000E8DC File Offset: 0x0000CADC
	public void HoverExit()
	{
		this.mpb.SetFloat(Item.PROPERTY_INTERACTABLE, 0f);
		base.GetComponentInChildren<MeshRenderer>().SetPropertyBlock(this.mpb);
	}

	// Token: 0x060001EC RID: 492 RVA: 0x0000E904 File Offset: 0x0000CB04
	public void SetKinematicNetworked(bool value, Vector3 position, Quaternion rotation)
	{
		base.photonView.RPC("SetKinematicRPC", RpcTarget.AllBuffered, new object[] { value, position, rotation });
	}

	// Token: 0x060001ED RID: 493 RVA: 0x0000E938 File Offset: 0x0000CB38
	[PunRPC]
	public void SetKinematicRPC(bool value, Vector3 position, Quaternion rotation)
	{
		this.rig.isKinematic = value;
		this.rig.position = position;
		this.rig.rotation = rotation;
	}

	// Token: 0x060001EE RID: 494 RVA: 0x0000E95E File Offset: 0x0000CB5E
	public bool HasData(DataEntryKey key)
	{
		return this.data != null && this.data.HasData(key);
	}

	// Token: 0x060001EF RID: 495 RVA: 0x0000E978 File Offset: 0x0000CB78
	public T GetData<T>(DataEntryKey key, Func<T> createDefault) where T : DataEntryValue, new()
	{
		if (this.data == null)
		{
			this.data = new ItemInstanceData(Guid.NewGuid());
			ItemInstanceDataHandler.AddInstanceData(this.data);
		}
		T t;
		if (this.data.TryGetDataEntry<T>(key, out t))
		{
			return t;
		}
		if (createDefault != null)
		{
			return this.data.RegisterEntry<T>(key, createDefault());
		}
		return this.data.RegisterNewEntry<T>(key);
	}

	// Token: 0x060001F0 RID: 496 RVA: 0x0000E9DC File Offset: 0x0000CBDC
	public T GetData<T>(DataEntryKey key) where T : DataEntryValue, new()
	{
		return this.GetData<T>(key, null);
	}

	// Token: 0x060001F1 RID: 497 RVA: 0x0000E9E6 File Offset: 0x0000CBE6
	internal void ForceSyncForFrames()
	{
		if (this.physicsSyncer != null)
		{
			this.physicsSyncer.ForceSyncForFrames();
		}
	}

	// Token: 0x060001F2 RID: 498 RVA: 0x0000EA04 File Offset: 0x0000CC04
	[PunRPC]
	public void SetItemInstanceDataRPC(ItemInstanceData instanceData)
	{
		this.data = instanceData;
		if (this.data != null)
		{
			this.OnInstanceDataRecieved();
			ItemComponent[] array = this.itemComponents;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].OnInstanceDataSet();
			}
		}
	}

	// Token: 0x060001F3 RID: 499 RVA: 0x0000EA43 File Offset: 0x0000CC43
	public virtual void OnInstanceDataRecieved()
	{
	}

	// Token: 0x060001F4 RID: 500 RVA: 0x0000EA48 File Offset: 0x0000CC48
	public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
	{
		base.OnPlayerEnteredRoom(newPlayer);
		if (!PhotonNetwork.IsMasterClient)
		{
			return;
		}
		this.ForceSyncForFrames();
		ItemState itemState = this.itemState;
		if ((itemState == ItemState.Ground || itemState == ItemState.Held || itemState == ItemState.InBackpack) && this.data != null)
		{
			this.view.RPC("SetItemInstanceDataRPC", newPlayer, new object[] { this.data });
		}
		if (this.itemState == ItemState.InBackpack)
		{
			ValueTuple<byte, BackpackReference> value = this.backpackReference.Value;
			byte item = value.Item1;
			BackpackReference item2 = value.Item2;
			this.view.RPC("PutInBackpackRPC", newPlayer, new object[] { item, item2 });
		}
		if (this.rig.isKinematic)
		{
			this.view.RPC("SetKinematicRPC", newPlayer, new object[]
			{
				this.rig.isKinematic,
				this.rig.position,
				this.rig.rotation
			});
		}
	}

	// Token: 0x060001F5 RID: 501 RVA: 0x0000EB4C File Offset: 0x0000CD4C
	[PunRPC]
	public void PutInBackpackRPC(byte slotID, BackpackReference backpackReference)
	{
		Transform[] backpackSlots = backpackReference.GetVisuals().backpackSlots;
		this.backpackReference = Optionable<ValueTuple<byte, BackpackReference>>.Some(new ValueTuple<byte, BackpackReference>(slotID, backpackReference));
		this.backpackSlotTransform = backpackSlots[(int)slotID];
		this.SetState(ItemState.InBackpack, null);
		backpackReference.GetVisuals().SetSpawnedBackpackItem(slotID, this);
		if (backpackReference.IsOnMyBack())
		{
			this.HideRenderers();
		}
	}

	// Token: 0x060001F6 RID: 502 RVA: 0x0000EBA6 File Offset: 0x0000CDA6
	[PunRPC]
	public void SetCookedAmountRPC(int amount)
	{
		this.GetData<IntItemData>(DataEntryKey.CookedAmount).Value = amount;
	}

	// Token: 0x060001F7 RID: 503 RVA: 0x0000EBB5 File Offset: 0x0000CDB5
	public void SetUseRemainingPercentage(float percentage)
	{
		this.GetData<FloatItemData>(DataEntryKey.UseRemainingPercentage).Value = Mathf.Clamp01(percentage);
	}

	// Token: 0x17000024 RID: 36
	// (get) Token: 0x060001F8 RID: 504 RVA: 0x0000EBCA File Offset: 0x0000CDCA
	// (set) Token: 0x060001F9 RID: 505 RVA: 0x0000EBD2 File Offset: 0x0000CDD2
	public bool inActiveList { get; private set; }

	// Token: 0x060001FA RID: 506 RVA: 0x0000EBDB File Offset: 0x0000CDDB
	public void WasActive()
	{
		if (!this.inActiveList)
		{
			Item.ALL_ACTIVE_ITEMS.Add(this);
		}
		this.inActiveList = true;
		this.timeSinceWasActive = 0f;
	}

	// Token: 0x060001FB RID: 507 RVA: 0x0000EC02 File Offset: 0x0000CE02
	private void UpdateEntryInActiveList()
	{
		if (this.inActiveList)
		{
			this.timeSinceWasActive += Time.deltaTime;
			if (this.timeSinceWasActive > 30f)
			{
				this.RemoveFromActiveList();
			}
		}
	}

	// Token: 0x060001FC RID: 508 RVA: 0x0000EC31 File Offset: 0x0000CE31
	private void RemoveFromActiveList()
	{
		if (this.inActiveList)
		{
			Item.ALL_ACTIVE_ITEMS.Remove(this);
			this.inActiveList = false;
		}
	}

	// Token: 0x060001FD RID: 509 RVA: 0x0000EC4E File Offset: 0x0000CE4E
	private void OnDestroy()
	{
		this.RemoveFromActiveList();
	}

	// Token: 0x060001FE RID: 510 RVA: 0x0000EC56 File Offset: 0x0000CE56
	public bool TryGetFeeder(out Character feeder)
	{
		if (this.trueHolderCharacter != null && this.trueHolderCharacter != this.holderCharacter)
		{
			feeder = this.trueHolderCharacter;
			return true;
		}
		feeder = null;
		return false;
	}

	// Token: 0x060001FF RID: 511 RVA: 0x0000EC88 File Offset: 0x0000CE88
	public bool IsValidToSpawn()
	{
		LootData component = base.GetComponent<LootData>();
		return !component || component.IsValidToSpawn();
	}

	// Token: 0x040001BD RID: 445
	public static readonly int PROPERTY_INTERACTABLE = Shader.PropertyToID("_Interactable");

	// Token: 0x040001BE RID: 446
	public static List<Item> ALL_ACTIVE_ITEMS = new List<Item>();

	// Token: 0x040001BF RID: 447
	public Vector3 defaultPos;

	// Token: 0x040001C0 RID: 448
	public Vector3 defaultForward = new Vector3(0f, 0f, 1f);

	// Token: 0x040001C1 RID: 449
	public float mass = 5f;

	// Token: 0x040001C2 RID: 450
	public ItemState itemState;

	// Token: 0x040001C3 RID: 451
	[SerializeField]
	private int carryWeight = 1;

	// Token: 0x040001C5 RID: 453
	public float usingTimePrimary;

	// Token: 0x040001C6 RID: 454
	public bool showUseProgress = true;

	// Token: 0x040001C7 RID: 455
	public Action OnPrimaryStarted;

	// Token: 0x040001C8 RID: 456
	public Action OnPrimaryHeld;

	// Token: 0x040001C9 RID: 457
	public Action OnPrimaryFinishedCast;

	// Token: 0x040001CA RID: 458
	public Action OnPrimaryReleased;

	// Token: 0x040001CB RID: 459
	public Action OnPrimaryCancelled;

	// Token: 0x040001CC RID: 460
	public Action OnConsumed;

	// Token: 0x040001CD RID: 461
	public Action OnSecondaryStarted;

	// Token: 0x040001CE RID: 462
	public Action OnSecondaryHeld;

	// Token: 0x040001CF RID: 463
	public Action OnSecondaryFinishedCast;

	// Token: 0x040001D0 RID: 464
	public Action OnSecondaryCancelled;

	// Token: 0x040001D1 RID: 465
	public Action<ItemState> OnStateChange;

	// Token: 0x040001D2 RID: 466
	public Action<float> OnScrolled;

	// Token: 0x040001D3 RID: 467
	public Action<float> OnScrolledMouseOnly;

	// Token: 0x040001D4 RID: 468
	public Action OnScrollButtonLeft;

	// Token: 0x040001D5 RID: 469
	public Action OnScrollButtonRight;

	// Token: 0x040001D6 RID: 470
	public Item.ItemUIData UIData;

	// Token: 0x040001D7 RID: 471
	[NonSerialized]
	public Transform backpackSlotTransform;

	// Token: 0x040001D8 RID: 472
	private Optionable<ValueTuple<byte, BackpackReference>> backpackReference;

	// Token: 0x040001D9 RID: 473
	private Optionable<RigidbodySyncData> m_lastState = Optionable<RigidbodySyncData>.None;

	// Token: 0x040001DA RID: 474
	protected PhotonView view;

	// Token: 0x040001DB RID: 475
	public int totalUses = -1;

	// Token: 0x040001DC RID: 476
	public ItemInstanceData data;

	// Token: 0x040001DD RID: 477
	public Item.ItemTags itemTags;

	// Token: 0x040001DE RID: 478
	public Rigidbody rig;

	// Token: 0x040001DF RID: 479
	internal ItemActionBase[] itemActions;

	// Token: 0x040001E0 RID: 480
	[HideInInspector]
	public Collider[] colliders;

	// Token: 0x040001E1 RID: 481
	public ushort itemID;

	// Token: 0x040001E2 RID: 482
	private MaterialPropertyBlock mpb;

	// Token: 0x040001E3 RID: 483
	public Renderer mainRenderer;

	// Token: 0x040001E4 RID: 484
	private double timeSinceTick;

	// Token: 0x040001E6 RID: 486
	private ItemComponent[] itemComponents;

	// Token: 0x040001E7 RID: 487
	protected Color originalTint;

	// Token: 0x040001E8 RID: 488
	private ItemPhysicsSyncer physicsSyncer;

	// Token: 0x040001E9 RID: 489
	[HideInInspector]
	public ItemParticles particles;

	// Token: 0x040001EA RID: 490
	private int packLayer;

	// Token: 0x040001EB RID: 491
	public Vector3 centerOfMass;

	// Token: 0x040001EC RID: 492
	private Character lastHolderCharacter;

	// Token: 0x040001ED RID: 493
	[ReadOnly]
	public Character wearerCharacter;

	// Token: 0x040001EE RID: 494
	[SerializeField]
	[ReadOnly]
	private Character _holderCharacter;

	// Token: 0x040001EF RID: 495
	[ReadOnly]
	public Character overrideHolderCharacter;

	// Token: 0x040001F1 RID: 497
	public bool canUseOnFriend;

	// Token: 0x040001F3 RID: 499
	public bool finishedCast;

	// Token: 0x040001F4 RID: 500
	internal float overrideProgress;

	// Token: 0x040001F5 RID: 501
	internal Optionable<bool> overrideUsability;

	// Token: 0x040001F6 RID: 502
	public Action onStashAction;

	// Token: 0x040001F7 RID: 503
	internal bool overrideForceProgress;

	// Token: 0x040001F9 RID: 505
	private float timeSinceWasActive;

	// Token: 0x020002EE RID: 750
	[Flags]
	public enum ItemTags
	{
		// Token: 0x040010B9 RID: 4281
		None = 0,
		// Token: 0x040010BA RID: 4282
		Mystical = 1,
		// Token: 0x040010BB RID: 4283
		PackagedFood = 2,
		// Token: 0x040010BC RID: 4284
		Berry = 4,
		// Token: 0x040010BD RID: 4285
		Mushroom = 8,
		// Token: 0x040010BE RID: 4286
		BingBong = 16,
		// Token: 0x040010BF RID: 4287
		GourmandRequirement = 32
	}

	// Token: 0x020002EF RID: 751
	[Serializable]
	public class ItemUIData
	{
		// Token: 0x040010C0 RID: 4288
		public string itemName;

		// Token: 0x040010C1 RID: 4289
		public Texture2D icon;

		// Token: 0x040010C2 RID: 4290
		public bool hasMainInteract = true;

		// Token: 0x040010C3 RID: 4291
		public string mainInteractPrompt;

		// Token: 0x040010C4 RID: 4292
		public bool hasSecondInteract;

		// Token: 0x040010C5 RID: 4293
		public string secondaryInteractPrompt;

		// Token: 0x040010C6 RID: 4294
		public bool hasScrollingInteract;

		// Token: 0x040010C7 RID: 4295
		public string scrollInteractPrompt;

		// Token: 0x040010C8 RID: 4296
		public bool canDrop = true;

		// Token: 0x040010C9 RID: 4297
		public bool canPocket = true;

		// Token: 0x040010CA RID: 4298
		public bool canThrow = true;

		// Token: 0x040010CB RID: 4299
		public bool isShootable;

		// Token: 0x040010CC RID: 4300
		public Vector3 iconPositionOffset;

		// Token: 0x040010CD RID: 4301
		public Vector3 iconRotationOffset;

		// Token: 0x040010CE RID: 4302
		public float iconScaleOffset = 1f;
	}
}
