using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x0200013B RID: 315
public class SFX_PlayOneShot : MonoBehaviour
{
	// Token: 0x0600091D RID: 2333 RVA: 0x0002E43A File Offset: 0x0002C63A
	public void Start()
	{
		if (this.playOnStart)
		{
			this.Play();
		}
	}

	// Token: 0x0600091E RID: 2334 RVA: 0x0002E44A File Offset: 0x0002C64A
	public void OnEnable()
	{
		if (this.playOnEnable)
		{
			base.StartCoroutine(this.<OnEnable>g__PlayAfterAnim|6_0());
		}
	}

	// Token: 0x0600091F RID: 2335 RVA: 0x0002E461 File Offset: 0x0002C661
	public void Play()
	{
		this.PlayOneShot();
	}

	// Token: 0x06000920 RID: 2336 RVA: 0x0002E46C File Offset: 0x0002C66C
	public void PlayOneShot()
	{
		Action action = this.beforePlayAction;
		if (action != null)
		{
			action();
		}
		if (this.sfx != null)
		{
			SFX_Player.instance.PlaySFX(this.sfx, base.transform.position, this.followTransform ? base.transform : null, null, 1f, false);
		}
		for (int i = 0; i < this.sfxs.Length; i++)
		{
			SFX_Player.instance.PlaySFX(this.sfxs[i], base.transform.position, this.followTransform ? base.transform : null, null, 1f, false);
		}
		Action action2 = this.afterPlayAction;
		if (action2 == null)
		{
			return;
		}
		action2();
	}

	// Token: 0x06000922 RID: 2338 RVA: 0x0002E534 File Offset: 0x0002C734
	[CompilerGenerated]
	private IEnumerator <OnEnable>g__PlayAfterAnim|6_0()
	{
		yield return new WaitForEndOfFrame();
		this.Play();
		yield break;
	}

	// Token: 0x0400081C RID: 2076
	public Action beforePlayAction;

	// Token: 0x0400081D RID: 2077
	public Action afterPlayAction;

	// Token: 0x0400081E RID: 2078
	public bool playOnStart;

	// Token: 0x0400081F RID: 2079
	public bool playOnEnable;

	// Token: 0x04000820 RID: 2080
	public bool followTransform = true;

	// Token: 0x04000821 RID: 2081
	public SFX_Instance sfx;

	// Token: 0x04000822 RID: 2082
	public SFX_Instance[] sfxs;
}
