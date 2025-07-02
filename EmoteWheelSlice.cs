using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x02000153 RID: 339
public class EmoteWheelSlice : UIWheelSlice, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	// Token: 0x060009AA RID: 2474 RVA: 0x000304B8 File Offset: 0x0002E6B8
	public void Init(EmoteWheelData data, EmoteWheel wheel)
	{
		this.emoteWheel = wheel;
		this.emoteData = data;
		if (data == null)
		{
			this.image.enabled = false;
			this.button.interactable = false;
			return;
		}
		this.image.enabled = true;
		this.image.sprite = data.emoteSprite;
		this.button.interactable = true;
	}

	// Token: 0x060009AB RID: 2475 RVA: 0x0003051E File Offset: 0x0002E71E
	public void Hover()
	{
		this.emoteWheel.Hover(this.emoteData);
	}

	// Token: 0x060009AC RID: 2476 RVA: 0x00030531 File Offset: 0x0002E731
	public void Dehover()
	{
		this.emoteWheel.Dehover(this.emoteData);
	}

	// Token: 0x060009AD RID: 2477 RVA: 0x00030544 File Offset: 0x0002E744
	public void OnPointerEnter(PointerEventData eventData)
	{
		this.Hover();
	}

	// Token: 0x060009AE RID: 2478 RVA: 0x0003054C File Offset: 0x0002E74C
	public void OnPointerExit(PointerEventData eventData)
	{
		this.Dehover();
	}

	// Token: 0x04000885 RID: 2181
	private EmoteWheel emoteWheel;

	// Token: 0x04000886 RID: 2182
	private EmoteWheelData emoteData;

	// Token: 0x04000887 RID: 2183
	public Image image;
}
