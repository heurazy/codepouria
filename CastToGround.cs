using System;
using UnityEngine;

// Token: 0x02000052 RID: 82
public class CastToGround : MonoBehaviour
{
	// Token: 0x06000385 RID: 901 RVA: 0x00015518 File Offset: 0x00013718
	private void Start()
	{
		if (this.castOnStart)
		{
			this.castToGround();
		}
	}

	// Token: 0x06000386 RID: 902 RVA: 0x00015528 File Offset: 0x00013728
	public void castToGround()
	{
		RaycastHit raycastHit;
		if (Physics.Raycast(base.transform.position, Vector3.down, out raycastHit))
		{
			base.transform.position = raycastHit.point + this.offset;
			base.transform.rotation = Quaternion.FromToRotation(Vector3.up, raycastHit.normal);
		}
	}

	// Token: 0x06000387 RID: 903 RVA: 0x00015587 File Offset: 0x00013787
	private void Update()
	{
	}

	// Token: 0x0400040B RID: 1035
	public bool castOnStart = true;

	// Token: 0x0400040C RID: 1036
	public Vector3 offset;
}
