using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Photon.Pun;
using Photon.Voice.Unity;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using Zorro.Core;
using Zorro.Core.CLI;

// Token: 0x02000007 RID: 7
[DefaultExecutionOrder(-100)]
public class Character : MonoBehaviourPun
{
	// Token: 0x17000003 RID: 3
	// (get) Token: 0x06000038 RID: 56 RVA: 0x00002D17 File Offset: 0x00000F17
	public Player player
	{
		get
		{
			return PlayerHandler.GetPlayer(this.view.Owner);
		}
	}

	// Token: 0x17000004 RID: 4
	// (get) Token: 0x06000039 RID: 57 RVA: 0x00002D2C File Offset: 0x00000F2C
	public static Character observedCharacter
	{
		get
		{
			Character specCharacter = MainCameraMovement.specCharacter;
			if (specCharacter)
			{
				return specCharacter;
			}
			return Character.localCharacter;
		}
	}

	// Token: 0x17000005 RID: 5
	// (get) Token: 0x0600003A RID: 58 RVA: 0x00002D4E File Offset: 0x00000F4E
	// (set) Token: 0x0600003B RID: 59 RVA: 0x00002D56 File Offset: 0x00000F56
	public PlayerGhost Ghost { get; set; }

	// Token: 0x17000006 RID: 6
	// (get) Token: 0x0600003C RID: 60 RVA: 0x00002D5F File Offset: 0x00000F5F
	public bool IsLocal
	{
		get
		{
			return this == Character.localCharacter;
		}
	}

	// Token: 0x17000007 RID: 7
	// (get) Token: 0x0600003D RID: 61 RVA: 0x00002D6C File Offset: 0x00000F6C
	public Vector3 Center
	{
		get
		{
			return this.GetBodypart(BodypartType.Torso).transform.position;
		}
	}

	// Token: 0x17000008 RID: 8
	// (get) Token: 0x0600003E RID: 62 RVA: 0x00002D7F File Offset: 0x00000F7F
	public Vector3 Head
	{
		get
		{
			return this.GetBodypart(BodypartType.Head).transform.position;
		}
	}

	// Token: 0x17000009 RID: 9
	// (get) Token: 0x0600003F RID: 63 RVA: 0x00002D92 File Offset: 0x00000F92
	public string characterName
	{
		get
		{
			if (!this.isBot)
			{
				return this.view.Owner.NickName;
			}
			return "Bot";
		}
	}

	// Token: 0x06000040 RID: 64 RVA: 0x00002DB4 File Offset: 0x00000FB4
	public static bool GetCharacterWithPhotonID(int photonID, out Character characterResult)
	{
		for (int i = 0; i < Character.AllCharacters.Count; i++)
		{
			if (Character.AllCharacters[i] != null && Character.AllCharacters[i].photonView.ViewID == photonID)
			{
				characterResult = Character.AllCharacters[i];
				return true;
			}
		}
		characterResult = null;
		return false;
	}

	// Token: 0x06000041 RID: 65 RVA: 0x00002E14 File Offset: 0x00001014
	private void OnDestroy()
	{
		Character.AllCharacters.Remove(this);
	}

	// Token: 0x06000042 RID: 66 RVA: 0x00002E24 File Offset: 0x00001024
	private void Awake()
	{
		if (!this.isBot)
		{
			Character.AllCharacters.Add(this);
		}
		this.view = base.GetComponent<PhotonView>();
		if (this.view != null)
		{
			if (!this.isBot)
			{
				PlayerHandler.RegisterCharacter(this);
				base.gameObject.name = string.Format("Character [{0} : {1}]", this.view.Owner.NickName, this.view.Owner.ActorNumber);
				if (this.view.IsMine)
				{
					Character.localCharacter = this;
					VoiceClientHandler.LocalPlayerAssigned(base.GetComponentInChildren<Recorder>());
				}
			}
			else
			{
				base.gameObject.name = "Bot";
			}
		}
		this.refs.animatedVariables = base.GetComponentInChildren<AnimatedVariables>();
		this.refs.movement = base.GetComponent<CharacterMovement>();
		this.refs.carriying = base.GetComponent<CharacterCarrying>();
		this.refs.ragdoll = base.GetComponent<CharacterRagdoll>();
		this.refs.ropeHandling = base.GetComponent<CharacterRopeHandling>();
		this.refs.rigCreator = base.GetComponentInChildren<RigCreator>();
		this.refs.animations = base.GetComponentInChildren<CharacterAnimations>();
		this.refs.animator = this.refs.rigCreator.GetComponent<Animator>();
		this.refs.items = base.GetComponent<CharacterItems>();
		this.refs.climbing = base.GetComponent<CharacterClimbing>();
		this.refs.afflictions = base.GetComponent<CharacterAfflictions>();
		this.refs.view = base.GetComponent<PhotonView>();
		this.refs.heatEmission = base.GetComponentInChildren<CharacterHeatEmission>();
		this.refs.vineClimbing = base.GetComponentInChildren<CharacterVineClimbing>();
		this.refs.customization = base.GetComponentInChildren<CharacterCustomization>();
		this.refs.stats = base.GetComponentInChildren<CharacterStats>();
		this.refs.grabbing = base.GetComponent<CharacterGrabbing>();
		this.refs.hideTheBody = base.GetComponentInChildren<HideTheBody>();
		this.refs.badgeUnlocker = base.GetComponent<BadgeUnlocker>();
		this.refs.ikRigBuilder = this.refs.rigCreator.GetComponent<RigBuilder>();
		if (this.refs.ikRigBuilder)
		{
			this.refs.ikRig = this.refs.rigCreator.GetComponentInChildren<Rig>();
			this.refs.IKHandTargetLeft = this.refs.ikRig.transform.Find("IK_Arm_Left/Target");
			this.refs.IKHandTargetRight = this.refs.ikRig.transform.Find("IK_Arm_Right/Target");
			if (this.refs.IKHandTargetLeft)
			{
				this.refs.ikLeft = this.refs.IKHandTargetLeft.transform.parent.GetComponent<TwoBoneIKConstraint>();
				this.refs.ikRight = this.refs.IKHandTargetRight.transform.parent.GetComponent<TwoBoneIKConstraint>();
			}
		}
		this.CreateHelperObjects();
		this.input.Init();
	}

	// Token: 0x06000043 RID: 67 RVA: 0x00003125 File Offset: 0x00001325
	[ConsoleCommand]
	public static void GainFullStamina()
	{
		Character.localCharacter.AddStamina(1f);
	}

