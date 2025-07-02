using System;
using UnityEngine;

// Token: 0x02000074 RID: 116
public class DisableIfEditor : MonoBehaviour
{
	// Token: 0x06000421 RID: 1057 RVA: 0x00017D4B File Offset: 0x00015F4B
	private void Start()
	{
		if (Application.isEditor)
		{
			base.gameObject.SetActive(false);
		}
	}
}
