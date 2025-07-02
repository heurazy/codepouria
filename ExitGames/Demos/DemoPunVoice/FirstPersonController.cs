using System;
using UnityEngine;

namespace ExitGames.Demos.DemoPunVoice
{
	// Token: 0x020002B5 RID: 693
	public class FirstPersonController : BaseController
	{
		// Token: 0x170000CB RID: 203
		// (get) Token: 0x060010C6 RID: 4294 RVA: 0x00053260 File Offset: 0x00051460
		public Vector3 Velocity
		{
			get
			{
				return this.rigidBody.linearVelocity;
			}
		}

		// Token: 0x060010C7 RID: 4295 RVA: 0x0005326D File Offset: 0x0005146D
		protected override void SetCamera()
		{
			base.SetCamera();
			this.mouseLook.Init(base.transform, this.camTrans);
		}

		// Token: 0x060010C8 RID: 4296 RVA: 0x0005328C File Offset: 0x0005148C
		protected override void Move(float h, float v)
		{
			Vector3 vector = this.camTrans.forward * v + this.camTrans.right * h;
			vector.x *= this.speed;
			vector.z *= this.speed;
			vector.y = 0f;
			this.rigidBody.linearVelocity = vector;
		}

		// Token: 0x060010C9 RID: 4297 RVA: 0x00053301 File Offset: 0x00051501
		private void Update()
		{
			this.RotateView();
		}

		// Token: 0x060010CA RID: 4298 RVA: 0x0005330C File Offset: 0x0005150C
		private void RotateView()
		{
			this.oldYRotation = base.transform.eulerAngles.y;
			this.mouseLook.LookRotation(base.transform, this.camTrans);
			this.velRotation = Quaternion.AngleAxis(base.transform.eulerAngles.y - this.oldYRotation, Vector3.up);
			this.rigidBody.linearVelocity = this.velRotation * this.rigidBody.linearVelocity;
		}

		// Token: 0x04000F6D RID: 3949
		[SerializeField]
		private MouseLookHelper mouseLook = new MouseLookHelper();

		// Token: 0x04000F6E RID: 3950
		private float oldYRotation;

		// Token: 0x04000F6F RID: 3951
		private Quaternion velRotation;
	}
}
