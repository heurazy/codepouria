using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

// Token: 0x020000EE RID: 238
[ExecuteInEditMode]
public class LogoCombineSequence : MonoBehaviour
{
	// Token: 0x06000725 RID: 1829 RVA: 0x00025B9C File Offset: 0x00023D9C
	private void Start()
	{
		if (this.volume.profile.TryGet<ChromaticAberration>(out this.chromaticAberration))
		{
			this.chromaticAberration.intensity.value = 0f;
		}
		if (this.volume.profile.TryGet<Bloom>(out this.bloom))
		{
			this.bloom.intensity.value = 0f;
		}
		if (this.volume.profile.TryGet<LensDistortion>(out this.lensDistortion))
		{
			this.lensDistortion.intensity.value = 0f;
		}
	}

	// Token: 0x06000726 RID: 1830 RVA: 0x00025C30 File Offset: 0x00023E30
	private void Update()
	{
		if (this.chromaticAberration != null)
		{
			this.chromaticAberration.intensity.value = this.chromaticAmplitude;
		}
		if (this.bloom != null)
		{
			this.bloom.intensity.value = this.bloomIntensity;
		}
		if (this.lensDistortion != null)
		{
			this.lensDistortion.intensity.value = this.lensIntensity;
			this.lensDistortion.scale.value = this.lensScale;
		}
		this.material.SetFloat("_StreakAmount", this.streakAmount);
		this.material.SetFloat("_StretchAmount", this.stretchAmount);
	}

	// Token: 0x06000727 RID: 1831 RVA: 0x00025CEC File Offset: 0x00023EEC
	private void OnValidate()
	{
		if (this.bloom != null)
		{
			this.bloom.intensity.value = this.bloomIntensity;
		}
		if (this.chromaticAberration != null)
		{
			this.chromaticAberration.intensity.value = this.chromaticAmplitude;
		}
		if (this.lensDistortion != null)
		{
			this.lensDistortion.intensity.value = this.lensIntensity;
			this.lensDistortion.scale.value = this.lensScale;
		}
		this.material.SetFloat("_StreakAmount", this.streakAmount);
		this.material.SetFloat("_StretchAmount", this.stretchAmount);
	}

	// Token: 0x040006B5 RID: 1717
	public float streakAmount;

	// Token: 0x040006B6 RID: 1718
	public float stretchAmount;

	// Token: 0x040006B7 RID: 1719
	public float chromaticAmplitude;

	// Token: 0x040006B8 RID: 1720
	public float lensScale;

	// Token: 0x040006B9 RID: 1721
	public float lensIntensity;

	// Token: 0x040006BA RID: 1722
	public float bloomIntensity;

	// Token: 0x040006BB RID: 1723
	public Material material;

	// Token: 0x040006BC RID: 1724
	public Volume volume;

	// Token: 0x040006BD RID: 1725
	private ChromaticAberration chromaticAberration;

	// Token: 0x040006BE RID: 1726
	private Bloom bloom;

	// Token: 0x040006BF RID: 1727
	private LensDistortion lensDistortion;
}
