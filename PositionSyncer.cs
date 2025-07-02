using System;
using System.Runtime.CompilerServices;
using Photon.Pun;
using Photon.Realtime;
using Unity.Mathematics;
using UnityEngine;
using Zorro.Core;
using Zorro.Core.Serizalization;

// Token: 0x02000107 RID: 263
public class PositionSyncer : PhotonBinaryStreamSerializer<PositionSyncer.Pos>
{
	// Token: 0x060007C6 RID: 1990 RVA: 0x0002916C File Offset: 0x0002736C
	public override PositionSyncer.Pos GetDataToWrite()
	{
		this.lastSent = Optionable<float3>.Some(base.transform.position);
		return new PositionSyncer.Pos
		{
			Position = base.transform.position
		};
	}

	// Token: 0x060007C7 RID: 1991 RVA: 0x000291B4 File Offset: 0x000273B4
	public override void OnDataReceived(PositionSyncer.Pos data)
	{
		base.OnDataReceived(data);
		this.currentPos = base.transform.position;
	}

	// Token: 0x060007C8 RID: 1992 RVA: 0x000291D0 File Offset: 0x000273D0
	public override bool ShouldSendData()
	{
		PositionSyncer.<>c__DisplayClass6_0 CS$<>8__locals1;
		CS$<>8__locals1.last = this.lastSent.Value;
		CS$<>8__locals1.n = base.transform.position;
		if (!PositionSyncer.<ShouldSendData>g__IsSame|6_0(ref CS$<>8__locals1))
		{
			return true;
		}
		if (this.forceSyncFrames > 0)
		{
			this.forceSyncFrames--;
			return true;
		}
		return false;
	}

	// Token: 0x060007C9 RID: 1993 RVA: 0x00029228 File Offset: 0x00027428
	private void Update()
	{
		if (this.photonView.IsMine)
		{
			return;
		}
		double num = (double)(1f / (float)PhotonNetwork.SerializationRate);
		this.sinceLastPackage += Time.deltaTime;
		float num2 = (float)((double)this.sinceLastPackage / num);
		if (this.RemoteValue.IsSome)
		{
			PositionSyncer.Pos value = this.RemoteValue.Value;
			base.transform.position = Vector3.Lerp(this.currentPos, value.Position, num2);
		}
	}

	// Token: 0x060007CA RID: 1994 RVA: 0x000292A9 File Offset: 0x000274A9
	public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
	{
		base.OnPlayerEnteredRoom(newPlayer);
		if (this.photonView.IsMine)
		{
			this.forceSyncFrames = 10;
		}
	}

	// Token: 0x060007CC RID: 1996 RVA: 0x000292D0 File Offset: 0x000274D0
	[CompilerGenerated]
	internal static bool <ShouldSendData>g__IsSame|6_0(ref PositionSyncer.<>c__DisplayClass6_0 A_0)
	{
		return Mathf.Approximately(A_0.last.x, A_0.n.x) && Mathf.Approximately(A_0.last.y, A_0.n.y) && Mathf.Approximately(A_0.last.z, A_0.n.z);
	}

	// Token: 0x0400073D RID: 1853
	private Vector3 currentPos;

	// Token: 0x0400073E RID: 1854
	private int forceSyncFrames;

	// Token: 0x0400073F RID: 1855
	private Optionable<float3> lastSent;

	// Token: 0x02000348 RID: 840
	public struct Pos : IBinarySerializable
	{
		// Token: 0x0600136A RID: 4970 RVA: 0x0005CC63 File Offset: 0x0005AE63
		public void Serialize(BinarySerializer serializer)
		{
			serializer.WriteHalf3((half3)this.Position);
		}

		// Token: 0x0600136B RID: 4971 RVA: 0x0005CC76 File Offset: 0x0005AE76
		public void Deserialize(BinaryDeserializer deserializer)
		{
			this.Position = deserializer.ReadHalf3();
		}

		// Token: 0x0400121A RID: 4634
		public float3 Position;
	}
}
