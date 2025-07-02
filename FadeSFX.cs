using System;
using UnityEngine;

// Token: 0x020001C6 RID: 454
public class FadeSFX : MonoBehaviour
{
	// Token: 0x06000C38 RID: 3128 RVA: 0x0003CE6E File Offset: 0x0003B06E
	private void Update()
	{
		AudioListener.volume = this.f;
	}

	// Token: 0x04000B30 RID: 2864
	public float f;
}
