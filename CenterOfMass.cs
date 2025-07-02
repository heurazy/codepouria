using System;
using UnityEngine;

// Token: 0x020001A0 RID: 416
public class CenterOfMass : MonoBehaviour
{
	// Token: 0x06000B69 RID: 2921 RVA: 0x00038438 File Offset: 0x00036638
	private void Start()
	{
		if (this.onlyOnGround)
		{
			this.item = base.GetComponent<Item>();
			if (this.item.itemState != ItemState.Ground)
			{
				return;
			}
		}
		this.rb = base.GetComponent<Rigidbody>();
		this.rb.centerOfMass = this.localCenterOfMass;
		this.rb.angularDamping = this.angularDamping;
		if (this.centerOfMassTransform)
		{
			this.rb.centerOfMass = this.centerOfMassTransform.localPosition;
		}
	}

	// Token: 0x06000B6A RID: 2922 RVA: 0x000384B8 File Offset: 0x000366B8
	private void Update()
	{
		if (this.onlyOnGround && this.item.itemState != ItemState.Ground)
		{
			return;
		}
		if (this.centerOfMassTransform)
		{
			this.rb.centerOfMass = this.centerOfMassTransform.localPosition;
		}
		else
		{
			this.rb.centerOfMass = this.localCenterOfMass;
		}
		this.rb.angularDamping = this.angularDamping;
	}

	// Token: 0x06000B6B RID: 2923 RVA: 0x00038522 File Offset: 0x00036722
	private void OnDrawGizmosSelected()
	{
		if (this.rb)
		{
			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere(this.rb.worldCenterOfMass, 0.5f);
		}
	}

	// Token: 0x04000A79 RID: 2681
	public bool onlyOnGround;

	// Token: 0x04000A7A RID: 2682
	private Item item;

	// Token: 0x04000A7B RID: 2683
	public Transform centerOfMassTransform;

	// Token: 0x04000A7C RID: 2684
	public Vector3 localCenterOfMass;

	// Token: 0x04000A7D RID: 2685
	public float angularDamping = 3f;

	// Token: 0x04000A7E RID: 2686
	private Rigidbody rb;
}
