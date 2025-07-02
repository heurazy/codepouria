using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zorro.Core;

// Token: 0x02000159 RID: 345
public class InventoryItemUI : MonoBehaviour
{
	// Token: 0x060009D8 RID: 2520 RVA: 0x00031037 File Offset: 0x0002F237
	public void Start()
	{
		this.startingSizeDelta = this.rectTransform.sizeDelta;
	}

	// Token: 0x060009D9 RID: 2521 RVA: 0x0003104C File Offset: 0x0002F24C
	private void UpdateCookedAmount()
	{
		if (this._itemData == null)
		{
			this.cookedAmount = 0;
			this.icon.color = Color.white;
			return;
		}
		IntItemData intItemData;
		if (this._itemData.TryGetDataEntry<IntItemData>(DataEntryKey.CookedAmount, out intItemData) && this.cookedAmount != intItemData.Value)
		{
			this.icon.color = Color.white;
			this.icon.color = ItemCooking.GetCookColor(intItemData.Value);
			this.cookedAmount = intItemData.Value;
		}
	}

	// Token: 0x060009DA RID: 2522 RVA: 0x000310CC File Offset: 0x0002F2CC
	public void SetItem(ItemSlot slot)
	{
		if (this.isBackpack)
		{
			if (Character.observedCharacter.data.carriedPlayer)
			{
				this.icon.color = Character.observedCharacter.data.carriedPlayer.refs.customization.PlayerColor;
				this.icon.texture = this.carryingIcon;
				this.backpackFilledSlotsObject.SetActive(false);
				return;
			}
			this.icon.texture = this.backpackIcon;
			if (slot.IsEmpty())
			{
				this._hasBackpack = false;
				this.icon.color = new Color(0f, 0f, 0f, 0.5f);
				this.backpackFilledSlotsObject.SetActive(false);
				return;
			}
			this._hasBackpack = true;
			this.icon.color = Color.white;
			BackpackData backpackData;
			if (this.backpackFilledSlotsObject != null && slot.data.TryGetDataEntry<BackpackData>(DataEntryKey.BackpackData, out backpackData))
			{
				int num = backpackData.FilledSlotCount();
				this.backpackFilledSlotsObject.SetActive(num > 0);
				this.backpackFilledSlotsAmountText.text = num.ToString();
			}
			return;
		}
		else
		{
			this.UpdateNameText();
			this.UpdateCookedAmount();
			if (this._itemPrefab == slot.prefab)
			{
				this.TrySetFuel(slot.data);
				return;
			}
			this._itemPrefab = slot.prefab;
			this._itemData = slot.data;
			this.SetSelected();
			if (!slot.IsEmpty())
			{
				if (this._itemPrefab == null)
				{
					this.icon.transform.localScale = Vector3.zero;
					this.icon.transform.DOScale(1f, 0.5f).SetEase(Ease.OutElastic);
				}
				this.icon.texture = this._itemPrefab.UIData.icon;
				this.icon.enabled = true;
				this.TrySetFuel(slot.data);
				return;
			}
			this.fill.enabled = false;
			this.icon.enabled = false;
			this._itemPrefab = null;
			this._itemData = null;
			this.nameText.enabled = false;
			this.nameText.text = "";
			this.TrySetFuel(null);
			return;
		}
	}

	// Token: 0x060009DB RID: 2523 RVA: 0x00031304 File Offset: 0x0002F504
	public void TrySetFuel(ItemInstanceData data)
	{
		if (!this.fuelBar)
		{
			return;
		}
		if (Character.observedCharacter != Character.localCharacter)
		{
			this.fuelBar.SetActive(false);
			return;
		}
		if (data == null || this._itemPrefab == null || !data.HasData(DataEntryKey.UseRemainingPercentage))
		{
			this.fuelBar.SetActive(false);
			this.fuelBarFill.fillAmount = 1f;
			return;
		}
		this.fuelBar.SetActive(true);
		FloatItemData floatItemData;
		if (data.TryGetDataEntry<FloatItemData>(DataEntryKey.UseRemainingPercentage, out floatItemData))
		{
			this.fuelBarFill.fillAmount = floatItemData.Value;
		}
	}

	// Token: 0x060009DC RID: 2524 RVA: 0x000313A0 File Offset: 0x0002F5A0
	public void UpdateNameText()
	{
		string text;
		if (this._itemPrefab != null || (this.isBackpack && this._hasBackpack))
		{
			if (this._itemPrefab != null)
			{
				text = this._itemPrefab.GetItemName(this._itemData);
			}
			else
			{
				text = "Backpack";
			}
		}
		else
		{
			text = "";
		}
		if (this.nameText.text != text)
		{
			this.SetSelected();
		}
		this.nameText.text = text;
	}

