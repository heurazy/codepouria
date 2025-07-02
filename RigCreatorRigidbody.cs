using System;
using UnityEngine;

// Token: 0x0200002D RID: 45
[ExecuteInEditMode]
public class RigCreatorRigidbody : MonoBehaviour
{
	// Token: 0x0600028E RID: 654 RVA: 0x00011737 File Offset: 0x0000F937
	private void Awake()
	{
		if (!Application.isEditor || Application.isPlaying)
		{
			Object.Destroy(this);
			return;
		}
		this.SetValues();
	}

	// Token: 0x0600028F RID: 655 RVA: 0x00011754 File Offset: 0x0000F954
	private Rigidbody Rig()
	{
		if (!this.rig)
		{
			this.rig = base.GetComponentInParent<Rigidbody>();
		}
		return this.rig;
	}

	// Token: 0x06000290 RID: 656 RVA: 0x00011775 File Offset: 0x0000F975
	private RigCreator RigCreator()
	{
		if (!this.rigCreator)
		{
			this.rigCreator = base.GetComponentInParent<RigCreator>();
		}
		return this.rigCreator;
	}

	// Token: 0x06000291 RID: 657 RVA: 0x00011796 File Offset: 0x0000F996
	private void Update()
	{
		if (this.mass != this.Rig().mass)
		{
			this.RigCreator().RigidbodyChanged(this, this.Rig().mass);
			this.SetValues();
		}
	}

	// Token: 0x06000292 RID: 658 RVA: 0x000117C8 File Offset: 0x0000F9C8
	private void SetValues()
	{
		this.mass = this.Rig().mass;
	}

	// Token: 0x04000312 RID: 786
	internal float mass;

	// Token: 0x04000313 RID: 787
	internal Rigidbody rig;

	// Token: 0x04000314 RID: 788
	internal RigCreator rigCreator;
}
