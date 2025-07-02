using System;
using UnityEngine;

// Token: 0x020001B7 RID: 439
public class DebugRopeSpawner : MonoBehaviour
{
	// Token: 0x06000C0E RID: 3086 RVA: 0x0003C6E4 File Offset: 0x0003A8E4
	public void Spawn()
	{
		for (int i = base.transform.childCount - 1; i >= 0; i--)
		{
			Object.DestroyImmediate(base.transform.GetChild(i).gameObject);
		}
		for (int j = 0; j < this.segments; j++)
		{
			GameObject gameObject = HelperFunctions.SpawnPrefab(this.ropeSegment, base.transform.position + base.transform.up * -this.spacing * (float)j, base.transform.rotation, base.transform);
			if (j > 0)
			{
				gameObject.GetComponent<ConfigurableJoint>().connectedBody = base.transform.GetChild(j - 1).GetComponent<Rigidbody>();
			}
		}
	}

	// Token: 0x04000B12 RID: 2834
	public GameObject ropeSegment;

	// Token: 0x04000B13 RID: 2835
	public int segments = 10;

	// Token: 0x04000B14 RID: 2836
	public float spacing = 0.4f;
}
