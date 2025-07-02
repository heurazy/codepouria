using System;
using UnityEngine;

// Token: 0x020000C5 RID: 197
public class Action_Torch : OnItemStateChangedAction
{
	// Token: 0x06000639 RID: 1593 RVA: 0x00021B14 File Offset: 0x0001FD14
	public override void RunAction(ItemState state)
	{
		if (state == ItemState.Held)
		{
			for (int i = 0; i < this.particles.Length; i++)
			{
				ParticleSystem.MainModule main = this.particles[i].main;
				Debug.Log("char is null? " + (base.character == null).ToString());
				main.customSimulationSpace = base.character.refs.animationPositionTransform;
			}
		}
	}

	// Token: 0x0600063A RID: 1594 RVA: 0x00021B80 File Offset: 0x0001FD80
	private void Update()
	{
		this.torchLight.intensity = this.lightCurve.Evaluate(Time.time * this.lightSpeed) * this.lightIntensity;
	}

	// Token: 0x04000610 RID: 1552
	public ParticleSystem[] particles;

	// Token: 0x04000611 RID: 1553
	public Light torchLight;

	// Token: 0x04000612 RID: 1554
	public AnimationCurve lightCurve;

	// Token: 0x04000613 RID: 1555
	public float lightSpeed = 1f;

	// Token: 0x04000614 RID: 1556
	public float lightIntensity = 10f;
}
