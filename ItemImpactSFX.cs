using System;
using UnityEngine;

// Token: 0x020001DF RID: 479
public class ItemImpactSFX : MonoBehaviour
{
	// Token: 0x06000C9F RID: 3231 RVA: 0x0003EFEB File Offset: 0x0003D1EB
	private void Start()
	{
		this.rig = base.GetComponent<Rigidbody>();
		this.item = base.GetComponent<Item>();
	}

	// Token: 0x06000CA0 RID: 3232 RVA: 0x0003F005 File Offset: 0x0003D205
	private void Update()
	{
		if (this.rig)
		{
			this.vel = Mathf.Lerp(this.vel, Vector3.Magnitude(this.rig.linearVelocity), 10f * Time.deltaTime);
		}
	}

	// Token: 0x06000CA1 RID: 3233 RVA: 0x0003F040 File Offset: 0x0003D240
	private void OnCollisionEnter(Collision collision)
	{
		if (this.rig)
		{
			if (this.item)
			{
				if (!this.item.holderCharacter)
				{
					if (this.vel > 2f)
					{
						for (int i = 0; i < this.impact.Length; i++)
						{
							this.impact[i].Play(base.transform.position);
						}
					}
				}
				else if (this.vel > 6f)
				{
					for (int j = 0; j < this.impact.Length; j++)
					{
						this.impact[j].Play(base.transform.position);
					}
				}
			}
			if (!this.item && !collision.rigidbody && this.vel > 6f)
			{
				for (int k = 0; k < this.impact.Length; k++)
				{
					this.impact[k].Play(base.transform.position);
				}
			}
			this.vel = 0f;
		}
	}

	// Token: 0x04000BA3 RID: 2979
	public float vel;

	// Token: 0x04000BA4 RID: 2980
	private Rigidbody rig;

	// Token: 0x04000BA5 RID: 2981
	private Item item;

	// Token: 0x04000BA6 RID: 2982
	public SFX_Instance[] impact;
}
