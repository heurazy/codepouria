using System;
using UnityEngine;

// Token: 0x0200029D RID: 669
public class WallPieceSpawner : MonoBehaviour
{
	// Token: 0x06000FF2 RID: 4082 RVA: 0x00050ED0 File Offset: 0x0004F0D0
	private void Go()
	{
		this.wall = base.GetComponent<Wall>();
		this.wall.WallInit();
		this.root = base.transform.Find("Pieces");
		this.Clear();
		for (int i = 0; i < 50; i++)
		{
			this.DoSpawns();
		}
	}

	// Token: 0x06000FF3 RID: 4083 RVA: 0x00050F24 File Offset: 0x0004F124
	private void DoSpawns()
	{
		for (int i = 0; i < this.wall.gridSize.x; i++)
		{
			for (int j = 0; j < this.wall.gridSize.y; j++)
			{
				WallPiece randomPiece = this.GetRandomPiece();
				if (this.wall.PieceFits(randomPiece, i, j))
				{
					this.SpawnPiece(randomPiece, i, j);
				}
			}
		}
	}

	// Token: 0x06000FF4 RID: 4084 RVA: 0x00050F88 File Offset: 0x0004F188
	private void SpawnPiece(WallPiece piece, int x, int y)
	{
		WallPiece component = HelperFunctions.SpawnPrefab(piece.gameObject, this.wall.GetGridPos(x, y), Quaternion.identity, this.root).GetComponent<WallPiece>();
		component.wallPosition = new Vector2Int(x, y);
		this.wall.AddPiece(component);
	}

	// Token: 0x06000FF5 RID: 4085 RVA: 0x00050FD7 File Offset: 0x0004F1D7
	private WallPiece GetRandomPiece()
	{
		return this.pieces[Random.Range(0, this.pieces.Length)];
	}

	// Token: 0x06000FF6 RID: 4086 RVA: 0x00050FF0 File Offset: 0x0004F1F0
	private void Clear()
	{
		this.root = base.transform.Find("Pieces");
		for (int i = this.root.childCount - 1; i >= 0; i--)
		{
			Object.DestroyImmediate(this.root.GetChild(i).gameObject);
		}
	}

	// Token: 0x04000F00 RID: 3840
	public WallPiece[] pieces;

	// Token: 0x04000F01 RID: 3841
	private Transform root;

	// Token: 0x04000F02 RID: 3842
	private Wall wall;
}
