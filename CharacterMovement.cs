using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Zorro.ControllerSupport;
using Zorro.Core;
using Zorro.Settings;

// Token: 0x0200000E RID: 14
public class CharacterMovement : MonoBehaviour
{
	// Token: 0x06000130 RID: 304 RVA: 0x00009C54 File Offset: 0x00007E54
	private void Start()
	{
		this.character = base.GetComponent<Character>();
		this.mouseSensSetting = GameHandler.Instance.SettingsHandler.GetSetting<MouseSensitivitySetting>();
		this.controllerSensSetting = GameHandler.Instance.SettingsHandler.GetSetting<ControllerSensitivitySetting>();
		this.invertXSetting = GameHandler.Instance.SettingsHandler.GetSetting<InvertXSetting>();
		this.invertYSetting = GameHandler.Instance.SettingsHandler.GetSetting<InvertYSetting>();
	}

	// Token: 0x06000131 RID: 305 RVA: 0x00009CC1 File Offset: 0x00007EC1
	internal bool CanMoveCamera()
	{
		return !this.character.data.usingWheel;
	}

	// Token: 0x06000132 RID: 306 RVA: 0x00009CD8 File Offset: 0x00007ED8
	private void Update()
	{
		if (this.character.data.lastStoodOnPlayer)
		{
			this.CheckForPalJump(this.character.data.lastStoodOnPlayer);
		}
		if (this.character.IsLocal)
		{
			if (Singleton<MainCameraMovement>.Instance && Singleton<MainCameraMovement>.Instance.isGodCam)
			{
				this.character.input.ResetInput();
			}
			else
			{
				this.character.input.Sample(this.character.CanDoInput());
			}
		}
		if (this.CanMoveCamera())
		{
			this.CameraLook();
		}
		if (this.character.input.jumpWasPressed)
		{
			this.TryToJump();
		}
		this.SetMovementState();
		this.character.CalculateWorldMovementDir();
	}

	// Token: 0x06000133 RID: 307 RVA: 0x00009D9B File Offset: 0x00007F9B
	private void SetCrouch(bool setCrouch)
	{
		if (setCrouch != this.character.data.isCrouching)
		{
			this.character.refs.view.RPC("RPCA_SetCrouch", RpcTarget.All, new object[] { setCrouch });
		}
	}

	// Token: 0x06000134 RID: 308 RVA: 0x00009DDA File Offset: 0x00007FDA
	[PunRPC]
	public void RPCA_SetCrouch(bool setCrouch)
	{
		this.character.data.isCrouching = setCrouch;
	}

	// Token: 0x06000135 RID: 309 RVA: 0x00009DF0 File Offset: 0x00007FF0
	private void SetMovementState()
	{
		if (!this.character.refs.view.IsMine)
		{
			return;
		}
		if (this.character.input.crouchToggleWasPressed)
		{
			this.crouchToggleEnabled = !this.crouchToggleEnabled;
		}
		if (this.crouchToggleEnabled || (this.character.input.crouchIsPressed && this.character.data.isGrounded))
		{
			this.SetCrouch(true);
		}
		else
		{
			this.SetCrouch(false);
		}
		if (this.character.data.sinceGrounded > 0.2f || this.character.data.isSprinting || this.character.data.isClimbing || this.character.data.isRopeClimbing)
		{
			this.SetCrouch(false);
		}
		if (!this.character.data.isGrounded || this.character.data.isSprinting)
		{
			this.crouchToggleEnabled = false;
		}
		if (!this.character.data.isGrounded)
		{
			this.character.data.isSprinting = this.character.input.movementInput.y > 0.01f && (this.character.input.sprintIsPressed || this.sprintToggleEnabled) && this.character.CheckSprint();
			if (!this.character.data.isSprinting)
			{
				this.sprintToggleEnabled = false;
			}
			return;
		}
		if (this.character.input.sprintToggleWasPressed)
		{
			this.sprintToggleEnabled = true;
		}
		this.character.data.isSprinting = this.character.input.movementInput.y > 0.01f && (this.character.input.sprintIsPressed || this.sprintToggleEnabled) && this.character.CheckSprint() && !this.character.OutOfRegularStamina();
		if (this.character.data.isSprinting)
		{
			this.character.UseStamina(this.sprintStaminaUsage * Time.deltaTime, true);
			return;
		}
		this.sprintToggleEnabled = false;
	}

