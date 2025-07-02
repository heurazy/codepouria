using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x0200019D RID: 413
public class ButtonHoverFeedback : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	// Token: 0x06000B5C RID: 2908 RVA: 0x00038266 File Offset: 0x00036466
	private void Start()
	{
		Button component = base.GetComponent<Button>();
		if (component == null)
		{
			return;
		}
		component.onClick.AddListener(new UnityAction(this.OnClick));
	}

	// Token: 0x06000B5D RID: 2909 RVA: 0x00038289 File Offset: 0x00036489
	private void OnClick()
	{
		this.vel += 15f;
	}

	// Token: 0x06000B5E RID: 2910 RVA: 0x0003829D File Offset: 0x0003649D
	public void OnPointerEnter(PointerEventData eventData)
	{
		this.targetScale = 1.15f;
	}

	// Token: 0x06000B5F RID: 2911 RVA: 0x000382AA File Offset: 0x000364AA
	public void OnPointerExit(PointerEventData eventData)
	{
		this.targetScale = 1f;
	}

	// Token: 0x06000B60 RID: 2912 RVA: 0x000382B7 File Offset: 0x000364B7
	private void OnEnable()
	{
		base.transform.localScale = Vector3.one;
		this.scale = 1f;
		this.vel = 0f;
		this.targetScale = 1f;
	}

	// Token: 0x06000B61 RID: 2913 RVA: 0x000382EC File Offset: 0x000364EC
	private void Update()
	{
		this.vel = FRILerp.Lerp(this.vel, (this.targetScale - this.scale) * 25f, 20f, true);
		this.scale += this.vel * Time.deltaTime;
		base.transform.localScale = Vector3.one * this.scale;
	}

	// Token: 0x04000A71 RID: 2673
	private float scale = 1f;

	// Token: 0x04000A72 RID: 2674
	private float vel;

	// Token: 0x04000A73 RID: 2675
	private float targetScale = 1f;
}
