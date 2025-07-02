using System;
using Photon.Pun;
using UnityEngine;
using Zorro.ControllerSupport;
using Zorro.Core;

// Token: 0x020001A3 RID: 419
public class CharacterClimbing : MonoBehaviour
{
	// Token: 0x06000B7A RID: 2938 RVA: 0x00038A78 File Offset: 0x00036C78
	private void Start()
	{
		this.view = base.GetComponent<PhotonView>();
		this.character = base.GetComponent<Character>();
		Character character = this.character;
		character.dragTowardsAction = (Action<Vector3, float>)Delegate.Combine(character.dragTowardsAction, new Action<Vector3, float>(this.GetDragged));
	}

	// Token: 0x06000B7B RID: 2939 RVA: 0x00038AC4 File Offset: 0x00036CC4
	private void FixedUpdate()
	{
		if (this.character.data.currentClimbHandle)
		{
			this.HandleClimbHandle();
		}
		if (this.character.data.isClimbing)
		{
			this.Climbing();
		}
	}

	// Token: 0x06000B7C RID: 2940 RVA: 0x00038AFC File Offset: 0x00036CFC
	private void Update()
	{
		if (!this.view.IsMine)
		{
			return;
		}
		this.ClimbHandleUpdate();
		if (!this.character.data.isClimbing)
		{
			this.sprintHasBeenPressedSinceClimb = false;
			this.climbToggledOn = false;
			if (this.character.data.currentClimbHandle == null)
			{
				this.TryToStartWallClimb(false, default(Vector3), false);
			}
			return;
		}
		if (this.character.input.sprintWasPressed || this.character.input.sprintToggleWasPressed)
		{
			this.sprintHasBeenPressedSinceClimb = true;
		}
		if (this.sprintHasBeenPressedSinceClimb && (this.character.input.sprintIsPressed || this.character.input.sprintToggleIsPressed) && this.character.data.sinceClimbJump > 1f && this.character.data.outOfStaminaFor < 0.5f && this.character.input.movementInput.magnitude > 0.1f && this.character.input.movementInput.normalized.y > -0.9f)
		{
			this.character.refs.view.RPC("RPCA_ClimbJump", RpcTarget.All, Array.Empty<object>());
		}
		this.sinceShake += Time.deltaTime;
		if (this.character.OutOfStamina() && this.sinceShake > 0.1f && this.character.refs.view.IsMine)
		{
			GamefeelHandler.instance.AddPerlinShake(3f * Mathf.Clamp01(this.character.data.outOfStaminaFor * 1f), 0.2f, 10f);
			this.sinceShake = 0f;
		}
		float num = this.maxStaminaUsage * Mathf.Clamp(this.character.input.movementInput.magnitude, 0f, 1f);
		num = Mathf.Clamp(num, this.minStaminaUsage, this.maxStaminaUsage);
		num *= this.GetAngleUsage();
		this.character.UseStamina(num * Time.deltaTime * this.character.data.staminaMod, true);
		this.TestAchievement();
		if (this.character.input.jumpWasPressed || (this.character.input.usePrimaryWasReleased && !this.climbToggledOn) || this.character.data.currentRagdollControll < 0.25f)
		{
			this.view.RPC("StopClimbingRpc", RpcTarget.All, new object[] { this.GetFallSpeed() });
		}
	}

	// Token: 0x06000B7D RID: 2941 RVA: 0x00038DB0 File Offset: 0x00036FB0
	private float GetAngleUsage()
	{
		float num = Vector3.Angle(Vector3.up, this.character.data.climbNormal);
		float num2 = Mathf.InverseLerp(40f, 60f, num);
		return Mathf.Lerp(0.2f, 1f, num2);
	}

	// Token: 0x06000B7E RID: 2942 RVA: 0x00038DFC File Offset: 0x00036FFC
	private void ClimbHandleUpdate()
	{
		if (this.character.data.currentClimbHandle && this.view.IsMine)
		{
			if (this.character.input.jumpWasPressed)
			{
				this.CancelHandle(true);
				return;
			}
			if (this.character.data.isRopeClimbing)
			{
				this.CancelHandle(false);
				return;
			}
			if (this.character.data.isVineClimbing)
			{
				this.CancelHandle(false);
				return;
			}
		}
		else
		{
			this.handleOffset = Vector2.zero;
		}
	}

