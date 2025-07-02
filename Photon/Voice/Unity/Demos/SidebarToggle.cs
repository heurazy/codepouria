using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Photon.Voice.Unity.Demos
{
	// Token: 0x020002BB RID: 699
	public class SidebarToggle : MonoBehaviour
	{
		// Token: 0x060010F2 RID: 4338 RVA: 0x00054292 File Offset: 0x00052492
		private void Awake()
		{
			this.sidebarButton.onClick.RemoveAllListeners();
			this.sidebarButton.onClick.AddListener(new UnityAction(this.ToggleSidebar));
			this.ToggleSidebar(this.sidebarOpen);
		}

		// Token: 0x060010F3 RID: 4339 RVA: 0x000542CC File Offset: 0x000524CC
		[ContextMenu("ToggleSidebar")]
		private void ToggleSidebar()
		{
			this.sidebarOpen = !this.sidebarOpen;
			this.ToggleSidebar(this.sidebarOpen);
		}

		// Token: 0x060010F4 RID: 4340 RVA: 0x000542E9 File Offset: 0x000524E9
		private void ToggleSidebar(bool open)
		{
			if (!open)
			{
				this.panelsHolder.SetPosX(0f);
				return;
			}
			this.panelsHolder.SetPosX(this.sidebarWidth);
		}

		// Token: 0x04000F92 RID: 3986
		[SerializeField]
		private Button sidebarButton;

		// Token: 0x04000F93 RID: 3987
		[SerializeField]
		private RectTransform panelsHolder;

		// Token: 0x04000F94 RID: 3988
		private float sidebarWidth = 300f;

		// Token: 0x04000F95 RID: 3989
		private bool sidebarOpen = true;
	}
}
