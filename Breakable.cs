using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x020000C8 RID: 200
[RequireComponent(typeof(PhotonView))]
public class Breakable : MonoBehaviour
{
	// Token: 0x06000648 RID: 1608 RVA: 0x00021F45 File Offset: 0x00020145
	private void Awake()
	{
		this.item = base.GetComponent<Item>();
	}

	// Token: 0x06000649 RID: 1609 RVA: 0x00021F54 File Offset: 0x00020154
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

	// Token: 0x0600064A RID: 1610 RVA: 0x00021FB8 File Offset: 0x000201B8
	public virtual void Break(Collision coll)
	{
		if (this.alreadyBroke)
		{
			return;
		}
		this.alreadyBroke = true;
		for (int i = 0; i < this.breakSFX.Count; i++)
		{
			this.breakSFX[i].Play(base.transform.position);
		}
		for (int j = 0; j < this.instantiateOnBreak.Count; j++)
		{
			Item component = PhotonNetwork.Instantiate("0_Items/" + this.instantiateOnBreak[j].name, this.instantiatePoints[j].position, this.instantiatePoints[j].rotation, 0, null).GetComponent<Item>();
			if (component)
			{
				IntItemData intItemData;
				if (this.item.data.TryGetDataEntry<IntItemData>(DataEntryKey.CookedAmount, out intItemData))
				{
					component.photonView.RPC("SetCookedAmountRPC", RpcTarget.All, new object[] { intItemData.Value });
				}
				if (this.spawnsItemsKinematic)
				{
					component.rig.isKinematic = true;
					component.transform.position = coll.contacts[0].point;
					component.transform.up = coll.contacts[0].normal;
				}
				else
				{
					component.rig.linearVelocity = this.item.rig.linearVelocity;
					component.rig.angularVelocity = this.item.rig.angularVelocity;
				}
				if (this.playAnimationOnInstantiatedObject)
				{
					Animator componentInChildren = component.GetComponentInChildren<Animator>();
					if (componentInChildren)
					{
						componentInChildren.Play(this.animString, 0, 0f);
					}
				}
			}
		}
		for (int k = 0; k < this.instantiateNonItemOnBreak.Count; k++)
		{
			Rigidbody component2 = Object.Instantiate<GameObject>(this.instantiateNonItemOnBreak[k], base.transform.position, base.transform.rotation).GetComponent<Rigidbody>();
			if (component2)
			{
				component2.linearVelocity = this.item.rig.linearVelocity;
				component2.angularVelocity = this.item.rig.angularVelocity;
			}
		}
		PhotonNetwork.Destroy(base.gameObject);
	}

	// Token: 0x0400061B RID: 1563
	private Item item;

	// Token: 0x0400061C RID: 1564
	public bool breakOnCollision;

	// Token: 0x0400061D RID: 1565
	public float minBreakVelocity;

	// Token: 0x0400061E RID: 1566
	public List<GameObject> instantiateOnBreak;

	// Token: 0x0400061F RID: 1567
	public List<SFX_Instance> breakSFX;

	// Token: 0x04000620 RID: 1568
	public List<GameObject> instantiateNonItemOnBreak;

	// Token: 0x04000621 RID: 1569
	public List<Transform> instantiatePoints;

	// Token: 0x04000622 RID: 1570
	public bool spawnsItemsKinematic;

	// Token: 0x04000623 RID: 1571
	public bool playAnimationOnInstantiatedObject;

	// Token: 0x04000624 RID: 1572
	public string animString;

	// Token: 0x04000625 RID: 1573
	private bool alreadyBroke;
}
