using System;
using System.Collections.Generic;
using pworld.Scripts.Extensions;
using UnityEngine;

// Token: 0x02000228 RID: 552
public class PropSpawner : MonoBehaviour
{
	// Token: 0x06000E06 RID: 3590 RVA: 0x00046A68 File Offset: 0x00044C68
	private void OnDrawGizmosSelected()
	{
		Vector3 vector = base.transform.position + this.area.y * 0.5f * base.transform.up;
		Vector3 vector2 = base.transform.position - this.area.y * 0.5f * base.transform.up;
		Vector3 vector3 = base.transform.position - this.area.x * 0.5f * base.transform.right;
		Vector3 vector4 = base.transform.position + this.area.x * 0.5f * base.transform.right;
		Gizmos.DrawLine(vector2, vector);
		Gizmos.DrawLine(vector3, vector4);
		Gizmos.color = Color.cyan;
		Gizmos.DrawLine(vector2, vector2 + base.transform.forward * this.rayLength);
		Gizmos.DrawLine(vector, vector + base.transform.forward * this.rayLength);
		Gizmos.DrawLine(vector3, vector3 + base.transform.forward * this.rayLength);
		Gizmos.DrawLine(vector4, vector4 + base.transform.forward * this.rayLength);
		Matrix4x4 matrix = Gizmos.matrix;
		Gizmos.matrix = Matrix4x4.TRS(base.transform.position + base.transform.forward * this.rayLength / 2f, base.transform.rotation, Vector3.one);
		Gizmos.DrawWireCube(Vector3.zero, this.area.xyn(this.rayLength));
		Gizmos.matrix = matrix;
	}

	// Token: 0x06000E07 RID: 3591 RVA: 0x00046C4A File Offset: 0x00044E4A
	public void Go()
	{
		this.Clear();
		this.Add();
	}

	// Token: 0x06000E08 RID: 3592 RVA: 0x00046C58 File Offset: 0x00044E58
	public void Add()
	{
		if (this.chanceToUseSpawner < 0.999f && Random.value > this.chanceToUseSpawner)
		{
			return;
		}
		int num = 50000;
		int num2 = 0;
		while (num2 < this.nrOfSpawns && num > 0)
		{
			num--;
			if (this.TryToSpawn(num2))
			{
				num2++;
				if (this.syncTransforms)
				{
					Physics.SyncTransforms();
				}
			}
		}
	}

	// Token: 0x06000E09 RID: 3593 RVA: 0x00046CB8 File Offset: 0x00044EB8
	public void Clear()
	{
		for (int i = base.transform.childCount - 1; i >= 0; i--)
		{
			Object.DestroyImmediate(base.transform.GetChild(i).gameObject);
		}
	}

	// Token: 0x06000E0A RID: 3594 RVA: 0x00046CF3 File Offset: 0x00044EF3
	public void GoAll()
	{
		base.GetComponentInParent<PropGrouper>().RunAll(true);
	}

	// Token: 0x06000E0B RID: 3595 RVA: 0x00046D01 File Offset: 0x00044F01
	public void ClearAll()
	{
		base.GetComponentInParent<PropGrouper>().ClearAll();
	}

	// Token: 0x06000E0C RID: 3596 RVA: 0x00046D10 File Offset: 0x00044F10
	private bool TryToSpawn(int currentSpawnCount)
	{
		PropSpawner.SpawnData randomPoint = this.GetRandomPoint();
		if (randomPoint == null)
		{
			return false;
		}
		for (int i = 0; i < this.constraints.Count; i++)
		{
			if (!this.constraints[i].CheckConstraint(randomPoint))
			{
				return false;
			}
		}
		randomPoint.spawnCount = currentSpawnCount;
		return this.Spawn(randomPoint) != null;
	}

