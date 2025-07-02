using System;
using UnityEngine;

// Token: 0x02000282 RID: 642
public class StupidRockPlacerHandler : MonoBehaviour
{
	// Token: 0x06000F6B RID: 3947 RVA: 0x0004E353 File Offset: 0x0004C553
	private void Start()
	{
	}

	// Token: 0x06000F6C RID: 3948 RVA: 0x0004E358 File Offset: 0x0004C558
	private void ReDo()
	{
		StupidRockPlacer[] componentsInChildren = base.GetComponentsInChildren<StupidRockPlacer>();
		StupidRockPlacer[] array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Clear();
		}
		foreach (StupidRockPlacer stupidRockPlacer in componentsInChildren)
		{
			int num = stupidRockPlacer.amount;
			stupidRockPlacer.amount = (int)(this.amount * (float)stupidRockPlacer.amount);
			stupidRockPlacer.Go();
			stupidRockPlacer.amount = num;
		}
	}

	// Token: 0x06000F6D RID: 3949 RVA: 0x0004E3C4 File Offset: 0x0004C5C4
	private void Clear()
	{
		StupidRockPlacer[] componentsInChildren = base.GetComponentsInChildren<StupidRockPlacer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].Clear();
		}
	}

	// Token: 0x06000F6E RID: 3950 RVA: 0x0004E3EE File Offset: 0x0004C5EE
	private void Update()
	{
	}

	// Token: 0x04000E73 RID: 3699
	public float amount = 1f;
}
