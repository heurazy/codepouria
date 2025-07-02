using System;
using UnityEngine;

// Token: 0x0200026A RID: 618
public class SetRockColors : MonoBehaviour
{
	// Token: 0x06000EE5 RID: 3813 RVA: 0x0004AD4C File Offset: 0x00048F4C
	private void Start()
	{
		foreach (Material material in this.matsToEdit)
		{
			material.SetColor("_TopColor", this.topColor);
			material.SetColor("_Tint", this.tint);
		}
	}

	// Token: 0x04000DBD RID: 3517
	public Color topColor;

	// Token: 0x04000DBE RID: 3518
	[ColorUsage(false, true)]
	public Color tint;

	// Token: 0x04000DBF RID: 3519
	public Material[] matsToEdit;
}