	// Token: 0x06000E0D RID: 3597 RVA: 0x00046D6C File Offset: 0x00044F6C
	private GameObject Spawn(PropSpawner.SpawnData spawnData)
	{
		GameObject gameObject = HelperFunctions.SpawnPrefab(this.props[Random.Range(0, this.props.Length)], spawnData.pos, HelperFunctions.GetRandomRotationWithUp(Vector3.up), base.transform);
		for (int i = 0; i < this.modifiers.Count; i++)
		{
			this.modifiers[i].ModifyObject(gameObject, spawnData);
		}
		for (int j = 0; j < this.postConstraints.Count; j++)
		{
			if (!this.postConstraints[j].CheckConstraint(gameObject, spawnData))
			{
				Object.DestroyImmediate(gameObject);
				return null;
			}
		}
		return gameObject;
	}

	// Token: 0x06000E0E RID: 3598 RVA: 0x00046E08 File Offset: 0x00045008
	private PropSpawner.SpawnData GetRandomPoint()
	{
		Vector3 vector = base.transform.position;
		Vector2 vector2 = new Vector2(Random.value, Random.value);
		vector += base.transform.right * Mathf.Lerp(-this.area.x * 0.5f, this.area.x * 0.5f, vector2.x);
		vector += base.transform.up * Mathf.Lerp(-this.area.y * 0.5f, this.area.y * 0.5f, vector2.y);
		if (!this.raycastPosition)
		{
			return new PropSpawner.SpawnData
			{
				pos = vector,
				normal = -base.transform.forward,
				rayDir = base.transform.forward,
				hit = default(RaycastHit),
				spawnerTransform = base.transform,
				placement = vector2
			};
		}
		RaycastHit raycastHit = HelperFunctions.LineCheck(vector, vector + (base.transform.forward + this.rayDirectionOffset).normalized * this.rayLength, this.layerType, 0f, QueryTriggerInteraction.Ignore);
		if (raycastHit.transform)
		{
			return new PropSpawner.SpawnData
			{
				pos = raycastHit.point,
				normal = raycastHit.normal,
				rayDir = base.transform.forward,
				hit = raycastHit,
				spawnerTransform = base.transform,
				placement = vector2
			};
		}
		return null;
	}

	// Token: 0x04000D0F RID: 3343
	public Vector2 area;

	// Token: 0x04000D10 RID: 3344
	public Vector3 rayDirectionOffset;

	// Token: 0x04000D11 RID: 3345
	public float rayLength = 5000f;

	// Token: 0x04000D12 RID: 3346
	public bool raycastPosition = true;

	// Token: 0x04000D13 RID: 3347
	public int nrOfSpawns = 500;

	// Token: 0x04000D14 RID: 3348
	[Range(0f, 1f)]
	public float chanceToUseSpawner = 1f;

	// Token: 0x04000D15 RID: 3349
	public GameObject[] props;

	// Token: 0x04000D16 RID: 3350
	public bool syncTransforms = true;

	// Token: 0x04000D17 RID: 3351
	public HelperFunctions.LayerType layerType = HelperFunctions.LayerType.TerrainMap;

	// Token: 0x04000D18 RID: 3352
	[SerializeReference]
	public List<PropSpawnerMod> modifiers = new List<PropSpawnerMod>();

	// Token: 0x04000D19 RID: 3353
	[SerializeReference]
	public List<PropSpawnerConstraint> constraints = new List<PropSpawnerConstraint>();

	// Token: 0x04000D1A RID: 3354
	[SerializeReference]
	public List<PropSpawnerConstraintPost> postConstraints = new List<PropSpawnerConstraintPost>();

	// Token: 0x020003A5 RID: 933
	public class SpawnData
	{
		// Token: 0x04001373 RID: 4979
		public Transform spawnerTransform;

		// Token: 0x04001374 RID: 4980
		public Vector3 pos;

		// Token: 0x04001375 RID: 4981
		public Vector3 normal;

		// Token: 0x04001376 RID: 4982
		public Vector3 rayDir;

		// Token: 0x04001377 RID: 4983
		public RaycastHit hit;

		// Token: 0x04001378 RID: 4984
		public Vector2 placement;

		// Token: 0x04001379 RID: 4985
		public int spawnCount;
	}
}
