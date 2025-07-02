using System;
using UnityEngine;

// Token: 0x0200026B RID: 619
public class SFXOnChildCount : MonoBehaviour
{
	// Token: 0x06000EE7 RID: 3815 RVA: 0x0004AD9A File Offset: 0x00048F9A
	private void Start()
	{
		this.index = base.transform.childCount;
	}

	// Token: 0x06000EE8 RID: 3816 RVA: 0x0004ADB0 File Offset: 0x00048FB0
	private void Update()
	{
		if (this.index != base.transform.childCount)
		{
			for (int i = 0; i < this.sfx.Length; i++)
			{
				this.sfx[i].Play(default(Vector3));
			}
		}
		this.index = base.transform.childCount;
	}

	// Token: 0x04000DC0 RID: 3520
	public SFX_Instance[] sfx;

	// Token: 0x04000DC1 RID: 3521
	private int index;
}
