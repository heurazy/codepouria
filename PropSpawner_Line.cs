using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200024D RID: 589
public class PropSpawner_Line : MonoBehaviour
{
	// Token: 0x06000E57 RID: 3671 RVA: 0x00047E20 File Offset: 0x00046020
	private void OnDrawGizmosSelected()
	{
		Vector3 vector = base.transform.position + this.height * 0.5f * base.transform.up;
		Gizmos.DrawLine(base.transform.position - this.height * 0.5f * base.transform.up, vector);
	}

	// Token: 0x06000E58 RID: 3672 RVA: 0x00047E8C File Offset: 0x0004608C
	public void Go()
	{
		this.Clear();
		this.Add();
	}

	// Token: 0x06000E59 RID: 3673 RVA: 0x00047E9C File Offset: 0x0004609C
	public void Add()
	{
		int num = 50000;
		int num2 = 0;
		Physics.SyncTransforms();
		while (num2 < this.nrOfSpawns && num > 0)
		{
			num--;
			if (this.TryToSpawn())
			{
				num2++;
				if (this.syncTransforms)
				{
					Physics.SyncTransforms();
				}
			}
		}
	}

	// Token: 0x06000E5A RID: 3674 RVA: 0x00047EE4 File Offset: 0x000460E4
	public void Clear()
	{
		for (int i = base.transform.childCount - 1; i >= 0; i--)
		{
			Object.DestroyImmediate(base.transform.GetChild(i).gameObject);
		}
	}

	// Token: 0x06000E5B RID: 3675 RVA: 0x00047F1F File Offset: 0x0004611F
	public void GoAll()
	{
		base.GetComponentInParent<PropGrouper>().RunAll(true);
	}

	// Token: 0x06000E5C RID: 3676 RVA: 0x00047F2D File Offset: 0x0004612D
	public void ClearAll()
	{
		base.GetComponentInParent<PropGrouper>().ClearAll();
	}

	// Token: 0x06000E5D RID: 3677 RVA: 0x00047F3C File Offset: 0x0004613C
	private bool TryToSpawn()
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
		return this.Spawn(randomPoint) != null;
	}

	// Token: 0x06000E5E RID: 3678 RVA: 0x00047F90 File Offset: 0x00046190
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

	// Token: 0x06000E5F RID: 3679 RVA: 0x0004802C File Offset: 0x0004622C
	private PropSpawner.SpawnData GetRandomPoint()
	{
		Vector3 vector = base.transform.position + base.transform.up * Mathf.Lerp(-0.5f, 0.5f, Random.value) * this.height;
		Vector3 normalized = Vector3.ProjectOnPlane(Random.onUnitSphere, base.transform.up).normalized;
		if (!this.rayCastSpawn)
		{
			return new PropSpawner.SpawnData
			{
				pos = vector,
				normal = normalized,
				rayDir = normalized,
				hit = default(RaycastHit),
				spawnerTransform = base.transform
			};
		}
		RaycastHit raycastHit = HelperFunctions.LineCheck(vector, vector + normalized * this.rayLength, this.layerType, 0f, QueryTriggerInteraction.Ignore);
		if (raycastHit.transform)
		{
			return new PropSpawner.SpawnData
			{
				pos = raycastHit.point,
				normal = raycastHit.normal,
				rayDir = normalized,
				hit = raycastHit,
				spawnerTransform = base.transform
			};
		}
		return null;
	}

	// Token: 0x04000D5F RID: 3423
	public float height = 200f;

	// Token: 0x04000D60 RID: 3424
	public float rayLength = 5000f;

	// Token: 0x04000D61 RID: 3425
	public int nrOfSpawns = 500;

	// Token: 0x04000D62 RID: 3426
	public bool rayCastSpawn = true;

	// Token: 0x04000D63 RID: 3427
	public GameObject[] props;

	// Token: 0x04000D64 RID: 3428
	public bool syncTransforms = true;

	// Token: 0x04000D65 RID: 3429
	public HelperFunctions.LayerType layerType = HelperFunctions.LayerType.TerrainMap;

	// Token: 0x04000D66 RID: 3430
	[SerializeReference]
	public List<PropSpawnerMod> modifiers = new List<PropSpawnerMod>();

	// Token: 0x04000D67 RID: 3431
	[SerializeReference]
	public List<PropSpawnerConstraint> constraints = new List<PropSpawnerConstraint>();

	// Token: 0x04000D68 RID: 3432
	[SerializeReference]
	public List<PropSpawnerConstraintPost> postConstraints = new List<PropSpawnerConstraintPost>();
}
