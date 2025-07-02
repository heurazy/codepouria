using System;
using UnityEngine;

// Token: 0x020001B0 RID: 432
public class CoverageToVolume : MonoBehaviour
{
	// Token: 0x06000BF2 RID: 3058 RVA: 0x0003BDB4 File Offset: 0x00039FB4
	private void Update()
	{
		if (this.aM && this.sound)
		{
			if (this.aM.obstruction <= 0.6f)
			{
				this.vol = this.max;
			}
			if (this.aM.obstruction > 0.6f)
			{
				this.vol = this.mid;
			}
			if (this.aM.obstruction >= 0.8f)
			{
				this.vol = this.min;
			}
			this.sound.volume = Mathf.Lerp(this.sound.volume, this.vol * this.mod, 0.5f * Time.deltaTime);
		}
	}

	// Token: 0x04000ACF RID: 2767
	public float mod;

	// Token: 0x04000AD0 RID: 2768
	public AudioSource sound;

	// Token: 0x04000AD1 RID: 2769
	public AmbienceAudio aM;

	// Token: 0x04000AD2 RID: 2770
	public float max = 0.1f;

	// Token: 0x04000AD3 RID: 2771
	public float mid = 0.05f;

	// Token: 0x04000AD4 RID: 2772
	public float min = 0.025f;

	// Token: 0x04000AD5 RID: 2773
	private float vol;
}
