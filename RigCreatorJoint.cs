using System;
using UnityEngine;

// Token: 0x0200002C RID: 44
[ExecuteInEditMode]
public class RigCreatorJoint : MonoBehaviour
{
	// Token: 0x06000286 RID: 646 RVA: 0x000115D6 File Offset: 0x0000F7D6
	private void Awake()
	{
		if (!Application.isEditor || Application.isPlaying)
		{
			Object.Destroy(this);
		}
	}

	// Token: 0x06000287 RID: 647 RVA: 0x000115EC File Offset: 0x0000F7EC
	private ConfigurableJoint Joint()
	{
		if (!this.joint)
		{
			this.joint = base.GetComponentInParent<ConfigurableJoint>();
		}
		return this.joint;
	}

	// Token: 0x06000288 RID: 648 RVA: 0x0001160D File Offset: 0x0000F80D
	private Rigidbody Rig()
	{
		if (!this.rig)
		{
			this.rig = base.GetComponentInParent<Rigidbody>();
		}
		return this.rig;
	}

	// Token: 0x06000289 RID: 649 RVA: 0x0001162E File Offset: 0x0000F82E
	private RigCreator RigCreator()
	{
		if (!this.rigCreator)
		{
			this.rigCreator = base.GetComponentInParent<RigCreator>();
		}
		return this.rigCreator;
	}

	// Token: 0x0600028A RID: 650 RVA: 0x0001164F File Offset: 0x0000F84F
	private void Update()
	{
		if (this.spring != this.CurrentSpring())
		{
			this.SetSpring(this.spring);
			this.RigCreator().JointChanged(this, this.CurrentSpring());
		}
	}

	// Token: 0x0600028B RID: 651 RVA: 0x00011680 File Offset: 0x0000F880
	private float CurrentSpring()
	{
		return this.Joint().angularXDrive.positionSpring / (this.Rig().mass * this.RigCreator().springMultiplier);
	}

	// Token: 0x0600028C RID: 652 RVA: 0x000116B8 File Offset: 0x0000F8B8
	internal void SetSpring(float spring)
	{
		JointDrive angularXDrive = this.Joint().angularXDrive;
		angularXDrive.positionSpring = this.Rig().mass * spring * this.RigCreator().springMultiplier;
		angularXDrive.positionDamper = this.Rig().mass * spring * 0.1f * this.RigCreator().springMultiplier;
		this.Joint().angularXDrive = angularXDrive;
		this.Joint().angularYZDrive = angularXDrive;
	}

	// Token: 0x0400030E RID: 782
	public float spring;

	// Token: 0x0400030F RID: 783
	internal ConfigurableJoint joint;

	// Token: 0x04000310 RID: 784
	internal Rigidbody rig;

	// Token: 0x04000311 RID: 785
	internal RigCreator rigCreator;
}
