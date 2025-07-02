using System;
using UnityEngine;

// Token: 0x02000226 RID: 550
public class PropDeleter : MonoBehaviour
{
	// Token: 0x06000DFE RID: 3582 RVA: 0x00046624 File Offset: 0x00044824
	private void OnDrawGizmosSelected()
	{
		Gizmos.DrawWireSphere(base.transform.position, this.radius);
	}

	// Token: 0x06000DFF RID: 3583 RVA: 0x0004663C File Offset: 0x0004483C
	public void Go()
	{
		foreach (Collider collider in Physics.OverlapSphere(base.transform.position, this.radius, HelperFunctions.GetMask(this.layerType)))
		{
			if (!(collider == null) && !(collider.gameObject == null))
			{
				int j = 0;
				Transform transform = collider.transform;
				while (j < 5)
				{
					j++;
					Transform parent = transform.parent;
					if (parent == null)
					{
						break;
					}
					PropGrouper componentInParent = transform.GetComponentInParent<PropGrouper>();
					if (!(componentInParent == null))
					{
						Transform transform2 = componentInParent.transform;
						bool flag = false;
						for (int k = 0; k < this.requiredParents.Length; k++)
						{
							if (transform2 == this.requiredParents[k])
							{
								flag = true;
							}
						}
						if (!flag && this.requiredParents.Length != 0)
						{
							break;
						}
						if (parent.GetComponent<PropSpawner>() || parent.GetComponent<PropSpawner_Line>())
						{
							Object.DestroyImmediate(transform.gameObject);
							break;
						}
						transform = parent;
					}
				}
			}
		}
	}

	// Token: 0x04000D0B RID: 3339
	public HelperFunctions.LayerType layerType;

	// Token: 0x04000D0C RID: 3340
	public float radius = 10f;

	// Token: 0x04000D0D RID: 3341
	public Transform[] requiredParents;
}
