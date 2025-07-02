using System;
using UnityEngine;

// Token: 0x02000051 RID: 81
public class CampfireSectionGroundStealer : MonoBehaviour
{
	// Token: 0x06000383 RID: 899 RVA: 0x00015478 File Offset: 0x00013678
	private void Awake()
	{
		foreach (object obj in this.groundParent.transform)
		{
			Transform transform = (Transform)obj;
			if (transform.GetComponent<MeshRenderer>().bounds.center.y > base.transform.position.y + this.offset)
			{
				transform.SetParent(base.transform, true);
			}
		}
	}

	// Token: 0x04000409 RID: 1033
	public float offset;

	// Token: 0x0400040A RID: 1034
	public GameObject groundParent;
}
