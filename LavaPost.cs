using System;
using UnityEngine;

// Token: 0x020001EA RID: 490
public class LavaPost : MonoBehaviour
{
	// Token: 0x06000CE8 RID: 3304 RVA: 0x0004081C File Offset: 0x0003EA1C
	private void Start()
	{
		this.rend = base.GetComponent<MeshRenderer>();
	}

	// Token: 0x06000CE9 RID: 3305 RVA: 0x0004082C File Offset: 0x0003EA2C
	private void LateUpdate()
	{
		if (this.lava1 == null)
		{
			return;
		}
		bool flag = MainCamera.instance.transform.position.z < this.thresholdTransform.position.z;
		if (this.firstIsActive != flag)
		{
			this.alpha = Mathf.MoveTowards(this.alpha, 0f, Time.deltaTime);
			if (this.alpha < 0.001f)
			{
				this.firstIsActive = flag;
			}
		}
		else
		{
			this.alpha = Mathf.MoveTowards(this.alpha, 1f, Time.deltaTime);
		}
		Shader.SetGlobalFloat("LavaHeight", this.firstIsActive ? this.lava1.position.y : this.lava2.position.y);
		if (MainCamera.instance.transform.position.z < this.lavaFadeIn.position.z)
		{
			this.rend.enabled = false;
		}
		else
		{
			this.rend.enabled = true;
		}
		Shader.SetGlobalFloat("LavaAlpha", this.alpha);
		Shader.SetGlobalFloat("LavaStart", this.lavaStart.position.z);
	}

	// Token: 0x04000BE5 RID: 3045
	private MeshRenderer rend;

	// Token: 0x04000BE6 RID: 3046
	public Transform lava1;

	// Token: 0x04000BE7 RID: 3047
	public Transform lava2;

	// Token: 0x04000BE8 RID: 3048
	public Transform thresholdTransform;

	// Token: 0x04000BE9 RID: 3049
	public Transform lavaFadeIn;

	// Token: 0x04000BEA RID: 3050
	public Transform lavaStart;

	// Token: 0x04000BEB RID: 3051
	private float alpha;

	// Token: 0x04000BEC RID: 3052
	private bool firstIsActive;
}
