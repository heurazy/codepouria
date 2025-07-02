using System;
using UnityEngine;

namespace UnityStandardAssets.CrossPlatformInput
{
	// Token: 0x020002AA RID: 682
	public class InputAxisScrollbar : MonoBehaviour
	{
		// Token: 0x0600104D RID: 4173 RVA: 0x00051DBE File Offset: 0x0004FFBE
		private void Update()
		{
		}

		// Token: 0x0600104E RID: 4174 RVA: 0x00051DC0 File Offset: 0x0004FFC0
		public void HandleInput(float value)
		{
			CrossPlatformInputManager.SetAxis(this.axis, value * 2f - 1f);
		}

		// Token: 0x04000F2A RID: 3882
		public string axis;
	}
}
