using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000178 RID: 376
public class UIWheelSlice : MonoBehaviour
{
	// Token: 0x06000A8E RID: 2702 RVA: 0x0003381D File Offset: 0x00031A1D
	public Vector3 GetUpVector()
	{
		return Quaternion.Euler(0f, 0f, this.offsetRotation) * base.transform.up;
	}

	// Token: 0x04000970 RID: 2416
	public Button button;

	// Token: 0x04000971 RID: 2417
	private float offsetRotation = 22.5f;
}
