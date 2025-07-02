using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;

// Token: 0x020000DB RID: 219
public abstract class ItemComponent : MonoBehaviourPunCallbacks
{
	// Token: 0x060006A8 RID: 1704 RVA: 0x000234E1 File Offset: 0x000216E1
	public virtual void Awake()
	{
		this.item = base.GetComponent<Item>();
		this.photonView = base.GetComponent<PhotonView>();
		base.StartCoroutine(this.InitializeNextFrame());
	}

	// Token: 0x060006A9 RID: 1705 RVA: 0x00023508 File Offset: 0x00021708
	public IEnumerator InitializeNextFrame()
	{
		yield return null;
		yield break;
	}

	// Token: 0x060006AA RID: 1706 RVA: 0x00023510 File Offset: 0x00021710
	public T GetData<T>(DataEntryKey key) where T : DataEntryValue, new()
	{
		return this.item.GetData<T>(key);
	}

	// Token: 0x060006AB RID: 1707 RVA: 0x0002351E File Offset: 0x0002171E
	public T GetData<T>(DataEntryKey key, Func<T> getNew) where T : DataEntryValue, new()
	{
		return this.item.GetData<T>(key, getNew);
	}

	// Token: 0x060006AC RID: 1708 RVA: 0x0002352D File Offset: 0x0002172D
	public bool HasData(DataEntryKey key)
	{
		return this.item.data != null && this.item.data.HasData(key);
	}

	// Token: 0x060006AD RID: 1709
	public abstract void OnInstanceDataSet();

	// Token: 0x060006AE RID: 1710 RVA: 0x0002354F File Offset: 0x0002174F
	public void ForceSync()
	{
		if (!this.photonView.IsMine)
		{
			Debug.LogError("Not allowed to force sync an object you don't own..");
			return;
		}
		this.photonView.RPC("SetItemInstanceDataRPC", RpcTarget.Others, new object[] { this.item.data });
	}

	// Token: 0x04000658 RID: 1624
	[NonSerialized]
	public Item item;

	// Token: 0x04000659 RID: 1625
	protected new PhotonView photonView;
}
