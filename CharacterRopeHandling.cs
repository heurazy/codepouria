using System;
using Photon.Pun;
using UnityEngine;
using Zorro.Core;

// Token: 0x020001A6 RID: 422
public class CharacterRopeHandling : MonoBehaviour
{
	// Token: 0x06000BB4 RID: 2996 RVA: 0x0003A817 File Offset: 0x00038A17
	private void Awake()
	{
		this.view = base.GetComponent<PhotonView>();
	}

	// Token: 0x06000BB5 RID: 2997 RVA: 0x0003A825 File Offset: 0x00038A25
	private void Start()
	{
		this.character = base.GetComponent<Character>();
	}

	// Token: 0x06000BB6 RID: 2998 RVA: 0x0003A834 File Offset: 0x00038A34
	private void Update()
	{
		if (!this.view.IsMine)
		{
			return;
		}
		if (this.character.data.isRopeClimbing)
		{
			if (!this.character.data.heldRope.UnityObjectExists<Rope>())
			{
				this.view.RPC("StopRopeClimbingRpc", RpcTarget.All, Array.Empty<object>());
				return;
			}
			if (this.character.data.heldRope != null)
			{
				float angleAtPercent = this.character.data.heldRope.climbingAPI.GetAngleAtPercent(this.character.data.ropePercent);
				if (!this.character.data.heldRope.IsActive() || (angleAtPercent > this.maxRopeAngle && 180f - angleAtPercent > this.maxRopeAngle))
				{
					Debug.Log(string.Format("Rope climbing failed. Angle up: {0} Angle down: {1}", angleAtPercent, 180f - angleAtPercent));
					this.view.RPC("StopRopeClimbingRpc", RpcTarget.All, Array.Empty<object>());
					return;
				}
			}
			float num = ((this.character.input.movementInput.y < 0f) ? 3f : 1f);
			this.character.data.ropePercent += this.character.data.heldRope.climbingAPI.GetMove() * this.character.input.movementInput.y * num * this.climbSpeed * this.climbSpeedMod * Time.deltaTime * this.character.data.heldRope.climbingAPI.UpMult(this.character.data.ropePercent);
			this.character.data.ropePercent = Mathf.Clamp01(this.character.data.ropePercent);
			float num2 = this.staminaUsage;
			if (this.character.input.movementInput.y > 0.01f)
			{
				num2 = this.staminaUsageUp;
			}
			if (this.character.IsLocal && (this.character.input.jumpWasPressed || !this.character.UseStamina(num2 * Time.deltaTime, true) || this.character.data.currentRagdollControll < 0.3f))
			{
				this.view.RPC("StopRopeClimbingRpc", RpcTarget.All, Array.Empty<object>());
			}
		}
	}

	// Token: 0x06000BB7 RID: 2999 RVA: 0x0003AAA0 File Offset: 0x00038CA0
	[PunRPC]
	private void StopRopeClimbingRpc()
	{
		if (this.character.data.heldRope != null)
		{
			this.character.data.heldRope.RemoveCharacterClimbing(this.character);
		}
		this.character.data.isRopeClimbing = false;
		this.character.data.isJumping = false;
		this.character.data.sinceGrounded = 0f;
		this.character.data.heldRope = null;
		Debug.Log("Stop Climbing");
	}

	// Token: 0x06000BB8 RID: 3000 RVA: 0x0003AB32 File Offset: 0x00038D32
	private void FixedUpdate()
	{
		if (this.character.data.isRopeClimbing)
		{
			this.Climbing();
			return;
		}
		this.TryToStartWallClimb();
	}

	// Token: 0x06000BB9 RID: 3001 RVA: 0x0003AB54 File Offset: 0x00038D54
	private void Climbing()
	{
		this.character.data.ropeClimbWorldNormal = this.character.data.ropeClimbNormal;
		this.character.data.ropeClimbWorldUp = this.character.data.heldRope.climbingAPI.GetUp(this.character.data.ropePercent);
		this.character.AddForce(this.ClimbForce(), 1f, 1f);
	}

	// Token: 0x06000BBA RID: 3002 RVA: 0x0003ABD6 File Offset: 0x00038DD6
	private Vector3 ClimbForce()
	{
		return (this.GetPosition() - this.character.TorsoPos()) * this.climbForce;
	}

	// Token: 0x06000BBB RID: 3003 RVA: 0x0003ABFC File Offset: 0x00038DFC
	private Vector3 GetPosition()
	{
		return this.character.data.heldRope.climbingAPI.GetPosition(this.character.data.ropePercent) + this.character.data.ropeClimbWorldNormal * 0.5f;
	}

	// Token: 0x06000BBC RID: 3004 RVA: 0x0003AC52 File Offset: 0x00038E52
	private void TryToStartWallClimb()
	{
	}

	// Token: 0x06000BBD RID: 3005 RVA: 0x0003AC54 File Offset: 0x00038E54
	[PunRPC]
	public void GrabRopeRpc(PhotonView ropeView, int segmentIndex)
	{
		Rope componentInChildren = ropeView.GetComponentInChildren<Rope>();
		if (componentInChildren == null)
		{
			Debug.LogError("Failed to get rope from network object");
			return;
		}
		Debug.Log("Start Rope Climbing!");
		componentInChildren.AddCharacterClimbing(this.character);
		this.character.data.isRopeClimbing = true;
		this.character.data.heldRope = componentInChildren;
		this.character.data.ropePercent = componentInChildren.climbingAPI.GetPercentFromSegmentIndex(segmentIndex);
		this.character.data.ropeClimbNormal = -this.character.data.lookDirection_Flat;
		this.character.data.isClimbing = false;
		this.character.data.isVineClimbing = false;
	}

	// Token: 0x04000A91 RID: 2705
	private Character character;

	// Token: 0x04000A92 RID: 2706
	public float climbForce;

	// Token: 0x04000A93 RID: 2707
	public float climbSpeed;

	// Token: 0x04000A94 RID: 2708
	public float climbSpeedMod = 1f;

	// Token: 0x04000A95 RID: 2709
	public float climbDrag = 0.85f;

	// Token: 0x04000A96 RID: 2710
	public float staminaUsage;

	// Token: 0x04000A97 RID: 2711
	public float staminaUsageUp;

	// Token: 0x04000A98 RID: 2712
	private PhotonView view;

	// Token: 0x04000A99 RID: 2713
	public float maxRopeAngle = 90f;
}
