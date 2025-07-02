using System;
using UnityEngine;

namespace UnityStandardAssets.CrossPlatformInput
{
	// Token: 0x020002A8 RID: 680
	public class ButtonHandler : MonoBehaviour
	{
		// Token: 0x0600102C RID: 4140 RVA: 0x00051C00 File Offset: 0x0004FE00
		private void OnEnable()
		{
		}

		// Token: 0x0600102D RID: 4141 RVA: 0x00051C02 File Offset: 0x0004FE02
		public void SetDownState()
		{
			CrossPlatformInputManager.SetButtonDown(this.Name);
		}

		// Token: 0x0600102E RID: 4142 RVA: 0x00051C0F File Offset: 0x0004FE0F
		public void SetUpState()
		{
			CrossPlatformInputManager.SetButtonUp(this.Name);
		}

		// Token: 0x0600102F RID: 4143 RVA: 0x00051C1C File Offset: 0x0004FE1C
		public void SetAxisPositiveState()
		{
			CrossPlatformInputManager.SetAxisPositive(this.Name);
		}

		// Token: 0x06001030 RID: 4144 RVA: 0x00051C29 File Offset: 0x0004FE29
		public void SetAxisNeutralState()
		{
			CrossPlatformInputManager.SetAxisZero(this.Name);
		}

		// Token: 0x06001031 RID: 4145 RVA: 0x00051C36 File Offset: 0x0004FE36
		public void SetAxisNegativeState()
		{
			CrossPlatformInputManager.SetAxisNegative(this.Name);
		}

		// Token: 0x06001032 RID: 4146 RVA: 0x00051C43 File Offset: 0x0004FE43
		public void Update()
		{
		}

		// Token: 0x04000F26 RID: 3878
		public string Name;
	}
}
