using System;
using UnityEngine;

namespace Photon.Chat.Demo
{
	// Token: 0x020002CA RID: 714
	public class IgnoreUiRaycastWhenInactive : MonoBehaviour, ICanvasRaycastFilter
	{
		// Token: 0x060011B7 RID: 4535 RVA: 0x000574AF File Offset: 0x000556AF
		public bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
		{
			return base.gameObject.activeInHierarchy;
		}
	}
}
