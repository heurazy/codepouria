using System;
using System.Collections.Generic;
using Zorro.Core.Serizalization;

// Token: 0x02000030 RID: 48
public struct StatusSyncData : IBinarySerializable
{
	// Token: 0x06000299 RID: 665 RVA: 0x0001182C File Offset: 0x0000FA2C
	public void Serialize(BinarySerializer serializer)
	{
		serializer.WriteInt(this.statusList.Count);
		for (int i = 0; i < this.statusList.Count; i++)
		{
			serializer.WriteFloat(this.statusList[i]);
		}
	}

	// Token: 0x0600029A RID: 666 RVA: 0x00011874 File Offset: 0x0000FA74
	public void Deserialize(BinaryDeserializer deserializer)
	{
		int num = deserializer.ReadInt();
		this.statusList = new List<float>();
		for (int i = 0; i < num; i++)
		{
			this.statusList.Add(deserializer.ReadFloat());
		}
	}

	// Token: 0x0400031A RID: 794
	public List<float> statusList;
}
