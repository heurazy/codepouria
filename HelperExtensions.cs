using System;
using UnityEngine;

// Token: 0x02000098 RID: 152
public static class HelperExtensions
{
	// Token: 0x06000576 RID: 1398 RVA: 0x0001F0AC File Offset: 0x0001D2AC
	public static LayerMask ToLayerMask(this HelperFunctions.LayerType me)
	{
		return HelperFunctions.GetMask(me);
	}
}
