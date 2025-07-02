using System;
using System.Collections.Generic;
using System.Linq;
using Zorro.Core.Serizalization;

// Token: 0x020000A2 RID: 162
public class ItemInstanceData : IBinarySerializable
{
	// Token: 0x060005D7 RID: 1495 RVA: 0x00020AAD File Offset: 0x0001ECAD
	public ItemInstanceData()
	{
	}

	// Token: 0x060005D8 RID: 1496 RVA: 0x00020AC0 File Offset: 0x0001ECC0
	public ItemInstanceData(Guid guid)
	{
		this.guid = guid;
	}

	// Token: 0x060005D9 RID: 1497 RVA: 0x00020ADC File Offset: 0x0001ECDC
	public void Serialize(BinarySerializer serializer)
	{
		List<KeyValuePair<DataEntryKey, DataEntryValue>> list = this.data.ToList<KeyValuePair<DataEntryKey, DataEntryValue>>();
		byte b = (byte)list.Count;
		serializer.WriteByte(b);
		foreach (KeyValuePair<DataEntryKey, DataEntryValue> keyValuePair in list)
		{
			DataEntryKey key = keyValuePair.Key;
			DataEntryValue value = keyValuePair.Value;
			serializer.WriteByte((byte)key);
			serializer.WriteByte(DataEntryValue.GetTypeValue(value.GetType()));
			value.Serialize(serializer);
		}
	}

	// Token: 0x060005DA RID: 1498 RVA: 0x00020B70 File Offset: 0x0001ED70
	public void Deserialize(BinaryDeserializer deserializer)
	{
		byte b = deserializer.ReadByte();
		this.data = new Dictionary<DataEntryKey, DataEntryValue>((int)b);
		for (int i = 0; i < (int)b; i++)
		{
			DataEntryKey dataEntryKey = (DataEntryKey)deserializer.ReadByte();
			DataEntryValue newFromValue = DataEntryValue.GetNewFromValue(deserializer.ReadByte());
			newFromValue.Init();
			newFromValue.Deserialize(deserializer);
			this.data.Add(dataEntryKey, newFromValue);
		}
	}

	// Token: 0x060005DB RID: 1499 RVA: 0x00020BC9 File Offset: 0x0001EDC9
	public bool HasData(DataEntryKey key)
	{
		return this.data.ContainsKey(key);
	}

	// Token: 0x060005DC RID: 1500 RVA: 0x00020BD8 File Offset: 0x0001EDD8
	public bool TryGetDataEntry<T>(DataEntryKey key, out T value) where T : DataEntryValue
	{
		DataEntryValue dataEntryValue;
		bool flag = this.data.TryGetValue(key, out dataEntryValue);
		if (flag)
		{
			value = (T)((object)dataEntryValue);
			return flag;
		}
		value = default(T);
		return flag;
	}

	// Token: 0x060005DD RID: 1501 RVA: 0x00020C0C File Offset: 0x0001EE0C
	public T RegisterNewEntry<T>(DataEntryKey key) where T : DataEntryValue, new()
	{
		T t = new T();
		t.Init();
		this.data.Add(key, t);
		return t;
	}

	// Token: 0x060005DE RID: 1502 RVA: 0x00020C3D File Offset: 0x0001EE3D
	public T RegisterEntry<T>(DataEntryKey key, T value) where T : DataEntryValue, new()
	{
		value.Init();
		this.data.Add(key, value);
		return value;
	}

	// Token: 0x040005D9 RID: 1497
	public Guid guid;

	// Token: 0x040005DA RID: 1498
	public Dictionary<DataEntryKey, DataEntryValue> data = new Dictionary<DataEntryKey, DataEntryValue>();
}
