using System;
using Photon.Pun;
using pworld.Scripts.Extensions;
using UnityEngine;

// Token: 0x0200021E RID: 542
public class PointPinger : MonoBehaviour
{
	// Token: 0x06000DDE RID: 3550 RVA: 0x00046086 File Offset: 0x00044286
	private void Awake()
	{
		this.character = base.GetComponent<Character>();
		this.photonView = base.GetComponent<PhotonView>();
	}

	// Token: 0x06000DDF RID: 3551 RVA: 0x000460A0 File Offset: 0x000442A0
	private void Start()
	{
	}

	// Token: 0x06000DE0 RID: 3552 RVA: 0x000460A4 File Offset: 0x000442A4
	private void Update()
	{
		if (!this.photonView.IsMine)
		{
			return;
		}
		this.coolDownLeft -= Time.deltaTime;
		if (this.coolDownLeft > 0f)
		{
			return;
		}
		if (!this.character.input.pingWasPressed)
		{
			return;
		}
		if (this.character.data.dead)
		{
			return;
		}
		RaycastHit raycastHit;
		if (Camera.main.ScreenPointToRay(Input.mousePosition).Raycast(out raycastHit, HelperFunctions.LayerType.TerrainMap.ToLayerMask(), -1f))
		{
			this.coolDownLeft = this.coolDown;
			this.photonView.RPC("ReceivePoint_Rpc", RpcTarget.All, new object[] { raycastHit.point, raycastHit.normal });
		}
	}

	// Token: 0x06000DE1 RID: 3553 RVA: 0x0004616C File Offset: 0x0004436C
	[PunRPC]
	private void ReceivePoint_Rpc(Vector3 point, Vector3 hitNormal)
	{
		RaycastHit raycastHit;
		bool flag = PExt.LineCast(this.character.Head, Character.localCharacter.Head, out raycastHit, HelperFunctions.LayerType.TerrainMap.ToLayerMask(), true);
		float num = Vector3.Distance(this.character.Head, Character.localCharacter.Head);
		PointPing component = this.pointPrefab.GetComponent<PointPing>();
		Vector2 visibilityFullNoneNoLos = component.visibilityFullNoneNoLos;
		float num2 = 1f - Mathf.InverseLerp(visibilityFullNoneNoLos.x, visibilityFullNoneNoLos.x + (visibilityFullNoneNoLos.y - visibilityFullNoneNoLos.x) * (flag ? component.NoLosVisibilityMul : 1f), num);
		if (num2 <= 0f)
		{
			return;
		}
		if (this.pingInstance != null)
		{
			Object.DestroyImmediate(this.pingInstance);
		}
		this.pingInstance = Object.Instantiate<GameObject>(this.pointPrefab, point, Quaternion.LookRotation((point - this.character.Head).normalized, Vector3.up));
		PointPing component2 = this.pingInstance.GetComponent<PointPing>();
		component2.hitNormal = hitNormal;
		component2.pointPinger = this;
		component2.renderer.material = Object.Instantiate<Material>(this.character.refs.mainRenderer.sharedMaterial);
		component2.material.SetFloat("_Opacity", num2);
		Object.Destroy(this.pingInstance, 2f);
	}

	// Token: 0x04000CFB RID: 3323
	public GameObject pointPrefab;

	// Token: 0x04000CFC RID: 3324
	public float coolDown;

	// Token: 0x04000CFD RID: 3325
	public Character character;

	// Token: 0x04000CFE RID: 3326
	private GameObject pingInstance;

	// Token: 0x04000CFF RID: 3327
	private float coolDownLeft = 1f;

	// Token: 0x04000D00 RID: 3328
	private PhotonView photonView;
}
