using System;
using System.Collections.Generic;
using UnityEngine.Localization;
using Zorro.Settings;

// Token: 0x02000131 RID: 305
public class PushToTalkSetting : EnumSetting<PushToTalkSetting.PushToTalkType>, IExposedSetting
{
	// Token: 0x060008E2 RID: 2274 RVA: 0x0002DBAA File Offset: 0x0002BDAA
	public override void ApplyValue()
	{
	}

	// Token: 0x060008E3 RID: 2275 RVA: 0x0002DBAC File Offset: 0x0002BDAC
	protected override PushToTalkSetting.PushToTalkType GetDefaultValue()
	{
		return PushToTalkSetting.PushToTalkType.VoiceActivation;
	}

	// Token: 0x060008E4 RID: 2276 RVA: 0x0002DBAF File Offset: 0x0002BDAF
	public override List<LocalizedString> GetLocalizedChoices()
	{
		return null;
	}

	// Token: 0x060008E5 RID: 2277 RVA: 0x0002DBB2 File Offset: 0x0002BDB2
	public string GetDisplayName()
	{
		return "Microphone mode";
	}

	// Token: 0x060008E6 RID: 2278 RVA: 0x0002DBB9 File Offset: 0x0002BDB9
	public string GetCategory()
	{
		return "Audio";
	}

	// Token: 0x060008E7 RID: 2279 RVA: 0x0002DBC0 File Offset: 0x0002BDC0
	public override List<string> GetUnlocalizedChoices()
	{
		return new List<string> { "Voice Activation", "Push To Talk [V]" };
	}

	// Token: 0x0200035B RID: 859
	public enum PushToTalkType
	{
		// Token: 0x0400124C RID: 4684
		VoiceActivation,
		// Token: 0x0400124D RID: 4685
		PushToTalk
	}
}
