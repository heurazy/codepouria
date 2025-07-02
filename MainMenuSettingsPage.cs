using System;
using Zorro.UI;

// Token: 0x02000160 RID: 352
public class MainMenuSettingsPage : UIPage, IHaveParentPage
{
	// Token: 0x060009FD RID: 2557 RVA: 0x00031CE4 File Offset: 0x0002FEE4
	public ValueTuple<UIPage, PageTransistion> GetParentPage()
	{
		return new ValueTuple<UIPage, PageTransistion>(this.pageHandler.GetPage<MainMenuMainPage>(), new SetActivePageTransistion());
	}
}
