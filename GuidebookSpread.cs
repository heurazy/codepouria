using System;
using TMPro;
using UnityEngine;

// Token: 0x020000D8 RID: 216
public class GuidebookSpread : MonoBehaviour
{
	// Token: 0x06000698 RID: 1688 RVA: 0x0002315C File Offset: 0x0002135C
	internal void SetPageLeft(RectTransform prefab)
	{
		if (this.pageLeftTransform != null)
		{
			Object.DestroyImmediate(this.pageLeftTransform.gameObject);
		}
		this.pageLeftTransform = Object.Instantiate<RectTransform>(prefab, base.transform);
		this.pageLeftTransform.offsetMax = new Vector2(-this.page1AlignmentRight, -this.page1AlignmentTop);
		this.pageLeftTransform.offsetMin = new Vector2(this.page1AlignmentLeft, this.page1AlignmentBottom);
	}

	// Token: 0x06000699 RID: 1689 RVA: 0x000231D4 File Offset: 0x000213D4
	internal void SetPageRight(RectTransform prefab)
	{
		if (this.pageRightTransform != null)
		{
			Object.DestroyImmediate(this.pageRightTransform.gameObject);
		}
		this.pageRightTransform = Object.Instantiate<RectTransform>(prefab, base.transform);
		this.pageRightTransform.offsetMax = new Vector2(-this.page1AlignmentLeft, -this.page1AlignmentTop);
		this.pageRightTransform.offsetMin = new Vector2(this.page1AlignmentRight, this.page1AlignmentTop);
	}

	// Token: 0x0600069A RID: 1690 RVA: 0x0002324C File Offset: 0x0002144C
	internal void ClearContents()
	{
		for (int i = base.transform.childCount - 1; i >= 0; i--)
		{
			Object.DestroyImmediate(base.transform.GetChild(i).gameObject);
		}
	}

	// Token: 0x04000649 RID: 1609
	public TextMeshProUGUI pageNumberLeft;

	// Token: 0x0400064A RID: 1610
	public TextMeshProUGUI pageNumberRight;

	// Token: 0x0400064B RID: 1611
	public RectTransform pageLeftTransform;

	// Token: 0x0400064C RID: 1612
	public RectTransform pageRightTransform;

	// Token: 0x0400064D RID: 1613
	public float page1AlignmentLeft;

	// Token: 0x0400064E RID: 1614
	public float page1AlignmentRight;

	// Token: 0x0400064F RID: 1615
	public float page1AlignmentTop;

	// Token: 0x04000650 RID: 1616
	public float page1AlignmentBottom;
}
