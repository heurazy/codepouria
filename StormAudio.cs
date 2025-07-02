using System;
using UnityEngine;

// Token: 0x0200027F RID: 639
public class StormAudio : MonoBehaviour
{
	// Token: 0x06000F59 RID: 3929 RVA: 0x0004DA64 File Offset: 0x0004BC64
	private void Start()
	{
		GameObject gameObject = GameObject.FindGameObjectWithTag("Storm");
		GameObject gameObject2 = GameObject.FindGameObjectWithTag("Rain");
		if (gameObject == null || gameObject2 == null)
		{
			base.enabled = false;
			return;
		}
		this.stormVisual = gameObject.GetComponent<StormVisual>();
		this.rainVisual = gameObject2.GetComponent<StormVisual>();
	}

	// Token: 0x06000F5A RID: 3930 RVA: 0x0004DAB9 File Offset: 0x0004BCB9
	private void Update()
	{
		this.StormPlay(this.stormVisual, this.loopStorm, this.lPStorm);
		this.RainPlay();
	}

	// Token: 0x06000F5B RID: 3931 RVA: 0x0004DADC File Offset: 0x0004BCDC
	private void RainPlay()
	{
		this.loopRainHeavy.volume = Mathf.Lerp(this.loopRainHeavy.volume, 0f, Time.deltaTime * 0.25f);
		this.loopRainSoft.volume = Mathf.Lerp(this.loopRainSoft.volume, 0f, Time.deltaTime * 0.05f);
		if (this.stormVisual == null)
		{
			return;
		}
		if (this.rainVisual)
		{
			if (!this.rainVisual.playerInWindZone)
			{
				this.loopRainHeavy.volume = Mathf.Lerp(this.loopRainHeavy.volume, 0f, Time.deltaTime * 0.25f);
				this.loopRainSoft.volume = Mathf.Lerp(this.loopRainSoft.volume, 0f, Time.deltaTime * 0.05f);
			}
			if (this.rainVisual.playerInWindZone && this.aM)
			{
				if (this.aM.obstruction < 0.6f)
				{
					this.loopRainHeavy.volume = Mathf.Lerp(this.loopRainHeavy.volume, 0.25f, Time.deltaTime * 2f);
					this.loopRainSoft.volume = Mathf.Lerp(this.loopRainSoft.volume, 0.005f, Time.deltaTime * 2f);
				}
				if (this.aM.obstruction >= 0.6f)
				{
					this.loopRainHeavy.volume = Mathf.Lerp(this.loopRainHeavy.volume, 0.15f, Time.deltaTime * 2f);
					this.loopRainSoft.volume = Mathf.Lerp(this.loopRainSoft.volume, 0.25f, Time.deltaTime * 2f);
				}
			}
		}
	}

	// Token: 0x06000F5C RID: 3932 RVA: 0x0004DCB4 File Offset: 0x0004BEB4
	private void StormPlay(StormVisual sV, AudioLoop aL, AudioLowPassFilter lFilter)
	{
		if (sV && aL && lFilter)
		{
			if (!sV.playerInWindZone)
			{
				aL.volume = Mathf.Lerp(aL.volume, 0f, Time.deltaTime * 0.25f);
				aL.pitch = Mathf.Lerp(aL.pitch, 0.25f, Time.deltaTime * 0.25f);
				lFilter.cutoffFrequency = Mathf.Lerp(lFilter.cutoffFrequency, 8000f, Time.deltaTime * 1f);
			}
			if (sV.playerInWindZone)
			{
				aL.pitch = Mathf.Lerp(aL.pitch, 1f, Time.deltaTime * 0.25f);
				if (this.aM.obstruction >= 0.6f)
				{
					lFilter.cutoffFrequency = Mathf.Lerp(lFilter.cutoffFrequency, 500f, Time.deltaTime * 0.25f);
					aL.volume = Mathf.Lerp(aL.volume, 0.05f, Time.deltaTime * 0.25f);
					return;
				}
				lFilter.cutoffFrequency = Mathf.Lerp(lFilter.cutoffFrequency, 8000f, Time.deltaTime * 1f);
				aL.volume = Mathf.Lerp(aL.volume, 0.25f, Time.deltaTime * 0.25f);
			}
		}
	}

	// Token: 0x04000E5E RID: 3678
	public AmbienceAudio aM;

	// Token: 0x04000E5F RID: 3679
	public AudioLoop loopStorm;

	// Token: 0x04000E60 RID: 3680
	public AudioLowPassFilter lPStorm;

	// Token: 0x04000E61 RID: 3681
	public AudioLoop loopRainHeavy;

	// Token: 0x04000E62 RID: 3682
	public AudioLoop loopRainSoft;

	// Token: 0x04000E63 RID: 3683
	public StormVisual stormVisual;

	// Token: 0x04000E64 RID: 3684
	public StormVisual rainVisual;
}
