using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000CD RID: 205
public class ConstructablePreview : MonoBehaviour
{
	// Token: 0x06000663 RID: 1635 RVA: 0x0002287C File Offset: 0x00020A7C
	public void SetValid(bool valid)
	{
		this.enableIfValid.SetActive(valid);
		this.enableIfInvalid.SetActive(!valid);
	}

	// Token: 0x06000664 RID: 1636 RVA: 0x0002289C File Offset: 0x00020A9C
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		foreach (ConstructablePreview.ConstructablePreviewAvoidanceSphere constructablePreviewAvoidanceSphere in this.avoidanceSpheres)
		{
			Gizmos.DrawWireSphere(base.transform.TransformPoint(constructablePreviewAvoidanceSphere.position), constructablePreviewAvoidanceSphere.radius);
		}
	}

	// Token: 0x06000665 RID: 1637 RVA: 0x00022910 File Offset: 0x00020B10
	public bool CollisionValid()
	{
		foreach (ConstructablePreview.ConstructablePreviewAvoidanceSphere constructablePreviewAvoidanceSphere in this.avoidanceSpheres)
		{
			if (Physics.CheckSphere(base.transform.TransformPoint(constructablePreviewAvoidanceSphere.position), constructablePreviewAvoidanceSphere.radius, HelperFunctions.GetMask(constructablePreviewAvoidanceSphere.layerType), QueryTriggerInteraction.Ignore))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x04000638 RID: 1592
	public GameObject enableIfValid;

	// Token: 0x04000639 RID: 1593
	public GameObject enableIfInvalid;

	// Token: 0x0400063A RID: 1594
	public List<ConstructablePreview.ConstructablePreviewAvoidanceSphere> avoidanceSpheres;

	// Token: 0x0200032A RID: 810
	[Serializable]
	public class ConstructablePreviewAvoidanceSphere
	{
		// Token: 0x040011A4 RID: 4516
		public Vector3 position;

		// Token: 0x040011A5 RID: 4517
		public float radius;

		// Token: 0x040011A6 RID: 4518
		public HelperFunctions.LayerType layerType;
	}
}
