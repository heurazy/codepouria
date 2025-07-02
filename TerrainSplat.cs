using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

// Token: 0x02000285 RID: 645
[ExecuteInEditMode]
public class TerrainSplat : MonoBehaviour
{
	// Token: 0x06000F7D RID: 3965 RVA: 0x0004E99C File Offset: 0x0004CB9C
	private void SetTerrainVariables()
	{
		Shader.SetGlobalFloat("TerrainTriplanarScale", this.TerrainTriplanarScale);
		Shader.SetGlobalTexture("TerrainTextureR", this.TerrainTextureR);
		Shader.SetGlobalColor("TerrainColorR", this.TerrainColorR.linear);
		Shader.SetGlobalVector("TerrainSmoothR", this.TerrainSmoothR);
		Shader.SetGlobalTexture("TerrainTextureG", this.TerrainTextureG);
		Shader.SetGlobalColor("TerrainColorG", this.TerrainColorG.linear);
		Shader.SetGlobalVector("TerrainSmoothG", this.TerrainSmoothG);
		Shader.SetGlobalTexture("TerrainTextureB", this.TerrainTextureB);
		Shader.SetGlobalColor("TerrainColorB", this.TerrainColorB.linear);
		Shader.SetGlobalVector("TerrainSmoothB", this.TerrainSmoothB);
		Shader.SetGlobalTexture("TerrainTextureA", this.TerrainTextureA);
		Shader.SetGlobalColor("TerrainColorA", this.TerrainColorA.linear);
		Shader.SetGlobalVector("TerrainSmoothA", this.TerrainSmoothA);
	}

	// Token: 0x06000F7E RID: 3966 RVA: 0x0004EAA1 File Offset: 0x0004CCA1
	private void Start()
	{
		this.Generate(TerrainBrush.BrushType.All);
	}

	// Token: 0x06000F7F RID: 3967 RVA: 0x0004EAAA File Offset: 0x0004CCAA
	private void GenerateAll()
	{
		this.Generate(TerrainBrush.BrushType.All);
	}

	// Token: 0x06000F80 RID: 3968 RVA: 0x0004EAB4 File Offset: 0x0004CCB4
	public void Generate(TerrainBrush.BrushType brushType)
	{
		this.SetTerrainVariables();
		this.GetBounds();
		if (brushType == TerrainBrush.BrushType.All)
		{
			this.SampleHeightMap();
			this.CreateHeighMap();
		}
		this.CreateColorData(brushType);
		this.ApplyBrushes(brushType);
		if (brushType == TerrainBrush.BrushType.All || brushType == TerrainBrush.BrushType.Splat)
		{
			this.splatMap = this.CreateTexture(this.splatMap, this.splatColors);
		}
		if (brushType == TerrainBrush.BrushType.All || brushType == TerrainBrush.BrushType.Detail)
		{
			this.detailMap = this.CreateTexture(this.detailMap, this.detailColors);
		}
		this.SetShaderData(brushType);
	}

	// Token: 0x06000F81 RID: 3969 RVA: 0x0004EB34 File Offset: 0x0004CD34
	private void SampleHeightMap()
	{
		this.heights = new Color[this.splatRess, this.splatRess];
		for (int i = 0; i < this.splatRess; i++)
		{
			for (int j = 0; j < this.splatRess; j++)
			{
				this.heights[i, j] = this.SampleHeight(i, j);
			}
		}
	}

	// Token: 0x06000F82 RID: 3970 RVA: 0x0004EB90 File Offset: 0x0004CD90
	private Color SampleHeight(int x, int y)
	{
		return new Color(HelperFunctions.GetGroundPos(this.GetPosFromIndex(x, y) + Vector3.up * 1000f, HelperFunctions.LayerType.Terrain, 0f).y / 10f, 0f, 0f, 0f);
	}

	// Token: 0x06000F83 RID: 3971 RVA: 0x0004EBE4 File Offset: 0x0004CDE4
	private void CreateHeighMap()
	{
		if (this.heightMap)
		{
			Object.DestroyImmediate(this.heightMap);
		}
		this.heightMap = new Texture2D(this.splatRess, this.splatRess, TextureFormat.RFloat, 0, true);
		this.heightMap.filterMode = FilterMode.Bilinear;
		this.heightMap.wrapMode = TextureWrapMode.Clamp;
		this.heightMap.SetPixels(HelperFunctions.GridToFlatArray<Color>(this.heights));
		this.heightMap.Apply();
	}

