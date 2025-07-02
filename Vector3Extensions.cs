using System;
using UnityEngine;

// Token: 0x0200009A RID: 154
public static class Vector3Extensions
{
	// Token: 0x060005BA RID: 1466 RVA: 0x000201DF File Offset: 0x0001E3DF
	public static Vector2 XZ(this Vector3 vector)
	{
		return new Vector2(vector.x, vector.z);
	}

	// Token: 0x060005BB RID: 1467 RVA: 0x000201F2 File Offset: 0x0001E3F2
	public static Vector3 Flat(this Vector3 vector)
	{
		return new Vector3(vector.x, 0f, vector.z);
	}
}
