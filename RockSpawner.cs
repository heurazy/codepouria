using System;
using UnityEngine;

// Token: 0x02000259 RID: 601
public class RockSpawner : MonoBehaviour
{
	// Token: 0x06000E8B RID: 3723 RVA: 0x00048DF0 File Offset: 0x00046FF0
	private void OnDrawGizmosSelected()
	{
		Gizmos.DrawLine(base.transform.position - this.area.y * 0.5f * base.transform.forward, base.transform.position + this.area.y * 0.5f * base.transform.forward);
		Gizmos.DrawLine(base.transform.position - this.area.x * 0.5f * base.transform.right, base.transform.position + this.area.x * 0.5f * base.transform.right);
	}

	// Token: 0x06000E8C RID: 3724 RVA: 0x00048ECC File Offset: 0x000470CC
	public void Go()
	{
		this.Clear();
		for (int i = 0; i < this.nrOfSpawns; i++)
		{
			this.DoSpawn();
		}
	}

	// Token: 0x06000E8D RID: 3725 RVA: 0x00048EF8 File Offset: 0x000470F8
	private void Clear()
	{
		for (int i = base.transform.childCount - 1; i >= 0; i--)
		{
			Object.DestroyImmediate(base.transform.GetChild(i).gameObject);
		}
	}

	// Token: 0x06000E8E RID: 3726 RVA: 0x00048F34 File Offset: 0x00047134
	private void DoSpawn()
	{
		RockSpawner.ReturnData? randomPoint = this.GetRandomPoint();
		if (randomPoint == null)
		{
			return;
		}
		GameObject gameObject = this.rocks[Random.Range(0, this.rocks.Length)];
		Quaternion quaternion = gameObject.transform.rotation;
		if (this.rotation == RockSpawner.OriginalRotation.RaycastNormal)
		{
			quaternion = HelperFunctions.GetRandomRotationWithUp(randomPoint.Value.normal);
		}
		quaternion = Quaternion.Lerp(quaternion, Random.rotation, Mathf.Pow(Random.value, this.rotationPow) * this.maxRotation);
		GameObject gameObject2 = Object.Instantiate<GameObject>(gameObject, randomPoint.Value.pos, quaternion, base.transform);
		gameObject2.transform.position += base.transform.up * -this.downMove;
		gameObject2.transform.Rotate(base.transform.eulerAngles, Space.World);
		gameObject2.transform.localScale *= Random.Range(this.minScale, this.maxScale);
		Physics.SyncTransforms();
	}

	// Token: 0x06000E8F RID: 3727 RVA: 0x00049038 File Offset: 0x00047238
	private RockSpawner.ReturnData? GetRandomPoint()
	{
		Vector3 vector = base.transform.position;
		vector += base.transform.right * Mathf.Lerp(-this.area.x * 0.5f, this.area.x * 0.5f, Random.value);
		vector += base.transform.forward * Mathf.Lerp(-this.area.y * 0.5f, this.area.y * 0.5f, Random.value);
		if (!this.raycast)
		{
			return new RockSpawner.ReturnData?(new RockSpawner.ReturnData
			{
				pos = vector,
				normal = Vector3.up
			});
		}
		RaycastHit raycastHit = HelperFunctions.LineCheck(vector, vector + base.transform.up * -5000f, HelperFunctions.LayerType.Terrain, 0f, QueryTriggerInteraction.Ignore);
		if (raycastHit.transform)
		{
			return new RockSpawner.ReturnData?(new RockSpawner.ReturnData
			{
				pos = raycastHit.point,
				normal = raycastHit.normal
			});
		}
		return null;
	}

	// Token: 0x04000D7B RID: 3451
	public Vector2 area;

	// Token: 0x04000D7C RID: 3452
	public GameObject[] rocks;

	// Token: 0x04000D7D RID: 3453
	public int nrOfSpawns = 500;

	// Token: 0x04000D7E RID: 3454
	public float downMove;

	// Token: 0x04000D7F RID: 3455
	public RockSpawner.OriginalRotation rotation;

	// Token: 0x04000D80 RID: 3456
	public bool raycast = true;

	// Token: 0x04000D81 RID: 3457
	public float minScale = 1f;

	// Token: 0x04000D82 RID: 3458
	public float maxScale = 2f;

	// Token: 0x04000D83 RID: 3459
	public float maxRotation = 1f;

	// Token: 0x04000D84 RID: 3460
	public float rotationPow;

	// Token: 0x020003AC RID: 940
	public enum OriginalRotation
	{
		// Token: 0x04001386 RID: 4998
		PrefabRotation,
		// Token: 0x04001387 RID: 4999
		RaycastNormal
	}

	// Token: 0x020003AD RID: 941
	private struct ReturnData
	{
		// Token: 0x04001388 RID: 5000
		public Vector3 pos;

		// Token: 0x04001389 RID: 5001
		public Vector3 normal;
	}
}
