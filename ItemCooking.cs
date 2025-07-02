using System;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;
using Zorro.Core;

// Token: 0x0200009F RID: 159
public class ItemCooking : ItemComponent
{
	// Token: 0x17000059 RID: 89
	// (get) Token: 0x060005C6 RID: 1478 RVA: 0x00020569 File Offset: 0x0001E769
	public bool canBeCooked
	{
		get
		{
			return !this.disableCooking;
		}
	}

	// Token: 0x060005C7 RID: 1479 RVA: 0x00020574 File Offset: 0x0001E774
	public override void OnInstanceDataSet()
	{
		this.UpdateCookedBehavior();
	}

	// Token: 0x060005C8 RID: 1480 RVA: 0x0002057C File Offset: 0x0001E77C
	public virtual void UpdateCookedBehavior()
	{
		IntItemData data = this.item.GetData<IntItemData>(DataEntryKey.CookedAmount);
		if (!this.setup)
		{
			this.setup = true;
			this.renderers = base.GetComponentsInChildren<MeshRenderer>();
			this.defaultTints = new Color[this.renderers.Length];
			for (int i = 0; i < this.renderers.Length; i++)
			{
				this.defaultTints[i] = this.renderers[i].material.GetColor("_Tint");
			}
		}
		int num = data.Value - this.timesCookedLocal;
		this.CookVisually(data.Value);
		if (num > 0)
		{
			this.ChangeStatsCooked(data.Value);
		}
		this.timesCookedLocal = data.Value;
	}

	// Token: 0x060005C9 RID: 1481 RVA: 0x00020630 File Offset: 0x0001E830
	protected virtual void CookVisually(int cookedAmount)
	{
		for (int i = 0; i < this.renderers.Length; i++)
		{
			if (cookedAmount > 0)
			{
				Debug.Log(string.Format("Cooked amount is {0}", cookedAmount));
				this.renderers[i].material.SetColor("_Tint", this.defaultTints[i] * ItemCooking.GetCookColor(cookedAmount));
			}
		}
	}

	// Token: 0x060005CA RID: 1482 RVA: 0x00020698 File Offset: 0x0001E898
	public static Color GetCookColor(int cookAmount)
	{
		Color color = Color.white;
		if (cookAmount == 1)
		{
			color = ItemCooking.DefaultCookColorMultiplier;
		}
		else if (cookAmount == 2)
		{
			color = ItemCooking.DefaultCookColorMultiplier * 0.5f;
		}
		else if (cookAmount > 2)
		{
			color = ItemCooking.BurntCookColorMultiplier;
		}
		color.a = 1f;
		return color;
	}

	// Token: 0x060005CB RID: 1483 RVA: 0x000206E4 File Offset: 0x0001E8E4
	[PunRPC]
	private void FinishCookingRPC()
	{
		this.CancelCookingVisuals();
		IntItemData data = base.GetData<IntItemData>(DataEntryKey.CookedAmount);
		if (this.wreckWhenCooked)
		{
			data.Value = 5;
		}
		else if (data.Value < 12)
		{
			data.Value++;
		}
		this.item.WasActive();
		this.UpdateCookedBehavior();
	}

	// Token: 0x060005CC RID: 1484 RVA: 0x00020739 File Offset: 0x0001E939
	public void StartCookingVisuals()
	{
		this.photonView.RPC("EnableCookingSmokeRPC", RpcTarget.All, new object[] { true });
	}

	// Token: 0x060005CD RID: 1485 RVA: 0x0002075B File Offset: 0x0001E95B
	[PunRPC]
	private void EnableCookingSmokeRPC(bool active)
	{
		this.item.particles.EnableSmoke(active);
	}

