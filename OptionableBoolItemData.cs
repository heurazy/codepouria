using System;
using Zorro.Core.Serizalization;

// Token: 0x020000D4 RID: 212
public class OptionableBoolItemData : DataEntryValue
{
	// Token: 0x06000687 RID: 1671 RVA: 0x00022E58 File Offset: 0x00021058
	public override void SerializeValue(BinarySerializer serializer)
	{
		serializer.WriteBool(this.HasData);
		if (this.HasData)
		{
			serializer.WriteBool(this.Value);
		}
	}

	// Token: 0x06000688 RID: 1672 RVA: 0x00022E7A File Offset: 0x0002107A
	public override void DeserializeValue(BinaryDeserializer deserializer)
	{
		this.HasData = deserializer.ReadBool();
		if (this.HasData)
		{
			this.Value = deserializer.ReadBool();
		}
	}

	// Token: 0x06000689 RID: 1673 RVA: 0x00022E9C File Offset: 0x0002109C
	public override string ToString()
	{
		if (!this.HasData)
		{
			return "No Data";
		}
		return this.Value.ToString();
	}

	// Token: 0x04000641 RID: 1601
	public bool HasData;

	// Token: 0x04000642 RID: 1602
	public bool Value;
}
