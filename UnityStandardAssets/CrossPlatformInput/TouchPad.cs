using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UnityStandardAssets.CrossPlatformInput
{
	// Token: 0x020002AD RID: 685
	[RequireComponent(typeof(Image))]
	public class TouchPad : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler
	{
		// Token: 0x0600105E RID: 4190 RVA: 0x000520FC File Offset: 0x000502FC
		private void OnEnable()
		{
			this.CreateVirtualAxes();
		}

		// Token: 0x0600105F RID: 4191 RVA: 0x00052104 File Offset: 0x00050304
		private void Start()
		{
			this.m_Image = base.GetComponent<Image>();
			this.m_Center = this.m_Image.transform.position;
		}

		// Token: 0x06001060 RID: 4192 RVA: 0x00052128 File Offset: 0x00050328
		private void CreateVirtualAxes()
		{
			this.m_UseX = this.axesToUse == TouchPad.AxisOption.Both || this.axesToUse == TouchPad.AxisOption.OnlyHorizontal;
			this.m_UseY = this.axesToUse == TouchPad.AxisOption.Both || this.axesToUse == TouchPad.AxisOption.OnlyVertical;
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

		// Token: 0x06001061 RID: 4193 RVA: 0x000521B1 File Offset: 0x000503B1
		private void UpdateVirtualAxes(Vector3 value)
		{
			value = value.normalized;
			if (this.m_UseX)
			{
				this.m_HorizontalVirtualAxis.Update(value.x);
			}
			if (this.m_UseY)
			{
				this.m_VerticalVirtualAxis.Update(value.y);
			}
		}

		// Token: 0x06001062 RID: 4194 RVA: 0x000521EE File Offset: 0x000503EE
		public void OnPointerDown(PointerEventData data)
		{
			this.m_Dragging = true;
			this.m_Id = data.pointerId;
			if (this.controlStyle != TouchPad.ControlStyle.Absolute)
			{
				this.m_Center = data.position;
			}
		}

		// Token: 0x06001063 RID: 4195 RVA: 0x0005221C File Offset: 0x0005041C
		private void Update()
		{
			if (!this.m_Dragging)
			{
				return;
			}
			if (Input.touchCount >= this.m_Id + 1 && this.m_Id != -1)
			{
				if (this.controlStyle == TouchPad.ControlStyle.Swipe)
				{
					this.m_Center = this.m_PreviousTouchPos;
					this.m_PreviousTouchPos = Input.touches[this.m_Id].position;
				}
				Vector2 normalized = new Vector2(Input.touches[this.m_Id].position.x - this.m_Center.x, Input.touches[this.m_Id].position.y - this.m_Center.y).normalized;
				normalized.x *= this.Xsensitivity;
				normalized.y *= this.Ysensitivity;
				this.UpdateVirtualAxes(new Vector3(normalized.x, normalized.y, 0f));
			}
		}

		// Token: 0x06001064 RID: 4196 RVA: 0x0005231D File Offset: 0x0005051D
		public void OnPointerUp(PointerEventData data)
		{
			this.m_Dragging = false;
			this.m_Id = -1;
			this.UpdateVirtualAxes(Vector3.zero);
		}

		// Token: 0x06001065 RID: 4197 RVA: 0x00052338 File Offset: 0x00050538
		private void OnDisable()
		{
			if (CrossPlatformInputManager.AxisExists(this.horizontalAxisName))
			{
				CrossPlatformInputManager.UnRegisterVirtualAxis(this.horizontalAxisName);
			}
			if (CrossPlatformInputManager.AxisExists(this.verticalAxisName))
			{
				CrossPlatformInputManager.UnRegisterVirtualAxis(this.verticalAxisName);
			}
		}

		// Token: 0x04000F34 RID: 3892
		public TouchPad.AxisOption axesToUse;

		// Token: 0x04000F35 RID: 3893
		public TouchPad.ControlStyle controlStyle;

		// Token: 0x04000F36 RID: 3894
		public string horizontalAxisName = "Horizontal";

		// Token: 0x04000F37 RID: 3895
		public string verticalAxisName = "Vertical";

		// Token: 0x04000F38 RID: 3896
		public float Xsensitivity = 1f;

		// Token: 0x04000F39 RID: 3897
		public float Ysensitivity = 1f;

		// Token: 0x04000F3A RID: 3898
		private Vector3 m_StartPos;

		// Token: 0x04000F3B RID: 3899
		private Vector2 m_PreviousDelta;

		// Token: 0x04000F3C RID: 3900
		private Vector3 m_JoytickOutput;

		// Token: 0x04000F3D RID: 3901
		private bool m_UseX;

		// Token: 0x04000F3E RID: 3902
		private bool m_UseY;

		// Token: 0x04000F3F RID: 3903
		private CrossPlatformInputManager.VirtualAxis m_HorizontalVirtualAxis;

		// Token: 0x04000F40 RID: 3904
		private CrossPlatformInputManager.VirtualAxis m_VerticalVirtualAxis;

		// Token: 0x04000F41 RID: 3905
		private bool m_Dragging;

		// Token: 0x04000F42 RID: 3906
		private int m_Id = -1;

		// Token: 0x04000F43 RID: 3907
		private Vector2 m_PreviousTouchPos;

		// Token: 0x04000F44 RID: 3908
		private Vector3 m_Center;

		// Token: 0x04000F45 RID: 3909
		private Image m_Image;

		// Token: 0x020003C7 RID: 967
		public enum AxisOption
		{
			// Token: 0x040013F4 RID: 5108
			Both,
			// Token: 0x040013F5 RID: 5109
			OnlyHorizontal,
			// Token: 0x040013F6 RID: 5110
			OnlyVertical
		}

		// Token: 0x020003C8 RID: 968
		public enum ControlStyle
		{
			// Token: 0x040013F8 RID: 5112
			Absolute,
			// Token: 0x040013F9 RID: 5113
			Relative,
			// Token: 0x040013FA RID: 5114
			Swipe
		}
	}
}
