using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x0200014D RID: 333
public class BadgeUI : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
	// Token: 0x06000986 RID: 2438 RVA: 0x0002FFF0 File Offset: 0x0002E1F0
	public void Init(BadgeData data)
	{
		this.data = data;
		if (data)
		{
			this.icon.texture = data.icon;
			this.icon.color = new Color(1f, 1f, 1f, (float)(data.IsLocked ? 0 : 1));
			this.icon.enabled = true;
			this.blank.enabled = false;
			return;
		}
		this.icon.enabled = false;
		this.blank.enabled = true;
	}

	// Token: 0x06000987 RID: 2439 RVA: 0x0003007A File Offset: 0x0002E27A
	public void Hover()
	{
		this.manager.selectedBadge = this;
	}

	// Token: 0x06000988 RID: 2440 RVA: 0x00030088 File Offset: 0x0002E288
	public void Dehover()
	{
		if (this.manager.selectedBadge == this)
		{
			this.manager.selectedBadge = null;
		}
	}

	// Token: 0x06000989 RID: 2441 RVA: 0x000300A9 File Offset: 0x0002E2A9
	public void OnPointerEnter(PointerEventData eventData)
	{
		this.Hover();
	}

	// Token: 0x0600098A RID: 2442 RVA: 0x000300B1 File Offset: 0x0002E2B1
	public void OnPointerExit(PointerEventData eventData)
	{
		this.Dehover();
	}

	// Token: 0x0600098B RID: 2443 RVA: 0x000300B9 File Offset: 0x0002E2B9
	public void OnSelect(BaseEventData eventData)
	{
		this.Hover();
	}

	// Token: 0x0600098C RID: 2444 RVA: 0x000300C1 File Offset: 0x0002E2C1
	public void OnDeselect(BaseEventData eventData)
	{
		this.Dehover();
	}

	// Token: 0x0400086E RID: 2158
	public BadgeManager manager;

	// Token: 0x0400086F RID: 2159
	public RawImage icon;

	// Token: 0x04000870 RID: 2160
	public RawImage blank;

	// Token: 0x04000871 RID: 2161
	public BadgeData data;

	// Token: 0x04000872 RID: 2162
	public CanvasGroup canvasGroup;
}
