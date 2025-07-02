using System;
using UnityEngine;

// Token: 0x0200010A RID: 266
public class PlayerShaderParams : MonoBehaviour
{
	// Token: 0x060007DD RID: 2013 RVA: 0x00029BC0 File Offset: 0x00027DC0
	private void Update()
	{
		if (Character.localCharacter == null)
		{
			return;
		}
		Shader.SetGlobalVector("PlayerPos", Character.localCharacter.Center + this.playerCenterOffset);
	}

	// Token: 0x0400075A RID: 1882
	public Vector3 playerCenterOffset;
}
