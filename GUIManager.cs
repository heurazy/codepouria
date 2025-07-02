using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Zorro.Core;

// Token: 0x02000097 RID: 151
public class GUIManager : MonoBehaviour
{
	// Token: 0x17000054 RID: 84
	// (get) Token: 0x0600052C RID: 1324 RVA: 0x0001D8B0 File Offset: 0x0001BAB0
	public bool wheelActive
	{
		get
		{
			return this.emoteWheel.gameObject.activeSelf || this.backpackWheel.gameObject.activeSelf;
		}
	}

	// Token: 0x17000055 RID: 85
	// (get) Token: 0x0600052D RID: 1325 RVA: 0x0001D8D6 File Offset: 0x0001BAD6
	// (set) Token: 0x0600052E RID: 1326 RVA: 0x0001D8DE File Offset: 0x0001BADE
	internal IInteractible currentInteractable { get; private set; }

	// Token: 0x17000056 RID: 86
	// (get) Token: 0x0600052F RID: 1327 RVA: 0x0001D8E7 File Offset: 0x0001BAE7
	// (set) Token: 0x06000530 RID: 1328 RVA: 0x0001D8EF File Offset: 0x0001BAEF
	public ControllerManager controllerManager { get; private set; }

	// Token: 0x14000001 RID: 1
	// (add) Token: 0x06000531 RID: 1329 RVA: 0x0001D8F8 File Offset: 0x0001BAF8
	// (remove) Token: 0x06000532 RID: 1330 RVA: 0x0001D930 File Offset: 0x0001BB30
	public event GUIManager.MenuWindowEvent OnMenuWindowOpened;

	// Token: 0x14000002 RID: 2
	// (add) Token: 0x06000533 RID: 1331 RVA: 0x0001D968 File Offset: 0x0001BB68
	// (remove) Token: 0x06000534 RID: 1332 RVA: 0x0001D9A0 File Offset: 0x0001BBA0
	public event GUIManager.MenuWindowEvent OnMenuWindowClosed;

	// Token: 0x06000535 RID: 1333 RVA: 0x0001D9D5 File Offset: 0x0001BBD5
	private void Awake()
	{
		GUIManager.instance = this;
		this.controllerManager = new ControllerManager();
		this.controllerManager.Init();
		this.InitReticleList();
	}

	// Token: 0x06000536 RID: 1334 RVA: 0x0001D9FC File Offset: 0x0001BBFC
	private void OnDestroy()
	{
		this.controllerManager.Destroy();
		if (this.character != null)
		{
			CharacterItems characterItems = this.character.refs.items;
			characterItems.onSlotEquipped = (Action)Delegate.Remove(characterItems.onSlotEquipped, new Action(this.OnSlotEquipped));
			GameUtils gameUtils = GameUtils.instance;
			gameUtils.OnUpdatedFeedData = (Action)Delegate.Remove(gameUtils.OnUpdatedFeedData, new Action(this.OnUpdatedFeedData));
		}
	}

	// Token: 0x06000537 RID: 1335 RVA: 0x0001DA79 File Offset: 0x0001BC79
	private void Start()
	{
		this.UpdateItemPrompts();
		this.OnInteractChange();
		this.throwGO.SetActive(false);
		this.spectatingObject.SetActive(false);
		this.heroObject.SetActive(false);
	}

	// Token: 0x06000538 RID: 1336 RVA: 0x0001DAAC File Offset: 0x0001BCAC
	private void LateUpdate()
	{
		this.UpdateDebug();
		this.UpdateBinocularOverlay();
		this.UpdateWindowStatus();
		if (Character.localCharacter)
		{
			if (Interaction.instance.currentHovered != this.currentInteractable)
			{
				this.OnInteractChange();
			}
			if (this.wasPitonClimbing)
			{
				this.RefreshInteractablePrompt();
			}
			this.interactPromptLunge.SetActive(Character.localCharacter.data.isClimbing && Character.localCharacter.data.currentStamina < 0.05f && Character.localCharacter.data.currentStamina > 0.0001f);
			this.wasPitonClimbing = Character.localCharacter.data.climbingSpikeCount > 0 && Character.localCharacter.data.isClimbing;
			if (!this.character)
			{
				this.character = Character.localCharacter;
				CharacterItems characterItems = this.character.refs.items;
				characterItems.onSlotEquipped = (Action)Delegate.Combine(characterItems.onSlotEquipped, new Action(this.OnSlotEquipped));
				GameUtils gameUtils = GameUtils.instance;
				gameUtils.OnUpdatedFeedData = (Action)Delegate.Combine(gameUtils.OnUpdatedFeedData, new Action(this.OnUpdatedFeedData));
			}
			this.UpdateReticle();
			this.UpdateThrow();
			this.UpdateRope();
			this.UpdateDyingBar();
			this.UpdateEmoteWheel();
			this.TestUpdateItemPrompts();
			this.UpdateSpectate();
			this.UpdatePaused();
		}
		if (Character.observedCharacter)
		{
			this.UpdateItems();
		}
	}

	// Token: 0x17000057 RID: 87
	// (get) Token: 0x06000539 RID: 1337 RVA: 0x0001DC26 File Offset: 0x0001BE26
	// (set) Token: 0x0600053A RID: 1338 RVA: 0x0001DC2E File Offset: 0x0001BE2E
	public bool windowShowingCursor { get; private set; }

	// Token: 0x17000058 RID: 88
	// (get) Token: 0x0600053B RID: 1339 RVA: 0x0001DC37 File Offset: 0x0001BE37
	// (set) Token: 0x0600053C RID: 1340 RVA: 0x0001DC3F File Offset: 0x0001BE3F
	public bool windowBlockingInput { get; private set; }

