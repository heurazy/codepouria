using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001C5 RID: 453
public class FadeIn : MonoBehaviour
{
	// Token: 0x06000C35 RID: 3125 RVA: 0x0003CE00 File Offset: 0x0003B000
	private void Awake()
	{
		Color color = this.fade.color;
		color.a = 1f;
		this.fade.color = color;
		this.fade.DOFade(0f, 2f).OnComplete(new TweenCallback(this.Disable));
	}

	// Token: 0x06000C36 RID: 3126 RVA: 0x0003CE58 File Offset: 0x0003B058
	private void Disable()
	{
		base.gameObject.SetActive(false);
	}

	// Token: 0x04000B2F RID: 2863
	public Image fade;
}
