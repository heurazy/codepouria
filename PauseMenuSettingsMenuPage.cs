using System;
using Zorro.UI;

// Token: 0x0200016C RID: 364
public class PauseMenuSettingsMenuPage : UIPage, IHaveParentPage
{
	// Token: 0x06000A56 RID: 2646 RVA: 0x00032699 File Offset: 0x00030899
	public ValueTuple<UIPage, PageTransistion> GetParentPage()
	{
		return new ValueTuple<UIPage, PageTransistion>(this.pageHandler.GetPage<PauseMenuMainPage>(), new SetActivePageTransistion());
	}
}
