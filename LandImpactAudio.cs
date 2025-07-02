using System;
using UnityEngine;

// Token: 0x020001E8 RID: 488
public class LandImpactAudio : MonoBehaviour
{
	// Token: 0x06000CDA RID: 3290 RVA: 0x000400D8 File Offset: 0x0003E2D8
	private void Start()
	{
		this.character = base.transform.root.GetComponent<Character>();
	}

	// Token: 0x06000CDB RID: 3291 RVA: 0x000400F0 File Offset: 0x0003E2F0
	private void Update()
	{
		this.yVel = base.transform.position.y - this.prevY;
		this.prevY = base.transform.position.y;
		if (this.yVel < -0.025f)
		{
			this.storeYVel = this.yVel;
		}
		if (this.yVel > 0.025f)
		{
			this.storeYVel = 0f;
		}
		this.impactVelocity = this.storeYVel;
		if (!this.t && this.character.data.isGrounded)
		{
			if (this.impactVelocity < -0.2f && !this.t)
			{
				this.impactHeavy.SetActive(true);
				this.t = true;
			}
			if (this.impactVelocity < -0.1f && !this.t)
			{
				this.impactMedium.SetActive(true);
				this.t = true;
			}
			if (this.impactVelocity < -0.05f && !this.t)
			{
				this.impactSmall.SetActive(true);
				this.t = true;
			}
			this.storeYVel = 0f;
		}
		if (this.character.data.isGrounded)
		{
			this.storeYVel = 0f;
		}
		if (!this.character.data.isGrounded)
		{
			this.t = false;
			this.impactHeavy.SetActive(false);
			this.impactMedium.SetActive(false);
			this.impactSmall.SetActive(false);
		}
	}

	// Token: 0x04000BD3 RID: 3027
	private Character character;

	// Token: 0x04000BD4 RID: 3028
	public float impactVelocity;

	// Token: 0x04000BD5 RID: 3029
	private float yVel;

	// Token: 0x04000BD6 RID: 3030
	private float storeYVel;

	// Token: 0x04000BD7 RID: 3031
	private float prevY;

	// Token: 0x04000BD8 RID: 3032
	private bool t;

	// Token: 0x04000BD9 RID: 3033
	public GameObject impactSmall;

	// Token: 0x04000BDA RID: 3034
	public GameObject impactMedium;

	// Token: 0x04000BDB RID: 3035
	public GameObject impactHeavy;
}
