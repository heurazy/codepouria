using System;
using UnityEngine;

// Token: 0x0200018F RID: 399
public class Billboard : MonoBehaviour
{
	// Token: 0x06000AED RID: 2797 RVA: 0x0003616B File Offset: 0x0003436B
	private void LateUpdate()
	{
		base.transform.rotation = Quaternion.LookRotation(-(MainCamera.instance.transform.position - base.transform.position));
	}
}
