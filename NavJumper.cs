using System;
using System.Collections.Generic;
using System.Linq;
using pworld.Scripts.Extensions;
using UnityEngine;

// Token: 0x02000205 RID: 517
public class NavJumper : MonoBehaviour
{
	// Token: 0x06000D5B RID: 3419 RVA: 0x000435FD File Offset: 0x000417FD
	private void Start()
	{
	}

	// Token: 0x06000D5C RID: 3420 RVA: 0x00043600 File Offset: 0x00041800
	private void Jump()
	{
		List<RaycastHit> list = new List<RaycastHit>();
		for (int i = 0; i < this.castsPerJump; i++)
		{
			RaycastHit raycastHit;
			if (Physics.Raycast(base.transform.position + (ExtMath.RandInsideUnitCircle() * this.castRadius).xny(this.castHeight), Vector3.down * this.castHeight, out raycastHit))
			{
				list.Add(raycastHit);
			}
		}
		Debug.Log(string.Format("Total: {0}", list.Count));
		list = list.Where((RaycastHit hit) => Vector3.Angle(hit.normal, Vector3.up) < 50f).ToList<RaycastHit>();
		Debug.Log(string.Format("After angle: {0}", list.Count));
		list = list.Where((RaycastHit hit) => Vector3.Distance(hit.point, base.transform.position) < this.maxDistance).ToList<RaycastHit>();
		Debug.Log(string.Format("After distance: {0}", list.Count));
		list = list.Where((RaycastHit hit) => hit.point.z > base.transform.position.z && hit.point.y > base.transform.position.y).ToList<RaycastHit>();
		list = list.Where((RaycastHit hit) => hit.point.y > base.transform.position.y).ToList<RaycastHit>();
		Debug.Log(string.Format("After Z: {0}", list.Count));
		if (list.Count == 0)
		{
			return;
		}
		RaycastHit raycastHit2 = list.OrderByDescending((RaycastHit hit) => hit.point.z).First<RaycastHit>();
		Debug.DrawLine(base.transform.position + Vector3.up, raycastHit2.point + Vector3.up, Color.green, 10f);
		base.transform.position = raycastHit2.point;
	}

	// Token: 0x06000D5D RID: 3421 RVA: 0x000437C5 File Offset: 0x000419C5
	private void Update()
	{
	}

	// Token: 0x04000C82 RID: 3202
	public int castsPerJump = 100;

	// Token: 0x04000C83 RID: 3203
	public float maxDistance = 3f;

	// Token: 0x04000C84 RID: 3204
	public float castRadius = 1f;

	// Token: 0x04000C85 RID: 3205
	public float castHeight = 100f;

	// Token: 0x04000C86 RID: 3206
	private int fails;
}
