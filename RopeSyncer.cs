using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zorro.Core;

// Token: 0x0200011E RID: 286
public class RopeSyncer : PhotonBinaryStreamSerializer<RopeSyncData>
{
	// Token: 0x0600086C RID: 2156 RVA: 0x0002CD9A File Offset: 0x0002AF9A
	protected override void Awake()
	{
		if (!this.rope)
		{
			this.rope = base.GetComponent<Rope>();
		}
	}

	// Token: 0x0600086D RID: 2157 RVA: 0x0002CDB8 File Offset: 0x0002AFB8
	public override RopeSyncData GetDataToWrite()
	{
		RopeSyncData syncData = this.rope.GetSyncData();
		syncData.updateVisualizerManually = this.updateVisualizerManually;
		return syncData;
	}

	// Token: 0x0600086E RID: 2158 RVA: 0x0002CDDF File Offset: 0x0002AFDF
	public override void OnDataReceived(RopeSyncData data)
	{
		base.OnDataReceived(data);
		this.rope.SetSyncData(data);
	}

	// Token: 0x0600086F RID: 2159 RVA: 0x0002CDF4 File Offset: 0x0002AFF4
	public override bool ShouldSendData()
	{
		List<Transform> ropeSegments = this.rope.GetRopeSegments();
		if (this.rope.isClimbable && this.startSyncTime.IsNone)
		{
			this.startSyncTime = Optionable<float>.Some(Time.realtimeSinceStartup);
		}
		if (ropeSegments.Count == 0)
		{
			return false;
		}
		Vector3 pos = ropeSegments.First<Transform>().position;
		if ((from character in PlayerHandler.GetAllPlayerCharacters()
			select Vector3.Distance(character.Center, pos)).Min((float f) => f) > 100f)
		{
			return false;
		}
		if (this.startSyncTime.IsSome && Time.realtimeSinceStartup - this.startSyncTime.Value > 60f)
		{
			this.updateVisualizerManually = true;
			this.syncIndex++;
			if (this.syncIndex < 600)
			{
				return false;
			}
			this.syncIndex = 0;
		}
		return !this.rope.creatorLeft;
	}

	// Token: 0x040007DF RID: 2015
	public Rope rope;

	// Token: 0x040007E0 RID: 2016
	public Optionable<float> startSyncTime = Optionable<float>.None;

	// Token: 0x040007E1 RID: 2017
	private int syncIndex;

	// Token: 0x040007E2 RID: 2018
	private bool updateVisualizerManually;
}
