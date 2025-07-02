using System;
using UnityEngine;

// Token: 0x0200003C RID: 60
public class Arrow : MonoBehaviour
{
	// Token: 0x060002F0 RID: 752 RVA: 0x00012EF6 File Offset: 0x000110F6
	private void Start()
	{
	}

	// Token: 0x060002F1 RID: 753 RVA: 0x00012EF8 File Offset: 0x000110F8
	public void stuckArrow(bool stuck)
	{
		this.isStuck = stuck;
		this.arrowRB.isKinematic = stuck;
		this.arrowCollider.enabled = !stuck;
	}

	// Token: 0x060002F2 RID: 754 RVA: 0x00012F1C File Offset: 0x0001111C
	private void Update()
	{
		if (base.transform.parent == null && this.isStuck)
		{
			this.stuckArrow(false);
		}
	}

	// Token: 0x060002F3 RID: 755 RVA: 0x00012F40 File Offset: 0x00011140
	private void OnDrawGizmosSelected()
	{
	}

	// Token: 0x04000397 RID: 919
	public bool isStuck = true;

	// Token: 0x04000398 RID: 920
	public Rigidbody arrowRB;

	// Token: 0x04000399 RID: 921
	public Collider arrowCollider;
}
