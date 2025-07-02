using System;
using Photon.Pun;
using pworld.Scripts.Extensions;
using UnityEngine;
using Zorro.Core;

// Token: 0x02000298 RID: 664
public class VineShooter : ItemComponent
{
	// Token: 0x06000FD2 RID: 4050 RVA: 0x000503EC File Offset: 0x0004E5EC
	public override void Awake()
	{
		this.actionReduceUses = base.GetComponent<Action_ReduceUses>();
		this.camera = Camera.main;
		base.Awake();
		Item item = this.item;
		item.OnPrimaryFinishedCast = (Action)Delegate.Combine(item.OnPrimaryFinishedCast, new Action(this.OnPrimaryFinishedCast));
	}

	// Token: 0x06000FD3 RID: 4051 RVA: 0x0005043D File Offset: 0x0004E63D
	private void OnDestroy()
	{
		Item item = this.item;
		item.OnPrimaryFinishedCast = (Action)Delegate.Remove(item.OnPrimaryFinishedCast, new Action(this.OnPrimaryFinishedCast));
	}

	// Token: 0x06000FD4 RID: 4052 RVA: 0x00050468 File Offset: 0x0004E668
	public void Update()
	{
		RaycastHit raycastHit;
		this.item.overrideUsability = Optionable<bool>.Some(this.WillAttach(out raycastHit));
	}

	// Token: 0x06000FD5 RID: 4053 RVA: 0x00050490 File Offset: 0x0004E690
	private void OnPrimaryFinishedCast()
	{
		Debug.Log("VineShooter shoot");
		RaycastHit raycastHit;
		if (!this.WillAttach(out raycastHit))
		{
			return;
		}
		if (this.disableOnFire != null)
		{
			this.disableOnFire.SetActive(false);
		}
		JungleVine component = this.vinePrefab.GetComponent<JungleVine>();
		Vector2 vector = new Vector2(component.minDown, component.maxDown);
		int num = 10;
		for (int i = 0; i < num; i++)
		{
			float num2 = -Vector2.Lerp(vector, Vector2.zero, (float)i / ((float)num - 1f)).PRndRange();
			Vector3 vector2 = this.camera.transform.position + this.camera.transform.forward * 1f;
			Vector3 vector3 = this.camera.transform.position - Vector3.up * 0.2f;
			RaycastHit raycastHit2;
			if (Physics.Raycast(vector2, Vector3.down, out raycastHit2, 4f, HelperFunctions.LayerType.TerrainMap.ToLayerMask(), QueryTriggerInteraction.UseGlobal))
			{
				vector3 = raycastHit2.point + Vector3.up * 1.5f;
			}
			Debug.Log(string.Format("from: {0}, to: {1}, hang: {2}", vector3, raycastHit.point, num2));
			Vector3 vector4;
			if (JungleVine.CheckVinePath(vector3, raycastHit.point, num2, out vector4))
			{
				JungleVine component2 = PhotonNetwork.Instantiate(this.vinePrefab.name, vector3, Quaternion.identity, 0, null).GetComponent<JungleVine>();
				component2.photonView.RPC("ForceBuildVine_RPC", RpcTarget.AllBuffered, new object[] { vector3, raycastHit.point, num2, vector4 });
				SpawnedVine spawnedVine;
				component2.TryGetComponent<SpawnedVine>(out spawnedVine);
				this.actionReduceUses.RunAction();
				component.SetRendererBounds();
				Debug.DrawLine(vector3, raycastHit.point, Color.green, 5f);
				GameUtils.instance.IncrementPermanentItemsPlaced();
				return;
			}
			Debug.DrawLine(vector3, raycastHit.point, Color.red, 5f);
		}
	}

	// Token: 0x06000FD6 RID: 4054 RVA: 0x000506B0 File Offset: 0x0004E8B0
	public bool WillAttach(out RaycastHit hit)
	{
		hit = default(RaycastHit);
		return Character.localCharacter.data.isGrounded && Physics.Raycast(MainCamera.instance.transform.position, MainCamera.instance.transform.forward, out hit, this.maxLength, HelperFunctions.LayerType.TerrainMap.ToLayerMask(), QueryTriggerInteraction.UseGlobal);
	}

	// Token: 0x06000FD7 RID: 4055 RVA: 0x00050712 File Offset: 0x0004E912
	public override void OnInstanceDataSet()
	{
	}

	// Token: 0x04000EE9 RID: 3817
	public GameObject vinePrefab;

	// Token: 0x04000EEA RID: 3818
	public GameObject disableOnFire;

	// Token: 0x04000EEB RID: 3819
	public float maxLength = 40f;

	// Token: 0x04000EEC RID: 3820
	private Camera camera;

	// Token: 0x04000EED RID: 3821
	private Action_ReduceUses actionReduceUses;
}
