using System;
using UnityEngine;

// Token: 0x020001EE RID: 494
public class LevelGeneration : MonoBehaviour
{
	// Token: 0x06000CFC RID: 3324 RVA: 0x00041164 File Offset: 0x0003F364
	public void Generate()
	{
	}

	// Token: 0x06000CFD RID: 3325 RVA: 0x00041168 File Offset: 0x0003F368
	private void RandomizeBiomeVariants()
	{
		for (int i = 0; i < base.transform.childCount; i++)
		{
			BiomeVariant[] componentsInChildren = base.transform.GetChild(i).GetComponentsInChildren<BiomeVariant>(true);
			BiomeVariant[] array = componentsInChildren;
			for (int j = 0; j < array.Length; j++)
			{
				array[j].gameObject.SetActive(false);
			}
			if (componentsInChildren.Length != 0)
			{
				componentsInChildren[Random.Range(0, componentsInChildren.Length)].gameObject.SetActive(true);
			}
		}
	}

	// Token: 0x06000CFE RID: 3326 RVA: 0x000411D6 File Offset: 0x0003F3D6
	private void Clear()
	{
		Object.FindFirstObjectByType<LightVolume>().SetSize();
		base.GetComponent<PropGrouper>().ClearAll();
	}

	// Token: 0x04000BF8 RID: 3064
	public int seed;

	// Token: 0x04000BF9 RID: 3065
	public bool updateLightmap = true;
}
