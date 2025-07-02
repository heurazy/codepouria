using System;
using UnityEngine;

// Token: 0x0200025E RID: 606
public class RotationTest : MonoBehaviour
{
	// Token: 0x06000E9C RID: 3740 RVA: 0x000495D7 File Offset: 0x000477D7
	private void Update()
	{
		base.transform.Rotate(this.refVector.up, Time.deltaTime * 90f, Space.World);
	}

	// Token: 0x04000D93 RID: 3475
	public Transform refVector;
}
