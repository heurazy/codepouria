using System;
using UnityEngine;
using UnityEngine.UI;

namespace ExitGames.Demos.DemoPunVoice
{
	// Token: 0x020002B2 RID: 690
	[RequireComponent(typeof(Toggle))]
	[DisallowMultipleComponent]
	public class BetterToggle : MonoBehaviour
	{
		// Token: 0x14000003 RID: 3
		// (add) Token: 0x060010A5 RID: 4261 RVA: 0x000529B0 File Offset: 0x00050BB0
		// (remove) Token: 0x060010A6 RID: 4262 RVA: 0x000529E4 File Offset: 0x00050BE4
		public static event BetterToggle.OnToggle ToggleValueChanged;

		// Token: 0x060010A7 RID: 4263 RVA: 0x00052A17 File Offset: 0x00050C17
		private void Start()
		{
			this.toggle = base.GetComponent<Toggle>();
			this.toggle.onValueChanged.AddListener(delegate
			{
				this.OnToggleValueChanged();
			});
		}

		// Token: 0x060010A8 RID: 4264 RVA: 0x00052A41 File Offset: 0x00050C41
		public void OnToggleValueChanged()
		{
			if (BetterToggle.ToggleValueChanged != null)
			{
				BetterToggle.ToggleValueChanged(this.toggle);
			}
		}

		// Token: 0x04000F52 RID: 3922
		private Toggle toggle;

		// Token: 0x020003C9 RID: 969
		// (Invoke) Token: 0x0600150E RID: 5390
		public delegate void OnToggle(Toggle toggle);
	}
}
