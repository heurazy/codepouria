using System;
using HorizonBasedAmbientOcclusion.Universal;
using UnityEngine;
using UnityEngine.Rendering;
using Zorro.Settings;

// Token: 0x0200010C RID: 268
public class PostHandler : MonoBehaviour
{
	// Token: 0x060007E3 RID: 2019 RVA: 0x00029DBD File Offset: 0x00027FBD
	private void Start()
	{
		this.AOSetting = GameHandler.Instance.SettingsHandler.GetSetting<AOSetting>();
	}

	// Token: 0x060007E4 RID: 2020 RVA: 0x00029DD4 File Offset: 0x00027FD4
	private void LateUpdate()
	{
		HBAO hbao;
		if (this.volume.sharedProfile.TryGet<HBAO>(out hbao))
		{
			hbao.active = this.AOSetting.Value == OffOnMode.ON;
		}
	}

	// Token: 0x04000763 RID: 1891
	public AOSetting AOSetting;

	// Token: 0x04000764 RID: 1892
	public Volume volume;
}
