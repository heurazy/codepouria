using System;
using UnityEngine;
using Zorro.UI;

// Token: 0x0200015F RID: 351
public class MainMenuPopupHandler : UIPageHandlerStartPageSelector
{
	// Token: 0x060009FB RID: 2555 RVA: 0x00031CA8 File Offset: 0x0002FEA8
	public override UIPage GetStartPage()
	{
		string text = "FirstTimeStartup2";
		if (PlayerPrefs.HasKey(text))
		{
			return this.mainPage;
		}
		PlayerPrefs.SetInt(text, 1);
		PlayerPrefs.Save();
		return this.firstTimeSetupPage;
	}

	// Token: 0x040008EF RID: 2287
	public MainMenuMainPage mainPage;

	// Token: 0x040008F0 RID: 2288
	public MainMenuFirstTimeSetupPage firstTimeSetupPage;
}
