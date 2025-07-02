using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Photon.Pun;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000015 RID: 21
public class Guidebook : Item
{
	// Token: 0x0600018B RID: 395 RVA: 0x0000CB2F File Offset: 0x0000AD2F
	public override void OnEnable()
	{
		base.OnEnable();
		if (this.isSinglePage)
		{
			base.Invoke("OpenSinglePage", 0.01f);
		}
	}

	// Token: 0x0600018C RID: 396 RVA: 0x0000CB50 File Offset: 0x0000AD50
	private void OpenSinglePage()
	{
		RenderTexture renderTexture = new RenderTexture(this.guidebookRenderTexture);
		renderTexture.Create();
		this.guidebookRenderTexture = renderTexture;
		this.currentRenderTexture = renderTexture;
		this.renderCamera.targetTexture = this.guidebookRenderTexture;
		this.currentlyVisibleLeftPageIndex = 2;
		this.currentlyVisibleRightPageIndex = 3;
		this.nextVisibleLeftPageIndex = 0;
		this.nextVisibleRightPageIndex = 1;
		this.UpdatePageDisplay();
		for (int i = 0; i < this.pageRenderers.Length; i++)
		{
			this.pageRenderers[i].material.SetTexture(Guidebook.BASETEX, this.currentRenderTexture);
		}
	}

	// Token: 0x0600018D RID: 397 RVA: 0x0000CBE1 File Offset: 0x0000ADE1
	public override void OnDisable()
	{
		base.OnDisable();
		if (this.isSinglePage)
		{
			Object.Destroy(this.renderCamera.targetTexture);
		}
	}

	// Token: 0x0600018E RID: 398 RVA: 0x0000CC01 File Offset: 0x0000AE01
	internal void ToggleGuidebook()
	{
		if (base.photonView.IsMine)
		{
			base.photonView.RPC("ToggleGuidebook_RPC", RpcTarget.All, new object[] { !this.isOpen });
		}
	}

	// Token: 0x0600018F RID: 399 RVA: 0x0000CC38 File Offset: 0x0000AE38
	[PunRPC]
	public void ToggleGuidebook_RPC(bool open)
	{
		this.isOpen = open;
		if (this.isOpen)
		{
			if (!this.isSinglePage)
			{
				this.anim.Play("Open", 0, 0f);
			}
			this.coll.enabled = false;
			this.renderCamera.targetTexture = this.guidebookRenderTexture;
			this.currentlyVisibleLeftPageIndex = 2;
			this.currentlyVisibleRightPageIndex = 3;
			this.nextVisibleLeftPageIndex = 0;
			this.nextVisibleRightPageIndex = 1;
			this.UpdatePageDisplay();
			for (int i = 0; i < this.pageRenderers.Length; i++)
			{
				this.pageRenderers[i].material.SetTexture(Guidebook.BASETEX, this.currentRenderTexture);
			}
			return;
		}
		if (!this.isSinglePage)
		{
			this.anim.Play("Close", 0, 0f);
		}
		this.coll.enabled = true;
		this.bookTransform.DOLocalMove(Vector3.zero, 0.25f, false);
		this.bookTransform.DOLocalRotate(Vector3.zero, 0.25f, RotateMode.Fast);
		for (int j = 0; j < this.pageRenderers.Length; j++)
		{
			this.pageRenderers[j].material.SetTexture(Guidebook.BASETEX, this.currentRenderTexture);
		}
	}

	// Token: 0x06000190 RID: 400 RVA: 0x0000CD70 File Offset: 0x0000AF70
	private void LateUpdate()
	{
		if (this.isOpen && base.holderCharacter.IsLocal)
		{
			this.bookTransform.position = Vector3.Lerp(this.bookTransform.position, MainCamera.instance.cam.transform.position + MainCamera.instance.cam.transform.forward * this.readingDistance, Time.deltaTime * 10f);
			this.bookTransform.forward = MainCamera.instance.cam.transform.forward;
		}
	}

	// Token: 0x06000191 RID: 401 RVA: 0x0000CE12 File Offset: 0x0000B012
	private void PopulatePages()
	{
		this.pageSpreads = base.GetComponentsInChildren<GuidebookSpread>(true).ToList<GuidebookSpread>();
	}

	// Token: 0x06000192 RID: 402 RVA: 0x0000CE28 File Offset: 0x0000B028
	private void PopulatePageNumbers()
	{
		for (int i = 0; i < this.pageSpreads.Count; i++)
		{
		}
	}

