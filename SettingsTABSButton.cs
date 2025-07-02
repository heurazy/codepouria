using System;
using UnityEngine;
using Zorro.UI;

// Token: 0x02000170 RID: 368
public class SettingsTABSButton : TAB_Button
{
	// Token: 0x06000A63 RID: 2659 RVA: 0x00032950 File Offset: 0x00030B50
	private void Update()
	{
		Color color = (base.Selected ? Color.black : Color.white);
		this.text.color = Color.Lerp(this.text.color, color, Time.unscaledDeltaTime * 7f);
		this.SelectedGraphic.gameObject.SetActive(base.Selected);
	}

	// Token: 0x04000928 RID: 2344
	public SettingsCategory category;

	// Token: 0x04000929 RID: 2345
	public GameObject SelectedGraphic;
}
