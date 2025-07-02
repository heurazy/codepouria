using System;
using UnityEngine;

// Token: 0x0200010B RID: 267
public class PlayerTrackParticles : MonoBehaviour
{
	// Token: 0x060007DF RID: 2015 RVA: 0x00029BFC File Offset: 0x00027DFC
	private void Start()
	{
		if (this.bounds.center != base.transform.position)
		{
			this.bounds.center = base.transform.position;
		}
	}

	// Token: 0x060007E0 RID: 2016 RVA: 0x00029C34 File Offset: 0x00027E34
	private void Update()
	{
		if (Character.localCharacter == null)
		{
			return;
		}
		this.inBounds = this.bounds.Contains(Character.localCharacter.Center);
		if (!this.inBounds)
		{
			return;
		}
		if (Vector3.Distance(this.lastPlayerPos, Character.localCharacter.Center) > this.repositionDelta)
		{
			Vector3 position = this.fx.transform.position;
			if (this.x)
			{
				position = new Vector3(Character.localCharacter.Center.x, position.y, position.z);
			}
			if (this.y)
			{
				position = new Vector3(position.x, Character.localCharacter.Center.y, position.z);
			}
			if (this.z)
			{
				position = new Vector3(position.x, position.y, Character.localCharacter.Center.z);
			}
			this.fx.transform.position = position;
			this.lastPlayerPos = Character.localCharacter.Center;
		}
	}

	// Token: 0x060007E1 RID: 2017 RVA: 0x00029D44 File Offset: 0x00027F44
	private void OnDrawGizmosSelected()
	{
		if (this.bounds.center != base.transform.position)
		{
			this.bounds.center = base.transform.position;
		}
		Gizmos.DrawWireCube(this.bounds.center, this.bounds.size);
	}

	// Token: 0x0400075B RID: 1883
	public Bounds bounds;

	// Token: 0x0400075C RID: 1884
	public GameObject fx;

	// Token: 0x0400075D RID: 1885
	[Header("Track Axis")]
	public bool x;

	// Token: 0x0400075E RID: 1886
	public bool y;

	// Token: 0x0400075F RID: 1887
	public bool z;

	// Token: 0x04000760 RID: 1888
	public float repositionDelta = 50f;

	// Token: 0x04000761 RID: 1889
	private Vector3 lastPlayerPos = Vector3.positiveInfinity;

	// Token: 0x04000762 RID: 1890
	public bool inBounds;
}
