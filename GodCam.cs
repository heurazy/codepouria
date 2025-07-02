using System;
using UnityEngine;

// Token: 0x02000014 RID: 20
[Serializable]
public class GodCam
{
	// Token: 0x06000183 RID: 387 RVA: 0x0000C4A2 File Offset: 0x0000A6A2
	public void Update(Transform transform, MainCamera cam)
	{
		this.DoOrbiting(transform, cam);
		this.DoRotation(transform, cam);
		this.DoMovement(transform, cam);
		this.DoFOV(transform, cam);
		this.DoGamefeel(transform, cam);
	}

	// Token: 0x06000184 RID: 388 RVA: 0x0000C4CC File Offset: 0x0000A6CC
	private void DoOrbiting(Transform transform, MainCamera cam)
	{
		if (!this.isOrbiting)
		{
			if (Input.GetKey(KeyCode.Mouse0) && this.canOrbit)
			{
				Character orbitCharacter = this.GetOrbitCharacter(transform, cam);
				if (orbitCharacter)
				{
					this.isOrbiting = true;
					this.orbitingCharacter = orbitCharacter;
					this.orbitingPoint = orbitCharacter.Center;
				}
				else
				{
					RaycastHit raycastHit = HelperFunctions.LineCheck(transform.position, transform.TransformPoint(Vector3.forward * 5000f), HelperFunctions.LayerType.AllPhysical, 0f, QueryTriggerInteraction.Ignore);
					if (raycastHit.transform)
					{
						this.isOrbiting = true;
						this.orbitingCharacter = null;
						this.orbitingPoint = raycastHit.point;
					}
				}
			}
		}
		else if (!Input.GetKey(KeyCode.Mouse0))
		{
			this.isOrbiting = false;
		}
		if (this.isOrbiting)
		{
			this.orbitinAmount = Mathf.MoveTowards(this.orbitinAmount, 1f, Time.unscaledDeltaTime * Mathf.Lerp(this.orbitinAmount, 1f, 0.3f));
		}
		else
		{
			this.orbitinAmount = Mathf.Lerp(this.orbitinAmount, 0f, Time.unscaledDeltaTime * 2f);
		}
		if (this.orbitinAmount > 0.001f)
		{
			if (this.orbitingCharacter)
			{
				this.orbitingPoint = this.orbitingCharacter.Center;
			}
			Vector3 normalized = (this.orbitingPoint - transform.position).normalized;
			Vector3 vector = FRILerp.Lerp(transform.forward, normalized, 2f * this.orbitinAmount, false);
			this.lookVel = FRILerp.Lerp(this.lookVel, Vector2.zero, 2f * this.orbitinAmount, false);
			this.lookData = HelperFunctions.DirectionToLook(vector);
		}
	}

	// Token: 0x06000185 RID: 389 RVA: 0x0000C694 File Offset: 0x0000A894
	private Character GetOrbitCharacter(Transform transform, MainCamera cam)
	{
		float num = 15f;
		Character character = null;
		foreach (Character character2 in Character.AllCharacters)
		{
			float num2 = Vector3.Angle(character2.Center - transform.position, transform.forward);
			if (num2 < num && HelperFunctions.LineCheck(transform.position, character2.Center, HelperFunctions.LayerType.TerrainMap, 0f, QueryTriggerInteraction.Ignore).transform == null)
			{
				num = num2;
				character = character2;
			}
		}
		return character;
	}

	// Token: 0x06000186 RID: 390 RVA: 0x0000C73C File Offset: 0x0000A93C
	private void DoGamefeel(Transform transform, MainCamera cam)
	{
		transform.Rotate(GamefeelHandler.instance.GetRotation(), Space.World);
	}

