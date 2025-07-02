using System;
using UnityEngine;

// Token: 0x02000274 RID: 628
public class SlideTrail : MonoBehaviour
{
	// Token: 0x06000F39 RID: 3897 RVA: 0x0004CE70 File Offset: 0x0004B070
	private void Start()
	{
		ParticleSystem[] componentsInChildren = base.GetComponentsInChildren<ParticleSystem>();
		this.l = componentsInChildren[0];
		this.r = componentsInChildren[1];
		this.character = base.GetComponentInParent<Character>();
	}

	// Token: 0x06000F3A RID: 3898 RVA: 0x0004CEA4 File Offset: 0x0004B0A4
	private void Update()
	{
		this.l.transform.position = this.character.GetBodypartRig(BodypartType.Hand_L).position;
		this.r.transform.position = this.character.GetBodypartRig(BodypartType.Hand_R).position;
		if (this.character.IsSliding() && this.character.data.outOfStaminaFor > 2f)
		{
			this.HandlePart(this.l, this.l.transform.position);
			this.HandlePart(this.r, this.r.transform.position);
			return;
		}
		this.SetPartOn(this.l, false);
		this.SetPartOn(this.r, false);
	}

	// Token: 0x06000F3B RID: 3899 RVA: 0x0004CF6C File Offset: 0x0004B16C
	private void HandlePart(ParticleSystem part, Vector3 position)
	{
		if (HelperFunctions.LineCheck(position, position - this.character.data.groundNormal * 0.3f, HelperFunctions.LayerType.Terrain, 0f, QueryTriggerInteraction.Ignore).transform)
		{
			this.SetPartOn(part, true);
			return;
		}
		this.SetPartOn(part, false);
	}

	// Token: 0x06000F3C RID: 3900 RVA: 0x0004CFC6 File Offset: 0x0004B1C6
	private void SetPartOn(ParticleSystem part, bool on)
	{
		if (on && !part.isPlaying)
		{
			part.Play(true);
			return;
		}
		if (!on && part.isPlaying)
		{
			part.Stop(true);
		}
	}

	// Token: 0x04000E24 RID: 3620
	private ParticleSystem l;

	// Token: 0x04000E25 RID: 3621
	private ParticleSystem r;

	// Token: 0x04000E26 RID: 3622
	private Character character;
}
