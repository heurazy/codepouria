using System;
using Zorro.UI;

// Token: 0x0200016F RID: 367
public class SettingsTABS : TABS<SettingsTABSButton>
{
	// Token: 0x06000A61 RID: 2657 RVA: 0x00032934 File Offset: 0x00030B34
	public override void OnSelected(SettingsTABSButton button)
	{
		this.SettingsMenu.ShowSettings(button.category);
	}

	// Token: 0x04000927 RID: 2343
	public SharedSettingsMenu SettingsMenu;
}
