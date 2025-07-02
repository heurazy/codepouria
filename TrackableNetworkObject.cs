using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000145 RID: 325
[RequireComponent(typeof(PhotonView))]
public class TrackableNetworkObject : ItemComponent
{
	// Token: 0x17000076 RID: 118
	// (get) Token: 0x0600094A RID: 2378 RVA: 0x0002EF6E File Offset: 0x0002D16E
	// (set) Token: 0x0600094B RID: 2379 RVA: 0x0002EF76 File Offset: 0x0002D176
	public new PhotonView photonView { get; private set; }

	// Token: 0x17000077 RID: 119
	// (get) Token: 0x0600094C RID: 2380 RVA: 0x0002EF7F File Offset: 0x0002D17F
	public bool hasTracker
	{
		get
		{
			return this.currentTracker != null;
		}
	}

	// Token: 0x0600094D RID: 2381 RVA: 0x0002EF8D File Offset: 0x0002D18D
	public override void Awake()
	{
		base.Awake();
		this.photonView = base.GetComponent<PhotonView>();
	}

	// Token: 0x0600094E RID: 2382 RVA: 0x0002EFA1 File Offset: 0x0002D1A1
	private new void OnEnable()
	{
		TrackableNetworkObject.ALL_TRACKABLES.Add(this);
	}

	// Token: 0x0600094F RID: 2383 RVA: 0x0002EFAE File Offset: 0x0002D1AE
	public new void OnDisable()
	{
		TrackableNetworkObject.ALL_TRACKABLES.Remove(this);
	}

	// Token: 0x06000950 RID: 2384 RVA: 0x0002EFBC File Offset: 0x0002D1BC
	private void Start()
	{
		this.Init();
		if (TrackableNetworkObject.OnTrackableObjectCreated != null)
		{
			Debug.Log(string.Format("OnTrackableObjectCreated on photon view {0} with instance ID {1}", this.photonView.ViewID, this.instanceID));
			TrackableNetworkObject.OnTrackableObjectCreated(this.instanceID);
		}
	}

	// Token: 0x06000951 RID: 2385 RVA: 0x0002F010 File Offset: 0x0002D210
	public override void OnJoinedRoom()
	{
		this.Init();
		if (TrackableNetworkObject.OnTrackableObjectCreated != null)
		{
			TrackableNetworkObject.OnTrackableObjectCreated(this.instanceID);
		}
	}

	// Token: 0x06000952 RID: 2386 RVA: 0x0002F030 File Offset: 0x0002D230
	public static TrackableNetworkObject GetTrackableObject(int instanceID)
	{
		for (int i = 0; i < TrackableNetworkObject.ALL_TRACKABLES.Count; i++)
		{
			if (TrackableNetworkObject.ALL_TRACKABLES[i] != null && TrackableNetworkObject.ALL_TRACKABLES[i].instanceID == instanceID)
			{
				return TrackableNetworkObject.ALL_TRACKABLES[i];
			}
		}
		return null;
	}

	// Token: 0x06000953 RID: 2387 RVA: 0x0002F088 File Offset: 0x0002D288
	private void Init()
	{
		if (!PhotonNetwork.InRoom || !PhotonNetwork.IsMasterClient)
		{
			return;
		}
		if (base.GetData<IntItemData>(DataEntryKey.InstanceID).Value == 0)
		{
			this.instanceID = TrackableNetworkObject.currentMaxInstanceID;
			TrackableNetworkObject.currentMaxInstanceID++;
			this.photonView.RPC("SetInstanceIDRPC", RpcTarget.All, new object[]
			{
				this.instanceID,
				TrackableNetworkObject.currentMaxInstanceID
			});
			Debug.Log(string.Format("Setting instance id to {0}", this.instanceID));
		}
	}

	// Token: 0x06000954 RID: 2388 RVA: 0x0002F115 File Offset: 0x0002D315
	public override void OnInstanceDataSet()
	{
		this.instanceID = base.GetData<IntItemData>(DataEntryKey.InstanceID).Value;
	}

	// Token: 0x06000955 RID: 2389 RVA: 0x0002F129 File Offset: 0x0002D329
	[PunRPC]
	public void SetInstanceIDRPC(int instanceID, int maxInstanceID)
	{
		base.GetData<IntItemData>(DataEntryKey.InstanceID).Value = instanceID;
		TrackableNetworkObject.currentMaxInstanceID = maxInstanceID;
		Debug.Log(string.Format("ACTUALLY Setting instance id to {0}", instanceID));
	}

	// Token: 0x04000841 RID: 2113
	public static List<TrackableNetworkObject> ALL_TRACKABLES = new List<TrackableNetworkObject>();

	// Token: 0x04000842 RID: 2114
	public int instanceID;

	// Token: 0x04000843 RID: 2115
	private static int currentMaxInstanceID = 1;

	// Token: 0x04000844 RID: 2116
	public TrackNetworkedObject currentTracker;

	// Token: 0x04000846 RID: 2118
	public static Action<int> OnTrackableObjectCreated;
}