	// Token: 0x06000044 RID: 68 RVA: 0x00003138 File Offset: 0x00001338
	private void CreateHelperObjects()
	{
		this.refs.helperObjects = new GameObject("helperObjects").transform;
		this.refs.helperObjects.transform.SetParent(base.transform);
		this.refs.helperObjects.transform.localPosition = Vector3.zero;
		this.refs.helperObjects.transform.localRotation = Quaternion.identity;
		this.refs.animationHeadTransform = Object.Instantiate<GameObject>(this.refs.helperObjects.gameObject, this.refs.helperObjects).transform;
		this.refs.animationHeadTransform.gameObject.name = "animationHead";
		this.refs.animationHipTransform = Object.Instantiate<GameObject>(this.refs.animationHeadTransform.gameObject, this.refs.helperObjects).transform;
		this.refs.animationHipTransform.gameObject.name = "animationHip";
		this.refs.animationItemTransform = Object.Instantiate<GameObject>(this.refs.animationHeadTransform.gameObject, this.refs.helperObjects).transform;
		this.refs.animationItemTransform.gameObject.name = "animationItem";
		this.refs.animationLookTransform = Object.Instantiate<GameObject>(this.refs.animationHeadTransform.gameObject, this.refs.helperObjects).transform;
		this.refs.animationLookTransform.gameObject.name = "animationLook";
		this.refs.animationPositionTransform = Object.Instantiate<GameObject>(this.refs.animationHeadTransform.gameObject, this.refs.helperObjects).transform;
		this.refs.animationPositionTransform.gameObject.name = "animationPosition";
	}

	// Token: 0x06000045 RID: 69 RVA: 0x00003320 File Offset: 0x00001520
	private void Start()
	{
		this.refs.hip = this.GetBodypart(BodypartType.Hip);
		this.refs.head = this.GetBodypart(BodypartType.Head);
		base.gameObject.name = string.Format("Character [{0} : {1}]", this.view.Owner.NickName, this.view.Owner.ActorNumber);
		CharacterAfflictions afflictions = this.refs.afflictions;
		afflictions.OnAddedIncrementalStatus = (Action<CharacterAfflictions.STATUSTYPE, float>)Delegate.Combine(afflictions.OnAddedIncrementalStatus, new Action<CharacterAfflictions.STATUSTYPE, float>(this.OnAddedStatus));
		this.smoothedCamPos = this.GetBodypart(BodypartType.Head).transform.TransformPoint(Vector3.up * 1f);
	}

	// Token: 0x06000046 RID: 70 RVA: 0x000033DD File Offset: 0x000015DD
	private void OnAddedStatus(CharacterAfflictions.STATUSTYPE sTATUSTYPE, float amount)
	{
		if (sTATUSTYPE == CharacterAfflictions.STATUSTYPE.Cold && amount > 0f)
		{
			this.data.sinceAddedCold = 0f;
		}
	}

	// Token: 0x06000047 RID: 71 RVA: 0x000033FC File Offset: 0x000015FC
	private void Update()
	{
		if (!this.data.dead)
		{
			this.data.sinceDied = 0f;
		}
		if (!this.IsLocal)
		{
			return;
		}
		if (this.data.dead)
		{
			this.HandleDeath();
			return;
		}
		if (this.data.passedOut)
		{
			this.HandlePassedOut();
			return;
		}
		this.HandleLife();
	}

	// Token: 0x06000048 RID: 72 RVA: 0x0000345D File Offset: 0x0000165D
	private Vector3 DeathPos()
	{
		return new Vector3(0f, 5000f, -5000f);
	}

	// Token: 0x06000049 RID: 73 RVA: 0x00003473 File Offset: 0x00001673
	private void HandleDeath()
	{
		this.data.sinceDied += Time.deltaTime;
	}

	// Token: 0x0600004A RID: 74 RVA: 0x0000348C File Offset: 0x0000168C
	private void HandlePassedOut()
	{
		if (this.refs.afflictions.statusSum < 1f && !this.UnPassOutCalled)
		{
			this.view.RPC("RPCA_UnPassOut", RpcTarget.All, Array.Empty<object>());
		}
		if (this.data.deathTimer > 1f)
		{
			this.refs.items.EquipSlot(Optionable<byte>.None);
			Debug.Log("DYING");
			this.view.RPC("RPCA_Die", RpcTarget.All, new object[] { this.Center + Vector3.up * 0.2f + Vector3.forward * 0.1f });
		}
	}

	// Token: 0x0600004B RID: 75 RVA: 0x0000354C File Offset: 0x0000174C
	[ConsoleCommand]
	public static void Die()
	{
		Character.localCharacter.refs.items.EquipSlot(Optionable<byte>.None);
		Debug.Log("DYING");
		Character.localCharacter.view.RPC("RPCA_Die", RpcTarget.All, new object[] { Character.localCharacter.Center + Vector3.up * 0.2f + Vector3.forward * 0.1f });
	}

	// Token: 0x0600004C RID: 76 RVA: 0x000035D1 File Offset: 0x000017D1
	internal void DieInstantly()
	{
		Character.localCharacter.view.RPC("RPCA_Die", RpcTarget.All, new object[] { Character.localCharacter.Center });
	}

	// Token: 0x0600004D RID: 77 RVA: 0x00003600 File Offset: 0x00001800
	[PunRPC]
	public void RPCA_Die(Vector3 itemSpawnPoint)
	{
		this.refs.items.EquipSlot(Optionable<byte>.None);
		this.data.dead = true;
		this.data.fullyPassedOut = true;
		this.data.deathTimer = 1f;
		this.data.passedOut = true;
		this.refs.stats.justDied = true;
		this.refs.stats.Record(false, 0f);
		RoomProperties.me.SaveReconnectData();
		ItemSlot[] itemSlots = this.player.itemSlots;
		this.refs.items.DropAllItems(true);
		Debug.Log(base.gameObject.name + " died");
		((GameObject)Object.Instantiate(Resources.Load("Skeleton"))).GetComponent<Skelleton>().SpawnSkelly(this);
		this.WarpPlayer(this.DeathPos(), false);
		this.CheckEndGame();
		Debug.Log("DIE");
	}

