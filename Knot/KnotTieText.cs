using System;
using pworld.Scripts.Extensions;
using pworld.Scripts.PPhys;
using UnityEngine;

namespace Knot
{
	// Token: 0x020002D3 RID: 723
	public class KnotTieText : MonoBehaviour
	{
		// Token: 0x060011E9 RID: 4585 RVA: 0x000584A8 File Offset: 0x000566A8
		private void Awake()
		{
			this.spring = base.GetComponent<PPhysSpringBase>();
		}

		// Token: 0x060011EA RID: 4586 RVA: 0x000584B6 File Offset: 0x000566B6
		private void Start()
		{
		}

		// Token: 0x060011EB RID: 4587 RVA: 0x000584B8 File Offset: 0x000566B8
		private void Update()
		{
			this.timeAlive += Time.deltaTime;
			if (this.timeAlive > this.lifeTime)
			{
				Object.Destroy(base.gameObject);
			}
			if (this.timeAlive > this.lifeTime - 1f)
			{
				this.spring.Target = 0.ToVec();
			}
			base.transform.position += Vector3.up * (Time.deltaTime * this.velocity);
		}

		// Token: 0x04001042 RID: 4162
		public float velocity;

		// Token: 0x04001043 RID: 4163
		public PPhysSpringBase spring;

		// Token: 0x04001044 RID: 4164
		public float lifeTime;

		// Token: 0x04001045 RID: 4165
		private float timeAlive;
	}
}
