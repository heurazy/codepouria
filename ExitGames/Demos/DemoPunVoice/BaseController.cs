using System;
using Photon.Pun;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace ExitGames.Demos.DemoPunVoice
{
	// Token: 0x020002B1 RID: 689
	[RequireComponent(typeof(PhotonView))]
	[RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(Animator))]
	public abstract class BaseController : MonoBehaviour
	{
		// Token: 0x06001099 RID: 4249 RVA: 0x00052810 File Offset: 0x00050A10
		protected virtual void OnEnable()
		{
			ChangePOV.CameraChanged += this.ChangePOV_CameraChanged;
		}

		// Token: 0x0600109A RID: 4250 RVA: 0x00052824 File Offset: 0x00050A24
		protected virtual void OnDisable()
		{
			ChangePOV.CameraChanged -= this.ChangePOV_CameraChanged;
		}

		// Token: 0x0600109B RID: 4251 RVA: 0x00052838 File Offset: 0x00050A38
		protected virtual void ChangePOV_CameraChanged(Camera camera)
		{
			if (camera != this.ControllerCamera)
			{
				base.enabled = false;
				this.HideCamera(this.ControllerCamera);
				return;
			}
			this.ShowCamera(this.ControllerCamera);
		}

		// Token: 0x0600109C RID: 4252 RVA: 0x00052868 File Offset: 0x00050A68
		protected virtual void Start()
		{
			if (base.GetComponent<PhotonView>().IsMine)
			{
				this.Init();
				this.SetCamera();
				return;
			}
			base.enabled = false;
		}

		// Token: 0x0600109D RID: 4253 RVA: 0x0005288B File Offset: 0x00050A8B
		protected virtual void Init()
		{
			this.rigidBody = base.GetComponent<Rigidbody>();
			this.animator = base.GetComponent<Animator>();
		}

		// Token: 0x0600109E RID: 4254 RVA: 0x000528A5 File Offset: 0x00050AA5
		protected virtual void SetCamera()
		{
			this.camTrans = this.ControllerCamera.transform;
			this.camTrans.position += this.cameraDistance * base.transform.forward;
		}

		// Token: 0x0600109F RID: 4255 RVA: 0x000528E4 File Offset: 0x00050AE4
		protected virtual void UpdateAnimator(float h, float v)
		{
			bool flag = h != 0f || v != 0f;
			this.animator.SetBool("IsWalking", flag);
		}

		// Token: 0x060010A0 RID: 4256 RVA: 0x0005291C File Offset: 0x00050B1C
		protected virtual void FixedUpdate()
		{
			this.h = CrossPlatformInputManager.GetAxisRaw("Horizontal");
			this.v = CrossPlatformInputManager.GetAxisRaw("Vertical");
			this.UpdateAnimator(this.h, this.v);
			this.Move(this.h, this.v);
		}

		// Token: 0x060010A1 RID: 4257 RVA: 0x0005296D File Offset: 0x00050B6D
		protected virtual void ShowCamera(Camera camera)
		{
			if (camera != null)
			{
				camera.gameObject.SetActive(true);
			}
		}

		// Token: 0x060010A2 RID: 4258 RVA: 0x00052984 File Offset: 0x00050B84
		protected virtual void HideCamera(Camera camera)
		{
			if (camera != null)
			{
				camera.gameObject.SetActive(false);
			}
		}

		// Token: 0x060010A3 RID: 4259
		protected abstract void Move(float h, float v);

		// Token: 0x04000F4A RID: 3914
		public Camera ControllerCamera;

		// Token: 0x04000F4B RID: 3915
		protected Rigidbody rigidBody;

		// Token: 0x04000F4C RID: 3916
		protected Animator animator;

		// Token: 0x04000F4D RID: 3917
		protected Transform camTrans;

		// Token: 0x04000F4E RID: 3918
		private float h;

		// Token: 0x04000F4F RID: 3919
		private float v;

		// Token: 0x04000F50 RID: 3920
		[SerializeField]
		protected float speed = 5f;

		// Token: 0x04000F51 RID: 3921
		[SerializeField]
		private float cameraDistance;
	}
}
