using System;
using UnityEngine;

// Token: 0x020001C2 RID: 450
public class EndCutsceneScoutHelper : MonoBehaviour
{
	// Token: 0x06000C2A RID: 3114 RVA: 0x0003CB58 File Offset: 0x0003AD58
	private void OnEnable()
	{
		base.GetComponent<Animator>().SetBool("Alone", this.alone);
	}

	// Token: 0x04000B25 RID: 2853
	public bool alone;
}
