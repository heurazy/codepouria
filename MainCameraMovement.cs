using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Zorro.Core;

// Token: 0x02000020 RID: 32
[DefaultExecutionOrder(500)]
public class MainCameraMovement : Singleton<MainCameraMovement>
{
	// Token: 0x17000025 RID: 37
	// (get) Token: 0x06000219 RID: 537 RVA: 0x0000F161 File Offset: 0x0000D361
	// (set) Token: 0x0600021A RID: 538 RVA: 0x0000F168 File Offset: 0x0000D368
	public static Character specCharacter { get; protected set; }

	// Token: 0x0600021B RID: 539 RVA: 0x0000F170 File Offset: 0x0000D370
	private void Start()
	{
		this.cam = base.GetComponent<MainCamera>();
		this.currentFov = this.cam.cam.fieldOfView;
		this.fovSetting = GameHandler.Instance.SettingsHandler.GetSetting<FovSetting>();
	}

	// Token: 0x17000026 RID: 38
	// (get) Token: 0x0600021C RID: 540 RVA: 0x0000F1A9 File Offset: 0x0000D3A9
	public static bool IsSpectating
	{
		get
		{
			return Singleton<MainCameraMovement>.Instance.isSpectating;
		}
	}

	// Token: 0x0600021D RID: 541 RVA: 0x0000F1B8 File Offset: 0x0000D3B8
	private void LateUpdate()
	{
		if (this.isGodCam)
		{
			this.godcam.Update(base.transform, this.cam);
			return;
		}
		this.UpdateVariables();
		if (this.cam.camOverride)
		{
			this.OverrideCam();
			return;
		}
		if (Character.localCharacter && Character.localCharacter.data.fullyPassedOut)
		{
			this.Spectate();
			if (!this.isSpectating)
			{
				this.StartSpectate();
			}
			return;
		}
		if (this.isSpectating)
		{
			this.StopSpectating();
		}
		MainCameraMovement.specCharacter = null;
		this.CharacterCam();
	}

	// Token: 0x0600021E RID: 542 RVA: 0x0000F250 File Offset: 0x0000D450
	private void StartSpectate()
	{
		this.isSpectating = true;
	}

	// Token: 0x0600021F RID: 543 RVA: 0x0000F259 File Offset: 0x0000D459
	private void StopSpectating()
	{
		this.isSpectating = false;
		if (Character.localCharacter.Ghost != null)
		{
			PhotonNetwork.Destroy(Character.localCharacter.Ghost.gameObject);
		}
	}

	// Token: 0x06000220 RID: 544 RVA: 0x0000F288 File Offset: 0x0000D488
	private void UpdateVariables()
	{
		this.sinceSwitch += Time.deltaTime;
	}

	// Token: 0x06000221 RID: 545 RVA: 0x0000F29C File Offset: 0x0000D49C
	private void Spectate()
	{
		Character specCharacter = MainCameraMovement.specCharacter;
		if (!this.HandleSpecSelection())
		{
			this.NoOneToSpectate();
			return;
		}
		PlayerGhost playerGhost = Character.localCharacter.Ghost;
		if (playerGhost == null && Character.localCharacter.data.dead)
		{
			playerGhost = PhotonNetwork.Instantiate("PlayerGhost", Vector3.zero, Quaternion.identity, 0, null).GetComponent<PlayerGhost>();
			playerGhost.m_view.RPC("RPCA_InitGhost", RpcTarget.AllBuffered, new object[]
			{
				Character.localCharacter.refs.view,
				MainCameraMovement.specCharacter.refs.view
			});
		}
		if (playerGhost && playerGhost.m_target != MainCameraMovement.specCharacter)
		{
			playerGhost.m_view.RPC("RPCA_SetTarget", RpcTarget.AllBuffered, new object[] { MainCameraMovement.specCharacter.refs.view });
		}
		base.transform.position = MainCameraMovement.specCharacter.Center;
		Vector3 vector = MainCameraMovement.specCharacter.data.lookDirection;
		if (Character.localCharacter != null)
		{
			vector = Character.localCharacter.data.lookDirection;
		}
		base.transform.rotation = Quaternion.LookRotation(vector);
		this.spectateZoom += Character.localCharacter.input.scrollInput * -0.5f;
		this.spectateZoom = Mathf.Clamp(this.spectateZoom, this.spectateZoomMin, this.spectateZoomMax);
		Character.localCharacter.data.spectateZoom = Mathf.Lerp(Character.localCharacter.data.spectateZoom, this.spectateZoom, Time.deltaTime * 5f);
		base.transform.position += base.transform.TransformDirection(new Vector3(0f, 0.5f, -1f * Character.localCharacter.data.spectateZoom));
	}

