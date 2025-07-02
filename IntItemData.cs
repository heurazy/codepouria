using System;
using Zorro.Core.Serizalization;

// Token: 0x020000D3 RID: 211
public class IntItemData : DataEntryValue
{
	// Token: 0x06000683 RID: 1667 RVA: 0x00022E27 File Offset: 0x00021027
	public override void SerializeValue(BinarySerializer serializer)
	{
		serializer.WriteInt(this.Value);
	}

	// Token: 0x06000684 RID: 1668 RVA: 0x00022E35 File Offset: 0x00021035
	public override void DeserializeValue(BinaryDeserializer deserializer)
	{
		this.Value = deserializer.ReadInt();
	}

	// Token: 0x06000685 RID: 1669 RVA: 0x00022E43 File Offset: 0x00021043
	public override string ToString()
	{
		return this.Value.ToString();
	}

	// Token: 0x04000640 RID: 1600
	public int Value;
}
