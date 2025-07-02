using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Zorro.Core;

// Token: 0x02000260 RID: 608
public class Scoutmaster : MonoBehaviour
{
	// Token: 0x170000BB RID: 187
	// (get) Token: 0x06000EA0 RID: 3744 RVA: 0x0004969D File Offset: 0x0004789D
	private bool targetForced
	{
		get
		{
			return Time.time < this.targetForcedUntil;
		}
	}

	// Token: 0x06000EA1 RID: 3745 RVA: 0x000496AC File Offset: 0x000478AC
	public static bool GetPrimaryScoutmaster(out Scoutmaster scoutmaster)
	{
		if (Scoutmaster.AllScoutmasters.Count == 0)
		{
			scoutmaster = null;
			return false;
		}
		scoutmaster = Scoutmaster.AllScoutmasters[0];
		return true;
	}

	// Token: 0x170000BC RID: 188
	// (get) Token: 0x06000EA2 RID: 3746 RVA: 0x000496CD File Offset: 0x000478CD
	// (set) Token: 0x06000EA3 RID: 3747 RVA: 0x000496D5 File Offset: 0x000478D5
	public Character currentTarget
	{
		get
		{
			return this._currentTarget;
		}
		set
		{
			if (this.targetForced)
			{
				return;
			}
			this._currentTarget = value;
		}
	}

	// Token: 0x06000EA4 RID: 3748 RVA: 0x000496E7 File Offset: 0x000478E7
	private void OnEnable()
	{
		Scoutmaster.AllScoutmasters.Add(this);
	}

	// Token: 0x06000EA5 RID: 3749 RVA: 0x000496F4 File Offset: 0x000478F4
	internal void SetCurrentTarget(Character setCurrentTarget, float forceForTime = 0f)
	{
		if (setCurrentTarget != this.currentTarget)
		{
			this.view.RPC("RPCA_SetCurrentTarget", RpcTarget.All, new object[]
			{
				(setCurrentTarget == null) ? (-1) : setCurrentTarget.photonView.ViewID,
				forceForTime
			});
		}
	}

	// Token: 0x06000EA6 RID: 3750 RVA: 0x0004974E File Offset: 0x0004794E
	[PunRPC]
	private void RPCA_SetCurrentTarget(int targetViewID, float forceForTime)
	{
		if (targetViewID == -1)
		{
			this.currentTarget = null;
		}
		else
		{
			this.currentTarget = PhotonNetwork.GetPhotonView(targetViewID).GetComponent<Character>();
		}
		if (forceForTime > 0f)
		{
			this.targetForcedUntil = Time.time + forceForTime;
		}
	}

	// Token: 0x06000EA7 RID: 3751 RVA: 0x00049783 File Offset: 0x00047983
	private void OnDestroy()
	{
		this.mat.SetFloat("_Strength", 0f);
	}

	// Token: 0x06000EA8 RID: 3752 RVA: 0x0004979A File Offset: 0x0004799A
	private void OnDisable()
	{
		this.mat.SetFloat("_Strength", 0f);
		Scoutmaster.AllScoutmasters.Remove(this);
	}

	// Token: 0x06000EA9 RID: 3753 RVA: 0x000497BD File Offset: 0x000479BD
	private void Start()
	{
		this.animVars = base.GetComponentInChildren<ScoutmasterAnimVars>();
		this.character = base.GetComponent<Character>();
		this.view = base.GetComponent<PhotonView>();
		this.mat.SetFloat("_Strength", 0f);
	}

