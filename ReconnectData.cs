using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using Zorro.Core;
using Zorro.Core.Serizalization;

// Token: 0x020000FE RID: 254
internal struct ReconnectData
{
	// Token: 0x0600078D RID: 1933 RVA: 0x00028390 File Offset: 0x00026590
	public void PrintData()
	{
		Debug.Log(string.Format("Reconnect Data: Position: {0}, Dead: {1}, FullyPassedOut: {2}, DeathTimer: {3}", new object[] { this.position, this.dead, this.fullyPassedOut, this.deathTimer }));
	}

	// Token: 0x0600078E RID: 1934 RVA: 0x000283EC File Offset: 0x000265EC
	public static ReconnectData CreateFromCharacter(Character character)
	{
		return new ReconnectData
		{
			isValid = true,
			position = character.Center,
			dead = character.data.dead,
			fullyPassedOut = character.data.fullyPassedOut,
			deathTimer = character.data.deathTimer,
			currentStatuses = character.refs.afflictions.currentStatuses,
			inventorySyncData = new InventorySyncData(Player.localPlayer.itemSlots, Player.localPlayer.backpackSlot, Player.localPlayer.tempFullSlot)
		};
	}

	// Token: 0x0600078F RID: 1935 RVA: 0x00028490 File Offset: 0x00026690
	public byte[] Serialize()
	{
		BinarySerializer binarySerializer = new BinarySerializer(100, Allocator.Temp);
		binarySerializer.WriteBool(this.isValid);
		binarySerializer.WriteFloat3(this.position);
		binarySerializer.WriteBool(this.dead);
		binarySerializer.WriteBool(this.fullyPassedOut);
		binarySerializer.WriteFloat(this.deathTimer);
		new StatusSyncData
		{
			statusList = new List<float>(Character.localCharacter.refs.afflictions.currentStatuses)
		}.Serialize(binarySerializer);
		this.inventorySyncData.Serialize(binarySerializer);
		byte[] array = binarySerializer.buffer.ToByteArray();
		binarySerializer.Dispose();
		return array;
	}

	// Token: 0x06000790 RID: 1936 RVA: 0x00028538 File Offset: 0x00026738
	public static ReconnectData Deserialize(byte[] data)
	{
		ReconnectData reconnectData = default(ReconnectData);
		BinaryDeserializer binaryDeserializer = new BinaryDeserializer(data, Allocator.Temp);
		reconnectData.isValid = binaryDeserializer.ReadBool();
		reconnectData.position = binaryDeserializer.ReadFloat3();
		reconnectData.dead = binaryDeserializer.ReadBool();
		reconnectData.fullyPassedOut = binaryDeserializer.ReadBool();
		reconnectData.deathTimer = binaryDeserializer.ReadFloat();
		reconnectData.currentStatuses = IBinarySerializable.Deserialize<StatusSyncData>(binaryDeserializer).statusList.ToArray();
		reconnectData.inventorySyncData = IBinarySerializable.Deserialize<InventorySyncData>(binaryDeserializer);
		binaryDeserializer.Dispose();
		return reconnectData;
	}

	// Token: 0x04000709 RID: 1801
	public bool isValid;

	// Token: 0x0400070A RID: 1802
	public Vector3 position;

	// Token: 0x0400070B RID: 1803
	public bool dead;

	// Token: 0x0400070C RID: 1804
	public bool fullyPassedOut;

	// Token: 0x0400070D RID: 1805
	public float deathTimer;

	// Token: 0x0400070E RID: 1806
	public int maxMountainProgress;

	// Token: 0x0400070F RID: 1807
	public float[] currentStatuses;

	// Token: 0x04000710 RID: 1808
	public InventorySyncData inventorySyncData;
}
