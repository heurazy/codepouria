using System;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x0200029F RID: 671
public class WarpSFX : MonoBehaviour
{
	// Token: 0x06000FFE RID: 4094 RVA: 0x000512BF File Offset: 0x0004F4BF
	private void Update()
	{
		this.warpSFX.volume = this.vol.weight / 2f;
		this.warpSFX.pitch = 1f + this.vol.weight * 2f;
	}

	// Token: 0x04000F09 RID: 3849
	public Volume vol;

	// Token: 0x04000F0A RID: 3850
	public AudioSource warpSFX;
}