	// Token: 0x0600053D RID: 1341 RVA: 0x0001DC48 File Offset: 0x0001BE48
	public void UpdateWindowStatus()
	{
		this.windowShowingCursor = false;
		this.windowBlockingInput = false;
		foreach (MenuWindow menuWindow in MenuWindow.AllActiveWindows)
		{
			if (menuWindow.blocksPlayerInput)
			{
				this.lastBlockedInput = Time.frameCount;
			}
			if (menuWindow.showCursorWhileOpen)
			{
				this.windowShowingCursor = true;
			}
		}
		if (Time.frameCount < this.lastBlockedInput + 2)
		{
			this.windowBlockingInput = true;
		}
	}

	// Token: 0x0600053E RID: 1342 RVA: 0x0001DCD8 File Offset: 0x0001BED8
	public void UpdatePaused()
	{
		if (Character.localCharacter.input.pauseWasPressed && !this.pauseMenu.isOpen)
		{
			if (this.wheelActive)
			{
				return;
			}
			if (this.endScreen.isOpen)
			{
				return;
			}
			this.pauseMenu.Open();
			Character.localCharacter.input.pauseWasPressed = false;
		}
	}

	// Token: 0x0600053F RID: 1343 RVA: 0x0001DD38 File Offset: 0x0001BF38
	private void OnSlotEquipped()
	{
		for (int i = 0; i < this.items.Length; i++)
		{
			if (i < Character.localCharacter.player.itemSlots.Length)
			{
				this.items[i].SetSelected();
			}
		}
		this.backpack.SetSelected();
	}

	// Token: 0x06000540 RID: 1344 RVA: 0x0001DD84 File Offset: 0x0001BF84
	private void OnUpdatedFeedData()
	{
		GUIManager.<>c__DisplayClass124_0 CS$<>8__locals1 = new GUIManager.<>c__DisplayClass124_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.feedData = GameUtils.instance.GetFeedDataForReceiver(Character.localCharacter.photonView.ViewID);
		int j;
		int num;
		for (j = 0; j < CS$<>8__locals1.feedData.Count; j = num + 1)
		{
			if (!this.friendUseItemProgressList.Any((UI_UseItemProgressFriend f) => f.giverID == CS$<>8__locals1.feedData[j].giverID))
			{
				UI_UseItemProgressFriend ui_UseItemProgressFriend = Object.Instantiate<UI_UseItemProgressFriend>(this.friendUseItemProgressPrefab, this.friendProgressTF);
				this.friendUseItemProgressList.Add(ui_UseItemProgressFriend);
				ui_UseItemProgressFriend.Init(CS$<>8__locals1.feedData[j]);
			}
			num = j;
		}
		int i;
		for (i = 0; i < this.friendUseItemProgressList.Count; i = num + 1)
		{
			if (!CS$<>8__locals1.feedData.Any((FeedData f) => f.giverID == CS$<>8__locals1.<>4__this.friendUseItemProgressList[i].giverID))
			{
				this.friendUseItemProgressList[i].Kill();
				this.friendUseItemProgressList.RemoveAt(i);
			}
			num = i;
		}
	}

	// Token: 0x06000541 RID: 1345 RVA: 0x0001DED8 File Offset: 0x0001C0D8
	public void SetHeroTitle(string text, AudioClip stinger)
	{
		if (this._heroRoutine != null)
		{
			base.StopCoroutine(this._heroRoutine);
		}
		if (this.stingerSound && stinger != null)
		{
			this.stingerSound.clip = stinger;
			this.stingerSound.Play();
		}
		this._heroRoutine = base.StartCoroutine(this.<SetHeroTitle>g__HeroRoutine|125_0(text));
	}

	// Token: 0x06000542 RID: 1346 RVA: 0x0001DF39 File Offset: 0x0001C139
	public void OpenBackpackWheel(BackpackReference backpackReference)
	{
		if (!this.wheelActive && !this.windowBlockingInput)
		{
			Character.localCharacter.data.usingBackpackWheel = true;
			this.backpackWheel.InitWheel(backpackReference);
		}
	}

	// Token: 0x06000543 RID: 1347 RVA: 0x0001DF67 File Offset: 0x0001C167
	public void CloseBackpackWheel()
	{
		Debug.Log("Close Input Wheel");
		Character.localCharacter.data.usingBackpackWheel = false;
		this.backpackWheel.gameObject.SetActive(false);
	}

	// Token: 0x06000544 RID: 1348 RVA: 0x0001DF94 File Offset: 0x0001C194
	private void UpdateEmoteWheel()
	{
		if (Character.localCharacter.input.emoteIsPressed)
		{
			if (!this.wheelActive && !this.windowBlockingInput)
			{
				this.emoteWheel.SetActive(true);
				Character.localCharacter.data.usingEmoteWheel = true;
				return;
			}
		}
		else if (Character.localCharacter.data.usingEmoteWheel)
		{
			this.emoteWheel.SetActive(false);
			Character.localCharacter.data.usingEmoteWheel = false;
		}
	}

	// Token: 0x06000545 RID: 1349 RVA: 0x0001E00C File Offset: 0x0001C20C
	private void UpdateDyingBar()
	{
		this.dyingBarObject.gameObject.SetActive(Character.localCharacter.data.fullyPassedOut || Character.localCharacter.data.dead);
		if (this.dyingBarObject.gameObject.activeSelf)
		{
			this.dyingBarImage.fillAmount = 1f - Character.localCharacter.data.deathTimer;
			this.dyingBarImage.color = this.dyingBarGradient.Evaluate(1f - Character.localCharacter.data.deathTimer);
			if (Character.localCharacter.data.deathTimer >= 1f && !this.dead)
			{
				this.dyingBarAnimator.Play("Dead", 0, 0f);
				this.dead = true;
				return;
			}
		}
		else
		{
			this.dead = false;
		}
	}

