using System;
using Unity.Mathematics;
using UnityEngine;

// Token: 0x02000183 RID: 387
public class AmbienceAudio : MonoBehaviour
{
	// Token: 0x06000ABF RID: 2751 RVA: 0x00034564 File Offset: 0x00032764
	private void Start()
	{
		this.ambienceVolumes = base.GetComponent<Animator>();
		this.dayNight = Object.FindAnyObjectByType<DayNightManager>();
		this.stingerSource.clip = this.startStinger[global::UnityEngine.Random.Range(0, this.startStinger.Length)];
		this.stingerSource.Play();
		this.volcanoObj = GameObject.Find("VolcanoModel");
		if (GameObject.Find("Airport"))
		{
			base.gameObject.SetActive(false);
			if (this.voice)
			{
				this.reverbFilter.enabled = false;
				this.echoFilter.enabled = false;
				this.lowPassFilter.enabled = false;
			}
		}
	}

	// Token: 0x06000AC0 RID: 2752 RVA: 0x00034614 File Offset: 0x00032814
	private void FixedUpdate()
	{
		this.naturelessTerrain -= 0.1f;
		if (this.naturelessTerrain > 0f)
		{
			this.ambienceVolumes.SetBool("Natureless", true);
		}
		if (this.naturelessTerrain < 0f)
		{
			this.ambienceVolumes.SetBool("Natureless", false);
		}
		try
		{
			float num = math.saturate(LightVolume.Instance().SamplePositionAlpha(base.transform.position));
			num = math.saturate(1f - math.remap(0f, 0.3f, 0f, 1f, num));
			this.reverb.room = (int)math.remap(0f, 1f, -4000f, -100f, num);
		}
		catch
		{
			Debug.LogError("You probably need to bake the lightmap");
		}
		if (this.volcanoObj)
		{
			this.vulcanoT -= Time.deltaTime;
			if (this.vulcanoT <= 0f)
			{
				this.volcano = false;
				this.vulcanoT = 0f;
				this.reverb.enabled = true;
			}
			if (this.vulcanoT > 0f)
			{
				this.volcano = true;
				this.reverb.enabled = false;
			}
			if (Vector3.Distance(base.transform.position, this.volcanoObj.transform.position) < 200f)
			{
				this.vulcanoT = 10f;
			}
			this.ambienceVolumes.SetBool("Volcano", this.volcano);
		}
		if (this.ambienceVolumes && this.dayNight)
		{
			this.ambienceVolumes.SetFloat("Height", base.transform.position.y);
			this.ambienceVolumes.SetFloat("Time", this.dayNight.timeOfDay);
			if (this.dayNight.timeOfDay > 5.5f && this.dayNight.timeOfDay < 6.5f && this.t != 1)
			{
				this.t = 1;
				this.stingerSource.clip = this.sunRiseStinger[global::UnityEngine.Random.Range(0, this.sunRiseStinger.Length)];
				if (!this.volcano)
				{
					this.stingerSource.Play();
				}
			}
			if (this.dayNight.timeOfDay > 19.5f && this.dayNight.timeOfDay < 20f && this.t != 2)
			{
				this.t = 2;
				this.stingerSource.clip = this.sunSetStinger[global::UnityEngine.Random.Range(0, this.sunSetStinger.Length)];
				if (!this.volcano)
				{
					this.stingerSource.Play();
				}
			}
			if (this.dayNight.timeOfDay > 21.2f && this.dayNight.timeOfDay < 26f && this.t != 3)
			{
				this.t = 3;
				this.stingerSource.clip = this.nightStinger[global::UnityEngine.Random.Range(0, this.nightStinger.Length)];
				if (!this.volcano)
				{
					this.stingerSource.Play();
				}
			}
		}
		this.priorityMusicTimer -= Time.deltaTime;
		CharacterData component = base.transform.root.GetComponent<CharacterData>();
		if (component.sinceDead > 0.5f && !Character.localCharacter.warping && !component.passedOut && !component.dead && !component.fullyPassedOut)
		{
			if (base.transform.position.z > this.beachStingerZ && !this.playedBeach)
			{
				this.playedBeach = true;
				this.mainMusic.clip = this.climbStingerBeach;
				this.mainMusic.volume = 0.35f;
				this.mainMusic.Play();
				this.priorityMusicTimer = 120f;
				Debug.Log("Played beach stinger");
			}
			if (base.transform.position.z > this.tropicsStingerZ && !this.playedTropics)
			{
				this.playedTropics = true;
				this.mainMusic.clip = this.climbStingerTropics;
				this.mainMusic.volume = 0.5f;
				this.mainMusic.Play();
				this.priorityMusicTimer = 120f;
				Debug.Log("Played tropics stinger");
			}
			if (base.transform.position.z > this.alpineStingerZ && !this.playedAlpine)
			{
				this.mainMusic.volume = 0.4f;
				this.playedAlpine = true;
				this.mainMusic.clip = this.climbStingerAlpine;
				this.mainMusic.Play();
				this.priorityMusicTimer = 120f;
				Debug.Log("Played alpine stinger");
			}
			if (base.transform.position.z > this.calderaStingerZ && !this.playedCaldera)
			{
				if (!this.volcanoObj)
				{
					this.volcanoObj = GameObject.Find("VolcanoModel");
				}
				this.mainMusic.volume = 0.75f;
				this.playedCaldera = true;
				this.mainMusic.clip = this.climbStingerCaldera;
				this.mainMusic.Play();
				this.priorityMusicTimer = 120f;
				Debug.Log("Played caldera stinger");
			}
			if (base.transform.position.y > this.kilnStingerY && !this.playedKiln)
			{
				this.inKiln -= Time.deltaTime;
				if (this.inKiln < -2f)
				{
					this.mainMusic.volume = 0.6f;
					this.playedKiln = true;
					this.mainMusic.clip = this.climbStingerKiln;
					this.mainMusic.Play();
					this.priorityMusicTimer = 120f;
				}
				Debug.Log("Played kiln stinger");
			}
			else
			{
				this.inKiln = 0f;
			}
			if (base.transform.position.z > this.peaksTingerZ && !this.playedPeak)
			{
				this.mainMusic.volume = 1f;
				this.playedPeak = true;
				this.mainMusic.clip = this.climbStingerPeak;
				this.mainMusic.Play();
				this.priorityMusicTimer = 120f;
			}
		}
		else
		{
			this.stingerSource.volume = Mathf.Lerp(this.stingerSource.volume, 0f, 0.05f);
			this.mainMusic.volume = Mathf.Lerp(this.mainMusic.volume, 0f, 0.05f);
		}
		if (this.priorityMusicTimer > 0f)
		{
			this.stingerSource.volume = Mathf.Lerp(this.stingerSource.volume, 0f, 0.05f);
		}
		if (this.priorityMusicTimer <= 0f)
		{
			this.stingerSource.volume = Mathf.Lerp(this.stingerSource.volume, 0.35f, 0.05f);
			this.priorityMusicTimer = 0f;
		}
	}

