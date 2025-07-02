using System;
using Unity.Mathematics;
using UnityEngine;

// Token: 0x02000299 RID: 665
public class VoiceObscuranceFilter : MonoBehaviour
{
	// Token: 0x06000FD9 RID: 4057 RVA: 0x00050728 File Offset: 0x0004E928
	private void Start()
	{
		this.anim = base.GetComponent<Animator>();
		if (GameObject.Find("Airport"))
		{
			this.lowPass.enabled = false;
			this.echo.enabled = false;
			this.reverb.enabled = false;
		}
	}

	// Token: 0x06000FDA RID: 4058 RVA: 0x00050778 File Offset: 0x0004E978
	private void Update()
	{
		if (!this.head)
		{
			this.head = MainCamera.instance.transform;
		}
		if (this.head)
		{
			this.reverbAddition = math.saturate(LightVolume.Instance().SamplePositionAlpha(base.transform.position));
			if (Physics.Linecast(base.transform.position, this.head.position, out this.hit, this.layer))
			{
				this.lowPass.cutoffFrequency = Mathf.Lerp(this.lowPass.cutoffFrequency, 1500f, 1f * Time.deltaTime);
			}
			else
			{
				this.lowPass.cutoffFrequency = Mathf.Lerp(this.lowPass.cutoffFrequency, 7500f, 1f * Time.deltaTime);
			}
			if (Vector3.Distance(base.transform.position, this.head.position) > 60f)
			{
				this.anim.SetFloat("Obscurance", 1f, Time.deltaTime, 0.5f);
				this.echo.wetMix = Mathf.Lerp(this.echo.wetMix, 0.35f, 5f * Time.deltaTime);
				this.echo.dryMix = Mathf.Lerp(this.echo.dryMix, 0.5f, 5f * Time.deltaTime);
				this.echo.decayRatio = Mathf.Lerp(this.echo.decayRatio, 0.3f, 5f * Time.deltaTime);
				this.echo.delay = Mathf.Lerp(this.echo.delay, 500f, 5f * Time.deltaTime);
				return;
			}
			this.anim.SetFloat("Obscurance", this.reverbAddition);
			this.echo.wetMix = Mathf.Lerp(this.echo.wetMix, 0f, 1f * Time.deltaTime);
			this.echo.dryMix = Mathf.Lerp(this.echo.dryMix, 1f, 1f * Time.deltaTime);
			this.echo.decayRatio = Mathf.Lerp(this.echo.decayRatio, 0f, 1f * Time.deltaTime);
			this.echo.delay = Mathf.Lerp(this.echo.delay, 10f, 1f * Time.deltaTime);
		}
	}

	// Token: 0x04000EEE RID: 3822
	public LayerMask layer;

	// Token: 0x04000EEF RID: 3823
	private RaycastHit hit;

	// Token: 0x04000EF0 RID: 3824
	public Transform head;

	// Token: 0x04000EF1 RID: 3825
	public AudioLowPassFilter lowPass;

	// Token: 0x04000EF2 RID: 3826
	public AudioReverbFilter reverb;

	// Token: 0x04000EF3 RID: 3827
	public AudioEchoFilter echo;

	// Token: 0x04000EF4 RID: 3828
	public float reverbAddition;

	// Token: 0x04000EF5 RID: 3829
	private Animator anim;
}
