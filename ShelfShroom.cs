using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000271 RID: 625
[RequireComponent(typeof(PhotonView))]
public class ShelfShroom : MonoBehaviour
{
	// Token: 0x06000F2B RID: 3883 RVA: 0x0004CA30 File Offset: 0x0004AC30
	private void Awake()
	{
		this.item = base.GetComponent<Item>();
	}

	// Token: 0x06000F2C RID: 3884 RVA: 0x0004CA40 File Offset: 0x0004AC40
	private void OnCollisionEnter(Collision collision)
	{
		if (!this.item.photonView.IsMine)
		{
			return;
		}
		if (this.item.itemState == ItemState.Ground && this.breakOnCollision && this.item.rig && collision.relativeVelocity.magnitude > this.minBreakVelocity)
		{
			this.Break(collision);
		}
	}

	// Token: 0x06000F2D RID: 3885 RVA: 0x0004CAA4 File Offset: 0x0004ACA4
	public void Break(Collision coll)
	{
		if (this.alreadyBroke)
		{
			return;
		}
		this.alreadyBroke = true;
		string text = "0_Items/" + this.instantiateOnBreak.name;
		Quaternion quaternion = Quaternion.Euler(0f, (float)Random.Range(0, 360), 0f);
		if (this.stickToNormal)
		{
			quaternion = Quaternion.LookRotation(Vector3.forward, coll.contacts[0].normal);
		}
		PhotonNetwork.Instantiate(text, coll.contacts[0].point, quaternion, 0, null);
		PhotonNetwork.Destroy(base.gameObject);
	}

	// Token: 0x04000E13 RID: 3603
	private Item item;

	// Token: 0x04000E14 RID: 3604
	public bool breakOnCollision;

	// Token: 0x04000E15 RID: 3605
	public float minBreakVelocity;

	// Token: 0x04000E16 RID: 3606
	public GameObject instantiateOnBreak;

	// Token: 0x04000E17 RID: 3607
	public Transform instantiatePoint;

	// Token: 0x04000E18 RID: 3608
	public bool stickToNormal;

	// Token: 0x04000E19 RID: 3609
	private bool alreadyBroke;
}
