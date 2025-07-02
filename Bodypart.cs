using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000022 RID: 34
public class Bodypart : MonoBehaviour
{
	// Token: 0x17000028 RID: 40
	// (get) Token: 0x0600023B RID: 571 RVA: 0x000100DF File Offset: 0x0000E2DF
	public Rigidbody Rig
	{
		get
		{
			if (this.rig == null)
			{
				this.rig = base.GetComponent<Rigidbody>();
			}
			return this.rig;
		}
	}

	// Token: 0x0600023C RID: 572 RVA: 0x00010104 File Offset: 0x0000E304
	private void Awake()
	{
		this.startLocal = base.transform.localRotation;
		this.prevPos = base.transform.position;
		this.prevRot = base.transform.rotation;
		this.rig = base.GetComponent<Rigidbody>();
	}

	// Token: 0x0600023D RID: 573 RVA: 0x00010150 File Offset: 0x0000E350
	private void Start()
	{
		this.character = base.GetComponentInParent<Character>();
		this.joint = base.GetComponent<ConfigurableJoint>();
		if (this.joint)
		{
			this.jointParent = this.joint.connectedBody.GetComponent<Bodypart>();
		}
		if (this.rig)
		{
			this.rig.maxAngularVelocity = 50f;
		}
		this.localCenterOfMass = HelperFunctions.GetCenterOfMass(base.transform);
	}

	// Token: 0x0600023E RID: 574 RVA: 0x000101C6 File Offset: 0x0000E3C6
	internal void RegisterCollider(RigCreatorCollider rigCreatorCollider)
	{
		this.colliders.Add(rigCreatorCollider);
	}

	// Token: 0x0600023F RID: 575 RVA: 0x000101D4 File Offset: 0x0000E3D4
	internal void InitBodypart(BodypartType setPartType)
	{
		this.partType = setPartType;
	}

	// Token: 0x06000240 RID: 576 RVA: 0x000101DD File Offset: 0x0000E3DD
	private Vector3 WorldCenterOfMass()
	{
		return base.transform.position;
	}

	// Token: 0x06000241 RID: 577 RVA: 0x000101EC File Offset: 0x0000E3EC
	public void SaveAnimationData()
	{
		if (this != this.character.refs.hip)
		{
			this.targetOffsetRelativeToHip = this.WorldCenterOfMass() - this.character.refs.hip.transform.position;
		}
		this.targetRotation = base.transform.localRotation;
		this.targetForward = base.transform.forward;
		this.targetUp = base.transform.up;
	}

	// Token: 0x06000242 RID: 578 RVA: 0x0001026F File Offset: 0x0000E46F
	public void ResetTransform()
	{
		base.transform.rotation = this.rig.rotation;
		base.transform.position = this.rig.position;
	}

	// Token: 0x06000243 RID: 579 RVA: 0x0001029D File Offset: 0x0000E49D
	internal void Animate(float force, float torque)
	{
		if (this.rig.isKinematic)
		{
			this.SnapToAnim();
			return;
		}
		this.FollowRotation_Joint();
		this.FollowRotation_Rotation(torque);
		this.FollowRotation_Position(force);
	}

	// Token: 0x06000244 RID: 580 RVA: 0x000102C8 File Offset: 0x0000E4C8
	public void SnapToAnim()
	{
		Vector3 vector = this.WorldTargetPos() - this.WorldCenterOfMass();
		base.transform.position += vector;
		base.transform.rotation = Quaternion.LookRotation(this.targetForward, this.targetUp);
		if (this.rig.isKinematic)
		{
			return;
		}
		this.rig.linearVelocity *= 0f;
		this.rig.angularVelocity *= 0f;
	}

	// Token: 0x06000245 RID: 581 RVA: 0x0001035E File Offset: 0x0000E55E
	private void DrawDebug()
	{
	}

