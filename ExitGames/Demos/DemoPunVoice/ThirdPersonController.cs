using System;
using UnityEngine;

namespace ExitGames.Demos.DemoPunVoice
{
	// Token: 0x020002B8 RID: 696
	public class ThirdPersonController : BaseController
	{
		// Token: 0x060010D9 RID: 4313 RVA: 0x00053604 File Offset: 0x00051804
		protected override void Move(float h, float v)
		{
			this.rigidBody.linearVelocity = v * this.speed * base.transform.forward;
			base.transform.rotation *= Quaternion.AngleAxis(this.movingTurnSpeed * h * Time.deltaTime, Vector3.up);
		}

		// Token: 0x04000F78 RID: 3960
		[SerializeField]
		private float movingTurnSpeed = 360f;
	}
}
