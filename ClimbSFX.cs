using System;
using UnityEngine;

// Token: 0x020001AC RID: 428
public class ClimbSFX : MonoBehaviour
{
	// Token: 0x06000BE9 RID: 3049 RVA: 0x0003BA61 File Offset: 0x00039C61
	private void Start()
	{
		this.character = base.transform.root.GetComponent<Character>();
	}

	// Token: 0x06000BEA RID: 3050 RVA: 0x0003BA7C File Offset: 0x00039C7C
	private void Update()
	{
		if (this.character)
		{
			if (!this.character.data.isClimbing && this.sToggle)
			{
				this.sToggle = false;
				this.surfaceOff.SetActive(true);
				this.surfaceOnHeavy.SetActive(false);
				this.surfaceOn.SetActive(false);
			}
			if (this.character.data.isClimbing && !this.sToggle)
			{
				this.sToggle = true;
				this.surfaceOn.SetActive(true);
				if (this.character.data.avarageVelocity.y <= -6f)
				{
					this.surfaceOnHeavy.SetActive(true);
				}
				this.surfaceOff.SetActive(false);
			}
			if (!this.character.data.isRopeClimbing && this.rToggle)
			{
				this.rToggle = false;
				this.ropeOff.SetActive(true);
				this.ropeOn.SetActive(false);
			}
			if (this.character.data.isRopeClimbing && !this.rToggle)
			{
				this.rToggle = true;
				this.ropeOn.SetActive(true);
				this.ropeOff.SetActive(false);
			}
		}
	}

	// Token: 0x04000ABB RID: 2747
	private Character character;

	// Token: 0x04000ABC RID: 2748
	public GameObject ropeOn;

	// Token: 0x04000ABD RID: 2749
	public GameObject ropeOff;

	// Token: 0x04000ABE RID: 2750
	private bool rToggle;

	// Token: 0x04000ABF RID: 2751
	public GameObject surfaceOn;

	// Token: 0x04000AC0 RID: 2752
	public GameObject surfaceOff;

	// Token: 0x04000AC1 RID: 2753
	public GameObject surfaceOnHeavy;

	// Token: 0x04000AC2 RID: 2754
	private bool sToggle;
}
