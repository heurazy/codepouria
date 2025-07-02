using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization;
using Zorro.Core;
using Zorro.Settings;
using Zorro.Settings.DebugUI;

// Token: 0x0200012E RID: 302
public class MicrophoneSetting : Setting, IEnumSetting, IExposedSetting
{
	// Token: 0x060008CA RID: 2250 RVA: 0x0002D900 File Offset: 0x0002BB00
	public List<MicrophoneSetting.MicrophoneInfo> GetChoices()
	{
		string[] devices = Microphone.devices;
		List<MicrophoneSetting.MicrophoneInfo> list = new List<MicrophoneSetting.MicrophoneInfo>();
		foreach (string text in devices)
		{
			list.Add(new MicrophoneSetting.MicrophoneInfo
			{
				id = text,
				name = text
			});
		}
		return list;
	}

	// Token: 0x060008CB RID: 2251 RVA: 0x0002D94C File Offset: 0x0002BB4C
	public override void Load(ISettingsSaveLoad loader)
	{
		string value;
		if (loader.TryLoadString(base.GetType(), out value))
		{
			List<MicrophoneSetting.MicrophoneInfo> choices = this.GetChoices();
			this.Value = choices.Find((MicrophoneSetting.MicrophoneInfo x) => x.id == value);
			if (string.IsNullOrEmpty(this.Value.id))
			{
				Debug.LogWarning("Failed to load setting of type " + base.GetType().FullName + " from PlayerPrefs. Value not found in choices.");
				this.Value = this.GetDefaultValue();
				return;
			}
		}
		else
		{
			Debug.LogWarning("Failed to load setting of type " + base.GetType().FullName + " from PlayerPrefs.");
			this.Value = this.GetDefaultValue();
		}
	}

	// Token: 0x060008CC RID: 2252 RVA: 0x0002D9FC File Offset: 0x0002BBFC
	private MicrophoneSetting.MicrophoneInfo GetDefaultValue()
	{
		if (this.GetChoices().Count == 0)
		{
			Debug.LogError("No voice devices found.");
			return default(MicrophoneSetting.MicrophoneInfo);
		}
		return this.GetChoices().First<MicrophoneSetting.MicrophoneInfo>();
	}

	// Token: 0x060008CD RID: 2253 RVA: 0x0002DA35 File Offset: 0x0002BC35
	public override void Save(ISettingsSaveLoad saver)
	{
		saver.SaveString(base.GetType(), this.Value.id);
	}

	// Token: 0x060008CE RID: 2254 RVA: 0x0002DA50 File Offset: 0x0002BC50
	public override void ApplyValue()
	{
		string text = "Voice setting applied: ";
		MicrophoneSetting.MicrophoneInfo value = this.Value;
		Debug.Log(text + value.ToString());
	}

	// Token: 0x060008CF RID: 2255 RVA: 0x0002DA80 File Offset: 0x0002BC80
	public override SettingUI GetDebugUI(ISettingHandler settingHandler)
	{
		return new EnumSettingsUI(this, settingHandler);
	}

	// Token: 0x060008D0 RID: 2256 RVA: 0x0002DA89 File Offset: 0x0002BC89
	public override GameObject GetSettingUICell()
	{
		return SingletonAsset<InputCellMapper>.Instance.EnumSettingCell;
	}

	// Token: 0x060008D1 RID: 2257 RVA: 0x0002DA95 File Offset: 0x0002BC95
	public List<LocalizedString> GetLocalizedChoices()
	{
		return null;
	}

	// Token: 0x060008D2 RID: 2258 RVA: 0x0002DA98 File Offset: 0x0002BC98
	public List<string> GetUnlocalizedChoices()
	{
		return (from info in this.GetChoices()
			select info.name).ToList<string>();
	}

	// Token: 0x060008D3 RID: 2259 RVA: 0x0002DACC File Offset: 0x0002BCCC
	public int GetValue()
	{
		return (from info in this.GetChoices()
			select info.id).ToList<string>().IndexOf(this.Value.id);
	}

	// Token: 0x060008D4 RID: 2260 RVA: 0x0002DB18 File Offset: 0x0002BD18
	public void SetValue(int v, ISettingHandler settingHandler, bool fromUI)
	{
		MicrophoneSetting.MicrophoneInfo microphoneInfo = this.GetChoices()[v];
		this.Value = microphoneInfo;
		this.ApplyValue();
		settingHandler.SaveSetting(this);
	}

	// Token: 0x060008D5 RID: 2261 RVA: 0x0002DB46 File Offset: 0x0002BD46
	public string GetDisplayName()
	{
		return "Microphone";
	}

	// Token: 0x060008D6 RID: 2262 RVA: 0x0002DB4D File Offset: 0x0002BD4D
	public string GetCategory()
	{
		return "Audio";
	}

	// Token: 0x04000806 RID: 2054
	public MicrophoneSetting.MicrophoneInfo Value;

	// Token: 0x02000358 RID: 856
	public struct MicrophoneInfo
	{
		// Token: 0x06001390 RID: 5008 RVA: 0x0005CFA2 File Offset: 0x0005B1A2
		public override string ToString()
		{
			return this.id + " (" + this.name + ")";
		}

		// Token: 0x04001245 RID: 4677
		public string id;

		// Token: 0x04001246 RID: 4678
		public string name;
	}
}