	// Token: 0x060009DD RID: 2525 RVA: 0x00031420 File Offset: 0x0002F620
	public void SetSelected()
	{
		Optionable<byte> currentSelectedSlot = Character.observedCharacter.refs.items.currentSelectedSlot;
		bool flag = currentSelectedSlot.IsSome && (int)currentSelectedSlot.Value == base.transform.GetSiblingIndex();
		if (this.isTemporarySlot)
		{
			flag = true;
		}
		if (this.isBackpack)
		{
			flag = currentSelectedSlot.Value == 3;
		}
		if (this._itemPrefab != null || (this.isBackpack && this._hasBackpack) || this.isTemporarySlot)
		{
			if (flag)
			{
				this.mySequence.Kill(false);
				this.rectTransform.DOKill(false);
				this.rectTransform.DOSizeDelta(this.startingSizeDelta * 1.2f, 0.5f, false).SetEase(Ease.OutElastic);
				this.fill.enabled = true;
				this.fill.transform.localScale = Vector3.zero;
				this.fill.transform.DOKill(false);
				this.fill.transform.DOScale(1f, 0.25f).SetEase(Ease.OutCubic);
				this.nameText.enabled = true;
				return;
			}
			this.mySequence.Kill(false);
			this.rectTransform.DOKill(false);
			this.rectTransform.DOSizeDelta(this.startingSizeDelta, 0.2f, false).SetEase(Ease.OutCubic);
			this.fill.enabled = false;
			this.nameText.enabled = false;
			return;
		}
		else
		{
			if (flag)
			{
				this.mySequence.Kill(false);
				this.mySequence = DOTween.Sequence();
				this.mySequence.Append(this.rectTransform.DOSizeDelta(this.startingSizeDelta * 1.2f, 0.075f, false).SetEase(Ease.OutCubic));
				this.mySequence.Append(this.rectTransform.DOSizeDelta(this.startingSizeDelta, 0.125f, false).SetEase(Ease.InSine));
				return;
			}
			this.mySequence.Kill(false);
			this.rectTransform.DOKill(false);
			this.rectTransform.sizeDelta = this.startingSizeDelta;
			return;
		}
	}

	// Token: 0x060009DE RID: 2526 RVA: 0x00031648 File Offset: 0x0002F848
	private void OnDisable()
	{
		this.mySequence.Kill(false);
		this.rectTransform.DOKill(false);
		this.rectTransform.sizeDelta = this.startingSizeDelta;
		this.fill.enabled = false;
		this.nameText.enabled = false;
		this.nameText.text = "";
	}

	// Token: 0x040008C6 RID: 2246
	public RectTransform rectTransform;

	// Token: 0x040008C7 RID: 2247
	public RawImage icon;

	// Token: 0x040008C8 RID: 2248
	public Image fill;

	// Token: 0x040008C9 RID: 2249
	public Image selectedSlotIcon;

	// Token: 0x040008CA RID: 2250
	public Texture defaultIcon;

	// Token: 0x040008CB RID: 2251
	public TextMeshProUGUI nameText;

	// Token: 0x040008CC RID: 2252
	public bool isBackpack;

	// Token: 0x040008CD RID: 2253
	public GameObject backpackFilledSlotsObject;

	// Token: 0x040008CE RID: 2254
	public TextMeshProUGUI backpackFilledSlotsAmountText;

	// Token: 0x040008CF RID: 2255
	private Sequence mySequence;

	// Token: 0x040008D0 RID: 2256
	private Item _itemPrefab;

	// Token: 0x040008D1 RID: 2257
	private bool _hasBackpack;

	// Token: 0x040008D2 RID: 2258
	public GameObject fuelBar;

	// Token: 0x040008D3 RID: 2259
	public Image fuelBarFill;

	// Token: 0x040008D4 RID: 2260
	public Texture backpackIcon;

	// Token: 0x040008D5 RID: 2261
	public Texture carryingIcon;

	// Token: 0x040008D6 RID: 2262
	public ItemInstanceData _itemData;

	// Token: 0x040008D7 RID: 2263
	private int cookedAmount;

	// Token: 0x040008D8 RID: 2264
	public bool isTemporarySlot;

	// Token: 0x040008D9 RID: 2265
	private Vector2 startingSizeDelta;
}