	// Token: 0x06000F84 RID: 3972 RVA: 0x0004EC60 File Offset: 0x0004CE60
	private void SetShaderData(TerrainBrush.BrushType brushType)
	{
		if (brushType == TerrainBrush.BrushType.All || brushType == TerrainBrush.BrushType.Detail)
		{
			Shader.SetGlobalTexture("TerrainDetail", this.detailMap);
		}
		if (brushType == TerrainBrush.BrushType.All || brushType == TerrainBrush.BrushType.Splat)
		{
			Shader.SetGlobalTexture("TerrainSplat", this.splatMap);
		}
		if (brushType == TerrainBrush.BrushType.All)
		{
			Shader.SetGlobalTexture("TerrainHeight", this.heightMap);
		}
		Shader.SetGlobalVector("TerrainCenter", this.bounds.center);
		Shader.SetGlobalVector("TerrainSize", this.bounds.size);
	}

	// Token: 0x06000F85 RID: 3973 RVA: 0x0004ECE4 File Offset: 0x0004CEE4
	private void OnDestroy()
	{
		if (this.splatMap)
		{
			Object.DestroyImmediate(this.splatMap);
		}
	}

	// Token: 0x06000F86 RID: 3974 RVA: 0x0004ED00 File Offset: 0x0004CF00
	private void CreateColorData(TerrainBrush.BrushType brushType)
	{
		if (brushType == TerrainBrush.BrushType.All || brushType == TerrainBrush.BrushType.Splat)
		{
			this.splatColors = new Color[this.splatRess, this.splatRess];
			for (int i = 0; i < this.splatRess; i++)
			{
				for (int j = 0; j < this.splatRess; j++)
				{
					this.splatColors[i, j] = TerrainSplat.GetColor(this.baseColor);
				}
			}
		}
		if (brushType == TerrainBrush.BrushType.All || brushType == TerrainBrush.BrushType.Detail)
		{
			this.detailColors = new Color[this.splatRess, this.splatRess];
			for (int k = 0; k < this.splatRess; k++)
			{
				for (int l = 0; l < this.splatRess; l++)
				{
					this.detailColors[k, l] = new Color(0.5f, 0.5f, 0.5f, 0f);
				}
			}
		}
	}

	// Token: 0x06000F87 RID: 3975 RVA: 0x0004EDCC File Offset: 0x0004CFCC
	private void ApplyBrushes(TerrainBrush.BrushType brushType)
	{
		foreach (TerrainBrush terrainBrush in HelperFunctions.SortBySiblingIndex<TerrainBrush>(Object.FindObjectsByType<TerrainBrush>(FindObjectsSortMode.InstanceID)).ToArray<TerrainBrush>())
		{
			if (brushType == TerrainBrush.BrushType.All || brushType == terrainBrush.brushType)
			{
				this.ApplySplatBrush(terrainBrush);
			}
		}
	}

	// Token: 0x06000F88 RID: 3976 RVA: 0x0004EE10 File Offset: 0x0004D010
	private void ApplySplatBrush(TerrainBrush item)
	{
		if (item.brushType == TerrainBrush.BrushType.Splat)
		{
			item.ApplySplatData(this.splatColors, this.bounds);
			return;
		}
		item.ApplySplatData(this.detailColors, this.bounds);
	}

	// Token: 0x06000F89 RID: 3977 RVA: 0x0004EE40 File Offset: 0x0004D040
	private void GetBounds()
	{
		Renderer[] array = HelperFunctions.GetComponentListFromComponentArray<TerrainSplatMesh, Renderer>(Object.FindObjectsByType<TerrainSplatMesh>(FindObjectsSortMode.None)).ToArray();
		this.bounds = HelperFunctions.GetTotalBounds(array);
	}

	// Token: 0x06000F8A RID: 3978 RVA: 0x0004EE6C File Offset: 0x0004D06C
	private Texture2D CreateTexture(Texture2D texture, Color[,] colors)
	{
		if (texture)
		{
			Object.DestroyImmediate(texture);
		}
		texture = new Texture2D(this.splatRess, this.splatRess, DefaultFormat.LDR, TextureCreationFlags.None);
		texture.filterMode = FilterMode.Bilinear;
		texture.wrapMode = TextureWrapMode.Clamp;
		texture.SetPixels(HelperFunctions.GridToFlatArray<Color>(colors));
		texture.Apply();
		return texture;
	}

	// Token: 0x06000F8B RID: 3979 RVA: 0x0004EEBD File Offset: 0x0004D0BD
	private Vector3 GetPosFromIndex(int x, int y)
	{
		return this.GetPos((float)x / ((float)this.splatRess - 1f), (float)y / ((float)this.splatRess - 1f));
	}

	// Token: 0x06000F8C RID: 3980 RVA: 0x0004EEE8 File Offset: 0x0004D0E8
	private Vector3 GetPos(float pX, float pY)
	{
		Vector3 vector = Vector3.right * this.bounds.size.x * Mathf.Lerp(-0.5f, 0.5f, pX);
		Vector3 vector2 = Vector3.forward * this.bounds.size.z * Mathf.Lerp(-0.5f, 0.5f, pY);
		return this.bounds.center + vector + vector2;
	}