	// Token: 0x06000246 RID: 582 RVA: 0x00010360 File Offset: 0x0000E560
	private void FollowRotation_Joint()
	{
		if (!this.joint)
		{
			return;
		}
		this.joint.SetTargetRotationLocal(this.targetRotation, this.startLocal);
	}

	// Token: 0x06000247 RID: 583 RVA: 0x00010394 File Offset: 0x0000E594
	private void FollowRotation_Rotation(float torque)
	{
		if (this.rig.isKinematic)
		{
			return;
		}
		Vector3 vector = Vector3.Cross(base.transform.forward, this.targetForward).normalized * Vector3.Angle(base.transform.forward, this.targetForward);
		vector += Vector3.Cross(base.transform.up, this.targetUp).normalized * Vector3.Angle(base.transform.up, this.targetUp);
		this.rig.AddTorque(vector * torque, ForceMode.Acceleration);
	}

	// Token: 0x06000248 RID: 584 RVA: 0x0001043C File Offset: 0x0000E63C
	private void FollowRotation_Position(float force)
	{
		if (!this.character)
		{
			return;
		}
		if (this == this.character.refs.hip)
		{
			return;
		}
		if (this.targetOffsetRelativeToHip == Vector3.zero)
		{
			return;
		}
		Vector3 vector = (this.WorldTargetPos() - this.WorldCenterOfMass()) * force;
		this.AddForce(vector, ForceMode.Acceleration);
		if (this.jointParent)
		{
			Vector3 vector2 = vector * this.rig.mass;
			this.jointParent.AddForce(-vector2 * 0.5f, ForceMode.Force);
			this.character.refs.hip.AddForce(-vector2 * 0.5f, ForceMode.Force);
		}
	}

	// Token: 0x06000249 RID: 585 RVA: 0x00010504 File Offset: 0x0000E704
	private Vector3 WorldTargetPos()
	{
		return this.character.refs.hip.transform.position + this.targetOffsetRelativeToHip;
	}

	// Token: 0x0600024A RID: 586 RVA: 0x0001052C File Offset: 0x0000E72C
	internal void Drag(float drag, bool ignoreRagdoll = false)
	{
		if (!ignoreRagdoll)
		{
			drag = Mathf.Lerp(1f, drag, this.character.data.currentRagdollControll);
		}
		if (this.rig.isKinematic)
		{
			return;
		}
		this.rig.linearVelocity *= drag;
		this.rig.angularVelocity *= drag;
	}

	// Token: 0x0600024B RID: 587 RVA: 0x00010598 File Offset: 0x0000E798
	private void OnCollisionEnter(Collision collision)
	{
		if (this.character == null)
		{
			return;
		}
		if (collision.collider.transform.root == base.transform.root)
		{
			return;
		}
		Action<Collision> action = this.collisionEnterAction;
		if (action != null)
		{
			action(collision);
		}
		this.character.refs.movement.OnCollision(collision, true);
	}

	// Token: 0x0600024C RID: 588 RVA: 0x00010600 File Offset: 0x0000E800
	private void OnCollisionStay(Collision collision)
	{
		if (!this.character)
		{
			return;
		}
		if (collision.collider.transform.root == base.transform.root)
		{
			return;
		}
		Action<Collision> action = this.collisionStayAction;
		if (action != null)
		{
			action(collision);
		}
		this.character.refs.movement.OnCollision(collision, false);
	}

	// Token: 0x0600024D RID: 589 RVA: 0x00010667 File Offset: 0x0000E867
	internal void Gravity(Vector3 gravity)
	{
		this.AddForce(gravity, ForceMode.Acceleration);
	}

	// Token: 0x0600024E RID: 590 RVA: 0x00010671 File Offset: 0x0000E871
	public void AddForce(Vector3 force, ForceMode forceMode)
	{
		if (this.rig.isKinematic)
		{
			return;
		}
		if (forceMode == ForceMode.Acceleration)
		{
			force *= this.rig.mass;
		}
		this.forcesToAdd += force;
	}

