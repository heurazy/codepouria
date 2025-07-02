using System;
using UnityEngine;

// Token: 0x02000280 RID: 640
public class StormVisual : MonoBehaviour
{
	// Token: 0x06000F5E RID: 3934 RVA: 0x0004DE18 File Offset: 0x0004C018
	private void Start()
	{
		this.zone = base.GetComponentInParent<WindChillZone>();
		this.fogConfig = base.GetComponentInParent<FogConfig>();
		if (this.quadRend)
		{
			this.quadMat = this.quadRend.material;
		}
	}

	// Token: 0x06000F5F RID: 3935 RVA: 0x0004DE50 File Offset: 0x0004C050
	private void LateUpdate()
	{
		this.playerInWindZone = this.zone.windActive && this.zone.characterInsideBounds;
		if (this.playerInWindZone)
		{
			if (!this.part.isPlaying)
			{
				this.part.Play();
			}
			this.windFactor = Mathf.Lerp(this.windFactor, Mathf.Clamp01(this.zone.hasBeenActiveFor * 0.2f), Time.deltaTime);
		}
		else
		{
			if (this.part.isPlaying)
			{
				this.part.Stop();
			}
			this.windFactor = Mathf.Lerp(this.windFactor, 0f, Time.deltaTime);
		}
		if (this.stormType == StormVisual.StormType.Rain)
		{
			DayNightManager.instance.rainstormWindFactor = this.windFactor;
		}
		else if (this.stormType == StormVisual.StormType.Snow)
		{
			DayNightManager.instance.snowstormWindFactor = this.windFactor;
		}
		if (this.zone.characterInsideBounds)
		{
			base.transform.position = Character.observedCharacter.Center;
			base.transform.rotation = Quaternion.LookRotation(this.zone.currentWindDirection);
			if (this.fogConfig && this.zone.windActive)
			{
				this.fogConfig.SetFog();
			}
			if (this.quadMat)
			{
				this.quadRend.enabled = true;
				this.quadMat.SetFloat("_Alpha", this.windFactor);
				return;
			}
		}
		else if (this.quadRend)
		{
			this.quadRend.enabled = false;
		}
	}

	// Token: 0x04000E65 RID: 3685
	public ParticleSystem part;

	// Token: 0x04000E66 RID: 3686
	public MeshRenderer quadRend;

	// Token: 0x04000E67 RID: 3687
	private Material quadMat;

	// Token: 0x04000E68 RID: 3688
	private FogConfig fogConfig;

	// Token: 0x04000E69 RID: 3689
	public AudioLoop stormSFX;

	// Token: 0x04000E6A RID: 3690
	public bool playerInWindZone;

	// Token: 0x04000E6B RID: 3691
	private WindChillZone zone;

	// Token: 0x04000E6C RID: 3692
	public StormVisual.StormType stormType;

	// Token: 0x04000E6D RID: 3693
	public float windFactor;

	// Token: 0x020003B7 RID: 951
	public enum StormType
	{
		// Token: 0x040013AB RID: 5035
		Rain,
		// Token: 0x040013AC RID: 5036
		Snow
	}
}
