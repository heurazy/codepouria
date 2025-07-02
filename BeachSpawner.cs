using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200018E RID: 398
public class BeachSpawner : MonoBehaviour
{
	// Token: 0x06000AE8 RID: 2792 RVA: 0x00035F44 File Offset: 0x00034144
	private void Spawn()
	{
		this.Clear();
		int num = Random.Range(this.treeSpawnRange.x, this.treeSpawnRange.y);
		int num2 = 20;
		for (int i = 0; i < num; i++)
		{
			if (!this.TrySpawn(this.palmTrees[Random.Range(0, this.palmTrees.Length)]) && num2 > 0)
			{
				num2--;
				i--;
			}
		}
	}

	// Token: 0x06000AE9 RID: 2793 RVA: 0x00035FB0 File Offset: 0x000341B0
	private void Clear()
	{
		foreach (GameObject gameObject in this.spawned)
		{
			Object.DestroyImmediate(gameObject);
		}
		this.spawned.Clear();
	}

	// Token: 0x06000AEA RID: 2794 RVA: 0x0003600C File Offset: 0x0003420C
	private bool TrySpawn(GameObject go)
	{
		float num = Random.Range(0f, 360f);
		float num2 = Random.Range(0f, this.radius);
		Vector3 vector = new Vector3(Mathf.Cos(num), 0f, Mathf.Sin(num)) * num2 + this.treeParent.position;
		RaycastHit raycastHit;
		if (Physics.Linecast(vector + Vector3.up * 100f, vector - Vector3.up * 100f, out raycastHit, this.layerMask.value, QueryTriggerInteraction.UseGlobal))
		{
			Debug.Log(raycastHit.collider.gameObject.name, raycastHit.collider.gameObject);
			if (raycastHit.collider.gameObject.CompareTag("Sand"))
			{
				GameObject gameObject = Object.Instantiate<GameObject>(go, raycastHit.point, Quaternion.Euler(0f, Random.Range(0f, 360f), 0f));
				gameObject.transform.SetParent(this.treeParent);
				this.spawned.Add(gameObject);
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000AEB RID: 2795 RVA: 0x00036132 File Offset: 0x00034332
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = new Color(0.73f, 0.57f, 0f);
		Gizmos.DrawWireSphere(this.treeParent.position, this.radius);
	}

	// Token: 0x040009F6 RID: 2550
	public GameObject[] palmTrees;

	// Token: 0x040009F7 RID: 2551
	public float radius;

	// Token: 0x040009F8 RID: 2552
	public Vector2Int treeSpawnRange;

	// Token: 0x040009F9 RID: 2553
	public List<GameObject> spawned;

	// Token: 0x040009FA RID: 2554
	public Transform treeParent;

	// Token: 0x040009FB RID: 2555
	public LayerMask layerMask;
}
