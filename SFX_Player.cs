using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

// Token: 0x0200013A RID: 314
public class SFX_Player : MonoBehaviour
{
	// Token: 0x06000911 RID: 2321 RVA: 0x0002E140 File Offset: 0x0002C340
	private void Start()
	{
		this.defaultSource = base.GetComponentInChildren<AudioSource>().gameObject;
		SFX_Player.instance = this;
		for (int i = 0; i < 20; i++)
		{
			this.CreateNewSource();
		}
	}

	// Token: 0x06000912 RID: 2322 RVA: 0x0002E178 File Offset: 0x0002C378
	public SFX_Player.SoundEffectHandle PlaySFX(SFX_Instance SFX, Vector3 position, Transform followTransform = null, SFX_Settings overrideSettings = null, float volumeMultiplier = 1f, bool loop = false)
	{
		if (SFX == null)
		{
			return null;
		}
		if (SFX.clips.Length == 0)
		{
			return null;
		}
		if (!SFX.ReadyToPlay())
		{
			return null;
		}
		if (SFX.settings.spatialBlend > 0f && Vector3.Distance(MainCamera.instance.transform.position, position) > SFX.settings.range / 2f)
		{
			return null;
		}
		if (this.nrOfSoundsPlayed + 1 >= AudioSettings.GetConfiguration().numRealVoices)
		{
			this.StopOldest();
		}
		SFX.OnPlayed();
		SFX_Player.SoundEffectHandle soundEffectHandle = new SFX_Player.SoundEffectHandle();
		soundEffectHandle.Init(base.StartCoroutine(this.IPlaySFX(SFX, position, followTransform, overrideSettings, volumeMultiplier, loop, soundEffectHandle)));
		return soundEffectHandle;
	}

	// Token: 0x06000913 RID: 2323 RVA: 0x0002E224 File Offset: 0x0002C424
	private void StopOldest()
	{
		this.currentlyPlayed[0].source.StopPlaying();
	}

	// Token: 0x06000914 RID: 2324 RVA: 0x0002E23C File Offset: 0x0002C43C
	private IEnumerator IPlaySFX(SFX_Instance SFX, Vector3 position, Transform followTransform, SFX_Settings overrideSettings, float volumeMultiplier, bool loop, SFX_Player.SoundEffectHandle handle)
	{
		SFX_Player.SFX_Source source = this.GetAvailibleSource();
		AudioClip clip = SFX.GetClip();
		if (clip == null)
		{
			Debug.LogError("Trying to play null sound >:I");
			yield break;
		}
		SFX_Settings settings = SFX.settings;
		if (overrideSettings != null)
		{
			settings = overrideSettings;
		}
		float c = 0f;
		float t = clip.length;
		source.source.clip = clip;
		source.source.transform.position = position;
		source.source.volume = settings.volume * Random.Range(1f - settings.volume_Variation, 1f) * volumeMultiplier;
		source.source.pitch = settings.pitch + Random.Range(-settings.pitch_Variation * 0.5f, settings.pitch_Variation * 0.5f);
		source.source.maxDistance = settings.range;
		source.source.spatialBlend = settings.spatialBlend;
		source.source.dopplerLevel = settings.dopplerLevel;
		source.source.loop = loop;
		source.source.outputAudioMixerGroup = this.defaultMixerGroup;
		Vector3 relativePos = Vector3.zero;
		if (followTransform)
		{
			relativePos = followTransform.InverseTransformPoint(position);
		}
		source.StartPlaying(handle);
		while (c < t || loop)
		{
			c += Time.deltaTime * settings.pitch;
			if (followTransform)
			{
				source.source.transform.position = followTransform.TransformPoint(relativePos);
			}
			yield return null;
		}
		source.StopPlaying();
		yield break;
	}

	// Token: 0x06000915 RID: 2325 RVA: 0x0002E28C File Offset: 0x0002C48C
	private SFX_Player.SFX_Source GetAvailibleSource()
	{
		for (int i = 0; i < this.sources.Count; i++)
		{
			if (!this.sources[i].isPlaying)
			{
				return this.sources[i];
			}
		}
		return this.CreateNewSource();
	}

	// Token: 0x06000916 RID: 2326 RVA: 0x0002E2D8 File Offset: 0x0002C4D8
	private SFX_Player.SFX_Source CreateNewSource()
	{
		SFX_Player.SFX_Source sfx_Source = new SFX_Player.SFX_Source();
		GameObject gameObject = Object.Instantiate<GameObject>(this.defaultSource, base.transform.position, base.transform.rotation, base.transform);
		sfx_Source.source = gameObject.GetComponent<AudioSource>();
		sfx_Source.player = this;
		this.sources.Add(sfx_Source);
		return sfx_Source;
	}

