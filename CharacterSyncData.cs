using System;
using Unity.Mathematics;
using UnityEngine;
using Zorro.Core;
using Zorro.Core.Serizalization;

// Token: 0x020000F6 RID: 246
public struct CharacterSyncData : IBinarySerializable
{
	// Token: 0x0600074C RID: 1868 RVA: 0x00026AC0 File Offset: 0x00024CC0
	public void Serialize(BinarySerializer serializer)
	{
		serializer.WriteFloat3(this.hipLocation);
		serializer.WriteHalf2(new half2((half)this.lookValues.x, (half)this.lookValues.y));
		CharacterSyncData.Flags flags = CharacterSyncData.Flags.NONE;
		if (this.sprintIsPressed)
		{
			flags |= CharacterSyncData.Flags.SPRINT;
		}
		if (this.movementInput.x > 0.01f)
		{
			flags |= CharacterSyncData.Flags.WALK_RIGHT;
		}
		if (this.movementInput.x < -0.01f)
		{
			flags |= CharacterSyncData.Flags.WALK_LEFT;
		}
		if (this.movementInput.y > 0.01f)
		{
			flags |= CharacterSyncData.Flags.WALK_FORWARD;
		}
		if (this.movementInput.y < -0.01f)
		{
			flags |= CharacterSyncData.Flags.WALK_BACKWARD;
		}
		if (this.ropeClimbing)
		{
			flags |= CharacterSyncData.Flags.ROPE_CLIMBING;
		}
		if (this.isClimbing)
		{
			flags |= CharacterSyncData.Flags.CLIMBING;
		}
		if (this.isGrounded)
		{
			flags |= CharacterSyncData.Flags.IS_GROUNDED;
		}
		serializer.WriteByte((byte)flags);
		serializer.WriteHalf((half)this.sinceGrounded);
		if (this.ropeClimbing)
		{
			serializer.WriteHalf((half)this.ropePercent);
		}
		serializer.WriteHalf3((half3)this.averageVelocity);
		if (this.isClimbing)
		{
			serializer.WriteHalf3((half3)this.climbPos);
		}
		serializer.WriteHalf((half)this.stammina);
		serializer.WriteHalf((half)this.extraStammina);
		serializer.WriteHalf((half)this.spectateZoom);
	}

	// Token: 0x0600074D RID: 1869 RVA: 0x00026C24 File Offset: 0x00024E24
	public void Deserialize(BinaryDeserializer deserializer)
	{
		this.hipLocation = deserializer.ReadFloat3();
		this.lookValues = new Vector2(deserializer.ReadHalf(), deserializer.ReadHalf());
		CharacterSyncData.Flags flags = (CharacterSyncData.Flags)deserializer.ReadByte();
		Vector2 zero = Vector2.zero;
		this.sprintIsPressed = flags.HasFlagUnsafe(CharacterSyncData.Flags.SPRINT);
		if (flags.HasFlagUnsafe(CharacterSyncData.Flags.WALK_RIGHT))
		{
			zero.x += 1f;
		}
		if (flags.HasFlagUnsafe(CharacterSyncData.Flags.WALK_LEFT))
		{
			zero.x -= 1f;
		}
		if (flags.HasFlagUnsafe(CharacterSyncData.Flags.WALK_FORWARD))
		{
			zero.y += 1f;
		}
		if (flags.HasFlagUnsafe(CharacterSyncData.Flags.WALK_BACKWARD))
		{
			zero.y -= 1f;
		}
		this.movementInput = zero;
		this.sinceGrounded = deserializer.ReadHalf();
		this.ropeClimbing = flags.HasFlagUnsafe(CharacterSyncData.Flags.ROPE_CLIMBING);
		if (this.ropeClimbing)
		{
			this.ropePercent = deserializer.ReadHalf();
		}
		this.averageVelocity = deserializer.ReadHalf3();
		this.isClimbing = flags.HasFlagUnsafe(CharacterSyncData.Flags.CLIMBING);
		this.isGrounded = flags.HasFlagUnsafe(CharacterSyncData.Flags.IS_GROUNDED);
		if (this.isClimbing)
		{
			this.climbPos = deserializer.ReadHalf3();
		}
		this.stammina = deserializer.ReadHalf();
		this.extraStammina = deserializer.ReadHalf();
		this.spectateZoom = deserializer.ReadHalf();
	}

	// Token: 0x040006E2 RID: 1762
	public float3 hipLocation;

	// Token: 0x040006E3 RID: 1763
	public float2 lookValues;

	// Token: 0x040006E4 RID: 1764
	public Vector2 movementInput;

	// Token: 0x040006E5 RID: 1765
	public bool sprintIsPressed;

	// Token: 0x040006E6 RID: 1766
	public float sinceGrounded;

	// Token: 0x040006E7 RID: 1767
	public bool ropeClimbing;

	// Token: 0x040006E8 RID: 1768
	public float ropePercent;

	// Token: 0x040006E9 RID: 1769
	public float3 averageVelocity;

	// Token: 0x040006EA RID: 1770
	public bool isClimbing;

	// Token: 0x040006EB RID: 1771
	public bool isGrounded;

	// Token: 0x040006EC RID: 1772
	public float3 climbPos;

	// Token: 0x040006ED RID: 1773
	public float stammina;

	// Token: 0x040006EE RID: 1774
	public float extraStammina;

	// Token: 0x040006EF RID: 1775
	public float spectateZoom;

	// Token: 0x0200033E RID: 830
	[Flags]
	public enum Flags : byte
	{
		// Token: 0x040011F4 RID: 4596
		NONE = 0,
		// Token: 0x040011F5 RID: 4597
		SPRINT = 1,
		// Token: 0x040011F6 RID: 4598
		ROPE_CLIMBING = 2,
		// Token: 0x040011F7 RID: 4599
		WALK_RIGHT = 4,
		// Token: 0x040011F8 RID: 4600
		WALK_LEFT = 8,
		// Token: 0x040011F9 RID: 4601
		WALK_FORWARD = 16,
		// Token: 0x040011FA RID: 4602
		WALK_BACKWARD = 32,
		// Token: 0x040011FB RID: 4603
		CLIMBING = 64,
		// Token: 0x040011FC RID: 4604
		IS_GROUNDED = 128
	}
}