	// Token: 0x06000222 RID: 546 RVA: 0x0000F488 File Offset: 0x0000D688
	private void NoOneToSpectate()
	{
	}

	// Token: 0x06000223 RID: 547 RVA: 0x0000F48C File Offset: 0x0000D68C
	private bool HandleSpecSelection()
	{
		if (MainCameraMovement.specCharacter && MainCameraMovement.specCharacter.data.dead)
		{
			MainCameraMovement.specCharacter = null;
		}
		if (MainCameraMovement.specCharacter == null)
		{
			this.GetSpecPlayer();
		}
		if (MainCameraMovement.specCharacter == null)
		{
			return false;
		}
		if (Character.localCharacter.input.spectateLeftWasPressed && this.sinceSwitch > 0.2f)
		{
			Transitions.instance.PlayTransition(TransitionType.SpectateSwitch, new Action(this.SwapSpecPlayerLeft), 5f, 5f);
			this.sinceSwitch = 0f;
		}
		if (Character.localCharacter.input.spectateRightWasPressed && this.sinceSwitch > 0.2f)
		{
			Transitions.instance.PlayTransition(TransitionType.SpectateSwitch, new Action(this.SwapSpecPlayerRight), 5f, 5f);
			this.sinceSwitch = 0f;
		}
		return !(MainCameraMovement.specCharacter == null);
	}

	// Token: 0x06000224 RID: 548 RVA: 0x0000F582 File Offset: 0x0000D782
	public void SwapSpecPlayerLeft()
	{
		this.SwapSpecPlayer(-1);
	}

	// Token: 0x06000225 RID: 549 RVA: 0x0000F58B File Offset: 0x0000D78B
	public void SwapSpecPlayerRight()
	{
		this.SwapSpecPlayer(1);
	}

	// Token: 0x06000226 RID: 550 RVA: 0x0000F594 File Offset: 0x0000D794
	private void SwapSpecPlayer(int add)
	{
		List<Character> list = new List<Character>();
		foreach (Character character in PlayerHandler.GetAllPlayerCharacters())
		{
			if (!character.data.dead && !character.isBot)
			{
				list.Add(character);
			}
		}
		if (list.Count == 0)
		{
			MainCameraMovement.specCharacter = null;
			return;
		}
		int num = MainCameraMovement.specCharacter.GetPlayerListID(list);
		num += add;
		if (num < 0)
		{
			num = list.Count - 1;
		}
		if (num >= list.Count)
		{
			num = 0;
		}
		MainCameraMovement.specCharacter = list[num];
	}

	// Token: 0x06000227 RID: 551 RVA: 0x0000F644 File Offset: 0x0000D844
	private void GetSpecPlayer()
	{
		List<Character> allPlayerCharacters = PlayerHandler.GetAllPlayerCharacters();
		if (allPlayerCharacters.Count == 0)
		{
			return;
		}
		for (int i = 0; i < allPlayerCharacters.Count; i++)
		{
			if (!allPlayerCharacters[i].data.dead && !allPlayerCharacters[i].isBot)
			{
				MainCameraMovement.specCharacter = allPlayerCharacters[i];
				return;
			}
		}
	}

