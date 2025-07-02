using System;

namespace UnityEngine.UI.ProceduralImage
{
	// Token: 0x020002A3 RID: 675
	[AttributeUsage(AttributeTargets.Class)]
	public class ModifierID : Attribute
	{
		// Token: 0x06001007 RID: 4103 RVA: 0x00051482 File Offset: 0x0004F682
		public ModifierID(string name)
		{
			this.name = name;
		}

		// Token: 0x170000C1 RID: 193
		// (get) Token: 0x06001008 RID: 4104 RVA: 0x00051491 File Offset: 0x0004F691
		public string Name
		{
			get
			{
				return this.name;
			}
		}

		// Token: 0x04000F14 RID: 3860
		private string name;
	}
}