	// Token: 0x06000546 RID: 1350 RVA: 0x0001E0F0 File Offset: 0x0001C2F0
	private void UpdateSpectate()
	{
		if (MainCameraMovement.specCharacter != this.currentSpecCharacter)
		{
			this.currentSpecCharacter = MainCameraMovement.specCharacter;
			if (this.currentSpecCharacter)
			{
				this.spectatingObject.SetActive(true);
				if (this.currentSpecCharacter == Character.localCharacter)
				{
					this.spectatingNameText.text = "YOURSELF";
					this.spectatingNameText.color = this.spectatingYourselfColor;
					return;
				}
				this.spectatingNameText.text = MainCameraMovement.specCharacter.characterName;
				this.spectatingNameText.color = this.spectatingNameColor;
				return;
			}
			else
			{
				this.spectatingObject.SetActive(false);
			}
		}
	}

	// Token: 0x06000547 RID: 1351 RVA: 0x0001E1A0 File Offset: 0x0001C3A0
	private void UpdateRope()
	{
		RopeSpool ropeSpool;
		if (Character.localCharacter.data.currentItem && Character.localCharacter.data.currentItem.TryGetComponent<RopeSpool>(out ropeSpool))
		{
			this.ui_rope.gameObject.SetActive(true);
			if (ropeSpool.rope)
			{
				this.ui_rope.UpdateRope(ropeSpool.rope.GetRopeSegments().Count);
			}
			Shader.SetGlobalFloat(this.ROPE_INVERT, (float)(ropeSpool.isAntiRope ? 1 : 0));
			return;
		}
		this.ui_rope.gameObject.SetActive(false);
	}

	// Token: 0x06000548 RID: 1352 RVA: 0x0001E240 File Offset: 0x0001C440
	private void UpdateThrow()
	{
		this.throwGO.SetActive(Character.localCharacter.refs.items.throwChargeLevel > 0f);
		if (Character.localCharacter.refs.items.throwChargeLevel > 0f)
		{
			float num = Mathf.Lerp(0.692f, 0.808f, Character.localCharacter.refs.items.throwChargeLevel);
			this.throwBar.fillAmount = num;
			this.throwBar.color = this.throwGradient.Evaluate(Character.localCharacter.refs.items.throwChargeLevel);
		}
	}

	// Token: 0x06000549 RID: 1353 RVA: 0x0001E2E8 File Offset: 0x0001C4E8
	private void UpdateReticle()
	{
		this.reticleDefaultImage.color = ((this.character.data.sinceCanClimb < 0.05f) ? this.reticleColorHighlight : this.reticleColorDefault);
		if (Character.localCharacter.data.fullyPassedOut || Character.localCharacter.data.dead)
		{
			this.SetReticle(null);
			return;
		}
		if (this.reticleLock > 0f)
		{
			this.reticleLock -= Time.deltaTime;
			return;
		}
		if (Character.localCharacter.data.currentClimbHandle != null)
		{
			this.SetReticle(this.reticleSpike);
			return;
		}
		if (Character.localCharacter.data.isRopeClimbing)
		{
			this.SetReticle(this.reticleRope);
			return;
		}
		if (Character.localCharacter.data.sincePalJump < 0.5f)
		{
			this.SetReticle(this.reticleBoost);
			return;
		}
		if (Character.localCharacter.refs.items.throwChargeLevel > 0f)
		{
			this.SetReticle(this.reticleThrow);
			return;
		}
		if (Character.localCharacter.data.sincePressClimb < 0.1f && Character.localCharacter.refs.climbing.CanClimb())
		{
			this.SetReticle(this.reticleClimbTry);
			return;
		}
		if (Character.localCharacter.data.isClimbing)
		{
			if (Character.localCharacter.OutOfStamina())
			{
				this.SetReticle(this.reticleX);
				return;
			}
			this.SetReticle(this.reticleClimb);
			return;
		}
		else
		{
			if (Character.localCharacter.data.isReaching)
			{
				this.SetReticle(this.reticleReach);
				return;
			}
			if (Character.localCharacter.data.isVineClimbing)
			{
				this.SetReticle(this.reticleVine);
				return;
			}
			if (Character.localCharacter.data.currentItem && Character.localCharacter.data.currentItem.UIData.isShootable && Character.localCharacter.data.currentItem.CanUsePrimary())
			{
				this.SetReticle(this.reticleShoot);
				return;
			}
			this.SetReticle(this.reticleDefault);
			return;
		}
	}

	// Token: 0x0600054A RID: 1354 RVA: 0x0001E508 File Offset: 0x0001C708
	public void ReticleLand()
	{
		RectTransform component = this.reticleDefault.GetComponent<RectTransform>();
		component.sizeDelta = new Vector2(40f, 10f);
		component.DOSizeDelta(new Vector2(10f, 10f), 0.33f, false).SetEase(Ease.InOutCubic);
	}

	// Token: 0x0600054B RID: 1355 RVA: 0x0001E557 File Offset: 0x0001C757
	public void Grasp()
	{
		this.SetReticle(this.reticleGrasp);
		this.reticleGrasp.GetComponent<Animator>().Play("Play", 0, 0f);
		this.reticleLock = 1f;
	}

	// Token: 0x0600054C RID: 1356 RVA: 0x0001E58B File Offset: 0x0001C78B
	public void ClimbJump()
	{
		this.SetReticle(this.reticleClimbJump);
		this.reticleLock = 0.5f;
	}

