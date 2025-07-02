using System;
using System.Collections.Generic;
using System.Linq;
using pworld.Scripts.Extensions;
using UnityEngine;
using Zorro.Core;

// Token: 0x02000281 RID: 641
public class StupidRockPlacer : MonoBehaviour
{
	// Token: 0x170000BF RID: 191
	// (get) Token: 0x06000F61 RID: 3937 RVA: 0x0004DFE8 File Offset: 0x0004C1E8
	public Vector3 size
	{
		get
		{
			return base.transform.localScale.xyz();
		}
	}

	// Token: 0x06000F62 RID: 3938 RVA: 0x0004DFFA File Offset: 0x0004C1FA
	private void OnDrawGizmosSelected()
	{
		Gizmos.DrawWireCube(base.transform.position + this.size / 2f, this.size);
	}

	// Token: 0x06000F63 RID: 3939 RVA: 0x0004E027 File Offset: 0x0004C227
	public void Clear()
	{
		if (this.rockParent)
		{
			this.rockParent.KillAllChildren(true, false, true);
		}
	}

	// Token: 0x06000F64 RID: 3940 RVA: 0x0004E044 File Offset: 0x0004C244
	private void Start()
	{
	}

	// Token: 0x06000F65 RID: 3941 RVA: 0x0004E048 File Offset: 0x0004C248
	private void ValidatePool()
	{
		foreach (Transform transform in (from t in this.pieceRoot.GetComponentsInChildren<Transform>()
			where t != this.pieceRoot
			select t).ToList<Transform>())
		{
			transform.gameObject.GetOrAddComponent<PutMeInWall>();
			transform.gameObject.layer = LayerMask.NameToLayer("Terrain");
			PExt.DirtyObj(transform.gameObject);
		}
	}

	// Token: 0x06000F66 RID: 3942 RVA: 0x0004E0DC File Offset: 0x0004C2DC
	public void Go()
	{
		this.rockParent = null;
		this.rockParent = base.transform.parent.Find("Rocks: " + base.gameObject.name);
		if (!this.rockParent)
		{
			this.rockParent = new GameObject("Rocks: " + base.gameObject.name).transform;
			this.rockParent.SetParent(base.transform.parent);
		}
		this.rockParent.SetSiblingIndex(base.transform.GetSiblingIndex() + 1);
		this.rocks = (from x in this.pieceRoot.GetComponentsInChildren<PutMeInWall>()
			select x.gameObject).ToList<GameObject>();
		this.lastPlaced = new List<GameObject>();
		int num = 0;
		int num2 = 0;
		while (num2 < this.amount || num > this.amount * 10)
		{
			num++;
			Vector3 vector = base.transform.position + new Vector3(this.size.x.Rand(), this.size.y.Rand(), 0f);
			GameObject random = this.rocks.GetRandom<GameObject>();
			Vector3? wallPosition = random.GetComponent<PutMeInWall>().GetWallPosition(vector, base.transform.localScale.z);
			if (wallPosition == null)
			{
				num2--;
			}
			else
			{
				GameObject gameObject = Object.Instantiate<GameObject>(random, wallPosition.Value, ExtQuaternion.RandomRotation());
				gameObject.transform.SetParent(this.rockParent);
				PutMeInWall putMeInWall;
				if (!gameObject.TryGetComponent<PutMeInWall>(out putMeInWall))
				{
					putMeInWall = gameObject.AddComponent<PutMeInWall>();
				}
				putMeInWall.gameObject.SetActive(true);
				this.lastPlaced.Add(gameObject);
				putMeInWall.RandomScale();
				Physics.SyncTransforms();
				PExt.DirtyObj(gameObject);
			}
			num2++;
		}
	}

	// Token: 0x06000F67 RID: 3943 RVA: 0x0004E2C8 File Offset: 0x0004C4C8
	public void RemoveLastPlaced()
	{
		foreach (GameObject gameObject in this.lastPlaced)
		{
			gameObject == null;
		}
		this.lastPlaced = new List<GameObject>();
	}

	// Token: 0x06000F68 RID: 3944 RVA: 0x0004E328 File Offset: 0x0004C528
	private void Update()
	{
	}

	// Token: 0x04000E6E RID: 3694
	public List<GameObject> rocks;

	// Token: 0x04000E6F RID: 3695
	public Transform pieceRoot;

	// Token: 0x04000E70 RID: 3696
	public int amount = 10;

	// Token: 0x04000E71 RID: 3697
	public Transform rockParent;

	// Token: 0x04000E72 RID: 3698
	public List<GameObject> lastPlaced = new List<GameObject>();
}