	// Token: 0x06000EAA RID: 3754 RVA: 0x000497F8 File Offset: 0x000479F8
	private void CalcVars()
	{
		this.sinceLookForTarget += Time.deltaTime;
		bool flag = this.currentTarget && this.CanSeeTarget(this.currentTarget);
		if (this.currentTarget)
		{
			if (!flag)
			{
				this.sinceSeenTarget += Time.deltaTime;
			}
			else
			{
				this.sinceSeenTarget = 0f;
			}
		}
		else
		{
			this.sinceSeenTarget = 0f;
		}
		if (this.currentTarget)
		{
			this.distanceToTarget = Vector3.Distance(this.character.Center, this.currentTarget.Center);
		}
		this.sinceAnyoneCanSeeMe += Time.deltaTime;
		if (this.AnyoneCanSeeMe())
		{
			this.sinceAnyoneCanSeeMe = 0f;
		}
		if (!this.currentTarget)
		{
			this.targetHasSeenMeCounter = 0f;
			return;
		}
		bool flag2 = Vector3.Distance(this.character.Center, this.currentTarget.Center) < 10f;
		bool flag3 = HelperFunctions.LineCheck(this.character.Center, this.currentTarget.Head, HelperFunctions.LayerType.TerrainMap, 0f, QueryTriggerInteraction.Ignore).transform == null;
		if (Vector3.Angle(this.currentTarget.data.lookDirection, this.character.Center - this.currentTarget.Head) > 70f)
		{
			flag3 = false;
		}
		if (flag2 && flag3)
		{
			this.targetHasSeenMeCounter += Time.deltaTime * 1f;
			return;
		}
		if (flag3)
		{
			this.targetHasSeenMeCounter += Time.deltaTime * 0.3f;
			return;
		}
		if (flag2 && flag)
		{
			this.targetHasSeenMeCounter += Time.deltaTime * 0.15f;
			return;
		}
		this.targetHasSeenMeCounter = Mathf.MoveTowards(this.targetHasSeenMeCounter, 0f, Time.deltaTime * 0.1f);
	}

	// Token: 0x06000EAB RID: 3755 RVA: 0x000499E8 File Offset: 0x00047BE8
	private bool CanSeeTarget(Character currentTarget)
	{
		return HelperFunctions.LineCheck(this.character.Head, currentTarget.Center + Random.insideUnitSphere * 0.5f, HelperFunctions.LayerType.TerrainMap, 0f, QueryTriggerInteraction.Ignore).transform == null;
	}

	// Token: 0x06000EAC RID: 3756 RVA: 0x00049A34 File Offset: 0x00047C34
	private void DoVisuals()
	{
		float num = 0f;
		if (this.currentTarget)
		{
			this.currentTarget.data.myersDistance = Vector3.Distance(this.character.Center, this.currentTarget.Center);
		}
		if (this.currentTarget && this.currentTarget.IsLocal)
		{
			num = Mathf.InverseLerp(50f, 5f, this.distanceToTarget);
		}
		this.mat.SetFloat("_Strength", Mathf.Lerp(this.mat.GetFloat("_Strength"), num, Time.deltaTime * 0.5f));
	}

	// Token: 0x06000EAD RID: 3757 RVA: 0x00049AE0 File Offset: 0x00047CE0
	private void FixedUpdate()
	{
		if (this.animVars.reaching && this.character.data.grabbedPlayer == null && this.currentTarget)
		{
			Rigidbody bodypartRig = this.character.GetBodypartRig(BodypartType.Hand_R);
			Vector3 normalized = (this.currentTarget.Center - bodypartRig.transform.position).normalized;
			bodypartRig.AddForce(normalized * this.reachForce, ForceMode.Acceleration);
		}
	}