	// Token: 0x0600004E RID: 78 RVA: 0x000036FC File Offset: 0x000018FC
	public void CheckEndGame()
	{
		if (PhotonNetwork.IsMasterClient)
		{
			bool flag = true;
			for (int i = 0; i < Character.AllCharacters.Count; i++)
			{
				if (!Character.AllCharacters[i].data.dead)
				{
					flag = false;
				}
			}
			if (flag)
			{
				this.EndGame();
			}
		}
	}

	// Token: 0x0600004F RID: 79 RVA: 0x00003749 File Offset: 0x00001949
	[ConsoleCommand]
	internal static void TestWin()
	{
		Character.localCharacter.photonView.RPC("RPCEndGame_ForceWin", RpcTarget.All, Array.Empty<object>());
	}

	// Token: 0x06000050 RID: 80 RVA: 0x00003765 File Offset: 0x00001965
	internal void EndGame()
	{
		base.photonView.RPC("RPCEndGame", RpcTarget.All, Array.Empty<object>());
	}

	// Token: 0x06000051 RID: 81 RVA: 0x0000377D File Offset: 0x0000197D
	[PunRPC]
	private void RPCEndGame_ForceWin()
	{
		Character.forceWin = true;
		this.RPCEndGame();
		Character.forceWin = false;
	}

	// Token: 0x06000052 RID: 82 RVA: 0x00003794 File Offset: 0x00001994
	[PunRPC]
	private void RPCEndGame()
	{
		bool flag = false;
		foreach (Character character in Character.AllCharacters)
		{
			if (Character.CheckWinCondition(character))
			{
				character.refs.stats.Win();
				flag = true;
			}
		}
		foreach (Character character2 in Character.AllCharacters)
		{
			if (!Character.CheckWinCondition(character2))
			{
				character2.refs.stats.Lose(flag);
			}
		}
		MenuWindow.CloseAllWindows();
		if (flag)
		{
			GlobalEvents.TriggerSomeoneWonRun();
			Singleton<PeakHandler>.Instance.EndCutscene();
		}
		else
		{
			GUIManager.instance.endScreen.Open();
		}
		GlobalEvents.TriggerRunEnded();
	}

	// Token: 0x06000053 RID: 83 RVA: 0x0000387C File Offset: 0x00001A7C
	public static bool CheckWinCondition(Character c)
	{
		return Character.forceWin || (c.data.isRopeClimbing && c.data.heldRope.isHelicopterRope) || Singleton<MountainProgressHandler>.Instance.IsAtPeak(c.Center);
	}

	// Token: 0x06000054 RID: 84 RVA: 0x000038B8 File Offset: 0x00001AB8
	[PunRPC]
	private void RPCA_UnPassOut()
	{
		this.UnPassOutCalled = true;
		this.data.deathTimer = 0f;
		if (this.IsLocal)
		{
			Transitions.instance.PlayTransition(TransitionType.FadeToBlack, new Action(this.UnPassOutDone), 1f, 1f);
			return;
		}
		this.UnPassOutDone();
	}

	// Token: 0x06000055 RID: 85 RVA: 0x0000390C File Offset: 0x00001B0C
	private void UnPassOutDone()
	{
		Debug.Log("UhPassOut");
		Action unPassOutAction = this.UnPassOutAction;
		if (unPassOutAction != null)
		{
			unPassOutAction();
		}
		this.data.fullyPassedOut = false;
		this.data.passedOut = false;
	}

	// Token: 0x06000056 RID: 86 RVA: 0x00003941 File Offset: 0x00001B41
	[ConsoleCommand]
	public static void PassOut()
	{
		CharacterAfflictions.Starve();
		Character.localCharacter.view.RPC("RPCA_PassOut", RpcTarget.All, Array.Empty<object>());
	}

	// Token: 0x06000057 RID: 87 RVA: 0x00003964 File Offset: 0x00001B64
	[PunRPC]
	public void RPCA_PassOut()
	{
		this.UnPassOutCalled = false;
		this.data.passedOut = true;
		if (RoomProperties.me != null)
		{
			RoomProperties.me.SaveReconnectData();
		}
		this.refs.stats.justPassedOut = true;
		this.refs.stats.Record(false, 0f);
		GlobalEvents.OnCharacterPassedOut(this);
		if (this.IsLocal)
		{
			RoomProperties.me.SaveReconnectData();
			this.refs.items.DropAllItems(false);
			Transitions.instance.PlayTransition(TransitionType.FadeToBlack, new Action(this.<RPCA_PassOut>g__PassOutDone|53_0), 1f, 1f);
		}
		else
		{
			this.<RPCA_PassOut>g__PassOutDone|53_0();
		}
		Debug.Log("PASS OUT");
	}

	// Token: 0x06000058 RID: 88 RVA: 0x00003A24 File Offset: 0x00001C24
	private void HandleLife()
	{
		if (this.refs.afflictions.statusSum >= 1f)
		{
			this.data.passOutValue = Mathf.MoveTowards(this.data.passOutValue, 1f, Time.deltaTime / 5f);
			if (this.data.passOutValue > 0.999f)
			{
				this.view.RPC("RPCA_PassOut", RpcTarget.All, Array.Empty<object>());
				return;
			}
		}
		else
		{
			this.data.passOutValue = Mathf.MoveTowards(this.data.passOutValue, 0f, Time.deltaTime / 5f);
		}
	}

	// Token: 0x06000059 RID: 89 RVA: 0x00003AC7 File Offset: 0x00001CC7
	public void PassOutInstantly()
	{
		this.data.passOutValue = 1f;
		this.view.RPC("RPCA_PassOut", RpcTarget.All, Array.Empty<object>());
	}

	// Token: 0x0600005A RID: 90 RVA: 0x00003AF0 File Offset: 0x00001CF0
	private void FixedUpdate()
	{
		this.UpdateVariablesFixed();
		if (this.data.dead)
		{
			this.refs.ragdoll.MoveAllRigsInDirection(this.DeathPos() - this.Center);
			this.refs.ragdoll.HaltBodyVelocity();
		}
	}

