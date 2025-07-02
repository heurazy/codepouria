using System;
using UnityEngine;

// Token: 0x0200027E RID: 638
public class StepSoundCollection : MonoBehaviour
{
	// Token: 0x06000F57 RID: 3927 RVA: 0x0004D8BC File Offset: 0x0004BABC
	public void PlayStep(Vector3 pos, int index)
	{
		if (index == 0)
		{
			for (int i = 0; i < this.stepDefault.Length; i++)
			{
				this.stepDefault[i].Play(pos);
			}
		}
		if (index == 1)
		{
			for (int j = 0; j < this.beachSand.Length; j++)
			{
				this.beachSand[j].Play(pos);
			}
		}
		if (index == 2)
		{
			for (int k = 0; k < this.beachRock.Length; k++)
			{
				this.beachRock[k].Play(pos);
			}
		}
		if (index == 3)
		{
			for (int l = 0; l < this.jungleGrass.Length; l++)
			{
				this.jungleGrass[l].Play(pos);
			}
		}
		if (index == 4)
		{
			for (int m = 0; m < this.jungleRock.Length; m++)
			{
				this.jungleRock[m].Play(pos);
			}
		}
		if (index == 5)
		{
			for (int n = 0; n < this.iceSnow.Length; n++)
			{
				this.iceSnow[n].Play(pos);
			}
		}
		if (index == 6)
		{
			for (int num = 0; num < this.iceSnow.Length; num++)
			{
				this.iceSnow[num].Play(pos);
			}
		}
		if (index == 7)
		{
			for (int num2 = 0; num2 < this.metal.Length; num2++)
			{
				this.metal[num2].Play(pos);
			}
		}
		if (index == 8)
		{
			for (int num3 = 0; num3 < this.wood.Length; num3++)
			{
				this.wood[num3].Play(pos);
			}
		}
		if (index == 9)
		{
			for (int num4 = 0; num4 < this.volcanoRock.Length; num4++)
			{
				this.volcanoRock[num4].Play(pos);
			}
		}
	}

	// Token: 0x04000E54 RID: 3668
	public SFX_Instance[] stepDefault;

	// Token: 0x04000E55 RID: 3669
	public SFX_Instance[] beachSand;

	// Token: 0x04000E56 RID: 3670
	public SFX_Instance[] beachRock;

	// Token: 0x04000E57 RID: 3671
	public SFX_Instance[] jungleGrass;

	// Token: 0x04000E58 RID: 3672
	public SFX_Instance[] jungleRock;

	// Token: 0x04000E59 RID: 3673
	public SFX_Instance[] iceSnow;

	// Token: 0x04000E5A RID: 3674
	public SFX_Instance[] iceRock;

	// Token: 0x04000E5B RID: 3675
	public SFX_Instance[] metal;

	// Token: 0x04000E5C RID: 3676
	public SFX_Instance[] wood;

	// Token: 0x04000E5D RID: 3677
	public SFX_Instance[] volcanoRock;
}
