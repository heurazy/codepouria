using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200026C RID: 620
public class SFXOnImage : MonoBehaviour
{
	// Token: 0x06000EEA RID: 3818 RVA: 0x0004AE14 File Offset: 0x00049014
	private void Update()
	{
		if (this.image.texture != this.tex)
		{
			for (int i = 0; i < this.equipSound.Length; i++)
			{
				this.equipSound[i].Play(default(Vector3));
			}
		}
		this.tex = this.image.texture;
	}

	// Token: 0x04000DC2 RID: 3522
	public RawImage image;

	// Token: 0x04000DC3 RID: 3523
	private Texture tex;

	// Token: 0x04000DC4 RID: 3524
	public SFX_Instance[] equipSound;
}
