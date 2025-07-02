using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000296 RID: 662
public class UpdateEvent : MonoBehaviour
{
	// Token: 0x06000FC5 RID: 4037 RVA: 0x0004FE81 File Offset: 0x0004E081
	private void Update()
	{
		this.updateEvent.Invoke();
	}

	// Token: 0x04000ED3 RID: 3795
	public UnityEvent updateEvent;
}
