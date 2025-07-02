using System;
using Photon.Pun;
using UnityEngine;
using Zorro.Core;

// Token: 0x020000CC RID: 204
public class Constructable : ItemComponent
{
	// Token: 0x06000657 RID: 1623 RVA: 0x0002248D File Offset: 0x0002068D
	public override void OnInstanceDataSet()
	{
	}

	// Token: 0x06000658 RID: 1624 RVA: 0x00022490 File Offset: 0x00020690
	protected virtual void Update()
	{
		if (this.item.holderCharacter && this.item.holderCharacter.IsLocal)
		{
			if (!this.constructing)
			{
				this.TryUpdatePreview();
			}
			else if (this.constructing && Vector3.Distance(MainCamera.instance.transform.position, this.currentConstructHit.point) > this.maxConstructDistance)
			{
				this.DestroyPreview();
				this.item.CancelUsePrimary();
			}
		}
		else
		{
			this.DestroyPreview();
		}
		if (!this.valid)
		{
			this.item.overrideUsability = Optionable<bool>.Some(false);
			return;
		}
		this.item.overrideUsability = Optionable<bool>.None;
	}

	// Token: 0x06000659 RID: 1625 RVA: 0x00022543 File Offset: 0x00020743
	private void OnDestroy()
	{
		this.DestroyPreview();
	}

	// Token: 0x0600065A RID: 1626 RVA: 0x0002254C File Offset: 0x0002074C
	public virtual void TryUpdatePreview()
	{
		RaycastHit raycastHit = HelperFunctions.LineCheckIgnoreItem(MainCamera.instance.transform.position, MainCamera.instance.transform.position + MainCamera.instance.transform.forward.normalized * this.maxConstructDistance, HelperFunctions.LayerType.TerrainMap, this.item);
		this.currentConstructHit = raycastHit;
		this.valid = this.CurrentHitIsValid();
		if (raycastHit.collider == null)
		{
			this.DestroyPreview();
			return;
		}
		this.CreateOrMovePreview();
	}

	// Token: 0x0600065B RID: 1627 RVA: 0x000225DA File Offset: 0x000207DA
	private void OnDrawGizmosSelected()
	{
		if (this.currentConstructHit.collider != null)
		{
			Gizmos.color = Color.green;
			Gizmos.DrawSphere(this.currentConstructHit.point, 0.5f);
		}
	}

	// Token: 0x0600065C RID: 1628 RVA: 0x00022610 File Offset: 0x00020810
	private void CreateOrMovePreview()
	{
		if (this.currentPreview == null)
		{
			this.currentPreview = Object.Instantiate<ConstructablePreview>(this.previewPrefab);
		}
		this.currentPreview.transform.position = this.currentConstructHit.point;
		if (this.angleToNormal)
		{
			Vector3 normalized = Vector3.ProjectOnPlane(MainCamera.instance.transform.forward, this.currentConstructHit.normal).normalized;
			this.currentPreview.transform.rotation = Quaternion.LookRotation(normalized, this.currentConstructHit.normal);
		}
		else
		{
			Vector3 normalized2 = Vector3.ProjectOnPlane(MainCamera.instance.transform.forward, Vector3.up).normalized;
			this.currentPreview.transform.rotation = Quaternion.LookRotation(normalized2, Vector3.up);
		}
		if (!this.currentPreview.CollisionValid())
		{
			this.valid = false;
		}
		this.currentPreview.SetValid(this.valid);
	}

	// Token: 0x0600065D RID: 1629 RVA: 0x0002270C File Offset: 0x0002090C
	internal void DestroyPreview()
	{
		this.constructing = false;
		if (this.currentPreview != null)
		{
			Object.Destroy(this.currentPreview.gameObject);
		}
	}

	// Token: 0x0600065E RID: 1630 RVA: 0x00022734 File Offset: 0x00020934
	private bool CurrentHitIsValid()
	{
		return this.currentConstructHit.distance <= this.maxConstructDistance && (this.maxConstructVerticalAngle <= 0f || Vector3.Angle(Vector3.up, this.currentConstructHit.normal) <= this.maxConstructVerticalAngle);
	}

	// Token: 0x0600065F RID: 1631 RVA: 0x00022783 File Offset: 0x00020983
	public virtual void StartConstruction()
	{
		if (this.valid)
		{
			this.constructing = true;
		}
	}

	// Token: 0x06000660 RID: 1632 RVA: 0x00022794 File Offset: 0x00020994
	public virtual void FinishConstruction()
	{
		if (!this.constructing)
		{
			return;
		}
		if (this.currentPreview == null)
		{
			return;
		}
		if (this.constructedPrefab.GetComponent<PhotonView>() == null)
		{
			this.photonView.RPC("CreatePrefabRPC", RpcTarget.AllBuffered, new object[]
			{
				this.currentPreview.transform.position,
				this.currentPreview.transform.rotation
			});
		}
		else
		{
			PhotonNetwork.Instantiate(this.constructedPrefab.name, this.currentPreview.transform.position, this.currentPreview.transform.rotation, 0, null);
		}
		if (this.item.holderCharacter.IsLocal)
		{
			GameUtils.instance.IncrementPermanentItemsPlaced();
		}
	}

	// Token: 0x06000661 RID: 1633 RVA: 0x00022864 File Offset: 0x00020A64
	[PunRPC]
	protected void CreatePrefabRPC(Vector3 position, Quaternion rotation)
	{
		Object.Instantiate<GameObject>(this.constructedPrefab, position, rotation);
	}

	// Token: 0x0400062E RID: 1582
	public ConstructablePreview previewPrefab;

	// Token: 0x0400062F RID: 1583
	public GameObject constructedPrefab;

	// Token: 0x04000630 RID: 1584
	public float maxPreviewDistance;

	// Token: 0x04000631 RID: 1585
	public float maxConstructDistance;

	// Token: 0x04000632 RID: 1586
	public float maxConstructVerticalAngle;

	// Token: 0x04000633 RID: 1587
	public bool angleToNormal;

	// Token: 0x04000634 RID: 1588
	[SerializeField]
	protected ConstructablePreview currentPreview;

	// Token: 0x04000635 RID: 1589
	protected RaycastHit currentConstructHit;

	// Token: 0x04000636 RID: 1590
	protected bool constructing;

	// Token: 0x04000637 RID: 1591
	private bool valid;
}
