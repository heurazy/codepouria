using System;
using UnityEngine;

// Token: 0x0200021F RID: 543
public class PrefabTester : MonoBehaviour
{
	// Token: 0x06000DE3 RID: 3555 RVA: 0x000462CF File Offset: 0x000444CF
	private void Awake()
	{
		this.instance = base.transform.GetChild(0).gameObject;
	}

	// Token: 0x06000DE4 RID: 3556 RVA: 0x000462E8 File Offset: 0x000444E8
	public void Update()
	{
		if (Input.GetKeyDown(KeyCode.T))
		{
			if (this.instance != null)
			{
				Object.Destroy(this.instance);
			}
			this.instance = Object.Instantiate<GameObject>(this.prefab, base.transform.position, base.transform.rotation);
		}
	}

	// Token: 0x04000D01 RID: 3329
	public GameObject prefab;

	// Token: 0x04000D02 RID: 3330
	public GameObject instance;
}
