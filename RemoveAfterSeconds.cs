using System;
using UnityEngine;

// Token: 0x02000253 RID: 595
public class RemoveAfterSeconds : MonoBehaviour
{
	// Token: 0x06000E76 RID: 3702 RVA: 0x00048B17 File Offset: 0x00046D17
	public void Config(bool setShrink, float setSeconds)
	{
		this.seconds = setSeconds;
		this.shrink = setShrink;
	}

	// Token: 0x06000E77 RID: 3703 RVA: 0x00048B28 File Offset: 0x00046D28
	private void Update()
	{
		if (this.seconds >= 0f)
		{
			this.seconds -= Time.deltaTime;
			return;
		}
		if (this.shrink && base.transform.localScale.x > 0.01f)
		{
			base.transform.localScale = Vector3.Lerp(base.transform.localScale, Vector3.zero, Time.deltaTime);
			return;
		}
		Object.Destroy(base.gameObject);
	}

	// Token: 0x04000D75 RID: 3445
	public float seconds = 5f;

	// Token: 0x04000D76 RID: 3446
	public bool shrink;
}