	// Token: 0x0600005B RID: 91 RVA: 0x00003B44 File Offset: 0x00001D44
	private void UpdateVariablesFixed()
	{
		float targetRagdollControll = this.data.GetTargetRagdollControll();
		if (targetRagdollControll < this.data.currentRagdollControll)
		{
			this.data.currentRagdollControll = targetRagdollControll;
		}
		else if (this.data.currentRagdollControll > 0.5f)
		{
			this.data.currentRagdollControll = Mathf.MoveTowards(this.data.currentRagdollControll, targetRagdollControll, Time.fixedDeltaTime * 1f);
		}
		else
		{
			this.data.currentRagdollControll = Mathf.MoveTowards(this.data.currentRagdollControll, targetRagdollControll, Time.fixedDeltaTime * 0.5f);
		}
		this.data.staminaDelta = this.data.currentStamina + this.data.extraStamina - this.data.lastFrameTotalStamina;
		this.data.lastFrameTotalStamina = this.data.currentStamina + this.data.extraStamina;
		if (this.data.isGrounded)
		{
			this.data.groundedFor += Time.fixedDeltaTime;
			this.data.sinceGrounded = 0f;
			this.data.lastGroundedHeight = this.Center.y;
		}
		else
		{
			this.data.groundedFor = 0f;
			this.data.sinceGrounded += Time.fixedDeltaTime;
		}
		if (this.data.isClimbing || this.data.isRopeClimbing || this.data.isVineClimbing)
		{
			this.data.sinceClimb = 0f;
		}
		if (this.data.dead)
		{
			this.data.sinceDead = 0f;
		}
		if (this.OutOfStamina())
		{
			this.data.outOfStaminaFor += Time.fixedDeltaTime;
		}
		else
		{
			this.data.outOfStaminaFor = 0f;
		}
		this.data.staminaMod = Mathf.Max(Mathf.Clamp01(this.GetTotalStamina() * 5f), 0.2f);
		this.data.sinceClimbJump += Time.fixedDeltaTime;
		if (this.data.isGrounded)
		{
			this.data.fallSeconds -= Time.fixedDeltaTime;
		}
		else
		{
			this.data.fallSeconds -= Time.fixedDeltaTime * 0.2f;
		}
		if (this.data.fullyPassedOut)
		{
			if (this.input.interactIsPressed)
			{
				this.data.deathTimer += Time.fixedDeltaTime * 0.33f;
			}
			else if (!this.data.carrier)
			{
				if (!this.HasMeaningfulTempStatuses() && this.NobodyIsAlive())
				{
					this.data.deathTimer += Time.fixedDeltaTime / 10f;
				}
				else
				{
					this.data.deathTimer += Time.fixedDeltaTime / 60f;
				}
			}
		}
		else
		{
			this.data.sinceDied = 0f;
		}
		if (this.input.usePrimaryIsPressed && this.data.currentItem == null)
		{
			this.data.sincePressClimb = 0f;
		}
		if (this.input.useSecondaryIsPressed && this.data.currentItem == null)
		{
			this.data.sincePressReach = 0f;
		}
		this.data.sincePressClimb += Time.fixedDeltaTime;
		this.data.sincePressReach += Time.fixedDeltaTime;
		this.data.sinceAddedCold += Time.fixedDeltaTime;
		this.data.sinceStartClimb += Time.fixedDeltaTime;
		this.data.sinceGrabFriend += Time.fixedDeltaTime;
		this.data.sinceClimbHandle += Time.fixedDeltaTime;
		this.data.sinceFallSlide += Time.fixedDeltaTime;
		this.data.sinceUseStamina += Time.fixedDeltaTime;
		this.data.sinceClimb += Time.fixedDeltaTime;
		this.data.sinceJump += Time.fixedDeltaTime;
		this.data.sinceDead += Time.fixedDeltaTime;
		this.data.overrideIKForSeconds -= Time.fixedDeltaTime;
		this.data.slippy -= Time.deltaTime;
		this.data.sinceLetGoOfFriend += Time.fixedDeltaTime;
		this.data.sinceStandOnPlayer += Time.fixedDeltaTime;
		this.data.sincePalJump += Time.fixedDeltaTime;
		this.data.sinceItemAttach += Time.fixedDeltaTime;
		this.data.sinceCanClimb += Time.fixedDeltaTime;
		this.data.passedOutOnTheBeach -= Time.fixedDeltaTime;
		if (this.CanRegenStamina())
		{
			this.AddStamina(Time.fixedDeltaTime * 0.2f);
		}
	}

