using System;
using UnityEngine;

// Token: 0x020001C7 RID: 455
public class FallAudio : MonoBehaviour
{
	// Token: 0x06000C3A RID: 3130 RVA: 0x0003CE84 File Offset: 0x0003B084
	private void Update()
	{
		this.yVel = base.transform.position.y - this.prevY;
		this.prevY = base.transform.position.y;
		this.au.volume = Mathf.Lerp(this.au.volume, Mathf.Abs(this.yVel) / 10f, Time.deltaTime * 10f);
		if (this.au.volume > 0.5f)
		{
			this.au.volume = 0.5f;
		}
	}

	// Token: 0x04000B31 RID: 2865
	public AudioSource au;

	// Token: 0x04000B32 RID: 2866
	private float yVel;

	// Token: 0x04000B33 RID: 2867
	private float prevY;
}
