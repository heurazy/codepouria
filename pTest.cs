using System;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x0200024E RID: 590
public class pTest : MonoBehaviour
{
	// Token: 0x06000E61 RID: 3681 RVA: 0x000481AA File Offset: 0x000463AA
	private void Awake()
	{
		this.agent = base.GetComponent<NavMeshAgent>();
		this.agent.updatePosition = false;
		this.agent.updateRotation = false;
	}

	// Token: 0x06000E62 RID: 3682 RVA: 0x000481D0 File Offset: 0x000463D0
	private void Start()
	{
	}

	// Token: 0x06000E63 RID: 3683 RVA: 0x000481D2 File Offset: 0x000463D2
	private void Update()
	{
	}

	// Token: 0x06000E64 RID: 3684 RVA: 0x000481D4 File Offset: 0x000463D4
	private void OnDrawGizmosSelected()
	{
		BoxCollider boxCollider = base.GetComponent<BoxCollider>();
		Vector3 center = boxCollider.bounds.center;
		Collider[] array = (from c in Physics.OverlapBox(center, boxCollider.bounds.extents, boxCollider.transform.rotation)
			where c != boxCollider
			select c).ToArray<Collider>();
		Debug.Log(string.Format("position: {0}, extents: {1}", center, boxCollider.bounds.extents));
		foreach (Collider collider in array)
		{
			Debug.Log("Collider: " + collider.name);
		}
		Gizmos.color = ((array.Length != 0) ? Color.red : Color.green);
		Gizmos.DrawWireCube(center, boxCollider.bounds.extents * 2f);
	}

	// Token: 0x04000D69 RID: 3433
	private NavMeshAgent agent;
}
