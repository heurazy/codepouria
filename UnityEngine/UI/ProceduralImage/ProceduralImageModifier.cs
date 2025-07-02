using System;

namespace UnityEngine.UI.ProceduralImage
{
	// Token: 0x020002A6 RID: 678
	[DisallowMultipleComponent]
	public abstract class ProceduralImageModifier : MonoBehaviour
	{
		// Token: 0x170000C8 RID: 200
		// (get) Token: 0x06001023 RID: 4131 RVA: 0x00051A6A File Offset: 0x0004FC6A
		protected Graphic _Graphic
		{
			get
			{
				if (this.graphic == null)
				{
					this.graphic = base.GetComponent<Graphic>();
				}
				return this.graphic;
			}
		}

		// Token: 0x06001024 RID: 4132
		public abstract Vector4 CalculateRadius(Rect imageRect);

		// Token: 0x04000F1F RID: 3871
		protected Graphic graphic;
	}
}
