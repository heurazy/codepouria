using System;
using Photon.Pun;
using UnityEngine;
using Zorro.Core;

// Token: 0x020000D6 RID: 214
public class Flare : ItemComponent
{
	// Token: 0x0600068F RID: 1679 RVA: 0x00022F26 File Offset: 0x00021126
	public override void Awake()
	{
		base.Awake();
		this.trackable = base.GetComponent<TrackableNetworkObject>();
	}

	// Token: 0x06000690 RID: 1680 RVA: 0x00022F3A File Offset: 0x0002113A
	public override void OnInstanceDataSet()
	{
		if (base.HasData(DataEntryKey.Color))
		{
			this.flareColor = base.GetData<ColorItemData>(DataEntryKey.Color).Value;
		}
	}

	// Token: 0x06000691 RID: 1681 RVA: 0x00022F5C File Offset: 0x0002115C
	private void Update()
	{
		bool value = base.GetData<BoolItemData>(DataEntryKey.FlareActive).Value;
		this.item.UIData.canPocket = !value;
		if (value && !this.trackable.hasTracker)
		{
			this.EnableFlareVisuals();
		}
		if (value && Singleton<MountainProgressHandler>.Instance.IsAtPeak(base.transform) && !Singleton<PeakHandler>.Instance.summonedHelicopter)
		{
			base.GetComponent<PhotonView>().RPC("TriggerHelicopter", RpcTarget.AllBuffered, Array.Empty<object>());
		}
	}

	// Token: 0x06000692 RID: 1682 RVA: 0x00022FD7 File Offset: 0x000211D7
	[PunRPC]
	public void TriggerHelicopter()
	{
		Singleton<PeakHandler>.Instance.SummonHelicopter();
	}

	// Token: 0x06000693 RID: 1683 RVA: 0x00022FE3 File Offset: 0x000211E3
	public void LightFlare()
	{
		base.GetComponent<PhotonView>().RPC("SetFlareLitRPC", RpcTarget.AllBuffered, Array.Empty<object>());
	}

	// Token: 0x06000694 RID: 1684 RVA: 0x00022FFC File Offset: 0x000211FC
	[PunRPC]
	public void SetFlareLitRPC()
	{
		if (this.item.holderCharacter)
		{
			this.flareColor = this.item.holderCharacter.refs.customization.PlayerColor;
			this.flareColor.a = 1f;
			base.GetData<ColorItemData>(DataEntryKey.Color).Value = this.flareColor;
			string text = "Set flare color to ";
			Color value = base.GetData<ColorItemData>(DataEntryKey.Color).Value;
			Debug.Log(text + value.ToString());
		}
		base.GetData<BoolItemData>(DataEntryKey.FlareActive).Value = true;
	}

	// Token: 0x06000695 RID: 1685 RVA: 0x00023098 File Offset: 0x00021298
	public void EnableFlareVisuals()
	{
		Debug.Log(string.Format("Lighting flare with photon ID {0} with instance ID {1}", this.photonView.ViewID, this.trackable.instanceID));
		TrackNetworkedObject component = Object.Instantiate<TrackNetworkedObject>(this.flareVFXPrefab, base.transform.position, base.transform.rotation).GetComponent<TrackNetworkedObject>();
		component.SetObject(this.trackable);
		component.gameObject.GetComponent<ParticleSystem>().main.startColor = this.flareColor;
		string text = "Lit flare with color ";
		Color color = this.flareColor;
		Debug.Log(text + color.ToString());
	}

	// Token: 0x04000645 RID: 1605
	private TrackableNetworkObject trackable;

	// Token: 0x04000646 RID: 1606
	public TrackNetworkedObject flareVFXPrefab;

	// Token: 0x04000647 RID: 1607
	public Color flareColor;
}
