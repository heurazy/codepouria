using System;
using UnityEngine;

// Token: 0x020001CF RID: 463
public class FollowBodypart : MonoBehaviour
{
	// Token: 0x06000C61 RID: 3169 RVA: 0x0003D9C2 File Offset: 0x0003BBC2
	private void Start()
	{
		this.target = base.GetComponentInParent<Character>().GetBodypart(this.followPart).transform;
	}

	// Token: 0x06000C62 RID: 3170 RVA: 0x0003D9E0 File Offset: 0x0003BBE0
	private void LateUpdate()
	{
		base.transform.position = this.target.position;
	}

	// Token: 0x04000B62 RID: 2914
	public BodypartType followPart;

	// Token: 0x04000B63 RID: 2915
	private Transform target;
}
