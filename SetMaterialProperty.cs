using System;
using UnityEngine;

// Token: 0x02000268 RID: 616
public class SetMaterialProperty : MonoBehaviour
{
	// Token: 0x06000EDE RID: 3806 RVA: 0x0004ACD1 File Offset: 0x00048ED1
	public void Go()
	{
		this.SetVal(this.propertyValue);
	}

	// Token: 0x06000EDF RID: 3807 RVA: 0x0004ACE0 File Offset: 0x00048EE0
	public void SetVal(float val)
	{
		Renderer component = base.GetComponent<Renderer>();
		MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
		component.GetPropertyBlock(materialPropertyBlock);
		materialPropertyBlock.SetFloat(this.propertyName, val);
		component.SetPropertyBlock(materialPropertyBlock);
	}

	// Token: 0x04000DB9 RID: 3513
	public string propertyName;

	// Token: 0x04000DBA RID: 3514
	public float propertyValue;
}
