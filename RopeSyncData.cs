using System;
using Unity.Mathematics;
using UnityEngine;
using Zorro.Core.Serizalization;

// Token: 0x0200011D RID: 285
public struct RopeSyncData : IBinarySerializable
{
	// Token: 0x0600086A RID: 2154 RVA: 0x0002CCE0 File Offset: 0x0002AEE0
	public void Serialize(BinarySerializer serializer)
	{
		serializer.WriteBool(this.isVisible);
		serializer.WriteBool(this.updateVisualizerManually);
		if (this.segments == null)
		{
			serializer.WriteUshort(0);
			return;
		}
		ushort num = (ushort)this.segments.Length;
		serializer.WriteUshort(num);
		for (int i = 0; i < (int)num; i++)
		{
			this.segments[i].Serialize(serializer);
		}
	}

	// Token: 0x0600086B RID: 2155 RVA: 0x0002CD44 File Offset: 0x0002AF44
	public void Deserialize(BinaryDeserializer deserializer)
	{
		this.isVisible = deserializer.ReadBool();
		this.updateVisualizerManually = deserializer.ReadBool();
		ushort num = deserializer.ReadUShort();
		this.segments = new RopeSyncData.SegmentData[(int)num];
		for (int i = 0; i < (int)num; i++)
		{
			this.segments[i] = IBinarySerializable.Deserialize<RopeSyncData.SegmentData>(deserializer);
		}
	}

	// Token: 0x040007DC RID: 2012
	public RopeSyncData.SegmentData[] segments;

	// Token: 0x040007DD RID: 2013
	public bool isVisible;

	// Token: 0x040007DE RID: 2014
	public bool updateVisualizerManually;

	// Token: 0x02000351 RID: 849
	public struct SegmentData : IBinarySerializable
	{
		// Token: 0x0600137D RID: 4989 RVA: 0x0005CDF8 File Offset: 0x0005AFF8
		public void Serialize(BinarySerializer serializer)
		{
			serializer.WriteFloat3(this.position);
			serializer.WriteQuaternion(this.rotation);
		}

		// Token: 0x0600137E RID: 4990 RVA: 0x0005CE12 File Offset: 0x0005B012
		public void Deserialize(BinaryDeserializer deserializer)
		{
			this.position = deserializer.ReadFloat3();
			this.rotation = deserializer.ReadQuaternion();
		}

		// Token: 0x04001233 RID: 4659
		public float3 position;

		// Token: 0x04001234 RID: 4660
		public Quaternion rotation;
	}
}
