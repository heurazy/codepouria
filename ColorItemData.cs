using System;
using Unity.Mathematics;
using UnityEngine;
using Zorro.Core.Serizalization;

// Token: 0x020000D1 RID: 209
public class ColorItemData : DataEntryValue
{
	// Token: 0x0600067B RID: 1659 RVA: 0x00022D69 File Offset: 0x00020F69
	public override void SerializeValue(BinarySerializer serializer)
	{
		serializer.WriteFloat4(new float4(this.Value.r, this.Value.g, this.Value.b, this.Value.a));
	}

	// Token: 0x0600067C RID: 1660 RVA: 0x00022DA4 File Offset: 0x00020FA4
	public override void DeserializeValue(BinaryDeserializer deserializer)
	{
		float4 @float = deserializer.ReadFloat4();
		this.Value = new Color(@float.x, @float.y, @float.z, @float.w);
	}

	// Token: 0x0600067D RID: 1661 RVA: 0x00022DDB File Offset: 0x00020FDB
	public override string ToString()
	{
		return this.Value.ToString();
	}

	// Token: 0x0400063E RID: 1598
	public Color Value;
}
