using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020001F2 RID: 498
public class LocalPlayerEvent : MonoBehaviour
{
	// Token: 0x06000D06 RID: 3334 RVA: 0x000412E0 File Offset: 0x0003F4E0
	public void Start()
	{
		if (base.GetComponentInParent<Character>().IsLocal)
		{
			this.isLocalEvent.Invoke();
		}
	}

	// Token: 0x04000C04 RID: 3076
	public UnityEvent isLocalEvent;
}
