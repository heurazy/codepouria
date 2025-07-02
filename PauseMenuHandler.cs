using System;
using Zorro.UI;

// Token: 0x0200016A RID: 362
public class PauseMenuHandler : UIPageHandler
{
	// Token: 0x06000A50 RID: 2640 RVA: 0x00032610 File Offset: 0x00030810
	private void OnEnable()
	{
		if (!(this.currentPage is PauseMenuMainPage))
		{
			base.TransistionToPage<PauseMenuMainPage>();
		}
	}
}
