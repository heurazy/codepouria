using System;
using UnityEngine;

// Token: 0x0200018A RID: 394
public class AudioLoop : MonoBehaviour
{
	// Token: 0x06000ADD RID: 2781 RVA: 0x00035B9C File Offset: 0x00033D9C
	private void Update()
	{
		this.loop.volume = Mathf.Lerp(this.loop.volume, this.volume, 2f * Time.deltaTime);
		this.loop.pitch = Mathf.Lerp(this.loop.pitch, this.pitch, 2f * Time.deltaTime);
	}

	// Token: 0x040009E9 RID: 2537
	public AudioSource loop;

	// Token: 0x040009EA RID: 2538
	public float volume;

	// Token: 0x040009EB RID: 2539
	public float pitch = 1f;
}
