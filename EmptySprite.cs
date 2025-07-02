using System;
using UnityEngine;

// Token: 0x02000221 RID: 545
public static class EmptySprite
{
	// Token: 0x06000DEB RID: 3563 RVA: 0x000463E9 File Offset: 0x000445E9
	public static Sprite Get()
	{
		if (EmptySprite.instance == null)
		{
			EmptySprite.instance = Resources.Load<Sprite>("procedural_ui_image_default_sprite");
		}
		return EmptySprite.instance;
	}

	// Token: 0x06000DEC RID: 3564 RVA: 0x0004640C File Offset: 0x0004460C
	public static bool IsEmptySprite(Sprite s)
	{
		return EmptySprite.Get() == s;
	}

	// Token: 0x04000D06 RID: 3334
	private static Sprite instance;
}
