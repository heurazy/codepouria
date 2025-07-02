using System;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x0200013D RID: 317
[Serializable]
public class DrawPool
{
	// Token: 0x04000828 RID: 2088
	public Material material;

	// Token: 0x04000829 RID: 2089
	public Mesh mesh;

	// Token: 0x0400082A RID: 2090
	[HideInInspector]
	public Matrix4x4[] matricies;

	// Token: 0x0400082B RID: 2091
	[FormerlySerializedAs("pool")]
	public GameObject transformsParent;
}
