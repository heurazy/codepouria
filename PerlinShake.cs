using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000210 RID: 528
public class PerlinShake : MonoBehaviour
{
	// Token: 0x06000DA6 RID: 3494 RVA: 0x00044C51 File Offset: 0x00042E51
	private void Start()
	{
	}

	// Token: 0x06000DA7 RID: 3495 RVA: 0x00044C54 File Offset: 0x00042E54
	private void Update()
	{
		Vector2 zero = Vector2.zero;
		for (int i = this.shakes.Count - 1; i >= 0; i--)
		{
			zero.x += (Mathf.PerlinNoise(Time.time * this.shakes[i].scale, 0f) - 0.5f) * this.shakes[i].amount * (this.shakes[i].duration / this.shakes[i].startDuration);
			zero.y += (Mathf.PerlinNoise(0f, Time.time * this.shakes[i].scale) - 0.5f) * this.shakes[i].amount * (this.shakes[i].duration / this.shakes[i].startDuration);
			this.shakes[i].duration -= Time.deltaTime;
			if (this.shakes[i].duration < 0f)
			{
				this.shakes.RemoveAt(i);
			}
		}
		base.transform.localEulerAngles = zero;
	}

	// Token: 0x06000DA8 RID: 3496 RVA: 0x00044DA8 File Offset: 0x00042FA8
	public void AddShake(float amount = 1f, float duration = 0.2f, float scale = 15f)
	{
		PerlinShakeInstance perlinShakeInstance = new PerlinShakeInstance();
		perlinShakeInstance.amount = amount;
		perlinShakeInstance.duration = duration;
		perlinShakeInstance.startDuration = duration;
		perlinShakeInstance.scale = scale;
		this.shakes.Add(perlinShakeInstance);
	}

	// Token: 0x06000DA9 RID: 3497 RVA: 0x00044DE4 File Offset: 0x00042FE4
	public void AddShake(Vector3 pos, float amount = 1f, float duration = 0.2f, float scale = 15f, float range = 50f)
	{
		float num = Mathf.InverseLerp(range, 0f, Vector3.Distance(MainCamera.instance.transform.position, pos));
		if (num <= 0.001f)
		{
			return;
		}
		this.AddShake(amount * num, duration * num, scale);
	}

	// Token: 0x04000CC1 RID: 3265
	public List<PerlinShakeInstance> shakes = new List<PerlinShakeInstance>();
}
