using System;
using System.Collections.Generic;
using pworld.Scripts.Extensions;
using UnityEngine;

// Token: 0x02000110 RID: 272
public class RockSpawnerGD : MonoBehaviour
{
	// Token: 0x060007F0 RID: 2032 RVA: 0x00029F24 File Offset: 0x00028124
	public void createDeck()
	{
		this.deck.Clear();
		for (int i = 0; i < this.objectsToSpawn.Length; i++)
		{
			for (int j = 0; j < this.objectsToSpawn[i].maxCount; j++)
			{
				this.deck.Add(this.objectsToSpawn[i]);
			}
		}
		this.shuffleDeck();
	}

	// Token: 0x060007F1 RID: 2033 RVA: 0x00029F80 File Offset: 0x00028180
	public void shuffleDeck()
	{
		for (int i = 0; i < this.deck.Count; i++)
		{
			SpawnObject spawnObject = this.deck[i];
			int num = Random.Range(i, this.objectsToSpawn.Length);
			this.deck[i] = this.deck[num];
			this.deck[num] = spawnObject;
		}
	}

	// Token: 0x060007F2 RID: 2034 RVA: 0x00029FE4 File Offset: 0x000281E4
	public SpawnObject DrawFromDeck()
	{
		SpawnObject spawnObject = this.deck[0];
		this.deck.RemoveAt(0);
		return spawnObject;
	}

	// Token: 0x060007F3 RID: 2035 RVA: 0x0002A000 File Offset: 0x00028200
	public void spawnObjects()
	{
		this.clearList();
		this.createDeck();
		int count = this.deck.Count;
		int num = count / this.layerCount;
		if (this.layerCount > count)
		{
			num = count;
		}
		for (int i = 0; i < count; i++)
		{
			float num2 = (float)i * this.yBias + 1f;
			RaycastHit raycastHit;
			if (Physics.Raycast(base.transform.position - base.transform.up + (base.transform.right * Random.Range(-1f, 1f) * this.shape.size.x / 2f + base.transform.forward * (Mathf.Pow(Random.Range(-1f, 1f), num2) * this.shape.size.z / 2f)), -base.transform.up, out raycastHit))
			{
				SpawnObject spawnObject = this.DrawFromDeck();
				GameObject gameObject = Object.Instantiate<GameObject>(spawnObject.prefab);
				gameObject.transform.position = raycastHit.point + new Vector3(Random.Range(-spawnObject.posJitter.x, spawnObject.posJitter.x), Random.Range(-spawnObject.posJitter.y, spawnObject.posJitter.y), Random.Range(-spawnObject.posJitter.z, spawnObject.posJitter.z));
				gameObject.transform.eulerAngles += new Vector3(Random.Range(-spawnObject.randomRot.x, spawnObject.randomRot.x), Random.Range(-spawnObject.randomRot.y, spawnObject.randomRot.y), Random.Range(-spawnObject.randomRot.z, spawnObject.randomRot.z));
				gameObject.transform.localScale += new Vector3(Random.Range(-spawnObject.randomScale.x, spawnObject.randomScale.x), Random.Range(-spawnObject.randomScale.y, spawnObject.randomScale.y), Random.Range(-spawnObject.randomScale.z, spawnObject.randomScale.z));
				gameObject.transform.localScale += Vector3.one * Random.Range(-spawnObject.uniformScale, spawnObject.uniformScale);
				gameObject.transform.localScale = Vector3.Scale(gameObject.transform.localScale, Vector3.one - new Vector3((float)Random.Range(0f, spawnObject.inversion.x).PCeilToInt(), (float)Random.Range(0f, spawnObject.inversion.y).PCeilToInt(), (float)Random.Range(0f, spawnObject.inversion.z).PCeilToInt()).normalized * 2f);
				gameObject.transform.localScale *= spawnObject.scaleMultiplier;
				this.spawnedObjects.Add(gameObject);
				gameObject.transform.parent = base.transform;
				if (i % num == 0)
				{
					Physics.SyncTransforms();
				}
			}
		}
	}

	// Token: 0x060007F4 RID: 2036 RVA: 0x0002A3A0 File Offset: 0x000285A0
	public void clearList()
	{
		for (int i = 0; i < this.spawnedObjects.Count; i++)
		{
			Object.DestroyImmediate(this.spawnedObjects[i]);
		}
		this.spawnedObjects.Clear();
	}

	// Token: 0x060007F5 RID: 2037 RVA: 0x0002A3DF File Offset: 0x000285DF
	public void OnValidate()
	{
		this.shape.size = new Vector3(this.colliderScale.x, 0f, this.colliderScale.y);
	}

	// Token: 0x060007F6 RID: 2038 RVA: 0x0002A40C File Offset: 0x0002860C
	private void OnDrawGizmosSelected()
	{
	}

	// Token: 0x04000769 RID: 1897
	public Vector2 colliderScale;

	// Token: 0x0400076A RID: 1898
	public SpawnObject[] objectsToSpawn;

	// Token: 0x0400076B RID: 1899
	public List<GameObject> spawnedObjects;

	// Token: 0x0400076C RID: 1900
	public List<SpawnObject> deck;

	// Token: 0x0400076D RID: 1901
	public Vector2 castShape;

	// Token: 0x0400076E RID: 1902
	public BoxCollider shape;

	// Token: 0x0400076F RID: 1903
	public float yBias;

	// Token: 0x04000770 RID: 1904
	[Range(1f, 99f)]
	public int layerCount;
}
