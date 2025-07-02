using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200019C RID: 412
public class BugleSFX : MonoBehaviourPun
{
	// Token: 0x06000B56 RID: 2902 RVA: 0x00037FDC File Offset: 0x000361DC
	private void Start()
	{
		this.item = base.GetComponent<Item>();
	}

	// Token: 0x06000B57 RID: 2903 RVA: 0x00037FEC File Offset: 0x000361EC
	private void UpdateTooting()
	{
		if (base.photonView.IsMine)
		{
			bool flag = this.item.isUsingPrimary;
			if (this.magicBugle && this.magicBugle.currentFuel <= 0f)
			{
				flag = false;
			}
			if (flag != this.hold)
			{
				if (flag)
				{
					int num = Random.Range(0, this.bugle.Length);
					base.photonView.RPC("RPC_StartToot", RpcTarget.All, new object[] { num });
				}
				else
				{
					base.photonView.RPC("RPC_EndToot", RpcTarget.All, Array.Empty<object>());
				}
				this.hold = flag;
			}
		}
	}

	// Token: 0x06000B58 RID: 2904 RVA: 0x00038090 File Offset: 0x00036290
	[PunRPC]
	private void RPC_StartToot(int clip)
	{
		this.currentClip = clip;
		this.hold = true;
		if (this.particle1 && this.particle2)
		{
			if (!this.particle1.isPlaying)
			{
				this.particle1.Play();
			}
			if (!this.particle2.isPlaying)
			{
				this.particle2.Play();
			}
			ParticleSystem.EmissionModule emission = this.particle1.emission;
			ParticleSystem.EmissionModule emission2 = this.particle2.emission;
			emission.enabled = true;
			emission2.enabled = true;
		}
	}

	// Token: 0x06000B59 RID: 2905 RVA: 0x00038120 File Offset: 0x00036320
	[PunRPC]
	private void RPC_EndToot()
	{
		this.hold = false;
		if (this.particle1 && this.particle2)
		{
			ParticleSystem.EmissionModule emission = this.particle1.emission;
			ParticleSystem.EmissionModule emission2 = this.particle2.emission;
			emission.enabled = false;
			emission2.enabled = false;
		}
	}

	// Token: 0x06000B5A RID: 2906 RVA: 0x00038178 File Offset: 0x00036378
	private void Update()
	{
		this.UpdateTooting();
		if (this.hold && !this.t)
		{
			this.buglePlayer.clip = this.bugle[this.currentClip];
			this.buglePlayer.Play();
			this.buglePlayer.volume = 0f;
			this.t = true;
		}
		if (this.hold)
		{
			this.buglePlayer.volume = Mathf.Lerp(this.buglePlayer.volume, this.volume, 10f * Time.deltaTime);
		}
		if (!this.hold)
		{
			this.buglePlayer.volume = Mathf.Lerp(this.buglePlayer.volume, 0f, 10f * Time.deltaTime);
		}
		if (!this.hold && this.t)
		{
			this.t = false;
		}
	}

	// Token: 0x04000A66 RID: 2662
	private Item item;

	// Token: 0x04000A67 RID: 2663
	public bool hold;

	// Token: 0x04000A68 RID: 2664
	private bool t;

	// Token: 0x04000A69 RID: 2665
	private int currentClip;

	// Token: 0x04000A6A RID: 2666
	public AudioClip[] bugle;

	// Token: 0x04000A6B RID: 2667
	public AudioSource buglePlayer;

	// Token: 0x04000A6C RID: 2668
	public AudioSource bugleEnd;

	// Token: 0x04000A6D RID: 2669
	public MagicBugle magicBugle;

	// Token: 0x04000A6E RID: 2670
	public ParticleSystem particle1;

	// Token: 0x04000A6F RID: 2671
	public ParticleSystem particle2;

	// Token: 0x04000A70 RID: 2672
	public float volume = 0.35f;
}
