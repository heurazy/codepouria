using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Serialization;
using Zorro.Core;
using Zorro.Core.CLI;

// Token: 0x02000054 RID: 84
[ConsoleClassCustomizer("Customization")]
public class CharacterCustomization : MonoBehaviour
{
	// Token: 0x1700003C RID: 60
	// (get) Token: 0x0600038F RID: 911 RVA: 0x00015854 File Offset: 0x00013A54
	public Color PlayerColor
	{
		get
		{
			CharacterCustomizationData customizationData = CharacterCustomization.GetCustomizationData(this._character.photonView.Owner);
			return Singleton<Customization>.Instance.skins[customizationData.currentSkin].color;
		}
	}

	// Token: 0x06000390 RID: 912 RVA: 0x0001588D File Offset: 0x00013A8D
	private void Awake()
	{
		this.view = base.GetComponent<PhotonView>();
		this._character = base.GetComponent<Character>();
	}

	// Token: 0x06000391 RID: 913 RVA: 0x000158A8 File Offset: 0x00013AA8
	public void Start()
	{
		if (this.view.IsMine && !this._character.isBot)
		{
			this.SetRandomIdle();
			InRoomState inRoomState = GameHandler.GetService<ConnectionService>().StateMachine.CurrentState as InRoomState;
			if (inRoomState != null && !inRoomState.hasLoadedCustomization)
			{
				inRoomState.hasLoadedCustomization = true;
				base.StartCoroutine(this.GetCosmeticsFromSteamRoutine());
				if (this._character.IsLocal)
				{
					this.refs.mainRenderer.updateWhenOffscreen = true;
				}
			}
		}
		Character character = this._character;
		character.reviveAction = (Action)Delegate.Combine(character.reviveAction, new Action(this.OnRevive));
		Character character2 = this._character;
		character2.UnPassOutAction = (Action)Delegate.Combine(character2.UnPassOutAction, new Action(this.OnRevive));
		PersistentPlayerDataService service = GameHandler.GetService<PersistentPlayerDataService>();
		service.SubscribeToPlayerDataChange(this._character.photonView.Owner, new Action<PersistentPlayerData>(this.OnPlayerDataChange));
		this.OnPlayerDataChange(service.GetPlayerData(this.view.Owner));
	}

	// Token: 0x06000392 RID: 914 RVA: 0x000159B4 File Offset: 0x00013BB4
	private IEnumerator GetCosmeticsFromSteamRoutine()
	{
		while (!Singleton<AchievementManager>.Instance.gotStats)
		{
			yield return null;
		}
		this.TryGetCosmeticsFromSteam();
		yield break;
	}

	// Token: 0x06000393 RID: 915 RVA: 0x000159C4 File Offset: 0x00013BC4
	private void TryGetCosmeticsFromSteam()
	{
		int num;
		if (Singleton<AchievementManager>.Instance.GetSteamStatInt(STEAMSTATTYPE.LoadedCosmeticsPreviously, out num))
		{
			if (num > 0)
			{
				int num2;
				if (Singleton<AchievementManager>.Instance.GetSteamStatInt(STEAMSTATTYPE.Cosmetic_Skin, out num2) && num2 != -1)
				{
					CharacterCustomization.SetCharacterSkinColor(num2);
				}
				else
				{
					this.SetRandomSkinColor();
				}
				int num3;
				if (Singleton<AchievementManager>.Instance.GetSteamStatInt(STEAMSTATTYPE.Cosmetic_Eyes, out num3) && num3 != -1)
				{
					CharacterCustomization.SetCharacterEyes(num3);
				}
				else
				{
					this.SetRandomEyes();
				}
				int num4;
				if (Singleton<AchievementManager>.Instance.GetSteamStatInt(STEAMSTATTYPE.Cosmetic_Mouth, out num4) && num4 != -1)
				{
					CharacterCustomization.SetCharacterMouth(num4);
				}
				else
				{
					this.SetRandomMouth();
				}
				int num5;
				if (Singleton<AchievementManager>.Instance.GetSteamStatInt(STEAMSTATTYPE.Cosmetic_Accessory, out num5) && num4 != -1)
				{
					CharacterCustomization.SetCharacterAccessory(num5);
				}
				else
				{
					this.SetRandomAccessory();
				}
				int num6;
				if (Singleton<AchievementManager>.Instance.GetSteamStatInt(STEAMSTATTYPE.Cosmetic_Outfit, out num6) && num6 != -1)
				{
					CharacterCustomization.SetCharacterOutfit(num6);
				}
				else
				{
					this.SetRandomOutfit();
				}
				int num7;
				if (Singleton<AchievementManager>.Instance.GetSteamStatInt(STEAMSTATTYPE.Cosmetic_Hat, out num7) && num7 != -1)
				{
					CharacterCustomization.SetCharacterHat(num7);
				}
				else
				{
					this.SetRandomHat();
				}
				int num8;
				if (Singleton<AchievementManager>.Instance.GetSteamStatInt(STEAMSTATTYPE.MaxAscent, out num8))
				{
					CharacterCustomization.SetCharacterSash(num8);
				}
			}
			else
			{
				this.RandomizeCosmetics();
			}
			Singleton<AchievementManager>.Instance.SetSteamStat(STEAMSTATTYPE.LoadedCosmeticsPreviously, 1);
			return;
		}
		this.SetRandomSkinColor();
	}