	// Token: 0x06000F8D RID: 3981 RVA: 0x0004EF6C File Offset: 0x0004D16C
	internal static Color GetColor(TerrainSplat.SplatColor color)
	{
		if (color == TerrainSplat.SplatColor.Black)
		{
			return new Color(0f, 0f, 0f, 0f);
		}
		if (color == TerrainSplat.SplatColor.Red)
		{
			return new Color(1f, 0f, 0f, 0f);
		}
		if (color == TerrainSplat.SplatColor.Green)
		{
			return new Color(0f, 1f, 0f, 0f);
		}
		if (color == TerrainSplat.SplatColor.Blue)
		{
			return new Color(0f, 0f, 1f, 0f);
		}
		if (color == TerrainSplat.SplatColor.Alpha)
		{
			return new Color(0f, 0f, 0f, 1f);
		}
		if (color == TerrainSplat.SplatColor.HalfRed)
		{
			return new Color(0.5f, 0f, 0f, 0f);
		}
		if (color == TerrainSplat.SplatColor.HalfGreen)
		{
			return new Color(0f, 0.5f, 0f, 0f);
		}
		if (color == TerrainSplat.SplatColor.HalfBlue)
		{
			return new Color(0f, 0f, 0.5f, 0f);
		}
		return new Color(0f, 0f, 0f, 0.5f);
	}

	// Token: 0x06000F8E RID: 3982 RVA: 0x0004F084 File Offset: 0x0004D284
	internal Color GetSplatPixelAtWorldPos(Vector3 point)
	{
		float num = Mathf.InverseLerp(this.bounds.min.x, this.bounds.max.x, point.x);
		float num2 = Mathf.InverseLerp(this.bounds.min.z, this.bounds.max.z, point.z);
		Vector2Int vector2Int = new Vector2Int(Mathf.RoundToInt(num * (float)this.splatMap.width), Mathf.RoundToInt(num2 * (float)this.splatMap.height));
		return this.splatMap.GetPixel(vector2Int.x, vector2Int.y);
	}

	// Token: 0x04000E7E RID: 3710
	public float TerrainTriplanarScale = 0.2f;

	// Token: 0x04000E7F RID: 3711
	public Texture2D TerrainTextureR;

	// Token: 0x04000E80 RID: 3712
	public Color TerrainColorR;

	// Token: 0x04000E81 RID: 3713
	public Vector2 TerrainSmoothR = new Vector2(0f, 1f);

	// Token: 0x04000E82 RID: 3714
	public Texture2D TerrainTextureG;

	// Token: 0x04000E83 RID: 3715
	public Color TerrainColorG;

	// Token: 0x04000E84 RID: 3716
	public Vector2 TerrainSmoothG = new Vector2(0f, 1f);

	// Token: 0x04000E85 RID: 3717
	public Texture2D TerrainTextureB;

	// Token: 0x04000E86 RID: 3718
	public Color TerrainColorB;

	// Token: 0x04000E87 RID: 3719
	public Vector2 TerrainSmoothB = new Vector2(0f, 1f);

	// Token: 0x04000E88 RID: 3720
	public Texture2D TerrainTextureA;

	// Token: 0x04000E89 RID: 3721
	public Color TerrainColorA;

	// Token: 0x04000E8A RID: 3722
	public Vector2 TerrainSmoothA = new Vector2(0f, 1f);

	// Token: 0x04000E8B RID: 3723
	public int splatRess;

	// Token: 0x04000E8C RID: 3724
	public TerrainSplat.SplatColor baseColor;

	// Token: 0x04000E8D RID: 3725
	public bool displayBrushes;

	// Token: 0x04000E8E RID: 3726
	public Texture2D splatMap;

	// Token: 0x04000E8F RID: 3727
	public Texture2D heightMap;

	// Token: 0x04000E90 RID: 3728
	public Texture2D detailMap;

	// Token: 0x04000E91 RID: 3729
	private Bounds bounds;

	// Token: 0x04000E92 RID: 3730
	private Color[,] splatColors;

	// Token: 0x04000E93 RID: 3731
	private Color[,] detailColors;

	// Token: 0x04000E94 RID: 3732
	private Color[,] heights;

	// Token: 0x020003BA RID: 954
	public enum SplatColor
	{
		// Token: 0x040013B4 RID: 5044
		Black,
		// Token: 0x040013B5 RID: 5045
		Red,
		// Token: 0x040013B6 RID: 5046
		Green,
		// Token: 0x040013B7 RID: 5047
		Blue,
		// Token: 0x040013B8 RID: 5048
		Alpha,
		// Token: 0x040013B9 RID: 5049
		HalfRed,
		// Token: 0x040013BA RID: 5050
		HalfGreen,
		// Token: 0x040013BB RID: 5051
		HalfBlue,
		// Token: 0x040013BC RID: 5052
		HalfAlpha
	}
}
