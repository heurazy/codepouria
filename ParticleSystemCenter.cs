using System;
using UnityEngine;

// Token: 0x02000103 RID: 259
[RequireComponent(typeof(ParticleSystem))]
public class ParticleSystemCenter : MonoBehaviour
{
	// Token: 0x060007A2 RID: 1954 RVA: 0x00028913 File Offset: 0x00026B13
	private void Start()
	{
	}

	// Token: 0x060007A3 RID: 1955 RVA: 0x00028915 File Offset: 0x00026B15
	private void Update()
	{
		this.setPosition();
	}

	// Token: 0x060007A4 RID: 1956 RVA: 0x00028920 File Offset: 0x00026B20
	public void setPosition()
	{
		if (this.psr == null)
		{
			this.psr = base.GetComponent<ParticleSystemRenderer>();
			this.material = this.psr.material;
		}
		this.pos = base.transform.position;
		this.material.SetVector(ParticleSystemCenter.Center, this.pos);
	}

	// Token: 0x0400071B RID: 1819
	private static readonly int Center = Shader.PropertyToID("_Center");

	// Token: 0x0400071C RID: 1820
	private Vector3 pos;

	// Token: 0x0400071D RID: 1821
	public Material material;

	// Token: 0x0400071E RID: 1822
	private ParticleSystemRenderer psr;
}
