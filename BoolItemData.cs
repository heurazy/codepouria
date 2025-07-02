using System;
using Zorro.Core.Serizalization;

// Token: 0x020000D0 RID: 208
public class BoolItemData : DataEntryValue
{
	// Token: 0x06000677 RID: 1655 RVA: 0x00022D38 File Offset: 0x00020F38
	public override void SerializeValue(BinarySerializer serializer)
	{
		serializer.WriteBool(this.Value);
	}

	// Token: 0x06000678 RID: 1656 RVA: 0x00022D46 File Offset: 0x00020F46
	public override void DeserializeValue(BinaryDeserializer deserializer)
	{
		this.Value = deserializer.ReadBool();
	}

	// Token: 0x06000679 RID: 1657 RVA: 0x00022D54 File Offset: 0x00020F54
	public override string ToString()
	{
		return this.Value.ToString();
	}

	// Token: 0x0400063D RID: 1597
	public bool Value;
}
