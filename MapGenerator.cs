using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000086 RID: 134
public class MapGenerator : MonoBehaviour
{
	// Token: 0x060004AC RID: 1196 RVA: 0x0001B500 File Offset: 0x00019700
	public void GenerateAll()
	{
		if (this.seed != 0)
		{
			Debug.Log("Set Seed");
			Random.InitState(this.seed);
		}
		for (int i = 0; i < this.stages.Count; i++)
		{
			if (this.stages[i].gameObject.activeInHierarchy)
			{
				this.stages[i].Generate(0);
				Debug.Log(i.ToString() + " " + Random.state.GetHashCode().ToString());
			}
		}
	}

	// Token: 0x060004AD RID: 1197 RVA: 0x0001B59C File Offset: 0x0001979C
	public void ClearAll()
	{
		for (int i = 0; i < this.stages.Count; i++)
		{
			if (this.stages[i].gameObject.activeInHierarchy)
			{
				this.stages[i].ClearSpawnedObjects();
			}
		}
	}

	// Token: 0x040004ED RID: 1261
	public int seed;

	// Token: 0x040004EE RID: 1262
	public List<MapGenerationStage> stages;
}
