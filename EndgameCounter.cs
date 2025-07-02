using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using TMPro;
using UnityEngine;

// Token: 0x020001C3 RID: 451
public class EndgameCounter : MonoBehaviour
{
	// Token: 0x06000C2C RID: 3116 RVA: 0x0003CB78 File Offset: 0x0003AD78
	public void UpdateCounter(int value)
	{
		this.counterGroup.gameObject.SetActive(true);
		this.counterGroup.alpha = 0f;
		this.counterGroup.DOFade(1f, 0.25f);
		this.counter.text = value.ToString() ?? "";
		this.counter.transform.localScale = Vector3.one * 2f;
		this.counter.alpha = 0f;
		this.counter.DOScale(1f, 0.25f).SetEase(Ease.OutCubic);
		this.counter.DOFade(1f, 0.25f).SetEase(Ease.OutCubic);
	}

	// Token: 0x06000C2D RID: 3117 RVA: 0x0003CC40 File Offset: 0x0003AE40
	public void Win()
	{
		this.winGroup.gameObject.SetActive(true);
		this.winGroup.alpha = 0f;
		this.winGroup.DOFade(1f, 1f);
	}

	// Token: 0x06000C2E RID: 3118 RVA: 0x0003CC79 File Offset: 0x0003AE79
	public void Lose()
	{
		this.loseGroup.gameObject.SetActive(true);
		this.loseGroup.alpha = 0f;
		this.loseGroup.DOFade(1f, 1f);
	}

	// Token: 0x06000C2F RID: 3119 RVA: 0x0003CCB2 File Offset: 0x0003AEB2
	public void Disable()
	{
		this.counterGroup.gameObject.SetActive(false);
	}

	// Token: 0x04000B26 RID: 2854
	public CanvasGroup counterGroup;

	// Token: 0x04000B27 RID: 2855
	public CanvasGroup winGroup;

	// Token: 0x04000B28 RID: 2856
	public CanvasGroup loseGroup;

	// Token: 0x04000B29 RID: 2857
	public TextMeshProUGUI counter;
}
