using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine.Localization;
using Zorro.Settings;

// Token: 0x0200012B RID: 299
public class LobbyTypeSetting : EnumSetting<LobbyTypeSetting.LobbyType>, IExposedSetting, IConditionalSetting
{
	// Token: 0x060008B7 RID: 2231 RVA: 0x0002D847 File Offset: 0x0002BA47
	public override void ApplyValue()
	{
	}

	// Token: 0x060008B8 RID: 2232 RVA: 0x0002D849 File Offset: 0x0002BA49
	protected override LobbyTypeSetting.LobbyType GetDefaultValue()
	{
		return LobbyTypeSetting.LobbyType.Friends;
	}

	// Token: 0x060008B9 RID: 2233 RVA: 0x0002D84C File Offset: 0x0002BA4C
	public override List<LocalizedString> GetLocalizedChoices()
	{
		return null;
	}

	// Token: 0x060008BA RID: 2234 RVA: 0x0002D84F File Offset: 0x0002BA4F
	public override List<string> GetUnlocalizedChoices()
	{
		return new List<string> { "Friends", "Invite Only" };
	}

	// Token: 0x060008BB RID: 2235 RVA: 0x0002D86C File Offset: 0x0002BA6C
	public string GetDisplayName()
	{
		return "Lobby Mode";
	}

	// Token: 0x060008BC RID: 2236 RVA: 0x0002D873 File Offset: 0x0002BA73
	public string GetCategory()
	{
		return "General";
	}

	// Token: 0x060008BD RID: 2237 RVA: 0x0002D87A File Offset: 0x0002BA7A
	public bool ShouldShow()
	{
		return !PhotonNetwork.InRoom;
	}

	// Token: 0x02000356 RID: 854
	public enum LobbyType
	{
		// Token: 0x0400123F RID: 4671
		Friends,
		// Token: 0x04001240 RID: 4672
		InviteOnly
	}
}
