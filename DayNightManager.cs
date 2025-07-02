using System;
using Photon.Pun;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x020001B6 RID: 438
public class DayNightManager : MonoBehaviour
{
	// Token: 0x170000AB RID: 171
	// (get) Token: 0x06000BFE RID: 3070 RVA: 0x0003BFBB File Offset: 0x0003A1BB
	public float timeOfDayNormalized
	{
		get
		{
			return this.timeOfDay % 24f / 24f;
		}
	}

	// Token: 0x170000AC RID: 172
	// (get) Token: 0x06000BFF RID: 3071 RVA: 0x0003BFCF File Offset: 0x0003A1CF
	public float isDay
	{
		get
		{
			return (float)((this.timeOfDay >= this.dayStart && this.timeOfDay < this.dayEnd) ? 1 : 0);
		}
	}

	// Token: 0x06000C00 RID: 3072 RVA: 0x0003BFF2 File Offset: 0x0003A1F2
	private void Awake()
	{
		DayNightManager.instance = this;
	}

	// Token: 0x06000C01 RID: 3073 RVA: 0x0003BFFC File Offset: 0x0003A1FC
	private void Start()
	{
		this.timeOfDay = this.startingTimeOfDay;
		this.UpdateCycle();
		this.photonView = base.GetComponent<PhotonView>();
		float num = (this.dayEnd - this.dayStart) / 24f;
		float num2 = 1f - num;
		this.dayNightRatio = num / num2;
	}

	// Token: 0x06000C02 RID: 3074 RVA: 0x0003C04C File Offset: 0x0003A24C
	public void setTimeOfDay(float timeToSet)
	{
		if (timeToSet > 48f)
		{
			this.timeOfDay = 48f;
		}
		this.timeOfDay = timeToSet;
	}

	// Token: 0x06000C03 RID: 3075 RVA: 0x0003C068 File Offset: 0x0003A268
	private void Update()
	{
		this.timeOfDay += 1f / (this.dayLengthInMinutes * 60f) * Time.deltaTime * 24f;
		if (this.timeOfDay > 24f)
		{
			this.timeOfDay -= 24f;
			this.passedMidnight = true;
		}
		if (this.passedMidnight && this.timeOfDay >= 5.5f)
		{
			this.dayCount++;
			this.passedMidnight = false;
		}
		if (PhotonNetwork.IsMasterClient)
		{
			this.syncTimer += Time.deltaTime;
			if (this.syncTimer > 5f)
			{
				this.photonView.RPC("RPCA_SyncTime", RpcTarget.All, new object[] { this.timeOfDay });
				this.syncTimer = 0f;
			}
		}
		this.UpdateCycle();
	}

	// Token: 0x06000C04 RID: 3076 RVA: 0x0003C14C File Offset: 0x0003A34C
	public string DayCountString()
	{
		return "Day " + DayNightManager.IntToNumberWord(DayNightManager.instance.dayCount);
	}

	// Token: 0x06000C05 RID: 3077 RVA: 0x0003C168 File Offset: 0x0003A368
	public string TimeOfDayString()
	{
		if (this.timeOfDay >= 23.5f)
		{
			return "night";
		}
		if (this.timeOfDay >= 17.5f)
		{
			return "evening";
		}
		if (this.timeOfDay >= 11.5f)
		{
			return "afternoon";
		}
		if (this.timeOfDay >= 5.5f)
		{
			return "morning";
		}
		return "night";
	}

	// Token: 0x06000C06 RID: 3078 RVA: 0x0003C1C8 File Offset: 0x0003A3C8
	private static string IntToNumberWord(int x)
	{
		if (x == 1)
		{
			return "One";
		}
		if (x == 2)
		{
			return "Two";
		}
		if (x == 3)
		{
			return "Three";
		}
		if (x == 4)
		{
			return "Four";
		}
		if (x == 5)
		{
			return "Five";
		}
		if (x == 6)
		{
			return "Six";
		}
		if (x == 2)
		{
			return "Seven";
		}
		if (x == 3)
		{
			return "Eight";
		}
		if (x == 4)
		{
			return "Nine";
		}
		if (x == 5)
		{
			return "Ten";
		}
		return x.ToString() ?? "";
	}

