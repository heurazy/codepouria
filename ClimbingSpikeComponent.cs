using System;
using UnityEngine;
using Zorro.Core;

// Token: 0x02000012 RID: 18
public class ClimbingSpikeComponent : ItemComponent
{
	// Token: 0x0600017D RID: 381 RVA: 0x0000C3D2 File Offset: 0x0000A5D2
	public override void OnInstanceDataSet()
	{
	}

	// Token: 0x0600017E RID: 382 RVA: 0x0000C3D4 File Offset: 0x0000A5D4
	private void Start()
	{
		this.item.overrideUsability = Optionable<bool>.Some(false);
	}

	// Token: 0x0400017A RID: 378
	public GameObject hammeredVersionPrefab;

	// Token: 0x0400017B RID: 379
	public GameObject climbingSpikePreviewPrefab;

	// Token: 0x0400017C RID: 380
	public float climbingSpikeStartDistance;

	// Token: 0x0400017D RID: 381
	public float climbingSpikePreviewDisableDistance;

	// Token: 0x0400017E RID: 382
	public float climbingSpikeStartDistanceGrounded;

	// Token: 0x0400017F RID: 383
	public float climbingSpikePreviewDisableDistanceGrounded;
}