	// Token: 0x06000EAE RID: 3758 RVA: 0x00049B64 File Offset: 0x00047D64
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(base.transform.position, this.achievementDistance);
	}

	// Token: 0x06000EAF RID: 3759 RVA: 0x00049B88 File Offset: 0x00047D88
	private void Update()
	{
		this.DoVisuals();
		if (!this.view.IsMine)
		{
			return;
		}
		this.tpCounter += Time.deltaTime;
		this.ResetInput();
		this.CalcVars();
		if (this.chillForSeconds > 0f)
		{
			this.chillForSeconds -= Time.deltaTime;
			return;
		}
		if (this.currentTarget == null)
		{
			this.EvasiveBehaviour();
			this.LookForTarget();
		}
		else
		{
			if (this.distanceToTarget > 80f)
			{
				this.TeleportCloseToTarget();
			}
			else
			{
				this.Chase();
			}
			this.VerifyTarget();
		}
		this.achievementTestTick += Time.deltaTime;
		if (this.achievementTestTick > 1f)
		{
			this.achievementTestTick = 0f;
			this.TestAchievement();
		}
	}

	// Token: 0x06000EB0 RID: 3760 RVA: 0x00049C54 File Offset: 0x00047E54
	private void TestAchievement()
	{
		if (Vector3.Distance(this.character.Center, Character.localCharacter.Center) <= this.achievementDistance)
		{
			Singleton<AchievementManager>.Instance.ThrowAchievement(ACHIEVEMENTTYPE.MentorshipBadge);
		}
	}

	// Token: 0x06000EB1 RID: 3761 RVA: 0x00049C84 File Offset: 0x00047E84
	private void VerifyTarget()
	{
		if (this.ViableTargets() < 2)
		{
			this.SetCurrentTarget(null, 0f);
			return;
		}
		Character closestOther = this.GetClosestOther(this.currentTarget);
		Character highestCharacter = this.GetHighestCharacter(null);
		Character highestCharacter2 = this.GetHighestCharacter(highestCharacter);
		if (highestCharacter.Center.y > this.maxAggroHeight)
		{
			this.SetCurrentTarget(null, 0f);
			return;
		}
		if (this.currentTarget != highestCharacter)
		{
			this.SetCurrentTarget(null, 0f);
			return;
		}
		if (highestCharacter.Center.y < highestCharacter2.Center.y + this.attackHeightDelta - 20f)
		{
			this.SetCurrentTarget(null, 0f);
			return;
		}
		if (Vector3.Distance(closestOther.Center, this.currentTarget.Center) < 15f)
		{
			this.SetCurrentTarget(null, 0f);
			return;
		}
	}

	// Token: 0x06000EB2 RID: 3762 RVA: 0x00049D5C File Offset: 0x00047F5C
	private Character GetClosestOther(Character currentTarget)
	{
		List<Character> allCharacters = Character.AllCharacters;
		float num = float.MaxValue;
		Character character = null;
		foreach (Character character2 in allCharacters)
		{
			if (!character2.isBot && !(character2 == currentTarget))
			{
				float num2 = Vector3.Distance(character2.Center, currentTarget.Center);
				if (num2 < num)
				{
					num = num2;
					character = character2;
				}
			}
		}
		return character;
	}

	// Token: 0x06000EB3 RID: 3763 RVA: 0x00049DE0 File Offset: 0x00047FE0
	private void EvasiveBehaviour()
	{
		if (!this.discovered)
		{
			this.discovered = this.GetPlayerWhoSeesMe();
		}
		if (this.discovered)
		{
			this.Flee();
			if (this.sinceAnyoneCanSeeMe > 0.5f)
			{
				this.TeleportFarAway();
			}
		}
	}

	// Token: 0x06000EB4 RID: 3764 RVA: 0x00049E2C File Offset: 0x0004802C
	public void TeleportFarAway()
	{
		if (this.tpCounter < 5f)
		{
			return;
		}
		this.tpCounter = 0f;
		Debug.Log("Teleporting");
		this.view.RPC("WarpPlayerRPC", RpcTarget.All, new object[]
		{
			new Vector3(0f, 0f, 5000f),
			false
		});
		this.view.RPC("StopClimbingRpc", RpcTarget.All, new object[] { 0f });
		this.discovered = null;
	}

	// Token: 0x06000EB5 RID: 3765 RVA: 0x00049EC4 File Offset: 0x000480C4
	private Character GetPlayerWhoSeesMe()
	{
		Vector3 vector = this.character.Center + Vector3.up * Random.Range(-0.5f, 0.5f);
		foreach (Character character in Character.AllCharacters)
		{
			if (!character.isBot && Vector3.Angle(vector - character.Head, character.data.lookDirection) <= 80f && HelperFunctions.LineCheck(character.Head, vector, HelperFunctions.LayerType.TerrainMap, 0f, QueryTriggerInteraction.Ignore).transform == null)
			{
				return character;
			}
		}
		return null;
	}

	// Token: 0x06000EB6 RID: 3766 RVA: 0x00049F90 File Offset: 0x00048190
	private void Flee()
	{
		Vector3 normalized = (this.character.Center - this.discovered.Center).normalized;
		Vector3 vector = this.character.Center + normalized * 10f;
		if (this.character.data.isClimbing)
		{
			this.ClimbTowards(vector, 1f);
			return;
		}
		this.WalkTowards(vector, 1f);
		this.character.input.sprintIsPressed = true;
	}

	// Token: 0x06000EB7 RID: 3767 RVA: 0x0004A01C File Offset: 0x0004821C
	private bool AnyoneCanSeeMe()
	{
		Vector3 vector = this.character.Head + Vector3.up * 0.3f + Random.insideUnitSphere * 0.5f;
		Vector3 vector2 = this.character.HipPos() - Vector3.up * 0.3f + Random.insideUnitSphere * 0.5f;
		return this.AnyoneCanSeePos(vector) || this.AnyoneCanSeePos(vector2);
	}

	// Token: 0x06000EB8 RID: 3768 RVA: 0x0004A0A8 File Offset: 0x000482A8
	private bool AnyoneCanSeePos(Vector3 pos)
	{
		foreach (Character character in Character.AllCharacters)
		{
			if (!character.isBot && Vector3.Angle(pos - character.Head, character.data.lookDirection) <= 80f)
			{
				if (HelperFunctions.LineCheck(character.Head, pos, HelperFunctions.LayerType.TerrainMap, 0f, QueryTriggerInteraction.Ignore).transform == null)
				{
					Debug.DrawLine(character.Head, pos, Color.blue);
					return true;
				}
				Debug.DrawLine(character.Head, pos, Color.red);
			}
		}
		return false;
	}

	// Token: 0x06000EB9 RID: 3769 RVA: 0x0004A170 File Offset: 0x00048370
	private void TeleportCloseToTarget()
	{
		this.Teleport(this.currentTarget, 50f, 70f, 15f);
	}

	// Token: 0x06000EBA RID: 3770 RVA: 0x0004A190 File Offset: 0x00048390
	private void Teleport(Character target, float minDistanceToTarget = 35f, float maxDistanceToTarget = 45f, float maxHeightDifference = 15f)
	{
		if (this.tpCounter < 5f)
		{
			return;
		}
		this.tpCounter = 0f;
		Debug.Log("Trying to Teleport");
		if (target == null)
		{
			target = this.GetHighestCharacter(null);
		}
		Vector3 center = this.character.Center;
		int i = 50;
		while (i > 0)
		{
			i--;
			Vector3 onUnitSphere = Random.onUnitSphere;
			Vector3 vector = target.Center + Vector3.up * 500f + onUnitSphere * 95f;
			Vector3 vector2 = Vector3.down;
			if (i < 25)
			{
				vector = target.Center + Vector3.up;
				vector2 = Random.onUnitSphere;
			}
			RaycastHit raycastHit = HelperFunctions.LineCheck(vector, vector + vector2 * 1000f, HelperFunctions.LayerType.TerrainMap, 0f, QueryTriggerInteraction.Ignore);
			if (raycastHit.transform)
			{
				float num = Vector3.Distance(raycastHit.point, target.Center);
				float num2 = Mathf.Abs(raycastHit.point.y - target.Center.y);
				if (num < maxDistanceToTarget && num2 < maxHeightDifference && num > minDistanceToTarget && !this.AnyoneCanSeePos(raycastHit.point + Vector3.up))
				{
					Debug.Log("Teleporting");
					this.view.RPC("WarpPlayerRPC", RpcTarget.All, new object[]
					{
						raycastHit.point + Vector3.up,
						false
					});
					this.view.RPC("StopClimbingRpc", RpcTarget.All, new object[] { 0f });
					this.discovered = null;
					return;
				}
			}
		}
	}

	// Token: 0x06000EBB RID: 3771 RVA: 0x0004A34C File Offset: 0x0004854C
	private void Chase()
	{
		if (this.sinceSeenTarget > 30f && !this.AnyoneCanSeeMe())
		{
			this.sinceSeenTarget = 0f;
			this.TeleportCloseToTarget();
			if (Random.value < 0.1f)
			{
				this.currentTarget = null;
			}
			return;
		}
		if (this.character.data.isClimbing)
		{
			this.ClimbTowards(this.currentTarget.Head, 1f);
			if (this.currentTarget.Center.y < this.character.Center.y && !HelperFunctions.LineCheck(this.character.Center, this.currentTarget.Center, HelperFunctions.LayerType.TerrainMap, 0f, QueryTriggerInteraction.Ignore).transform)
			{
				this.character.refs.climbing.StopClimbing();
				return;
			}
		}
		else
		{
			if (this.character.data.grabbedPlayer)
			{
				this.HoldPlayer();
				return;
			}
			this.LookAt(this.currentTarget.Head);
			float num = Vector3.Distance(this.character.Center, this.currentTarget.Center);
			if (num > 5f || this.targetHasSeenMeCounter > 1f)
			{
				this.WalkTowards(this.currentTarget.Head, 1f);
			}
			if (this.targetHasSeenMeCounter > 1f)
			{
				this.character.input.sprintIsPressed = num < 15f;
				if (Vector3.Distance(this.character.Center, this.currentTarget.Center) < 3f && this.character.data.sinceClimb > 1f && this.character.data.isGrounded)
				{
					this.character.input.useSecondaryIsPressed = true;
				}
			}
		}
	}

	// Token: 0x06000EBC RID: 3772 RVA: 0x0004A524 File Offset: 0x00048724
	private void StandStill()
	{
	}

	// Token: 0x06000EBD RID: 3773 RVA: 0x0004A526 File Offset: 0x00048726
	private void ResetInput()
	{
		this.character.input.ResetInput();
	}

	// Token: 0x06000EBE RID: 3774 RVA: 0x0004A538 File Offset: 0x00048738
	private void HoldPlayer()
	{
		this.currentTarget.data.sinceGrounded = 0f;
		this.character.input.useSecondaryIsPressed = true;
		Vector3 lookDirection = this.character.data.lookDirection;
		lookDirection.y = 0.6f;
		lookDirection.Normalize();
		this.character.data.lookValues = HelperFunctions.DirectionToLook(lookDirection);
		if (!this.isThrowing)
		{
			this.view.RPC("RPCA_Throw", RpcTarget.All, Array.Empty<object>());
		}
	}

	// Token: 0x06000EBF RID: 3775 RVA: 0x0004A5C8 File Offset: 0x000487C8
	[PunRPC]
	public void RPCA_Throw()
	{
		base.StartCoroutine(this.IThrow());
	}

	// Token: 0x06000EC0 RID: 3776 RVA: 0x0004A5D7 File Offset: 0x000487D7
	private IEnumerator IThrow()
	{
		this.isThrowing = true;
		if (this.view.IsMine)
		{
			this.RotateToMostEvilThrowDirection();
		}
		if (this.currentTarget.IsLocal)
		{
			GamefeelHandler.instance.AddPerlinShake(15f, 0.5f, 15f);
		}
		GamefeelHandler.instance.AddPerlinShake(3f, 3f, 15f);
		float c = 0f;
		while (c < 3.2f)
		{
			this.currentTarget.data.lookValues = HelperFunctions.DirectionToLook(this.character.Head - this.currentTarget.Head);
			c += Time.deltaTime;
			yield return null;
		}
		Vector3 vector = -this.character.data.lookDirection;
		vector.y = 0f;
		vector.Normalize();
		vector.y = 0.3f;
		this.character.refs.grabbing.Throw(vector * 1500f, 3f);
		this.currentTarget.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Injury, 0.25f, true);
		this.isThrowing = false;
		this.chillForSeconds = 2f;
		yield break;
	}

	// Token: 0x06000EC1 RID: 3777 RVA: 0x0004A5E8 File Offset: 0x000487E8
	private void RotateToMostEvilThrowDirection()
	{
		Vector3[] circularDirections = HelperFunctions.GetCircularDirections(10);
		float num = 10f;
		float num2 = 1000f;
		Vector3 center = this.character.Center;
		Vector3 vector = this.character.data.lookDirection_Flat;
		float num3 = 0f;
		foreach (Vector3 vector2 in circularDirections)
		{
			Vector3 vector3 = center + vector2 * num;
			if (!HelperFunctions.LineCheck(center, vector3, HelperFunctions.LayerType.TerrainMap, 0f, QueryTriggerInteraction.Ignore).transform)
			{
				RaycastHit raycastHit = HelperFunctions.LineCheck(vector3, center + vector3 + Vector3.down * num2, HelperFunctions.LayerType.TerrainMap, 0f, QueryTriggerInteraction.Ignore);
				if (raycastHit.transform && raycastHit.distance > num3)
				{
					vector = vector2;
					num3 = raycastHit.distance;
				}
			}
		}
		this.character.data.lookValues = HelperFunctions.DirectionToLook(-vector);
	}

	// Token: 0x06000EC2 RID: 3778 RVA: 0x0004A6F0 File Offset: 0x000488F0
	private void ClimbTowards(Vector3 targetPos, float mult)
	{
		this.LookAt(targetPos);
		float num = Mathf.Clamp(this.character.GetBodypart(BodypartType.Torso).transform.InverseTransformPoint(targetPos).x * 0.25f, -1f, 1f);
		this.character.input.movementInput = new Vector2(num, mult);
		this.character.data.currentStamina = 1f;
	}

	// Token: 0x06000EC3 RID: 3779 RVA: 0x0004A764 File Offset: 0x00048964
	private void WalkTowards(Vector3 targetPos, float mult)
	{
		this.LookAt(targetPos);
		float num = HelperFunctions.FlatDistance(this.character.Center, targetPos);
		if (Vector3.Distance(this.character.Center, targetPos) < 5f)
		{
			if (num < 2.5f)
			{
				mult *= 0f;
			}
			else if (num < 1.5f)
			{
				mult *= -1f;
			}
		}
		this.character.input.movementInput = new Vector2(0f, mult);
		this.character.refs.climbing.TryClimb();
		if (HelperFunctions.LineCheck(this.character.Center, this.character.Center + Vector3.down * 3f, HelperFunctions.LayerType.TerrainMap, 0f, QueryTriggerInteraction.Ignore).transform == null)
		{
			this.character.input.jumpWasPressed = true;
		}
	}

	// Token: 0x06000EC4 RID: 3780 RVA: 0x0004A84D File Offset: 0x00048A4D
	private void LookAt(Vector3 lookAtPos)
	{
		this.character.data.lookValues = HelperFunctions.DirectionToLook(lookAtPos - this.character.Head);
	}

	// Token: 0x06000EC5 RID: 3781 RVA: 0x0004A87C File Offset: 0x00048A7C
	private int ViableTargets()
	{
		List<Character> allCharacters = Character.AllCharacters;
		int num = 0;
		foreach (Character character in allCharacters)
		{
			if (!character.isBot && !character.data.dead && !character.data.fullyPassedOut)
			{
				num++;
			}
		}
		return num;
	}

	// Token: 0x06000EC6 RID: 3782 RVA: 0x0004A8F0 File Offset: 0x00048AF0
	private void LookForTarget()
	{
		if (this.ViableTargets() < 2)
		{
			return;
		}
		if (this.sinceLookForTarget < 30f)
		{
			return;
		}
		this.sinceLookForTarget = 0f;
		if (Random.value > 0.1f)
		{
			return;
		}
		Character highestCharacter = this.GetHighestCharacter(null);
		Character highestCharacter2 = this.GetHighestCharacter(highestCharacter);
		if (highestCharacter.Center.y > highestCharacter2.Center.y + this.attackHeightDelta && highestCharacter.Center.y < this.maxAggroHeight)
		{
			this.SetCurrentTarget(highestCharacter, 0f);
		}
	}

	// Token: 0x06000EC7 RID: 3783 RVA: 0x0004A97C File Offset: 0x00048B7C
	private Character GetHighestCharacter(Character ignoredCharacter)
	{
		List<Character> allCharacters = Character.AllCharacters;
		Character character = null;
		foreach (Character character2 in allCharacters)
		{
			if (!character2.isBot && !character2.data.dead && !character2.data.fullyPassedOut && !(character2 == ignoredCharacter) && (character == null || character2.Center.y > character.Center.y))
			{
				character = character2;
			}
		}
		return character;
	}

	// Token: 0x04000D97 RID: 3479
	public float reachForce;

	// Token: 0x04000D98 RID: 3480
	private float targetForcedUntil;

	// Token: 0x04000D99 RID: 3481
	private Character _currentTarget;

	// Token: 0x04000D9A RID: 3482
	internal static List<Scoutmaster> AllScoutmasters = new List<Scoutmaster>();

	// Token: 0x04000D9B RID: 3483
	public Character discovered;

	// Token: 0x04000D9C RID: 3484
	private ScoutmasterAnimVars animVars;

	// Token: 0x04000D9D RID: 3485
	public float achievementDistance;

	// Token: 0x04000D9E RID: 3486
	private Character character;

	// Token: 0x04000D9F RID: 3487
	private PhotonView view;

	// Token: 0x04000DA0 RID: 3488
	public Material mat;

	// Token: 0x04000DA1 RID: 3489
	private float sinceLookForTarget;

	// Token: 0x04000DA2 RID: 3490
	private float distanceToTarget;

	// Token: 0x04000DA3 RID: 3491
	private float sinceAnyoneCanSeeMe = 10f;

	// Token: 0x04000DA4 RID: 3492
	private float achievementTestTick;

	// Token: 0x04000DA5 RID: 3493
	private float attackHeightDelta = 100f;

	// Token: 0x04000DA6 RID: 3494
	private float tpCounter;

	// Token: 0x04000DA7 RID: 3495
	public float targetHasSeenMeCounter;

	// Token: 0x04000DA8 RID: 3496
	private float sinceSeenTarget;

	// Token: 0x04000DA9 RID: 3497
	private bool isThrowing;

	// Token: 0x04000DAA RID: 3498
	private float chillForSeconds;

	// Token: 0x04000DAB RID: 3499
	private float maxAggroHeight = 825f;
}