	// Token: 0x0600024F RID: 591 RVA: 0x000106AA File Offset: 0x0000E8AA
	internal void ToggleUseGravity(bool useGrav)
	{
		if (this.rig.useGravity != useGrav)
		{
			this.rig.useGravity = useGrav;
		}
	}

	// Token: 0x06000250 RID: 592 RVA: 0x000106C6 File Offset: 0x0000E8C6
	internal void ApplyForces()
	{
		this.rig.AddForce(this.forcesToAdd, ForceMode.Force);
		this.forcesToAdd *= 0f;
	}

	// Token: 0x06000251 RID: 593 RVA: 0x000106F0 File Offset: 0x0000E8F0
	internal void AddMovementForce(float movementForce)
	{
		if (!this.character)
		{
			return;
		}
		Vector3 worldMovementInput_Lerp = this.character.data.worldMovementInput_Lerp;
		this.AddForce(movementForce * worldMovementInput_Lerp, ForceMode.Acceleration);
	}

	// Token: 0x06000252 RID: 594 RVA: 0x0001072C File Offset: 0x0000E92C
	internal void SetPhysicsMaterial(Bodypart.FrictionType setFrictionType, PhysicsMaterial slipperyMat, PhysicsMaterial normalMat)
	{
		foreach (RigCreatorCollider rigCreatorCollider in this.colliders)
		{
			if (this.frictionType == Bodypart.FrictionType.Grippy)
			{
				rigCreatorCollider.col.sharedMaterial = normalMat;
			}
			else if (this.frictionType == Bodypart.FrictionType.Slippery)
			{
				rigCreatorCollider.col.sharedMaterial = slipperyMat;
			}
			else if (setFrictionType == Bodypart.FrictionType.Grippy)
			{
				rigCreatorCollider.col.sharedMaterial = normalMat;
			}
			else
			{
				rigCreatorCollider.col.sharedMaterial = slipperyMat;
			}
		}
	}

	// Token: 0x04000224 RID: 548
	private Character character;

	// Token: 0x04000225 RID: 549
	public BodypartType partType;

	// Token: 0x04000226 RID: 550
	public Bodypart.FrictionType frictionType;

	// Token: 0x04000227 RID: 551
	private Rigidbody rig;

	// Token: 0x04000228 RID: 552
	internal Bodypart jointParent;

	// Token: 0x04000229 RID: 553
	private Quaternion startLocal;

	// Token: 0x0400022A RID: 554
	private Vector3 localCenterOfMass;

	// Token: 0x0400022B RID: 555
	private ConfigurableJoint joint;

	// Token: 0x0400022C RID: 556
	private Quaternion targetRotation;

	// Token: 0x0400022D RID: 557
	private Quaternion lastTargetRotation;

	// Token: 0x0400022E RID: 558
	private Vector3 targetForward;

	// Token: 0x0400022F RID: 559
	private Vector3 targetUp;

	// Token: 0x04000230 RID: 560
	private Vector3 targetOffsetRelativeToHip;

	// Token: 0x04000231 RID: 561
	internal List<RigCreatorCollider> colliders = new List<RigCreatorCollider>();

	// Token: 0x04000232 RID: 562
	public Vector3 forcesToAdd;

	// Token: 0x04000233 RID: 563
	private Vector3 prevPos;

	// Token: 0x04000234 RID: 564
	private Quaternion prevRot;

	// Token: 0x04000235 RID: 565
	public Action<Collision> collisionEnterAction;

	// Token: 0x04000236 RID: 566
	public Action<Collision> collisionStayAction;

	// Token: 0x020002F3 RID: 755
	public enum FrictionType
	{
		// Token: 0x040010D9 RID: 4313
		Unspecified,
		// Token: 0x040010DA RID: 4314
		Grippy,
		// Token: 0x040010DB RID: 4315
		Slippery
	}
}
