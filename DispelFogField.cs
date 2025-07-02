using System;
using UnityEngine;
using Zorro.Core;

// Token: 0x020001BE RID: 446
public class DispelFogField : MonoBehaviour
{
	// Token: 0x06000C1E RID: 3102 RVA: 0x0003C8A8 File Offset: 0x0003AAA8
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(base.transform.position, this.innerRadius);
		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere(base.transform.position, this.outerRadius);
	}

	// Token: 0x06000C1F RID: 3103 RVA: 0x0003C8F5 File Offset: 0x0003AAF5
	public void OnDisable()
	{
		Singleton<OrbFogHandler>.Instance.dispelFogAmount = 0f;
	}

	// Token: 0x06000C20 RID: 3104 RVA: 0x0003C908 File Offset: 0x0003AB08
	public void Update()
	{
		float num = Vector3.Distance(Character.observedCharacter.Center, base.transform.position);
		if (Character.observedCharacter && num <= this.outerRadius)
		{
			Singleton<OrbFogHandler>.Instance.dispelFogAmount = Mathf.InverseLerp(this.outerRadius, this.innerRadius, num);
			return;
		}
		Singleton<OrbFogHandler>.Instance.dispelFogAmount = 0f;
	}

	// Token: 0x04000B19 RID: 2841
	public float innerRadius = 7.5f;

	// Token: 0x04000B1A RID: 2842
	public float outerRadius = 12.5f;

	// Token: 0x04000B1B RID: 2843
	private float lastEnteredTime;

	// Token: 0x04000B1C RID: 2844
	private bool inflicting;
}
