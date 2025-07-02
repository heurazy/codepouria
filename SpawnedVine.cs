using System;
using System.Collections;
using System.Runtime.CompilerServices;
using pworld.Scripts.Extensions;
using UnityEngine;

// Token: 0x0200013F RID: 319
public class SpawnedVine : MonoBehaviour
{
	// Token: 0x0600092E RID: 2350 RVA: 0x0002E751 File Offset: 0x0002C951
	private void Start()
	{
		this.vine = base.GetComponent<JungleVine>();
		this.SpawnVine();
	}

	// Token: 0x0600092F RID: 2351 RVA: 0x0002E768 File Offset: 0x0002C968
	public void SpawnVine()
	{
		if (this.startObject != null)
		{
			this.startObject.transform.position = this.vine.colliderRoot.GetChild(0).transform.position;
			Vector3 position = this.vine.GetPosition(1f);
			position = new Vector3(position.x, this.startObject.transform.position.y, position.z);
			this.startObject.transform.LookAt(position);
			if (this.endObject != null)
			{
				this.endObject.transform.forward = this.vine.colliderRoot.GetLastChild().transform.up;
			}
		}
		if (this.endObject != null)
		{
			this.endObject.transform.position = this.vine.GetPosition(1f);
		}
		base.StartCoroutine(this.<SpawnVine>g__waveFX|7_0());
	}

	// Token: 0x06000930 RID: 2352 RVA: 0x0002E86E File Offset: 0x0002CA6E
	private void Update()
	{
	}

	// Token: 0x06000933 RID: 2355 RVA: 0x0002E894 File Offset: 0x0002CA94
	[CompilerGenerated]
	private IEnumerator <SpawnVine>g__waveFX|7_0()
	{
		float normalizedTime = 0f;
		while (normalizedTime < 1f)
		{
			normalizedTime += Time.deltaTime / this.vineWaveDecay;
			float num = Mathf.Lerp(100f, 0f, normalizedTime);
			this.vineRenderer.material.SetFloat(SpawnedVine.JitterAmount, num);
			yield return null;
		}
		yield break;
	}

	// Token: 0x0400082E RID: 2094
	private static readonly int JitterAmount = Shader.PropertyToID("_JitterAmount");

	// Token: 0x0400082F RID: 2095
	private JungleVine vine;

	// Token: 0x04000830 RID: 2096
	public MeshRenderer vineRenderer;

	// Token: 0x04000831 RID: 2097
	public float vineWaveDecay = 0.5f;

	// Token: 0x04000832 RID: 2098
	public GameObject startObject;

	// Token: 0x04000833 RID: 2099
	public GameObject endObject;
}
