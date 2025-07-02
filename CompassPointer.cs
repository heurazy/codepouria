using System;
using UnityEngine;

// Token: 0x020000CB RID: 203
public class CompassPointer : MonoBehaviour
{
	// Token: 0x06000652 RID: 1618 RVA: 0x0002227B File Offset: 0x0002047B
	private void Awake()
	{
		this.item = base.GetComponentInParent<Item>();
	}

	// Token: 0x06000653 RID: 1619 RVA: 0x00022289 File Offset: 0x00020489
	private void Update()
	{
		this.UpdateHeading();
	}

	// Token: 0x06000654 RID: 1620 RVA: 0x00022294 File Offset: 0x00020494
	protected void UpdateHeading()
	{
		bool flag = true;
		switch (this.compassType)
		{
		case CompassPointer.CompassType.Normal:
			this.heading = Vector3.forward;
			break;
		case CompassPointer.CompassType.Warp:
			flag = false;
			this.needle.RotateAround(this.needle.transform.position, this.needle.right, this.warpSpeed * Time.deltaTime * this.speedMultiplier);
			break;
		case CompassPointer.CompassType.Pirate:
			this.UpdateHeadingPirate();
			break;
		}
		if (flag)
		{
			this.heading = Vector3.ProjectOnPlane(this.heading, base.transform.forward);
			this.needle.transform.rotation = Quaternion.LookRotation(Vector3.Slerp(this.needle.transform.forward, this.heading, 10f * Time.deltaTime), base.transform.up);
		}
	}

	// Token: 0x06000655 RID: 1621 RVA: 0x00022374 File Offset: 0x00020574
	protected void UpdateHeadingPirate()
	{
		if (Luggage.ALL_LUGGAGE.Count == 0)
		{
			this.heading = Quaternion.Euler(0f, Time.time * this.warpSpeed, 0f) * Vector3.forward;
		}
		if (this.item.inActiveList)
		{
			float num = float.MaxValue;
			foreach (Luggage luggage in Luggage.ALL_LUGGAGE)
			{
				if (Vector3.Distance(luggage.Center(), base.transform.position) < num)
				{
					num = Vector3.Distance(luggage.Center(), base.transform.position);
					this.currentLuggageVector = luggage.Center() - base.transform.position;
				}
			}
			this.heading = this.currentLuggageVector;
		}
	}

	// Token: 0x04000627 RID: 1575
	public CompassPointer.CompassType compassType;

	// Token: 0x04000628 RID: 1576
	public Transform needle;

	// Token: 0x04000629 RID: 1577
	public float warpSpeed = 2f;

	// Token: 0x0400062A RID: 1578
	public float speedMultiplier = 1f;

	// Token: 0x0400062B RID: 1579
	private Item item;

	// Token: 0x0400062C RID: 1580
	protected Vector3 heading;

	// Token: 0x0400062D RID: 1581
	private Vector3 currentLuggageVector = Vector3.zero;

	// Token: 0x02000329 RID: 809
	public enum CompassType
	{
		// Token: 0x040011A1 RID: 4513
		Normal,
		// Token: 0x040011A2 RID: 4514
		Warp,
		// Token: 0x040011A3 RID: 4515
		Pirate
	}
}
