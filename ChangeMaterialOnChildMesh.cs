using System;
using UnityEngine;

// Token: 0x020001A1 RID: 417
public class ChangeMaterialOnChildMesh : MonoBehaviour
{
	// Token: 0x06000B6D RID: 2925 RVA: 0x00038564 File Offset: 0x00036764
	public void Go()
	{
		MeshRenderer[] componentsInChildren = base.GetComponentsInChildren<MeshRenderer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].material = this.material;
		}
	}

	// Token: 0x04000A7F RID: 2687
	public Material material;
}
