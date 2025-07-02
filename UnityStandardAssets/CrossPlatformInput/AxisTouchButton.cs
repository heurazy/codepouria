using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UnityStandardAssets.CrossPlatformInput
{
	// Token: 0x020002A7 RID: 679
	public class AxisTouchButton : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler
	{
		// Token: 0x06001026 RID: 4134 RVA: 0x00051A94 File Offset: 0x0004FC94
		private void OnEnable()
		{
			if (!CrossPlatformInputManager.AxisExists(this.axisName))
			{
				this.m_Axis = new CrossPlatformInputManager.VirtualAxis(this.axisName);
				CrossPlatformInputManager.RegisterVirtualAxis(this.m_Axis);
			}
			else
			{
				this.m_Axis = CrossPlatformInputManager.VirtualAxisReference(this.axisName);
			}
			this.FindPairedButton();
		}

		// Token: 0x06001027 RID: 4135 RVA: 0x00051AE4 File Offset: 0x0004FCE4
		private void FindPairedButton()
		{
			AxisTouchButton[] array = Object.FindObjectsOfType(typeof(AxisTouchButton)) as AxisTouchButton[];
			if (array != null)
			{
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i].axisName == this.axisName && array[i] != this)
					{
						this.m_PairedWith = array[i];
					}
				}
			}
		}

		// Token: 0x06001028 RID: 4136 RVA: 0x00051B40 File Offset: 0x0004FD40
		private void OnDisable()
		{
			this.m_Axis.Remove();
		}

		// Token: 0x06001029 RID: 4137 RVA: 0x00051B50 File Offset: 0x0004FD50
		public void OnPointerDown(PointerEventData data)
		{
			if (this.m_PairedWith == null)
			{
				this.FindPairedButton();
			}
			this.m_Axis.Update(Mathf.MoveTowards(this.m_Axis.GetValue, this.axisValue, this.responseSpeed * Time.deltaTime));
		}

		// Token: 0x0600102A RID: 4138 RVA: 0x00051B9E File Offset: 0x0004FD9E
		public void OnPointerUp(PointerEventData data)
		{
			this.m_Axis.Update(Mathf.MoveTowards(this.m_Axis.GetValue, 0f, this.responseSpeed * Time.deltaTime));
		}

		// Token: 0x04000F20 RID: 3872
		public string axisName = "Horizontal";

		// Token: 0x04000F21 RID: 3873
		public float axisValue = 1f;

		// Token: 0x04000F22 RID: 3874
		public float responseSpeed = 3f;

		// Token: 0x04000F23 RID: 3875
		public float returnToCentreSpeed = 3f;

		// Token: 0x04000F24 RID: 3876
		private AxisTouchButton m_PairedWith;

		// Token: 0x04000F25 RID: 3877
		private CrossPlatformInputManager.VirtualAxis m_Axis;
	}
}
