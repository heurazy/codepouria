using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x02000227 RID: 551
public class PropGrouper : MonoBehaviour
{
	// Token: 0x06000E01 RID: 3585 RVA: 0x00046770 File Offset: 0x00044970
	public void RunAll(bool updateLightmap = true)
	{
		PropGrouper.<>c__DisplayClass2_0 CS$<>8__locals1;
		CS$<>8__locals1.<>4__this = this;
		if (!this.Verify())
		{
			return;
		}
		this.ClearAll();
		PropSpawner[] componentsInChildren = base.GetComponentsInChildren<PropSpawner>();
		List<PropSpawner> list = new List<PropSpawner>();
		CS$<>8__locals1.late = new List<PropSpawner>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			PropGrouper.PropGrouperTiming propGrouperTiming = componentsInChildren[i].GetComponentInParent<PropGrouper>().timing;
			if (propGrouperTiming == PropGrouper.PropGrouperTiming.Early)
			{
				list.Add(componentsInChildren[i]);
			}
			else if (propGrouperTiming == PropGrouper.PropGrouperTiming.Late)
			{
				CS$<>8__locals1.late.Add(componentsInChildren[i]);
			}
		}
		PropSpawner_Line[] componentsInChildren2 = base.GetComponentsInChildren<PropSpawner_Line>();
		List<PropSpawner_Line> list2 = new List<PropSpawner_Line>();
		CS$<>8__locals1.lateL = new List<PropSpawner_Line>();
		for (int j = 0; j < componentsInChildren2.Length; j++)
		{
			PropGrouper.PropGrouperTiming propGrouperTiming2 = componentsInChildren2[j].GetComponentInParent<PropGrouper>().timing;
			if (propGrouperTiming2 == PropGrouper.PropGrouperTiming.Early)
			{
				list2.Add(componentsInChildren2[j]);
			}
			else if (propGrouperTiming2 == PropGrouper.PropGrouperTiming.Late)
			{
				CS$<>8__locals1.lateL.Add(componentsInChildren2[j]);
			}
		}
		foreach (PropSpawner propSpawner in list)
		{
			propSpawner.Go();
		}
		foreach (PropSpawner_Line propSpawner_Line in list2)
		{
			propSpawner_Line.Go();
		}
	}

	// Token: 0x06000E02 RID: 3586 RVA: 0x000468D0 File Offset: 0x00044AD0
	private bool Verify()
	{
		foreach (PropSpawner propSpawner in base.GetComponentsInChildren<PropSpawner>())
		{
			if (propSpawner.props == null)
			{
				Debug.LogError("Missing spawns on " + propSpawner.name, propSpawner.gameObject);
				return false;
			}
			GameObject[] props = propSpawner.props;
			for (int j = 0; j < props.Length; j++)
			{
				if (props[j] == null)
				{
					Debug.LogError("Missing prefab on " + propSpawner.name, propSpawner.gameObject);
					return false;
				}
			}
		}
		return true;
	}

	// Token: 0x06000E03 RID: 3587 RVA: 0x00046960 File Offset: 0x00044B60
	public void ClearAll()
	{
		PropSpawner[] componentsInChildren = base.GetComponentsInChildren<PropSpawner>();
		int num = 0;
		PropSpawner[] array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Clear();
			num++;
		}
		PropSpawner_Line[] componentsInChildren2 = base.GetComponentsInChildren<PropSpawner_Line>();
		num = 0;
		PropSpawner_Line[] array2 = componentsInChildren2;
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i].Clear();
			num++;
		}
	}

	// Token: 0x06000E05 RID: 3589 RVA: 0x000469BC File Offset: 0x00044BBC
	[CompilerGenerated]
	private void <RunAll>g__Done|2_0(ref PropGrouper.<>c__DisplayClass2_0 A_1)
	{
		foreach (PropSpawner propSpawner in A_1.late)
		{
			propSpawner.Go();
		}
		foreach (PropSpawner_Line propSpawner_Line in A_1.lateL)
		{
			propSpawner_Line.Go();
		}
		PropDeleter[] componentsInChildren = base.GetComponentsInChildren<PropDeleter>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].Go();
		}
	}

	// Token: 0x04000D0E RID: 3342
	public PropGrouper.PropGrouperTiming timing;

	// Token: 0x020003A3 RID: 931
	public enum PropGrouperTiming
	{
		// Token: 0x0400136E RID: 4974
		Early,
		// Token: 0x0400136F RID: 4975
		Late
	}
}
