using System;
using System.Collections;
using TMPro;
using UnityEngine;

// Token: 0x020001F1 RID: 497
public class LoadingScreenAnimationSimple : MonoBehaviour
{
	// Token: 0x06000D03 RID: 3331 RVA: 0x000412AF File Offset: 0x0003F4AF
	private void Start()
	{
		base.StartCoroutine(this.AnimateRoutine());
	}

	// Token: 0x06000D04 RID: 3332 RVA: 0x000412BE File Offset: 0x0003F4BE
	private IEnumerator AnimateRoutine()
	{
		float dots = 0f;
		for (;;)
		{
			yield return new WaitForSeconds(this.yieldTime);
			if (dots == 0f)
			{
				this.loading.text = "LOADING";
			}
			else if (dots == 1f)
			{
				this.loading.text = "LOADING.";
			}
			else if (dots == 2f)
			{
				this.loading.text = "LOADING..";
			}
			else if (dots == 3f)
			{
				this.loading.text = "LOADING...";
			}
			float num = dots;
			dots = num + 1f;
			if (dots > 3f)
			{
				dots = 0f;
			}
		}
		yield break;
	}

	// Token: 0x04000C02 RID: 3074
	public float yieldTime = 1f;

	// Token: 0x04000C03 RID: 3075
	public TMP_Text loading;
}