	// Token: 0x06000136 RID: 310 RVA: 0x0000A024 File Offset: 0x00008224
	private void CameraLook()
	{
		float num = 0.1f;
		if (InputHandler.GetCurrentUsedInputScheme() == InputScheme.KeyboardMouse)
		{
			num *= this.mouseSensSetting.Value;
		}
		else
		{
			num *= this.controllerSensSetting.Value;
		}
		CharacterData data = this.character.data;
		data.lookValues.x = data.lookValues.x + this.character.input.lookInput.x * num * (float)((this.invertYSetting.Value == OffOnMode.OFF) ? 1 : (-1));
		CharacterData data2 = this.character.data;
		data2.lookValues.y = data2.lookValues.y + this.character.input.lookInput.y * num * (float)((this.invertXSetting.Value == OffOnMode.OFF) ? 1 : (-1));
		this.character.data.lookValues.y = Mathf.Clamp(this.character.data.lookValues.y, -85f, 85f);
		this.character.RecalculateLookDirections();
	}

	// Token: 0x06000137 RID: 311 RVA: 0x0000A128 File Offset: 0x00008328
	private void FixedUpdate()
	{
		this.UpdateVariables();
		this.RaycastGroundCheck();
		this.EvaluateGroundChecks();
		if (this.character.data.isGrounded && this.character.CheckStand())
		{
			this.Stand();
		}
		Vector3 gravityForce = this.GetGravityForce();
		float num = this.GetMovementForce();
		if (this.character.data.currentItem)
		{
			this.character.refs.items.AddGravity(gravityForce);
			this.character.refs.items.AddMovementForce(num);
			this.character.refs.items.AddDrag(this.drag, 1f);
		}
		for (int i = 0; i < this.character.refs.ragdoll.partList.Count; i++)
		{
			this.character.refs.ragdoll.partList[i].Animate(this.animationForce * this.character.data.currentRagdollControll, this.animationTorque * this.character.data.currentRagdollControll);
			if (!this.character.data.isGrounded)
			{
				this.character.refs.ragdoll.partList[i].Gravity(gravityForce * this.character.data.currentRagdollControll);
			}
			this.character.refs.ragdoll.partList[i].ToggleUseGravity(this.character.data.currentRagdollControll < 0.9f);
			this.character.refs.ragdoll.partList[i].AddMovementForce(num * this.character.data.currentRagdollControll);
			this.character.refs.ragdoll.partList[i].Drag(this.drag, false);
			this.character.refs.ragdoll.partList[i].ApplyForces();
		}
	}

	// Token: 0x06000138 RID: 312 RVA: 0x0000A350 File Offset: 0x00008550
	public void ApplyExtraDrag(float extraDrag, bool ignoreRagdoll = false)
	{
		if (this.character.data.currentItem)
		{
			this.character.refs.items.AddDrag(Mathf.Lerp(1f, extraDrag, this.character.data.currentRagdollControll), 1f);
		}
		for (int i = 0; i < this.character.refs.ragdoll.partList.Count; i++)
		{
			this.character.refs.ragdoll.partList[i].Drag(extraDrag, ignoreRagdoll);
		}
	}

	// Token: 0x06000139 RID: 313 RVA: 0x0000A3F0 File Offset: 0x000085F0
	private float GetMovementForce()
	{
		if (!this.character.CheckMovement())
		{
			return 0f;
		}
		float num = this.movementForce * this.movementModifier;
		if (this.character.data.isSprinting)
		{
			num *= this.sprintMultiplier;
		}
		if (this.character.data.isCrouching)
		{
			num *= 0.5f;
		}
		return num;
	}

