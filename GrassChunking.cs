using System;
using Unity.Mathematics;
using UnityEngine;

// Token: 0x02000093 RID: 147
public static class GrassChunking
{
	// Token: 0x06000521 RID: 1313 RVA: 0x0001D524 File Offset: 0x0001B724
	public static int3 GetChunkFromPosition(float3 p)
	{
		int num = Mathf.FloorToInt(p.x * GrassChunking.CHUNK_SIZE_INV);
		int num2 = Mathf.FloorToInt(p.y * GrassChunking.CHUNK_SIZE_INV);
		int num3 = Mathf.FloorToInt(p.z * GrassChunking.CHUNK_SIZE_INV);
		return new int3(num, num2, num3);
	}

	// Token: 0x06000522 RID: 1314 RVA: 0x0001D570 File Offset: 0x0001B770
	public static bool ShouldDrawChunk(int3 cameraChunk, int3 renderChunk)
	{
		return Mathf.Abs(cameraChunk.x - renderChunk.x) <= 1 && Mathf.Abs(cameraChunk.y - renderChunk.y) <= 1 && Mathf.Abs(cameraChunk.z - renderChunk.z) <= 1;
	}

	// Token: 0x0400053D RID: 1341
	public static readonly float CHUNK_SIZE = 35f;

	// Token: 0x0400053E RID: 1342
	public static readonly float CHUNK_SIZE_INV = 1f / GrassChunking.CHUNK_SIZE;
}
