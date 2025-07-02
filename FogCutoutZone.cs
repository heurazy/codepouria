using System;
using UnityEngine;

// Token: 0x020001CC RID: 460
public class FogCutoutZone : MonoBehaviour
{
	// Token: 0x06000C57 RID: 3159 RVA: 0x0003D600 File Offset: 0x0003B800
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = new Color(1f, 1f, 1f, this.amount);
		Gizmos.DrawWireSphere(base.transform.position, this.min);
		Gizmos.DrawWireSphere(base.transform.position, this.max);
		Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
		Gizmos.DrawCube(base.transform.position + Vector3.forward * this.transitionPoint, new Vector3(300f, 9999f, 0.1f));
	}

	// Token: 0x04000B4F RID: 2895
	public float min = 10f;

	// Token: 0x04000B50 RID: 2896
	public float max = 100f;

	// Token: 0x04000B51 RID: 2897
	public float amount = 1f;

	// Token: 0x04000B52 RID: 2898
	public float transitionPoint;
}
