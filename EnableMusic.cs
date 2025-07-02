using System;
using UnityEngine;

// Token: 0x020001C1 RID: 449
public class EnableMusic : MonoBehaviour
{
	// Token: 0x06000C28 RID: 3112 RVA: 0x0003CB3A File Offset: 0x0003AD3A
	private void Update()
	{
		if (this.enable)
		{
			this.music.SetActive(true);
		}
	}

	// Token: 0x04000B23 RID: 2851
	public bool enable;

	// Token: 0x04000B24 RID: 2852
	public GameObject music;
}
