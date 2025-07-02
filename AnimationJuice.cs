using System;
using UnityEngine;
using Zorro.Core;

// Token: 0x02000186 RID: 390
public class AnimationJuice : MonoBehaviour
{
	// Token: 0x06000AC9 RID: 2761 RVA: 0x00034FA4 File Offset: 0x000331A4
	public void Screenshake(float amount)
	{
		Vector3 vector = base.transform.position;
		if (this.overrideGameFeelTransform)
		{
			vector = this.overrideGameFeelTransform.position;
		}
		GamefeelHandler.instance.AddPerlinShakeProximity(vector, amount, 0.3f, 15f, 5f);
	}

	// Token: 0x06000ACA RID: 2762 RVA: 0x00034FF4 File Offset: 0x000331F4
	public void PlayParticle(int index)
	{
		if (!this.particles.WithinRange(index))
		{
			Debug.LogError("PlayParticle index out of range");
			return;
		}
		ParticleSystem particleSystem = this.particles[index];
		if (particleSystem != null)
		{
			particleSystem.Play();
			return;
		}
		Debug.LogError("Particle could not be played, is null");
	}

	// Token: 0x040009D0 RID: 2512
	public Transform overrideGameFeelTransform;

	// Token: 0x040009D1 RID: 2513
	public ParticleSystem[] particles;
}
