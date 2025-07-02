using System;
using UnityEngine;

// Token: 0x02000055 RID: 85
public class ChunkingVizBox : MonoBehaviour
{
	// Token: 0x060003B2 RID: 946 RVA: 0x000166BA File Offset: 0x000148BA
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireCube(base.transform.position, base.transform.localScale);
	}

	// Token: 0x060003B3 RID: 947 RVA: 0x000166E4 File Offset: 0x000148E4
	private void LateUpdate()
	{
		Vector3 position = MainCamera.instance.transform.position;
		Bounds bounds = new Bounds(base.transform.position, base.transform.localScale);
		bool flag = bounds.Contains(position);
		if (this.m_lastState != flag)
		{
			GameObject[] array = this.objects;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(flag);
			}
		}
		this.m_lastState = flag;
	}

	// Token: 0x04000428 RID: 1064
	public GameObject[] objects;

	// Token: 0x04000429 RID: 1065
	private bool m_lastState = true;
}
