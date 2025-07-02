using System;
using UnityEngine;
using UnityEngine.UI.ProceduralImage;

// Token: 0x02000222 RID: 546
[ModifierID("Free")]
public class FreeModifier : ProceduralImageModifier
{
	// Token: 0x170000B7 RID: 183
	// (get) Token: 0x06000DED RID: 3565 RVA: 0x0004641E File Offset: 0x0004461E
	// (set) Token: 0x06000DEE RID: 3566 RVA: 0x00046426 File Offset: 0x00044626
	public Vector4 Radius
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

	// Token: 0x06000DEF RID: 3567 RVA: 0x0004643A File Offset: 0x0004463A
	public override Vector4 CalculateRadius(Rect imageRect)
	{
		return this.radius;
	}

	// Token: 0x06000DF0 RID: 3568 RVA: 0x00046444 File Offset: 0x00044644
	protected void OnValidate()
	{
		this.radius.x = Mathf.Max(0f, this.radius.x);
		this.radius.y = Mathf.Max(0f, this.radius.y);
		this.radius.z = Mathf.Max(0f, this.radius.z);
		this.radius.w = Mathf.Max(0f, this.radius.w);
	}

	// Token: 0x04000D07 RID: 3335
	[SerializeField]
	private Vector4 radius;
}
