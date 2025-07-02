using System;
using UnityEngine;

// Token: 0x02000176 RID: 374
public class TextUmamiEffect : DialogueEffect
{
	// Token: 0x1700009D RID: 157
	// (get) Token: 0x06000A86 RID: 2694 RVA: 0x000336C4 File Offset: 0x000318C4
	public virtual float colorSpeedMult
	{
		get
		{
			return 1f;
		}
	}

	// Token: 0x06000A87 RID: 2695 RVA: 0x000336CC File Offset: 0x000318CC
	public override void UpdateCharacter(int index)
	{
		float num = this.offset * (float)index;
		float num2 = Mathf.Sin((Time.time + num) / this.period);
		float num3 = 1f + num2 * this.amplitude;
		Vector3 vector = Vector3.one * num3;
		this.DTanimator.SetCharScale(index, vector);
		this.DTanimator.SetCharOffset(index, Vector3.up * num3 * this.charOffset);
		float num4 = (Mathf.Sin((Time.time + num) / (this.period / this.colorSpeedMult)) + 1f) * 0.5f;
		this.DTanimator.SetCharColor(index, this.colorGradient.Evaluate(num4));
	}

	// Token: 0x04000969 RID: 2409
	public bool abs;

	// Token: 0x0400096A RID: 2410
	public float amplitude = 0.2f;

	// Token: 0x0400096B RID: 2411
	public float period = 0.5f;

	// Token: 0x0400096C RID: 2412
	public float offset = 0.1f;

	// Token: 0x0400096D RID: 2413
	public float charOffset = 10f;

	// Token: 0x0400096E RID: 2414
	public Gradient colorGradient;
}
