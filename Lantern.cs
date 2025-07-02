using System;
using DG.Tweening;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200001F RID: 31
public class Lantern : ItemComponent
{
	// Token: 0x0600020D RID: 525 RVA: 0x0000EE22 File Offset: 0x0000D022
	public override void Awake()
	{
		base.Awake();
		this.item = base.GetComponent<Item>();
	}

	// Token: 0x0600020E RID: 526 RVA: 0x0000EE36 File Offset: 0x0000D036
	public override void OnEnable()
	{
		Item item = this.item;
		item.onStashAction = (Action)Delegate.Combine(item.onStashAction, new Action(this.SnuffLantern));
	}

	// Token: 0x0600020F RID: 527 RVA: 0x0000EE5F File Offset: 0x0000D05F
	public override void OnDisable()
	{
		Item item = this.item;
		item.onStashAction = (Action)Delegate.Remove(item.onStashAction, new Action(this.SnuffLantern));
	}

	// Token: 0x06000210 RID: 528 RVA: 0x0000EE88 File Offset: 0x0000D088
	private void Start()
	{
		if (base.HasData(DataEntryKey.FlareActive) && base.GetData<BoolItemData>(DataEntryKey.FlareActive).Value)
		{
			this.fireParticle.main.prewarm = true;
			this.fireParticle.Play();
		}
	}

	// Token: 0x06000211 RID: 529 RVA: 0x0000EECC File Offset: 0x0000D0CC
	public override void OnInstanceDataSet()
	{
		if (base.HasData(DataEntryKey.FlareActive))
		{
			this.lit = base.GetData<BoolItemData>(DataEntryKey.FlareActive).Value;
		}
		this.fuel = base.GetData<FloatItemData>(DataEntryKey.Fuel, new Func<FloatItemData>(this.SetupDefaultFuel)).Value;
		this.item.SetUseRemainingPercentage(this.fuel / this.startingFuel);
	}

	// Token: 0x06000212 RID: 530 RVA: 0x0000EF2C File Offset: 0x0000D12C
	private void Update()
	{
		if (!PhotonNetwork.InRoom)
		{
			return;
		}
		if (this.lanternLight.gameObject.activeSelf != this.lit)
		{
			this.lanternLight.gameObject.SetActive(this.lit);
			if (this.lit)
			{
				this.fireParticle.Play();
				this.lanternLight.intensity = 0f;
				this.lanternLight.DOIntensity(this.lightIntensity, 0.5f);
			}
			else
			{
				this.fireParticle.Clear();
				this.fireParticle.Stop();
			}
		}
		this.item.UIData.mainInteractPrompt = (this.lit ? this.actionPromptWhenLit : this.actionPromptWhenUnlit);
		this.item.usingTimePrimary = (this.lit ? this.useTimeWhenLit : this.useTimeWhenUnlit);
		base.GetData<OptionableIntItemData>(DataEntryKey.ItemUses).Value = ((this.fuel > 0f) ? (-1) : 0);
		this.UpdateFuel();
	}

	// Token: 0x06000213 RID: 531 RVA: 0x0000F02C File Offset: 0x0000D22C
	private void UpdateFuel()
	{
		if (this.lit && this.photonView.IsMine)
		{
			this.fuel -= Time.deltaTime;
			if (this.fuel <= 0f)
			{
				this.fuel = 0f;
				this.SnuffLantern();
			}
			base.GetData<FloatItemData>(DataEntryKey.Fuel, new Func<FloatItemData>(this.SetupDefaultFuel)).Value = this.fuel;
			this.item.SetUseRemainingPercentage(this.fuel / this.startingFuel);
		}
	}

	// Token: 0x06000214 RID: 532 RVA: 0x0000F0B5 File Offset: 0x0000D2B5
	private FloatItemData SetupDefaultFuel()
	{
		return new FloatItemData
		{
			Value = this.startingFuel
		};
	}

	// Token: 0x06000215 RID: 533 RVA: 0x0000F0C8 File Offset: 0x0000D2C8
	public void ToggleLantern()
	{
		this.photonView.RPC("LightLanternRPC", RpcTarget.All, new object[] { !this.lit });
	}

	// Token: 0x06000216 RID: 534 RVA: 0x0000F0F2 File Offset: 0x0000D2F2
	public void SnuffLantern()
	{
		this.photonView.RPC("LightLanternRPC", RpcTarget.All, new object[] { false });
	}

	// Token: 0x06000217 RID: 535 RVA: 0x0000F114 File Offset: 0x0000D314
	[PunRPC]
	public void LightLanternRPC(bool litValue)
	{
		this.fireParticle.main.prewarm = false;
		this.lit = litValue;
		base.GetData<BoolItemData>(DataEntryKey.FlareActive).Value = this.lit;
	}

	// Token: 0x040001FE RID: 510
	[SerializeField]
	private bool lit;

	// Token: 0x040001FF RID: 511
	public string actionPromptWhenUnlit;

	// Token: 0x04000200 RID: 512
	public string actionPromptWhenLit;

	// Token: 0x04000201 RID: 513
	public float useTimeWhenUnlit;

	// Token: 0x04000202 RID: 514
	public float useTimeWhenLit;

	// Token: 0x04000203 RID: 515
	public Light lanternLight;

	// Token: 0x04000204 RID: 516
	public float lightIntensity = 10f;

	// Token: 0x04000205 RID: 517
	public float startingFuel;

	// Token: 0x04000206 RID: 518
	[SerializeField]
	private float fuel;

	// Token: 0x04000207 RID: 519
	public ParticleSystem fireParticle;

	// Token: 0x04000208 RID: 520
	private new Item item;
}
