using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000005 RID: 5
public class Beehive : ItemComponent
{
	// Token: 0x0600001B RID: 27 RVA: 0x00002487 File Offset: 0x00000687
	public override void OnJoinedRoom()
	{
		this.Init();
	}

	// Token: 0x0600001C RID: 28 RVA: 0x0000248F File Offset: 0x0000068F
	public void Start()
	{
		if (PhotonNetwork.InRoom)
		{
			this.Init();
		}
	}

	// Token: 0x0600001D RID: 29 RVA: 0x000024A0 File Offset: 0x000006A0
	private void Init()
	{
		if (!this.initialized)
		{
			this.initialized = true;
			if (!PhotonNetwork.IsMasterClient)
			{
				return;
			}
			if (!base.HasData(DataEntryKey.InstanceID))
			{
				this.instanceID = Beehive.currentMaxInstanceID;
				Beehive.currentMaxInstanceID++;
				this.photonView.RPC("SetInstanceIDRPC", RpcTarget.All, new object[]
				{
					this.instanceID,
					Beehive.currentMaxInstanceID
				});
			}
			if (!base.HasData(DataEntryKey.SpawnedBees) && this.spawnBees)
			{
				this.currentBees = PhotonNetwork.Instantiate(this.beeSwarmPrefab.gameObject.name, base.transform.position, Quaternion.identity, 0, null).GetComponent<BeeSwarm>();
				this.currentBees.SetBeehive(this);
				base.GetData<BoolItemData>(DataEntryKey.SpawnedBees).Value = true;
			}
		}
	}

	// Token: 0x0600001E RID: 30 RVA: 0x00002575 File Offset: 0x00000775
	public override void OnInstanceDataSet()
	{
		this.instanceID = base.GetData<IntItemData>(DataEntryKey.InstanceID).Value;
	}

	// Token: 0x0600001F RID: 31 RVA: 0x00002589 File Offset: 0x00000789
	[PunRPC]
	public void SetInstanceIDRPC(int instanceID, int maxInstanceID)
	{
		base.GetData<IntItemData>(DataEntryKey.InstanceID).Value = instanceID;
		Beehive.currentMaxInstanceID = maxInstanceID;
	}

	// Token: 0x06000020 RID: 32 RVA: 0x0000259E File Offset: 0x0000079E
	public override void OnEnable()
	{
		base.OnEnable();
		Beehive.ALL_BEEHIVES.Add(this);
	}

	// Token: 0x06000021 RID: 33 RVA: 0x000025B1 File Offset: 0x000007B1
	public override void OnDisable()
	{
		base.OnDisable();
		Beehive.ALL_BEEHIVES.Remove(this);
	}

	// Token: 0x06000022 RID: 34 RVA: 0x000025C8 File Offset: 0x000007C8
	public static Beehive GetBeehive(int instanceID)
	{
		for (int i = 0; i < Beehive.ALL_BEEHIVES.Count; i++)
		{
			if (Beehive.ALL_BEEHIVES[i] != null && Beehive.ALL_BEEHIVES[i].instanceID == instanceID)
			{
				return Beehive.ALL_BEEHIVES[i];
			}
		}
		return null;
	}

	// Token: 0x06000023 RID: 35 RVA: 0x0000261D File Offset: 0x0000081D
	private void OnDestroy()
	{
		if (!PhotonNetwork.IsMasterClient)
		{
			return;
		}
		if (this.currentBees != null)
		{
			this.currentBees.HiveDestroyed(this.item.Center());
		}
	}

	// Token: 0x04000007 RID: 7
	public bool spawnBees = true;

	// Token: 0x04000008 RID: 8
	public BeeSwarm beeSwarmPrefab;

	// Token: 0x04000009 RID: 9
	public BeeSwarm currentBees;

	// Token: 0x0400000A RID: 10
	public int instanceID;

	// Token: 0x0400000B RID: 11
	private static int currentMaxInstanceID = 1;

	// Token: 0x0400000C RID: 12
	public static List<Beehive> ALL_BEEHIVES = new List<Beehive>();

	// Token: 0x0400000D RID: 13
	private bool initialized;
}
