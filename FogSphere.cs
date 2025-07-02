using System;
using UnityEngine;

// Token: 0x020001CD RID: 461
[ExecuteAlways]
public class FogSphere : MonoBehaviour
{
	// Token: 0x06000C59 RID: 3161 RVA: 0x0003D6D8 File Offset: 0x0003B8D8
	private void Start()
	{
		this.rend = base.GetComponent<Renderer>();
	}

	// Token: 0x06000C5A RID: 3162 RVA: 0x0003D6E6 File Offset: 0x0003B8E6
	private void OnDisable()
	{
		Shader.SetGlobalFloat("FogEnabled", 0f);
		Shader.SetGlobalFloat("_FogSphereSize", 9999999f);
	}

	// Token: 0x06000C5B RID: 3163 RVA: 0x0003D708 File Offset: 0x0003B908
	private void Update()
	{
		this.SetSize();
		this.SetSharderVars();
		if (this.currentSize > 120f)
		{
			this.t = false;
		}
		if (!this.t && this.currentSize < 120f)
		{
			this.t = true;
			for (int i = 0; i < this.fogStart.Length; i++)
			{
				this.fogStart[i].Play(default(Vector3));
			}
		}
		if (!this.t2 && this.REVEAL_AMOUNT > 0.1f)
		{
			this.t2 = true;
			for (int j = 0; j < this.fogReveal.Length; j++)
			{
				this.fogReveal[j].Play(default(Vector3));
			}
		}
		if (this.REVEAL_AMOUNT < 0.1f)
		{
			this.t2 = false;
		}
	}

	// Token: 0x06000C5C RID: 3164 RVA: 0x0003D7D4 File Offset: 0x0003B9D4
	private void SetSharderVars()
	{
		if (this.mpb == null)
		{
			this.mpb = new MaterialPropertyBlock();
		}
		this.rend.GetPropertyBlock(this.mpb);
		this.mpb.SetFloat("_PADDING", this.PADDING);
		this.mpb.SetFloat("_FogDepth", this.currentSize);
		this.mpb.SetFloat("_RevealAmount", this.REVEAL_AMOUNT);
		this.mpb.SetVector("_FogCenter", this.fogPoint);
		Shader.SetGlobalFloat("_FogSphereSize", this.currentSize);
		Shader.SetGlobalVector("FogCenter", this.fogPoint);
		Shader.SetGlobalFloat("FogEnabled", this.ENABLE);
		if (Application.isPlaying)
		{
			if (Character.localCharacter != null)
			{
				Character.localCharacter.data.isInFog = false;
				if (Mathf.Approximately(this.ENABLE, 1f) && Vector3.Distance(this.fogPoint, Character.localCharacter.Center) > this.currentSize)
				{
					Character.localCharacter.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Cold, 0.010500001f * Time.deltaTime, false);
					Character.localCharacter.data.isInFog = true;
				}
			}
			this.rend.SetPropertyBlock(this.mpb);
		}
	}

	// Token: 0x06000C5D RID: 3165 RVA: 0x0003D930 File Offset: 0x0003BB30
	private void SetSize()
	{
		float num = (this.currentSize + this.PADDING) * this.ratio;
		base.transform.localScale = Vector3.one * num;
	}

	// Token: 0x06000C5E RID: 3166 RVA: 0x0003D968 File Offset: 0x0003BB68
	private void OnDrawGizmosSelected()
	{
		Gizmos.DrawWireSphere(this.fogPoint, this.currentSize);
	}

	// Token: 0x04000B53 RID: 2899
	public float currentSize = 50f;

	// Token: 0x04000B54 RID: 2900
	[Range(0f, 1f)]
	public float ENABLE = 1f;

	// Token: 0x04000B55 RID: 2901
	[Range(0f, 1f)]
	public float REVEAL_AMOUNT;

	// Token: 0x04000B56 RID: 2902
	public float PADDING = 300f;

	// Token: 0x04000B57 RID: 2903
	public Vector3 fogPoint;

	// Token: 0x04000B58 RID: 2904
	private float ratio = 2f;

	// Token: 0x04000B59 RID: 2905
	private Renderer rend;

	// Token: 0x04000B5A RID: 2906
	public SFX_Instance[] fogStart;

	// Token: 0x04000B5B RID: 2907
	private bool t;

	// Token: 0x04000B5C RID: 2908
	public SFX_Instance[] fogReveal;

	// Token: 0x04000B5D RID: 2909
	private bool t2;

	// Token: 0x04000B5E RID: 2910
	private MaterialPropertyBlock mpb;
}
