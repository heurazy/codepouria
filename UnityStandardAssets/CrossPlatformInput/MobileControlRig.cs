using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UnityStandardAssets.CrossPlatformInput
{
	// Token: 0x020002AC RID: 684
	[ExecuteInEditMode]
	public class MobileControlRig : MonoBehaviour
	{
		// Token: 0x06001059 RID: 4185 RVA: 0x0005204D File Offset: 0x0005024D
		private void OnEnable()
		{
			this.CheckEnableControlRig();
		}

		// Token: 0x0600105A RID: 4186 RVA: 0x00052055 File Offset: 0x00050255
		private void Start()
		{
			if (Object.FindObjectOfType<EventSystem>() == null)
			{
				GameObject gameObject = new GameObject("EventSystem");
				gameObject.AddComponent<EventSystem>();
				gameObject.AddComponent<StandaloneInputModule>();
			}
		}

		// Token: 0x0600105B RID: 4187 RVA: 0x0005207B File Offset: 0x0005027B
		private void CheckEnableControlRig()
		{
			this.EnableControlRig(false);
		}

		// Token: 0x0600105C RID: 4188 RVA: 0x00052084 File Offset: 0x00050284
		private void EnableControlRig(bool enabled)
		{
			try
			{
				foreach (object obj in base.transform)
				{
					((Transform)obj).gameObject.SetActive(enabled);
				}
			}
			catch (Exception)
			{
			}
		}
	}
}
