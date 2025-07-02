using System;
using ExitGames.Client.Photon;
using Zorro.Core.Serizalization;
using Zorro.PhotonUtility;

// Token: 0x0200005F RID: 95
public class SyncMapHandlerDebugCommandPackage : CustomCommandPackage<CustomCommandType>
{
	// Token: 0x060003C6 RID: 966 RVA: 0x00016A5F File Offset: 0x00014C5F
	public SyncMapHandlerDebugCommandPackage()
	{
	}

	// Token: 0x060003C7 RID: 967 RVA: 0x00016A67 File Offset: 0x00014C67
	public SyncMapHandlerDebugCommandPackage(Segment segment, int[] playersToTeleport)
	{
		this.Segment = segment;
		this.PlayerToTeleport = playersToTeleport;
	}

	// Token: 0x060003C8 RID: 968 RVA: 0x00016A80 File Offset: 0x00014C80
	protected override void SerializeData(BinarySerializer binarySerializer)
	{
		binarySerializer.WriteByte((byte)this.Segment);
		binarySerializer.WriteByte((byte)this.PlayerToTeleport.Length);
		foreach (int num in this.PlayerToTeleport)
		{
			binarySerializer.WriteInt(num);
		}
	}

	// Token: 0x060003C9 RID: 969 RVA: 0x00016AC8 File Offset: 0x00014CC8
	public override void DeserializeData(BinaryDeserializer binaryDeserializer)
	{
		this.Segment = (Segment)binaryDeserializer.ReadByte();
		byte b = binaryDeserializer.ReadByte();
		this.PlayerToTeleport = new int[(int)b];
		for (int i = 0; i < (int)b; i++)
		{
			this.PlayerToTeleport[i] = binaryDeserializer.ReadInt();
		}
	}

	// Token: 0x060003CA RID: 970 RVA: 0x00016B0E File Offset: 0x00014D0E
	public override CustomCommandType GetCommandType()
	{
		return CustomCommandType.SyncMapHandlerDebugCommand;
	}

	// Token: 0x060003CB RID: 971 RVA: 0x00016B11 File Offset: 0x00014D11
	public override SendOptions GetSendOptions()
	{
		return SendOptions.SendReliable;
	}

	// Token: 0x0400043E RID: 1086
	public int[] PlayerToTeleport;

	// Token: 0x0400043F RID: 1087
	public Segment Segment;
}
