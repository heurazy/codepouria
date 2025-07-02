using System;
using System.Collections.Generic;
using UnityEngine;
using Zorro.Core;
using Zorro.Core.CLI;
using Zorro.Settings;

// Token: 0x02000137 RID: 311
public class SettingsHandler : ISettingHandler
{
	// Token: 0x06000904 RID: 2308 RVA: 0x0002DDE0 File Offset: 0x0002BFE0
	public SettingsHandler()
	{
		this.settings = new List<Setting>(30);
		this._settingsSaveLoad = new DefaultSettingsSaveLoad();
		this.AddSetting(new FovSetting());
		this.AddSetting(new FullscreenSetting());
		this.AddSetting(new ResolutionSetting());
		this.AddSetting(new FPSCapSetting());
		this.AddSetting(new VSyncSetting());
		this.AddSetting(new MicrophoneSetting());
		this.AddSetting(new RenderScaleSetting());
		this.AddSetting(new ShadowDistanceSettings());
		this.AddSetting(new PushToTalkSetting());
		this.AddSetting(new MasterVolumeSetting(SingletonAsset<StaticReferences>.Instance.masterMixerGroup));
		this.AddSetting(new SFXVolumeSetting(SingletonAsset<StaticReferences>.Instance.masterMixerGroup));
		this.AddSetting(new MusicVolumeSetting(SingletonAsset<StaticReferences>.Instance.masterMixerGroup));
		this.AddSetting(new VoiceVolumeSetting(SingletonAsset<StaticReferences>.Instance.masterMixerGroup));
		this.AddSetting(new MouseSensitivitySetting());
		this.AddSetting(new ControllerSensitivitySetting());
		this.AddSetting(new LodQuality());
		this.AddSetting(new AOSetting());
		this.AddSetting(new InvertXSetting());
		this.AddSetting(new InvertYSetting());
		this.AddSetting(new LobbyTypeSetting());
		DebugUIHandler instance = Singleton<DebugUIHandler>.Instance;
		if (instance != null)
		{
			instance.RegisterPage("Settings", () => new SettingsPage(this.settings, this));
		}
		SettingsHandler.Instance = this;
		Debug.Log("Settings Initlaized");
	}

	// Token: 0x06000905 RID: 2309 RVA: 0x0002DF40 File Offset: 0x0002C140
	public void AddSetting(Setting setting)
	{
		this.settings.Add(setting);
		setting.Load(this._settingsSaveLoad);
		setting.ApplyValue();
	}

	// Token: 0x06000906 RID: 2310 RVA: 0x0002DF60 File Offset: 0x0002C160
	public void SaveSetting(Setting setting)
	{
		setting.Save(this._settingsSaveLoad);
		this._settingsSaveLoad.WriteToDisk();
	}

	// Token: 0x06000907 RID: 2311 RVA: 0x0002DF7C File Offset: 0x0002C17C
	public T GetSetting<T>() where T : Setting
	{
		foreach (Setting setting in this.settings)
		{
			T t = setting as T;
			if (t != null)
			{
				return t;
			}
		}
		return default(T);
	}

	// Token: 0x06000908 RID: 2312 RVA: 0x0002DFEC File Offset: 0x0002C1EC
	public IEnumerable<Setting> GetAllSettings()
	{
		return this.settings;
	}

	// Token: 0x06000909 RID: 2313 RVA: 0x0002DFF4 File Offset: 0x0002C1F4
	public void Update()
	{
		foreach (Setting setting in this.settings)
		{
			setting.Update();
		}
	}

	// Token: 0x04000807 RID: 2055
	private List<Setting> settings;

	// Token: 0x04000808 RID: 2056
	private ISettingsSaveLoad _settingsSaveLoad;

	// Token: 0x04000809 RID: 2057
	public static SettingsHandler Instance;
}
