using System;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x0200020B RID: 523
public class PassOutPost : MonoBehaviour
{
	// Token: 0x06000D8B RID: 3467 RVA: 0x00044488 File Offset: 0x00042688
	private void Start()
	{
		this.vol = base.GetComponent<Volume>();
	}

	// Token: 0x06000D8C RID: 3468 RVA: 0x00044498 File Offset: 0x00042698
	private void Update()
	{
		if (!Character.localCharacter)
		{
			return;
		}
		this.vol.enabled = this.vol.weight > 0.0001f;
		if (Character.localCharacter.data.fullyPassedOut)
		{
			this.vol.weight = 0f;
			return;
		}
		this.vol.weight = Character.localCharacter.data.passOutValue;
	}

	// Token: 0x04000CA1 RID: 3233
	private Volume vol;
}
