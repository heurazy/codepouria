using System;
using ExitGames.Client.Photon;
using Zorro.Core.Serizalization;
using Zorro.PhotonUtility;

// Token: 0x02000060 RID: 96
public class SyncPersistentPlayerDataPackage : CustomCommandPackage<CustomCommandType>
{
	// Token: 0x1700003D RID: 61
	// (get) Token: 0x060003CC RID: 972 RVA: 0x00016B18 File Offset: 0x00014D18
	// (set) Token: 0x060003CD RID: 973 RVA: 0x00016B20 File Offset: 0x00014D20
	public PersistentPlayerData Data { get; set; }

	// Token: 0x060003CE RID: 974 RVA: 0x00016B29 File Offset: 0x00014D29
	protected override void SerializeData(BinarySerializer binarySerializer)
	{
		binarySerializer.WriteInt(this.ActorNumber);
		this.Data.Serialize(binarySerializer);
	}

	// Token: 0x060003CF RID: 975 RVA: 0x00016B43 File Offset: 0x00014D43
	public override void DeserializeData(BinaryDeserializer binaryDeserializer)
	{
		this.ActorNumber = binaryDeserializer.ReadInt();
		this.Data = IBinarySerializable.DeserializeClass<PersistentPlayerData>(binaryDeserializer);
	}

	// Token: 0x060003D0 RID: 976 RVA: 0x00016B5D File Offset: 0x00014D5D
	public override CustomCommandType GetCommandType()
	{
		return CustomCommandType.SyncPersistentPlayerData;
	}

	// Token: 0x060003D1 RID: 977 RVA: 0x00016B60 File Offset: 0x00014D60
	public override SendOptions GetSendOptions()
	{
		return SendOptions.SendReliable;
	}

	// Token: 0x04000440 RID: 1088
	public int ActorNumber;
}
