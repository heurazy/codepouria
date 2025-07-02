using System;
using UnityEngine;

// Token: 0x020002A1 RID: 673
public class WindHeightEffect : MonoBehaviour
{
	// Token: 0x06001002 RID: 4098 RVA: 0x0005139B File Offset: 0x0004F59B
	private void Start()
	{
		this.zone = base.GetComponent<WindChillZone>();
	}

	// Token: 0x06001003 RID: 4099 RVA: 0x000513AC File Offset: 0x0004F5AC
	private void Update()
	{
		if (Character.observedCharacter == null)
		{
			return;
		}
		this.zone.lightVolumeSampleThreshold_lower = Mathf.Lerp(this.from, this.to, Mathf.InverseLerp(this.fromHeight, this.toHeight, Character.observedCharacter.Center.y));
	}

	// Token: 0x04000F0F RID: 3855
	public float from;

	// Token: 0x04000F10 RID: 3856
	public float to;

	// Token: 0x04000F11 RID: 3857
	public float fromHeight;

	// Token: 0x04000F12 RID: 3858
	public float toHeight;

	// Token: 0x04000F13 RID: 3859
	private WindChillZone zone;
}
