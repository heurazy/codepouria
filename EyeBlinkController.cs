using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

// Token: 0x0200008C RID: 140
public class EyeBlinkController : MonoBehaviour
{
	// Token: 0x060004DF RID: 1247 RVA: 0x0001C354 File Offset: 0x0001A554
	private void Start()
	{
		this.character = base.GetComponentInParent<Character>();
		if (!this.character.IsLocal)
		{
			base.enabled = false;
			return;
		}
		foreach (ScriptableRendererFeature scriptableRendererFeature in this.rend.rendererFeatures)
		{
			if (scriptableRendererFeature.name == "Eye Blink")
			{
				this.rendererFeature = scriptableRendererFeature;
			}
		}
		this.setEyeBlinkActive();
	}

	// Token: 0x060004E0 RID: 1248 RVA: 0x0001C3E8 File Offset: 0x0001A5E8
	private void setEyeBlinkActive()
	{
		if (!this.character.IsLocal)
		{
			return;
		}
		this.eyeBlinkMaterial.SetFloat("_EyeOpen", (float)(this.enableEyeBlink ? 1 : 0));
		if (!this.enableEyeBlink)
		{
			this.eyeBlinkMaterial.SetFloat("_EyeOpen", 1f);
		}
	}

	// Token: 0x060004E1 RID: 1249 RVA: 0x0001C440 File Offset: 0x0001A640
	private void Update()
	{
		if (!this.character.IsLocal)
		{
			return;
		}
		if (this.character.data.passedOutOnTheBeach > 0f)
		{
			this.eyeOpenValue = 0f;
			this.enableEyeBlink = true;
		}
		else
		{
			this.eyeOpenValue = Mathf.MoveTowards(this.eyeOpenValue, 1f, Time.deltaTime * 0.15f);
			if (this.eyeOpenValue >= 0.999f)
			{
				this.enableEyeBlink = false;
			}
			else
			{
				this.enableEyeBlink = true;
			}
		}
		if (this.enableEyeBlink)
		{
			this.eyeBlinkMaterial.SetFloat("_EyeOpen", Mathf.Clamp01(this.openCurve.Evaluate(this.eyeOpenValue)));
		}
	}

	// Token: 0x0400051B RID: 1307
	private Character character;

	// Token: 0x0400051C RID: 1308
	public UniversalRendererData rend;

	// Token: 0x0400051D RID: 1309
	public Material eyeBlinkMaterial;

	// Token: 0x0400051E RID: 1310
	public bool enableEyeBlink;

	// Token: 0x0400051F RID: 1311
	public AnimationCurve openCurve;

	// Token: 0x04000520 RID: 1312
	[Range(0f, 1f)]
	public float eyeOpenValue;

	// Token: 0x04000521 RID: 1313
	private ScriptableRendererFeature rendererFeature;
}