	// Token: 0x0600054D RID: 1357 RVA: 0x0001E5A4 File Offset: 0x0001C7A4
	private void SetReticle(GameObject activeReticle)
	{
		if (activeReticle == this.lastReticle && activeReticle != null)
		{
			return;
		}
		this.lastReticle = activeReticle;
		for (int i = 0; i < this.reticleList.Count; i++)
		{
			if (this.reticleList[i] != activeReticle)
			{
				this.reticleList[i].SetActive(false);
			}
		}
		if (activeReticle)
		{
			activeReticle.SetActive(true);
		}
	}

	// Token: 0x0600054E RID: 1358 RVA: 0x0001E61C File Offset: 0x0001C81C
	private void InitReticleList()
	{
		this.reticleList.Add(this.reticleDefault);
		this.reticleList.Add(this.reticleRope);
		this.reticleList.Add(this.reticleSpike);
		this.reticleList.Add(this.reticleThrow);
		this.reticleList.Add(this.reticleReach);
		this.reticleList.Add(this.reticleX);
		this.reticleList.Add(this.reticleClimb);
		this.reticleList.Add(this.reticleClimbJump);
		this.reticleList.Add(this.reticleClimbTry);
		this.reticleList.Add(this.reticleGrasp);
		this.reticleList.Add(this.reticleVine);
		this.reticleList.Add(this.reticleBoost);
		this.reticleList.Add(this.reticleShoot);
	}

	// Token: 0x0600054F RID: 1359 RVA: 0x0001E706 File Offset: 0x0001C906
	private void UpdateDebug()
	{
	}

	// Token: 0x06000550 RID: 1360 RVA: 0x0001E708 File Offset: 0x0001C908
	private IEnumerator ScreenshotRoutine(bool disableHud)
	{
		bool cacheEnabled = this.hudCanvas.enabled;
		if (disableHud)
		{
			this.hudCanvas.enabled = false;
		}
		yield return null;
		string text = "";
		if (Application.isEditor)
		{
			text = "Screenshots/";
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
		}
		string text2 = "Screenshot_" + DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss") + ".png";
		ScreenCapture.CaptureScreenshot(Path.Combine(text, text2), 2);
		yield return null;
		this.hudCanvas.enabled = cacheEnabled;
		yield break;
	}

	// Token: 0x06000551 RID: 1361 RVA: 0x0001E720 File Offset: 0x0001C920
	public void AddStatusFX(CharacterAfflictions.STATUSTYPE type, float amount)
	{
		switch (type)
		{
		case CharacterAfflictions.STATUSTYPE.Injury:
			this.InjuryFX(amount);
			return;
		case CharacterAfflictions.STATUSTYPE.Hunger:
			this.HungerFX();
			return;
		case CharacterAfflictions.STATUSTYPE.Cold:
			this.ColdFX(amount);
			return;
		case CharacterAfflictions.STATUSTYPE.Poison:
			this.PoisonFX(amount);
			return;
		case CharacterAfflictions.STATUSTYPE.Curse:
			this.CurseFX(amount);
			return;
		case CharacterAfflictions.STATUSTYPE.Drowsy:
			this.DrowsyFX();
			return;
		case CharacterAfflictions.STATUSTYPE.Hot:
			this.HotFX(amount);
			return;
		}
		this.InjuryFX(amount);
	}

	// Token: 0x06000552 RID: 1362 RVA: 0x0001E796 File Offset: 0x0001C996
	private void InjuryFX(float amount)
	{
		GamefeelHandler.instance.AddPerlinShake((amount + 1f) * 5f, 0.3f, 15f);
		this.injurySVFX.Play(amount);
	}

	// Token: 0x06000553 RID: 1363 RVA: 0x0001E7C5 File Offset: 0x0001C9C5
	private void CurseFX(float amount)
	{
		GamefeelHandler.instance.AddPerlinShake((amount + 1f) * 30f, 0.3f, 15f);
		this.curseSVFX.Play(amount);
	}

	// Token: 0x06000554 RID: 1364 RVA: 0x0001E7F4 File Offset: 0x0001C9F4
	private void HungerFX()
	{
	}

	// Token: 0x06000555 RID: 1365 RVA: 0x0001E7F8 File Offset: 0x0001C9F8
	private void DrowsyFX()
	{
		float num = 1f;
		GamefeelHandler.instance.AddPerlinShake(num * 5f, 0.3f, 15f);
		this.drowsyFX.Play(num);
	}

	// Token: 0x06000556 RID: 1366 RVA: 0x0001E832 File Offset: 0x0001CA32
	private void PoisonFX(float amount)
	{
		amount = 0.5f;
		GamefeelHandler.instance.AddPerlinShake(amount * 5f, 0.3f, 15f);
		this.poisonSVFX.Play(amount);
	}

	// Token: 0x06000557 RID: 1367 RVA: 0x0001E862 File Offset: 0x0001CA62
	private void ColdFX(float amount)
	{
		amount = 1f;
		GamefeelHandler.instance.AddPerlinShake(amount * 2f, 1f, 30f);
		this.PlayFXSequence(ref this.coldSequence, this.coldVolume, amount);
	}

	// Token: 0x06000558 RID: 1368 RVA: 0x0001E899 File Offset: 0x0001CA99
	private void HotFX(float amount)
	{
		amount = 1f;
		GamefeelHandler.instance.AddPerlinShake(amount * 2f, 1f, 30f);
		this.hotSVFX.Play(amount);
	}

	// Token: 0x06000559 RID: 1369 RVA: 0x0001E8CC File Offset: 0x0001CACC
	private void PlayFXSequence(ref Sequence sequence, Volume volume, float amount)
	{
		sequence.Kill(false);
		sequence = DOTween.Sequence();
		sequence.Append(DOTween.To(() => volume.weight, delegate(float x)
		{
			volume.weight = x;
		}, amount, 0.06f));
		sequence.AppendInterval(0.25f * amount);
		sequence.Append(DOTween.To(() => volume.weight, delegate(float x)
		{
			volume.weight = x;
		}, 0f, 0.45f));
	}

