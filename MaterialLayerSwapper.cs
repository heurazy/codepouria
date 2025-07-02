using System;
using UnityEngine;

// Token: 0x020001FF RID: 511
public class MaterialLayerSwapper : MonoBehaviour
{
	// Token: 0x06000D42 RID: 3394 RVA: 0x00042C14 File Offset: 0x00040E14
	private void Swap()
	{
		string text = "_Color" + this.layer.x.ToString("F0");
		string text2 = "_Smooth" + this.layer.x.ToString("F0");
		string text3 = "_Height" + this.layer.x.ToString("F0");
		string text4 = "_Texture" + this.layer.x.ToString("F0");
		string text5 = "_Triplanar" + this.layer.x.ToString("F0");
		string text6 = "_UV" + this.layer.x.ToString("F0");
		string text7 = "_Flip" + this.layer.x.ToString("F0");
		string text8 = "_Remap" + this.layer.x.ToString("F0");
		Material material = base.GetComponentInChildren<Renderer>().sharedMaterials[this.targetMaterial];
		this.color = material.GetColor(text);
		this.smooth = material.GetFloat(text2);
		this.height = material.GetFloat(text3);
		this.texture = material.GetTexture(text4);
		this.triplanar = material.GetFloat(text5);
		this.uv = material.GetFloat(text6);
		this.flip = material.GetFloat(text7);
		this.remap = material.GetVector(text8);
		string text9 = "_Color" + this.layer.y.ToString("F0");
		string text10 = "_Smooth" + this.layer.y.ToString("F0");
		string text11 = "_Height" + this.layer.y.ToString("F0");
		string text12 = "_Texture" + this.layer.y.ToString("F0");
		string text13 = "_Triplanar" + this.layer.y.ToString("F0");
		string text14 = "_UV" + this.layer.y.ToString("F0");
		string text15 = "_Flip" + this.layer.y.ToString("F0");
		string text16 = "_Remap" + this.layer.y.ToString("F0");
		this.color2 = material.GetColor(text9);
		this.smooth2 = material.GetFloat(text10);
		this.height2 = material.GetFloat(text11);
		this.texture2 = material.GetTexture(text12);
		this.triplanar2 = material.GetFloat(text13);
		this.uv2 = material.GetFloat(text14);
		this.flip2 = material.GetFloat(text15);
		this.remap2 = material.GetVector(text16);
		material.SetColor(text9, this.color);
		material.SetFloat(text10, this.smooth);
		material.SetFloat(text11, this.height);
		material.SetTexture(text12, this.texture);
		material.SetFloat(text13, this.triplanar);
		material.SetFloat(text14, this.uv);
		material.SetFloat(text15, this.flip);
		material.SetVector(text16, this.remap);
		material.SetColor(text, this.color2);
		material.SetFloat(text2, this.smooth2);
		material.SetFloat(text3, this.height2);
		material.SetTexture(text4, this.texture2);
		material.SetFloat(text5, this.triplanar2);
		material.SetFloat(text6, this.uv2);
		material.SetFloat(text7, this.flip2);
		material.SetVector(text8, this.remap2);
	}

	// Token: 0x04000C63 RID: 3171
	public int targetMaterial;

	// Token: 0x04000C64 RID: 3172
	public Vector2Int layer;

	// Token: 0x04000C65 RID: 3173
	[ColorUsage(true, true)]
	public Color color;

	// Token: 0x04000C66 RID: 3174
	public float smooth;

	// Token: 0x04000C67 RID: 3175
	public float height;

	// Token: 0x04000C68 RID: 3176
	public Texture texture;

	// Token: 0x04000C69 RID: 3177
	public float triplanar;

	// Token: 0x04000C6A RID: 3178
	public float uv;

	// Token: 0x04000C6B RID: 3179
	public float flip;

	// Token: 0x04000C6C RID: 3180
	public Vector2 remap;

	// Token: 0x04000C6D RID: 3181
	[ColorUsage(true, true)]
	public Color color2;

	// Token: 0x04000C6E RID: 3182
	public float smooth2;

	// Token: 0x04000C6F RID: 3183
	public float height2;

	// Token: 0x04000C70 RID: 3184
	public Texture texture2;

	// Token: 0x04000C71 RID: 3185
	public float triplanar2;

	// Token: 0x04000C72 RID: 3186
	public float uv2;

	// Token: 0x04000C73 RID: 3187
	public float flip2;

	// Token: 0x04000C74 RID: 3188
	public Vector2 remap2;
}
