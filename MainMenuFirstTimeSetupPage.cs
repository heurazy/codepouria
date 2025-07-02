using System;
using UnityEngine.Events;
using UnityEngine.UI;
using Zorro.Settings.UI;
using Zorro.UI;

// Token: 0x0200015B RID: 347
public class MainMenuFirstTimeSetupPage : UIPage
{
	// Token: 0x060009E6 RID: 2534 RVA: 0x000317E0 File Offset: 0x0002F9E0
	public void Start()
	{
		SettingsHandler instance = SettingsHandler.Instance;
		MicrophoneSetting setting = instance.GetSetting<MicrophoneSetting>();
		this.MicSettingUI.Setup(setting, instance);
		this.ContinueButton.onClick.AddListener(new UnityAction(this.ContinueClicked));
	}

	// Token: 0x060009E7 RID: 2535 RVA: 0x00031823 File Offset: 0x0002FA23
	private void ContinueClicked()
	{
		this.pageHandler.TransistionToPage<MainMenuMainPage>();
	}

	// Token: 0x040008E2 RID: 2274
	public EnumSettingUI MicSettingUI;

	// Token: 0x040008E3 RID: 2275
	public Button ContinueButton;
}
