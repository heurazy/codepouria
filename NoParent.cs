using System;
using UnityEngine;

// Token: 0x02000208 RID: 520
public class NoParent : MonoBehaviour
{
	// Token: 0x06000D6C RID: 3436 RVA: 0x00043DBF File Offset: 0x00041FBF
	private void Start()
	{
		base.transform.parent = null;
	}
}
