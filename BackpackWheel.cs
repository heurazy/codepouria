using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zorro.Core;

// Token: 0x02000149 RID: 329
public class BackpackWheel : UIWheel
{
	// Token: 0x06000963 RID: 2403 RVA: 0x0002F3A4 File Offset: 0x0002D5A4
	public void InitWheel(BackpackReference bp)
	{
		this.backpack = bp;
		this.chosenSlice = Optionable<BackpackWheelSlice.SliceData>.None;
		this.chosenItemText.text = "";
		ItemSlot[] itemSlots = this.backpack.GetData().itemSlots;
		byte b = 0;
		while ((int)b < itemSlots.Length)
		{
			this.slices[(int)(b + 1)].InitItemSlot(new ValueTuple<BackpackReference, byte>(bp, b), this);
			b += 1;
		}
		base.gameObject.SetActive(true);
		this.slices[0].InitPickupBackpack(bp, this);
		if (Character.localCharacter.data.currentItem)
		{
			this.currentlyHeldItem.texture = Character.localCharacter.data.currentItem.UIData.icon;
			this.UpdateCookedAmount(Character.localCharacter.data.currentItem);
			this.currentlyHeldItem.enabled = true;
			return;
		}
		this.UpdateCookedAmount(null);
		this.currentlyHeldItem.enabled = false;
	}

	// Token: 0x06000964 RID: 2404 RVA: 0x0002F494 File Offset: 0x0002D694
	private void UpdateCookedAmount(Item item)
	{
		if (item == null || item.data == null)
		{
			this.currentlyHeldItemCookedAmount = 0;
			this.currentlyHeldItem.color = Color.white;
			return;
		}
		IntItemData intItemData;
		if (item.data.TryGetDataEntry<IntItemData>(DataEntryKey.CookedAmount, out intItemData) && this.currentlyHeldItemCookedAmount != intItemData.Value)
		{
			this.currentlyHeldItem.color = Color.white;
			this.currentlyHeldItem.color = ItemCooking.GetCookColor(intItemData.Value);
			this.currentlyHeldItemCookedAmount = intItemData.Value;
		}
	}

	// Token: 0x06000965 RID: 2405 RVA: 0x0002F51C File Offset: 0x0002D71C
	protected override void Update()
	{
		if (!Character.localCharacter.input.interactIsPressed)
		{
			this.Choose();
			GUIManager.instance.CloseBackpackWheel();
			return;
		}
		if (this.backpack.locationTransform != null && Vector3.Distance(this.backpack.locationTransform.position, Character.localCharacter.Center) > 6f)
		{
			GUIManager.instance.CloseBackpackWheel();
			return;
		}
		if (this.chosenSlice.IsSome && !this.chosenSlice.Value.isBackpackWear && !this.slices[(int)(this.chosenSlice.Value.slotID + 1)].image.enabled)
		{
			this.currentlyHeldItem.transform.position = Vector3.Lerp(this.currentlyHeldItem.transform.position, this.slices[(int)(this.chosenSlice.Value.slotID + 1)].transform.GetChild(0).GetChild(0).position, Time.deltaTime * 20f);
		}
		else
		{
			this.currentlyHeldItem.transform.localPosition = Vector3.Lerp(this.currentlyHeldItem.transform.localPosition, Vector3.zero, Time.deltaTime * 20f);
		}
		base.Update();
	}

	// Token: 0x06000966 RID: 2406 RVA: 0x0002F678 File Offset: 0x0002D878
	public void Choose()
	{
		if (this.chosenSlice.IsSome)
		{
			Debug.Log(string.Format("Chose slice {0}", this.chosenSlice.Value.slotID));
			if (this.chosenSlice.Value.isBackpackWear)
			{
				BackpackWheelSlice.SliceData sliceData = this.chosenSlice.Value;
				Backpack backpack;
				if (sliceData.backpackReference.TryGetBackpackItem(out backpack))
				{
					backpack.Wear(Character.localCharacter);
					return;
				}
			}
			else
			{
				if (this.chosenSlice.Value.isStashSlice)
				{
					this.TryStash(this.chosenSlice.Value.slotID);
					return;
				}
				BackpackWheelSlice.SliceData sliceData = this.chosenSlice.Value;
				Item item;
				if (sliceData.backpackReference.GetVisuals().TryGetSpawnedItem(this.chosenSlice.Value.slotID, out item))
				{
					item.Interact(Character.localCharacter);
					return;
				}
				if (Character.localCharacter.data.currentItem)
				{
					this.TryStash(this.chosenSlice.Value.slotID);
				}
			}
		}
	}

