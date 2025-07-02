using System;
using UnityEngine;

// Token: 0x02000071 RID: 113
public class DebugFogPoints : MonoBehaviour
{
	// Token: 0x06000414 RID: 1044 RVA: 0x00017B0B File Offset: 0x00015D0B
	private void Start()
	{
	}

	// Token: 0x06000415 RID: 1045 RVA: 0x00017B0D File Offset: 0x00015D0D
	private void Update()
	{
		this.fogRenderer.material.SetVector("_FogCenter", this.fogPoint.position);
	}

	// Token: 0x0400046A RID: 1130
	public Transform fogPoint;

	// Token: 0x0400046B RID: 1131
	public Renderer fogRenderer;
}
