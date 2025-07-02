using System;
using UnityEngine;

// Token: 0x020001C0 RID: 448
public class EnabledRandom : MonoBehaviour
{
	// Token: 0x06000C26 RID: 3110 RVA: 0x0003CB07 File Offset: 0x0003AD07
	private void Start()
	{
		this.odds = Random.Range(0, 4);
		if (this.odds < 2)
		{
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x04000B22 RID: 2850
	public int odds = 1;
}