	// Token: 0x06000193 RID: 403 RVA: 0x0000CE4C File Offset: 0x0000B04C
	internal void FlipPageRight()
	{
		if (base.photonView.IsMine && this.currentPageSet < this.pageSpreads.Count - 1)
		{
			this.currentPageSet++;
			base.photonView.RPC("FlipPageRight_RPC", RpcTarget.All, new object[] { this.currentPageSet });
		}
	}

	// Token: 0x06000194 RID: 404 RVA: 0x0000CEB0 File Offset: 0x0000B0B0
	internal void FlipPageLeft()
	{
		if (base.photonView.IsMine && this.currentPageSet >= 1)
		{
			this.currentPageSet--;
			base.photonView.RPC("FlipPageLeft_RPC", RpcTarget.All, new object[] { this.currentPageSet });
		}
	}

	// Token: 0x06000195 RID: 405 RVA: 0x0000CF08 File Offset: 0x0000B108
	[PunRPC]
	public void FlipPageRight_RPC(int currentPage)
	{
		this.currentlyVisibleLeftPageIndex = 2;
		this.currentlyVisibleRightPageIndex = 3;
		this.nextVisibleLeftPageIndex = 4;
		this.nextVisibleRightPageIndex = 5;
		this.anim.Play("Guidebook_FlipRight", 0, 0f);
		this.currentPageSet = currentPage;
		this.UpdatePageDisplay();
	}

	// Token: 0x06000196 RID: 406 RVA: 0x0000CF54 File Offset: 0x0000B154
	[PunRPC]
	public void FlipPageLeft_RPC(int currentPage)
	{
		this.currentlyVisibleLeftPageIndex = 2;
		this.currentlyVisibleRightPageIndex = 3;
		this.nextVisibleLeftPageIndex = 0;
		this.nextVisibleRightPageIndex = 1;
		this.anim.Play("Guidebook_FlipLeft", 0, 0f);
		this.currentPageSet = currentPage;
		this.UpdatePageDisplay();
	}

	// Token: 0x06000197 RID: 407 RVA: 0x0000CFA0 File Offset: 0x0000B1A0
	private void UpdatePageDisplay()
	{
		Graphics.CopyTexture(this.currentRenderTexture, this.lastRenderTexture);
		for (int i = 0; i < this.pageSpreads.Count; i++)
		{
			this.pageSpreads[i].gameObject.SetActive(i == this.currentPageSet);
		}
		this.renderCamera.Render();
		this.pageRenderers[this.currentlyVisibleLeftPageIndex].material.SetTexture(Guidebook.BASETEX, this.lastRenderTexture);
		this.pageRenderers[this.currentlyVisibleRightPageIndex].material.SetTexture(Guidebook.BASETEX, this.lastRenderTexture);
		this.pageRenderers[this.nextVisibleLeftPageIndex].material.SetTexture(Guidebook.BASETEX, this.currentRenderTexture);
		this.pageRenderers[this.nextVisibleRightPageIndex].material.SetTexture(Guidebook.BASETEX, this.currentRenderTexture);
	}

	// Token: 0x04000194 RID: 404
	public static int BASETEX = Shader.PropertyToID("_BaseTexture");

	// Token: 0x04000195 RID: 405
	public bool isSinglePage;

	// Token: 0x04000196 RID: 406
	public Animator anim;

	// Token: 0x04000197 RID: 407
	public int currentPageSet;

	// Token: 0x04000198 RID: 408
	[FormerlySerializedAs("pages")]
	[PreviouslySerializedAs("pages")]
	public List<GuidebookSpread> pageSpreads;

	// Token: 0x04000199 RID: 409
	public List<RectTransform> pagePrefabs;

	// Token: 0x0400019A RID: 410
	public Camera renderCamera;

	// Token: 0x0400019B RID: 411
	public Texture currentRenderTexture;

	// Token: 0x0400019C RID: 412
	public Texture lastRenderTexture;

	// Token: 0x0400019D RID: 413
	public Renderer[] pageRenderers;

	// Token: 0x0400019E RID: 414
	public Transform bookTransform;

	// Token: 0x0400019F RID: 415
	public float readingDistance = 0.4f;

	// Token: 0x040001A0 RID: 416
	public Collider coll;

	// Token: 0x040001A1 RID: 417
	[HideInInspector]
	public bool isOpen;

	// Token: 0x040001A2 RID: 418
	public RenderTexture guidebookRenderTexture;

	// Token: 0x040001A3 RID: 419
	private int currentlyVisibleLeftPageIndex;

	// Token: 0x040001A4 RID: 420
	private int currentlyVisibleRightPageIndex;

	// Token: 0x040001A5 RID: 421
	private int nextVisibleLeftPageIndex;

	// Token: 0x040001A6 RID: 422
	private int nextVisibleRightPageIndex;
}
