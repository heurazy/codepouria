using System;
using System.Collections;
using UnityEngine;

// Token: 0x020000B1 RID: 177
public class Action_ConsumeAndSpawn : ItemAction
{
	// Token: 0x06000606 RID: 1542 RVA: 0x0002137E File Offset: 0x0001F57E
	public override void RunAction()
	{
		if (base.character)
		{
			this.item.StartCoroutine(this.item.ConsumeDelayed(false));
			base.character.StartCoroutine(this.SpawnItemDelayed());
		}
	}

	// Token: 0x06000607 RID: 1543 RVA: 0x000213B7 File Offset: 0x0001F5B7
	public IEnumerator SpawnItemDelayed()
	{
		Character c = base.character;
		Item item = this.itemToSpawn;
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
		GameUtils.instance.InstantiateAndGrab(item, c);
		yield break;
	}

	// Token: 0x040005F8 RID: 1528
	public Item itemToSpawn;
}
