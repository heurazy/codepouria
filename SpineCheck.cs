using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200027A RID: 634
public class SpineCheck : CustomSpawnCondition
{
	// Token: 0x06000F4B RID: 3915 RVA: 0x0004D398 File Offset: 0x0004B598
	public override bool CheckCondition(PropSpawner.SpawnData data)
	{
		Transform transform = base.transform.Find("Spine");
		for (int i = 0; i < transform.childCount - 1; i++)
		{
			Transform child = transform.GetChild(i);
			Transform child2 = transform.GetChild(i + 1);
			if (HelperFunctions.LineCheck(child.position, child2.position, this.layerType, 0f, QueryTriggerInteraction.Ignore).transform)
			{
				return false;
			}
		}
		this.successEvent.Invoke();
		return true;
	}

	// Token: 0x04000E3A RID: 3642
	public HelperFunctions.LayerType layerType;

	// Token: 0x04000E3B RID: 3643
	public UnityEvent successEvent;
}
