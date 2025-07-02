using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000140 RID: 320
public class SpawnItemInHand : MonoBehaviour
{
	// Token: 0x06000934 RID: 2356 RVA: 0x0002E8A3 File Offset: 0x0002CAA3
	private IEnumerator Start()
	{
		while (!Character.localCharacter)
		{
			yield return null;
		}
		yield return null;
		yield return null;
		yield return null;
		yield return new WaitForSeconds(1.5f);
		Character.localCharacter.refs.items.SpawnItemInHand(this.item.gameObject.name);
		yield break;
	}

	// Token: 0x04000834 RID: 2100
	public Item item;
}
