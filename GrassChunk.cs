using System;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

// Token: 0x02000092 RID: 146
public class GrassChunk : GrassDataProvider
{
	// Token: 0x0600051C RID: 1308 RVA: 0x0001D493 File Offset: 0x0001B693
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.cyan;
		Gizmos.DrawWireCube(base.transform.position + Vector3.one * 50f, Vector3.one * GrassChunking.CHUNK_SIZE);
	}

	// Token: 0x0600051D RID: 1309 RVA: 0x0001D4D2 File Offset: 0x0001B6D2
	public override bool IsDirty()
	{
		return this.isDirty;
	}

	// Token: 0x0600051E RID: 1310 RVA: 0x0001D4DA File Offset: 0x0001B6DA
	public override ComputeBuffer GetData()
	{
		ComputeBuffer computeBuffer = new ComputeBuffer(this.GrassPoints.Count, UnsafeUtility.SizeOf<GrassPoint>());
		computeBuffer.SetData<GrassPoint>(this.GrassPoints);
		this.isDirty = false;
		return computeBuffer;
	}

	// Token: 0x0600051F RID: 1311 RVA: 0x0001D504 File Offset: 0x0001B704
	public void SetData(List<GrassPoint> grassPoints)
	{
		this.GrassPoints = grassPoints;
		this.isDirty = true;
	}

	// Token: 0x0400053B RID: 1339
	public List<GrassPoint> GrassPoints;

	// Token: 0x0400053C RID: 1340
	public bool isDirty = true;
}
