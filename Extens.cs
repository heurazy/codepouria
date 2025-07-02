using System;
using UnityEngine;

// Token: 0x02000023 RID: 35
public static class Extens
{
	// Token: 0x06000254 RID: 596 RVA: 0x000107D8 File Offset: 0x0000E9D8
	public static Vector3 EulerRescaled(this Quaternion quaternion)
	{
		Vector3 eulerAngles = quaternion.eulerAngles;
		return new Vector3(Mathf.Repeat(eulerAngles.x + 180f, 360f) - 180f, Mathf.Repeat(eulerAngles.y + 180f, 360f) - 180f, Mathf.Repeat(eulerAngles.z + 180f, 360f) - 180f);
	}

	// Token: 0x06000255 RID: 597 RVA: 0x00010846 File Offset: 0x0000EA46
	public static Quaternion Inverse(this Quaternion quaterion)
	{
		return Quaternion.Inverse(quaterion);
	}
}
