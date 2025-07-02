using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200029A RID: 666
public class Wall : MonoBehaviour
{
	// Token: 0x06000FDC RID: 4060 RVA: 0x00050A13 File Offset: 0x0004EC13
	internal void WallInit()
	{
		this.pieces = new List<WallPiece>();
	}

	// Token: 0x06000FDD RID: 4061 RVA: 0x00050A20 File Offset: 0x0004EC20
	internal void AddPiece(WallPiece piece)
	{
		this.pieces.Add(piece);
	}

	// Token: 0x06000FDE RID: 4062 RVA: 0x00050A30 File Offset: 0x0004EC30
	private void OnDrawGizmos()
	{
		Gizmos.color = new Color(1f, 1f, 1f, 0.1f);
		for (int i = 0; i < this.gridSize.x; i++)
		{
			for (int j = 0; j < this.gridSize.y; j++)
			{
				Gizmos.DrawWireCube(this.GetGridPos(i, j), new Vector3(this.gridCellSize, this.gridCellSize, 0.25f));
			}
		}
	}

	// Token: 0x06000FDF RID: 4063 RVA: 0x00050AAC File Offset: 0x0004ECAC
	internal Vector3 GetGridPos(int x, int y)
	{
		Vector2 vector = (this.gridSize - Vector2.one) * this.gridCellSize;
		Vector2 vector2 = base.transform.position - vector * 0.5f;
		Vector2 vector3 = base.transform.position + vector * 0.5f;
		float num = (float)x / ((float)this.gridSize.x - 1f);
		float num2 = (float)y / ((float)this.gridSize.y - 1f);
		return new Vector3(Mathf.Lerp(vector2.x, vector3.x, num), Mathf.Lerp(vector2.y, vector3.y, num2), base.transform.position.z);
	}

	// Token: 0x06000FE0 RID: 4064 RVA: 0x00050B84 File Offset: 0x0004ED84
	internal Vector3 SnapToPosition(Vector3 position)
	{
		Vector2 vector = (this.gridSize - Vector2.one) * this.gridCellSize;
		Vector2 vector2 = base.transform.position - vector * 0.5f;
		Vector2 vector3 = base.transform.position + vector * 0.5f;
		float num = Mathf.InverseLerp(vector2.x, vector3.x, position.x);
		float num2 = Mathf.InverseLerp(vector2.y, vector3.y, position.y);
		int num3 = Mathf.RoundToInt(num * ((float)this.gridSize.x - 1f));
		int num4 = Mathf.RoundToInt(num2 * ((float)this.gridSize.y - 1f));
		return this.GetGridPos(num3, num4);
	}

	// Token: 0x06000FE1 RID: 4065 RVA: 0x00050C60 File Offset: 0x0004EE60
	internal bool PieceFits(WallPiece piece, int x, int y)
	{
		foreach (WallPiece wallPiece in this.pieces)
		{
			if (this.CollisionCheck(piece, x, y, wallPiece))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06000FE2 RID: 4066 RVA: 0x00050CC0 File Offset: 0x0004EEC0
	private bool CollisionCheck(WallPiece newPiece, int newPosX, int newPosY, WallPiece existing)
	{
		for (int i = 0; i < newPiece.dimention.x; i++)
		{
			for (int j = 0; j < newPiece.dimention.y; j++)
			{
				Vector2Int vector2Int = new Vector2Int(newPosX + i, newPosY + j);
				if (this.CollisionCheckSpot(vector2Int, existing))
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06000FE3 RID: 4067 RVA: 0x00050D14 File Offset: 0x0004EF14
	private bool CollisionCheckSpot(Vector2Int checkPos, WallPiece existing)
	{
		for (int i = 0; i < existing.dimention.x; i++)
		{
			for (int j = 0; j < existing.dimention.y; j++)
			{
				if (new Vector2Int(existing.wallPosition.x + i, existing.wallPosition.y + j) == checkPos)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x04000EF6 RID: 3830
	public Vector2Int gridSize;

	// Token: 0x04000EF7 RID: 3831
	public float gridCellSize;

	// Token: 0x04000EF8 RID: 3832
	public List<WallPiece> pieces = new List<WallPiece>();
}
