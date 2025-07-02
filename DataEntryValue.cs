using System;
using Zorro.Core.Serizalization;

// Token: 0x020000CE RID: 206
public abstract class DataEntryValue : IBinarySerializable
{
	// Token: 0x06000667 RID: 1639 RVA: 0x0002299C File Offset: 0x00020B9C
	public void Serialize(BinarySerializer serializer)
	{
		this.SerializeValue(serializer);
	}

	// Token: 0x06000668 RID: 1640 RVA: 0x000229A5 File Offset: 0x00020BA5
	public void Deserialize(BinaryDeserializer deserializer)
	{
		this.DeserializeValue(deserializer);
	}

	// Token: 0x06000669 RID: 1641 RVA: 0x000229AE File Offset: 0x00020BAE
	public virtual void Init()
	{
	}

	// Token: 0x0600066A RID: 1642
	public abstract void SerializeValue(BinarySerializer serializer);

	// Token: 0x0600066B RID: 1643
	public abstract void DeserializeValue(BinaryDeserializer deserializer);

	// Token: 0x0600066C RID: 1644 RVA: 0x000229B0 File Offset: 0x00020BB0
	public static byte GetTypeValue(Type type)
	{
		if (type == typeof(IntItemData))
		{
			return 1;
		}
		if (type == typeof(OptionableIntItemData))
		{
			return 2;
		}
		if (type == typeof(BoolItemData))
		{
			return 3;
		}
		if (type == typeof(FloatItemData))
		{
			return 4;
		}
		if (type == typeof(OptionableBoolItemData))
		{
			return 5;
		}
		if (type == typeof(BackpackData))
		{
			return 6;
		}
		if (type == typeof(ColorItemData))
		{
			return 7;
		}
		return 0;
	}

	// Token: 0x0600066D RID: 1645 RVA: 0x00022A4C File Offset: 0x00020C4C
	public static DataEntryValue GetNewFromValue(byte value)
	{
		switch (value)
		{
		case 1:
			return new IntItemData();
		case 2:
			return new OptionableIntItemData();
		case 3:
			return new BoolItemData();
		case 4:
			return new FloatItemData();
		case 5:
			return new OptionableBoolItemData();
		case 6:
			return new BackpackData();
		case 7:
			return new ColorItemData();
		default:
			throw new NotImplementedException();
		}
	}
}
