using System;
using UnityEngine;

// Token: 0x02000277 RID: 631
public class SpawnGameObject : MonoBehaviour
{
	// Token: 0x06000F44 RID: 3908 RVA: 0x0004D1D3 File Offset: 0x0004B3D3
	public void Go()
	{
		Object.Instantiate<GameObject>(this.toSpawn, base.transform.position, base.transform.rotation);
	}

	// Token: 0x04000E2B RID: 3627
	public GameObject toSpawn;
}
