using System;
using Photon.Pun;
using Unity.Mathematics;
using UnityEngine;
using Zorro.Core;

// Token: 0x020000F7 RID: 247
[DefaultExecutionOrder(-100)]
public class CharacterSyncer : PhotonBinaryStreamSerializer<CharacterSyncData>
{
	// Token: 0x0600074E RID: 1870 RVA: 0x00026D9D File Offset: 0x00024F9D
	protected override void Awake()
	{
		base.Awake();
		this.m_character = base.GetComponent<Character>();
	}

	// Token: 0x0600074F RID: 1871 RVA: 0x00026DB4 File Offset: 0x00024FB4
	public override CharacterSyncData GetDataToWrite()
	{
		return new CharacterSyncData
		{
			hipLocation = this.m_character.GetBodypart(BodypartType.Hip).Rig.position,
			lookValues = this.m_character.data.lookValues,
			movementInput = this.m_character.input.movementInput,
			sprintIsPressed = this.m_character.input.sprintIsPressed,
			sinceGrounded = this.m_character.data.sinceGrounded,
			ropePercent = this.m_character.data.ropePercent,
			ropeClimbing = this.m_character.data.isRopeClimbing,
			averageVelocity = this.GetAverageVelocity(),
			isClimbing = this.m_character.data.isClimbing,
			isGrounded = this.m_character.data.isGrounded,
			climbPos = this.m_character.data.climbPos,
			stammina = this.m_character.data.currentStamina,
			extraStammina = this.m_character.data.extraStamina,
			spectateZoom = this.m_character.data.spectateZoom
		};
	}

	// Token: 0x06000750 RID: 1872 RVA: 0x00026F1C File Offset: 0x0002511C
	public Vector3 GetAverageVelocity()
	{
		Vector3 vector = Vector3.zero;
		foreach (Bodypart bodypart in this.m_character.refs.ragdoll.partList)
		{
			vector += bodypart.Rig.linearVelocity;
		}
		vector /= (float)this.m_character.refs.ragdoll.partList.Count;
		return vector;
	}

	// Token: 0x06000751 RID: 1873 RVA: 0x00026FB4 File Offset: 0x000251B4
	public override void OnDataReceived(CharacterSyncData data)
	{
		this.sinceLastPackageUpdate = 0f;
		base.OnDataReceived(data);
		this.lastPosition = Optionable<float3>.Some(this.m_character.GetBodypart(BodypartType.Hip).Rig.position);
		this.lastLook = Optionable<float2>.Some(this.m_character.data.lookValues);
		Vector3 averageVelocity = this.GetAverageVelocity();
		Vector3 vector = data.averageVelocity - averageVelocity;
		foreach (Bodypart bodypart in this.m_character.refs.ragdoll.partList)
		{
			if (!bodypart.Rig.isKinematic)
			{
				bodypart.Rig.linearVelocity += vector;
			}
		}
		this.m_character.input.movementInput = data.movementInput;
		this.m_character.input.sprintIsPressed = data.sprintIsPressed;
		if (Mathf.Abs(this.m_character.data.sinceGrounded - data.sinceGrounded) > 2f)
		{
			this.m_character.data.sinceGrounded = data.sinceGrounded;
		}
		if (data.ropeClimbing)
		{
			this.m_character.data.ropePercent = data.ropePercent;
		}
		if (data.isClimbing)
		{
			this.m_character.data.climbPos = data.climbPos;
		}
		this.m_character.data.currentStamina = data.stammina;
		this.m_character.data.extraStamina = data.extraStammina;
		this.m_character.data.spectateZoom = data.spectateZoom;
	}

	// Token: 0x06000752 RID: 1874 RVA: 0x0002718C File Offset: 0x0002538C
	private void Update()
	{
		if (this.photonView.IsMine)
		{
			return;
		}
		if (this.RemoteValue.IsNone)
		{
			return;
		}
		if (this.lastLook.IsNone)
		{
			return;
		}
		double num = (double)(1f / (float)PhotonNetwork.SerializationRate);
		this.sinceLastPackageUpdate += Time.deltaTime;
		float num2 = (float)((double)this.sinceLastPackageUpdate / num);
		Vector2 vector = this.RemoteValue.Value.lookValues;
		Vector2 vector2 = Vector2.Lerp(this.lastLook.Value, vector, num2);
		this.m_character.data.lookValues = vector2;
	}

	// Token: 0x06000753 RID: 1875 RVA: 0x00027230 File Offset: 0x00025430
	private void FixedUpdate()
	{
		if (this.photonView.IsMine)
		{
			return;
		}
		if (this.RemoteValue.IsNone)
		{
			return;
		}
		if (this.lastPosition.IsNone)
		{
			return;
		}
		if (this.m_character.data.carrier)
		{
			return;
		}
		if (!this.m_character.warping)
		{
			this.InterpolateRigPositions();
		}
	}

	// Token: 0x06000754 RID: 1876 RVA: 0x00027294 File Offset: 0x00025494
	private void InterpolateRigPositions()
	{
		Vector3 vector = this.RemoteValue.Value.hipLocation;
		double num = (double)(1f / (float)PhotonNetwork.SerializationRate);
		this.sinceLastPackage += Time.fixedDeltaTime * 0.6f;
		float num2 = (float)((double)this.sinceLastPackage / num);
		Vector3 vector2 = Vector3.Lerp(this.lastPosition.Value, vector, num2);
		Vector3 position = this.m_character.GetBodypart(BodypartType.Hip).Rig.position;
		Vector3 vector3 = vector2 - position;
		if (vector3.magnitude > 10f)
		{
			Debug.Log("Do Big move");
			this.m_character.refs.ragdoll.MoveAllRigsInDirection(vector3);
			return;
		}
		vector3.y *= 0f;
		float num3 = vector2.y - position.y;
		float num4 = Mathf.Abs(num3);
		float num5 = Mathf.InverseLerp(0.3f, 0.6f, num4) * Mathf.Sign(num3);
		vector3 += Vector3.up * num5;
		this.m_character.refs.ragdoll.MoveAllRigsInDirection(vector3 * 0.1f);
	}

	// Token: 0x040006F0 RID: 1776
	private Character m_character;

	// Token: 0x040006F1 RID: 1777
	private Optionable<float3> lastPosition;

	// Token: 0x040006F2 RID: 1778
	private Optionable<float2> lastLook;

	// Token: 0x040006F3 RID: 1779
	private float sinceLastPackageUpdate;
}
