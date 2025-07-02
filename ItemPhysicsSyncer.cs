using System;
using Photon.Pun;
using UnityEngine;
using Zorro.Core;

// Token: 0x02000105 RID: 261
public class ItemPhysicsSyncer : PhotonBinaryStreamSerializer<ItemPhysicsSyncData>
{
	// Token: 0x060007BD RID: 1981 RVA: 0x00028E42 File Offset: 0x00027042
	protected override void Awake()
	{
		base.Awake();
		this.m_photonView = base.GetComponent<PhotonView>();
		this.m_item = base.GetComponent<Item>();
	}

	// Token: 0x060007BE RID: 1982 RVA: 0x00028E62 File Offset: 0x00027062
	public void ForceSyncForFrames()
	{
		this.forceSyncFrames = 10;
	}

	// Token: 0x060007BF RID: 1983 RVA: 0x00028E6C File Offset: 0x0002706C
	private void FixedUpdate()
	{
		Rigidbody rig = this.m_item.rig;
		if (rig == null)
		{
			return;
		}
		if (this.m_photonView.IsMine)
		{
			return;
		}
		if (this.RemoteValue.IsNone)
		{
			return;
		}
		if (this.m_item.itemState != ItemState.Ground)
		{
			return;
		}
		if (this.m_lastPos.IsNone)
		{
			return;
		}
		double num = (double)(1f / (float)PhotonNetwork.SerializationRate);
		this.sinceLastPackage += Time.fixedDeltaTime * 0.6f;
		float num2 = (float)((double)this.sinceLastPackage / num);
		ItemPhysicsSyncData value = this.RemoteValue.Value;
		Vector3 vector = value.position;
		Vector3 vector2 = Vector3.Lerp(this.m_lastPos.Value, vector, num2);
		Vector3 vector3 = vector2 - rig.position;
		this.lastRecievedPosition = vector2;
		rig.MovePosition(rig.position + vector3 * 0.5f);
		rig.MoveRotation(Quaternion.RotateTowards(rig.rotation, value.rotation, Time.fixedDeltaTime * 90f));
	}

	// Token: 0x060007C0 RID: 1984 RVA: 0x00028F7C File Offset: 0x0002717C
	public override ItemPhysicsSyncData GetDataToWrite()
	{
		ItemPhysicsSyncData itemPhysicsSyncData = default(ItemPhysicsSyncData);
		Rigidbody rig = this.m_item.rig;
		if (rig != null)
		{
			itemPhysicsSyncData.linearVelocity = rig.linearVelocity;
			itemPhysicsSyncData.angularVelocity = rig.angularVelocity;
			itemPhysicsSyncData.position = rig.position;
			itemPhysicsSyncData.rotation = rig.rotation;
		}
		return itemPhysicsSyncData;
	}

	// Token: 0x060007C1 RID: 1985 RVA: 0x00028FEC File Offset: 0x000271EC
	public override bool ShouldSendData()
	{
		if (this.forceSyncFrames > 0 && this.m_item.itemState == ItemState.Ground)
		{
			this.forceSyncFrames--;
			return true;
		}
		return !this.m_item.rig.isKinematic && !this.m_item.rig.IsSleeping() && this.m_item.itemState == ItemState.Ground;
	}

	// Token: 0x060007C2 RID: 1986 RVA: 0x00029054 File Offset: 0x00027254
	public override void OnDataReceived(ItemPhysicsSyncData data)
	{
		base.OnDataReceived(data);
		Rigidbody rig = this.m_item.rig;
		if (rig == null)
		{
			return;
		}
		if (this.m_item.itemState != ItemState.Ground)
		{
			return;
		}
		if (rig.isKinematic)
		{
			return;
		}
		this.m_lastPos = Optionable<Vector3>.Some(rig.position);
		rig.linearVelocity = data.linearVelocity;
		rig.angularVelocity = data.angularVelocity;
		this.lastRecievedLinearVelocity = data.linearVelocity;
		this.lastRecievedAngularVelocity = data.angularVelocity;
	}

	// Token: 0x04000730 RID: 1840
	private Item m_item;

	// Token: 0x04000731 RID: 1841
	private PhotonView m_photonView;

	// Token: 0x04000732 RID: 1842
	private Optionable<Vector3> m_lastPos;

	// Token: 0x04000733 RID: 1843
	private Optionable<ItemState> m_lastState;

	// Token: 0x04000734 RID: 1844
	private Coroutine m_fadeRoutine;

	// Token: 0x04000735 RID: 1845
	private int forceSyncFrames;

	// Token: 0x04000736 RID: 1846
	[SerializeField]
	private Vector3 lastRecievedLinearVelocity;

	// Token: 0x04000737 RID: 1847
	[SerializeField]
	private Vector3 lastRecievedAngularVelocity;

	// Token: 0x04000738 RID: 1848
	[SerializeField]
	private Vector3 lastRecievedPosition;
}
