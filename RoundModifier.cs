using System;
using UnityEngine;
using UnityEngine.UI.ProceduralImage;

// Token: 0x02000224 RID: 548
[ModifierID("Round")]
public class RoundModifier : ProceduralImageModifier
{
	// Token: 0x06000DF8 RID: 3576 RVA: 0x000465C5 File Offset: 0x000447C5
	public override Vector4 CalculateRadius(Rect imageRect)
	{
		float num = Mathf.Min(imageRect.width, imageRect.height) * 0.5f;
		return new Vector4(num, num, num, num);
	}
}
