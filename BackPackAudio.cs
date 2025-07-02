using System;
using UnityEngine;

// Token: 0x0200018C RID: 396
public class BackPackAudio : MonoBehaviour
{
	// Token: 0x06000AE1 RID: 2785 RVA: 0x00035C4E File Offset: 0x00033E4E
	private void Start()
	{
		this.item = base.GetComponent<Backpack>();
	}

	// Token: 0x06000AE2 RID: 2786 RVA: 0x00035C5C File Offset: 0x00033E5C
	private void Update()
	{
		if (this.item)
		{
			if (this.item.holderCharacter)
			{
				if (!this.hT)
				{
					for (int i = 0; i < this.holdSFX.Length; i++)
					{
						this.holdSFX[i].Play(base.transform.position);
					}
					this.hT = true;
				}
			}
			else
			{
				this.hT = false;
			}
			if (this.item.rig.useGravity)
			{
				if (!this.dT)
				{
					for (int j = 0; j < this.dropSFX.Length; j++)
					{
						this.dropSFX[j].Play(base.transform.position);
					}
				}
				this.dT = true;
				return;
			}
			this.dT = false;
		}
	}

	// Token: 0x040009EE RID: 2542
	private Backpack item;

	// Token: 0x040009EF RID: 2543
	public SFX_Instance[] holdSFX;

	// Token: 0x040009F0 RID: 2544
	private bool hT;

	// Token: 0x040009F1 RID: 2545
	public SFX_Instance[] dropSFX;

	// Token: 0x040009F2 RID: 2546
	private bool dT;
}
