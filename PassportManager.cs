using System;
using System.Collections;
using DG.Tweening;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zorro.Core;
using Zorro.Core.CLI;

// Token: 0x02000104 RID: 260
public class PassportManager : MenuWindow
{
	// Token: 0x17000062 RID: 98
	// (get) Token: 0x060007A7 RID: 1959 RVA: 0x0002899D File Offset: 0x00026B9D
	public override bool openOnStart
	{
		get
		{
			return false;
		}
	}

	// Token: 0x17000063 RID: 99
	// (get) Token: 0x060007A8 RID: 1960 RVA: 0x000289A0 File Offset: 0x00026BA0
	public override bool selectOnOpen
	{
		get
		{
			return true;
		}
	}

	// Token: 0x17000064 RID: 100
	// (get) Token: 0x060007A9 RID: 1961 RVA: 0x000289A3 File Offset: 0x00026BA3
	public override Selectable objectToSelectOnOpen
	{
		get
		{
			return this.buttons[0].button;
		}
	}

	// Token: 0x17000065 RID: 101
	// (get) Token: 0x060007AA RID: 1962 RVA: 0x000289B2 File Offset: 0x00026BB2
	public override bool closeOnPause
	{
		get
		{
			return true;
		}
	}

	// Token: 0x17000066 RID: 102
	// (get) Token: 0x060007AB RID: 1963 RVA: 0x000289B5 File Offset: 0x00026BB5
	public override bool closeOnUICancel
	{
		get
		{
			return true;
		}
	}

	// Token: 0x17000067 RID: 103
	// (get) Token: 0x060007AC RID: 1964 RVA: 0x000289B8 File Offset: 0x00026BB8
	public override bool autoHideOnClose
	{
		get
		{
			return false;
		}
	}

	// Token: 0x060007AD RID: 1965 RVA: 0x000289BB File Offset: 0x00026BBB
	public void Awake()
	{
		PassportManager.instance = this;
		this.uiObject.SetActive(false);
	}

	// Token: 0x060007AE RID: 1966 RVA: 0x000289CF File Offset: 0x00026BCF
	[ConsoleCommand]
	public static void TestAllCosmetics()
	{
		if (PassportManager.instance != null)
		{
			PassportManager.instance.testUnlockAll = true;
		}
	}

	// Token: 0x060007AF RID: 1967 RVA: 0x000289EC File Offset: 0x00026BEC
	public static string GeneratePassportNumber(string name)
	{
		string text = PassportManager.GenerateCountryCode(name);
		int num = PassportManager.GenerateNumericCode(name, 9);
		return string.Format("{0}{1:D7}", text, num);
	}

	// Token: 0x060007B0 RID: 1968 RVA: 0x00028A1C File Offset: 0x00026C1C
	private static string GenerateCountryCode(string name)
	{
		name = name.ToUpper().Replace(" ", "");
		if (name.Length < 2)
		{
			name += "XX";
		}
		return string.Format("{0}", name[0]);
	}

	// Token: 0x060007B1 RID: 1969 RVA: 0x00028A6C File Offset: 0x00026C6C
	private static int GenerateNumericCode(string input, int length)
	{
		return Mathf.Abs(input.GetHashCode()) % (int)Mathf.Pow(10f, (float)length);
	}

	// Token: 0x060007B2 RID: 1970 RVA: 0x00028A87 File Offset: 0x00026C87
	public void ToggleOpen()
	{
		if (!this.closing)
		{
			if (!base.isOpen)
			{
				this.Open();
				this.uiObject.SetActive(true);
				this.OpenTab(this.activeType);
				return;
			}
			base.Close();
		}
	}

	// Token: 0x060007B3 RID: 1971 RVA: 0x00028AC0 File Offset: 0x00026CC0
	protected override void Initialize()
	{
		string characterName = Character.localCharacter.characterName;
		PassportManager.passportNumberString = PassportManager.GeneratePassportNumber(characterName);
		this.nameText.text = characterName;
		this.passportNumberText.text = PassportManager.passportNumberString;
	}

	// Token: 0x060007B4 RID: 1972 RVA: 0x00028AFF File Offset: 0x00026CFF
	protected override void OnClose()
	{
		base.StartCoroutine(this.CloseRoutine());
	}

	// Token: 0x060007B5 RID: 1973 RVA: 0x00028B0E File Offset: 0x00026D0E
	private IEnumerator CloseRoutine()
	{
		this.closing = true;
		this.anim.Play("Close");
		this.CameraIn();
		yield return new WaitForSeconds(0.5f);
		this.uiObject.SetActive(false);
		this.closing = false;
		yield break;
	}

	// Token: 0x060007B6 RID: 1974 RVA: 0x00028B20 File Offset: 0x00026D20
	public void OpenTab(Customization.Type type)
	{
		this.activeType = type;
		int num = 0;
		for (int i = 0; i < this.tabs.Length; i++)
		{
			if (this.tabs[i].type == type)
			{
				num = i;
			}
			else
			{
				this.tabs[i].Close();
			}
		}
		this.tabs[num].Open();
		if (num == 4)
		{
			this.CameraOut();
		}
		else
		{
			this.CameraIn();
		}
		this.SetButtons();
		this.dummy.UpdateDummy();
	}

