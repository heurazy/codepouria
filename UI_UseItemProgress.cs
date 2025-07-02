using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000295 RID: 661
public class UI_UseItemProgress : MonoBehaviour
{
	// Token: 0x170000C0 RID: 192
	// (get) Token: 0x06000FC1 RID: 4033 RVA: 0x0004FCD3 File Offset: 0x0004DED3
	private bool constantUseInteractableExists
	{
		get
		{
			return Interaction.instance.currentHeldInteractible != null;
		}
	}

	// Token: 0x06000FC2 RID: 4034 RVA: 0x0004FCE4 File Offset: 0x0004DEE4
	private void Update()
	{
		if (Character.localCharacter == null)
		{
			return;
		}
		Character.localCharacter.data.currentItem != null;
		bool flag = this.UpdateFillAmount();
		if (!this.fill.enabled && flag)
		{
			base.transform.DOKill(false);
			base.transform.localScale = Vector3.zero;
			base.transform.DOScale(1f, 0.25f).SetEase(Ease.OutBack);
		}
		this.fill.enabled = flag;
		this.empty.enabled = this.fill.enabled;
	}

	// Token: 0x06000FC3 RID: 4035 RVA: 0x0004FD8C File Offset: 0x0004DF8C
	private bool UpdateFillAmount()
	{
		bool flag = Character.localCharacter.data.currentItem != null;
		if (Character.localCharacter.refs.items.climbingSpikeCastProgress > 0f)
		{
			this.fill.fillAmount = Character.localCharacter.refs.items.climbingSpikeCastProgress;
			return true;
		}
		if (flag && Character.localCharacter.data.currentItem.shouldShowCastProgress)
		{
			float num = Mathf.Max(Character.localCharacter.data.currentItem.overrideProgress, Character.localCharacter.data.currentItem.castProgress);
			if (num > 0f)
			{
				this.fill.fillAmount = num;
				return true;
			}
		}
		else if (this.constantUseInteractableExists && Interaction.instance.constantInteractableProgress > 0f)
		{
			this.fill.fillAmount = Interaction.instance.constantInteractableProgress;
			return true;
		}
		return false;
	}

	// Token: 0x04000ED1 RID: 3793
	public Image fill;

	// Token: 0x04000ED2 RID: 3794
	public Image empty;
}
