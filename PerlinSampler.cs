using System;
using UnityEngine;

// Token: 0x0200009B RID: 155
[Serializable]
public class PerlinSampler
{
	// Token: 0x060005BC RID: 1468 RVA: 0x0002020C File Offset: 0x0001E40C
	public bool Sample(Vector2 pos, int seed = 0)
	{
		float num = this.SampleValue(pos, seed);
		return num > this.minMax.x && num < this.minMax.y;
	}

	// Token: 0x060005BD RID: 1469 RVA: 0x00020240 File Offset: 0x0001E440
	public float SampleValue(Vector2 pos, int seed = 0)
	{
		float num = 0f;
		for (int i = 0; i < this.iterations; i++)
		{
			float num2 = this.scale;
			num2 *= Mathf.Pow(this.roughness, (float)i);
			float num3 = Mathf.PerlinNoise((float)(12345 + seed) + pos.x * num2 * 0.1f, (float)(12345 + seed) + pos.y * num2 * 0.1f);
			if (i == 0)
			{
				num = num3;
			}
			else
			{
				float num4 = Mathf.Pow(this.roughness, (float)i);
				num = Mathf.Lerp(num, num3, num4);
			}
		}
		if (!Mathf.Approximately(this.pow, 1f))
		{
			num = Mathf.Pow(num, this.pow);
		}
		return num;
	}

	// Token: 0x040005C3 RID: 1475
	public float scale = 1f;

	// Token: 0x040005C4 RID: 1476
	public int iterations = 2;

	// Token: 0x040005C5 RID: 1477
	public float scaleIncrease = 3f;

	// Token: 0x040005C6 RID: 1478
	public float roughness = 0.3f;

	// Token: 0x040005C7 RID: 1479
	public float pow = 1f;

	// Token: 0x040005C8 RID: 1480
	public Vector2 minMax = new Vector2(0f, 1f);
}
