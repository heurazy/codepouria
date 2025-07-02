using System;
using System.Collections;
using System.Runtime.CompilerServices;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000173 RID: 371
public class StaminaBar : MonoBehaviour
{
	// Token: 0x06000A6E RID: 2670 RVA: 0x00032C5C File Offset: 0x00030E5C
	private void Start()
	{
		this.afflictions = base.GetComponentsInChildren<BarAffliction>();
		this.TAU = 6.2831855f;
		BarAffliction[] array = this.afflictions;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].gameObject.SetActive(false);
		}
	}

	// Token: 0x06000A6F RID: 2671 RVA: 0x00032CA4 File Offset: 0x00030EA4
	public void ChangeBar()
	{
		for (int i = 0; i < this.afflictions.Length; i++)
		{
			this.afflictions[i].ChangeAffliction(this);
		}
	}

	// Token: 0x06000A70 RID: 2672 RVA: 0x00032CD4 File Offset: 0x00030ED4
	private void Update()
	{
		if (Character.observedCharacter == null)
		{
			return;
		}
		for (int i = 0; i < this.afflictions.Length; i++)
		{
			this.afflictions[i].UpdateAffliction(this);
		}
		this.desiredStaminaSize = Mathf.Max(0f, Character.observedCharacter.data.currentStamina * this.fullBar.sizeDelta.x + this.staminaBarOffset);
		if (Character.observedCharacter.data.currentStamina <= 0.005f)
		{
			if (!this.outOfStamina)
			{
				this.outOfStamina = true;
				this.OutOfStaminaPulse();
			}
		}
		else
		{
			this.outOfStamina = false;
		}
		this.staminaBar.sizeDelta = new Vector2(Mathf.Lerp(this.staminaBar.sizeDelta.x, this.desiredStaminaSize, Time.deltaTime * 10f), this.staminaBar.sizeDelta.y);
		Color color = this.staminaGlow.color;
		float num = Mathf.Clamp01((this.staminaBar.sizeDelta.x - this.desiredStaminaSize) * 0.5f);
		this.sinTime += Time.deltaTime * 10f * num;
		color.a = num * 0.4f - Mathf.Abs(Mathf.Sin(this.sinTime)) * 0.2f;
		this.staminaGlow.color = color;
		this.desiredMaxStaminaSize = Mathf.Max(0f, Character.observedCharacter.GetMaxStamina() * this.fullBar.sizeDelta.x + this.staminaBarOffset);
		this.maxStaminaBar.sizeDelta = new Vector2(Mathf.Lerp(this.maxStaminaBar.sizeDelta.x, this.desiredMaxStaminaSize, Time.deltaTime * 10f), this.maxStaminaBar.sizeDelta.y);
		float statusSum = Character.observedCharacter.refs.afflictions.statusSum;
		this.staminaBarOutline.sizeDelta = new Vector2(14f + Mathf.Max(1f, statusSum) * this.fullBar.sizeDelta.x, this.staminaBarOutline.sizeDelta.y);
		this.staminaBarOutlineOverflowBar.gameObject.SetActive((double)statusSum > 1.005);
		this.staminaBar.gameObject.SetActive(this.staminaBar.sizeDelta.x > this.minStaminaBarWidth);
		this.maxStaminaBar.gameObject.SetActive(this.maxStaminaBar.sizeDelta.x > this.minStaminaBarWidth);
		bool flag = Character.observedCharacter.data.extraStamina > 0f;
		if (!this.extraBar.gameObject.activeSelf && flag)
		{
			this.extraBar.sizeDelta = Vector2.zero;
			this.extraBar.DOKill(false);
			this.extraBar.DOSizeDelta(new Vector2(45f, 45f), 0.25f, false).SetEase(Ease.OutCubic);
			this.extraBar.gameObject.SetActive(true);
			this.desiredExtraStaminaSize = Mathf.Max(0f, Character.observedCharacter.data.extraStamina * this.fullBar.sizeDelta.x);
			this.extraBarOutline.sizeDelta = new Vector2(Mathf.Max(20f, this.desiredExtraStaminaSize + 12f), this.extraBarOutline.sizeDelta.y);
			this.extraBarStamina.sizeDelta = new Vector2(this.desiredExtraStaminaSize, this.extraBarStamina.sizeDelta.y);
		}
		if (this.extraBar.gameObject.activeSelf)
		{
			this.desiredExtraStaminaSize = Mathf.Max(0f, Character.observedCharacter.data.extraStamina * this.fullBar.sizeDelta.x);
			this.extraBarStamina.sizeDelta = new Vector2(Mathf.Lerp(this.extraBarStamina.sizeDelta.x, Mathf.Max(6f, this.desiredExtraStaminaSize), Time.deltaTime * 10f), this.extraBarStamina.sizeDelta.y);
			if (Mathf.Abs(this.desiredExtraStaminaSize - this.extraBarStamina.sizeDelta.x) < 0.05f)
			{
				this.extraBarOutline.sizeDelta = new Vector2(Mathf.Lerp(this.extraBarOutline.sizeDelta.x, Mathf.Max(20f, this.desiredExtraStaminaSize + 12f), Time.deltaTime * 10f), this.extraBarOutline.sizeDelta.y);
			}
			else if (this.desiredExtraStaminaSize + 12f > this.extraBarOutline.sizeDelta.x)
			{
				this.extraBarOutline.sizeDelta = new Vector2(Mathf.Max(20f, this.desiredExtraStaminaSize + 12f), this.extraBarOutline.sizeDelta.y);
			}
			Color color2 = this.extraStaminaGlow.color;
			float num2 = Mathf.Clamp01((this.extraBar.sizeDelta.x - this.desiredExtraStaminaSize) * 0.5f);
			this.sinTime += Time.deltaTime * 10f * num2;
			color2.a = num2 * 0.4f - Mathf.Abs(Mathf.Sin(this.sinTime)) * 0.2f;
			this.extraStaminaGlow.color = color2;
			if (!flag && !this.sequencingExtraBar)
			{
				this.sequencingExtraBar = true;
				Sequence sequence = DOTween.Sequence();
				sequence.Append(this.extraBar.DOSizeDelta(new Vector2(this.extraBar.sizeDelta.x, 0f), 0.2f, false));
				sequence.OnComplete(new TweenCallback(this.DisableExtraBar));
			}
		}
		if (this.sinTime > this.TAU)
		{
			this.sinTime -= this.TAU;
		}
	}

	// Token: 0x06000A71 RID: 2673 RVA: 0x000332F0 File Offset: 0x000314F0
	public void OutOfStaminaPulse()
	{
		this.backing.color = this.outOfStaminaBackingColor;
		this.backing.DOColor(this.defaultBackingColor, 0.5f);
		this.noStaminaSFX.Play(default(Vector3));
	}

	// Token: 0x06000A72 RID: 2674 RVA: 0x00033339 File Offset: 0x00031539
	private void DisableExtraBar()
	{
		this.extraBar.gameObject.SetActive(false);
		this.sequencingExtraBar = false;
	}

	// Token: 0x06000A73 RID: 2675 RVA: 0x00033354 File Offset: 0x00031554
	public void AddRainbow()
	{
		if (this.rainbowRoutine != null)
		{
			base.StopCoroutine(this.rainbowRoutine);
		}
		this.rainbowStamina.enabled = true;
		this.rainbowStamina.color = new Color(1f, 1f, 1f, 0f);
		this.rainbowStamina.DOFade(1f, 0.5f);
	}

	// Token: 0x06000A74 RID: 2676 RVA: 0x000333BB File Offset: 0x000315BB
	public void RemoveRainbow()
	{
		this.rainbowStamina.DOFade(0f, 0.5f);
		this.rainbowRoutine = base.StartCoroutine(this.<RemoveRainbow>g__RemoveRainbowRoutine|36_0());
	}

	// Token: 0x06000A75 RID: 2677 RVA: 0x000333E5 File Offset: 0x000315E5
	public void PlayMoraleBoost(int scoutCount)
	{
		this.moraleBoostText.enabled = true;
		this.moraleBoostText.text = "MORALE BOOST!!";
		base.StartCoroutine(this.MoraleBoostRoutine());
	}

	// Token: 0x06000A76 RID: 2678 RVA: 0x00033410 File Offset: 0x00031610
	private IEnumerator MoraleBoostRoutine()
	{
		if (this.animator == null)
		{
			this.animator = new DOTweenTMPAnimator(this.moraleBoostText);
		}
		this.animator.Refresh();
		this.moraleBoostAnimator.Play("Boost", 0, 0f);
		for (int j = 0; j < this.animator.textInfo.characterCount; j++)
		{
			this.animator.SetCharScale(j, Vector3.zero);
		}
		yield return null;
		int num;
		for (int i = 0; i < this.animator.textInfo.characterCount; i = num + 1)
		{
			this.animator.DOScaleChar(i, Vector3.one, 0.2f).SetEase(Ease.OutBack);
			yield return new WaitForSeconds(0.033f);
			num = i;
		}
		yield return new WaitForSeconds(2f);
		yield return new WaitForSeconds(0.5f);
		this.moraleBoostText.enabled = false;
		yield break;
	}

	// Token: 0x06000A78 RID: 2680 RVA: 0x0003343D File Offset: 0x0003163D
	[CompilerGenerated]
	private IEnumerator <RemoveRainbow>g__RemoveRainbowRoutine|36_0()
	{
		yield return new WaitForSeconds(0.5f);
		this.rainbowStamina.enabled = false;
		yield break;
	}

	// Token: 0x04000935 RID: 2357
	public Image backing;

	// Token: 0x04000936 RID: 2358
	public RectTransform fullBar;

	// Token: 0x04000937 RID: 2359
	public RectTransform staminaBar;

	// Token: 0x04000938 RID: 2360
	public Image staminaGlow;

	// Token: 0x04000939 RID: 2361
	public Image extraStaminaGlow;

	// Token: 0x0400093A RID: 2362
	public RectTransform maxStaminaBar;

	// Token: 0x0400093B RID: 2363
	public RectTransform staminaBarOutline;

	// Token: 0x0400093C RID: 2364
	public RectTransform staminaBarOutlineOverflowBar;

	// Token: 0x0400093D RID: 2365
	public RectTransform extraBar;

	// Token: 0x0400093E RID: 2366
	public RectTransform extraBarStamina;

	// Token: 0x0400093F RID: 2367
	public RectTransform extraBarOutline;

	// Token: 0x04000940 RID: 2368
	public Image rainbowStamina;

	// Token: 0x04000941 RID: 2369
	[HideInInspector]
	public BarAffliction[] afflictions;

	// Token: 0x04000942 RID: 2370
	public float staminaBarOffset;

	// Token: 0x04000943 RID: 2371
	private float desiredStaminaSize;

	// Token: 0x04000944 RID: 2372
	private float desiredMaxStaminaSize;

	// Token: 0x04000945 RID: 2373
	private float desiredExtraStaminaSize;

	// Token: 0x04000946 RID: 2374
	public float minAfflictionWidth = 60f;

	// Token: 0x04000947 RID: 2375
	public float minStaminaBarWidth = 20f;

	// Token: 0x04000948 RID: 2376
	public TextMeshProUGUI moraleBoostText;

	// Token: 0x04000949 RID: 2377
	public Animator moraleBoostAnimator;

	// Token: 0x0400094A RID: 2378
	public Color defaultBackingColor;

	// Token: 0x0400094B RID: 2379
	public Color outOfStaminaBackingColor;

	// Token: 0x0400094C RID: 2380
	private float TAU;

	// Token: 0x0400094D RID: 2381
	public SFX_Instance noStaminaSFX;

	// Token: 0x0400094E RID: 2382
	private float allAfflictionSizes;

	// Token: 0x0400094F RID: 2383
	private bool outOfStamina;

	// Token: 0x04000950 RID: 2384
	private float sinTime;

	// Token: 0x04000951 RID: 2385
	private bool sequencingExtraBar;

	// Token: 0x04000952 RID: 2386
	private Coroutine rainbowRoutine;

	// Token: 0x04000953 RID: 2387
	private DOTweenTMPAnimator animator;
}
