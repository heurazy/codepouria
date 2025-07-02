using System;
using UnityEngine;

// Token: 0x0200013C RID: 316
public class SimpleDrawMesh : MonoBehaviour
{
	// Token: 0x06000923 RID: 2339 RVA: 0x0002E543 File Offset: 0x0002C743
	private void Start()
	{
		this.GatherPools();
	}

	// Token: 0x06000924 RID: 2340 RVA: 0x0002E54B File Offset: 0x0002C74B
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(this.distanceCheckObject.position, this.cullDistance);
	}

	// Token: 0x06000925 RID: 2341 RVA: 0x0002E56D File Offset: 0x0002C76D
	private void Update()
	{
		this.drawMeshes();
	}

	// Token: 0x06000926 RID: 2342 RVA: 0x0002E578 File Offset: 0x0002C778
	public void drawMeshes()
	{
		if (!this.poolsGathered)
		{
			return;
		}
		if (Character.localCharacter && this.distanceCheckObject && Vector3.Distance(Character.localCharacter.Center, this.distanceCheckObject.position) > this.cullDistance)
		{
			return;
		}
		for (int i = 0; i < this.drawPools.Length; i++)
		{
			Graphics.DrawMeshInstanced(this.drawPools[i].mesh, 0, this.drawPools[i].material, this.drawPools[i].matricies, this.drawPools[i].matricies.Length);
		}
	}

	// Token: 0x06000927 RID: 2343 RVA: 0x0002E61C File Offset: 0x0002C81C
	public void GatherPools()
	{
		for (int i = 0; i < this.drawPools.Length; i++)
		{
			Transform[] componentsInChildren = this.drawPools[i].transformsParent.GetComponentsInChildren<Transform>();
			this.drawPools[i].matricies = new Matrix4x4[componentsInChildren.Length];
			for (int j = 1; j < componentsInChildren.Length; j++)
			{
				this.drawPools[i].matricies[j] = Matrix4x4.TRS(componentsInChildren[j].position, componentsInChildren[j].rotation, componentsInChildren[j].localScale);
			}
		}
		this.poolsGathered = true;
	}

	// Token: 0x04000823 RID: 2083
	public DrawPool[] drawPools;

	// Token: 0x04000824 RID: 2084
	private bool poolsGathered;

	// Token: 0x04000825 RID: 2085
	private Matrix4x4[] matrices;

	// Token: 0x04000826 RID: 2086
	public float cullDistance = 10f;

	// Token: 0x04000827 RID: 2087
	public Transform distanceCheckObject;
}
