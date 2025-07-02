using System;
using UnityEngine;

// Token: 0x02000263 RID: 611
public class SegTest : MonoBehaviour
{
	// Token: 0x06000ED2 RID: 3794 RVA: 0x0004AB9C File Offset: 0x00048D9C
	private void Start()
	{
		ConfigurableJoint component = base.transform.GetChild(0).GetComponent<ConfigurableJoint>();
		this.joint2 = base.transform.GetChild(1).GetComponent<ConfigurableJoint>();
		this.joint2.connectedBody = component.GetComponent<Rigidbody>();
	}

	// Token: 0x06000ED3 RID: 3795 RVA: 0x0004ABE3 File Offset: 0x00048DE3
	private void Update()
	{
		this.joint2.connectedAnchor = new Vector3(0f, Mathf.Lerp(0.5f, -0.5f, this.val), 0f);
	}

	// Token: 0x04000DB1 RID: 3505
	[Range(0f, 1f)]
	public float val;

	// Token: 0x04000DB2 RID: 3506
	private ConfigurableJoint joint2;
}