	// Token: 0x06000C07 RID: 3079 RVA: 0x0003C24C File Offset: 0x0003A44C
	public string FloatToTimeString(float time)
	{
		time = Mathf.Clamp(time, 0f, 24f);
		int num = Mathf.FloorToInt(time);
		int num2 = Mathf.FloorToInt((time - (float)num) * 60f);
		string text = ((num < 12) ? "AM" : "PM");
		int num3 = ((num % 12 == 0) ? 12 : (num % 12));
		return string.Format("{0:D2}:{1:D2} {2}", num3, num2, text);
	}

	// Token: 0x06000C08 RID: 3080 RVA: 0x0003C2BA File Offset: 0x0003A4BA
	[PunRPC]
	public void RPCA_SyncTime(float time)
	{
		this.timeOfDay = time;
	}

	// Token: 0x06000C09 RID: 3081 RVA: 0x0003C2C3 File Offset: 0x0003A4C3
	private void OnValidate()
	{
		this.UpdateCycle();
	}

	// Token: 0x06000C0A RID: 3082 RVA: 0x0003C2CC File Offset: 0x0003A4CC
	public void UpdateCycle()
	{
		this.timeString = this.FloatToTimeString(this.timeOfDay);
		float timeOfDayNormalized = this.timeOfDayNormalized;
		Vector3 vector = this.highNoonRotation + new Vector3(0f, 0f, this.angleOffsetZ.Evaluate(timeOfDayNormalized));
		float num = timeOfDayNormalized;
		if (this.isDay < 0.5f)
		{
			if (num > this.dayEnd / 24f)
			{
				num = this.dayEnd / 24f - (num - this.dayEnd / 24f) * this.dayNightRatio;
			}
			else if (num < this.dayStart / 24f)
			{
				num = this.dayStart / 24f + (this.dayStart / 24f - num) * this.dayNightRatio;
			}
		}
		Vector3 vector2 = new Vector3((num * this.rotDir - 0.5f) * 360f, 0f, 0f);
		this.earth.transform.rotation = Quaternion.Euler(vector) * Quaternion.Euler(vector2);
		Color color = Color.Lerp(this.sunGradient.Evaluate(timeOfDayNormalized), this.specialSunColor, this.specialDayIntensity);
		Color color2 = Color.Lerp(this.skyTopGradient.Evaluate(timeOfDayNormalized), this.specialTopColor, this.specialDayIntensity);
		Color color3 = Color.Lerp(this.skyMidGradient.Evaluate(timeOfDayNormalized), this.specialMidColor, this.specialDayIntensity);
		Color color4 = Color.Lerp(this.skyBottomGradient.Evaluate(timeOfDayNormalized), this.specialBottomColor, this.specialDayIntensity);
		Shader.SetGlobalColor(DayNightManager.SkyTopColor, color2);
		Shader.SetGlobalColor(DayNightManager.SkyMidColor, color3);
		Shader.SetGlobalColor(DayNightManager.SkyBottomColor, color4);
		Shader.SetGlobalFloat(DayNightManager.TIMEOFDAY, timeOfDayNormalized);
		Shader.SetGlobalFloat(DayNightManager.Name, this.isDay);
		Shader.SetGlobalFloat(DayNightManager.FOG, this.fogGradient.Evaluate(timeOfDayNormalized).r);
		this.sun.color = color;
		this.moon.color = color;
		float num2 = -(this.snowstormWindFactor * 1.75f + this.rainstormWindFactor * 1.25f);
		this.sun.intensity = Mathf.Max(0.015f, (color.a * 2f - 1f) * 0.5f * this.sunIntensity + num2);
		this.moon.intensity = Mathf.Max(0.015f, (1f - color.a * 2f) * 0.5f * this.moonIntensity + num2);
		this.lensFlare.intensity = (color.a - 0.5f) * 2f + num2;
		if (color.a < 0.5f)
		{
			this.sun.enabled = false;
			this.moon.enabled = true;
			Shader.SetGlobalInt(DayNightManager.IsDayReal, 0);
		}
		else
		{
			this.moon.enabled = false;
			this.sun.enabled = true;
			Shader.SetGlobalInt(DayNightManager.IsDayReal, 1);
		}
		this.sun.shadowStrength = math.saturate(this.sunIntensity);
	}

