using System;
using Zorro.Core.Serizalization;

// Token: 0x020000D5 RID: 213
public class OptionableIntItemData : DataEntryValue
{
	// Token: 0x0600068B RID: 1675 RVA: 0x00022EBF File Offset: 0x000210BF
	public override void SerializeValue(BinarySerializer serializer)
	{
		serializer.WriteBool(this.HasData);
		if (this.HasData)
		{
			serializer.WriteInt(this.Value);
		}
	}

	// Token: 0x0600068C RID: 1676 RVA: 0x00022EE1 File Offset: 0x000210E1
	public override void DeserializeValue(BinaryDeserializer deserializer)
	{
		this.HasData = deserializer.ReadBool();
		if (this.HasData)
		{
			this.Value = deserializer.ReadInt();
		}
	}

	// Token: 0x0600068D RID: 1677 RVA: 0x00022F03 File Offset: 0x00021103
	public override string ToString()
	{
		if (!this.HasData)
		{
			return "No Data";
		}
		return this.Value.ToString();
	}

	// Token: 0x04000643 RID: 1603
	public bool HasData;

	// Token: 0x04000644 RID: 1604
	public int Value;
}
