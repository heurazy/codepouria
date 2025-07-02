using System;
using System.Linq;
using UnityEngine;

// Token: 0x0200017C RID: 380
public class VariationSwapper : MonoBehaviour
{
	// Token: 0x06000AA6 RID: 2726 RVA: 0x00033C9C File Offset: 0x00031E9C
	public void EnableRandom()
	{
		float num = this.Variations.Sum((VariationSwapper.Variation variation) => variation.chance);
		float num2 = Random.Range(0f, num);
		GameObject gameObject = this.Variations.First<VariationSwapper.Variation>().parent;
		float num3 = 0f;
		foreach (VariationSwapper.Variation variation2 in this.Variations)
		{
			num3 += variation2.chance;
			if (num2 < num3)
			{
				Debug.Log(string.Format("Found new: {0}", variation2.parent));
				gameObject = variation2.parent;
				break;
			}
		}
		if (gameObject != null)
		{
			VariationSwapper.Variation[] array = this.Variations;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].parent.SetActive(false);
			}
			gameObject.SetActive(true);
		}
	}

	// Token: 0x04000985 RID: 2437
	public VariationSwapper.Variation[] Variations;

	// Token: 0x02000380 RID: 896
	[Serializable]
	public class Variation
	{
		// Token: 0x040012F1 RID: 4849
		public GameObject parent;

		// Token: 0x040012F2 RID: 4850
		public float chance = 1f;
	}
}