	// Token: 0x06000394 RID: 916 RVA: 0x00015AEB File Offset: 0x00013CEB
	[ConsoleCommand]
	public static void Randomize()
	{
		Character.localCharacter.refs.customization.RandomizeCosmetics();
	}

	// Token: 0x06000395 RID: 917 RVA: 0x00015B01 File Offset: 0x00013D01
	public void RandomizeCosmetics()
	{
		this.SetRandomSkinColor();
		this.SetRandomEyes();
		this.SetRandomMouth();
		this.SetRandomAccessory();
		this.SetRandomOutfit();
		this.SetRandomHat();
	}

	// Token: 0x06000396 RID: 918 RVA: 0x00015B28 File Offset: 0x00013D28
	private void OnDestroy()
	{
		PersistentPlayerDataService service = GameHandler.GetService<PersistentPlayerDataService>();
		if (service != null)
		{
			service.UnsubscribeToPlayerDataChange(this._character.photonView.Owner, new Action<PersistentPlayerData>(this.OnPlayerDataChange));
		}
	}

	// Token: 0x06000397 RID: 919 RVA: 0x00015B60 File Offset: 0x00013D60
	public void SetCustomizationForRef(CustomizationRefs refs)
	{
		CustomizationRefs customizationRefs = this.refs;
		this.refs = refs;
		PersistentPlayerDataService service = GameHandler.GetService<PersistentPlayerDataService>();
		service.SubscribeToPlayerDataChange(this._character.photonView.Owner, new Action<PersistentPlayerData>(this.OnPlayerDataChange));
		this.OnPlayerDataChange(service.GetPlayerData(this.view.Owner));
		this.refs = customizationRefs;
	}

