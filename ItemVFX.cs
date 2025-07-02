using System;
using UnityEngine;

// Token: 0x020001E1 RID: 481
public class ItemVFX : MonoBehaviour
{
	// Token: 0x06000CA4 RID: 3236 RVA: 0x0003F15E File Offset: 0x0003D35E
	protected virtual void Start()
	{
		this.item = base.GetComponent<Item>();
		if (this.item.holderCharacter == null)
		{
			base.enabled = false;
		}
	}

	// Token: 0x06000CA5 RID: 3237 RVA: 0x0003F186 File Offset: 0x0003D386
	protected virtual void Update()
	{
		this.Shake();
		this.shakeSFX.volume = this.item.castProgress / 2f;
		this.shakeSFX.pitch = 1f + this.item.castProgress;
	}

	// Token: 0x06000CA6 RID: 3238 RVA: 0x0003F1C8 File Offset: 0x0003D3C8
	protected virtual void Shake()
	{
		if (!this.item.finishedCast)
		{
			GamefeelHandler.instance.AddPerlinShake(this.item.castProgress * this.shakeAmount * Time.deltaTime * 60f, 0.2f, 15f);
		}
		if (this.item.finishedCast)
		{
			for (int i = 0; i < this.doneSFX.Length; i++)
			{
				this.doneSFX[i].Play(base.transform.position);
			}
		}
		this.castProgress = this.item.castProgress;
	}

	// Token: 0x04000BAA RID: 2986
	protected Item item;

	// Token: 0x04000BAB RID: 2987
	public bool shake;

	// Token: 0x04000BAC RID: 2988
	public float shakeAmount = 1f;

	// Token: 0x04000BAD RID: 2989
	public float castProgress;

	// Token: 0x04000BAE RID: 2990
	public AudioSource shakeSFX;

	// Token: 0x04000BAF RID: 2991
	public SFX_Instance[] doneSFX;
}
