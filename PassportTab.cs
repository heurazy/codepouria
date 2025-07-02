using System;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x02000169 RID: 361
public class PassportTab : MonoBehaviour
{
	// Token: 0x06000A4C RID: 2636 RVA: 0x000325AE File Offset: 0x000307AE
	public void SetTab()
	{
		if (!this.opened)
		{
			this.manager.OpenTab(this.type);
		}
		EventSystem.current.SetSelectedGameObject(null);
	}

	// Token: 0x06000A4D RID: 2637 RVA: 0x000325D4 File Offset: 0x000307D4
	public void Open()
	{
		this.anim.SetBool("Open", true);
		this.opened = true;
	}

	// Token: 0x06000A4E RID: 2638 RVA: 0x000325EE File Offset: 0x000307EE
	public void Close()
	{
		this.anim.SetBool("Open", false);
		this.opened = false;
	}

	// Token: 0x04000915 RID: 2325
	public PassportManager manager;

	// Token: 0x04000916 RID: 2326
	public Customization.Type type;

	// Token: 0x04000917 RID: 2327
	public Animator anim;

	// Token: 0x04000918 RID: 2328
	private bool opened;
}
