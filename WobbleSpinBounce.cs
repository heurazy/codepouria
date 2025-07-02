using System;
using UnityEngine;

// Token: 0x02000180 RID: 384
public class WobbleSpinBounce : MonoBehaviour
{
	// Token: 0x06000AB3 RID: 2739 RVA: 0x00034068 File Offset: 0x00032268
	private void Start()
	{
		if (this.target == null)
		{
			this.target = base.transform;
		}
		this.startPos = this.target.position;
		this.startRot = base.transform.eulerAngles;
	}

	// Token: 0x06000AB4 RID: 2740 RVA: 0x000340A8 File Offset: 0x000322A8
	private void Update()
	{
		this.target.Rotate(this.rotateSpeed);
		if (this.bounceSize != Vector3.zero)
		{
			this.target.transform.position = this.startPos + new Vector3(Mathf.Sin(Time.time * this.bounceSpeed.x) * this.bounceSize.x, Mathf.Sin(Time.time * this.bounceSpeed.y) * this.bounceSize.y, Mathf.Sin(Time.time * this.bounceSpeed.z) * this.bounceSize.z);
		}
		if (this.wobbleAmount != Vector3.zero)
		{
			this.target.transform.eulerAngles = this.startRot + new Vector3(Mathf.Sin(Time.time * this.wobbleSpeed.x) * this.wobbleAmount.x, Mathf.Sin(Time.time * this.wobbleSpeed.y) * this.wobbleAmount.y, Mathf.Sin(Time.time * this.wobbleSpeed.z) * this.wobbleAmount.z);
		}
	}

	// Token: 0x0400098B RID: 2443
	public Transform target;

	// Token: 0x0400098C RID: 2444
	[Header("Rotate")]
	public Vector3 rotateSpeed;

	// Token: 0x0400098D RID: 2445
	public Vector3 wobbleSpeed;

	// Token: 0x0400098E RID: 2446
	public Vector3 wobbleAmount;

	// Token: 0x0400098F RID: 2447
	[Header("Position")]
	public Vector3 bounceSize;

	// Token: 0x04000990 RID: 2448
	public Vector3 bounceSpeed;

	// Token: 0x04000991 RID: 2449
	private Vector3 startPos;

	// Token: 0x04000992 RID: 2450
	private Vector3 startRot;
}
