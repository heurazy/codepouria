using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200017B RID: 379
public static class Util
{
	// Token: 0x1700009F RID: 159
	// (get) Token: 0x06000AA1 RID: 2721 RVA: 0x00033B01 File Offset: 0x00031D01
	public static Random random
	{
		get
		{
			if (Util.r == null)
			{
				Util.r = new Random();
			}
			return Util.r;
		}
	}

	// Token: 0x06000AA2 RID: 2722 RVA: 0x00033B1C File Offset: 0x00031D1C
	public static float RangeLerp(float min, float max, float minParam, float maxParam, float param, bool clamp = true, AnimationCurve curve = null)
	{
		if (maxParam - minParam == 0f)
		{
			return min;
		}
		float num = Mathf.Clamp((param - minParam) / (maxParam - minParam), 0f, 1f);
		if (curve != null && curve.keys.Length != 0)
		{
			num = curve.Evaluate(num);
		}
		float num2 = max - min;
		return min + num2 * num;
	}

	// Token: 0x06000AA3 RID: 2723 RVA: 0x00033B70 File Offset: 0x00031D70
	public static T RandomSelection<T>(this IEnumerable<T> enumerable, Func<T, int> weightFunc)
	{
		int num = 0;
		T t = default(T);
		foreach (T t2 in enumerable)
		{
			int num2 = weightFunc(t2);
			if (Util.random.Next(num + num2) >= num)
			{
				t = t2;
			}
			num += num2;
		}
		T t3 = t;
		return t;
	}

	// Token: 0x06000AA4 RID: 2724 RVA: 0x00033BE4 File Offset: 0x00031DE4
	public static Vector2 FlattenVector3(Vector3 original)
	{
		return new Vector2(original.x, original.z);
	}

	// Token: 0x06000AA5 RID: 2725 RVA: 0x00033BF8 File Offset: 0x00031DF8
	public static float GenerateNormalDistribution(float mean, float stdDev)
	{
		double num = 1.0 - (double)Random.value;
		double num2 = 1.0 - (double)Random.value;
		double num3 = Math.Sqrt(-2.0 * Math.Log(num)) * Math.Cos(6.283185307179586 * num2);
		Debug.Log(string.Concat(new string[]
		{
			"Created random distribution result:",
			num3.ToString(),
			" mean: ",
			mean.ToString(),
			" stdDev: ",
			stdDev.ToString()
		}));
		float num4 = (float)num3;
		return mean + num4 * stdDev;
	}

	// Token: 0x04000984 RID: 2436
	private static Random r;
}
