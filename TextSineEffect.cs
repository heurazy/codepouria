using System;
using UnityEngine;

// Token: 0x02000175 RID: 373
public class TextSineEffect : DialogueEffect
{
	// Token: 0x06000A84 RID: 2692 RVA: 0x0003362C File Offset: 0x0003182C
	public override void UpdateCharacter(int index)
	{
		float num = this.offset * (float)index;
		Vector3 vector = Vector3.up * (Mathf.Sin((Time.time + num) / this.period) * this.amplitude);
		if (this.abs)
		{
			vector = new Vector3(vector.x, Mathf.Abs(vector.y), vector.z);
		}
		this.DTanimator.SetCharOffset(index, vector);
	}

	// Token: 0x04000965 RID: 2405
	public bool abs;

	// Token: 0x04000966 RID: 2406
	public float amplitude = 3f;

	// Token: 0x04000967 RID: 2407
	public float period = 0.15f;

	// Token: 0x04000968 RID: 2408
	public float offset = 0.1f;
}
