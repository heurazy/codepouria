using System;
using UnityEngine;

// Token: 0x02000147 RID: 327
public class TrackPos : MonoBehaviour
{
	// Token: 0x0600095F RID: 2399 RVA: 0x0002F30E File Offset: 0x0002D50E
	private void Start()
	{
		this.startPos = base.transform.position;
		this.startRot = base.transform.rotation;
	}

	// Token: 0x06000960 RID: 2400 RVA: 0x0002F334 File Offset: 0x0002D534
	private void Update()
	{
		if (this.trackPos)
		{
			base.transform.position = this.trackTransform.position + this.startPos;
		}
		if (this.trackRot)
		{
			base.transform.rotation = this.trackTransform.rotation * this.startRot;
		}
	}

	// Token: 0x0400084B RID: 2123
	public Transform trackTransform;

	// Token: 0x0400084C RID: 2124
	private Vector3 startPos;

	// Token: 0x0400084D RID: 2125
	private Quaternion startRot;

	// Token: 0x0400084E RID: 2126
	public bool trackPos;

	// Token: 0x0400084F RID: 2127
	public bool trackRot;
}
