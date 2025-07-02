using System;
using UnityEngine;

// Token: 0x0200025A RID: 602
public class RopeAudio : MonoBehaviour
{
	// Token: 0x06000E91 RID: 3729 RVA: 0x000491AA File Offset: 0x000473AA
	private void Start()
	{
		this.prev = this.ropeSpool.segments;
	}

	// Token: 0x06000E92 RID: 3730 RVA: 0x000491C0 File Offset: 0x000473C0
	private void Update()
	{
		this.startT -= Time.deltaTime;
		this.prev = Mathf.Lerp(this.prev, this.ropeSpool.segments, Time.deltaTime * 20f);
		if (this.startT <= 0f)
		{
			this.loop1.volume = Mathf.Lerp(this.loop1.volume, Mathf.Abs(this.prev - this.ropeSpool.segments) / 6f, 20f * Time.deltaTime);
			this.loop1.pitch = Mathf.Lerp(this.loop1.pitch, 1f + Mathf.Abs(this.prev - this.ropeSpool.segments) / 2f, 20f * Time.deltaTime);
			this.loop2.volume = Mathf.Lerp(this.loop2.volume, Mathf.Abs(this.prev - this.ropeSpool.segments) / 3f, 10f * Time.deltaTime);
			this.loop2.pitch = Mathf.Lerp(this.loop2.pitch, 0.25f + Mathf.Abs(this.prev - this.ropeSpool.segments) / 2f, 10f * Time.deltaTime);
			if (this.loop1.volume > 0.075f)
			{
				this.loop1.volume = 0.075f;
			}
			if (this.loop2.volume > 0.075f)
			{
				this.loop2.volume = 0.075f;
			}
			if (!this.t && this.ropeSpool.segments == 40f)
			{
				for (int i = 0; i < this.min.Length; i++)
				{
					this.min[i].Play(base.transform.position);
				}
				this.t = true;
			}
			if (this.t && this.ropeSpool.segments == 3f)
			{
				for (int j = 0; j < this.max.Length; j++)
				{
					this.max[j].Play(base.transform.position);
				}
				this.t = false;
			}
		}
	}

	// Token: 0x04000D85 RID: 3461
	public RopeSpool ropeSpool;

	// Token: 0x04000D86 RID: 3462
	public AudioSource loop1;

	// Token: 0x04000D87 RID: 3463
	public AudioSource loop2;

	// Token: 0x04000D88 RID: 3464
	private float prev;

	// Token: 0x04000D89 RID: 3465
	public SFX_Instance[] min;

	// Token: 0x04000D8A RID: 3466
	public SFX_Instance[] max;

	// Token: 0x04000D8B RID: 3467
	private bool t;

	// Token: 0x04000D8C RID: 3468
	private float startT = 0.5f;
}