	// Token: 0x0600055A RID: 1370 RVA: 0x0001E95E File Offset: 0x0001CB5E
	public void StartSugarRush()
	{
		DOTween.To(() => this.sugarRushVolume.weight, delegate(float x)
		{
			this.sugarRushVolume.weight = x;
		}, 1f, 0.5f);
		GUIManager.instance.bar.AddRainbow();
	}

	// Token: 0x0600055B RID: 1371 RVA: 0x0001E997 File Offset: 0x0001CB97
	public void EndSugarRush()
	{
		DOTween.To(() => this.sugarRushVolume.weight, delegate(float x)
		{
			this.sugarRushVolume.weight = x;
		}, 0f, 0.5f);
		GUIManager.instance.bar.RemoveRainbow();
	}

	// Token: 0x0600055C RID: 1372 RVA: 0x0001E9D0 File Offset: 0x0001CBD0
	public void StartEnergyDrink()
	{
		this.energySVFX.StartFX();
	}

	// Token: 0x0600055D RID: 1373 RVA: 0x0001E9DD File Offset: 0x0001CBDD
	public void EndEnergyDrink()
	{
		this.energySVFX.EndFX();
	}

	// Token: 0x0600055E RID: 1374 RVA: 0x0001E9EA File Offset: 0x0001CBEA
	private void HeatFX(float amount)
	{
		amount = 1f;
		this.heatSVFX.Play(amount);
	}

	// Token: 0x0600055F RID: 1375 RVA: 0x0001E9FF File Offset: 0x0001CBFF
	public void StartHeat()
	{
		this.heatSVFX.StartFX();
	}

	// Token: 0x06000560 RID: 1376 RVA: 0x0001EA0C File Offset: 0x0001CC0C
	public void EndHeat()
	{
		this.heatSVFX.EndFX();
	}

	// Token: 0x06000561 RID: 1377 RVA: 0x0001EA1C File Offset: 0x0001CC1C
	private void OnInteractChange()
	{
		if (this.currentInteractable.UnityObjectExists<IInteractible>())
		{
			this.currentInteractable.HoverExit();
		}
		this.currentInteractable = Interaction.instance.currentHovered;
		if (this.currentInteractable.UnityObjectExists<IInteractible>())
		{
			this.currentInteractable.HoverEnter();
		}
		this.RefreshInteractablePrompt();
	}

	// Token: 0x06000562 RID: 1378 RVA: 0x0001EA70 File Offset: 0x0001CC70
	public void RefreshInteractablePrompt()
	{
		if (this.currentInteractable.UnityObjectExists<IInteractible>())
		{
			this.interactPromptText.text = this.currentInteractable.GetInteractionText();
			this.interactName.SetActive(true);
			this.interactPromptPrimary.SetActive(true);
			this.interactPromptSecondary.SetActive(false);
			this.interactPromptHold.SetActive(false);
			if (this.currentInteractable is Item)
			{
				this.interactNameText.text = ((Item)this.currentInteractable).GetItemName(null);
			}
			else
			{
				CharacterInteractible characterInteractible = this.currentInteractable as CharacterInteractible;
				if (characterInteractible != null)
				{
					this.interactPromptPrimary.SetActive(characterInteractible.IsPrimaryInteractible(Character.localCharacter));
					this.interactName.SetActive(false);
					if (characterInteractible.IsSecondaryInteractible(Character.localCharacter))
					{
						this.interactPromptSecondary.SetActive(true);
						this.secondaryInteractPromptText.text = characterInteractible.GetSecondaryInteractionText();
					}
				}
				else
				{
					this.interactNameText.text = this.currentInteractable.GetName();
				}
			}
		}
		else
		{
			this.interactName.SetActive(false);
			this.interactPromptPrimary.SetActive(false);
			this.interactPromptSecondary.SetActive(false);
			this.interactPromptHold.SetActive(false);
		}
		if (Character.localCharacter && Character.localCharacter.data.climbingSpikeCount > 0 && Character.localCharacter.data.isClimbing)
		{
			this.interactPromptSecondary.SetActive(true);
			this.secondaryInteractPromptText.text = "set piton";
		}
	}

	// Token: 0x06000563 RID: 1379 RVA: 0x0001EBF1 File Offset: 0x0001CDF1
	public void EnableBinocularOverlay()
	{
		bool enabled = this.binocularOverlay.enabled;
		this.binocularOverlay.enabled = true;
		this.sinceShowedBinocularOverlay = 0;
		this.hudCanvasGroup.DOFade(0f, 0.5f);
	}

	// Token: 0x06000564 RID: 1380 RVA: 0x0001EC28 File Offset: 0x0001CE28
	private void UpdateBinocularOverlay()
	{
		if (this.sinceShowedBinocularOverlay > 0)
		{
			this.binocularOverlay.enabled = false;
			this.hudCanvasGroup.DOFade(1f, 0.5f);
		}
		this.sinceShowedBinocularOverlay++;
	}

	// Token: 0x06000565 RID: 1381 RVA: 0x0001EC63 File Offset: 0x0001CE63
	public void BlurBinoculars()
	{
	}