	// Token: 0x06000187 RID: 391 RVA: 0x0000C750 File Offset: 0x0000A950
	private void DoFOV(Transform transform, MainCamera cam)
	{
		float num = cam.cam.fieldOfView / 70f;
		this.targetFov += -Input.mouseScrollDelta.y * 2f * Mathf.Lerp(num, 1f, 0.25f);
		this.targetFov = Mathf.Clamp(this.targetFov, 1f, 120f);
		cam.cam.fieldOfView = Mathf.Lerp(cam.cam.fieldOfView, this.targetFov, Time.unscaledDeltaTime * 5f);
	}

	// Token: 0x06000188 RID: 392 RVA: 0x0000C7E8 File Offset: 0x0000A9E8
	private void DoMovement(Transform transform, MainCamera cam)
	{
		this.currentKeyMult = Mathf.Lerp(this.currentKeyMult, this.currentKeyMultTarget, Time.unscaledDeltaTime * 2f);
		if (Input.GetKey(KeyCode.LeftShift))
		{
			this.sprintMult = Mathf.Lerp(this.sprintMult, 10f, Time.unscaledDeltaTime * 2f);
		}
		else
		{
			this.sprintMult = Mathf.Lerp(this.sprintMult, 1f, Time.unscaledDeltaTime * 2f);
		}
		Vector3 zero = Vector3.zero;
		if (Input.GetKey(KeyCode.W))
		{
			zero.z += 1f;
		}
		if (Input.GetKey(KeyCode.S))
		{
			zero.z -= 1f;
		}
		if (Input.GetKey(KeyCode.A))
		{
			zero.x -= 1f;
		}
		if (Input.GetKey(KeyCode.D))
		{
			zero.x += 1f;
		}
		if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.E))
		{
			zero.y += 1f;
		}
		if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.Q))
		{
			zero.y -= 1f;
		}
		this.vel += transform.TransformDirection(new Vector3(zero.x, zero.y, zero.z)) * this.force * this.sprintMult * this.currentKeyMult * Time.unscaledDeltaTime;
		this.vel = FRILerp.Lerp(this.vel, Vector3.zero, this.drag, false);
		transform.position += this.vel * Time.unscaledDeltaTime;
	}

	// Token: 0x06000189 RID: 393 RVA: 0x0000C9B0 File Offset: 0x0000ABB0
	private void DoRotation(Transform transform, MainCamera cam)
	{
		float num = cam.cam.fieldOfView / 70f;
		Vector2 vector = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
		this.lookVel += vector * 0.1f * this.lookSens * num;
		this.lookVel = FRILerp.Lerp(this.lookVel, Vector2.zero, this.lookDrag, false);
		this.lookData += this.lookVel * Time.unscaledDeltaTime;
		transform.rotation = Quaternion.LookRotation(HelperFunctions.LookToDirection(new Vector3(this.lookData.x, this.lookData.y, 0f), Vector3.forward));
	}

	// Token: 0x04000184 RID: 388
	public float lookSens = 5f;

	// Token: 0x04000185 RID: 389
	public float lookDrag = 3f;

	// Token: 0x04000186 RID: 390
	public float force = 5f;

	// Token: 0x04000187 RID: 391
	public float drag = 3f;

	// Token: 0x04000188 RID: 392
	private Vector3 vel = Vector3.zero;

	// Token: 0x04000189 RID: 393
	private Vector2 lookData = Vector2.zero;

	// Token: 0x0400018A RID: 394
	private Vector2 lookVel = Vector2.zero;

	// Token: 0x0400018B RID: 395
	private bool isOrbiting;

	// Token: 0x0400018C RID: 396
	private Vector3 orbitingPoint;

	// Token: 0x0400018D RID: 397
	private Character orbitingCharacter;

	// Token: 0x0400018E RID: 398
	private float currentKeyMult = 1f;

	// Token: 0x0400018F RID: 399
	private float currentKeyMultTarget = 1f;

	// Token: 0x04000190 RID: 400
	private float sprintMult = 1f;

	// Token: 0x04000191 RID: 401
	private float targetFov = 70f;

	// Token: 0x04000192 RID: 402
	private float orbitinAmount;

	// Token: 0x04000193 RID: 403
	internal bool canOrbit = true;
}