	// Token: 0x06000AC1 RID: 2753 RVA: 0x00034D10 File Offset: 0x00032F10
	private void Coverage()
	{
		float num = 8f;
		this.ceiling = false;
		if (Physics.Linecast(base.transform.position, base.transform.position + Vector3.up * 8f * num, out this.hit, this.layer))
		{
			this.ceiling = true;
		}
		if (Physics.Linecast(base.transform.position, base.transform.position + base.transform.forward * num, out this.hit, this.layer))
		{
			this.coverage += 1f;
		}
		if (Physics.Linecast(base.transform.position, base.transform.position + base.transform.forward * -num, out this.hit, this.layer))
		{
			this.coverage += 1f;
		}
		if (Physics.Linecast(base.transform.position, base.transform.position + base.transform.right * num, out this.hit, this.layer))
		{
			this.coverage += 1f;
		}
		if (Physics.Linecast(base.transform.position, base.transform.position + base.transform.right * -num, out this.hit, this.layer))
		{
			this.coverage += 1f;
		}
		if (Physics.Linecast(base.transform.position, base.transform.position + base.transform.up * num * 4f, out this.hit, this.layer))
		{
			this.coverage += 2f;
		}
	}

	// Token: 0x040009A1 RID: 2465
	public float obstruction;

	// Token: 0x040009A2 RID: 2466
	private float coverage;

