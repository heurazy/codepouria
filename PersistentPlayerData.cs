using System;
using Zorro.Core.Serizalization;

// Token: 0x0200006A RID: 106
public class PersistentPlayerData : IBinarySerializable
{
	// Token: 0x060003F0 RID: 1008 RVA: 0x00016ECD File Offset: 0x000150CD
	public void Serialize(BinarySerializer serializer)
	{
		this.customizationData.Serialize(serializer);
	}

	// Token: 0x060003F1 RID: 1009 RVA: 0x00016EDB File Offset: 0x000150DB
	public void Deserialize(BinaryDeserializer deserializer)
	{
		this.customizationData = IBinarySerializable.DeserializeClass<CharacterCustomizationData>(deserializer);
	}

	// Token: 0x0400044C RID: 1100
	public CharacterCustomizationData customizationData = new CharacterCustomizationData();
}
