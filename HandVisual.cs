using System;
using UnityEngine;

// Token: 0x020001D6 RID: 470
[DefaultExecutionOrder(-9999)]
[SelectionBase]
public class HandVisual : MonoBehaviour
{
	// Token: 0x06000C84 RID: 3204 RVA: 0x0003E2FF File Offset: 0x0003C4FF
	private void Awake()
	{
		base.transform.GetChild(0).gameObject.SetActive(false);
	}
}
