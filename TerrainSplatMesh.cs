using System;
using UnityEngine;

// Token: 0x02000286 RID: 646
public class TerrainSplatMesh : MonoBehaviour
{
	// Token: 0x06000F90 RID: 3984 RVA: 0x0004F1A4 File Offset: 0x0004D3A4
	private Mesh GetMesh()
	{
		if (this.mesh == null)
		{
			this.mesh = base.GetComponent<MeshFilter>().sharedMesh;
			this.verts = this.mesh.vertices;
			this.colors = this.mesh.colors;
		}
		return this.mesh;
	}

	// Token: 0x06000F91 RID: 3985 RVA: 0x0004F1F8 File Offset: 0x0004D3F8
	internal bool PointIsValid(Vector3 point)
	{
		if (this.vertexColorMask)
		{
			this.GetMesh();
			if (HelperFunctions.GetValue(HelperFunctions.GetVertexColorAtPoint(this.verts, this.colors, base.transform, point)) < 0.9f)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x04000E95 RID: 3733
	public bool vertexColorMask;

	// Token: 0x04000E96 RID: 3734
	private Mesh mesh;

	// Token: 0x04000E97 RID: 3735
	private Vector3[] verts;

	// Token: 0x04000E98 RID: 3736
	private Color[] colors;
}