	// Token: 0x06000566 RID: 1382 RVA: 0x0001EC68 File Offset: 0x0001CE68
	public void UpdateItems()
	{
		if (Character.observedCharacter == null)
		{
			return;
		}
		if (Character.observedCharacter == null || Character.observedCharacter.player == null)
		{
			for (int i = 0; i < this.items.Length; i++)
			{
				this.items[i].SetItem(null);
			}
			this.backpack.SetItem(null);
			this.UpdateItemPrompts();
			this.temporaryItem.gameObject.SetActive(false);
			return;
		}
		for (int j = 0; j < this.items.Length; j++)
		{
			if (j < Character.observedCharacter.player.itemSlots.Length)
			{
				this.items[j].SetItem(Character.observedCharacter.player.itemSlots[j]);
			}
		}
		this.backpack.SetItem(Character.observedCharacter.player.backpackSlot);
		if (!Character.observedCharacter.player.GetItemSlot(250).IsEmpty())
		{
			this.temporaryItem.gameObject.SetActive(true);
			this.temporaryItem.SetItem(Character.observedCharacter.player.GetItemSlot(250));
		}
		else
		{
			this.temporaryItem.gameObject.SetActive(false);
		}
		this.UpdateItemPrompts();
		this.bar.ChangeBar();
	}

	// Token: 0x06000567 RID: 1383 RVA: 0x0001EDB5 File Offset: 0x0001CFB5
	public void PlayDayNightText(int x)
	{
	}

	// Token: 0x06000568 RID: 1384 RVA: 0x0001EDB8 File Offset: 0x0001CFB8
	private void TestUpdateItemPrompts()
	{
		if (!Character.localCharacter || !Character.localCharacter.data.currentItem)
		{
			this.canUsePrimaryPrevious = false;
			this.canUseSecondaryPrevious = false;
			return;
		}
		bool flag = Character.localCharacter.data.currentItem.CanUsePrimary();
		bool flag2 = Character.localCharacter.data.currentItem.CanUseSecondary();
		if (flag != this.canUsePrimaryPrevious || flag2 != this.canUseSecondaryPrevious)
		{
			this.UpdateItemPrompts();
		}
		this.canUsePrimaryPrevious = flag;
		this.canUsePrimaryPrevious = flag2;
	}

	// Token: 0x06000569 RID: 1385 RVA: 0x0001EE48 File Offset: 0x0001D048
	public void UpdateItemPrompts()
	{
		if (Character.localCharacter != null && Character.localCharacter.data.currentItem)
		{
			Item currentItem = Character.localCharacter.data.currentItem;
			Item.ItemUIData uidata = currentItem.UIData;
			this.itemPromptMain.text = this.GetMainInteractPrompt(currentItem);
			this.itemPromptSecondary.text = this.GetSecondaryInteractPrompt(currentItem);
			this.itemPromptScroll.text = uidata.scrollInteractPrompt;
			this.itemPromptMain.gameObject.SetActive(uidata.hasMainInteract && Character.localCharacter.data.currentItem.CanUsePrimary());
			this.itemPromptSecondary.gameObject.SetActive(uidata.hasSecondInteract && Character.localCharacter.data.currentItem.CanUseSecondary());
			this.itemPromptScroll.gameObject.SetActive(uidata.hasScrollingInteract);
			this.itemPromptDrop.gameObject.SetActive(uidata.canDrop);
			this.itemPromptThrow.gameObject.SetActive(uidata.canThrow);
			return;
		}
		this.itemPromptMain.gameObject.SetActive(false);
		this.itemPromptSecondary.gameObject.SetActive(false);
		this.itemPromptScroll.gameObject.SetActive(false);
		this.itemPromptDrop.gameObject.SetActive(false);
		this.itemPromptThrow.gameObject.SetActive(false);
	}

	// Token: 0x0600056A RID: 1386 RVA: 0x0001EFC0 File Offset: 0x0001D1C0
	public void TheFogRises()
	{
		this.fogRises.SetActive(true);
		base.StartCoroutine(this.<TheFogRises>g__FogRisesRoutine|179_0());
	}

	// Token: 0x0600056B RID: 1387 RVA: 0x0001EFDB File Offset: 0x0001D1DB
	private string GetMainInteractPrompt(Item item)
	{
		return item.UIData.mainInteractPrompt;
	}

	// Token: 0x0600056C RID: 1388 RVA: 0x0001EFE8 File Offset: 0x0001D1E8
	public string GetSecondaryInteractPrompt(Item item)
	{
		return item.UIData.secondaryInteractPrompt;
	}

	// Token: 0x0600056D RID: 1389 RVA: 0x0001EFF5 File Offset: 0x0001D1F5
	public void TriggerMenuWindowOpened(MenuWindow window)
	{
		GUIManager.MenuWindowEvent onMenuWindowOpened = this.OnMenuWindowOpened;
		if (onMenuWindowOpened == null)
		{
			return;
		}
		onMenuWindowOpened(window);
	}

	// Token: 0x0600056E RID: 1390 RVA: 0x0001F008 File Offset: 0x0001D208
	public void TriggerMenuWindowClosed(MenuWindow window)
	{
		GUIManager.MenuWindowEvent onMenuWindowClosed = this.OnMenuWindowClosed;
		if (onMenuWindowClosed == null)
		{
			return;
		}
		onMenuWindowClosed(window);
	}

