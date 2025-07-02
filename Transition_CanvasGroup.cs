using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200028C RID: 652
public class Transition_CanvasGroup : Transition
{
	// Token: 0x06000FA0 RID: 4000 RVA: 0x0004F3EE File Offset: 0x0004D5EE
	private void Awake()
	{
		this.gr = base.GetComponent<CanvasGroup>();
	}

	// Token: 0x06000FA1 RID: 4001 RVA: 0x0004F3FC File Offset: 0x0004D5FC
	public override IEnumerator TransitionIn(float speed = 1f)
	{
		float c = 0f;
		float t = this.inCurve.keys[this.inCurve.keys.Length - 1].time;
		while (c < t)
		{
			c += Time.unscaledDeltaTime * speed * this.inSpeed;
			this.gr.alpha = this.inCurve.Evaluate(c);
			yield return null;
		}
		yield break;
	}

	// Token: 0x06000FA2 RID: 4002 RVA: 0x0004F412 File Offset: 0x0004D612
	public override IEnumerator TransitionOut(float speed = 1f)
	{
		float c = 0f;
		float t = this.outCurve.keys[this.outCurve.keys.Length - 1].time;
		while (c < t)
		{
			c += Time.unscaledDeltaTime * speed * this.outSpeed;
			this.gr.alpha = this.outCurve.Evaluate(c);
			yield return null;
		}
		yield break;
	}

	// Token: 0x04000EA6 RID: 3750
	private CanvasGroup gr;

	// Token: 0x04000EA7 RID: 3751
	public float inSpeed = 1f;

	// Token: 0x04000EA8 RID: 3752
	public AnimationCurve inCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

	// Token: 0x04000EA9 RID: 3753
	public float outSpeed = 1f;

	// Token: 0x04000EAA RID: 3754
	public AnimationCurve outCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
}