	// Token: 0x06000B7F RID: 2943 RVA: 0x00038E88 File Offset: 0x00037088
	public void CancelHandle(bool grabWall = true)
	{
		if (grabWall)
		{
			this.TryToStartWallClimb(true, this.character.data.currentClimbHandle.transform.forward, false);
		}
		this.character.data.currentClimbHandle.view.RPC("RPCA_UnHang", RpcTarget.All, new object[] { this.view });
		this.handleOffset = Vector2.zero;
	}

	// Token: 0x06000B80 RID: 2944 RVA: 0x00038EF4 File Offset: 0x000370F4
	private void HandleClimbHandle()
	{
		this.handleOffset = Vector2.Lerp(this.handleOffset, this.character.input.movementInput, Time.fixedDeltaTime);
		if (this.handleOffset.magnitude > 0.3f && this.view.IsMine)
		{
			this.CancelHandle(true);
			return;
		}
		this.character.data.sinceGrounded = 0f;
		Vector3 vector = (this.character.GetBodypartRig(BodypartType.Hand_R).position + this.character.GetBodypartRig(BodypartType.Hand_L).position) * 0.5f;
		Vector3 vector2 = this.character.data.currentClimbHandle.transform.TransformPoint(new Vector3(0f, -0.7f, -0.3f));
		this.character.MoveBodypartTowardsPoint(BodypartType.Hand_L, vector2, 100f, 1f);
		this.character.MoveBodypartTowardsPoint(BodypartType.Hand_R, vector2, 100f, 1f);
		Vector3 vector3 = this.character.TorsoPos() - vector;
		Vector3 vector4 = vector2 + vector3 - this.character.TorsoPos();
		vector4 += this.character.data.currentClimbHandle.transform.up * this.handleOffset.y;
		vector4 += this.character.data.currentClimbHandle.transform.right * this.handleOffset.x;
		this.character.AddForce(vector4 * 50f, 1f, 1f);
	}

	// Token: 0x06000B81 RID: 2945 RVA: 0x000390A0 File Offset: 0x000372A0
	public void StopClimbing()
	{
		if (!this.view.IsMine)
		{
			return;
		}
		Debug.Log("StopClimbing");
		this.view.RPC("StopClimbingRpc", RpcTarget.All, new object[] { this.GetFallSpeed() });
	}

	// Token: 0x06000B82 RID: 2946 RVA: 0x000390E0 File Offset: 0x000372E0
	[PunRPC]
	public void RPCA_ClimbJump()
	{
		this.character.data.sinceClimbJump = 0f;
		this.character.UseStamina(0.2f, true);
		this.playerSlide += this.character.input.movementInput.normalized * 8f;
		if (this.view.IsMine && !this.character.isBot)
		{
			GamefeelHandler.instance.AddPerlinShake(10f, 0.5f, 10f);
			GUIManager.instance.ClimbJump();
		}
	}

	// Token: 0x06000B83 RID: 2947 RVA: 0x00039184 File Offset: 0x00037384
	private void GetDragged(Vector3 targetPos, float force)
	{
		this.character.data.climbPos += Vector3.ClampMagnitude(targetPos - this.character.Center, 1f) * (force * Time.fixedDeltaTime * 0.1f);
	}

	// Token: 0x06000B84 RID: 2948 RVA: 0x000391DC File Offset: 0x000373DC
	private void Climbing()
	{
		if (this.character.data.sinceClimbJump > 0.5f)
		{
			this.playerSlide += Vector2.down * 60f * Mathf.Pow(Mathf.Clamp01(this.character.data.outOfStaminaFor * 0.2f), 1f) * Time.fixedDeltaTime;
		}
		if (!this.SampleWall(this.GetRequestedPostition()).transform)
		{
			if (this.view.IsMine)
			{
				this.view.RPC("StopClimbingRpc", RpcTarget.All, new object[] { this.GetFallSpeed() });
			}
			return;
		}
		this.character.refs.movement.ApplyExtraDrag(this.climbDrag, false);
		this.character.AddForce(this.GetClimbDirection(), 1f, 1f);
	}

	// Token: 0x06000B85 RID: 2949 RVA: 0x000392DC File Offset: 0x000374DC
	private float GetFallSpeed()
	{
		float num = Mathf.InverseLerp(-5f, -30f, this.playerSlide.y) * 5f;
		float num2 = 0f;
		return Mathf.Max(num, num2);
	}

