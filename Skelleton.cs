using System;
using UnityEngine;

// Token: 0x02000273 RID: 627
public class Skelleton : MonoBehaviour
{
	// Token: 0x06000F37 RID: 3895 RVA: 0x0004CDF8 File Offset: 0x0004AFF8
	public void SpawnSkelly(Character target)
	{
		foreach (Bodypart bodypart in base.transform.GetComponentsInChildren<Bodypart>())
		{
			Bodypart bodypart2 = target.GetBodypart(bodypart.partType);
			if (!(bodypart2 == null))
			{
				bodypart.transform.position = bodypart2.transform.position;
				bodypart.transform.rotation = bodypart2.transform.rotation;
			}
		}
	}
}
