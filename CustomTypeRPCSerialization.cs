using System;
using ExitGames.Client.Photon;
using Photon.Pun;
using Unity.Collections;
using Zorro.Core;
using Zorro.Core.Serizalization;

// Token: 0x020000F8 RID: 248
public static class CustomTypeRPCSerialization
{
	// Token: 0x06000756 RID: 1878 RVA: 0x000273D4 File Offset: 0x000255D4
	public static void Initialize()
	{
		PhotonPeer.RegisterType(typeof(PhotonView), byte.MaxValue, new SerializeMethod(CustomTypeRPCSerialization.SerializePhotonView), new DeserializeMethod(CustomTypeRPCSerialization.DeserializePhotonView));
		PhotonPeer.RegisterType(typeof(ItemInstanceData), 254, new SerializeMethod(CustomTypeRPCSerialization.SerializeItemData), new DeserializeMethod(CustomTypeRPCSerialization.DeserializeItemData));
		PhotonPeer.RegisterType(typeof(BackpackReference), 253, new SerializeMethod(CustomTypeRPCSerialization.SerializeBackpackRef), new DeserializeMethod(CustomTypeRPCSerialization.DeserializeBackpackRef));
	}

	// Token: 0x06000757 RID: 1879 RVA: 0x00027468 File Offset: 0x00025668
	private static object DeserializeBackpackRef(byte[] serializedcustomobject)
	{
		return IBinarySerializable.GetFromManagedArray<BackpackReference>(serializedcustomobject);
	}

	// Token: 0x06000758 RID: 1880 RVA: 0x00027475 File Offset: 0x00025675
	private static byte[] SerializeBackpackRef(object customobject)
	{
		return IBinarySerializable.ToManagedArray<BackpackReference>((BackpackReference)customobject);
	}

	// Token: 0x06000759 RID: 1881 RVA: 0x00027484 File Offset: 0x00025684
	private static object DeserializeItemData(byte[] serializedcustomobject)
	{
		NativeArray<byte> nativeArray = serializedcustomobject.ToNativeArray(Allocator.Temp);
		BinaryDeserializer binaryDeserializer = new BinaryDeserializer(nativeArray);
		Guid guid = binaryDeserializer.ReadGuid();
		ItemInstanceData itemInstanceData;
		if (!ItemInstanceDataHandler.TryGetInstanceData(guid, out itemInstanceData))
		{
			itemInstanceData = new ItemInstanceData(guid);
			ItemInstanceDataHandler.AddInstanceData(itemInstanceData);
		}
		itemInstanceData.Deserialize(binaryDeserializer);
		nativeArray.Dispose();
		return itemInstanceData;
	}

	// Token: 0x0600075A RID: 1882 RVA: 0x000274D0 File Offset: 0x000256D0
	private static byte[] SerializeItemData(object d)
	{
		ItemInstanceData itemInstanceData = (ItemInstanceData)d;
		BinarySerializer binarySerializer = new BinarySerializer(24, Allocator.Temp);
		binarySerializer.WriteGuid(itemInstanceData.guid);
		itemInstanceData.Serialize(binarySerializer);
		byte[] array = binarySerializer.buffer.ToByteArray();
		binarySerializer.Dispose();
		return array;
	}

	// Token: 0x0600075B RID: 1883 RVA: 0x00027511 File Offset: 0x00025711
	public static object DeserializePhotonView(byte[] data)
	{
		return PhotonView.Find(BitConverter.ToInt32(data));
	}

	// Token: 0x0600075C RID: 1884 RVA: 0x00027523 File Offset: 0x00025723
	public static byte[] SerializePhotonView(object customType)
	{
		return BitConverter.GetBytes(((PhotonView)customType).ViewID);
	}
}
