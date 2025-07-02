using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Photon.Chat.UtilityScripts
{
	// Token: 0x020002CC RID: 716
	public class EventSystemSpawner : MonoBehaviour
	{
		// Token: 0x060011BD RID: 4541 RVA: 0x00057566 File Offset: 0x00055766
		private void OnEnable()
		{
			if (Object.FindFirstObjectByType<EventSystem>() == null)
			{
				GameObject gameObject = new GameObject("EventSystem");
				gameObject.AddComponent<EventSystem>();
				gameObject.AddComponent<StandaloneInputModule>();
			}
		}
	}
}
