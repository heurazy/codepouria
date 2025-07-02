using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Zorro.Core;

// Token: 0x02000216 RID: 534
public class PlayerCustomizationDummy : MonoBehaviour
{
	// Token: 0x06000DC0 RID: 3520 RVA: 0x000455A4 File Offset: 0x000437A4
	public void UpdateDummy()
	{
		PersistentPlayerData playerData = GameHandler.GetService<PersistentPlayerDataService>().GetPlayerData(PhotonNetwork.LocalPlayer);
		this.SetPlayerColor(playerData.customizationData.currentSkin);
		this.SetPlayerCostume(playerData.customizationData.currentOutfit);
		this.SetPlayerHat(playerData.customizationData.currentHat);
		for (int i = 0; i < this.refs.EyeRenderers.Length; i++)
		{
			this.refs.EyeRenderers[i].material.SetTexture(PlayerCustomizationDummy.MainTex, Singleton<Customization>.Instance.eyes[playerData.customizationData.currentEyes].texture);
		}
		this.refs.accessoryRenderer.material.SetTexture(PlayerCustomizationDummy.MainTex, Singleton<Customization>.Instance.accessories[playerData.customizationData.currentAccessory].texture);
		this.refs.mouthRenderer.material.SetTexture(PlayerCustomizationDummy.MainTex, Singleton<Customization>.Instance.mouths[playerData.customizationData.currentMouth].texture);
	}

	// Token: 0x06000DC1 RID: 3521 RVA: 0x000456B0 File Offset: 0x000438B0
	public void SetPlayerCostume(int index)
	{
		this.refs.mainRenderer.sharedMesh = Singleton<Customization>.Instance.fits[index].fitMesh;
		List<Material> list = new List<Material>();
		list.Add(this.refs.mainRenderer.materials[0]);
		list.Add(Singleton<Customization>.Instance.fits[index].fitMaterial);
		list.Add(Singleton<Customization>.Instance.fits[index].fitMaterialShoes);
		this.refs.mainRenderer.SetSharedMaterials(list);
		if (Singleton<Customization>.Instance.fits[index].isSkirt)
		{
			this.refs.skirt.gameObject.SetActive(true);
			this.refs.shorts.gameObject.SetActive(false);
			this.refs.skirt.sharedMaterial = Singleton<Customization>.Instance.fits[index].fitPantsMaterial;
		}
		else
		{
			this.refs.skirt.gameObject.SetActive(false);
			this.refs.shorts.gameObject.SetActive(true);
			this.refs.shorts.sharedMaterial = Singleton<Customization>.Instance.fits[index].fitPantsMaterial;
		}
		this.refs.playerHats[0].material = Singleton<Customization>.Instance.fits[index].fitHatMaterial;
		this.refs.playerHats[1].material = Singleton<Customization>.Instance.fits[index].fitHatMaterial;
	}

	// Token: 0x06000DC2 RID: 3522 RVA: 0x00045834 File Offset: 0x00043A34
	public void SetPlayerHat(int index)
	{
		for (int i = 0; i < this.refs.playerHats.Length; i++)
		{
			this.refs.playerHats[i].gameObject.SetActive(index == i);
		}
	}

	// Token: 0x06000DC3 RID: 3523 RVA: 0x00045874 File Offset: 0x00043A74
	public void SetPlayerColor(int index)
	{
		if (index > Singleton<Customization>.Instance.skins.Length)
		{
			return;
		}
		for (int i = 0; i < this.refs.PlayerRenderers.Length; i++)
		{
			this.refs.PlayerRenderers[i].material.SetColor(PlayerCustomizationDummy.SkinColor, Singleton<Customization>.Instance.skins[index].color);
		}
		for (int j = 0; j < this.refs.EyeRenderers.Length; j++)
		{
			this.refs.EyeRenderers[j].material.SetColor(PlayerCustomizationDummy.SkinColor, Singleton<Customization>.Instance.skins[index].color);
		}
	}

	// Token: 0x04000CD4 RID: 3284
	private static readonly int MainTex = Shader.PropertyToID("_MainTex");

	// Token: 0x04000CD5 RID: 3285
	private static readonly int SkinColor = Shader.PropertyToID("_SkinColor");

	// Token: 0x04000CD6 RID: 3286
	public CustomizationRefs refs;
}
