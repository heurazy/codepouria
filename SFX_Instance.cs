using System;
using UnityEngine;

// Token: 0x02000138 RID: 312
[CreateAssetMenu(fileName = "SoundEffectInstance", menuName = "Landfall/SoundEffectInstance")]
public class SFX_Instance : ScriptableObject
{
	// Token: 0x0600090B RID: 2315 RVA: 0x0002E052 File Offset: 0x0002C252
	public AudioClip GetClip()
	{
		return this.clips[Random.Range(0, this.clips.Length)];
	}

	// Token: 0x0600090C RID: 2316 RVA: 0x0002E069 File Offset: 0x0002C269
	public void Play(Vector3 pos = default(Vector3))
	{
		SFX_Player.instance.PlaySFX(this, pos, null, null, 1f, false);
	}

	// Token: 0x0600090D RID: 2317 RVA: 0x0002E080 File Offset: 0x0002C280
	internal void OnPlayed()
	{
		this.lastTimePlayed = Time.unscaledTime;
	}

	// Token: 0x0600090E RID: 2318 RVA: 0x0002E08D File Offset: 0x0002C28D
	internal bool ReadyToPlay()
	{
		return this.lastTimePlayed > Time.unscaledTime + this.settings.cooldown || this.lastTimePlayed + this.settings.cooldown < Time.unscaledTime;
	}

	// Token: 0x0400080A RID: 2058
	public AudioClip[] clips;

	// Token: 0x0400080B RID: 2059
	public SFX_Settings settings;

	// Token: 0x0400080C RID: 2060
	internal float lastTimePlayed;
}
