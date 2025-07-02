using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x0200014A RID: 330
public class BackpackWheelSlice : UIWheelSlice, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	// Token: 0x17000078 RID: 120
	// (get) Token: 0x0600096C RID: 2412 RVA: 0x0002FA30 File Offset: 0x0002DC30
	// (set) Token: 0x0600096D RID: 2413 RVA: 0x0002FA38 File Offset: 0x0002DC38
	public byte backpackSlot { get; private set; }

	// Token: 0x0600096E RID: 2414 RVA: 0x0002FA41 File Offset: 0x0002DC41
	private void UpdateInteractable()
	{
		this.button.interactable = this.canInteract;
	}

	// Token: 0x17000079 RID: 121
	// (get) Token: 0x0600096F RID: 2415 RVA: 0x0002FA54 File Offset: 0x0002DC54
	private bool canInteract
	{
		get
		{
			return this.isBackpackWear || this.stashSlice || this.hasItem || Character.localCharacter.data.currentItem != null;
		}
	}

	// Token: 0x06000970 RID: 2416 RVA: 0x0002FA8C File Offset: 0x0002DC8C
	public void InitItemSlot([TupleElementNames(new string[] { null, "slotID" })] ValueTuple<BackpackReference, byte> slot, BackpackWheel wheel)
	{
		this.SharedInit(slot.Item1, wheel);
		this.backpackSlot = slot.Item2;
		this.backpackData = this.backpack.GetData();
		this.itemSlot = this.backpackData.itemSlots[(int)this.backpackSlot];
		Item prefab = this.itemSlot.prefab;
		this.SetItemIcon(prefab, this.itemSlot.data);
		this.UpdateInteractable();
	}

	// Token: 0x06000971 RID: 2417 RVA: 0x0002FB01 File Offset: 0x0002DD01
	public void InitPickupBackpack(BackpackReference backpack, BackpackWheel wheel)
	{
		this.backpackSlot = byte.MaxValue;
		this.SharedInit(backpack, wheel);
		this.UpdateInteractable();
	}

	// Token: 0x06000972 RID: 2418 RVA: 0x0002FB1C File Offset: 0x0002DD1C
	public void InitStashSlot(BackpackReference bpRef, BackpackWheel wheel)
	{
		this.backpack = bpRef;
		this.backpackWheel = wheel;
		this.SetItemIcon(Character.localCharacter.data.currentItem, (Character.localCharacter.data.currentItem != null) ? Character.localCharacter.data.currentItem.data : null);
		this.UpdateInteractable();
	}

	// Token: 0x06000973 RID: 2419 RVA: 0x0002FB80 File Offset: 0x0002DD80
	private void SharedInit(BackpackReference bpRef, BackpackWheel wheel)
	{
		this.backpack = bpRef;
		this.backpackWheel = wheel;
		if (bpRef.type == BackpackReference.BackpackType.Item)
		{
			Backpack component = Resources.Load<GameObject>("0_Items/Backpack").GetComponent<Backpack>();
			if (this.backpackSlot == 255)
			{
				base.gameObject.SetActive(true);
			}
			this.SetItemIcon(component, null);
			return;
		}
		this.SetItemIcon(null, null);
		if (this.backpackSlot == 255)
		{
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x06000974 RID: 2420 RVA: 0x0002FBF8 File Offset: 0x0002DDF8
	private void SetItemIcon(Item iconHolder, ItemInstanceData itemInstanceData)
	{
		if (iconHolder == null)
		{
			this.image.enabled = false;
			this.hasItem = false;
		}
		else
		{
			this.image.enabled = true;
			this.image.texture = iconHolder.UIData.icon;
			this.hasItem = true;
		}
		this.UpdateCookedAmount(iconHolder, itemInstanceData);
	}

	// Token: 0x06000975 RID: 2421 RVA: 0x0002FC54 File Offset: 0x0002DE54
	private void UpdateCookedAmount(Item item, ItemInstanceData itemInstanceData)
	{
		if (item == null || itemInstanceData == null)
		{
			this.cookedAmount = 0;
			this.image.color = Color.white;
			return;
		}
		IntItemData intItemData;
		if (itemInstanceData.TryGetDataEntry<IntItemData>(DataEntryKey.CookedAmount, out intItemData) && this.cookedAmount != intItemData.Value)
		{
			this.image.color = Color.white;
			this.image.color = ItemCooking.GetCookColor(intItemData.Value);
			this.cookedAmount = intItemData.Value;
		}
	}

	// Token: 0x1700007A RID: 122
	// (get) Token: 0x06000976 RID: 2422 RVA: 0x0002FCD0 File Offset: 0x0002DED0
	public bool isBackpackWear
	{
		get
		{
			return this.backpackSlot == byte.MaxValue;
		}
	}

	// Token: 0x06000977 RID: 2423 RVA: 0x0002FCE0 File Offset: 0x0002DEE0
	public void Hover()
	{
		BackpackWheelSlice.SliceData sliceData = new BackpackWheelSlice.SliceData
		{
			isBackpackWear = this.isBackpackWear,
			isStashSlice = this.stashSlice,
			backpackReference = this.backpack,
			slotID = this.backpackSlot
		};
		this.backpackWheel.Hover(sliceData);
	}

	// Token: 0x06000978 RID: 2424 RVA: 0x0002FD38 File Offset: 0x0002DF38
	public void Dehover()
	{
		BackpackWheelSlice.SliceData sliceData = new BackpackWheelSlice.SliceData
		{
			isBackpackWear = (this.backpackSlot == byte.MaxValue),
			isStashSlice = this.stashSlice,
			backpackReference = this.backpack,
			slotID = this.backpackSlot
		};
		this.backpackWheel.Dehover(sliceData);
	}

	// Token: 0x06000979 RID: 2425 RVA: 0x0002FD96 File Offset: 0x0002DF96
	public void OnPointerEnter(PointerEventData eventData)
	{
		this.Hover();
	}

	// Token: 0x0600097A RID: 2426 RVA: 0x0002FD9E File Offset: 0x0002DF9E
	public void OnPointerExit(PointerEventData eventData)
	{
		this.Dehover();
	}

	// Token: 0x04000857 RID: 2135
	private BackpackWheel backpackWheel;

	// Token: 0x04000859 RID: 2137
	private BackpackReference backpack;

	// Token: 0x0400085A RID: 2138
	private BackpackData backpackData;

	// Token: 0x0400085B RID: 2139
	private ItemSlot itemSlot;

	// Token: 0x0400085C RID: 2140
	public RawImage image;

	// Token: 0x0400085D RID: 2141
	public bool stashSlice;

	// Token: 0x0400085E RID: 2142
	private int cookedAmount;

	// Token: 0x0400085F RID: 2143
	private bool hasItem;

	// Token: 0x0200036A RID: 874
	public struct SliceData : IEquatable<BackpackWheelSlice.SliceData>
	{
		// Token: 0x060013C4 RID: 5060 RVA: 0x0005DA2B File Offset: 0x0005BC2B
		public bool Equals(BackpackWheelSlice.SliceData other)
		{
			return this.isBackpackWear == other.isBackpackWear && this.slotID == other.slotID;
		}

		// Token: 0x060013C5 RID: 5061 RVA: 0x0005DA4C File Offset: 0x0005BC4C
		public override bool Equals(object obj)
		{
			if (obj is BackpackWheelSlice.SliceData)
			{
				BackpackWheelSlice.SliceData sliceData = (BackpackWheelSlice.SliceData)obj;
				return this.Equals(sliceData);
			}
			return false;
		}

		// Token: 0x060013C6 RID: 5062 RVA: 0x0005DA71 File Offset: 0x0005BC71
		public override int GetHashCode()
		{
			return HashCode.Combine<bool, BackpackReference, byte>(this.isBackpackWear, this.backpackReference, this.slotID);
		}

		// Token: 0x04001295 RID: 4757
		public bool isBackpackWear;

		// Token: 0x04001296 RID: 4758
		public bool isStashSlice;

		// Token: 0x04001297 RID: 4759
		public BackpackReference backpackReference;

		// Token: 0x04001298 RID: 4760
		public byte slotID;
	}
}
