using System;
using UnityEngine;

// Token: 0x0200023D RID: 573
[Serializable]
public class MatAndID
{
	// Token: 0x06000E38 RID: 3640 RVA: 0x000478B6 File Offset: 0x00045AB6
	public MatAndID(Material mat, int id)
	{
		this.mat = mat;
		this.id = id;
	}

	// Token: 0x04000D4A RID: 3402
	public Material mat;

	// Token: 0x04000D4B RID: 3403
	public int id;
}
