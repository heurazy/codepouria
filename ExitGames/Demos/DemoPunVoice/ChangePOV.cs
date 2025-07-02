using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ExitGames.Demos.DemoPunVoice
{
	// Token: 0x020002B3 RID: 691
	public class ChangePOV : MonoBehaviour, IMatchmakingCallbacks
	{
		// Token: 0x14000004 RID: 4
		// (add) Token: 0x060010AB RID: 4267 RVA: 0x00052A6C File Offset: 0x00050C6C
		// (remove) Token: 0x060010AC RID: 4268 RVA: 0x00052AA0 File Offset: 0x00050CA0
		public static event ChangePOV.OnCameraChanged CameraChanged;

		// Token: 0x060010AD RID: 4269 RVA: 0x00052AD3 File Offset: 0x00050CD3
		private void OnEnable()
		{
			CharacterInstantiation.CharacterInstantiated += this.OnCharacterInstantiated;
			PhotonNetwork.AddCallbackTarget(this);
		}

		// Token: 0x060010AE RID: 4270 RVA: 0x00052AEC File Offset: 0x00050CEC
		private void OnDisable()
		{
			CharacterInstantiation.CharacterInstantiated -= this.OnCharacterInstantiated;
			PhotonNetwork.RemoveCallbackTarget(this);
		}

		// Token: 0x060010AF RID: 4271 RVA: 0x00052B08 File Offset: 0x00050D08
		private void Start()
		{
			this.defaultCamera = Camera.main;
			this.initialCameraPosition = new Vector3(this.defaultCamera.transform.position.x, this.defaultCamera.transform.position.y, this.defaultCamera.transform.position.z);
			this.initialCameraRotation = new Quaternion(this.defaultCamera.transform.rotation.x, this.defaultCamera.transform.rotation.y, this.defaultCamera.transform.rotation.z, this.defaultCamera.transform.rotation.w);
			this.FirstPersonCamActivator.onClick.AddListener(new UnityAction(this.FirstPersonMode));
			this.ThirdPersonCamActivator.onClick.AddListener(new UnityAction(this.ThirdPersonMode));
			this.OrthographicCamActivator.onClick.AddListener(new UnityAction(this.OrthographicMode));
		}

		// Token: 0x060010B0 RID: 4272 RVA: 0x00052C20 File Offset: 0x00050E20
		private void OnCharacterInstantiated(GameObject character)
		{
			this.firstPersonController = character.GetComponent<FirstPersonController>();
			this.firstPersonController.enabled = false;
			this.thirdPersonController = character.GetComponent<ThirdPersonController>();
			this.thirdPersonController.enabled = false;
			this.orthographicController = character.GetComponent<OrthographicController>();
			this.ButtonsHolder.SetActive(true);
		}

		// Token: 0x060010B1 RID: 4273 RVA: 0x00052C75 File Offset: 0x00050E75
		private void FirstPersonMode()
		{
			this.ToggleMode(this.firstPersonController);
		}

		// Token: 0x060010B2 RID: 4274 RVA: 0x00052C83 File Offset: 0x00050E83
		private void ThirdPersonMode()
		{
			this.ToggleMode(this.thirdPersonController);
		}

		// Token: 0x060010B3 RID: 4275 RVA: 0x00052C91 File Offset: 0x00050E91
		private void OrthographicMode()
		{
			this.ToggleMode(this.orthographicController);
		}

		// Token: 0x060010B4 RID: 4276 RVA: 0x00052CA0 File Offset: 0x00050EA0
		private void ToggleMode(BaseController controller)
		{
			if (controller == null)
			{
				return;
			}
			if (controller.ControllerCamera == null)
			{
				return;
			}
			controller.ControllerCamera.gameObject.SetActive(true);
			controller.enabled = true;
			this.FirstPersonCamActivator.interactable = !(controller == this.firstPersonController);
			this.ThirdPersonCamActivator.interactable = !(controller == this.thirdPersonController);
			this.OrthographicCamActivator.interactable = !(controller == this.orthographicController);
			this.BroadcastChange(controller.ControllerCamera);
		}

		// Token: 0x060010B5 RID: 4277 RVA: 0x00052D38 File Offset: 0x00050F38
		private void BroadcastChange(Camera camera)
		{
			if (camera == null)
			{
				return;
			}
			if (ChangePOV.CameraChanged != null)
			{
				ChangePOV.CameraChanged(camera);
			}
		}

		// Token: 0x060010B6 RID: 4278 RVA: 0x00052D56 File Offset: 0x00050F56
		public void OnFriendListUpdate(List<FriendInfo> friendList)
		{
		}

		// Token: 0x060010B7 RID: 4279 RVA: 0x00052D58 File Offset: 0x00050F58
		public void OnCreatedRoom()
		{
		}

		// Token: 0x060010B8 RID: 4280 RVA: 0x00052D5A File Offset: 0x00050F5A
		public void OnCreateRoomFailed(short returnCode, string message)
		{
		}

		// Token: 0x060010B9 RID: 4281 RVA: 0x00052D5C File Offset: 0x00050F5C
		public void OnJoinedRoom()
		{
		}

		// Token: 0x060010BA RID: 4282 RVA: 0x00052D5E File Offset: 0x00050F5E
		public void OnJoinRoomFailed(short returnCode, string message)
		{
		}

		// Token: 0x060010BB RID: 4283 RVA: 0x00052D60 File Offset: 0x00050F60
		public void OnJoinRandomFailed(short returnCode, string message)
		{
		}

		// Token: 0x060010BC RID: 4284 RVA: 0x00052D64 File Offset: 0x00050F64
		public void OnLeftRoom()
		{
			if (this.defaultCamera)
			{
				this.defaultCamera.gameObject.SetActive(true);
			}
			this.FirstPersonCamActivator.interactable = true;
			this.ThirdPersonCamActivator.interactable = true;
			this.OrthographicCamActivator.interactable = false;
			this.defaultCamera.transform.position = this.initialCameraPosition;
			this.defaultCamera.transform.rotation = this.initialCameraRotation;
			this.ButtonsHolder.SetActive(false);
		}

		// Token: 0x04000F54 RID: 3924
		private FirstPersonController firstPersonController;

		// Token: 0x04000F55 RID: 3925
		private ThirdPersonController thirdPersonController;

		// Token: 0x04000F56 RID: 3926
		private OrthographicController orthographicController;

		// Token: 0x04000F57 RID: 3927
		private Vector3 initialCameraPosition;

		// Token: 0x04000F58 RID: 3928
		private Quaternion initialCameraRotation;

		// Token: 0x04000F59 RID: 3929
		private Camera defaultCamera;

		// Token: 0x04000F5A RID: 3930
		[SerializeField]
		private GameObject ButtonsHolder;

		// Token: 0x04000F5B RID: 3931
		[SerializeField]
		private Button FirstPersonCamActivator;

		// Token: 0x04000F5C RID: 3932
		[SerializeField]
		private Button ThirdPersonCamActivator;

		// Token: 0x04000F5D RID: 3933
		[SerializeField]
		private Button OrthographicCamActivator;

		// Token: 0x020003CA RID: 970
		// (Invoke) Token: 0x06001512 RID: 5394
		public delegate void OnCameraChanged(Camera newCamera);
	}
}
