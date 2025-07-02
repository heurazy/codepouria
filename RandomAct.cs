using System;
using UnityEngine;

// Token: 0x02000251 RID: 593
public class RandomAct : MonoBehaviour
{
	// Token: 0x06000E72 RID: 3698 RVA: 0x00048AAF File Offset: 0x00046CAF
	private void Start()
	{
		base.GetComponent<Animator>().SetInteger("Act", this.act);
	}

	// Token: 0x04000D71 RID: 3441
	public int act;
}
