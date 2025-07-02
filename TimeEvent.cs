using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000288 RID: 648
public class TimeEvent : MonoBehaviour
{
	// Token: 0x06000F96 RID: 3990 RVA: 0x0004F2D8 File Offset: 0x0004D4D8
	private void Update()
	{
		this.counter += Time.deltaTime;
		if (this.counter > this.rate)
		{
			if (!this.repeating)
			{
				base.enabled = false;
			}
			this.timeEvent.Invoke();
			this.counter = 0f;
		}
	}

	// Token: 0x06000F97 RID: 3991 RVA: 0x0004F32A File Offset: 0x0004D52A
	private void OnEnable()
	{
		this.counter = 0f;
	}

	// Token: 0x04000E9A RID: 3738
	private float counter;

	// Token: 0x04000E9B RID: 3739
	public float rate = 2f;

	// Token: 0x04000E9C RID: 3740
	public bool repeating;

	// Token: 0x04000E9D RID: 3741
	public UnityEvent timeEvent;
}
