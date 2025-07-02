using System;
using UnityEngine;

// Token: 0x020000EA RID: 234
public class LazyGizmo : MonoBehaviour
{
	// Token: 0x06000702 RID: 1794 RVA: 0x00024C5C File Offset: 0x00022E5C
	private void DrawGizmos()
	{
		Gizmos.color = this.color;
		if (this.useTop)
		{
			Gizmos.DrawSphere(base.transform.position - Vector3.up * this.radius, this.radius);
			return;
		}
		Gizmos.DrawSphere(base.transform.position, this.radius);
	}

	// Token: 0x06000703 RID: 1795 RVA: 0x00024CBE File Offset: 0x00022EBE
	private void OnDrawGizmos()
	{
		if (!this.onSelected)
		{
			this.DrawGizmos();
		}
	}

	// Token: 0x06000704 RID: 1796 RVA: 0x00024CCE File Offset: 0x00022ECE
	private void OnDrawGizmosSelected()
	{
		if (this.onSelected)
		{
			this.DrawGizmos();
		}
	}

	// Token: 0x04000694 RID: 1684
	public bool onSelected = true;

	// Token: 0x04000695 RID: 1685
	public bool useTop;

	// Token: 0x04000696 RID: 1686
	public Color color;

	// Token: 0x04000697 RID: 1687
	public float radius;
}
