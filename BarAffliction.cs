using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200014E RID: 334
public class BarAffliction : MonoBehaviour
{
	// Token: 0x1700007D RID: 125
	// (get) Token: 0x0600098E RID: 2446 RVA: 0x000300D1 File Offset: 0x0002E2D1
	// (set) Token: 0x0600098F RID: 2447 RVA: 0x000300E3 File Offset: 0x0002E2E3
	public float width
	{
		get
		{
			return this.rtf.sizeDelta.x;
		}
		set
		{
			this.rtf.sizeDelta = new Vector2(value, this.rtf.sizeDelta.y);
		}
	}

	// Token: 0x06000990 RID: 2448 RVA: 0x00030106 File Offset: 0x0002E306
	public void OnEnable()
	{
		this.icon.transform.localScale = Vector3.zero;
		this.icon.transform.DOScale(1f, 0.5f).SetEase(Ease.OutElastic);
	}

	// Token: 0x06000991 RID: 2449 RVA: 0x00030140 File Offset: 0x0002E340
	public void ChangeAffliction(StaminaBar bar)
	{
		if (Character.observedCharacter == null)
		{
			return;
		}
		float currentStatus = Character.observedCharacter.refs.afflictions.GetCurrentStatus(this.afflictionType);
		this.size = bar.fullBar.sizeDelta.x * currentStatus;
		if (currentStatus > 0.01f)
		{
			if (this.size < bar.minAfflictionWidth)
			{
				this.size = bar.minAfflictionWidth;
			}
			base.gameObject.SetActive(true);
			return;
		}
		base.gameObject.SetActive(false);
	}

	// Token: 0x06000992 RID: 2450 RVA: 0x000301C9 File Offset: 0x0002E3C9
	public void UpdateAffliction(StaminaBar bar)
	{
		this.width = Mathf.Lerp(this.width, this.size, Mathf.Min(Time.deltaTime * 10f, 0.1f));
	}

	// Token: 0x04000873 RID: 2163
	public RectTransform rtf;

	// Token: 0x04000874 RID: 2164
	public Image icon;

	// Token: 0x04000875 RID: 2165
	public float size;

	// Token: 0x04000876 RID: 2166
	public CharacterAfflictions.STATUSTYPE afflictionType;
}
