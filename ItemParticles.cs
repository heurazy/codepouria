using System;
using UnityEngine;

// Token: 0x0200001C RID: 28
public class ItemParticles : MonoBehaviour
{
	// Token: 0x06000202 RID: 514 RVA: 0x0000ED1F File Offset: 0x0000CF1F
	public void EnableSmoke(bool active)
	{
		if (this.smoke)
		{
			if (active)
			{
				this.smoke.Play();
				return;
			}
			this.smoke.Stop();
		}
	}

	// Token: 0x040001FA RID: 506
	public ParticleSystem smoke;
}