	// Token: 0x060007B7 RID: 1975 RVA: 0x00028B9C File Offset: 0x00026D9C
	private void CameraIn()
	{
		this.dummyCamera.DOOrthoSize(0.6f, 0.2f);
		this.dummyCamera.transform.DOLocalMove(new Vector3(0f, 1.65f, 1f), 0.2f, false);
		if (this.camIn)
		{
			this.camIn.Play(default(Vector3));
		}
	}

	// Token: 0x060007B8 RID: 1976 RVA: 0x00028C0C File Offset: 0x00026E0C
	private void CameraOut()
	{
		this.dummyCamera.DOOrthoSize(1.3f, 0.2f);
		this.dummyCamera.transform.DOLocalMove(new Vector3(0f, 1.05f, 1f), 0.2f, false);
		if (this.camOut)
		{
			this.camOut.Play(default(Vector3));
		}
	}

	// Token: 0x060007B9 RID: 1977 RVA: 0x00028C7C File Offset: 0x00026E7C
	public void SetButtons()
	{
		CustomizationOption[] list = Singleton<Customization>.Instance.GetList(this.activeType);
		for (int i = 0; i < this.buttons.Length; i++)
		{
			if (i < list.Length)
			{
				this.buttons[i].SetButton(list[i], i);
			}
			else
			{
				this.buttons[i].SetButton(null, -1);
			}
		}
		this.SetActiveButton();
	}

	// Token: 0x060007BA RID: 1978 RVA: 0x00028CDC File Offset: 0x00026EDC
	private void SetActiveButton()
	{
		PersistentPlayerData playerData = GameHandler.GetService<PersistentPlayerDataService>().GetPlayerData(PhotonNetwork.LocalPlayer);
		int num = playerData.customizationData.currentSkin;
		if (this.activeType == Customization.Type.Accessory)
		{
			num = playerData.customizationData.currentAccessory;
		}
		else if (this.activeType == Customization.Type.Eyes)
		{
			num = playerData.customizationData.currentEyes;
		}
		else if (this.activeType == Customization.Type.Mouth)
		{
			num = playerData.customizationData.currentMouth;
		}
		else if (this.activeType == Customization.Type.Fit)
		{
			num = playerData.customizationData.currentOutfit;
		}
		else if (this.activeType == Customization.Type.Hat)
		{
			num = playerData.customizationData.currentHat;
		}
		for (int i = 0; i < this.buttons.Length; i++)
		{
			this.buttons[i].border.color = ((num == i) ? this.activeBorderColor : this.inactiveBorderColor);
		}
	}

	// Token: 0x060007BB RID: 1979 RVA: 0x00028DB4 File Offset: 0x00026FB4
	public void SetOption(CustomizationOption option, int index)
	{
		if (option.type == Customization.Type.Skin)
		{
			CharacterCustomization.SetCharacterSkinColor(index);
		}
		else if (option.type == Customization.Type.Eyes)
		{
			CharacterCustomization.SetCharacterEyes(index);
		}
		else if (option.type == Customization.Type.Mouth)
		{
			CharacterCustomization.SetCharacterMouth(index);
		}
		else if (option.type == Customization.Type.Accessory)
		{
			CharacterCustomization.SetCharacterAccessory(index);
		}
		else if (option.type == Customization.Type.Fit)
		{
			CharacterCustomization.SetCharacterOutfit(index);
		}
		else if (option.type == Customization.Type.Hat)
		{
			CharacterCustomization.SetCharacterHat(index);
		}
		this.SetActiveButton();
		this.dummy.UpdateDummy();
	}

	// Token: 0x0400071F RID: 1823
	public static PassportManager instance;

	// Token: 0x04000720 RID: 1824
	public Animator anim;

	// Token: 0x04000721 RID: 1825
	public GameObject uiObject;

	// Token: 0x04000722 RID: 1826
	public PassportTab[] tabs;

	// Token: 0x04000723 RID: 1827
	public Customization.Type activeType;

	// Token: 0x04000724 RID: 1828
	public PassportButton[] buttons;

	// Token: 0x04000725 RID: 1829
	public PlayerCustomizationDummy dummy;

	// Token: 0x04000726 RID: 1830
	public Camera dummyCamera;

	// Token: 0x04000727 RID: 1831
	public TextMeshProUGUI nameText;

	// Token: 0x04000728 RID: 1832
	public TextMeshProUGUI passportNumberText;

	// Token: 0x04000729 RID: 1833
	private static string passportNumberString;

	// Token: 0x0400072A RID: 1834
	public Color inactiveBorderColor;

	// Token: 0x0400072B RID: 1835
	public Color activeBorderColor;

	// Token: 0x0400072C RID: 1836
	public bool testUnlockAll;

	// Token: 0x0400072D RID: 1837
	public SFX_Instance camIn;

	// Token: 0x0400072E RID: 1838
	public SFX_Instance camOut;

	// Token: 0x0400072F RID: 1839
	private bool closing;
}
