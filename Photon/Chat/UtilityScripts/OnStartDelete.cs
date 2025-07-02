using System;
using UnityEngine;

namespace Photon.Chat.UtilityScripts
{
	// Token: 0x020002CD RID: 717
	public class OnStartDelete : MonoBehaviour
	{
		// Token: 0x060011BF RID: 4543 RVA: 0x00057594 File Offset: 0x00055794
		private void Start()
		{
			Object.Destroy(base.gameObject);
		}
	}
}
