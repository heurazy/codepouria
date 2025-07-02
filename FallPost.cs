using System;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x020001C8 RID: 456
public class FallPost : MonoBehaviour
{
	// Token: 0x06000C3C RID: 3132 RVA: 0x0003CF25 File Offset: 0x0003B125
	private void Start()
	{
		this.vol = base.GetComponent<Volume>();
	}

	// Token: 0x06000C3D RID: 3133 RVA: 0x0003CF34 File Offset: 0x0003B134
	private void Update()
	{
		if (Character.localCharacter == null)
		{
			return;
		}
		this.vol.enabled = this.vol.weight > 0.0001f;
		if (Character.localCharacter.data.fallSeconds > 0f)
		{
			this.vol.weight = Mathf.Lerp(this.vol.weight, 1f, Time.deltaTime);
			return;
		}
		this.vol.weight = Mathf.Lerp(this.vol.weight, 0f, Time.deltaTime);
	}

	// Token: 0x04000B34 RID: 2868
	private Volume vol;
}
