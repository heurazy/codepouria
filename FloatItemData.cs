using System;
using Zorro.Core.Serizalization;

// Token: 0x020000D2 RID: 210
public class FloatItemData : DataEntryValue
{
	// Token: 0x0600067F RID: 1663 RVA: 0x00022DF6 File Offset: 0x00020FF6
	public override void SerializeValue(BinarySerializer serializer)
	{
		serializer.WriteFloat(this.Value);
	}

	// Token: 0x06000680 RID: 1664 RVA: 0x00022E04 File Offset: 0x00021004
	public override void DeserializeValue(BinaryDeserializer deserializer)
	{
		this.Value = deserializer.ReadFloat();
	}

	// Token: 0x06000681 RID: 1665 RVA: 0x00022E12 File Offset: 0x00021012
	public override string ToString()
	{
		return this.Value.ToString();
	}

	// Token: 0x0400063F RID: 1599
	public float Value;
}