	// Token: 0x06000917 RID: 2327 RVA: 0x0002E333 File Offset: 0x0002C533
	private void OnPlayed(SFX_Player.SoundEffectHandle handle)
	{
		this.nrOfSoundsPlayed++;
		this.currentlyPlayed.Add(handle);
	}

	// Token: 0x06000918 RID: 2328 RVA: 0x0002E34F File Offset: 0x0002C54F
	private void OnStopped(SFX_Player.SoundEffectHandle handle)
	{
		this.nrOfSoundsPlayed--;
		this.currentlyPlayed.Remove(handle);
	}

	// Token: 0x06000919 RID: 2329 RVA: 0x0002E36C File Offset: 0x0002C56C
	public static void StopPlaying(SFX_Player.SoundEffectHandle handle, float fadeTime = 0f)
	{
		SFX_Player.SFX_Source sfxsourceFromHandle = SFX_Player.GetSFXSourceFromHandle(handle);
		if (sfxsourceFromHandle != null)
		{
			if (fadeTime == 0f)
			{
				sfxsourceFromHandle.StopPlaying();
				return;
			}
			SFX_Player.instance.StartCoroutine(SFX_Player.FadeOut(sfxsourceFromHandle, fadeTime));
		}
	}

	// Token: 0x0600091A RID: 2330 RVA: 0x0002E3A4 File Offset: 0x0002C5A4
	private static IEnumerator FadeOut(SFX_Player.SFX_Source source, float fadeTime)
	{
		float c = 0f;
		float startVolume = source.source.volume;
		while (c < fadeTime)
		{
			c += Time.deltaTime;
			source.source.volume = Mathf.Lerp(startVolume, 0f, c / fadeTime);
			yield return null;
		}
		source.StopPlaying();
		yield break;
	}

	// Token: 0x0600091B RID: 2331 RVA: 0x0002E3BC File Offset: 0x0002C5BC
	private static SFX_Player.SFX_Source GetSFXSourceFromHandle(SFX_Player.SoundEffectHandle handle)
	{
		foreach (SFX_Player.SFX_Source sfx_Source in SFX_Player.instance.sources)
		{
			if (sfx_Source.handle == handle)
			{
				return sfx_Source;
			}
		}
		return null;
	}

	// Token: 0x04000816 RID: 2070
	public AudioMixerGroup defaultMixerGroup;

	// Token: 0x04000817 RID: 2071
	private GameObject defaultSource;

	// Token: 0x04000818 RID: 2072
	public List<SFX_Player.SFX_Source> sources = new List<SFX_Player.SFX_Source>();

	// Token: 0x04000819 RID: 2073
	private List<SFX_Player.SoundEffectHandle> currentlyPlayed = new List<SFX_Player.SoundEffectHandle>();

	// Token: 0x0400081A RID: 2074
	public static SFX_Player instance;

	// Token: 0x0400081B RID: 2075
	private int nrOfSoundsPlayed;

	// Token: 0x0200035F RID: 863
	[Serializable]
	public class SFX_Source
	{
		// Token: 0x06001397 RID: 5015 RVA: 0x0005D000 File Offset: 0x0005B200
		public void StopPlaying()
		{
			if (!this.isPlaying)
			{
				return;
			}
			if (this.handle.corutine != null)
			{
				this.player.StopCoroutine(this.handle.corutine);
			}
			this.player.OnStopped(this.handle);
			this.source.Stop();
			this.isPlaying = false;
			this.handle.source = null;
			this.handle = null;
		}

		// Token: 0x06001398 RID: 5016 RVA: 0x0005D06F File Offset: 0x0005B26F
		public void StartPlaying(SFX_Player.SoundEffectHandle setHandle)
		{
			if (this.isPlaying)
			{
				return;
			}
			this.player.OnPlayed(setHandle);
			this.source.Play();
			this.isPlaying = true;
			this.handle = setHandle;
			this.handle.source = this;
		}

		// Token: 0x0400125B RID: 4699
		public AudioSource source;

		// Token: 0x0400125C RID: 4700
		public bool isPlaying;

		// Token: 0x0400125D RID: 4701
		public SFX_Player.SoundEffectHandle handle;

		// Token: 0x0400125E RID: 4702
		public SFX_Player player;
	}

	// Token: 0x02000360 RID: 864
	public class SoundEffectHandle
	{
		// Token: 0x0600139A RID: 5018 RVA: 0x0005D0B3 File Offset: 0x0005B2B3
		public void Init(Coroutine c)
		{
			this.corutine = c;
		}

		// Token: 0x0400125F RID: 4703
		public Coroutine corutine;

		// Token: 0x04001260 RID: 4704
		public SFX_Player.SFX_Source source;
	}
}
