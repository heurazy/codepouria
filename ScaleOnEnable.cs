using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

// Token: 0x0200025F RID: 607
public class ScaleOnEnable : MonoBehaviour
{
	// Token: 0x06000E9E RID: 3742 RVA: 0x00049604 File Offset: 0x00047804
	private void OnEnable()
	{
		base.transform.localScale = Vector3.zero;
		base.transform.DOScale(Vector3.one, this.time).SetEase(this.easeType);
		if (this.canvasGroup)
		{
			this.canvasGroup.alpha = 0f;
			this.canvasGroup.DOFade(1f, this.time).SetEase(this.easeType);
		}
	}

	// Token: 0x04000D94 RID: 3476
	public float time = 0.25f;

	// Token: 0x04000D95 RID: 3477
	public Ease easeType = Ease.OutBounce;

	// Token: 0x04000D96 RID: 3478
	public CanvasGroup canvasGroup;
}