	// Token: 0x060005CE RID: 1486 RVA: 0x00020770 File Offset: 0x0001E970
	private void ChangeStatsCooked(int totalCooked)
	{
		if (this.wreckWhenCooked && totalCooked > 0)
		{
			ItemComponent[] components = base.GetComponents<ItemComponent>();
			for (int i = components.Length - 1; i >= 0; i--)
			{
				if (components[i] != this)
				{
					Object.Destroy(components[i]);
				}
			}
			ItemAction[] components2 = base.GetComponents<ItemAction>();
			for (int j = components2.Length - 1; j >= 0; j--)
			{
				Object.Destroy(components2[j]);
			}
			this.item.overrideUsability = Optionable<bool>.Some(false);
			return;
		}
		Action_RestoreHunger component = base.GetComponent<Action_RestoreHunger>();
		if (component)
		{
			if (totalCooked < 2)
			{
				component.restorationAmount *= 2f;
			}
			else if (totalCooked > 2)
			{
				component.restorationAmount = Mathf.Max(component.restorationAmount - 0.05f, 0f);
			}
		}
		Action_GiveExtraStamina action_GiveExtraStamina = base.GetComponent<Action_GiveExtraStamina>();
		if (!action_GiveExtraStamina)
		{
			action_GiveExtraStamina = base.gameObject.AddComponent<Action_GiveExtraStamina>();
			action_GiveExtraStamina.OnConsumed = true;
		}
		if (totalCooked < 2)
		{
			action_GiveExtraStamina.amount = Mathf.Max(0.1f, action_GiveExtraStamina.amount * 1.5f);
		}
		else if (totalCooked > 2)
		{
			action_GiveExtraStamina.amount = 0f;
		}
		Action_ModifyStatus action_ModifyStatus = base.GetComponents<Action_ModifyStatus>().FirstOrDefault((Action_ModifyStatus a) => a.statusType == CharacterAfflictions.STATUSTYPE.Poison);
		base.GetComponent<Action_InflictPoison>();
		if (totalCooked > 3)
		{
			if (!action_ModifyStatus)
			{
				action_ModifyStatus = base.gameObject.AddComponent<Action_ModifyStatus>();
				action_ModifyStatus.OnConsumed = true;
				action_ModifyStatus.statusType = CharacterAfflictions.STATUSTYPE.Poison;
			}
			action_ModifyStatus.changeAmount += 0.1f;
		}
	}

	// Token: 0x060005CF RID: 1487 RVA: 0x000208F6 File Offset: 0x0001EAF6
	public void CancelCookingVisuals()
	{
		this.photonView.RPC("EnableCookingSmokeRPC", RpcTarget.All, new object[] { false });
	}

	// Token: 0x060005D0 RID: 1488 RVA: 0x00020918 File Offset: 0x0001EB18
	public void FinishCooking()
	{
		if (!this.photonView.AmController)
		{
			return;
		}
		this.photonView.RPC("FinishCookingRPC", RpcTarget.All, Array.Empty<object>());
		if (this.item.holderCharacter)
		{
			Action<ItemSlot[]> itemsChangedAction = this.item.holderCharacter.player.itemsChangedAction;
			if (itemsChangedAction != null)
			{
				itemsChangedAction(this.item.holderCharacter.player.itemSlots);
			}
			if (this.item.holderCharacter.GetComponent<CharacterItems>() && this.item.holderCharacter.GetComponent<CharacterItems>().cookSfx)
			{
				this.item.holderCharacter.GetComponent<CharacterItems>().cookSfx.Play(base.transform.position);
			}
		}
		Debug.Log("Cooking Finished");
	}

	// Token: 0x040005CF RID: 1487
	protected int timesCookedLocal;

	// Token: 0x040005D0 RID: 1488
	[SerializeField]
	protected bool disableCooking;

	// Token: 0x040005D1 RID: 1489
	[FormerlySerializedAs("burnInstantly")]
	public bool wreckWhenCooked;

	// Token: 0x040005D2 RID: 1490
	private MeshRenderer[] renderers;

	// Token: 0x040005D3 RID: 1491
	private Color[] defaultTints;

	// Token: 0x040005D4 RID: 1492
	private bool setup;

	// Token: 0x040005D5 RID: 1493
	public static Color DefaultCookColorMultiplier = new Color(0.66f, 0.47f, 0.25f);

	// Token: 0x040005D6 RID: 1494
	public static Color BurntCookColorMultiplier = new Color(0.05f, 0.05f, 0.1f);

	// Token: 0x040005D7 RID: 1495
	public const int COOKING_MAX = 12;
}