	// Token: 0x0600013A RID: 314 RVA: 0x0000A454 File Offset: 0x00008654
	private void TryToJump()
	{
		if (this.character.data.jumpsRemaining <= 0)
		{
			return;
		}
		if (!this.character.CheckJump())
		{
			return;
		}
		if (this.character.data.sinceGrounded > 0.2f)
		{
			return;
		}
		if (this.character.data.sinceJump < 0.3f)
		{
			return;
		}
		if (this.character.data.chargingJump)
		{
			return;
		}
		this.character.refs.view.RPC("JumpRpc", RpcTarget.All, new object[] { false });
	}

	// Token: 0x0600013B RID: 315 RVA: 0x0000A4F0 File Offset: 0x000086F0
	[PunRPC]
	public void JumpRpc(bool isPalJump)
	{
		CharacterMovement.<>c__DisplayClass37_0 CS$<>8__locals1 = new CharacterMovement.<>c__DisplayClass37_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.staminaCostMult = 1f;
		CS$<>8__locals1.jumpMult = 1f;
		CS$<>8__locals1.jumpDir = Vector3.up;
		if (isPalJump)
		{
			CS$<>8__locals1.staminaCostMult = 0f;
			CS$<>8__locals1.jumpMult = 2f;
			this.character.data.sincePalJump = 0f;
			CS$<>8__locals1.jumpDir += this.character.data.lookDirection_Flat * 0.25f;
			for (int i = 0; i < this.boostPlayer.Length; i++)
			{
				this.boostPlayer[i].Play(this.character.Center);
			}
		}
		this.character.data.jumpsRemaining--;
		this.character.data.isCrouching = false;
		this.character.data.chargingJump = true;
		this.character.OnStartJump();
		base.StartCoroutine(CS$<>8__locals1.<JumpRpc>g__IDoJump|0());
	}

	// Token: 0x0600013C RID: 316 RVA: 0x0000A604 File Offset: 0x00008804
	private void UpdateVariables()
	{
		if (this.character.refs.ragdoll == null || this.character.refs.ragdoll.partList == null)
		{
			return;
		}
		this.character.data.avarageLastFrameVelocity = this.character.data.avarageVelocity;
		this.character.data.avarageVelocity = Vector3.zero;
		for (int i = 0; i < this.character.refs.ragdoll.partList.Count; i++)
		{
			this.character.data.avarageVelocity += this.character.refs.ragdoll.partList[i].Rig.linearVelocity / (float)this.character.refs.ragdoll.partList.Count;
		}
	}

	// Token: 0x0600013D RID: 317 RVA: 0x0000A700 File Offset: 0x00008900
	private Vector3 GetGravityForce()
	{
		float num = 0f;
		if (!this.character.data.isGrounded && this.character.CheckGravity())
		{
			float sinceGrounded = this.character.data.sinceGrounded;
			float num2 = this.jumpGravityCurve.Evaluate(sinceGrounded * this.gravityCurveSpeed);
			if (this.character.data.isJumping)
			{
				num = Mathf.Lerp(this.jumpGravity, this.maxGravity, num2);
			}
			else
			{
				num = Mathf.Lerp(0f, this.maxGravity, num2);
			}
		}
		return num * Vector3.up;
	}

