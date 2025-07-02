using System;
using System.Linq;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x0200029B RID: 667
public class WallNavigatorTest : MonoBehaviour, ISerializationCallbackReceiver
{
	// Token: 0x06000FE5 RID: 4069 RVA: 0x00050D8A File Offset: 0x0004EF8A
	private void Start()
	{
	}

	// Token: 0x06000FE6 RID: 4070 RVA: 0x00050D8C File Offset: 0x0004EF8C
	private void Update()
	{
	}

	// Token: 0x06000FE7 RID: 4071 RVA: 0x00050D90 File Offset: 0x0004EF90
	private void TryFindValidPath()
	{
		this.color = Color.red;
		if (this.triangulation.vertices.Where((Vector3 vert) => Vector3.Distance(base.transform.position, vert) < this.sphereSize).ToList<Vector3>().Count > 0)
		{
			this.color = Color.green;
		}
	}

	// Token: 0x06000FE8 RID: 4072 RVA: 0x00050DDC File Offset: 0x0004EFDC
	private void Print()
	{
		Debug.Log(string.Format("Verts{0}, Indices{1}, Areas{2}", this.triangulation.vertices.Length, this.triangulation.indices.Length, this.triangulation.areas.Length));
	}

	// Token: 0x06000FE9 RID: 4073 RVA: 0x00050E2E File Offset: 0x0004F02E
	private void OnDrawGizmosSelected()
	{
		this.TryFindValidPath();
		Gizmos.color = this.color;
		Gizmos.DrawWireSphere(base.transform.position, this.sphereSize);
	}

	// Token: 0x06000FEA RID: 4074 RVA: 0x00050E57 File Offset: 0x0004F057
	public void OnBeforeSerialize()
	{
		this.triangulation = NavMesh.CalculateTriangulation();
	}

	// Token: 0x06000FEB RID: 4075 RVA: 0x00050E64 File Offset: 0x0004F064
	public void OnAfterDeserialize()
	{
	}

	// Token: 0x04000EF9 RID: 3833
	public float fDistance = 3f;

	// Token: 0x04000EFA RID: 3834
	public NavMeshSurface surface;

	// Token: 0x04000EFB RID: 3835
	private NavMeshTriangulation triangulation;

	// Token: 0x04000EFC RID: 3836
	public float sphereSize;

	// Token: 0x04000EFD RID: 3837
	private Color color;
}
