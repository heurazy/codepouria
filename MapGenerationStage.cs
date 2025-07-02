using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000085 RID: 133
public class MapGenerationStage : MonoBehaviour
{
	// Token: 0x1700004D RID: 77
	// (get) Token: 0x0600049F RID: 1183 RVA: 0x0001ABC5 File Offset: 0x00018DC5
	private bool singleObject
	{
		get
		{
			return this.spawnMode == MapGenerationStage.SpawnMode.SingleObject;
		}
	}

	// Token: 0x060004A0 RID: 1184 RVA: 0x0001ABD0 File Offset: 0x00018DD0
	private void OnDrawGizmosSelected()
	{
		if (this.useMinimumHeightLimit)
		{
			Gizmos.color = new Color(1f, 0.21f, 0f, 0.49f);
			Gizmos.DrawCube(base.transform.position + new Vector3(0f, this.minimumHeightLimit, 0f), new Vector3(1000f, 0.01f, 1000f));
		}
		if (this.useMaximumHeightLimit)
		{
			Gizmos.color = new Color(0f, 1f, 0.96f, 0.49f);
			Gizmos.DrawCube(base.transform.position + new Vector3(0f, this.maximumHeightLimit, 0f), new Vector3(1000f, 0.01f, 1000f));
		}
	}

	// Token: 0x060004A1 RID: 1185 RVA: 0x0001ACA5 File Offset: 0x00018EA5
	public void Generate(int seed = 0)
	{
		this.ClearSpawnedObjects();
		this.GenerateNodeMap();
		this.RunProximityPasses();
		this.SpawnObjectsFromNodeMap();
	}

	// Token: 0x060004A2 RID: 1186 RVA: 0x0001ACC0 File Offset: 0x00018EC0
	public void ClearSpawnedObjects()
	{
		for (int i = base.transform.childCount - 1; i >= 0; i--)
		{
			Object.DestroyImmediate(base.transform.GetChild(i).gameObject);
		}
		this.spawnedObjects.Clear();
	}

	// Token: 0x060004A3 RID: 1187 RVA: 0x0001AD08 File Offset: 0x00018F08
	private void GenerateNodeMap()
	{
		if (this.nodeSpacing == 0f)
		{
			Debug.LogError("NODE SPACING IS ZERO! THIS WOULD RESULT IN INFINITE SPAWNING!");
			return;
		}
		Vector2 vector = new Vector2(this.spawnRange.bounds.min.x, this.spawnRange.bounds.min.z);
		Vector2 vector2 = new Vector2(this.spawnRange.bounds.max.x, this.spawnRange.bounds.max.z);
		Vector2 vector3 = new Vector2(vector.x, vector.y);
		this.nodeMap.Clear();
		while (vector3.y <= vector2.y)
		{
			List<MapGenerationStage.GenerationNode> list = new List<MapGenerationStage.GenerationNode>();
			this.nodeMap.Add(list);
			while (vector3.x <= vector2.x)
			{
				Vector2 vector4 = new Vector2(vector3.x, vector3.y);
				if (this.randomizedNodeOffset > 0f)
				{
					vector4 += new Vector2(Random.Range(-this.randomizedNodeOffset, this.randomizedNodeOffset), Random.Range(-this.randomizedNodeOffset, this.randomizedNodeOffset));
				}
				list.Add(new MapGenerationStage.GenerationNode(new Vector2(vector4.x, vector4.y), this.defaultDensity));
				vector3.x += this.nodeSpacing;
			}
			vector3.x = vector.x;
			vector3.y += this.nodeSpacing;
		}
	}

	// Token: 0x060004A4 RID: 1188 RVA: 0x0001AE9F File Offset: 0x0001909F
	private void SpawnObjectsFromNodeMap()
	{
		this.RunActionOnAllNodes(new Action<MapGenerationStage.GenerationNode>(this.TrySpawnObject));
	}