	// Token: 0x06000B86 RID: 2950 RVA: 0x00039318 File Offset: 0x00037518
	private Vector3 GetRequestedPostition()
	{
		Vector3 normalized = Vector3.ProjectOnPlane(Vector3.up, this.character.data.climbNormal).normalized;
		Vector3 normalized2 = Vector3.Cross(normalized, this.character.data.climbNormal).normalized;
		Vector3 vector = Vector3.zero;
		ClimbModifierSurface climbMod = this.character.data.climbMod;
		if (climbMod && climbMod.onlySlideDown)
		{
			vector += normalized * -3f;
		}
		else if (this.character.data.sinceClimbJump > 0.5f && !this.character.OutOfStamina())
		{
			if (this.character.input.movementInput.y < 0f)
			{
				vector += normalized * -3f;
			}
			else
			{
				vector += normalized * (this.character.input.movementInput.y * this.character.data.staminaMod);
			}
		}
		float num = 1f;
		if (climbMod)
		{
			num = climbMod.speedMultiplier;
		}
		vector += this.playerSlide.y * normalized * num;
		vector += this.playerSlide.x * -normalized2 * num;
		vector += normalized * -0.5f * Mathf.Clamp01(this.character.data.slippy);
		this.playerSlide *= 0.97f;
		this.playerSlide = Vector2.MoveTowards(this.playerSlide, Vector2.zero, Time.deltaTime * 15f);
		vector += -normalized2 * (this.character.input.movementInput.x * this.character.data.staminaMod);
		if (this.character.data.currentClimbHandle)
		{
			Vector3 vector2 = Vector3.ClampMagnitude(this.HandlePos() - this.character.data.climbPos, 1f) * 5f;
			float num2 = 1f;
			if (this.character.data.sinceClimbHandle > 0.5f)
			{
				num2 = Mathf.Lerp(1f, 0.15f, this.character.input.movementInput.magnitude);
			}
			vector = Vector3.Lerp(vector, vector2, num2);
		}
		return this.character.data.climbPos + vector * (this.climbSpeed * Time.fixedDeltaTime * this.climbSpeedMod);
	}

	// Token: 0x06000B87 RID: 2951 RVA: 0x000395EB File Offset: 0x000377EB
	private Vector3 HandlePos()
	{
		return this.character.data.currentClimbHandle.transform.position + Vector3.down * 1.5f;
	}

	// Token: 0x06000B88 RID: 2952 RVA: 0x0003961B File Offset: 0x0003781B
	private Vector3 GetClimbDirection()
	{
		return (this.VisualClimberPos() - this.character.TorsoPos()) * this.climbForce;
	}

	// Token: 0x06000B89 RID: 2953 RVA: 0x0003963E File Offset: 0x0003783E
	private Vector3 VisualClimberPos()
	{
		return this.GetVisualClimberPos(this.character.data.climbPos, this.character.data.climbNormal);
	}

	// Token: 0x06000B8A RID: 2954 RVA: 0x00039666 File Offset: 0x00037866
	private Vector3 GetVisualClimberPos(Vector3 samplePos, Vector3 sampleNormal)
	{
		return samplePos + sampleNormal * 0.4f;
	}

	// Token: 0x06000B8B RID: 2955 RVA: 0x0003967C File Offset: 0x0003787C
	private RaycastHit SampleWall(Vector3 samplePos)
	{
		Vector3 vector = this.RaycastPos();
		Vector3 vector2 = samplePos + this.character.data.climbNormal * 0.5f;
		Vector3 vector3 = samplePos + this.character.data.climbNormal * -1f;
		RaycastHit raycastHit = HelperFunctions.LineCheck(vector, vector2, HelperFunctions.LayerType.TerrainMap, 0f, QueryTriggerInteraction.Ignore);
		if (raycastHit.transform == null)
		{
			raycastHit = HelperFunctions.LineCheck(vector, vector3, HelperFunctions.LayerType.TerrainMap, 0f, QueryTriggerInteraction.Ignore);
		}
		if (raycastHit.transform == null)
		{
			raycastHit = HelperFunctions.LineCheck(vector, vector3, HelperFunctions.LayerType.TerrainMap, 0.1f, QueryTriggerInteraction.Ignore);
		}
		if (raycastHit.transform == null)
		{
			raycastHit = HelperFunctions.LineCheck(vector, vector3, HelperFunctions.LayerType.TerrainMap, 0.2f, QueryTriggerInteraction.Ignore);
		}
		if (raycastHit.transform == null)
		{
			raycastHit = HelperFunctions.LineCheck(vector, vector3, HelperFunctions.LayerType.TerrainMap, 0.3f, QueryTriggerInteraction.Ignore);
		}
		if (raycastHit.transform == null)
		{
			raycastHit = HelperFunctions.LineCheck(vector, vector3, HelperFunctions.LayerType.TerrainMap, 0.4f, QueryTriggerInteraction.Ignore);
		}
		if (raycastHit.transform)
		{
			float num = Vector3.Angle(raycastHit.normal, Vector3.up) - 90f;
			if (num > 0f)
			{
				if (Mathf.Abs(num) > (float)(this.character.OutOfStamina() ? 60 : 80))
				{
					return default(RaycastHit);
				}
			}
			else if (this.character.data.sinceClimbJump > 0.3f)
			{
				if (this.character.input.movementInput.magnitude < 0.1f)
				{
					if (Mathf.Abs(num) > 60f)
					{
						return default(RaycastHit);
					}
				}
				else if (Mathf.Abs(num) > 40f)
				{
					return default(RaycastHit);
				}
			}
			this.character.data.climbMod = raycastHit.collider.GetComponent<ClimbModifierSurface>();
			if (this.character.data.climbMod != null)
			{
				this.character.data.climbMod.OnClimb(this.character);
			}
			this.character.data.climbPos = raycastHit.point;
			this.character.data.climbNormal = raycastHit.normal;
			this.character.data.climbHit = raycastHit;
		}
		return raycastHit;
	}

