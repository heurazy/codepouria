using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200028D RID: 653
public class Transition_Shader : Transition
{
	// Token: 0x06000FA4 RID: 4004 RVA: 0x0004F48F File Offset: 0x0004D68F
	private void Awake()
	{
		this.rend = base.GetComponent<MeshRenderer>();
		this.mat = Object.Instantiate<Material>(this.rend.sharedMaterial);
		this.rend.sharedMaterial = this.mat;
	}

	// Token: 0x06000FA5 RID: 4005 RVA: 0x0004F4C4 File Offset: 0x0004D6C4
	public override IEnumerator TransitionIn(float speed = 1f)
	{
		float c = 0f;
		float t = this.inCurve.keys[this.inCurve.keys.Length - 1].time;
		while (c < t)
		{
			c += Time.unscaledDeltaTime * speed * this.inSpeed;
			this.mat.SetFloat("_Progress", c);
			this.mat.SetInt("_In", 1);
			yield return null;
		}
		yield break;
	}

	// Token: 0x06000FA6 RID: 4006 RVA: 0x0004F4DA File Offset: 0x0004D6DA
	public override IEnumerator TransitionOut(float speed = 1f)
	{
		float c = 0f;
		float t = this.outCurve.keys[this.outCurve.keys.Length - 1].time;
		while (c < t)
		{
			c += Time.unscaledDeltaTime * speed * this.outSpeed;
			this.mat.SetFloat("_Progress", c);
			this.mat.SetInt("_In", 0);
			yield return null;
		}
		yield break;
	}

	// Token: 0x04000EAB RID: 3755
	private MeshRenderer rend;

	// Token: 0x04000EAC RID: 3756
	private Material mat;

	// Token: 0x04000EAD RID: 3757
	public float inSpeed = 1f;

	// Token: 0x04000EAE RID: 3758
	public AnimationCurve inCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

	// Token: 0x04000EAF RID: 3759
	public float outSpeed = 1f;

	// Token: 0x04000EB0 RID: 3760
	public AnimationCurve outCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
}
