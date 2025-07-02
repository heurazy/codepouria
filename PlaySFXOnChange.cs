using System;
using UnityEngine;

// Token: 0x0200021C RID: 540
public class PlaySFXOnChange : MonoBehaviour
{
	// Token: 0x06000DD6 RID: 3542 RVA: 0x00045E04 File Offset: 0x00044004
	private void Update()
	{
		if (this.refObj.active && !this.t)
		{
			this.t = true;
			for (int i = 0; i < this.sfxOn.Length; i++)
			{
				this.sfxOn[i].Play(default(Vector3));
			}
		}
		if (!this.refObj.active && this.t)
		{
			this.t = false;
			for (int j = 0; j < this.sfxOff.Length; j++)
			{
				this.sfxOff[j].Play(default(Vector3));
			}
		}
	}

	// Token: 0x04000CEB RID: 3307
	public SFX_Instance[] sfxOn;

	// Token: 0x04000CEC RID: 3308
	public SFX_Instance[] sfxOff;

	// Token: 0x04000CED RID: 3309
	private bool t;

	// Token: 0x04000CEE RID: 3310
	public GameObject refObj;
}
