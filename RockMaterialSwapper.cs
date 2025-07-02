using System;
using UnityEngine;

// Token: 0x02000258 RID: 600
public class RockMaterialSwapper : MonoBehaviour
{
	// Token: 0x06000E89 RID: 3721 RVA: 0x00048DA0 File Offset: 0x00046FA0
	private void Start()
	{
		Transform[] array = this.parents;
		for (int i = 0; i < array.Length; i++)
		{
			MeshRenderer[] componentsInChildren = array[i].GetComponentsInChildren<MeshRenderer>(true);
			for (int j = 0; j < componentsInChildren.Length; j++)
			{
				componentsInChildren[j].sharedMaterial = this.mat;
			}
		}
	}

	// Token: 0x04000D79 RID: 3449
	public Transform[] parents;

	// Token: 0x04000D7A RID: 3450
	public Material mat;
}