	// Token: 0x0600005C RID: 92 RVA: 0x00004080 File Offset: 0x00002280
	private bool NobodyIsAlive()
	{
		List<Character> allCharacters = Character.AllCharacters;
		for (int i = 0; i < allCharacters.Count; i++)
		{
			if (allCharacters[i].data.fullyConscious)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x0600005D RID: 93 RVA: 0x000040BC File Offset: 0x000022BC
	private bool HasMeaningfulTempStatuses()
	{
		float num = this.refs.afflictions.GetCurrentStatus(CharacterAfflictions.STATUSTYPE.Drowsy) + this.refs.afflictions.GetCurrentStatus(CharacterAfflictions.STATUSTYPE.Hot) + this.refs.afflictions.GetCurrentStatus(CharacterAfflictions.STATUSTYPE.Poison);
		if (!this.data.isInFog)
		{
			num += this.refs.afflictions.GetCurrentStatus(CharacterAfflictions.STATUSTYPE.Cold);
		}
		return this.refs.afflictions.statusSum - num < 1f;
	}

	// Token: 0x0600005E RID: 94 RVA: 0x0000413C File Offset: 0x0000233C
	private bool CanRegenStamina()
	{
		if (this.data.currentClimbHandle)
		{
			return true;
		}
		float num = ((this.data.currentStamina > 0f) ? 1f : 2f);
		return this.data.sinceGrounded <= 0.2f && this.data.sinceUseStamina >= num;
	}

	// Token: 0x0600005F RID: 95 RVA: 0x0000419F File Offset: 0x0000239F
	public float GetTotalStamina()
	{
		return this.data.currentStamina + this.data.extraStamina;
	}

	// Token: 0x06000060 RID: 96 RVA: 0x000041B8 File Offset: 0x000023B8
	internal Bodypart GetBodypart(BodypartType head)
	{
		return this.refs.ragdoll.partDict[head];
	}

	// Token: 0x06000061 RID: 97 RVA: 0x000041D0 File Offset: 0x000023D0
	internal Rigidbody GetBodypartRig(BodypartType head)
	{
		return this.refs.ragdoll.partDict[head].Rig;
	}

	// Token: 0x06000062 RID: 98 RVA: 0x000041F0 File Offset: 0x000023F0
	internal void CalculateWorldMovementDir()
	{
		Vector3 vector = default(Vector3) + this.data.lookDirection * this.input.movementInput.y;
		vector.y = 0f;
		vector = vector.normalized;
		vector += this.data.lookDirection_Right * this.input.movementInput.x;
		this.data.worldMovementInput = vector.normalized;
		Vector3 lookDirection = this.data.lookDirection;
		Vector3 lookDirection_Right = this.data.lookDirection_Right;
		lookDirection.y = 0f;
		lookDirection.Normalize();
		Vector3 vector2 = this.data.groundNormal;
		if (this.data.sinceGrounded > 0.2f)
		{
			vector2 = Vector3.up;
		}
		Vector3 vector3 = HelperFunctions.GroundDirection(vector2, -lookDirection_Right);
		Vector3 vector4 = HelperFunctions.GroundDirection(vector2, lookDirection);
		Vector3 vector5 = vector3 * this.input.movementInput.y + vector4 * this.input.movementInput.x;
		vector5 = Vector3.ClampMagnitude(vector5, 1f);
		this.data.worldMovementInput_Grounded = vector5;
		Vector3 vector6 = this.data.worldMovementInput_Grounded;
		float num = Mathf.Lerp(this.refs.movement.movementTurnSpeed, this.refs.movement.airMovementTurnSpeed, this.data.sinceGrounded * 4f);
		if (!this.data.isGrounded)
		{
			vector6 = this.data.worldMovementInput;
		}
		this.data.worldMovementInput_Lerp = Vector3.MoveTowards(this.data.worldMovementInput_Lerp, vector6, Time.deltaTime * num);
	}

	// Token: 0x06000063 RID: 99 RVA: 0x000043B0 File Offset: 0x000025B0
	internal void RecalculateLookDirections()
	{
		Vector3 normalized = HelperFunctions.LookToDirection(this.data.lookValues, Vector3.forward).normalized;
		this.data.lookDirection = normalized;
		normalized.y = 0f;
		normalized.Normalize();
		this.data.lookDirection_Flat = normalized;
		this.data.lookDirection_Right = Vector3.Cross(Vector3.up, this.data.lookDirection).normalized;
		this.data.lookDirection_Up = Vector3.Cross(this.data.lookDirection, this.data.lookDirection_Right).normalized;
	}

	// Token: 0x06000064 RID: 100 RVA: 0x00004461 File Offset: 0x00002661
	internal Vector3 GetCameraPos(float forwardOffset)
	{
		return this.GetBodypart(BodypartType.Head).transform.TransformPoint(Vector3.up * 1f + Vector3.forward * forwardOffset);
	}

	// Token: 0x06000065 RID: 101 RVA: 0x00004494 File Offset: 0x00002694
	internal Vector3 GetAnimationRelativePosition(Vector3 position)
	{
		Vector3 vector = position - this.refs.animationHipTransform.position;
		return this.refs.hip.Rig.position + vector;
	}

	// Token: 0x06000066 RID: 102 RVA: 0x000044D3 File Offset: 0x000026D3
	internal void OnLand(float sinceGrounded)
	{
		Action<float> action = this.landAction;
		if (action == null)
		{
			return;
		}
		action(sinceGrounded);
	}

	// Token: 0x06000067 RID: 103 RVA: 0x000044E6 File Offset: 0x000026E6
	internal void OnStartJump()
	{
		Action action = this.startJumpAction;
		if (action == null)
		{
			return;
		}
		action();
	}

	// Token: 0x06000068 RID: 104 RVA: 0x000044F8 File Offset: 0x000026F8
	internal void OnJump()
	{
		Action action = this.jumpAction;
		if (action == null)
		{
			return;
		}
		action();
	}

	// Token: 0x06000069 RID: 105 RVA: 0x0000450A File Offset: 0x0000270A
	internal void OnStartClimb()
	{
		Action action = this.startClimbAction;
		if (action == null)
		{
			return;
		}
		action();
	}

	// Token: 0x0600006A RID: 106 RVA: 0x0000451C File Offset: 0x0000271C
	internal Vector3 HipPos()
	{
		return this.GetBodypart(BodypartType.Hip).Rig.position;
	}

	// Token: 0x0600006B RID: 107 RVA: 0x0000452F File Offset: 0x0000272F
	internal Vector3 TorsoPos()
	{
		return this.GetBodypart(BodypartType.Torso).Rig.position;
	}

	// Token: 0x0600006C RID: 108 RVA: 0x00004544 File Offset: 0x00002744
	internal void AddForce(Vector3 move, float minRandomMultiplier = 1f, float maxRandomMultiplier = 1f)
	{
		foreach (Bodypart bodypart in this.refs.ragdoll.partList)
		{
			Vector3 vector = move;
			if (minRandomMultiplier != maxRandomMultiplier)
			{
				vector *= Random.Range(minRandomMultiplier, maxRandomMultiplier);
			}
			bodypart.AddForce(vector, ForceMode.Acceleration);
		}
	}

	// Token: 0x0600006D RID: 109 RVA: 0x000045B4 File Offset: 0x000027B4
	internal bool CheckStand()
	{
		return !this.data.isClimbing && !this.data.isRopeClimbing && !this.data.isVineClimbing;
	}

	// Token: 0x0600006E RID: 110 RVA: 0x000045E4 File Offset: 0x000027E4
	internal bool CheckGravity()
	{
		return !this.data.isClimbing && !this.data.isRopeClimbing && !this.data.isVineClimbing && !(this.data.currentClimbHandle != null);
	}

	// Token: 0x0600006F RID: 111 RVA: 0x00004634 File Offset: 0x00002834
	internal bool CheckMovement()
	{
		return !this.data.isClimbing && !this.data.isRopeClimbing && !this.data.isVineClimbing && !(this.data.currentClimbHandle != null);
	}

	// Token: 0x06000070 RID: 112 RVA: 0x00004684 File Offset: 0x00002884
	internal bool CheckJump()
	{
		return !this.data.fullyPassedOut && !this.data.isClimbing && !this.data.isRopeClimbing && !this.data.isVineClimbing && !(this.data.currentClimbHandle != null);
	}

	// Token: 0x06000071 RID: 113 RVA: 0x000046E4 File Offset: 0x000028E4
	internal bool CheckSprint()
	{
		return !this.data.isClimbing && !this.data.isRopeClimbing && !this.data.isVineClimbing && !(this.data.currentClimbHandle != null) && this.data.fullyConscious && (!this.data.currentItem || (!this.data.currentItem.isUsingPrimary && !this.data.currentItem.isUsingSecondary));
	}

	// Token: 0x06000072 RID: 114 RVA: 0x0000477C File Offset: 0x0000297C
	internal void SetRotation()
	{
		if (this.data.carrier)
		{
			this.refs.rigCreator.transform.rotation = this.data.carrier.refs.carryPosRef.rotation;
			return;
		}
		if (this.data.isRopeClimbing)
		{
			this.refs.rigCreator.transform.rotation = Quaternion.LookRotation(-this.data.ropeClimbWorldNormal, this.data.ropeClimbWorldUp);
			return;
		}
		if (this.data.isClimbing)
		{
			this.refs.rigCreator.transform.rotation = Quaternion.LookRotation(-this.data.climbNormal);
			return;
		}
		this.refs.rigCreator.transform.rotation = Quaternion.LookRotation(this.data.lookDirection_Flat);
	}

	// Token: 0x06000073 RID: 115 RVA: 0x0000486C File Offset: 0x00002A6C
	internal bool UseStamina(float usage, bool useBonusStamina = true)
	{
		if (usage == 0f)
		{
			return false;
		}
		usage *= Ascents.climbStaminaMultiplier;
		if (!this.view.IsMine)
		{
			return this.data.currentStamina + this.data.extraStamina > usage;
		}
		if (this.data.currentStamina == 0f)
		{
			if (this.data.extraStamina > 0f && useBonusStamina)
			{
				this.data.extraStamina -= usage;
				this.data.extraStamina = Mathf.Clamp(this.data.extraStamina, 0f, 1f);
				this.data.sinceUseStamina = 0f;
				GUIManager.instance.bar.ChangeBar();
				return true;
			}
			return false;
		}
		else
		{
			this.data.currentStamina -= usage;
			this.data.sinceUseStamina = 0f;
			GUIManager.instance.bar.ChangeBar();
			if (this.data.currentStamina <= 0f)
			{
				this.ClampStamina();
				return this.data.extraStamina > 0f;
			}
			return true;
		}
	}

	// Token: 0x06000074 RID: 116 RVA: 0x00004998 File Offset: 0x00002B98
	[PunRPC]
	public void MoraleBoost(float staminaAdd, int scoutCount)
	{
		GUIManager.instance.bar.PlayMoraleBoost(scoutCount);
		this.AddExtraStamina(staminaAdd);
	}

	// Token: 0x06000075 RID: 117 RVA: 0x000049B1 File Offset: 0x00002BB1
	public void AddStamina(float add)
	{
		if (!this.view.IsMine)
		{
			return;
		}
		this.data.currentStamina += add;
		this.ClampStamina();
		GUIManager.instance.bar.ChangeBar();
	}

	// Token: 0x06000076 RID: 118 RVA: 0x000049E9 File Offset: 0x00002BE9
	public void ClampStamina()
	{
		this.data.currentStamina = Mathf.Clamp(this.data.currentStamina, 0f, this.GetMaxStamina());
	}

	// Token: 0x06000077 RID: 119 RVA: 0x00004A11 File Offset: 0x00002C11
	public float GetMaxStamina()
	{
		return Mathf.Max(1f - this.refs.afflictions.statusSum, 0f);
	}

	// Token: 0x06000078 RID: 120 RVA: 0x00004A33 File Offset: 0x00002C33
	public void SetExtraStamina(float amt)
	{
		if (!this.view.IsMine)
		{
			return;
		}
		this.data.extraStamina = Mathf.Clamp(amt, 0f, 1f);
		GUIManager.instance.bar.ChangeBar();
	}

	// Token: 0x06000079 RID: 121 RVA: 0x00004A70 File Offset: 0x00002C70
	public void AddExtraStamina(float add)
	{
		if (!this.view.IsMine)
		{
			return;
		}
		this.data.extraStamina += add;
		this.data.extraStamina = Mathf.Clamp(this.data.extraStamina, 0f, 1f);
		GUIManager.instance.bar.ChangeBar();
	}

	// Token: 0x0600007A RID: 122 RVA: 0x00004AD2 File Offset: 0x00002CD2
	public void FeedItem(Item item)
	{
		base.photonView.RPC("GetFedItemRPC", RpcTarget.All, new object[] { item.photonView.ViewID });
	}

	// Token: 0x0600007B RID: 123 RVA: 0x00004B00 File Offset: 0x00002D00
	[PunRPC]
	public void GetFedItemRPC(int itemPhotonID)
	{
		if (!base.photonView.IsMine)
		{
			return;
		}
		PhotonView photonView = PhotonView.Find(itemPhotonID);
		if (photonView == null)
		{
			return;
		}
		Item item = ((photonView != null) ? photonView.GetComponent<Item>() : null);
		if (item == null)
		{
			return;
		}
		item.overrideHolderCharacter = this;
		if (item.OnPrimaryFinishedCast != null)
		{
			item.OnPrimaryFinishedCast();
		}
		item.overrideHolderCharacter = null;
	}

	// Token: 0x0600007C RID: 124 RVA: 0x00004B64 File Offset: 0x00002D64
	internal void DragTowards(Vector3 target, float force)
	{
		Action<Vector3, float> action = this.dragTowardsAction;
		if (action != null)
		{
			action(target, force);
		}
		Vector3 vector = Vector3.ClampMagnitude(target - this.Center, 1f);
		this.AddForce(vector * force, 1f, 1f);
	}

	// Token: 0x0600007D RID: 125 RVA: 0x00004BB2 File Offset: 0x00002DB2
	internal bool OutOfStamina()
	{
		return this.data.currentStamina < 0.005f && this.data.extraStamina < 0.001f;
	}

	// Token: 0x0600007E RID: 126 RVA: 0x00004BDA File Offset: 0x00002DDA
	internal bool OutOfRegularStamina()
	{
		return this.data.currentStamina < 0.005f;
	}

	// Token: 0x0600007F RID: 127 RVA: 0x00004BEE File Offset: 0x00002DEE
	internal bool IsSliding()
	{
		return this.data.isClimbing && this.OutOfStamina();
	}

	// Token: 0x06000080 RID: 128 RVA: 0x00004C05 File Offset: 0x00002E05
	internal bool CanDoInput()
	{
		return !GUIManager.instance.windowBlockingInput && !GUIManager.instance.wheelActive;
	}

	// Token: 0x06000081 RID: 129 RVA: 0x00004C24 File Offset: 0x00002E24
	internal int GetPlayerListID(List<Character> playerList)
	{
		for (int i = 0; i < playerList.Count; i++)
		{
			if (playerList[i] == this)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x06000082 RID: 130 RVA: 0x00004C54 File Offset: 0x00002E54
	internal void Fall(float seconds)
	{
		this.refs.view.RPC("RPCA_Fall", RpcTarget.All, new object[] { seconds });
	}

	// Token: 0x06000083 RID: 131 RVA: 0x00004C7B File Offset: 0x00002E7B
	[PunRPC]
	public void RPCA_Fall(float seconds)
	{
		if (seconds > this.data.fallSeconds)
		{
			this.data.fallSeconds = seconds;
		}
	}

	// Token: 0x06000084 RID: 132 RVA: 0x00004C98 File Offset: 0x00002E98
	[ConsoleCommand]
	public static void Revive()
	{
		Debug.Log(string.Format("Reviving, status: {0}, fullyPassedOut: {1}", Character.localCharacter.data.dead, Character.localCharacter.data.fullyPassedOut));
		if (Character.localCharacter.data.dead || Character.localCharacter.data.fullyPassedOut)
		{
			Character.localCharacter.view.RPC("RPCA_Revive", RpcTarget.All, new object[] { true });
		}
	}

	// Token: 0x06000085 RID: 133 RVA: 0x00004D24 File Offset: 0x00002F24
	[PunRPC]
	internal void RPCA_Revive(bool applyStatus)
	{
		Action action = this.reviveAction;
		if (action != null)
		{
			action();
		}
		this.data.dead = false;
		this.data.deathTimer = 0f;
		this.data.passedOut = false;
		this.data.fullyPassedOut = false;
		this.data.sinceGrounded = 0f;
		this.refs.afflictions.ClearAllStatus(true);
		this.refs.afflictions.SetStatus(CharacterAfflictions.STATUSTYPE.Crab, 0f);
		RoomProperties.me.SaveReconnectData();
		if (applyStatus)
		{
			this.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Curse, 0.05f, false);
			this.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Hunger, 0.3f, false);
		}
	}

	// Token: 0x06000086 RID: 134 RVA: 0x00004DEC File Offset: 0x00002FEC
	[PunRPC]
	internal void RPCA_ReviveAtPosition(Vector3 position, bool applyStatus)
	{
		this.refs.items.DropAllItems(true);
		this.RPCA_Revive(applyStatus);
		this.WarpPlayer(position, true);
		this.refs.stats.justDied = false;
		this.refs.stats.justRevived = true;
		this.refs.stats.Record(true, position.y);
	}

	// Token: 0x06000087 RID: 135 RVA: 0x00004E52 File Offset: 0x00003052
	[PunRPC]
	public void WarpPlayerRPC(Vector3 position, bool poof)
	{
		this.WarpPlayer(position, poof);
	}

	// Token: 0x06000088 RID: 136 RVA: 0x00004E5C File Offset: 0x0000305C
	public void PlayPoofVFX(Vector3 pos)
	{
		this.refs.poof.transform.position = pos;
		this.refs.poof.main.startColor = this.refs.customization.PlayerColor;
		this.refs.poof.Play();
		for (int i = 0; i < this.poofSFX.Length; i++)
		{
			this.poofSFX[i].Play(pos);
		}
	}

	// Token: 0x1700000A RID: 10
	// (get) Token: 0x06000089 RID: 137 RVA: 0x00004EDD File Offset: 0x000030DD
	// (set) Token: 0x0600008A RID: 138 RVA: 0x00004EE5 File Offset: 0x000030E5
	public bool warping { get; private set; }

	// Token: 0x0600008B RID: 139 RVA: 0x00004EF0 File Offset: 0x000030F0
	private void WarpPlayer(Vector3 position, bool poof)
	{
		Character.<>c__DisplayClass113_0 CS$<>8__locals1 = new Character.<>c__DisplayClass113_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.position = position;
		Debug.Log(string.Format("Starting move {0} to position {1} from {2} via MovePlayer routine", this.characterName, CS$<>8__locals1.position, this.Center));
		base.StartCoroutine(CS$<>8__locals1.<WarpPlayer>g__IMove|0());
		if (poof)
		{
			this.PlayPoofVFX(CS$<>8__locals1.position);
		}
	}

	// Token: 0x0600008C RID: 140 RVA: 0x00004F58 File Offset: 0x00003158
	internal void MoveBodypartTowardsPoint(BodypartType bodypart, Vector3 pos, float force, float clampDistance = 1f)
	{
		Bodypart bodypart2 = this.GetBodypart(bodypart);
		bodypart2.AddForce(Vector3.ClampMagnitude(pos - bodypart2.Rig.position, clampDistance) * force, ForceMode.Acceleration);
	}

	// Token: 0x0600008D RID: 141 RVA: 0x00004F94 File Offset: 0x00003194
	public static bool PlayerIsDeadOrDown()
	{
		foreach (Character character in Character.AllCharacters)
		{
			if (character.data.dead || character.data.fullyPassedOut)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600008E RID: 142 RVA: 0x00005000 File Offset: 0x00003200
	internal BodypartType GetPartType(Rigidbody rigidbody)
	{
		foreach (Bodypart bodypart in this.refs.ragdoll.partList)
		{
			if (bodypart.Rig == rigidbody)
			{
				return bodypart.partType;
			}
		}
		return (BodypartType)(-1);
	}

	// Token: 0x0600008F RID: 143 RVA: 0x00005070 File Offset: 0x00003270
	internal void LimitFalling()
	{
		this.data.sinceGrounded = Mathf.Min(this.data.sinceGrounded, 0.5f);
		this.data.sinceJump = Mathf.Min(this.data.sinceJump, 0.5f);
	}

	// Token: 0x06000090 RID: 144 RVA: 0x000050BD File Offset: 0x000032BD
	internal void AddIllegalStatus(string illegalStatus, float amount)
	{
		Action<string, float> action = this.illegalStatusAction;
		if (action == null)
		{
			return;
		}
		action(illegalStatus, amount);
	}

	// Token: 0x1700000B RID: 11
	// (get) Token: 0x06000091 RID: 145 RVA: 0x000050D1 File Offset: 0x000032D1
	// (set) Token: 0x06000092 RID: 146 RVA: 0x000050D9 File Offset: 0x000032D9
	public bool infiniteStam { get; private set; }

	// Token: 0x06000093 RID: 147 RVA: 0x000050E4 File Offset: 0x000032E4
	[ConsoleCommand]
	public static void InfiniteStamina()
	{
		if (!Character.localCharacter.infiniteStam)
		{
			Character.localCharacter.data.currentStamina = 1f;
		}
		Character.localCharacter.infiniteStam = !Character.localCharacter.infiniteStam;
		Debug.LogError(string.Format("Infinite Stamina: {0}", Character.localCharacter.infiniteStam));
	}

	// Token: 0x1700000C RID: 12
	// (get) Token: 0x06000094 RID: 148 RVA: 0x00005146 File Offset: 0x00003346
	// (set) Token: 0x06000095 RID: 149 RVA: 0x0000514E File Offset: 0x0000334E
	public bool statusesLocked { get; private set; }

	// Token: 0x06000096 RID: 150 RVA: 0x00005157 File Offset: 0x00003357
	[ConsoleCommand]
	public static void LockStatuses()
	{
		Character.localCharacter.statusesLocked = !Character.localCharacter.statusesLocked;
		Debug.LogError(string.Format("Statuses Locked: {0}", Character.localCharacter.statusesLocked));
	}

	// Token: 0x06000097 RID: 151 RVA: 0x0000518E File Offset: 0x0000338E
	private void OnGetMic(float db)
	{
	}

	// Token: 0x06000098 RID: 152 RVA: 0x00005190 File Offset: 0x00003390
	internal void StartPassedOutOnTheBeach()
	{
		Debug.Log("Starting passed out!");
		this.data.passedOutOnTheBeach = 3f;
		this.Fall(7f);
	}

	// Token: 0x0600009B RID: 155 RVA: 0x000051CB File Offset: 0x000033CB
	[CompilerGenerated]
	private void <RPCA_PassOut>g__PassOutDone|53_0()
	{
		this.data.fullyPassedOut = true;
	}

	// Token: 0x0400002B RID: 43
	public bool isBot;

	// Token: 0x0400002C RID: 44
	public static Character localCharacter;

	// Token: 0x0400002D RID: 45
	public CharacterInput input;

	// Token: 0x0400002E RID: 46
	public CharacterData data;

	// Token: 0x0400002F RID: 47
	public Character.CharacterRefs refs;

	// Token: 0x04000031 RID: 49
	private PhotonView view;

	// Token: 0x04000032 RID: 50
	public static List<Character> AllCharacters = new List<Character>();

	// Token: 0x04000033 RID: 51
	private Vector3 smoothedCamPos;

	// Token: 0x04000034 RID: 52
	public SFX_Instance[] poofSFX;

	// Token: 0x04000035 RID: 53
	private static bool forceWin;

	// Token: 0x04000036 RID: 54
	private bool UnPassOutCalled;

	// Token: 0x04000037 RID: 55
	public Action UnPassOutAction;

	// Token: 0x04000038 RID: 56
	private bool unPassOutCalled;

	// Token: 0x04000039 RID: 57
	public Action<float> landAction;

	// Token: 0x0400003A RID: 58
	public Action startJumpAction;

	// Token: 0x0400003B RID: 59
	public Action jumpAction;

	// Token: 0x0400003C RID: 60
	internal Action startClimbAction;

	// Token: 0x0400003D RID: 61
	public Action<Vector3, float> dragTowardsAction;

	// Token: 0x0400003E RID: 62
	public Action reviveAction;

	// Token: 0x04000040 RID: 64
	public Action<string, float> illegalStatusAction;

	// Token: 0x020002E6 RID: 742
	[Serializable]
	public class CharacterRefs
	{
		// Token: 0x04001078 RID: 4216
		public Transform carryPosRef;

		// Token: 0x04001079 RID: 4217
		public CharacterRopeHandling ropeHandling;

		// Token: 0x0400107A RID: 4218
		public CharacterClimbing climbing;

		// Token: 0x0400107B RID: 4219
		public CharacterMovement movement;

		// Token: 0x0400107C RID: 4220
		public CharacterRagdoll ragdoll;

		// Token: 0x0400107D RID: 4221
		public RigCreator rigCreator;

		// Token: 0x0400107E RID: 4222
		public Bodypart head;

		// Token: 0x0400107F RID: 4223
		public Bodypart hip;

		// Token: 0x04001080 RID: 4224
		public CharacterAnimations animations;

		// Token: 0x04001081 RID: 4225
		public Animator animator;

		// Token: 0x04001082 RID: 4226
		public RigBuilder ikRigBuilder;

		// Token: 0x04001083 RID: 4227
		public Rig ikRig;

		// Token: 0x04001084 RID: 4228
		public TwoBoneIKConstraint ikLeft;

		// Token: 0x04001085 RID: 4229
		public TwoBoneIKConstraint ikRight;

		// Token: 0x04001086 RID: 4230
		public CharacterItems items;

		// Token: 0x04001087 RID: 4231
		public AnimatedVariables animatedVariables;

		// Token: 0x04001088 RID: 4232
		public CharacterAfflictions afflictions;

		// Token: 0x04001089 RID: 4233
		public BadgeUnlocker badgeUnlocker;

		// Token: 0x0400108A RID: 4234
		public PhotonView view;

		// Token: 0x0400108B RID: 4235
		public CharacterHeatEmission heatEmission;

		// Token: 0x0400108C RID: 4236
		public CharacterVineClimbing vineClimbing;

		// Token: 0x0400108D RID: 4237
		public SkinnedMeshRenderer mainRenderer;

		// Token: 0x0400108E RID: 4238
		public CharacterCarrying carriying;

		// Token: 0x0400108F RID: 4239
		public CharacterCustomization customization;

		// Token: 0x04001090 RID: 4240
		public CharacterStats stats;

		// Token: 0x04001091 RID: 4241
		public CharacterGrabbing grabbing;

		// Token: 0x04001092 RID: 4242
		public HideTheBody hideTheBody;

		// Token: 0x04001093 RID: 4243
		public ParticleSystem poof;

		// Token: 0x04001094 RID: 4244
		public Transform IKHandTargetLeft;

		// Token: 0x04001095 RID: 4245
		public Transform IKHandTargetRight;

		// Token: 0x04001096 RID: 4246
		public Transform helperObjects;

		// Token: 0x04001097 RID: 4247
		public Transform animationHeadTransform;

		// Token: 0x04001098 RID: 4248
		public Transform animationHipTransform;

		// Token: 0x04001099 RID: 4249
		public Transform animationItemTransform;

		// Token: 0x0400109A RID: 4250
		public Transform animationLookTransform;

		// Token: 0x0400109B RID: 4251
		public Transform animationPositionTransform;

		// Token: 0x0400109C RID: 4252
		public Transform backpackTransform;
	}
}
