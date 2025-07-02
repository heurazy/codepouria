using System;
using System.Collections;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using TMPro;
using UnityEngine;

// Token: 0x02000174 RID: 372
public class TextFogEffect : MonoBehaviour
{
	// Token: 0x1700009C RID: 156
	// (get) Token: 0x06000A79 RID: 2681 RVA: 0x0003344C File Offset: 0x0003164C
	public virtual float colorSpeedMult
	{
		get
		{
			return 1f;
		}
	}

	// Token: 0x06000A7A RID: 2682 RVA: 0x00033453 File Offset: 0x00031653
	private void Awake()
	{
		this.m_TextComponent = base.GetComponent<TMP_Text>();
		this.DTanimator = new DOTweenTMPAnimator(this.m_TextComponent);
	}

	// Token: 0x06000A7B RID: 2683 RVA: 0x00033472 File Offset: 0x00031672
	private void Start()
	{
		this.Init();
	}

	// Token: 0x06000A7C RID: 2684 RVA: 0x0003347A File Offset: 0x0003167A
	private void OnEnable()
	{
		base.StartCoroutine(this.TextEffectRoutine());
	}

	// Token: 0x06000A7D RID: 2685 RVA: 0x00033489 File Offset: 0x00031689
	private IEnumerator TextEffectRoutine()
	{
		this.textInfo = this.m_TextComponent.textInfo;
		int characterCount = this.textInfo.characterCount;
		for (;;)
		{
			this.UpdateCharacter(Random.Range(0, characterCount));
			yield return new WaitForSeconds(this.period);
		}
		yield break;
	}

	// Token: 0x06000A7E RID: 2686 RVA: 0x00033498 File Offset: 0x00031698
	public virtual void Init()
	{
		this.textInfo = this.m_TextComponent.textInfo;
		int characterCount = this.textInfo.characterCount;
	}

	// Token: 0x06000A7F RID: 2687 RVA: 0x000334B7 File Offset: 0x000316B7
	private void TryDestroy()
	{
		this.destroyed = true;
		Object.Destroy(this);
	}

	// Token: 0x06000A80 RID: 2688 RVA: 0x000334C6 File Offset: 0x000316C6
	private void LateUpdate()
	{
		bool flag = this.destroyed;
	}

	// Token: 0x06000A81 RID: 2689 RVA: 0x000334CF File Offset: 0x000316CF
	protected virtual void EffectRoutine()
	{
		this.textInfo = this.m_TextComponent.textInfo;
		int characterCount = this.textInfo.characterCount;
	}

	// Token: 0x06000A82 RID: 2690 RVA: 0x000334F0 File Offset: 0x000316F0
	public void UpdateCharacter(int index)
	{
		if (this.period == 0f)
		{
			return;
		}
		float num = this.offset * (float)index;
		float num2 = Mathf.Sin((Time.time + num) / this.period);
		float num3 = 1f + num2 * this.amplitude;
		if (this.roundSin)
		{
			num3 = Mathf.Round(num3 * this.chunkiness) / this.chunkiness;
		}
		num3 = this.amplitude;
		this.DTanimator.DOOffsetChar(index, Random.insideUnitSphere * num3, this.shiftTime).SetEase(Ease.InOutCubic);
		float num4 = (Mathf.Sin((Time.time + num) / (this.period / this.colorSpeedMult)) + 1f) * 0.5f;
		this.DTanimator.SetCharColor(index, this.colorGradient.Evaluate(num4));
	}

	// Token: 0x04000954 RID: 2388
	public bool abs;

	// Token: 0x04000955 RID: 2389
	public float amplitude = 0.2f;

	// Token: 0x04000956 RID: 2390
	public float period = 0.5f;

	// Token: 0x04000957 RID: 2391
	public float offset = 0.1f;

	// Token: 0x04000958 RID: 2392
	public Gradient colorGradient;

	// Token: 0x04000959 RID: 2393
	public bool skewXtop = true;

	// Token: 0x0400095A RID: 2394
	public float skewX;

	// Token: 0x0400095B RID: 2395
	public bool skewYtop = true;

	// Token: 0x0400095C RID: 2396
	public float skewY;

	// Token: 0x0400095D RID: 2397
	public bool roundSin;

	// Token: 0x0400095E RID: 2398
	public float chunkiness = 1f;

	// Token: 0x0400095F RID: 2399
	public float updateChance = 0.1f;

	// Token: 0x04000960 RID: 2400
	public float shiftTime = 0.5f;

	// Token: 0x04000961 RID: 2401
	protected TMP_Text m_TextComponent;

	// Token: 0x04000962 RID: 2402
	protected TMP_TextInfo textInfo;

	// Token: 0x04000963 RID: 2403
	public DOTweenTMPAnimator DTanimator;

	// Token: 0x04000964 RID: 2404
	private bool destroyed;
}