	// Token: 0x06000570 RID: 1392 RVA: 0x0001F051 File Offset: 0x0001D251
	[CompilerGenerated]
	private IEnumerator <SetHeroTitle>g__HeroRoutine|125_0(string heroString)
	{
		this.heroCanvasObject.gameObject.SetActive(true);
		yield return null;
		string dayString = DayNightManager.instance.DayCountString();
		string timeOfDayString = DayNightManager.instance.TimeOfDayString();
		this.heroObject.gameObject.SetActive(true);
		this.heroImage.color = new Color(this.heroImage.color.r, this.heroImage.color.g, this.heroImage.color.b, 1f);
		this.heroShadowImage.color = new Color(this.heroShadowImage.color.r, this.heroShadowImage.color.g, this.heroShadowImage.color.b, 0.12f);
		this.heroDayText.text = "";
		this.heroTimeOfDayText.text = "";
		this.heroBG.color = new Color(0f, 0f, 0f, 0f);
		this.heroBG.DOFade(0.5f, 0.5f);
		int num;
		for (int i = 0; i < heroString.Length; i = num + 1)
		{
			this.heroText.text = heroString.Substring(0, i + 1);
			this.heroCamera.Render();
			yield return new WaitForSeconds(0.1f);
			num = i;
		}
		yield return new WaitForSeconds(0.5f);
		for (int i = 0; i < dayString.Length; i = num + 1)
		{
			this.heroDayText.text = dayString.Substring(0, i + 1);
			this.heroCamera.Render();
			yield return new WaitForSeconds(0.066f);
			num = i;
		}
		yield return new WaitForSeconds(0.5f);
		for (int i = 0; i < timeOfDayString.Length; i = num + 1)
		{
			this.heroTimeOfDayText.text = timeOfDayString.Substring(0, i + 1);
			this.heroCamera.Render();
			yield return new WaitForSeconds(0.066f);
			num = i;
		}
		yield return new WaitForSeconds(1.5f);
		this.heroImage.DOFade(0f, 2f);
		this.heroShadowImage.DOFade(0f, 1f);
		this.heroBG.DOFade(0f, 2f);
		yield return new WaitForSeconds(2f);
		this.heroObject.gameObject.SetActive(false);
		this.heroCanvasObject.gameObject.SetActive(false);
		yield break;
	}

	// Token: 0x06000575 RID: 1397 RVA: 0x0001F09D File Offset: 0x0001D29D
	[CompilerGenerated]
	private IEnumerator <TheFogRises>g__FogRisesRoutine|179_0()
	{
		yield return new WaitForSeconds(4f);
		this.fogRises.SetActive(false);
		yield break;
	}

	// Token: 0x0400054E RID: 1358
	public static GUIManager instance;

	// Token: 0x0400054F RID: 1359
	public Canvas hudCanvas;

	// Token: 0x04000550 RID: 1360
	public Canvas binocularOverlay;

	// Token: 0x04000551 RID: 1361
	public Canvas letterboxCanvas;

	// Token: 0x04000552 RID: 1362
	public BoardingPass boardingPass;

	// Token: 0x04000553 RID: 1363
	public StaminaBar bar;

	// Token: 0x04000554 RID: 1364
	public InventoryItemUI[] items;

	// Token: 0x04000555 RID: 1365
	public InventoryItemUI backpack;

	// Token: 0x04000556 RID: 1366
	public InventoryItemUI temporaryItem;

	// Token: 0x04000557 RID: 1367
	public CanvasGroup hudCanvasGroup;

	// Token: 0x04000558 RID: 1368
	public Sprite emptySprite;

	// Token: 0x04000559 RID: 1369
	public UI_Rope ui_rope;

	// Token: 0x0400055A RID: 1370
	public GameObject emoteWheel;

	// Token: 0x0400055B RID: 1371
	public BackpackWheel backpackWheel;

	// Token: 0x0400055C RID: 1372
	public UIPlayerNames playerNames;

	// Token: 0x0400055D RID: 1373
	public UI_UseItemProgressFriend friendUseItemProgressPrefab;

	// Token: 0x0400055E RID: 1374
	public Transform friendProgressTF;

	// Token: 0x0400055F RID: 1375
	public GameObject fogRises;

	// Token: 0x04000560 RID: 1376
	public LoadingScreen loadingScreenPrefab;

	// Token: 0x04000561 RID: 1377
	[FormerlySerializedAs("endgameCounter")]
	public EndgameCounter endgame;

	// Token: 0x04000562 RID: 1378
	public EndScreen endScreen;

	// Token: 0x04000563 RID: 1379
	[FormerlySerializedAs("pauseOptionsMenu")]
	public MenuWindow pauseMenu;

	// Token: 0x04000564 RID: 1380
	public List<UI_UseItemProgressFriend> friendUseItemProgressList = new List<UI_UseItemProgressFriend>();

	// Token: 0x04000565 RID: 1381
	private TextMeshProUGUI text;

	// Token: 0x04000567 RID: 1383
	public GameObject interactName;

	// Token: 0x04000568 RID: 1384
	public TextMeshProUGUI interactNameText;

	// Token: 0x04000569 RID: 1385
	public GameObject interactPromptPrimary;

	// Token: 0x0400056A RID: 1386
	public GameObject interactPromptSecondary;

	// Token: 0x0400056B RID: 1387
	public GameObject interactPromptHold;

	// Token: 0x0400056C RID: 1388
	public GameObject interactPromptLunge;

	// Token: 0x0400056D RID: 1389
	public TextMeshProUGUI interactPromptText;

	// Token: 0x0400056E RID: 1390
	public TextMeshProUGUI secondaryInteractPromptText;

	// Token: 0x0400056F RID: 1391
	public TextMeshProUGUI itemPromptMain;

	// Token: 0x04000570 RID: 1392
	public TextMeshProUGUI itemPromptScroll;

	// Token: 0x04000571 RID: 1393
	public TextMeshProUGUI itemPromptSecondary;

	// Token: 0x04000572 RID: 1394
	public TextMeshProUGUI itemPromptDrop;

	// Token: 0x04000573 RID: 1395
	public TextMeshProUGUI itemPromptThrow;

	// Token: 0x04000574 RID: 1396
	public GameObject throwGO;

	// Token: 0x04000575 RID: 1397
	public Image throwBar;

	// Token: 0x04000576 RID: 1398
	public Gradient throwGradient;

	// Token: 0x04000577 RID: 1399
	public GameObject dyingBarObject;