	// Token: 0x06000967 RID: 2407 RVA: 0x0002F788 File Offset: 0x0002D988
	private void TryStash(byte backpackSlotID)
	{
		Backpack backpack;
		if (this.backpack.TryGetBackpackItem(out backpack))
		{
			backpack.Stash(Character.localCharacter, backpackSlotID);
			return;
		}
		this.backpack.view.GetComponent<CharacterBackpackHandler>().StashInBackpack(Character.localCharacter, backpackSlotID);
	}

	// Token: 0x06000968 RID: 2408 RVA: 0x0002F7CC File Offset: 0x0002D9CC
	public void Hover(BackpackWheelSlice.SliceData sliceData)
	{
		if (sliceData.isBackpackWear)
		{
			if (sliceData.backpackReference.type == BackpackReference.BackpackType.Equipped)
			{
				return;
			}
			this.chosenItemText.text = "wear<br> Backpack";
			this.chosenSlice = Optionable<BackpackWheelSlice.SliceData>.Some(sliceData);
			return;
		}
		else
		{
			if (!sliceData.isStashSlice)
			{
				ItemSlot itemSlot = this.backpack.GetData().itemSlots[(int)sliceData.slotID];
				bool flag = false;
				if (itemSlot.IsEmpty() && Character.localCharacter.data.currentItem)
				{
					if (Character.localCharacter.data.currentItem)
					{
						this.chosenItemText.text = "stash<br>" + Character.localCharacter.data.currentItem.GetItemName(null);
						flag = true;
					}
				}
				else
				{
					Item prefab = itemSlot.prefab;
					if (prefab != null)
					{
						this.chosenItemText.text = "take<br>" + prefab.GetItemName(itemSlot.data);
						flag = true;
					}
				}
				if (flag)
				{
					this.chosenSlice = Optionable<BackpackWheelSlice.SliceData>.Some(sliceData);
				}
				return;
			}
			Item currentItem = Character.localCharacter.data.currentItem;
			if (currentItem != null)
			{
				this.chosenItemText.text = "stash<br>" + currentItem.GetItemName(null);
				this.chosenSlice = Optionable<BackpackWheelSlice.SliceData>.Some(sliceData);
				return;
			}
			this.chosenItemText.text = "";
			this.chosenSlice = Optionable<BackpackWheelSlice.SliceData>.None;
			return;
		}
	}

	// Token: 0x06000969 RID: 2409 RVA: 0x0002F934 File Offset: 0x0002DB34
	public void Dehover(BackpackWheelSlice.SliceData sliceData)
	{
		if (this.chosenSlice.IsSome && this.chosenSlice.Value.Equals(sliceData))
		{
			this.chosenItemText.text = "";
			this.chosenSlice = Optionable<BackpackWheelSlice.SliceData>.None;
		}
	}

	// Token: 0x0600096A RID: 2410 RVA: 0x0002F980 File Offset: 0x0002DB80
	protected override void TestSelectSliceGamepad(Vector2 gamepadVector)
	{
		float num = 0f;
		BackpackWheelSlice backpackWheelSlice = null;
		if (gamepadVector.sqrMagnitude >= 0.5f)
		{
			for (int i = 0; i < this.slices.Length; i++)
			{
				float num2 = Vector3.Angle(gamepadVector, this.slices[i].GetUpVector());
				if (backpackWheelSlice == null || num2 < num)
				{
					backpackWheelSlice = this.slices[i];
					num = num2;
				}
			}
		}
		if (backpackWheelSlice != null)
		{
			EventSystem.current.SetSelectedGameObject(backpackWheelSlice.button.gameObject);
			backpackWheelSlice.Hover();
			return;
		}
		EventSystem.current.SetSelectedGameObject(null);
		this.Dehover(this.chosenSlice.Value);
	}

	// Token: 0x04000851 RID: 2129
	public BackpackWheelSlice[] slices;

	// Token: 0x04000852 RID: 2130
	public TextMeshProUGUI chosenItemText;

	// Token: 0x04000853 RID: 2131
	public Optionable<BackpackWheelSlice.SliceData> chosenSlice;

	// Token: 0x04000854 RID: 2132
	public BackpackReference backpack;

	// Token: 0x04000855 RID: 2133
	public RawImage currentlyHeldItem;

	// Token: 0x04000856 RID: 2134
	private int currentlyHeldItemCookedAmount;
}
