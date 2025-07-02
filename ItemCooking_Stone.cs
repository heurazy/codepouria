using System;
using UnityEngine;

// Token: 0x020000A0 RID: 160
public class ItemCooking_Stone : ItemCooking
{
	// Token: 0x060005D3 RID: 1491 RVA: 0x00020A34 File Offset: 0x0001EC34
	protected override void CookVisually(int cookedAmount)
	{
		for (int i = 0; i < cookedAmount; i++)
		{
			foreach (Renderer renderer in base.GetComponentsInChildren<Renderer>())
			{
				renderer.material.SetColor("_Tint", Vector4.MoveTowards(renderer.material.GetColor("_Tint"), this.heatColor, 0.15f));
			}
		}
	}

	// Token: 0x040005D8 RID: 1496
	public Color heatColor;
}
