using System;
using UnityEngine.UI;

// Token: 0x02000162 RID: 354
public class CosmeticUnlockWindow : MenuWindow
{
	// Token: 0x17000081 RID: 129
	// (get) Token: 0x06000A03 RID: 2563 RVA: 0x00031DDB File Offset: 0x0002FFDB
	public new virtual Selectable objectToSelectOnOpen
	{
		get
		{
			return this.continueButton;
		}
	}

	// Token: 0x040008F3 RID: 2291
	public Button continueButton;
}
