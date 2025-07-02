using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace Photon.Chat.Demo
{
	// Token: 0x020002C7 RID: 711
	[ExecuteInEditMode]
	public class ChatAppIdCheckerUI : MonoBehaviour
	{
		// Token: 0x0600118F RID: 4495 RVA: 0x0005671C File Offset: 0x0005491C
		public void Update()
		{
			string text = string.Empty;
			if (string.IsNullOrEmpty(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat))
			{
				text = "<Color=Red>WARNING:</Color>\nPlease setup a Chat AppId in the PhotonServerSettings file.";
			}
			this.Description.text = text;
		}

		// Token: 0x04000FFC RID: 4092
		public Text Description;

		// Token: 0x04000FFD RID: 4093
		public bool WizardOpenedOnce;
	}
}