	// Token: 0x06000B8C RID: 2956 RVA: 0x000398C8 File Offset: 0x00037AC8
	private bool AcceptableGrabAngle(Vector3 normal)
	{
		float num = Vector3.Angle(normal, Vector3.up) - 90f;
		if (num > 0f)
		{
			if (Mathf.Abs(num) > 80f)
			{
				return false;
			}
		}
		else if (Mathf.Abs(num) > 40f)
		{
			return false;
		}
		return true;
	}

	// Token: 0x06000B8D RID: 2957 RVA: 0x00039910 File Offset: 0x00037B10
	private void TryToStartWallClimb(bool forceAttempt = false, Vector3 overide = default(Vector3), bool botGrab = false)
	{
		if (!this.CanClimb())
		{
			return;
		}
		if (this.character.isBot && !botGrab)
		{
			return;
		}
		Vector3 vector = MainCamera.instance.transform.position;
		Vector3 vector2 = this.character.data.lookDirection;
		if (botGrab)
		{
			vector = this.character.Center;
			vector2 = this.character.data.lookDirection_Flat.normalized;
		}
		if (forceAttempt)
		{
			vector2 = overide;
		}
		Vector3 vector3 = vector + vector2 * 1.25f;
		RaycastHit raycastHit = HelperFunctions.LineCheck(vector, vector3, HelperFunctions.LayerType.TerrainMap, 0f, QueryTriggerInteraction.Ignore);
		if (!raycastHit.transform)
		{
			raycastHit = HelperFunctions.LineCheck(vector, vector3, HelperFunctions.LayerType.TerrainMap, 0.05f, QueryTriggerInteraction.Ignore);
		}
		if (!raycastHit.transform)
		{
			raycastHit = HelperFunctions.LineCheck(vector, vector3, HelperFunctions.LayerType.TerrainMap, 0.1f, QueryTriggerInteraction.Ignore);
		}
		if (raycastHit.transform && this.AcceptableGrabAngle(raycastHit.normal))
		{
			this.character.data.sinceCanClimb = 0f;
			if (this.character.data.sincePressClimb < 0.1f || (this.character.input.jumpWasPressed && this.character.data.sinceGrounded > 0.1f) || forceAttempt || botGrab)
			{
				this.character.refs.items.EquipSlot(Optionable<byte>.None);
				if (InputHandler.GetCurrentUsedInputScheme() == InputScheme.Gamepad)
				{
					this.climbToggledOn = true;
				}
				this.view.RPC("StartClimbRpc", RpcTarget.All, new object[] { raycastHit.point, raycastHit.normal });
			}
		}
	}

	// Token: 0x06000B8E RID: 2958 RVA: 0x00039AC0 File Offset: 0x00037CC0
	public bool CanClimb()
	{
		return this.character.data.sinceClimb >= 0.2f && !this.character.data.isRopeClimbing && !this.character.data.isVineClimbing;
	}