	// Token: 0x060004A5 RID: 1189 RVA: 0x0001AEB4 File Offset: 0x000190B4
	private void SpawnObject(Vector3 spot, Vector3 normal)
	{
		GameObject gameObject;
		if (this.singleObject)
		{
			if (this.objectPrefab)
			{
				gameObject = Object.Instantiate<GameObject>(this.objectPrefab);
			}
			else
			{
				gameObject = new GameObject();
			}
		}
		else if (!this.singleObject && this.spawnList)
		{
			gameObject = Object.Instantiate<GameObject>(this.spawnList.GetSingleSpawn());
		}
		else
		{
			gameObject = new GameObject();
		}
		if (this.randomizeRotation)
		{
			if (this.randomizeRotationOnNormalPlane)
			{
				gameObject.transform.rotation = HelperFunctions.GetRandomRotationWithUp(normal);
			}
			else
			{
				gameObject.transform.eulerAngles = new Vector3(gameObject.transform.eulerAngles.x, (float)Random.Range(0, 360), gameObject.transform.eulerAngles.z);
			}
		}
		if (this.heightVariation != Vector2.zero)
		{
			spot += Vector3.up * Random.Range(this.heightVariation.x, this.heightVariation.y);
		}
		if (this.scaleVariation != Vector2.zero)
		{
			float num = Random.Range(this.scaleVariation.x, this.scaleVariation.y);
			gameObject.transform.localScale += new Vector3(num, num, num);
		}
		gameObject.transform.position = spot;
		gameObject.transform.SetParent(base.transform, true);
		LazyGizmo lazyGizmo = gameObject.AddComponent<LazyGizmo>();
		lazyGizmo.onSelected = false;
		lazyGizmo.color = this.testGizmoColor;
		lazyGizmo.radius = this.testGizmoSize;
		this.spawnedObjects.Add(gameObject);
	}

	// Token: 0x060004A6 RID: 1190 RVA: 0x0001B054 File Offset: 0x00019254
	private void TrySpawnObject(MapGenerationStage.GenerationNode node)
	{
		Vector3 point = new Vector3(node.position.x, base.transform.position.y, node.position.y);
		Vector3 vector = Vector3.up;
		if ((this.raycastDownward || this.allowedTags.Count > 0) && Physics.Raycast(point + Vector3.up * 50f, Vector3.down, out this.hit, 100f))
		{
			if (this.useMinimumHeightLimit && this.hit.point.y < base.transform.position.y + this.minimumHeightLimit)
			{
				node.valid = false;
				return;
			}
			if (this.useMaximumHeightLimit && this.hit.point.y > base.transform.position.y + this.maximumHeightLimit)
			{
				node.valid = false;
				return;
			}
			if (this.allowedTags.Count > 0 && !this.allowedTags.Contains(this.hit.collider.gameObject.tag))
			{
				node.valid = false;
				return;
			}
			if (this.raycastDownward)
			{
				point = this.hit.point;
				vector = this.hit.normal;
				Debug.DrawLine(point, point + vector * 10f, Color.red, 10f);
			}
		}
		if (!node.valid)
		{
			return;
		}
		if (Random.Range(0f, 1f) < node.probability)
		{
			this.SpawnObject(point, vector);
		}
	}

	// Token: 0x060004A7 RID: 1191 RVA: 0x0001B1EF File Offset: 0x000193EF
	private void RunProximityPasses()
	{
		this.RunActionOnAllNodes(new Action<MapGenerationStage.GenerationNode>(this.RunProximityPassesOnNode));
	}

	// Token: 0x060004A8 RID: 1192 RVA: 0x0001B204 File Offset: 0x00019404
	private void RunProximityPassesOnNode(MapGenerationStage.GenerationNode node)
	{
		this.RunPositionGradientPass(node);
		for (int i = 0; i < this.proximityPassData.Count; i++)
		{
			MapGenerationStage.GenerationProximityPassData generationProximityPassData = this.proximityPassData[i];
			List<GameObject> list = generationProximityPassData.previousStage.spawnedObjects;
			for (int j = 0; j < list.Count; j++)
			{
				float num = Vector3.Distance(node.position, Util.FlattenVector3(list[j].transform.position));
				if (num < generationProximityPassData.hardAvoidanceRadius * list[j].transform.localScale.x)
				{
					node.valid = false;
				}
				else if (num <= generationProximityPassData.minMaxProximity.y)
				{
					float num2 = Util.RangeLerp(generationProximityPassData.correlation, 0f, generationProximityPassData.minMaxProximity.x, generationProximityPassData.minMaxProximity.y, num, true, null);
					node.probability = Mathf.Clamp(node.probability + num2, this.minMaxDensity.x, this.minMaxDensity.y);
				}
			}
		}
	}

	// Token: 0x060004A9 RID: 1193 RVA: 0x0001B320 File Offset: 0x00019520
	private void RunPositionGradientPass(MapGenerationStage.GenerationNode node)
	{
		float num = (node.position.x - this.spawnRange.bounds.min.x) / (this.spawnRange.bounds.max.x - this.spawnRange.bounds.min.x);
		float num2 = (node.position.y - this.spawnRange.bounds.min.z) / (this.spawnRange.bounds.max.z - this.spawnRange.bounds.min.z);
		float num3 = 0f;
		float num4 = 0f;
		if (this.useCurveX)
		{
			num3 = this.curveX.Evaluate(num);
		}
		if (this.useCurveZ)
		{
			num4 = this.curveZ.Evaluate(num2);
		}
		node.probability = Mathf.Clamp(node.probability + num3 + num4, this.minMaxDensity.x, this.minMaxDensity.y);
	}

