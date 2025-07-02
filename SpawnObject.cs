using System;
using UnityEngine;

// Token: 0x02000111 RID: 273
[Serializable]
public class SpawnObject
{
	// Token: 0x04000771 RID: 1905
	public int maxCount;

	// Token: 0x04000772 RID: 1906
	public GameObject prefab;

	// Token: 0x04000773 RID: 1907
	public Vector3 inversion;

	// Token: 0x04000774 RID: 1908
	public Vector3 randomRot;

	// Token: 0x04000775 RID: 1909
	public Vector3 randomScale;

	// Token: 0x04000776 RID: 1910
	public float uniformScale;

	// Token: 0x04000777 RID: 1911
	public float scaleMultiplier = 1f;

	// Token: 0x04000778 RID: 1912
	public Vector3 posJitter;
}