	// Token: 0x06000398 RID: 920 RVA: 0x00015BC4 File Offset: 0x00013DC4
	private void OnPlayerDataChange(PersistentPlayerData playerData)
	{
		if (this._character.isBot)
		{
			return;
		}
		Debug.Log("On Player Data Change");
		int currentSkin = playerData.customizationData.currentSkin;
		if (this.useDebugColor)
		{
			currentSkin = this.debugColorIndex;
		}
		Renderer[] array = this.refs.PlayerRenderers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].material.SetColor(CharacterCustomization.SkinColor, Singleton<Customization>.Instance.skins[currentSkin].color);
		}
		array = this.refs.EyeRenderers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].material.SetColor(CharacterCustomization.SkinColor, Singleton<Customization>.Instance.skins[currentSkin].color);
		}
		int currentOutfit = playerData.customizationData.currentOutfit;
		this.refs.mainRenderer.sharedMesh = Singleton<Customization>.Instance.fits[currentOutfit].fitMesh;
		this.refs.mainRendererShadow.sharedMesh = Singleton<Customization>.Instance.fits[currentOutfit].fitMesh;
		List<Material> list = new List<Material>();
		list.Add(this.refs.mainRenderer.materials[0]);
		list.Add(Singleton<Customization>.Instance.fits[currentOutfit].fitMaterial);
		list.Add(Singleton<Customization>.Instance.fits[currentOutfit].fitMaterialShoes);
		this.refs.mainRenderer.SetSharedMaterials(list);
		if (Singleton<Customization>.Instance.fits[currentOutfit].isSkirt)
		{
			this.refs.skirt.gameObject.SetActive(true);
			this.refs.shortsShadow.gameObject.SetActive(false);
			this.refs.skirtShadow.gameObject.SetActive(true);
			this.refs.shorts.gameObject.SetActive(false);
			this.refs.skirt.sharedMaterial = Singleton<Customization>.Instance.fits[currentOutfit].fitPantsMaterial;
		}
		else
		{
			this.refs.skirt.gameObject.SetActive(false);
			this.refs.shortsShadow.gameObject.SetActive(true);
			this.refs.skirtShadow.gameObject.SetActive(false);
			this.refs.shorts.gameObject.SetActive(true);
			this.refs.shorts.sharedMaterial = Singleton<Customization>.Instance.fits[currentOutfit].fitPantsMaterial;
		}
		this.refs.playerHats[0].material = Singleton<Customization>.Instance.fits[currentOutfit].fitHatMaterial;
		this.refs.playerHats[1].material = Singleton<Customization>.Instance.fits[currentOutfit].fitHatMaterial;
		this.CurrentEyeTexture = Singleton<Customization>.Instance.eyes[playerData.customizationData.currentEyes].texture;
		array = this.refs.EyeRenderers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].material.SetTexture(CharacterCustomization.MainTex, this.CurrentEyeTexture);
		}
		this.refs.mouthRenderer.material.SetTexture(CharacterCustomization.MainTex, Singleton<Customization>.Instance.mouths[playerData.customizationData.currentMouth].texture);
		this.refs.accessoryRenderer.material.SetTexture(CharacterCustomization.MainTex, Singleton<Customization>.Instance.accessories[playerData.customizationData.currentAccessory].texture);
		MeshFilter meshFilter = null;
		for (int j = 0; j < this.refs.playerHats.Length; j++)
		{
			this.refs.playerHats[j].gameObject.SetActive(playerData.customizationData.currentHat == j);
			if (playerData.customizationData.currentHat == j)
			{
				meshFilter = this.refs.playerHats[j].GetComponent<MeshFilter>();
			}
		}
		if (!meshFilter)
		{
			meshFilter = this.refs.playerHats[0].GetComponent<MeshFilter>();
		}
		this.refs.hatShadowMeshFilter.sharedMesh = meshFilter.sharedMesh;
		this.refs.hatShadowMeshFilter.transform.SetPositionAndRotation(meshFilter.transform.position, meshFilter.transform.rotation);
		this.refs.hatShadowMeshFilter.transform.localScale = meshFilter.transform.localScale;
		List<Material> list2 = new List<Material>();
		list2.Add(this.refs.sashRenderer.materials[0]);
		list2.Add(this.refs.sashAscentMaterials[playerData.customizationData.currentSash]);
		this.refs.sashRenderer.SetMaterials(list2);
		if (this._character)
		{
			this._character.refs.hideTheBody.Refresh();
		}
	}

	// Token: 0x06000399 RID: 921 RVA: 0x000160A4 File Offset: 0x000142A4
	public static void SetCharacterSkinColor(int index)
	{
		if (index > Singleton<Customization>.Instance.skins.Length)
		{
			Debug.LogError("Trying to set color outside of range for Skins???? Please explain to me?? ");
			return;
		}
		CharacterCustomizationData customizationData = CharacterCustomization.GetCustomizationData(PhotonNetwork.LocalPlayer);
		customizationData.currentSkin = index;
		CharacterCustomization.SetCustomizationData(customizationData, PhotonNetwork.LocalPlayer);
		Singleton<AchievementManager>.Instance.SetSteamStat(STEAMSTATTYPE.Cosmetic_Skin, index);
		Debug.Log(string.Format("Set character color: {0}", index));
	}

	// Token: 0x0600039A RID: 922 RVA: 0x00016107 File Offset: 0x00014307
	public static void SetCharacterEyes(int index)
	{
		CharacterCustomizationData customizationData = CharacterCustomization.GetCustomizationData(PhotonNetwork.LocalPlayer);
		customizationData.currentEyes = index;
		CharacterCustomization.SetCustomizationData(customizationData, PhotonNetwork.LocalPlayer);
		Singleton<AchievementManager>.Instance.SetSteamStat(STEAMSTATTYPE.Cosmetic_Eyes, index);
		Debug.Log(string.Format("Set character eyes: {0}", index));
	}

	// Token: 0x0600039B RID: 923 RVA: 0x00016145 File Offset: 0x00014345
	public static void SetCharacterMouth(int index)
	{
		CharacterCustomizationData customizationData = CharacterCustomization.GetCustomizationData(PhotonNetwork.LocalPlayer);
		customizationData.currentMouth = index;
		CharacterCustomization.SetCustomizationData(customizationData, PhotonNetwork.LocalPlayer);
		Singleton<AchievementManager>.Instance.SetSteamStat(STEAMSTATTYPE.Cosmetic_Mouth, index);
		Debug.Log(string.Format("Setting Character Mouth: {0}", index));
	}

	// Token: 0x0600039C RID: 924 RVA: 0x00016184 File Offset: 0x00014384
	public static void SetCharacterAccessory(int index)
	{
		CharacterCustomizationData customizationData = CharacterCustomization.GetCustomizationData(PhotonNetwork.LocalPlayer);
		customizationData.currentAccessory = index;
		CharacterCustomization.SetCustomizationData(customizationData, PhotonNetwork.LocalPlayer);
		Singleton<AchievementManager>.Instance.SetSteamStat(STEAMSTATTYPE.Cosmetic_Accessory, index);
		Debug.Log(string.Format("Setting Character Accessory: {0}", index));
	}

	// Token: 0x0600039D RID: 925 RVA: 0x000161C3 File Offset: 0x000143C3
	public static void SetCharacterOutfit(int index)
	{
		CharacterCustomizationData customizationData = CharacterCustomization.GetCustomizationData(PhotonNetwork.LocalPlayer);
		customizationData.currentOutfit = index;
		CharacterCustomization.SetCustomizationData(customizationData, PhotonNetwork.LocalPlayer);
		Singleton<AchievementManager>.Instance.SetSteamStat(STEAMSTATTYPE.Cosmetic_Outfit, index);
		Debug.Log(string.Format("Setting Character outfit: {0}", index));
	}

	// Token: 0x0600039E RID: 926 RVA: 0x00016202 File Offset: 0x00014402
	public static void SetCharacterHat(int index)
	{
		CharacterCustomizationData customizationData = CharacterCustomization.GetCustomizationData(PhotonNetwork.LocalPlayer);
		customizationData.currentHat = index;
		CharacterCustomization.SetCustomizationData(customizationData, PhotonNetwork.LocalPlayer);
		Singleton<AchievementManager>.Instance.SetSteamStat(STEAMSTATTYPE.Cosmetic_Hat, index);
		Debug.Log(string.Format("Setting Character Hat: {0}", index));
	}

	// Token: 0x0600039F RID: 927 RVA: 0x00016241 File Offset: 0x00014441
	public static void SetCharacterSash(int index)
	{
		CharacterCustomizationData customizationData = CharacterCustomization.GetCustomizationData(PhotonNetwork.LocalPlayer);
		customizationData.currentSash = index;
		CharacterCustomization.SetCustomizationData(customizationData, PhotonNetwork.LocalPlayer);
		Debug.Log(string.Format("Setting Character Sash: {0}", index));
	}

	// Token: 0x060003A0 RID: 928 RVA: 0x00016273 File Offset: 0x00014473
	public void SetRandomSkinColor()
	{
		CharacterCustomization.SetCharacterSkinColor(Singleton<Customization>.Instance.GetRandomUnlockedIndex(Customization.Type.Skin));
	}

	// Token: 0x060003A1 RID: 929 RVA: 0x00016285 File Offset: 0x00014485
	public void SetRandomEyes()
	{
		CharacterCustomization.SetCharacterEyes(Singleton<Customization>.Instance.GetRandomUnlockedIndex(Customization.Type.Eyes));
	}

	// Token: 0x060003A2 RID: 930 RVA: 0x00016298 File Offset: 0x00014498
	public void SetRandomMouth()
	{
		CharacterCustomization.SetCharacterMouth(Singleton<Customization>.Instance.GetRandomUnlockedIndex(Customization.Type.Mouth));
	}

	// Token: 0x060003A3 RID: 931 RVA: 0x000162AB File Offset: 0x000144AB
	public void SetRandomAccessory()
	{
		CharacterCustomization.SetCharacterAccessory(Singleton<Customization>.Instance.GetRandomUnlockedIndex(Customization.Type.Accessory));
	}

	// Token: 0x060003A4 RID: 932 RVA: 0x000162BE File Offset: 0x000144BE
	public void SetRandomOutfit()
	{
		CharacterCustomization.SetCharacterOutfit(Singleton<Customization>.Instance.GetRandomUnlockedIndex(Customization.Type.Fit));
	}

	// Token: 0x060003A5 RID: 933 RVA: 0x000162D1 File Offset: 0x000144D1
	public void SetRandomHat()
	{
		CharacterCustomization.SetCharacterHat(Singleton<Customization>.Instance.GetRandomUnlockedIndex(Customization.Type.Hat));
	}

	// Token: 0x060003A6 RID: 934 RVA: 0x000162E4 File Offset: 0x000144E4
	public void SetRandomIdle()
	{
		if (this.view.IsMine)
		{
			this.view.RPC("SetCharacterIdle_RPC", RpcTarget.AllBuffered, new object[] { Random.Range(0, this.maxIdles) });
		}
	}

	// Token: 0x060003A7 RID: 935 RVA: 0x00016320 File Offset: 0x00014520
	[PunRPC]
	public void CharacterDied()
	{
		for (int i = 0; i < this.refs.EyeRenderers.Length; i++)
		{
			this.refs.EyeRenderers[i].material.SetTexture(CharacterCustomization.MainTex, this.deadEyes);
			this.refs.EyeRenderers[i].material.SetInt(CharacterCustomization.Spin, 0);
		}
	}

	// Token: 0x060003A8 RID: 936 RVA: 0x00016384 File Offset: 0x00014584
	[PunRPC]
	public void CharacterPassedOut()
	{
		for (int i = 0; i < this.refs.EyeRenderers.Length; i++)
		{
			this.refs.EyeRenderers[i].material.SetTexture(CharacterCustomization.MainTex, this.passedOutEyes);
			this.refs.EyeRenderers[i].material.SetInt(CharacterCustomization.Spin, 1);
		}
	}

	// Token: 0x060003A9 RID: 937 RVA: 0x000163E8 File Offset: 0x000145E8
	public void PulseStatus(Color c)
	{
		for (int i = 0; i < this.refs.PlayerRenderers.Length; i++)
		{
			for (int j = 0; j < this.refs.PlayerRenderers[i].materials.Length; j++)
			{
				this.refs.PlayerRenderers[i].materials[j].SetColor(CharacterCustomization.StatusColor, c);
				this.refs.PlayerRenderers[i].materials[j].SetFloat(CharacterCustomization.StatusGlow, 1f);
				this.refs.PlayerRenderers[i].materials[j].DOFloat(0f, CharacterCustomization.StatusGlow, 0.5f);
			}
		}
	}

	// Token: 0x060003AA RID: 938 RVA: 0x000164A0 File Offset: 0x000146A0
	[PunRPC]
	public void SetCharacterIdle_RPC(int index)
	{
		this.PlayerAnimator.SetFloat(CharacterCustomization.Idle, (float)index);
		Debug.Log(string.Format("Setting Character Idle: {0}", index));
	}

	// Token: 0x060003AB RID: 939 RVA: 0x000164C9 File Offset: 0x000146C9
	private static CharacterCustomizationData GetCustomizationData(Photon.Realtime.Player player)
	{
		return GameHandler.GetService<PersistentPlayerDataService>().GetPlayerData(player).customizationData;
	}

	// Token: 0x060003AC RID: 940 RVA: 0x000164DC File Offset: 0x000146DC
	private static void SetCustomizationData(CharacterCustomizationData customizationData, Photon.Realtime.Player player)
	{
		PersistentPlayerDataService service = GameHandler.GetService<PersistentPlayerDataService>();
		PersistentPlayerData playerData = service.GetPlayerData(player);
		playerData.customizationData = customizationData;
		service.SetPlayerData(player, playerData);
	}

	// Token: 0x060003AD RID: 941 RVA: 0x00016504 File Offset: 0x00014704
	public void Update()
	{
		if (this._character.data.passedOut && !this.isPassedOut)
		{
			this.isPassedOut = true;
			if (this.view.IsMine)
			{
				this.view.RPC("CharacterPassedOut", RpcTarget.AllBuffered, Array.Empty<object>());
			}
		}
		if (this._character.data.dead && !this.isDead)
		{
			this.isDead = true;
			if (this.view.IsMine)
			{
				this.view.RPC("CharacterDied", RpcTarget.AllBuffered, Array.Empty<object>());
			}
		}
	}

	// Token: 0x060003AE RID: 942 RVA: 0x00016599 File Offset: 0x00014799
	public void OnRevive()
	{
		if (this.view.IsMine)
		{
			this.view.RPC("OnRevive_RPC", RpcTarget.AllBuffered, Array.Empty<object>());
		}
	}

	// Token: 0x060003AF RID: 943 RVA: 0x000165C0 File Offset: 0x000147C0
	[PunRPC]
	public void OnRevive_RPC()
	{
		Debug.Log("test dead");
		this.isDead = false;
		this.isPassedOut = false;
		for (int i = 0; i < this.refs.EyeRenderers.Length; i++)
		{
			this.refs.EyeRenderers[i].material.SetTexture(CharacterCustomization.MainTex, this.CurrentEyeTexture);
			this.refs.EyeRenderers[i].material.SetInt(CharacterCustomization.Spin, 0);
		}
	}

	// Token: 0x04000415 RID: 1045
	private Character _character;

	// Token: 0x04000416 RID: 1046
	public CustomizationRefs refs;

	// Token: 0x04000417 RID: 1047
	private static readonly int MainTex = Shader.PropertyToID("_MainTex");

	// Token: 0x04000418 RID: 1048
	private static readonly int SkinColor = Shader.PropertyToID("_SkinColor");

	// Token: 0x04000419 RID: 1049
	private static readonly int Idle = Animator.StringToHash("Idle");

	// Token: 0x0400041A RID: 1050
	private static readonly int Spin = Shader.PropertyToID("_Spin");

	// Token: 0x0400041B RID: 1051
	private static readonly int VertexGhost = Shader.PropertyToID("_VertexGhost");

	// Token: 0x0400041C RID: 1052
	private static readonly int StatusColor = Shader.PropertyToID("_StatusColor");

	// Token: 0x0400041D RID: 1053
	private static readonly int StatusGlow = Shader.PropertyToID("_StatusGlow");

	// Token: 0x0400041E RID: 1054
	public bool useDebugColor;

	// Token: 0x0400041F RID: 1055
	public int debugColorIndex;

	// Token: 0x04000420 RID: 1056
	public int maxIdles;

	// Token: 0x04000421 RID: 1057
	public Animator PlayerAnimator;

	// Token: 0x04000422 RID: 1058
	private PhotonView view;

	// Token: 0x04000423 RID: 1059
	public Texture passedOutEyes;

	// Token: 0x04000424 RID: 1060
	private Texture CurrentEyeTexture;

	// Token: 0x04000425 RID: 1061
	[FormerlySerializedAs("diedTexture")]
	public Texture deadEyes;

	// Token: 0x04000426 RID: 1062
	[FormerlySerializedAs("isDead")]
	public bool isPassedOut;

	// Token: 0x04000427 RID: 1063
	public bool isDead;
}
