using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000179 RID: 377
public class UI_UseItemProgressFriend : MonoBehaviour
{
	// Token: 0x06000A90 RID: 2704 RVA: 0x00033858 File Offset: 0x00031A58
	public void Init(FeedData feedData)
	{
		this.giverID = feedData.giverID;
		this._maxTime = feedData.totalItemTime;
		Item item;
		if (ItemDatabase.TryGetItem(feedData.itemID, out item))
		{
			this.icon.texture = item.UIData.icon;
		}
		Vector2 sizeDelta = this.rect.sizeDelta;
		this.rect.sizeDelta = Vector2.zero;
		this.rect.DOSizeDelta(sizeDelta, 0.5f, false).SetEase(Ease.OutBack);
	}

	// Token: 0x06000A91 RID: 2705 RVA: 0x000338D8 File Offset: 0x00031AD8
	private void Update()
	{
		if (!this._dead)
		{
			this._currentTime += Time.deltaTime;
			this.fill.fillAmount = this._currentTime / this._maxTime;
		}
	}

	// Token: 0x06000A92 RID: 2706 RVA: 0x0003390C File Offset: 0x00031B0C
	public void Kill()
	{
		this._dead = true;
		Object.Destroy(base.gameObject);
	}

	// Token: 0x04000972 RID: 2418
	public RectTransform rect;

	// Token: 0x04000973 RID: 2419
	public Image fill;

	// Token: 0x04000974 RID: 2420
	public RawImage icon;

	// Token: 0x04000975 RID: 2421
	public int giverID;

	// Token: 0x04000976 RID: 2422
	private float _maxTime;

	// Token: 0x04000977 RID: 2423
	private float _currentTime;

	// Token: 0x04000978 RID: 2424
	private bool _dead;
}
