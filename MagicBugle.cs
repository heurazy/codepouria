using System;
using UnityEngine;
using UnityEngine.UI.Extensions;

// Token: 0x020000E1 RID: 225
public class MagicBugle : ItemComponent
{
	// Token: 0x1700005D RID: 93
	// (get) Token: 0x060006DF RID: 1759 RVA: 0x00024019 File Offset: 0x00022219
	public float currentFuel
	{
		get
		{
			return this.fuel;
		}
	}

	// Token: 0x060006E0 RID: 1760 RVA: 0x00024024 File Offset: 0x00022224
	public override void Awake()
	{
		base.Awake();
		Item item = this.item;
		item.OnPrimaryStarted = (Action)Delegate.Combine(item.OnPrimaryStarted, new Action(this.StartToot));
		Item item2 = this.item;
		item2.OnPrimaryCancelled = (Action)Delegate.Combine(item2.OnPrimaryCancelled, new Action(this.CancelToot));
	}

	// Token: 0x060006E1 RID: 1761 RVA: 0x00024088 File Offset: 0x00022288
	public void OnDestroy()
	{
		Item item = this.item;
		item.OnPrimaryHeld = (Action)Delegate.Remove(item.OnPrimaryHeld, new Action(this.StartToot));
		Item item2 = this.item;
		item2.OnPrimaryCancelled = (Action)Delegate.Remove(item2.OnPrimaryCancelled, new Action(this.CancelToot));
	}

	// Token: 0x060006E2 RID: 1762 RVA: 0x000240E4 File Offset: 0x000222E4
	public override void OnInstanceDataSet()
	{
		if (base.HasData(DataEntryKey.Fuel))
		{
			this.fuel = base.GetData<FloatItemData>(DataEntryKey.Fuel).Value;
			this.item.SetUseRemainingPercentage(this.fuel / this.totalTootTime);
			return;
		}
		if (this.photonView.IsMine)
		{
			this.fuel = this.totalTootTime;
			this.item.SetUseRemainingPercentage(1f);
		}
	}

	// Token: 0x060006E3 RID: 1763 RVA: 0x00024150 File Offset: 0x00022350
	private void Update()
	{
		this.UpdateToot();
	}

	// Token: 0x060006E4 RID: 1764 RVA: 0x00024158 File Offset: 0x00022358
	private void UpdateToot()
	{
		if (this.tooting && this.photonView.IsMine)
		{
			this.fuel -= Time.deltaTime;
			if (this.fuel <= 0f)
			{
				this.fuel = 0f;
				this.CancelToot();
			}
			else
			{
				this.tootTick -= Time.deltaTime;
				if (this.tootTick <= 0f)
				{
					this.massAffliction.RunAction();
					this.tootTick = 0.1f;
				}
			}
			base.GetData<FloatItemData>(DataEntryKey.Fuel).Value = this.fuel;
			this.item.SetUseRemainingPercentage(this.fuel / this.totalTootTime);
		}
	}

	// Token: 0x060006E5 RID: 1765 RVA: 0x00024214 File Offset: 0x00022414
	private void StartToot()
	{
		Debug.Log("Started toot");
		if (this.fuel >= this.initialTootCost)
		{
			this.fuel -= this.initialTootCost;
			this.tooting = true;
			this.item.SetUseRemainingPercentage(this.fuel / this.totalTootTime);
		}
	}

	// Token: 0x060006E6 RID: 1766 RVA: 0x0002426B File Offset: 0x0002246B
	private void CancelToot()
	{
		Debug.Log("Cancelled toot");
		this.tooting = false;
	}

	// Token: 0x04000676 RID: 1654
	public float initialTootCost;

	// Token: 0x04000677 RID: 1655
	public float totalTootTime;

	// Token: 0x04000678 RID: 1656
	private bool tooting;

	// Token: 0x04000679 RID: 1657
	[SerializeField]
	[ReadOnly]
	private float fuel;

	// Token: 0x0400067A RID: 1658
	public Action_ApplyMassAffliction massAffliction;

	// Token: 0x0400067B RID: 1659
	private float tootTick;
}
