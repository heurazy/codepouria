using System;

namespace UnityEngine.UI.ProceduralImage
{
	// Token: 0x020002A5 RID: 677
	public struct ProceduralImageInfo
	{
		// Token: 0x06001022 RID: 4130 RVA: 0x00051A08 File Offset: 0x0004FC08
		public ProceduralImageInfo(float width, float height, float fallOffDistance, float pixelSize, Vector4 radius, float borderWidth)
		{
			this.width = Mathf.Abs(width);
			this.height = Mathf.Abs(height);
			this.fallOffDistance = Mathf.Max(0f, fallOffDistance);
			this.radius = radius;
			this.borderWidth = Mathf.Max(borderWidth, 0f);
			this.pixelSize = Mathf.Max(0f, pixelSize);
		}

		// Token: 0x04000F19 RID: 3865
		public float width;

		// Token: 0x04000F1A RID: 3866
		public float height;

		// Token: 0x04000F1B RID: 3867
		public float fallOffDistance;

		// Token: 0x04000F1C RID: 3868
		public Vector4 radius;

		// Token: 0x04000F1D RID: 3869
		public float borderWidth;

		// Token: 0x04000F1E RID: 3870
		public float pixelSize;
	}
}