	// Token: 0x0600013E RID: 318 RVA: 0x0000A79C File Offset: 0x0000899C
	private void Stand()
	{
		float targetHeadHeight = this.character.data.targetHeadHeight;
		float num = Mathf.InverseLerp(targetHeadHeight, targetHeadHeight - this.standSmooth, this.character.data.currentHeadHeight);
		float num2 = Mathf.InverseLerp(targetHeadHeight, targetHeadHeight + this.standSmooth, this.character.data.currentHeadHeight);
		this.character.GetBodypart(BodypartType.Head).Rig.AddForce(Vector3.up * (num + -num2) * this.standForce * this.character.data.currentRagdollControll, ForceMode.Acceleration);
		this.character.GetBodypart(BodypartType.Torso).Rig.AddForce(Vector3.up * (num + -num2) * this.standForce * this.character.data.currentRagdollControll, ForceMode.Acceleration);
		this.character.GetBodypart(BodypartType.Hip).Rig.AddForce(Vector3.up * (num + -num2) * this.standForce * this.character.data.currentRagdollControll, ForceMode.Acceleration);
		Debug.DrawRay(this.character.GetBodypart(BodypartType.Hip).Rig.position + Vector3.right, Vector3.up * (num + -num2));
	}

	// Token: 0x0600013F RID: 319 RVA: 0x0000A8F8 File Offset: 0x00008AF8
	private void EvaluateGroundChecks()
	{
		CharacterMovement.PlayerGroundSample playerGroundSample = null;
		for (int i = 0; i < this.groundSamples.Count; i++)
		{
			if (playerGroundSample == null)
			{
				playerGroundSample = this.groundSamples[0];
			}
			else if (this.groundSamples[0].point.y > playerGroundSample.point.y)
			{
				playerGroundSample = this.groundSamples[0];
			}
		}
		if (playerGroundSample == null)
		{
			playerGroundSample = this.IsLodged();
		}
		if (playerGroundSample != null && this.CanStand())
		{
			if (!this.character.data.isGrounded)
			{
				this.Land(playerGroundSample);
			}
			this.character.data.hasClimbedSinceGrounded = false;
			this.character.data.jumpsRemaining = 1;
			this.character.data.isJumping = false;
			this.character.data.isGrounded = true;
			this.character.data.groundNormal = playerGroundSample.normal;
			this.character.data.groundPos = playerGroundSample.point;
			this.character.data.currentHeadHeight = this.character.GetBodypart(BodypartType.Head).Rig.transform.position.y - playerGroundSample.point.y;
		}
		else
		{
			this.character.data.isGrounded = false;
		}
		this.groundSamples.Clear();
		this.groundSamples_All.Clear();
	}

	// Token: 0x06000140 RID: 320 RVA: 0x0000AA68 File Offset: 0x00008C68
	private CharacterMovement.PlayerGroundSample IsLodged()
	{
		Vector3 zero = Vector3.zero;
		Vector3 zero2 = Vector3.zero;
		CharacterMovement.PlayerGroundSample playerGroundSample = null;
		for (int i = 0; i < this.groundSamples_All.Count; i++)
		{
			Vector3 normal = this.groundSamples_All[i].normal;
			if (normal.y > 0f)
			{
				zero2 = new Vector3(Mathf.Min(zero2.x, normal.x), Mathf.Min(zero2.y, normal.y), Mathf.Min(zero2.z, normal.z));
				zero = new Vector3(Mathf.Max(zero.x, normal.x), Mathf.Max(zero.y, normal.y), Mathf.Max(zero.z, normal.z));
			}
			Debug.DrawRay(this.groundSamples_All[i].point, this.groundSamples_All[i].normal * 1.5f, Color.blue);
			if (playerGroundSample == null)
			{
				playerGroundSample = this.groundSamples_All[i];
			}
			else if (this.groundSamples_All[i].point.y > playerGroundSample.point.y)
			{
				playerGroundSample = this.groundSamples_All[i];
			}
		}
		Vector3 vector = (zero + zero2) / 2f;
		if (vector.magnitude < 0.1f)
		{
			playerGroundSample = null;
		}
		if (playerGroundSample != null)
		{
			if (!this.AcceptableAngle(Vector3.Angle(vector, Vector3.up)))
			{
				Debug.DrawRay(playerGroundSample.point, vector * 2f, Color.red);
				playerGroundSample = null;
			}
			else
			{
				Debug.DrawRay(playerGroundSample.point, vector * 2f, Color.green);
			}
		}
		return playerGroundSample;
	}

