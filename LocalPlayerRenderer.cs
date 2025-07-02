using System;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x020001F3 RID: 499
public class LocalPlayerRenderer : MonoBehaviour
{
	// Token: 0x06000D08 RID: 3336 RVA: 0x00041304 File Offset: 0x0003F504
	private void Start()
	{
		Character componentInParent = base.GetComponentInParent<Character>();
		if (componentInParent && componentInParent.IsLocal)
		{
			base.GetComponent<MeshRenderer>().shadowCastingMode = this.renderMode;
		}
	}

	// Token: 0x04000C05 RID: 3077
	public ShadowCastingMode renderMode;
}
