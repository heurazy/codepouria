using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zorro.Core;

// Token: 0x020000C3 RID: 195
public class Action_SpawnGuidebookPage : ItemAction
{
	// Token: 0x06000633 RID: 1587 RVA: 0x000219E2 File Offset: 0x0001FBE2
	public override void RunAction()
	{
		if (base.character)
		{
			this.item.StartCoroutine(this.item.ConsumeDelayed(false));
			base.character.StartCoroutine(this.SpawnPageDelayed(this.PickGuidebookPage()));
		}
	}

	// Token: 0x06000634 RID: 1588 RVA: 0x00021A21 File Offset: 0x0001FC21
	public IEnumerator SpawnPageDelayed(GuidebookSpawnData itemToSpawn)
	{
		Item itemToGrab = itemToSpawn.GetComponent<Item>();
		Character c = base.character;
		float timeout = 2f;
		while (this != null)
		{
			timeout -= Time.deltaTime;
			if (timeout <= 0f)
			{
				yield break;
			}
			yield return null;
		}
		GameUtils.instance.InstantiateAndGrab(itemToGrab, c);
		yield break;
	}

	// Token: 0x06000635 RID: 1589 RVA: 0x00021A38 File Offset: 0x0001FC38
	public GuidebookSpawnData PickGuidebookPage()
	{
		int num = Singleton<AchievementManager>.Instance.GetTotalPagesSeen();
		Debug.Log("Total Pages Seen: " + num.ToString());
		if (num == 0)
		{
			return this.possiblePages[0];
		}
		if (num == 7)
		{
			return this.possiblePages[7];
		}
		this.possiblePages = this.possiblePages.Where((GuidebookSpawnData p) => p.CanSpawnRightNow()).ToList<GuidebookSpawnData>();
		if (num == 8)
		{
			return this.possiblePages[Random.Range(0, this.possiblePages.Count - 1)];
		}
		num = Mathf.Clamp(num, 0, this.possiblePages.Count - 1);
		return this.possiblePages[num];
	}

	// Token: 0x0400060F RID: 1551
	public List<GuidebookSpawnData> possiblePages;
}
