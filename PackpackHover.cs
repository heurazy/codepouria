using System;
using UnityEngine;

// Token: 0x0200020A RID: 522
public class PackpackHover : MonoBehaviour
{
	// Token: 0x06000D88 RID: 3464 RVA: 0x00044298 File Offset: 0x00042498
	private void Start()
	{
		this.forward = base.transform.forward;
		this.up = base.transform.up;
		this.item = base.GetComponent<Item>();
		this.rig = base.GetComponent<Rigidbody>();
		this.hit = HelperFunctions.LineCheck(base.transform.position, base.transform.position + Vector3.down * 2f, HelperFunctions.LayerType.TerrainMap, 0f, QueryTriggerInteraction.Ignore);
	}

	// Token: 0x06000D89 RID: 3465 RVA: 0x0004431C File Offset: 0x0004251C
	private void FixedUpdate()
	{
		if (this.rig == null)
		{
			return;
		}
		if (!this.hit.transform)
		{
			return;
		}
		if (this.item.itemState != ItemState.Ground)
		{
			return;
		}
		if (!this.item.photonView.IsMine)
		{
			return;
		}
		Vector3 vector = this.hit.point + this.hit.normal * 1f;
		this.rig.AddForce((vector - base.transform.position) * 60f, ForceMode.Acceleration);
		Vector3 vector2 = Vector3.Cross(base.transform.forward, this.forward).normalized * Vector3.Angle(base.transform.forward, this.forward);
		vector2 += Vector3.Cross(base.transform.up, this.up).normalized * Vector3.Angle(base.transform.up, this.up);
		this.rig.AddTorque(vector2 * 100f, ForceMode.Acceleration);
		this.rig.linearVelocity *= 0.8f;
		this.rig.angularVelocity *= 0.8f;
	}

	// Token: 0x04000C9C RID: 3228
	private Rigidbody rig;

	// Token: 0x04000C9D RID: 3229
	private RaycastHit hit;

	// Token: 0x04000C9E RID: 3230
	private Item item;

	// Token: 0x04000C9F RID: 3231
	private Vector3 forward;

	// Token: 0x04000CA0 RID: 3232
	private Vector3 up;
}
