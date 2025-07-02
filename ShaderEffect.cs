using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200026D RID: 621
public class ShaderEffect : MonoBehaviour
{
	// Token: 0x06000EEC RID: 3820 RVA: 0x0004AE7B File Offset: 0x0004907B
	private void Start()
	{
		this.prop = new MaterialPropertyBlock();
	}

	// Token: 0x06000EED RID: 3821 RVA: 0x0004AE88 File Offset: 0x00049088
	private void Update()
	{
		foreach (Renderer renderer in this.renderers)
		{
			this.PerRendere(renderer);
		}
	}

	// Token: 0x06000EEE RID: 3822 RVA: 0x0004AEB5 File Offset: 0x000490B5
	private void PerRendere(Renderer item)
	{
	}

	// Token: 0x06000EEF RID: 3823 RVA: 0x0004AEB8 File Offset: 0x000490B8
	internal void SetEffect(Material mat, string key, float value)
	{
		if (!this.currentEffects.Contains(mat))
		{
			this.AddEffect(mat);
		}
		foreach (Renderer renderer in this.renderers)
		{
			this.prop.SetFloat(key, value);
			renderer.SetPropertyBlock(this.prop);
		}
	}

	// Token: 0x06000EF0 RID: 3824 RVA: 0x0004AF0C File Offset: 0x0004910C
	private void AddEffect(Material mat)
	{
		foreach (Renderer renderer in this.renderers)
		{
			List<Material> list = new List<Material>();
			list.AddRange(renderer.sharedMaterials);
			list.Add(mat);
			renderer.sharedMaterials = list.ToArray();
		}
		this.currentEffects.Add(mat);
	}

	// Token: 0x06000EF1 RID: 3825 RVA: 0x0004AF63 File Offset: 0x00049163
	internal void ClearEffect(Material mat)
	{
		if (this.currentEffects.Count == 0)
		{
			return;
		}
		if (this.currentEffects.Contains(mat))
		{
			this.RemoveEffect(mat);
		}
	}

	// Token: 0x06000EF2 RID: 3826 RVA: 0x0004AF88 File Offset: 0x00049188
	private void RemoveEffect(Material mat)
	{
		foreach (Renderer renderer in this.renderers)
		{
			List<Material> list = new List<Material>();
			list.AddRange(renderer.sharedMaterials);
			list.Remove(mat);
			renderer.sharedMaterials = list.ToArray();
		}
		this.currentEffects.Remove(mat);
	}

	// Token: 0x04000DC5 RID: 3525
	public Renderer[] renderers;

	// Token: 0x04000DC6 RID: 3526
	private List<Material> currentEffects = new List<Material>();

	// Token: 0x04000DC7 RID: 3527
	private MaterialPropertyBlock prop;
}
