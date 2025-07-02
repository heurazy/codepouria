using System;
using UnityEngine;

// Token: 0x020001CB RID: 459
[ExecuteInEditMode]
public class FogConfig : MonoBehaviour
{
	// Token: 0x06000C53 RID: 3155 RVA: 0x0003D399 File Offset: 0x0003B599
	private void Start()
	{
		Shader.SetGlobalFloat("_WeatherBlend", 0f);
	}

	// Token: 0x06000C54 RID: 3156 RVA: 0x0003D3AC File Offset: 0x0003B5AC
	private void Update()
	{
		this.sinceSet += Time.deltaTime;
		if (FogConfig.currentFog == this && this.sinceSet > 0.1f && this.sinceSet < 10f)
		{
			float num = Shader.GetGlobalFloat("_WeatherBlend");
			if (num > 0f)
			{
				num = Mathf.MoveTowards(num, 0f, Time.deltaTime * 0.3f);
				Shader.SetGlobalFloat("_WeatherBlend", num);
			}
		}
	}

	// Token: 0x06000C55 RID: 3157 RVA: 0x0003D428 File Offset: 0x0003B628
	public void SetFog()
	{
		FogConfig.currentFog = this;
		this.sinceSet = 0f;
		Shader.SetGlobalTexture("_WindTexture", this.windTexture);
		float num = Shader.GetGlobalFloat("_WeatherBlend");
		num = Mathf.MoveTowards(num, this.maxVal, Time.deltaTime * 0.3f);
		Shader.SetGlobalFloat("_WeatherBlend", num);
		Shader.SetGlobalColor("WindTint", this.windTint);
		Shader.SetGlobalFloat("WindSkyBrightnessValue", this.windSkyBrightnessValue);
		Shader.SetGlobalFloat("WindTextureInfluence", this.windTextureInfluence);
		Shader.SetGlobalFloat("WindFogDensity", this.windFogDensity);
		Shader.SetGlobalFloat("WindFogTextureDensity", this.WindFogTextureDensity);
		Shader.SetGlobalFloat("WindMixInfluence", this.windMixInfluence);
		Shader.SetGlobalVector("WindSpeed", new Vector4(this.windSpeed.x, this.windSpeed.y, 0f, 0f));
		Vector3 forward = base.transform.forward;
		Vector3 vector = -Vector3.Cross(Vector3.up, forward);
		float num2 = Vector3.Angle(Vector3.up, forward);
		if (this.straightDown)
		{
			vector = Vector3.forward;
			num2 = 180f;
		}
		Shader.SetGlobalVector("WindRotationAxis", new Vector4(vector.x, vector.y, vector.z, 0f));
		Shader.SetGlobalFloat("WindRotationAngle", num2);
		Shader.SetGlobalFloat("WindSphereScale", this.windSphereScale);
	}

	// Token: 0x04000B42 RID: 2882
	public static FogConfig currentFog;

	// Token: 0x04000B43 RID: 2883
	public float windSkyBrightnessValue = 0.2f;

	// Token: 0x04000B44 RID: 2884
	public float windTextureInfluence = 0.2f;

	// Token: 0x04000B45 RID: 2885
	public Color windTint = Color.white;

	// Token: 0x04000B46 RID: 2886
	public Texture windTexture;

	// Token: 0x04000B47 RID: 2887
	public float windSphereScale = 5f;

	// Token: 0x04000B48 RID: 2888
	public Vector2 windSpeed;

	// Token: 0x04000B49 RID: 2889
	public float windFogDensity = 50f;

	// Token: 0x04000B4A RID: 2890
	public float WindFogTextureDensity = 15f;

	// Token: 0x04000B4B RID: 2891
	public float windMixInfluence;

	// Token: 0x04000B4C RID: 2892
	public float maxVal = 1f;

	// Token: 0x04000B4D RID: 2893
	public bool straightDown;

	// Token: 0x04000B4E RID: 2894
	private float sinceSet = 10f;
}
