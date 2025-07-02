using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001BF RID: 447
[ExecuteAlways]
[RequireComponent(typeof(GridLayoutGroup))]
public class DynamicGridCellSizer : MonoBehaviour
{
	// Token: 0x06000C22 RID: 3106 RVA: 0x0003C98F File Offset: 0x0003AB8F
	private void Awake()
	{
		this.grid = base.GetComponent<GridLayoutGroup>();
	}

	// Token: 0x06000C23 RID: 3107 RVA: 0x0003C99D File Offset: 0x0003AB9D
	private void Update()
	{
		if (base.transform.childCount != this.childCount)
		{
			this.childCount = base.transform.childCount;
			this.ResizeCells();
		}
	}

	// Token: 0x06000C24 RID: 3108 RVA: 0x0003C9CC File Offset: 0x0003ABCC
	public void ResizeCells()
	{
		this.iconCount = this.grid.transform.childCount;
		float width = this.gridRectTransform.rect.width;
		float height = this.gridRectTransform.rect.height;
		int num = Mathf.Max(1, Mathf.CeilToInt((float)this.iconCount / (float)this.maxIconsPerRow));
		Debug.Log("Rows!" + num.ToString());
		int num2 = Mathf.CeilToInt((float)this.iconCount / (float)num);
		float num3 = (width - (float)this.grid.padding.left - (float)this.grid.padding.right - this.grid.spacing.x * (float)(num2 - 1)) / (float)num2;
		float num4 = (height - (float)this.grid.padding.top - (float)this.grid.padding.bottom - this.grid.spacing.y * (float)(num - 1)) / (float)num;
		float num5 = Mathf.Min(num3, num4);
		this.grid.cellSize = new Vector2(num5, num5);
	}

	// Token: 0x04000B1D RID: 2845
	public RectTransform gridRectTransform;

	// Token: 0x04000B1E RID: 2846
	public int iconCount;

	// Token: 0x04000B1F RID: 2847
	public int maxIconsPerRow = 8;

	// Token: 0x04000B20 RID: 2848
	private GridLayoutGroup grid;

	// Token: 0x04000B21 RID: 2849
	private int childCount = -1;
}
