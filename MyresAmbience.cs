using System;
using UnityEngine;

// Token: 0x02000204 RID: 516
public class MyresAmbience : MonoBehaviour
{
	// Token: 0x06000D59 RID: 3417 RVA: 0x000434D0 File Offset: 0x000416D0
	private void Update()
	{
		if (this.anim)
		{
			if (this.anim.GetFloat("Myers Distance") > 60f)
			{
				this.fearMusic.volume = Mathf.Lerp(this.fearMusic.volume, 0f, 1f * Time.deltaTime);
			}
			if (this.anim.GetFloat("Myers Distance") < 50f)
			{
				this.fearMusic.volume = Mathf.Lerp(this.fearMusic.volume, 0.25f, 1f * Time.deltaTime);
			}
			if (this.anim.GetFloat("Myers Distance") < 25f)
			{
				this.fearMusic.volume = Mathf.Lerp(this.fearMusic.volume, 0.75f, 1f * Time.deltaTime);
			}
			if (this.anim.GetFloat("Myers Distance") == 0f)
			{
				this.fearMusic.volume = Mathf.Lerp(this.fearMusic.volume, 0f, 1f * Time.deltaTime);
			}
		}
	}

	// Token: 0x04000C80 RID: 3200
	public Animator anim;

	// Token: 0x04000C81 RID: 3201
	public AudioSource fearMusic;
}
