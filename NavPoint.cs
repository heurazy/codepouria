using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000206 RID: 518
[DefaultExecutionOrder(1000)]
public class NavPoint : MonoBehaviour
{
	// Token: 0x06000D62 RID: 3426 RVA: 0x0004387C File Offset: 0x00041A7C
	internal NavPoint GetNext(Vector3 targetDirection)
	{
		List<NavPoint> list = new List<NavPoint>();
		foreach (NavPoint navPoint in this.connections)
		{
			if (HelperFunctions.FlatAngle(targetDirection, navPoint.transform.position - base.transform.position) < 90f)
			{
				list.Add(navPoint);
			}
		}
		if (list.Count == 0)
		{
			return null;
		}
		return list[Random.Range(0, list.Count)];
	}

	// Token: 0x06000D63 RID: 3427 RVA: 0x0004391C File Offset: 0x00041B1C
	internal void MirrorConnections()
	{
		foreach (NavPoint navPoint in this.connections)
		{
			if (!navPoint.connections.Contains(this))
			{
				navPoint.connections.Add(this);
			}
		}
	}

	// Token: 0x06000D64 RID: 3428 RVA: 0x00043984 File Offset: 0x00041B84
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		foreach (NavPoint navPoint in this.connections)
		{
			Gizmos.DrawLine(base.transform.position + Vector3.up * 0.1f, navPoint.transform.position + Vector3.up * 0.1f);
		}
	}

	// Token: 0x04000C87 RID: 3207
	public List<NavPoint> connections = new List<NavPoint>();
}
