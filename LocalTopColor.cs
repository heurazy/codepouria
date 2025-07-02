using System;
using UnityEngine;

// Token: 0x02000084 RID: 132
public class LocalTopColor : MonoBehaviour
{
	// Token: 0x0600049C RID: 1180 RVA: 0x0001AB72 File Offset: 0x00018D72
	private void Start()
	{
		this.setTopVector();
	}

	// Token: 0x0600049D RID: 1181 RVA: 0x0001AB7C File Offset: 0x00018D7C
	private void setTopVector()
	{
		MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
		Vector3 vector = base.transform.InverseTransformDirection(Vector3.up);
		materialPropertyBlock.SetVector("_LocalTopDirection", vector);
		this.renderer.SetPropertyBlock(materialPropertyBlock);
	}

	// Token: 0x040004D0 RID: 1232
	public MeshRenderer renderer;
}
