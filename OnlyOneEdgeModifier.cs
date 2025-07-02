using System;
using UnityEngine;
using UnityEngine.UI.ProceduralImage;

// Token: 0x02000223 RID: 547
[ModifierID("Only One Edge")]
public class OnlyOneEdgeModifier : ProceduralImageModifier
{
	// Token: 0x170000B8 RID: 184
	// (get) Token: 0x06000DF2 RID: 3570 RVA: 0x000464D9 File Offset: 0x000446D9
	// (set) Token: 0x06000DF3 RID: 3571 RVA: 0x000464E1 File Offset: 0x000446E1
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

	// Token: 0x170000B9 RID: 185
	// (get) Token: 0x06000DF4 RID: 3572 RVA: 0x000464F5 File Offset: 0x000446F5
	// (set) Token: 0x06000DF5 RID: 3573 RVA: 0x000464FD File Offset: 0x000446FD
	public OnlyOneEdgeModifier.ProceduralImageEdge Side
	{
		get
		{
			return this.side;
		}
		set
		{
			this.side = value;
		}
	}

	// Token: 0x06000DF6 RID: 3574 RVA: 0x00046508 File Offset: 0x00044708
	public override Vector4 CalculateRadius(Rect imageRect)
	{
		switch (this.side)
		{
		case OnlyOneEdgeModifier.ProceduralImageEdge.Top:
			return new Vector4(this.radius, this.radius, 0f, 0f);
		case OnlyOneEdgeModifier.ProceduralImageEdge.Bottom:
			return new Vector4(0f, 0f, this.radius, this.radius);
		case OnlyOneEdgeModifier.ProceduralImageEdge.Left:
			return new Vector4(this.radius, 0f, 0f, this.radius);
		case OnlyOneEdgeModifier.ProceduralImageEdge.Right:
			return new Vector4(0f, this.radius, this.radius, 0f);
		default:
			return new Vector4(0f, 0f, 0f, 0f);
		}
	}

	// Token: 0x04000D08 RID: 3336
	[SerializeField]
	private float radius;

	// Token: 0x04000D09 RID: 3337
	[SerializeField]
	private OnlyOneEdgeModifier.ProceduralImageEdge side;

	// Token: 0x020003A2 RID: 930
	public enum ProceduralImageEdge
	{
		// Token: 0x04001369 RID: 4969
		Top,
		// Token: 0x0400136A RID: 4970
		Bottom,
		// Token: 0x0400136B RID: 4971
		Left,
		// Token: 0x0400136C RID: 4972
		Right
	}
}
