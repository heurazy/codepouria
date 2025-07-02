using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Device;
using UnityEngine.Localization;
using Zorro.Core;
using Zorro.Settings;
using Zorro.Settings.DebugUI;

// Token: 0x02000128 RID: 296
public class FullscreenSetting : Setting, IEnumSetting, IExposedSetting
{
	// Token: 0x0600089F RID: 2207 RVA: 0x0002D734 File Offset: 0x0002B934
	public override void ApplyValue()
	{
	}

	// Token: 0x060008A0 RID: 2208 RVA: 0x0002D736 File Offset: 0x0002B936
	public override SettingUI GetDebugUI(ISettingHandler settingHandler)
	{
		return new EnumSettingsUI(this, settingHandler);
	}

	// Token: 0x060008A1 RID: 2209 RVA: 0x0002D73F File Offset: 0x0002B93F
	public override GameObject GetSettingUICell()
	{
		return SingletonAsset<InputCellMapper>.Instance.EnumSettingCell;
	}

	// Token: 0x060008A2 RID: 2210 RVA: 0x0002D74B File Offset: 0x0002B94B
	public override void Load(ISettingsSaveLoad loader)
	{
		if (!PlayerPrefs.HasKey("FULLSCREEN_MODE"))
		{
			PlayerPrefs.SetInt("FULLSCREEN_MODE", 1);
			global::UnityEngine.Device.Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
		}
	}

	// Token: 0x060008A3 RID: 2211 RVA: 0x0002D76D File Offset: 0x0002B96D
	public override void Save(ISettingsSaveLoad saver)
	{
	}

	// Token: 0x060008A4 RID: 2212 RVA: 0x0002D76F File Offset: 0x0002B96F
	public string GetDisplayName()
	{
		return "Window Mode";
	}

	// Token: 0x060008A5 RID: 2213 RVA: 0x0002D776 File Offset: 0x0002B976
	public string GetCategory()
	{
		return "Graphics";
	}

	// Token: 0x060008A6 RID: 2214 RVA: 0x0002D77D File Offset: 0x0002B97D
	public List<LocalizedString> GetLocalizedChoices()
	{
		return null;
	}

	// Token: 0x060008A7 RID: 2215 RVA: 0x0002D780 File Offset: 0x0002B980
	public List<string> GetUnlocalizedChoices()
	{
		return new List<string> { "Windowed", "Fullscreen", "Windowed Fullscreen" };
	}

	// Token: 0x060008A8 RID: 2216 RVA: 0x0002D7A8 File Offset: 0x0002B9A8
	public int GetValue()
	{
		switch (global::UnityEngine.Device.Screen.fullScreenMode)
		{
		case FullScreenMode.ExclusiveFullScreen:
			return 1;
		case FullScreenMode.FullScreenWindow:
			return 2;
		case FullScreenMode.Windowed:
			return 0;
		}
		return 0;
	}

	// Token: 0x060008A9 RID: 2217 RVA: 0x0002D7DA File Offset: 0x0002B9DA
	public void SetValue(int v, ISettingHandler settingHandler, bool fromUI)
	{
		switch (v)
		{
		case 0:
			global::UnityEngine.Device.Screen.fullScreenMode = FullScreenMode.Windowed;
			return;
		case 1:
			global::UnityEngine.Device.Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
			return;
		case 2:
			global::UnityEngine.Device.Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
			return;
		default:
			return;
		}
	}
}
