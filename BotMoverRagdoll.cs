using System;
using UnityEngine;

// Token: 0x0200004B RID: 75
public class BotMoverRagdoll : MonoBehaviour
{
	// Token: 0x0600036D RID: 877 RVA: 0x00014F0E File Offset: 0x0001310E
	private void Awake()
	{
		this.bot = base.GetComponent<Bot>();
		this.rig_g = base.GetComponent<Rigidbody>();
	}

	// Token: 0x0600036E RID: 878 RVA: 0x00014F28 File Offset: 0x00013128
	private void Start()
	{
	}

	// Token: 0x0600036F RID: 879 RVA: 0x00014F2C File Offset: 0x0001312C
	private void FixedUpdate()
	{
		float fixedDeltaTime = Time.fixedDeltaTime;
		this.rig_g.AddForce(base.transform.forward * (this.bot.MovementInput.y * (this.movementSpeed * fixedDeltaTime)), ForceMode.Acceleration);
		Vector3 up = Vector3.up;
		Vector3 lookDirection = this.bot.LookDirection;
		Vector3 vector = Vector3.Cross(base.transform.up, up).normalized * Vector3.Angle(base.transform.up, up);
		Vector3 vector2 = Vector3.Cross(base.transform.forward, lookDirection).normalized * Vector3.Angle(base.transform.forward, lookDirection);
		this.rig_g.angularVelocity = FRILerp.PLerp(this.rig_g.angularVelocity, (vector2 + vector) * this.rotSpring, this.rotDamp, fixedDeltaTime);
	}

	// Token: 0x040003F9 RID: 1017
	private Bot bot;

	// Token: 0x040003FA RID: 1018
	public float movementSpeed;

	// Token: 0x040003FB RID: 1019
	private Rigidbody rig_g;

	// Token: 0x040003FC RID: 1020
	private Vector3 angularVel;

	// Token: 0x040003FD RID: 1021
	public float rotSpring = 15f;

	// Token: 0x040003FE RID: 1022
	public float rotDamp = 35f;
}