	// Token: 0x06000228 RID: 552 RVA: 0x0000F6A0 File Offset: 0x0000D8A0
	private void CharacterCam()
	{
		if (Character.localCharacter == null)
		{
			return;
		}
		this.cam.cam.fieldOfView = this.GetFov();
		if (Character.localCharacter == null)
		{
			return;
		}
		if (Character.localCharacter == null)
		{
			return;
		}
		if (Character.localCharacter.data.lookDirection != Vector3.zero)
		{
			base.transform.rotation = Quaternion.LookRotation(Character.localCharacter.data.lookDirection);
			float num = 1f - Character.localCharacter.data.currentRagdollControll;
			if (num > this.ragdollCam)
			{
				this.ragdollCam = Mathf.Lerp(this.ragdollCam, num, Time.deltaTime * 5f);
			}
			else
			{
				this.ragdollCam = Mathf.Lerp(this.ragdollCam, num, Time.deltaTime * 0.5f);
			}
			this.physicsRot = Quaternion.Lerp(this.physicsRot, Character.localCharacter.GetBodypartRig(BodypartType.Head).transform.rotation, Time.deltaTime * 10f);
			base.transform.rotation = Quaternion.Lerp(base.transform.rotation, this.physicsRot, this.ragdollCam);
			base.transform.Rotate(GamefeelHandler.instance.GetRotation(), Space.World);
		}
		Vector3 cameraPos = Character.localCharacter.GetCameraPos(this.GetHeadOffset());
		Vector3 position = Character.localCharacter.GetBodypart(BodypartType.Torso).transform.position;
		this.targetPlayerPovPosition = Vector3.Lerp(cameraPos, position, this.ragdollCam);
		if (Vector3.Distance(base.transform.position, this.targetPlayerPovPosition) > this.characterPovMaxDistance)
		{
			base.transform.position = this.targetPlayerPovPosition + (base.transform.position - this.targetPlayerPovPosition).normalized * this.characterPovMaxDistance;
		}
		base.transform.position = Vector3.Lerp(base.transform.position, this.targetPlayerPovPosition, Time.deltaTime * this.characterPovLerpRate);
	}

	// Token: 0x06000229 RID: 553 RVA: 0x0000F8B8 File Offset: 0x0000DAB8
	private void OverrideCam()
	{
		this.cam.cam.fieldOfView = this.cam.camOverride.fov;
		this.cam.transform.position = this.cam.camOverride.transform.position;
		this.cam.transform.rotation = this.cam.camOverride.transform.rotation;
	}

	// Token: 0x0600022A RID: 554 RVA: 0x0000F930 File Offset: 0x0000DB30
	private float GetHeadOffset()
	{
		if (Character.localCharacter.data.isClimbing)
		{
			this.currentForwardOffset = Mathf.Lerp(this.currentForwardOffset, -0.5f, Time.deltaTime * 5f);
		}
		else
		{
			this.currentForwardOffset = Mathf.Lerp(this.currentForwardOffset, -0.5f, Time.deltaTime * 5f);
		}
		return this.currentForwardOffset;
	}

	// Token: 0x0600022B RID: 555 RVA: 0x0000F998 File Offset: 0x0000DB98
	private float GetFov()
	{
		float value = this.fovSetting.Value;
		if (Character.localCharacter == null)
		{
			return value;
		}
		this.currentFov = Mathf.Lerp(this.currentFov, value + (float)(Character.localCharacter.data.isClimbing ? 40 : 0), Time.deltaTime * 5f);
		return this.currentFov;
	}

	// Token: 0x04000209 RID: 521
	private float currentFov;

	// Token: 0x0400020A RID: 522
	private float currentForwardOffset = 0.5f;

	// Token: 0x0400020B RID: 523
	private MainCamera cam;

	// Token: 0x0400020C RID: 524
	private FovSetting fovSetting;

	// Token: 0x0400020E RID: 526
	public float characterPovLerpRate = 5f;

	// Token: 0x0400020F RID: 527
	public float characterPovMaxDistance = 0.1f;

	// Token: 0x04000210 RID: 528
	private bool isSpectating;

	// Token: 0x04000211 RID: 529
	internal bool isGodCam;

	// Token: 0x04000212 RID: 530
	public GodCam godcam;

	// Token: 0x04000213 RID: 531
	private float spectateZoom = 2f;

	// Token: 0x04000214 RID: 532
	public float spectateZoomMin = 1f;

	// Token: 0x04000215 RID: 533
	public float spectateZoomMax = 5f;

	// Token: 0x04000216 RID: 534
	private float sinceSwitch;

	// Token: 0x04000217 RID: 535
	private float ragdollCam;

	// Token: 0x04000218 RID: 536
	private Quaternion physicsRot;

	// Token: 0x04000219 RID: 537
	private Vector3 targetPlayerPovPosition;
}
