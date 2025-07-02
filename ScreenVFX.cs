using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

// Token: 0x02000262 RID: 610
public class ScreenVFX : MonoBehaviour
{
	// Token: 0x06000ECB RID: 3787 RVA: 0x0004AA55 File Offset: 0x00048C55
	public void Test()
	{
		this.Play(1f);
	}

	// Token: 0x06000ECC RID: 3788 RVA: 0x0004AA64 File Offset: 0x00048C64
	public void Play(float amount)
	{
		base.gameObject.SetActive(true);
		this.renderer.material.SetFloat(ScreenVFX.INTENSITY, 1f);
		this.renderer.material.DOFloat(0f, ScreenVFX.INTENSITY, this.duration).SetDelay(this.delay).OnComplete(new TweenCallback(this.Disable));
	}

	// Token: 0x06000ECD RID: 3789 RVA: 0x0004AAD4 File Offset: 0x00048CD4
	public void StartFX()
	{
		base.gameObject.SetActive(true);
		this.renderer.material.SetFloat(ScreenVFX.INTENSITY, 0f);
		this.renderer.material.DOFloat(1f, ScreenVFX.INTENSITY, this.duration);
	}

	// Token: 0x06000ECE RID: 3790 RVA: 0x0004AB28 File Offset: 0x00048D28
	public void EndFX()
	{
		this.renderer.material.DOFloat(0f, ScreenVFX.INTENSITY, this.duration).OnComplete(new TweenCallback(this.Disable));
	}

	// Token: 0x06000ECF RID: 3791 RVA: 0x0004AB5C File Offset: 0x00048D5C
	private void Disable()
	{
		base.gameObject.SetActive(false);
	}

	// Token: 0x04000DAD RID: 3501
	private static readonly int INTENSITY = Shader.PropertyToID("_Intensity");

	// Token: 0x04000DAE RID: 3502
	public Renderer renderer;

	// Token: 0x04000DAF RID: 3503
	public float duration = 0.5f;

	// Token: 0x04000DB0 RID: 3504
	public float delay = 0.25f;
}