	// Token: 0x06000B8F RID: 2959 RVA: 0x00039B0F File Offset: 0x00037D0F
	private Vector3 RaycastPos()
	{
		return this.character.data.climbPos + this.character.data.climbNormal * 0.4f;
	}

	// Token: 0x06000B90 RID: 2960 RVA: 0x00039B40 File Offset: 0x00037D40
	[PunRPC]
	private void StartClimbRpc(Vector3 climbPos, Vector3 climbNormal)
	{
		float num = 0f;
		if (this.character.data.hasClimbedSinceGrounded)
		{
			Vector3 vector = this.GetVisualClimberPos(climbPos, climbNormal) - this.character.Center;
			float magnitude = Vector3.ProjectOnPlane(vector, climbNormal).magnitude;
			this.character.UseStamina(0.15f * magnitude, true);
			if (this.character.OutOfStamina())
			{
				num += -magnitude * this.outOfStamAttachSlide;
			}
		}
		if (this.character.data.avarageVelocity.y < 0f)
		{
			num += this.character.data.avarageVelocity.y * 1.5f;
		}
		this.character.OutOfStamina();
		this.playerSlide.y = num;
		this.character.data.climbPos = climbPos;
		this.character.data.climbNormal = climbNormal;
		this.character.data.hasClimbedSinceGrounded = true;
		this.character.data.isClimbing = true;
		this.character.data.isGrounded = false;
		this.character.data.sinceStartClimb = 0f;
		this.character.OnStartClimb();
	}

	// Token: 0x06000B91 RID: 2961 RVA: 0x00039C84 File Offset: 0x00037E84
	[PunRPC]
	public void StopClimbingRpc(float setFall)
	{
		this.character.data.isClimbing = false;
		this.character.data.isJumping = false;
		this.character.data.sinceGrounded = setFall;
		if (this.character.OutOfStamina())
		{
			this.character.data.sinceGrounded = Mathf.Clamp(this.character.data.sinceGrounded, 0.5f, 1000f);
		}
		this.playerSlide = Vector2.zero;
		this.climbToggledOn = false;
		Debug.Log("Stop Climbing");
	}

	// Token: 0x06000B92 RID: 2962 RVA: 0x00039D1C File Offset: 0x00037F1C
	internal void StartHang(ClimbHandle climbHandle)
	{
		this.character.data.currentClimbHandle = climbHandle;
		this.character.data.sinceClimbHandle = 0f;
		this.character.data.isClimbing = false;
		this.character.data.sinceGrounded = 0f;
	}

	// Token: 0x06000B93 RID: 2963 RVA: 0x00039D78 File Offset: 0x00037F78
	internal void TryClimb()
	{
		this.TryToStartWallClimb(false, default(Vector3), true);
	}

	// Token: 0x06000B94 RID: 2964 RVA: 0x00039D98 File Offset: 0x00037F98
	internal void TestAchievement()
	{
		if (!this.character.IsLocal)
		{
			return;
		}
		if (this.character.data.isClimbing && this.character.data.sinceGrounded > this.character.data.sinceClimb + 1f && (this.character.Center.y - this.character.data.lastGroundedHeight) * CharacterStats.unitsToMeters >= 50f)
		{
			Singleton<AchievementManager>.Instance.ThrowAchievement(ACHIEVEMENTTYPE.EnduranceBadge);
		}
	}

	// Token: 0x04000A81 RID: 2689
	private Character character;

	// Token: 0x04000A82 RID: 2690
	public float outOfStamAttachSlide = 1f;

	// Token: 0x04000A83 RID: 2691
	public float climbForce;

	// Token: 0x04000A84 RID: 2692
	public float climbSpeed;

	// Token: 0x04000A85 RID: 2693
	public float climbSpeedMod = 1f;

	// Token: 0x04000A86 RID: 2694
	public float climbDrag = 0.85f;

	// Token: 0x04000A87 RID: 2695
	public float maxStaminaUsage = 0.2f;

	// Token: 0x04000A88 RID: 2696
	public float minStaminaUsage = 0.02f;

	// Token: 0x04000A89 RID: 2697
	private PhotonView view;

	// Token: 0x04000A8A RID: 2698
	private Vector2 playerSlide;

	// Token: 0x04000A8B RID: 2699
	private float sinceShake;

	// Token: 0x04000A8C RID: 2700
	private Vector2 handleOffset;

	// Token: 0x04000A8D RID: 2701
	private bool sprintHasBeenPressedSinceClimb;

	// Token: 0x04000A8E RID: 2702
	private bool climbToggledOn;
}
