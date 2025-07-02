using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UnityStandardAssets.CrossPlatformInput
{
	// Token: 0x020002AB RID: 683
	public class Joystick : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler, IDragHandler
	{
		// Token: 0x06001050 RID: 4176 RVA: 0x00051DE2 File Offset: 0x0004FFE2
		private void OnEnable()
		{
			this.CreateVirtualAxes();
		}

		// Token: 0x06001051 RID: 4177 RVA: 0x00051DEA File Offset: 0x0004FFEA
		private void Start()
		{
			this.m_StartPos = base.transform.position;
		}

		// Token: 0x06001052 RID: 4178 RVA: 0x00051E00 File Offset: 0x00050000
		private void UpdateVirtualAxes(Vector3 value)
		{
			Vector3 vector = this.m_StartPos - value;
			vector.y = -vector.y;
			vector /= (float)this.MovementRange;
			if (this.m_UseX)
			{
				this.m_HorizontalVirtualAxis.Update(-vector.x);
			}
			if (this.m_UseY)
			{
				this.m_VerticalVirtualAxis.Update(vector.y);
			}
		}

		// Token: 0x06001053 RID: 4179 RVA: 0x00051E6C File Offset: 0x0005006C
		private void CreateVirtualAxes()
		{
			this.m_UseX = this.axesToUse == Joystick.AxisOption.Both || this.axesToUse == Joystick.AxisOption.OnlyHorizontal;
			this.m_UseY = this.axesToUse == Joystick.AxisOption.Both || this.axesToUse == Joystick.AxisOption.OnlyVertical;
			if (this.m_UseX)
			{
				this.m_HorizontalVirtualAxis = new CrossPlatformInputManager.VirtualAxis(this.horizontalAxisName);
				CrossPlatformInputManager.RegisterVirtualAxis(this.m_HorizontalVirtualAxis);
			}
			if (this.m_UseY)
			{
				this.m_VerticalVirtualAxis = new CrossPlatformInputManager.VirtualAxis(this.verticalAxisName);
				CrossPlatformInputManager.RegisterVirtualAxis(this.m_VerticalVirtualAxis);
			}
		}

		// Token: 0x06001054 RID: 4180 RVA: 0x00051EF8 File Offset: 0x000500F8
		public void OnDrag(PointerEventData data)
		{
			Vector3 zero = Vector3.zero;
			if (this.m_UseX)
			{
				int num = (int)(data.position.x - this.m_StartPos.x);
				num = Mathf.Clamp(num, -this.MovementRange, this.MovementRange);
				zero.x = (float)num;
			}
			if (this.m_UseY)
			{
				int num2 = (int)(data.position.y - this.m_StartPos.y);
				num2 = Mathf.Clamp(num2, -this.MovementRange, this.MovementRange);
				zero.y = (float)num2;
			}
			base.transform.position = new Vector3(this.m_StartPos.x + zero.x, this.m_StartPos.y + zero.y, this.m_StartPos.z + zero.z);
			this.UpdateVirtualAxes(base.transform.position);
		}

		// Token: 0x06001055 RID: 4181 RVA: 0x00051FDE File Offset: 0x000501DE
		public void OnPointerUp(PointerEventData data)
		{
			base.transform.position = this.m_StartPos;
			this.UpdateVirtualAxes(this.m_StartPos);
		}

		// Token: 0x06001056 RID: 4182 RVA: 0x00051FFD File Offset: 0x000501FD
		public void OnPointerDown(PointerEventData data)
		{
		}

		// Token: 0x06001057 RID: 4183 RVA: 0x00051FFF File Offset: 0x000501FF
		private void OnDisable()
		{
			if (this.m_UseX)
			{
				this.m_HorizontalVirtualAxis.Remove();
			}
			if (this.m_UseY)
			{
				this.m_VerticalVirtualAxis.Remove();
			}
		}

		// Token: 0x04000F2B RID: 3883
		public int MovementRange = 100;

		// Token: 0x04000F2C RID: 3884
		public Joystick.AxisOption axesToUse;

		// Token: 0x04000F2D RID: 3885
		public string horizontalAxisName = "Horizontal";

		// Token: 0x04000F2E RID: 3886
		public string verticalAxisName = "Vertical";

		// Token: 0x04000F2F RID: 3887
		private Vector3 m_StartPos;

		// Token: 0x04000F30 RID: 3888
		private bool m_UseX;

		// Token: 0x04000F31 RID: 3889
		private bool m_UseY;

		// Token: 0x04000F32 RID: 3890
		private CrossPlatformInputManager.VirtualAxis m_HorizontalVirtualAxis;

		// Token: 0x04000F33 RID: 3891
		private CrossPlatformInputManager.VirtualAxis m_VerticalVirtualAxis;

		// Token: 0x020003C6 RID: 966
		public enum AxisOption
		{
			// Token: 0x040013F0 RID: 5104
			Both,
			// Token: 0x040013F1 RID: 5105
			OnlyHorizontal,
			// Token: 0x040013F2 RID: 5106
			OnlyVertical
		}
	}
}
