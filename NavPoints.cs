using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000207 RID: 519
public class NavPoints : MonoBehaviour
{
	// Token: 0x06000D66 RID: 3430 RVA: 0x00043A33 File Offset: 0x00041C33
	private void Awake()
	{
		NavPoints.instance = this;
		this.points = new List<NavPoint>();
		this.points.AddRange(base.GetComponentsInChildren<NavPoint>());
	}

	// Token: 0x06000D67 RID: 3431 RVA: 0x00043A58 File Offset: 0x00041C58
	private void OnDrawGizmos()
	{
		if (!this.drawGizmos)
		{
			return;
		}
		Gizmos.color = Color.blue;
		foreach (NavPoint navPoint in this.points)
		{
			foreach (NavPoint navPoint2 in navPoint.connections)
			{
				Gizmos.DrawLine(navPoint.transform.position, navPoint2.transform.position);
			}
		}
	}

	// Token: 0x06000D68 RID: 3432 RVA: 0x00043B10 File Offset: 0x00041D10
	public void ConnectPoints()
	{
		this.points = new List<NavPoint>();
		this.points.AddRange(base.GetComponentsInChildren<NavPoint>());
		foreach (NavPoint navPoint in this.points)
		{
			this.CheckPoint(navPoint);
		}
		foreach (NavPoint navPoint2 in this.points)
		{
			navPoint2.MirrorConnections();
		}
	}

	// Token: 0x06000D69 RID: 3433 RVA: 0x00043BC0 File Offset: 0x00041DC0
	private void CheckPoint(NavPoint point)
	{
		point.connections = new List<NavPoint>();
		float num = float.PositiveInfinity;
		List<NavPoint> list = new List<NavPoint>();
		foreach (NavPoint navPoint in this.points)
		{
			if (!(navPoint == point) && !HelperFunctions.LineCheck(point.transform.position + Vector3.up, navPoint.transform.position + Vector3.up, HelperFunctions.LayerType.TerrainMap, 0f, QueryTriggerInteraction.Ignore).transform)
			{
				list.Add(navPoint);
				float num2 = Vector3.Distance(point.transform.position, navPoint.transform.position);
				if (num2 < num)
				{
					num = num2;
				}
			}
		}
		float num3 = num * 1.5f;
		foreach (NavPoint navPoint2 in list)
		{
			if (Vector3.Distance(point.transform.position, navPoint2.transform.position) < num3)
			{
				point.connections.Add(navPoint2);
			}
		}
	}

	// Token: 0x06000D6A RID: 3434 RVA: 0x00043D14 File Offset: 0x00041F14
	internal NavPoint GetNavPoint(Vector3 destination, Vector3 currentPos)
	{
		NavPoint navPoint = null;
		float num = float.PositiveInfinity;
		foreach (NavPoint navPoint2 in this.points)
		{
			float num2 = Vector3.Distance(currentPos, navPoint2.transform.position);
			if (num2 <= num && Vector3.Angle(destination - currentPos, navPoint2.transform.position - currentPos) <= 90f)
			{
				num = num2;
				navPoint = navPoint2;
			}
		}
		return navPoint;
	}

	// Token: 0x04000C88 RID: 3208
	public static NavPoints instance;

	// Token: 0x04000C89 RID: 3209
	public bool drawGizmos;

	// Token: 0x04000C8A RID: 3210
	private List<NavPoint> points = new List<NavPoint>();
}
