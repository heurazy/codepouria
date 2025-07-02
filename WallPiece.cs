using System;
using UnityEngine;

// Token: 0x0200029C RID: 668
public class WallPiece : MonoBehaviour
{
	// Token: 0x06000FEE RID: 4078 RVA: 0x00050E94 File Offset: 0x0004F094
	public void SnapToGrid()
	{
		base.transform.position = base.GetComponentInParent<Wall>().SnapToPosition(base.transform.position);
	}

	// Token: 0x06000FEF RID: 4079 RVA: 0x00050EB7 File Offset: 0x0004F0B7
	private void Start()
	{
	}

	// Token: 0x06000FF0 RID: 4080 RVA: 0x00050EB9 File Offset: 0x0004F0B9
	private void Update()
	{
	}

	// Token: 0x04000EFE RID: 3838
	public Vector2Int dimention = Vector2Int.one;

	// Token: 0x04000EFF RID: 3839
	internal Vector2Int wallPosition;
}
