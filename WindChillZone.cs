using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200008A RID: 138
public class WindChillZone : MonoBehaviour
{
	// Token: 0x060004CE RID: 1230 RVA: 0x0001BE89 File Offset: 0x0001A089
	private void Awake()
	{
		this.windZoneBounds.center = base.transform.position;
	}

	// Token: 0x060004CF RID: 1231 RVA: 0x0001BEA4 File Offset: 0x0001A0A4
	private void OnDrawGizmosSelected()
	{
		this.windZoneBounds.center = base.transform.position;
		Gizmos.color = this.gizmoColor;
		Gizmos.DrawCube(this.windZoneBounds.center, this.windZoneBounds.extents * 2f);
	}

	// Token: 0x060004D0 RID: 1232 RVA: 0x0001BEF7 File Offset: 0x0001A0F7
	private void Start()
	{
		this.view = base.GetComponent<PhotonView>();
	}

	// Token: 0x060004D1 RID: 1233 RVA: 0x0001BF08 File Offset: 0x0001A108
	private void Update()
	{
		if (Character.observedCharacter == null)
		{
			return;
		}
		this.HandleTime();
		this.characterInsideBounds = this.windZoneBounds.Contains(Character.observedCharacter.Center);
		if (this.windActive)
		{
			this.hasBeenActiveFor += Time.deltaTime;
		}
		else
		{
			this.hasBeenActiveFor = 0f;
		}
		if (this.characterInsideBounds && this.windActive)
		{
			if (Character.observedCharacter == Character.localCharacter)
			{
				this.ApplyCold();
				return;
			}
		}
		else
		{
			this.windPlayerFactor = 0f;
		}
	}

	// Token: 0x060004D2 RID: 1234 RVA: 0x0001BFA0 File Offset: 0x0001A1A0
	private void HandleTime()
	{
		this.untilSwitch -= Time.deltaTime;
		if (this.untilSwitch < 0f && PhotonNetwork.IsMasterClient)
		{
			this.view.RPC("RPCA_ToggleWind", RpcTarget.All, new object[]
			{
				!this.windActive,
				this.RandomWindDirection()
			});
			this.GetNextWindTime(this.windActive);
		}
	}

	// Token: 0x060004D3 RID: 1235 RVA: 0x0001C016 File Offset: 0x0001A216
	private void FixedUpdate()
	{
		if (Character.localCharacter == null)
		{
			return;
		}
		if (this.characterInsideBounds && this.windActive && Character.observedCharacter == Character.localCharacter)
		{
			this.AddWindForceToCharacter();
		}
	}

	// Token: 0x060004D4 RID: 1236 RVA: 0x0001C050 File Offset: 0x0001A250
	private void ApplyCold()
	{
		this.windPlayerFactor = WindChillZone.GetWindIntensityAtPoint(Character.localCharacter.Center, this.lightVolumeSampleThreshold_lower, this.lightVolumeSampleThreshold_margin);
		Character.localCharacter.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Cold, this.windPlayerFactor * this.windChillPerSecond * Time.deltaTime * Mathf.Clamp01(this.hasBeenActiveFor * 0.2f), false);
		if (this.setSlippy)
		{
			Character.localCharacter.data.slippy = Mathf.Clamp01(Mathf.Max(Character.localCharacter.data.slippy, this.windPlayerFactor * 10f));
		}
	}

	// Token: 0x060004D5 RID: 1237 RVA: 0x0001C0F6 File Offset: 0x0001A2F6
	private void AddWindForceToCharacter()
	{
		Character.localCharacter.AddForce(this.currentWindDirection * this.windForce * this.windPlayerFactor, 0.5f, 1f);
	}

	// Token: 0x060004D6 RID: 1238 RVA: 0x0001C128 File Offset: 0x0001A328
	private Vector3 RandomWindDirection()
	{
		return Vector3.Lerp(Vector3.right * ((Random.value > 0.5f) ? 1f : (-1f)), Vector3.forward, 0.2f).normalized;
	}

	// Token: 0x060004D7 RID: 1239 RVA: 0x0001C170 File Offset: 0x0001A370
	internal static float GetWindIntensityAtPoint(Vector3 point, float thresholdLower, float thresholdMargin)
	{
		float num = LightVolume.Instance().SamplePositionAlpha(point);
		float num2;
		if (num > thresholdLower + thresholdMargin)
		{
			num2 = 1f;
		}
		else if (num < thresholdLower)
		{
			num2 = 0f;
		}
		else
		{
			num2 = Util.RangeLerp(0f, 1f, thresholdLower, thresholdLower + thresholdMargin, num, true, null);
		}
		return num2;
	}

	// Token: 0x060004D8 RID: 1240 RVA: 0x0001C1C1 File Offset: 0x0001A3C1
	[PunRPC]
	private void RPCA_ToggleWind(bool set, Vector3 windDir)
	{
		this.windActive = set;
		this.untilSwitch = this.GetNextWindTime(this.windActive);
		this.currentWindDirection = windDir;
	}

	// Token: 0x060004D9 RID: 1241 RVA: 0x0001C1E3 File Offset: 0x0001A3E3
	private float GetNextWindTime(bool windActive)
	{
		if (windActive)
		{
			return Random.Range(this.windTimeRangeOn.x, this.windTimeRangeOn.y);
		}
		return Random.Range(this.windTimeRangeOff.x, this.windTimeRangeOff.y);
	}

	// Token: 0x04000505 RID: 1285
	public Vector2 windTimeRangeOn;

	// Token: 0x04000506 RID: 1286
	public Vector2 windTimeRangeOff;

	// Token: 0x04000507 RID: 1287
	[Range(0f, 1f)]
	public float lightVolumeSampleThreshold_lower;

	// Token: 0x04000508 RID: 1288
	[Range(0f, 1f)]
	public float lightVolumeSampleThreshold_margin;

	// Token: 0x04000509 RID: 1289
	public Bounds windZoneBounds;

	// Token: 0x0400050A RID: 1290
	internal Vector3 currentWindDirection;

	// Token: 0x0400050B RID: 1291
	private Color gizmoColor = new Color(0f, 0f, 1f, 0.5f);

	// Token: 0x0400050C RID: 1292
	private float untilSwitch;

	// Token: 0x0400050D RID: 1293
	public float windChillPerSecond = 0.01f;

	// Token: 0x0400050E RID: 1294
	public float windForce = 15f;

	// Token: 0x0400050F RID: 1295
	internal float hasBeenActiveFor;

	// Token: 0x04000510 RID: 1296
	private PhotonView view;

	// Token: 0x04000511 RID: 1297
	public bool characterInsideBounds;

	// Token: 0x04000512 RID: 1298
	public bool windActive;

	// Token: 0x04000513 RID: 1299
	public float windPlayerFactor;

	// Token: 0x04000514 RID: 1300
	public bool setSlippy;
}
