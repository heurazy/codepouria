using System;
using UnityEngine;

// Token: 0x02000141 RID: 321
public class SpecialDayManager : MonoBehaviour
{
	// Token: 0x06000936 RID: 2358 RVA: 0x0002E8BA File Offset: 0x0002CABA
	private void Start()
	{
		this.zones = Object.FindObjectsByType<SpecialDayZone>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
		this.startFog = AmbienceManager.instance.maxFog;
	}

	// Token: 0x06000937 RID: 2359 RVA: 0x0002E8DC File Offset: 0x0002CADC
	private void Update()
	{
		float num = 0f;
		if (!Character.observedCharacter)
		{
			return;
		}
		for (int i = 0; i < this.zones.Length; i++)
		{
			if (this.zones[i].outerBounds.Contains(Character.observedCharacter.Center))
			{
				Color specialSunColor = DayNightManager.instance.specialSunColor;
				Color specialTopColor = DayNightManager.instance.specialTopColor;
				Color specialMidColor = DayNightManager.instance.specialMidColor;
				Color specialBottomColor = DayNightManager.instance.specialBottomColor;
				float maxFog = AmbienceManager.instance.maxFog;
				float num2 = Vector3.Distance(Character.observedCharacter.Center, this.zones[i].bounds.ClosestPoint(Character.observedCharacter.Center));
				num2 /= this.zones[i].blendSize;
				float num3 = 1f - num2 * 2f;
				DayNightManager.instance.specialDayIntensity = Mathf.Max(num, num3);
				DayNightManager.instance.specialSunColor = Color.Lerp(specialSunColor, this.zones[i].specialSunColor, num3);
				if (this.zones[i].specialLight != null)
				{
					Color color = Color.Lerp(specialSunColor, this.zones[i].specialSunColor, num3);
					color *= num3;
					this.zones[i].specialLight.color = color;
					DayNightManager.instance.specialSunColor *= 1f - num3;
				}
				Shader.SetGlobalFloat("SpecialDayBlend", Mathf.Lerp(0f, 1f, num3));
				DayNightManager.instance.specialTopColor = Color.Lerp(specialTopColor, this.zones[i].specialTopColor, num3);
				DayNightManager.instance.specialMidColor = Color.Lerp(specialMidColor, this.zones[i].specialMidColor, num3);
				DayNightManager.instance.specialBottomColor = Color.Lerp(specialBottomColor, this.zones[i].specialBottomColor, num3);
				float num4 = Mathf.Lerp(maxFog, this.zones[i].fogDensity, num3);
				num4 = Mathf.Lerp(this.startFog, num4, Mathf.Max(num, num3));
				AmbienceManager.instance.maxFog = num4;
				if (this.zones[i].globalShaderVals.Length != 0)
				{
					float num5 = 0f;
					for (int j = 0; j < this.zones[i].globalShaderVals.Length; j++)
					{
						float num6 = Mathf.Lerp(num5, this.zones[i].globalShaderVals[j].value, num3);
						num6 *= Mathf.Max(num, num3);
						Shader.SetGlobalFloat(this.zones[i].globalShaderVals[j].floatName, num6);
						num5 = num6;
					}
				}
				num = num3;
			}
		}
	}

	// Token: 0x06000938 RID: 2360 RVA: 0x0002EB9C File Offset: 0x0002CD9C
	private void OnDisable()
	{
		Shader.SetGlobalFloat("SpecialDayBlend", 0f);
		for (int i = 0; i < this.zones.Length; i++)
		{
			if (this.zones[i].globalShaderVals.Length != 0)
			{
				for (int j = 0; j < this.zones[i].globalShaderVals.Length; j++)
				{
					Shader.SetGlobalFloat(this.zones[i].globalShaderVals[j].floatName, 0f);
				}
			}
		}
	}

	// Token: 0x04000835 RID: 2101
	public SpecialDayZone[] zones;

	// Token: 0x04000836 RID: 2102
	private float startFog;
}
