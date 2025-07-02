using System;
using UnityEngine;

// Token: 0x020000F2 RID: 242
public class MaterialSetter : MonoBehaviour
{
	// Token: 0x0600073A RID: 1850 RVA: 0x000263C4 File Offset: 0x000245C4
	public void setMaterial()
	{
		Renderer[] componentsInChildren = base.GetComponentsInChildren<Renderer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].sharedMaterial = this.material;
		}
	}

	// Token: 0x040006CB RID: 1739
	public Material material;
}
