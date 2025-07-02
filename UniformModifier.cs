using System;
using UnityEngine;
using UnityEngine.UI.ProceduralImage;

// Token: 0x02000225 RID: 549
[ModifierID("Uniform")]
public class UniformModifier : ProceduralImageModifier
{
	// Token: 0x170000BA RID: 186
	// (get) Token: 0x06000DFA RID: 3578 RVA: 0x000465F0 File Offset: 0x000447F0
	// (set) Token: 0x06000DFB RID: 3579 RVA: 0x000465F8 File Offset: 0x000447F8
	public float Radius
	{
		get
		{
			return this.radius;
		}
		set
		{
			this.radius = value;
			base._Graphic.SetVerticesDirty();
		}
	}

	// Token: 0x06000DFC RID: 3580 RVA: 0x0004660C File Offset: 0x0004480C
	public override Vector4 CalculateRadius(Rect imageRect)
	{
		float num = this.radius;
		return new Vector4(num, num, num, num);
	}

	// Token: 0x04000D0A RID: 3338
	[SerializeField]
	private float radius;
}
