using System;
using UnityEngine;
using UnityEngine.Serialization;
using Zorro.Core.Serizalization;

// Token: 0x0200005A RID: 90
[Serializable]
public class CharacterCustomizationData : IBinarySerializable
{
	// Token: 0x060003C0 RID: 960 RVA: 0x00016914 File Offset: 0x00014B14
	public void Serialize(BinarySerializer serializer)
	{
		serializer.WriteInt(this.currentSkin);
		serializer.WriteInt(this.currentAccessory);
		serializer.WriteInt(this.currentEyes);
		serializer.WriteInt(this.currentMouth);
		serializer.WriteInt(this.currentOutfit);
		serializer.WriteInt(this.currentHat);
		serializer.WriteInt(this.currentSash);
	}

	// Token: 0x060003C1 RID: 961 RVA: 0x00016978 File Offset: 0x00014B78
	public void Deserialize(BinaryDeserializer deserializer)
	{
		this.currentSkin = deserializer.ReadInt();
		this.currentAccessory = deserializer.ReadInt();
		this.currentEyes = deserializer.ReadInt();
		this.currentMouth = deserializer.ReadInt();
		this.currentOutfit = deserializer.ReadInt();
		this.currentHat = deserializer.ReadInt();
		this.currentSash = deserializer.ReadInt();
	}

	// Token: 0x04000431 RID: 1073
	[SerializeField]
	public int currentSkin;

	// Token: 0x04000432 RID: 1074
	[SerializeField]
	public int currentAccessory;

	// Token: 0x04000433 RID: 1075
	[SerializeField]
	public int currentEyes;

	// Token: 0x04000434 RID: 1076
	[SerializeField]
	public int currentMouth;

	// Token: 0x04000435 RID: 1077
	[FormerlySerializedAs("currentFit")]
	[SerializeField]
	public int currentOutfit;

	// Token: 0x04000436 RID: 1078
	[SerializeField]
	public int currentHat;

	// Token: 0x04000437 RID: 1079
	[SerializeField]
	public int currentSash;
}