	// Token: 0x06000C0B RID: 3083 RVA: 0x0003C5DD File Offset: 0x0003A7DD
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawLine(this.earth.transform.position, this.sun.transform.position);
	}

	// Token: 0x04000AEA RID: 2794
	public static DayNightManager instance;

	// Token: 0x04000AEB RID: 2795
	private static readonly int TIMEOFDAY = Shader.PropertyToID("_TimeOfDay");

	// Token: 0x04000AEC RID: 2796
	private static readonly int FOG = Shader.PropertyToID("EXTRAFOG");

	// Token: 0x04000AED RID: 2797
	private static readonly int Name = Shader.PropertyToID("IsDay");

	// Token: 0x04000AEE RID: 2798
	private static readonly int IsDayReal = Shader.PropertyToID("IsDayReal");

	// Token: 0x04000AEF RID: 2799
	private static readonly int SkyTopColor = Shader.PropertyToID("SkyTopColor");

	// Token: 0x04000AF0 RID: 2800
	private static readonly int SkyMidColor = Shader.PropertyToID("SkyMidColor");

	// Token: 0x04000AF1 RID: 2801
	private static readonly int SkyBottomColor = Shader.PropertyToID("SkyBottomColor");

	// Token: 0x04000AF2 RID: 2802
	[Range(0f, 48f)]
	public float timeOfDay;

	// Token: 0x04000AF3 RID: 2803
	public float dayLengthInMinutes = 10f;

	// Token: 0x04000AF4 RID: 2804
	public float startingTimeOfDay = 9f;

	// Token: 0x04000AF5 RID: 2805
	public float dayStart = 5f;

	// Token: 0x04000AF6 RID: 2806
	public float dayEnd = 21f;

	// Token: 0x04000AF7 RID: 2807
	public int dayCount = 1;

	// Token: 0x04000AF8 RID: 2808
	public LensFlareComponentSRP lensFlare;

	// Token: 0x04000AF9 RID: 2809
	public AnimationCurve angleOffsetZ;

	// Token: 0x04000AFA RID: 2810
	public Vector3 highNoonRotation;

	// Token: 0x04000AFB RID: 2811
	public Transform earth;

	// Token: 0x04000AFC RID: 2812
	public Light sun;

	// Token: 0x04000AFD RID: 2813
	public Light moon;

	// Token: 0x04000AFE RID: 2814
	public float sunIntensity;

	// Token: 0x04000AFF RID: 2815
	public float moonIntensity;

	// Token: 0x04000B00 RID: 2816
	public Gradient sunGradient;

	// Token: 0x04000B01 RID: 2817
	public Gradient skyTopGradient;

	// Token: 0x04000B02 RID: 2818
	public Gradient skyMidGradient;

	// Token: 0x04000B03 RID: 2819
	public Gradient skyBottomGradient;

	// Token: 0x04000B04 RID: 2820
	public Gradient fogGradient;

	// Token: 0x04000B05 RID: 2821
	[Header("Special Day")]
	[Range(0f, 1f)]
	public float specialDayIntensity;

	// Token: 0x04000B06 RID: 2822
	public Color specialSunColor;

	// Token: 0x04000B07 RID: 2823
	public Color specialTopColor;

	// Token: 0x04000B08 RID: 2824
	public Color specialMidColor;

	// Token: 0x04000B09 RID: 2825
	public Color specialBottomColor;

	// Token: 0x04000B0A RID: 2826
	public string timeString;

	// Token: 0x04000B0B RID: 2827
	public float rotDir = 1f;

	// Token: 0x04000B0C RID: 2828
	public float snowstormWindFactor;

	// Token: 0x04000B0D RID: 2829
	public float rainstormWindFactor;

	// Token: 0x04000B0E RID: 2830
	private PhotonView photonView;

	// Token: 0x04000B0F RID: 2831
	public float dayNightRatio = 2f;

	// Token: 0x04000B10 RID: 2832
	public float syncTimer;

	// Token: 0x04000B11 RID: 2833
	private bool passedMidnight;
}
