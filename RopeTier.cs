using System;
using Photon.Pun;
using UnityEngine;
using Zorro.Core;

// Token: 0x0200011F RID: 287
public class RopeTier : ItemComponent
{
	// Token: 0x06000871 RID: 2161 RVA: 0x0002CF0C File Offset: 0x0002B10C
	private new void Awake()
	{
		this.view = base.GetComponent<PhotonView>();
		this.item = base.GetComponent<Item>();
		this.spool = base.GetComponent<RopeSpool>();
	}

	// Token: 0x06000872 RID: 2162 RVA: 0x0002CF32 File Offset: 0x0002B132
	public override void OnInstanceDataSet()
	{
	}

	// Token: 0x17000073 RID: 115
	// (get) Token: 0x06000873 RID: 2163 RVA: 0x0002CF34 File Offset: 0x0002B134
	public bool LookingToPlaceAnchor
	{
		get
		{
			return this.ropeAnchor != null;
		}
	}

	// Token: 0x06000874 RID: 2164 RVA: 0x0002CF42 File Offset: 0x0002B142
	private void OnDestroy()
	{
		if (this.ropeAnchor)
		{
			Object.DestroyImmediate(this.ropeAnchor.gameObject);
		}
	}

	// Token: 0x06000875 RID: 2165 RVA: 0x0002CF64 File Offset: 0x0002B164
	public void Update()
	{
		if (!this.view.IsMine)
		{
			return;
		}
		if (this.item.itemState != ItemState.Held)
		{
			return;
		}
		if (this.releaseCheck)
		{
			if (Character.localCharacter.input.usePrimaryWasReleased)
			{
				this.releaseCheck = false;
			}
			return;
		}
		if (!Character.localCharacter.input.usePrimaryIsPressed)
		{
			this.item.overrideProgress = 0f;
			this.item.overrideForceProgress = false;
			if (this.ropeAnchor != null)
			{
				Object.DestroyImmediate(this.ropeAnchor.gameObject);
			}
			return;
		}
		if (this.ropeAnchor != null && this.goodAnchorPlace != null && Vector3.Distance(this.goodAnchorPlace.Value.point, base.transform.position) > this.maxAnchorGhostDistance)
		{
			this.item.overrideProgress = 0f;
			this.item.overrideForceProgress = false;
			Object.DestroyImmediate(this.ropeAnchor.gameObject);
			return;
		}
		if (this.ropeAnchor == null)
		{
			this.ropeAnchor = Object.Instantiate<GameObject>(this.anchorPreview).GetComponent<RopeAnchor>();
			this.ropeAnchor.anchorPoint.gameObject.SetActive(false);
			this.goodAnchorPlace = null;
			this.timeWithGoodAnchor = 0f;
		}
		if (this.goodAnchorPlace == null)
		{
			RaycastHit raycastHit = HelperFunctions.LineCheck(Camera.main.transform.position, Camera.main.transform.position + Camera.main.transform.forward * this.maxAnchorGhostDistance, HelperFunctions.LayerType.TerrainMap, 0f, QueryTriggerInteraction.Ignore);
			Debug.DrawLine(Camera.main.transform.position, raycastHit.point, Color.red);
			if (raycastHit.collider == null)
			{
				return;
			}
			if (this.item == null)
			{
				Debug.Log("Item is null");
			}
			if (this.item.holderCharacter == null)
			{
				Debug.Log("Item holder is null");
			}
			float num = Vector3.Distance(raycastHit.point, this.item.holderCharacter.Center);
			this.ropeAnchor.Ghost = true;
			this.ropeAnchor.transform.position = raycastHit.point;
			this.ropeAnchor.transform.up = raycastHit.normal;
			this.ropeAnchor.transform.forward = Vector3.Cross(Camera.main.transform.right, raycastHit.normal);
			if (num < this.maxAnchorDistance)
			{
				this.goodAnchorPlace = new RaycastHit?(raycastHit);
				this.ropeAnchor.Ghost = false;
			}
			return;
		}
		else
		{
			this.item.overrideForceProgress = false;
			if (this.goodAnchorPlace == null)
			{
				return;
			}
			this.timeWithGoodAnchor += Time.deltaTime;
			this.item.overrideForceProgress = true;
			this.item.overrideProgress = this.timeWithGoodAnchor / this.castTime;
			if (this.timeWithGoodAnchor < this.castTime)
			{
				return;
			}
			Debug.Log("Cast anchor");
			this.item.overrideForceProgress = false;
			this.item.overrideProgress = 0f;
			GameObject gameObject = PhotonNetwork.Instantiate(this.anchorPrefab.name, this.ropeAnchor.transform.position, this.ropeAnchor.transform.rotation, 0, null);
			if (this.item.photonView.IsMine)
			{
				Singleton<AchievementManager>.Instance.AddToRunBasedFloat(RUNBASEDVALUETYPE.RopePlaced, this.spool.rope.GetLengthInMeters());
				GameUtils.instance.IncrementPermanentItemsPlaced();
			}
			this.spool.rope.photonView.RPC("AttachToAnchor_Rpc", RpcTarget.AllBuffered, new object[] { gameObject.GetComponent<PhotonView>() });
			Object.DestroyImmediate(this.ropeAnchor.gameObject);
			this.releaseCheck = true;
			this.ropeAnchor = null;
			return;
		}
	}

	// Token: 0x040007E3 RID: 2019
	public GameObject anchorPreview;

	// Token: 0x040007E4 RID: 2020
	public GameObject anchorPrefab;

	// Token: 0x040007E5 RID: 2021
	public float maxAnchorGhostDistance = 10f;

	// Token: 0x040007E6 RID: 2022
	public float maxAnchorDistance = 5f;

	// Token: 0x040007E7 RID: 2023
	public float castTime;

	// Token: 0x040007E8 RID: 2024
	private RaycastHit? goodAnchorPlace;

	// Token: 0x040007E9 RID: 2025
	public float timeWithGoodAnchor;

	// Token: 0x040007EA RID: 2026
	private new Item item;

	// Token: 0x040007EB RID: 2027
	private RopeSpool spool;

	// Token: 0x040007EC RID: 2028
	public RopeAnchor ropeAnchor;

	// Token: 0x040007ED RID: 2029
	private PhotonView view;

	// Token: 0x040007EE RID: 2030
	private bool releaseCheck;
}
