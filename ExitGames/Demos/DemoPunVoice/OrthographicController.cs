using System;
using UnityEngine;

namespace ExitGames.Demos.DemoPunVoice
{
	// Token: 0x020002B7 RID: 695
	public class OrthographicController : ThirdPersonController
	{
		// Token: 0x060010D4 RID: 4308 RVA: 0x00053557 File Offset: 0x00051757
		protected override void Init()
		{
			base.Init();
			this.ControllerCamera = Camera.main;
		}

		// Token: 0x060010D5 RID: 4309 RVA: 0x0005356A File Offset: 0x0005176A
		protected override void SetCamera()
		{
			base.SetCamera();
			this.offset = this.camTrans.position - base.transform.position;
		}

		// Token: 0x060010D6 RID: 4310 RVA: 0x00053593 File Offset: 0x00051793
		protected override void Move(float h, float v)
		{
			base.Move(h, v);
			this.CameraFollow();
		}

		// Token: 0x060010D7 RID: 4311 RVA: 0x000535A4 File Offset: 0x000517A4
		private void CameraFollow()
		{
			Vector3 vector = base.transform.position + this.offset;
			this.camTrans.position = Vector3.Lerp(this.camTrans.position, vector, this.smoothing * Time.deltaTime);
		}

		// Token: 0x04000F76 RID: 3958
		public float smoothing = 5f;

		// Token: 0x04000F77 RID: 3959
		private Vector3 offset;
	}
}
