using System;
using UnityEngine;

// Token: 0x0200003E RID: 62
public class AscentDebug : MonoBehaviour
{
	// Token: 0x06000300 RID: 768 RVA: 0x000134FE File Offset: 0x000116FE
	private void Awake()
	{
		Ascents.currentAscent = this.testAscent;
	}

	// Token: 0x040003B0 RID: 944
	public int testAscent;
}