	// Token: 0x060004AA RID: 1194 RVA: 0x0001B444 File Offset: 0x00019644
	private void RunActionOnAllNodes(Action<MapGenerationStage.GenerationNode> Action)
	{
		for (int i = 0; i < this.nodeMap.Count; i++)
		{
			List<MapGenerationStage.GenerationNode> list = this.nodeMap[i];
			for (int j = 0; j < list.Count; j++)
			{
				MapGenerationStage.GenerationNode generationNode = list[j];
				Action(generationNode);
			}
		}
	}

	// Token: 0x040004D1 RID: 1233
	public BoxCollider spawnRange;

	// Token: 0x040004D2 RID: 1234
	public float nodeSpacing = 1f;

	// Token: 0x040004D3 RID: 1235
	[Range(0f, 1f)]
	public float defaultDensity;

	// Token: 0x040004D4 RID: 1236
	public Vector2 minMaxDensity = new Vector2(0f, 1f);

	// Token: 0x040004D5 RID: 1237
	public float randomizedNodeOffset;

	// Token: 0x040004D6 RID: 1238
	public bool useCurveX;

	// Token: 0x040004D7 RID: 1239
	public AnimationCurve curveX;

	// Token: 0x040004D8 RID: 1240
	public bool useCurveZ;

	// Token: 0x040004D9 RID: 1241
	public AnimationCurve curveZ;

	// Token: 0x040004DA RID: 1242
	public List<MapGenerationStage.GenerationProximityPassData> proximityPassData;

	// Token: 0x040004DB RID: 1243
	public bool useMinimumHeightLimit;

	// Token: 0x040004DC RID: 1244
	public float minimumHeightLimit;

	// Token: 0x040004DD RID: 1245
	public bool useMaximumHeightLimit;

	// Token: 0x040004DE RID: 1246
	public float maximumHeightLimit;

	// Token: 0x040004DF RID: 1247
	public MapGenerationStage.SpawnMode spawnMode;

	// Token: 0x040004E0 RID: 1248
	public GameObject objectPrefab;

	// Token: 0x040004E1 RID: 1249
	public SpawnList spawnList;

	// Token: 0x040004E2 RID: 1250
	public bool randomizeRotation = true;

	// Token: 0x040004E3 RID: 1251
	public bool randomizeRotationOnNormalPlane = true;

	// Token: 0x040004E4 RID: 1252
	public bool raycastDownward = true;

	// Token: 0x040004E5 RID: 1253
	public List<string> allowedTags;

	// Token: 0x040004E6 RID: 1254
	public Vector2 heightVariation;

	// Token: 0x040004E7 RID: 1255
	public Vector2 scaleVariation;

	// Token: 0x040004E8 RID: 1256
	public Color testGizmoColor = Color.red;

	// Token: 0x040004E9 RID: 1257
	public float testGizmoSize = 0.5f;

	// Token: 0x040004EA RID: 1258
	public List<List<MapGenerationStage.GenerationNode>> nodeMap = new List<List<MapGenerationStage.GenerationNode>>();

	// Token: 0x040004EB RID: 1259
	public List<GameObject> spawnedObjects;

	// Token: 0x040004EC RID: 1260
	private RaycastHit hit;

	// Token: 0x0200030B RID: 779
	public enum SpawnMode
	{
		// Token: 0x0400112F RID: 4399
		SingleObject,
		// Token: 0x04001130 RID: 4400
		SpawnList
	}

	// Token: 0x0200030C RID: 780
	public class GenerationNode
	{
		// Token: 0x060012B0 RID: 4784 RVA: 0x0005AC9C File Offset: 0x00058E9C
		public GenerationNode(Vector2 pos, float defaultProbability)
		{
			this.position = pos;
			this.probability = defaultProbability;
			this.valid = true;
		}

		// Token: 0x04001131 RID: 4401
		public Vector2 position;

		// Token: 0x04001132 RID: 4402
		public float probability;

		// Token: 0x04001133 RID: 4403
		public bool valid;
	}

	// Token: 0x0200030D RID: 781
	[Serializable]
	public class GenerationProximityPassData
	{
		// Token: 0x04001134 RID: 4404
		public MapGenerationStage previousStage;

		// Token: 0x04001135 RID: 4405
		public float hardAvoidanceRadius;

		// Token: 0x04001136 RID: 4406
		public Vector2 minMaxProximity;

		// Token: 0x04001137 RID: 4407
		public float correlation;
	}
}
