using System;
using UnityEngine;

// Token: 0x02000094 RID: 148
public abstract class GrassDataProvider : MonoBehaviour
{
	// Token: 0x06000524 RID: 1316
	public abstract bool IsDirty();

	// Token: 0x06000525 RID: 1317
	public abstract ComputeBuffer GetData();
}
