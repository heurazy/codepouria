using System;
using UnityEngine;

// Token: 0x02000284 RID: 644
public class TerrainBrush : MonoBehaviour
{
	// Token: 0x06000F74 RID: 3956 RVA: 0x0004E4CE File Offset: 0x0004C6CE
	private void Start()
	{
	}

	// Token: 0x06000F75 RID: 3957 RVA: 0x0004E4D0 File Offset: 0x0004C6D0
	public void Generate()
	{
		Object.FindAnyObjectByType<TerrainSplat>().Generate(this.brushType);
	}

	// Token: 0x06000F76 RID: 3958 RVA: 0x0004E4E4 File Offset: 0x0004C6E4
	private Bounds GetBounds()
	{
		Bounds bounds = new Bounds(base.transform.position, Vector3.zero);
		bounds.Encapsulate(base.transform.position + base.transform.right * 0.5f * base.transform.localScale.x * 1.4f);
		bounds.Encapsulate(base.transform.position + base.transform.right * -0.5f * base.transform.localScale.x * 1.4f);
		bounds.Encapsulate(base.transform.position + base.transform.forward * 0.5f * base.transform.localScale.z * 1.4f);
		bounds.Encapsulate(base.transform.position + base.transform.forward * -0.5f * base.transform.localScale.z * 1.4f);
		return bounds;
	}

	// Token: 0x06000F77 RID: 3959 RVA: 0x0004E638 File Offset: 0x0004C838
	private Vector3 GetPos(float pX, float pY)
	{
		Vector3 vector = base.transform.right * base.transform.localScale.x * Mathf.Lerp(-0.5f, 0.5f, pX);
		Vector3 vector2 = base.transform.forward * base.transform.localScale.z * Mathf.Lerp(-0.5f, 0.5f, pY);
		return base.transform.position + vector + vector2;
	}

	// Token: 0x06000F78 RID: 3960 RVA: 0x0004E6C8 File Offset: 0x0004C8C8
	internal void ApplySplatData(Color[,] colors, Bounds totalBounds)
	{
		foreach (Vector2Int vector2Int in HelperFunctions.GetIndexesInBounds(colors.GetLength(0), colors.GetLength(1), this.GetBounds(), totalBounds))
		{
			Vector3 vector = HelperFunctions.IDToWorldPos(vector2Int.x, vector2Int.y, colors.GetLength(0), colors.GetLength(1), totalBounds);
			if (this.brushType == TerrainBrush.BrushType.Splat)
			{
				colors[vector2Int.x, vector2Int.y] = this.SampleSplatColor(vector, colors[vector2Int.x, vector2Int.y]);
			}
			else
			{
				colors[vector2Int.x, vector2Int.y] = this.SampleDetailColor(vector, colors[vector2Int.x, vector2Int.y]);
			}
		}
	}

	// Token: 0x06000F79 RID: 3961 RVA: 0x0004E7BC File Offset: 0x0004C9BC
	private Color SampleSplatColor(Vector3 pos, Color beforeColor)
	{
		float num = this.SampleMask(pos);
		Color color = Color.Lerp(beforeColor * 2f, TerrainSplat.GetColor(this.color) * 2f, num * this.strength);
		float magnitude = new Vector4(color.r, color.g, color.b, color.a).magnitude;
		return color / magnitude;
	}

	// Token: 0x06000F7A RID: 3962 RVA: 0x0004E82C File Offset: 0x0004CA2C
	private Color SampleDetailColor(Vector3 pos, Color beforeColor)
	{
		float num = this.SampleMask(pos);
		Color color = this.detailColor;
		color.a *= num;
		Color color2;
		if (beforeColor.a <= 0.01f)
		{
			color2 = color;
		}
		else
		{
			float num2 = color.a / beforeColor.a;
			Color color3 = Color.Lerp(beforeColor, color, num2);
			color3.a = Mathf.Lerp(beforeColor.a, color.a, num2);
			color2 = color3;
		}
		return color2;
	}

	// Token: 0x06000F7B RID: 3963 RVA: 0x0004E89C File Offset: 0x0004CA9C
	private float SampleMask(Vector3 pos)
	{
		Vector3 vector = base.transform.InverseTransformPoint(pos);
		float num = -0.5f;
		float num2 = 0.5f;
		float num3 = Mathf.InverseLerp(num, num2, vector.x);
		float num4 = -0.5f;
		float num5 = 0.5f;
		float num6 = Mathf.InverseLerp(num4, num5, vector.z);
		float num7 = this.texture.GetPixel(Mathf.RoundToInt(num3 * (float)this.texture.width), Mathf.RoundToInt(num6 * (float)this.texture.height)).r;
		num7 = Mathf.InverseLerp(this.minMaxSlider.x, this.minMaxSlider.y, num7);
		return Mathf.Clamp01(num7);
	}

	// Token: 0x04000E77 RID: 3703
	public TerrainBrush.BrushType brushType;

	// Token: 0x04000E78 RID: 3704
	public Texture2D texture;

	// Token: 0x04000E79 RID: 3705
	public TerrainSplat.SplatColor color;

	// Token: 0x04000E7A RID: 3706
	[Range(0f, 1f)]
	public float strength = 1f;

	// Token: 0x04000E7B RID: 3707
	public Color detailColor = new Color(1f, 1f, 1f, 1f);

	// Token: 0x04000E7C RID: 3708
	public Vector2 minMaxSlider = new Vector2(0f, 1f);

	// Token: 0x04000E7D RID: 3709
	private TerrainSplat splat;

	// Token: 0x020003B9 RID: 953
	public enum BrushType
	{
		// Token: 0x040013B0 RID: 5040
		Splat,
		// Token: 0x040013B1 RID: 5041
		Detail,
		// Token: 0x040013B2 RID: 5042
		All
	}
}
