using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityStandardAssets.CrossPlatformInput
{
	// Token: 0x020002AE RID: 686
	public abstract class VirtualInput
	{
		// Token: 0x170000CA RID: 202
		// (get) Token: 0x06001067 RID: 4199 RVA: 0x000523A5 File Offset: 0x000505A5
		// (set) Token: 0x06001068 RID: 4200 RVA: 0x000523AD File Offset: 0x000505AD
		public Vector3 virtualMousePosition { get; private set; }

		// Token: 0x06001069 RID: 4201 RVA: 0x000523B6 File Offset: 0x000505B6
		public bool AxisExists(string name)
		{
			return this.m_VirtualAxes.ContainsKey(name);
		}

		// Token: 0x0600106A RID: 4202 RVA: 0x000523C4 File Offset: 0x000505C4
		public bool ButtonExists(string name)
		{
			return this.m_VirtualButtons.ContainsKey(name);
		}

		// Token: 0x0600106B RID: 4203 RVA: 0x000523D4 File Offset: 0x000505D4
		public void RegisterVirtualAxis(CrossPlatformInputManager.VirtualAxis axis)
		{
			if (this.m_VirtualAxes.ContainsKey(axis.name))
			{
				Debug.LogError("There is already a virtual axis named " + axis.name + " registered.");
				return;
			}
			this.m_VirtualAxes.Add(axis.name, axis);
			if (!axis.matchWithInputManager)
			{
				this.m_AlwaysUseVirtual.Add(axis.name);
			}
		}

		// Token: 0x0600106C RID: 4204 RVA: 0x0005243C File Offset: 0x0005063C
		public void RegisterVirtualButton(CrossPlatformInputManager.VirtualButton button)
		{
			if (this.m_VirtualButtons.ContainsKey(button.name))
			{
				Debug.LogError("There is already a virtual button named " + button.name + " registered.");
				return;
			}
			this.m_VirtualButtons.Add(button.name, button);
			if (!button.matchWithInputManager)
			{
				this.m_AlwaysUseVirtual.Add(button.name);
			}
		}

		// Token: 0x0600106D RID: 4205 RVA: 0x000524A2 File Offset: 0x000506A2
		public void UnRegisterVirtualAxis(string name)
		{
			if (this.m_VirtualAxes.ContainsKey(name))
			{
				this.m_VirtualAxes.Remove(name);
			}
		}

		// Token: 0x0600106E RID: 4206 RVA: 0x000524BF File Offset: 0x000506BF
		public void UnRegisterVirtualButton(string name)
		{
			if (this.m_VirtualButtons.ContainsKey(name))
			{
				this.m_VirtualButtons.Remove(name);
			}
		}

		// Token: 0x0600106F RID: 4207 RVA: 0x000524DC File Offset: 0x000506DC
		public CrossPlatformInputManager.VirtualAxis VirtualAxisReference(string name)
		{
			if (!this.m_VirtualAxes.ContainsKey(name))
			{
				return null;
			}
			return this.m_VirtualAxes[name];
		}

		// Token: 0x06001070 RID: 4208 RVA: 0x000524FA File Offset: 0x000506FA
		public void SetVirtualMousePositionX(float f)
		{
			this.virtualMousePosition = new Vector3(f, this.virtualMousePosition.y, this.virtualMousePosition.z);
		}

		// Token: 0x06001071 RID: 4209 RVA: 0x0005251E File Offset: 0x0005071E
		public void SetVirtualMousePositionY(float f)
		{
			this.virtualMousePosition = new Vector3(this.virtualMousePosition.x, f, this.virtualMousePosition.z);
		}

		// Token: 0x06001072 RID: 4210 RVA: 0x00052542 File Offset: 0x00050742
		public void SetVirtualMousePositionZ(float f)
		{
			this.virtualMousePosition = new Vector3(this.virtualMousePosition.x, this.virtualMousePosition.y, f);
		}

		// Token: 0x06001073 RID: 4211
		public abstract float GetAxis(string name, bool raw);

		// Token: 0x06001074 RID: 4212
		public abstract bool GetButton(string name);

		// Token: 0x06001075 RID: 4213
		public abstract bool GetButtonDown(string name);

		// Token: 0x06001076 RID: 4214
		public abstract bool GetButtonUp(string name);

		// Token: 0x06001077 RID: 4215
		public abstract void SetButtonDown(string name);

		// Token: 0x06001078 RID: 4216
		public abstract void SetButtonUp(string name);

		// Token: 0x06001079 RID: 4217
		public abstract void SetAxisPositive(string name);

		// Token: 0x0600107A RID: 4218
		public abstract void SetAxisNegative(string name);

		// Token: 0x0600107B RID: 4219
		public abstract void SetAxisZero(string name);

		// Token: 0x0600107C RID: 4220
		public abstract void SetAxis(string name, float value);

		// Token: 0x0600107D RID: 4221
		public abstract Vector3 MousePosition();

		// Token: 0x04000F47 RID: 3911
		protected Dictionary<string, CrossPlatformInputManager.VirtualAxis> m_VirtualAxes = new Dictionary<string, CrossPlatformInputManager.VirtualAxis>();

		// Token: 0x04000F48 RID: 3912
		protected Dictionary<string, CrossPlatformInputManager.VirtualButton> m_VirtualButtons = new Dictionary<string, CrossPlatformInputManager.VirtualButton>();

		// Token: 0x04000F49 RID: 3913
		protected List<string> m_AlwaysUseVirtual = new List<string>();
	}
}
