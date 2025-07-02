using System;
using UnityEngine;

// Token: 0x02000182 RID: 386
public class AddScreenshake : MonoBehaviour
{
	// Token: 0x06000ABC RID: 2748 RVA: 0x000344CA File Offset: 0x000326CA
	private void Start()
	{
		if (!this.auto)
		{
			return;
		}
		this.Shake();
	}

	// Token: 0x06000ABD RID: 2749 RVA: 0x000344DC File Offset: 0x000326DC
	public void Shake()
	{
		if (this.positional)
		{
			GamefeelHandler.instance.AddPerlinShakeProximity(base.transform.position, this.amount, this.duration, this.scale, 15f);
			return;
		}
		GamefeelHandler.instance.AddPerlinShake(this.amount, this.duration, this.scale);
	}

	// Token: 0x0400099C RID: 2460
	public float amount = 5f;

	// Token: 0x0400099D RID: 2461
	public float duration = 0.3f;

	// Token: 0x0400099E RID: 2462
	public float scale = 12f;

	// Token: 0x0400099F RID: 2463
	public bool auto;

	// Token: 0x040009A0 RID: 2464
	public bool positional;
}
