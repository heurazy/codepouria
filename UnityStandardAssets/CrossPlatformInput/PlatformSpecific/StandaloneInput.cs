using System;
using UnityEngine;

namespace UnityStandardAssets.CrossPlatformInput.PlatformSpecific
{
	// Token: 0x020002B0 RID: 688
	public class StandaloneInput : VirtualInput
	{
		// Token: 0x0600108D RID: 4237 RVA: 0x0005278F File Offset: 0x0005098F
		public override float GetAxis(string name, bool raw)
		{
			if (!raw)
			{
				return Input.GetAxis(name);
			}
			return Input.GetAxisRaw(name);
		}

		// Token: 0x0600108E RID: 4238 RVA: 0x000527A1 File Offset: 0x000509A1
		public override bool GetButton(string name)
		{
			return Input.GetButton(name);
		}

		// Token: 0x0600108F RID: 4239 RVA: 0x000527A9 File Offset: 0x000509A9
		public override bool GetButtonDown(string name)
		{
			return Input.GetButtonDown(name);
		}

		// Token: 0x06001090 RID: 4240 RVA: 0x000527B1 File Offset: 0x000509B1
		public override bool GetButtonUp(string name)
		{
			return Input.GetButtonUp(name);
		}

		// Token: 0x06001091 RID: 4241 RVA: 0x000527B9 File Offset: 0x000509B9
		public override void SetButtonDown(string name)
		{
			throw new Exception(" This is not possible to be called for standalone input. Please check your platform and code where this is called");
		}

		// Token: 0x06001092 RID: 4242 RVA: 0x000527C5 File Offset: 0x000509C5
		public override void SetButtonUp(string name)
		{
			throw new Exception(" This is not possible to be called for standalone input. Please check your platform and code where this is called");
		}

		// Token: 0x06001093 RID: 4243 RVA: 0x000527D1 File Offset: 0x000509D1
		public override void SetAxisPositive(string name)
		{
			throw new Exception(" This is not possible to be called for standalone input. Please check your platform and code where this is called");
		}

		// Token: 0x06001094 RID: 4244 RVA: 0x000527DD File Offset: 0x000509DD
		public override void SetAxisNegative(string name)
		{
			throw new Exception(" This is not possible to be called for standalone input. Please check your platform and code where this is called");
		}

		// Token: 0x06001095 RID: 4245 RVA: 0x000527E9 File Offset: 0x000509E9
		public override void SetAxisZero(string name)
		{
			throw new Exception(" This is not possible to be called for standalone input. Please check your platform and code where this is called");
		}

		// Token: 0x06001096 RID: 4246 RVA: 0x000527F5 File Offset: 0x000509F5
		public override void SetAxis(string name, float value)
		{
			throw new Exception(" This is not possible to be called for standalone input. Please check your platform and code where this is called");
		}

		// Token: 0x06001097 RID: 4247 RVA: 0x00052801 File Offset: 0x00050A01
		public override Vector3 MousePosition()
		{
			return Input.mousePosition;
		}
	}
}