	// Token: 0x06000141 RID: 321 RVA: 0x0000AC2F File Offset: 0x00008E2F
	private bool CanStand()
	{
		return this.character.data.sinceJump > 0.3f && this.character.data.currentClimbHandle == null;
	}

	// Token: 0x06000142 RID: 322 RVA: 0x0000AC60 File Offset: 0x00008E60
	private void Land(CharacterMovement.PlayerGroundSample bestSample)
	{
		if (this.character.data.sinceGrounded > 0.5f)
		{
			this.CheckFallDamage();
			if (this.character.IsLocal)
			{
				GUIManager.instance.ReticleLand();
			}
			this.character.OnLand(this.character.data.sinceGrounded);
		}
	}

	// Token: 0x06000143 RID: 323 RVA: 0x0000ACBC File Offset: 0x00008EBC
	private void CheckFallDamage()
	{
		if (this.FallTime() > this.fallDamageTime)
		{
			float num = Mathf.Max(this.FallFactor(3f, 1.5f), 0.05f);
			float num2 = num;
			num = Mathf.Min(num, this.MaxVelDmg());
			float num3 = num / num2;
			if (num >= 0.025f)
			{
				if (num > 0.3f && this.character.IsLocal)
				{
					this.character.Fall(num * 5f);
				}
				num *= Ascents.fallDamageMultiplier;
				if (this.character.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Injury, num, false))
				{
					Singleton<AchievementManager>.Instance.AddToRunBasedFloat(RUNBASEDVALUETYPE.FallDamageTaken, num);
				}
			}
		}
	}

	// Token: 0x06000144 RID: 324 RVA: 0x0000AD65 File Offset: 0x00008F65
	private float MaxVelDmg()
	{
		return Mathf.Pow(Mathf.InverseLerp(10f, 20f, this.character.data.avarageLastFrameVelocity.magnitude), 1.5f);
	}

	// Token: 0x06000145 RID: 325 RVA: 0x0000AD98 File Offset: 0x00008F98
	private float FallTime()
	{
		float num = Mathf.Min(this.character.data.sinceJump, this.character.data.sinceGrounded);
		if (this.character.data.sinceGrounded - this.character.data.sinceJump > -0.05f)
		{
			num -= 0.5f;
		}
		return num;
	}

	// Token: 0x06000146 RID: 326 RVA: 0x0000ADFC File Offset: 0x00008FFC
	private float FallFactor(float maxTime = 3f, float pow = 1.5f)
	{
		float num = this.FallTime();
		return Mathf.Pow(Mathf.InverseLerp(this.fallDamageTime, maxTime, num), 1.5f);
	}

	// Token: 0x06000147 RID: 327 RVA: 0x0000AE27 File Offset: 0x00009027
	public void AddGroundSample(CharacterMovement.PlayerGroundSample sample)
	{
		this.groundSamples.Add(sample);
	}

	// Token: 0x06000148 RID: 328 RVA: 0x0000AE35 File Offset: 0x00009035
	public void AddGroundSample_All(CharacterMovement.PlayerGroundSample sample)
	{
		this.groundSamples_All.Add(sample);
	}

	// Token: 0x06000149 RID: 329 RVA: 0x0000AE44 File Offset: 0x00009044
	private bool AcceptableAngle(float angle)
	{
		float num = this.maxAngle;
		return angle < num;
	}

	// Token: 0x0600014A RID: 330 RVA: 0x0000AE5C File Offset: 0x0000905C
	private void RaycastGroundCheck()
	{
		Vector3 position = this.character.GetBodypartRig(BodypartType.Hip).position;
		Vector3 vector = position + Vector3.down * (this.character.data.targetHipHeight + 0.3f);
		RaycastHit raycastHit = HelperFunctions.LineCheck(position, vector, HelperFunctions.LayerType.TerrainMap, 0f, QueryTriggerInteraction.Ignore);
		if (raycastHit.transform)
		{
			float num = Vector3.Angle(Vector3.up, raycastHit.normal);
			if (!this.AcceptableAngle(num))
			{
				if (!this.character.data.isClimbing && !this.character.data.isRopeClimbing && ((double)this.character.data.sinceFallSlide < 0.2 || this.character.data.sinceGrounded < 2f))
				{
					this.character.data.sinceFallSlide = 0f;
					this.shakeCooldown += Time.deltaTime;
					this.ApplyExtraDrag(0.9f, false);
					this.LowerFall(num);
					if (this.shakeCooldown > 0.1f && this.FallTime() > this.fallDamageTime)
					{
						if (this.character.IsLocal)
						{
							GamefeelHandler.instance.AddPerlinShake(5f * this.FallFactor(3f, 1f), 0.2f, 10f);
						}
						this.shakeCooldown = 0f;
					}
				}
				return;
			}
			if (this.StandableRig(raycastHit.rigidbody) && this.DoGroundChecks() && this.character.data.groundedFor > 0.1f)
			{
				this.AddGroundSample(new CharacterMovement.PlayerGroundSample(raycastHit.point, raycastHit.normal));
			}
		}
	}

	// Token: 0x0600014B RID: 331 RVA: 0x0000B020 File Offset: 0x00009220
	private void LowerFall(float upAngle)
	{
		float num = Mathf.InverseLerp(60f, 40f, upAngle);
		if (this.character.data.sinceGrounded > 1f)
		{
			this.character.data.sinceGrounded = Mathf.MoveTowards(this.character.data.sinceGrounded, 1f, num * Time.deltaTime * 2f);
		}
	}

	// Token: 0x0600014C RID: 332 RVA: 0x0000B08C File Offset: 0x0000928C
	internal void OnCollision(Collision collision, bool collisionEnter)
	{
		CollisionModifier component = collision.collider.GetComponent<CollisionModifier>();
		if (component)
		{
			component.Collide(this.character, collision.contacts[0]);
		}
		bool flag = false;
		if (this.StandOnPlayer(collision))
		{
			flag = true;
		}
		else if (!this.StandableRig(collision.rigidbody))
		{
			return;
		}
		float num = Vector3.Angle(Vector3.up, collision.contacts[0].normal);
		if (this.DoGroundChecks())
		{
			if (this.AcceptableAngle(num) || flag)
			{
				this.AddGroundSample(new CharacterMovement.PlayerGroundSample(collision.contacts[0].point, collision.contacts[0].normal));
			}
			this.AddGroundSample_All(new CharacterMovement.PlayerGroundSample(collision.contacts[0].point, collision.contacts[0].normal));
		}
	}

	// Token: 0x0600014D RID: 333 RVA: 0x0000B16C File Offset: 0x0000936C
	private bool StandOnPlayer(Collision collision)
	{
		if (this.character.data.sincePalJump < 0.5f)
		{
			return false;
		}
		if (!collision.rigidbody)
		{
			return false;
		}
		Character componentInParent = collision.rigidbody.GetComponentInParent<Character>();
		if (componentInParent == this.character)
		{
			return false;
		}
		if (!componentInParent)
		{
			return false;
		}
		if (this.character.data.isCrouching)
		{
			return false;
		}
		if (!componentInParent.data.isCrouching)
		{
			return false;
		}
		this.character.data.sinceStandOnPlayer = 0f;
		this.character.data.lastStoodOnPlayer = componentInParent;
		return true;
	}

	// Token: 0x0600014E RID: 334 RVA: 0x0000B214 File Offset: 0x00009414
	private void CheckForPalJump(Character c)
	{
		if (this.character.data.sinceStandOnPlayer < 0.3f && c.data.sinceJump < 0.3f)
		{
			this.character.data.lastStoodOnPlayer = null;
			if (this.character.refs.view.IsMine)
			{
				this.character.refs.view.RPC("JumpRpc", RpcTarget.All, new object[] { true });
			}
		}
	}

	// Token: 0x0600014F RID: 335 RVA: 0x0000B29C File Offset: 0x0000949C
	private bool StandableRig(Rigidbody rig)
	{
		return rig == null || rig.mass > 500f || rig.isKinematic;
	}

	// Token: 0x06000150 RID: 336 RVA: 0x0000B2C3 File Offset: 0x000094C3
	private bool DoGroundChecks()
	{
		return !this.character.data.isClimbing;
	}

	// Token: 0x04000131 RID: 305
	private Character character;

	// Token: 0x04000132 RID: 306
	public float movementForce = 10f;

	// Token: 0x04000133 RID: 307
	public float movementModifier = 1f;

	// Token: 0x04000134 RID: 308
	public float sprintMultiplier = 1f;

	// Token: 0x04000135 RID: 309
	public float sprintStaminaUsage = 0.025f;

	// Token: 0x04000136 RID: 310
	public float drag = 0.85f;

	// Token: 0x04000137 RID: 311
	public float movementTurnSpeed = 2f;

	// Token: 0x04000138 RID: 312
	public float animationForce = 100f;

	// Token: 0x04000139 RID: 313
	public float animationTorque = 10f;

	// Token: 0x0400013A RID: 314
	public float standForce;

	// Token: 0x0400013B RID: 315
	public float standSmooth = 0.2f;

	// Token: 0x0400013C RID: 316
	public float jumpImpulse;

	// Token: 0x0400013D RID: 317
	public float jumpGravity = 10f;

	// Token: 0x0400013E RID: 318
	public float jumpStaminaUsage;

	// Token: 0x0400013F RID: 319
	public float jumpStaminaUsageSprinting;

	// Token: 0x04000140 RID: 320
	public float maxGravity = -20f;

	// Token: 0x04000141 RID: 321
	public AnimationCurve jumpGravityCurve;

	// Token: 0x04000142 RID: 322
	public float gravityCurveSpeed = 1f;

	// Token: 0x04000143 RID: 323
	public float airMovementTurnSpeed = 2f;

	// Token: 0x04000144 RID: 324
	public SFX_Instance[] boostPlayer;

	// Token: 0x04000145 RID: 325
	private MouseSensitivitySetting mouseSensSetting;

	// Token: 0x04000146 RID: 326
	private ControllerSensitivitySetting controllerSensSetting;

	// Token: 0x04000147 RID: 327
	private InvertXSetting invertXSetting;

	// Token: 0x04000148 RID: 328
	private InvertYSetting invertYSetting;

	// Token: 0x04000149 RID: 329
	private bool sprintToggleEnabled;

	// Token: 0x0400014A RID: 330
	private bool crouchToggleEnabled;

	// Token: 0x0400014B RID: 331
	private float fallDamageTime = 1.5f;

	// Token: 0x0400014C RID: 332
	private float shakeCooldown;

	// Token: 0x0400014D RID: 333
	private float maxAngle = 50f;

	// Token: 0x0400014E RID: 334
	private List<CharacterMovement.PlayerGroundSample> groundSamples = new List<CharacterMovement.PlayerGroundSample>();

	// Token: 0x0400014F RID: 335
	private List<CharacterMovement.PlayerGroundSample> groundSamples_All = new List<CharacterMovement.PlayerGroundSample>();

	// Token: 0x020002EC RID: 748
	[Serializable]
	public class PlayerGroundSample
	{
		// Token: 0x06001267 RID: 4711 RVA: 0x0005A2D8 File Offset: 0x000584D8
		public PlayerGroundSample(Vector3 point, Vector3 normal)
		{
			this.point = point;
			this.normal = normal;
		}

		// Token: 0x040010B2 RID: 4274
		public Vector3 point;

		// Token: 0x040010B3 RID: 4275
		public Vector3 normal;
	}
}