	// Token: 0x04000578 RID: 1400
	public RectTransform dyingBarRect;

	// Token: 0x04000579 RID: 1401
	public Image dyingBarImage;

	// Token: 0x0400057A RID: 1402
	public Gradient dyingBarGradient;

	// Token: 0x0400057B RID: 1403
	public Animator dyingBarAnimator;

	// Token: 0x0400057C RID: 1404
	public GameObject spectatingObject;

	// Token: 0x0400057D RID: 1405
	public TextMeshProUGUI spectatingNameText;

	// Token: 0x0400057E RID: 1406
	public Color spectatingNameColor;

	// Token: 0x0400057F RID: 1407
	public Color spectatingYourselfColor;

	// Token: 0x04000580 RID: 1408
	public GameObject heroObject;

	// Token: 0x04000581 RID: 1409
	public GameObject heroCanvasObject;

	// Token: 0x04000582 RID: 1410
	public Camera heroCamera;

	// Token: 0x04000583 RID: 1411
	public Image heroBG;

	// Token: 0x04000584 RID: 1412
	public RawImage heroImage;

	// Token: 0x04000585 RID: 1413
	public RawImage heroShadowImage;

	// Token: 0x04000586 RID: 1414
	public TextMeshProUGUI heroText;

	// Token: 0x04000587 RID: 1415
	public TextMeshProUGUI heroDayText;

	// Token: 0x04000588 RID: 1416
	public TextMeshProUGUI heroTimeOfDayText;

	// Token: 0x04000589 RID: 1417
	public AudioSource stingerSound;

	// Token: 0x0400058A RID: 1418
	public Volume blurVolume;

	// Token: 0x0400058B RID: 1419
	public Volume coldVolume;

	// Token: 0x0400058C RID: 1420
	public Volume sugarRushVolume;

	// Token: 0x0400058D RID: 1421
	public ScreenVFX injurySVFX;

	// Token: 0x0400058E RID: 1422
	public ScreenVFX coldSVFX;

	// Token: 0x0400058F RID: 1423
	public ScreenVFX poisonSVFX;

	// Token: 0x04000590 RID: 1424
	public ScreenVFX sugarRushSVFX;

	// Token: 0x04000591 RID: 1425
	public ScreenVFX hotSVFX;

	// Token: 0x04000592 RID: 1426
	public ScreenVFX energySVFX;

	// Token: 0x04000593 RID: 1427
	public ScreenVFX drowsyFX;

	// Token: 0x04000594 RID: 1428
	public ScreenVFX heatSVFX;

	// Token: 0x04000595 RID: 1429
	public ScreenVFX curseSVFX;

	// Token: 0x04000596 RID: 1430
	private Character character;

	// Token: 0x04000597 RID: 1431
	public GameObject reticleDefault;

	// Token: 0x04000598 RID: 1432
	public GameObject reticleX;

	// Token: 0x04000599 RID: 1433
	public GameObject reticleClimb;

	// Token: 0x0400059A RID: 1434
	public GameObject reticleClimbJump;

	// Token: 0x0400059B RID: 1435
	public GameObject reticleThrow;

	// Token: 0x0400059C RID: 1436
	public GameObject reticleReach;

	// Token: 0x0400059D RID: 1437
	public GameObject reticleGrasp;

	// Token: 0x0400059E RID: 1438
	public GameObject reticleSpike;

	// Token: 0x0400059F RID: 1439
	public GameObject reticleRope;

	// Token: 0x040005A0 RID: 1440
	public GameObject reticleClimbTry;

	// Token: 0x040005A1 RID: 1441
	public GameObject reticleVine;

	// Token: 0x040005A2 RID: 1442
	public GameObject reticleBoost;

	// Token: 0x040005A3 RID: 1443
	public GameObject reticleShoot;

	// Token: 0x040005A4 RID: 1444
	public Image reticleDefaultImage;

	// Token: 0x040005A5 RID: 1445
	public Color reticleColorDefault;

	// Token: 0x040005A6 RID: 1446
	public Color reticleColorHighlight;

	// Token: 0x040005A7 RID: 1447
	private Coroutine _heroRoutine;

	// Token: 0x040005A9 RID: 1449
	public BadgeManager mainBadgeManager;

	// Token: 0x040005AC RID: 1452
	private bool wasPitonClimbing;

	// Token: 0x040005AF RID: 1455
	private int lastBlockedInput;

	// Token: 0x040005B0 RID: 1456
	private bool dead;

	// Token: 0x040005B1 RID: 1457
	private Character currentSpecCharacter;

	// Token: 0x040005B2 RID: 1458
	private int ROPE_INVERT = Shader.PropertyToID("Invert");

	// Token: 0x040005B3 RID: 1459
	private float reticleLock;

	// Token: 0x040005B4 RID: 1460
	private GameObject lastReticle;

	// Token: 0x040005B5 RID: 1461
	private List<GameObject> reticleList = new List<GameObject>();

	// Token: 0x040005B6 RID: 1462
	private Sequence injurySequence;

	// Token: 0x040005B7 RID: 1463
	private Sequence hungerSequence;

	// Token: 0x040005B8 RID: 1464
	private Sequence coldSequence;

	// Token: 0x040005B9 RID: 1465
	private Sequence poisonSequence;

	// Token: 0x040005BA RID: 1466
	public int sinceShowedBinocularOverlay = 10;

	// Token: 0x040005BB RID: 1467
	private bool canUsePrimaryPrevious;

	// Token: 0x040005BC RID: 1468
	private bool canUseSecondaryPrevious;

	// Token: 0x02000318 RID: 792
	// (Invoke) Token: 0x060012C7 RID: 4807
	public delegate void MenuWindowEvent(MenuWindow window);
}
