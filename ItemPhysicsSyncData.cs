using System;
using Unity.Mathematics;
using UnityEngine;
using Zorro.Core.Serizalization;

// Token: 0x02000106 RID: 262
public struct ItemPhysicsSyncData : IBinarySerializable
{
	// Token: 0x060007C4 RID: 1988 RVA: 0x000290F2 File Offset: 0x000272F2
	public void Serialize(BinarySerializer serializer)
	{
		serializer.WriteFloat3(this.position);
		serializer.WriteQuaternion(this.rotation);
		serializer.WriteHalf3((half3)this.linearVelocity);
		serializer.WriteHalf3((half3)this.angularVelocity);
	}

	// Token: 0x060007C5 RID: 1989 RVA: 0x0002912E File Offset: 0x0002732E
	public void Deserialize(BinaryDeserializer deserializer)
	{
		this.position = deserializer.ReadFloat3();
		this.rotation = deserializer.ReadQuaternion();
		this.linearVelocity = deserializer.ReadHalf3();
		this.angularVelocity = deserializer.ReadHalf3();
	}

	// Token: 0x04000739 RID: 1849
	public float3 position;

	// Token: 0x0400073A RID: 1850
	public Quaternion rotation;

	// Token: 0x0400073B RID: 1851
	public float3 linearVelocity;

	// Token: 0x0400073C RID: 1852
	public float3 angularVelocity;
}
