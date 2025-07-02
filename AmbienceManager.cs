using System;
using UnityEngine;

// Token: 0x0200003A RID: 58
[ExecuteInEditMode]
public class AmbienceManager : MonoBehaviour
{
	// Token: 0x060002E0 RID: 736 RVA: 0x00012A4F File Offset: 0x00010C4F
	private void Awake()
	{
		AmbienceManager.instance = this;
	}

	// Token: 0x060002E1 RID: 737 RVA: 0x00012A57 File Offset: 0x00010C57
	private void OnValidate()
	{
		if (!Application.isPlaying)
		{
			this.Start();
		}
	}

	// Token: 0x060002E2 RID: 738 RVA: 0x00012A66 File Offset: 0x00010C66
	private void UpdateFog()
	{
		if (Application.isPlaying)
		{
			this.useFog = true;
		}
		Shader.SetGlobalFloat(AmbienceManager.Usefog, (float)(this.useFog ? 1 : 0));
		Shader.SetGlobalFloat(AmbienceManager.Maxfog, this.maxFog);
	}

	// Token: 0x060002E3 RID: 739 RVA: 0x00012A9D File Offset: 0x00010C9D
	private void Start()
	{
		this.UpdateFog();
	}

	// Token: 0x060002E4 RID: 740 RVA: 0x00012AA8 File Offset: 0x00010CA8
	private void Update()
	{
		this.UpdateFog();
		RenderSettings.skybox = this.skyboxMaterial;
		RenderSettings.fogColor = this.fogColor;
		if (!this.dayNight)
		{
			return;
		}
		Color color = this.ambienceGradient.Evaluate(this.dayNight.timeOfDayNormalized);
		RenderSettings.ambientLight = color * this.brightness * color.a;
	}

	// Token: 0x060002E5 RID: 741 RVA: 0x00012B12 File Offset: 0x00010D12
	public void ToggleFog()
	{
		this.useFog = !this.useFog;
		Shader.SetGlobalFloat(AmbienceManager.Usefog, (float)(this.useFog ? 1 : 0));
	}

	// Token: 0x04000379 RID: 889
	private static readonly int Maxfog = Shader.PropertyToID("MAXFOG");

	// Token: 0x0400037A RID: 890
	private static readonly int Usefog = Shader.PropertyToID("USEFOG");

	// Token: 0x0400037B RID: 891
	public Color ambienceColor;

	// Token: 0x0400037C RID: 892
	public Gradient ambienceGradient;

	// Token: 0x0400037D RID: 893
	public Color fogColor;

	// Token: 0x0400037E RID: 894
	public float brightness = 1f;

	// Token: 0x0400037F RID: 895
	public Material skyboxMaterial;

	// Token: 0x04000380 RID: 896
	public DayNightManager dayNight;

	// Token: 0x04000381 RID: 897
	public float maxFog = 500f;

	// Token: 0x04000382 RID: 898
	public bool useFog = true;

	// Token: 0x04000383 RID: 899
	public static AmbienceManager instance;
}