	// Token: 0x040009A3 RID: 2467
	public bool ceiling;

	// Token: 0x040009A4 RID: 2468
	public LayerMask layer;

	// Token: 0x040009A5 RID: 2469
	private RaycastHit hit;

	// Token: 0x040009A6 RID: 2470
	public AudioReverbZone reverb;

	// Token: 0x040009A7 RID: 2471
	private DayNightManager dayNight;

	// Token: 0x040009A8 RID: 2472
	private Animator ambienceVolumes;

	// Token: 0x040009A9 RID: 2473
	private int t;

	// Token: 0x040009AA RID: 2474
	public AudioSource stingerSource;

	// Token: 0x040009AB RID: 2475
	public AudioClip[] startStinger;

	// Token: 0x040009AC RID: 2476
	public AudioClip[] sunRiseStinger;

	// Token: 0x040009AD RID: 2477
	public AudioClip[] sunSetStinger;

	// Token: 0x040009AE RID: 2478
	public AudioClip[] nightStinger;

	// Token: 0x040009AF RID: 2479
	public bool volcano;

	// Token: 0x040009B0 RID: 2480
	public GameObject volcanoObj;

	// Token: 0x040009B1 RID: 2481
	public float vulcanoT;

	// Token: 0x040009B2 RID: 2482
	public float naturelessTerrain;

	// Token: 0x040009B3 RID: 2483
	public AudioSource mainMusic;

	// Token: 0x040009B4 RID: 2484
	public AudioClip climbStingerBeach;

	// Token: 0x040009B5 RID: 2485
	private bool playedBeach;

	// Token: 0x040009B6 RID: 2486
	public AudioClip climbStingerTropics;

	// Token: 0x040009B7 RID: 2487
	private bool playedTropics;

	// Token: 0x040009B8 RID: 2488
	public AudioClip climbStingerAlpine;

	// Token: 0x040009B9 RID: 2489
	private bool playedAlpine;

	// Token: 0x040009BA RID: 2490
	public AudioClip climbStingerCaldera;

	// Token: 0x040009BB RID: 2491
	private bool playedCaldera;

	// Token: 0x040009BC RID: 2492
	public AudioClip climbStingerKiln;

	// Token: 0x040009BD RID: 2493
	private bool playedKiln;

	// Token: 0x040009BE RID: 2494
	public AudioClip climbStingerPeak;

	// Token: 0x040009BF RID: 2495
	private bool playedPeak;

	// Token: 0x040009C0 RID: 2496
	private float priorityMusicTimer;

	// Token: 0x040009C1 RID: 2497
	public float beachStingerZ;

	// Token: 0x040009C2 RID: 2498
	public float tropicsStingerZ;

	// Token: 0x040009C3 RID: 2499
	public float alpineStingerZ;

	// Token: 0x040009C4 RID: 2500
	public float calderaStingerZ;

	// Token: 0x040009C5 RID: 2501
	public float kilnStingerY;

	// Token: 0x040009C6 RID: 2502
	public float peaksTingerZ;

	// Token: 0x040009C7 RID: 2503
	public Transform voice;

	// Token: 0x040009C8 RID: 2504
	public AudioReverbFilter reverbFilter;

	// Token: 0x040009C9 RID: 2505
	public AudioEchoFilter echoFilter;

	// Token: 0x040009CA RID: 2506
	public AudioLowPassFilter lowPassFilter;

	// Token: 0x040009CB RID: 2507
	private float inKiln;
}
