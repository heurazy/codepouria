using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020001E3 RID: 483
public class JustEvent : MonoBehaviour
{
	// Token: 0x06000CC5 RID: 3269 RVA: 0x0003FC3B File Offset: 0x0003DE3B
	private void CallEvent1()
	{
		this.event1.Invoke();
	}

	// Token: 0x04000BC3 RID: 3011
	public UnityEvent event1;
}
