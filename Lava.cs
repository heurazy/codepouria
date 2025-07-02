using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Zorro.Core;

// Token: 0x020001E9 RID: 489
public class Lava : MonoBehaviour
{
	// Token: 0x06000CDD RID: 3293 RVA: 0x00040272 File Offset: 0x0003E472
	private void Start()
	{
		this.bounds = base.GetComponentInChildren<MeshRenderer>().bounds;
	}

	// Token: 0x06000CDE RID: 3294 RVA: 0x00040285 File Offset: 0x0003E485
	private void FixedUpdate()
	{
		if (Character.localCharacter == null)
		{
			return;
		}
		this.Movement();
		if (Character.localCharacter)
		{
			this.DoEffects();
			this.Heat();
		}
		this.TryCookItems();
	}

	// Token: 0x06000CDF RID: 3295 RVA: 0x000402BC File Offset: 0x0003E4BC
	private void Heat()
	{
		Character localCharacter = Character.localCharacter;
		if (localCharacter == null)
		{
			return;
		}
		this.counter += Time.deltaTime;
		if (this.OutsideBounds(localCharacter.Center))
		{
			return;
		}
		float num = localCharacter.Center.y - base.transform.position.y;
		float num2 = 1f - Mathf.Clamp01(num / this.height);
		if (num2 < 0.01f)
		{
			return;
		}
		if (this.counter < this.heatRate)
		{
			return;
		}
		this.counter = 0f;
		localCharacter.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Hot, num2 * this.heat, false);
	}

	// Token: 0x06000CE0 RID: 3296 RVA: 0x0004036C File Offset: 0x0003E56C
	private bool OutsideBounds(Vector3 pos)
	{
		return pos.x > this.bounds.max.x || pos.x < this.bounds.min.x || pos.z > this.bounds.max.z || pos.z < this.bounds.min.z;
	}

	// Token: 0x06000CE1 RID: 3297 RVA: 0x000403E4 File Offset: 0x0003E5E4
	private void DoEffects()
	{
		Character localCharacter = Character.localCharacter;
		if (this.OutsideBounds(localCharacter.Center))
		{
			return;
		}
		if (localCharacter.Center.y > base.transform.position.y)
		{
			return;
		}
		localCharacter.AddForce(Vector3.up * 80f, 0.5f, 1f);
		localCharacter.data.sinceGrounded = 0f;
		localCharacter.refs.movement.ApplyExtraDrag(0.8f, true);
		if (this.hitPlayers.Contains(localCharacter))
		{
			return;
		}
		if (localCharacter.data.dead)
		{
			return;
		}
		if (localCharacter.refs.afflictions.statusSum > 1.9f)
		{
			return;
		}
		this.HitPlayer(localCharacter);
		base.StartCoroutine(this.IHoldPlayer(localCharacter));
	}

	// Token: 0x06000CE2 RID: 3298 RVA: 0x000404B4 File Offset: 0x0003E6B4
	private void HitPlayer(Character item)
	{
		item.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Injury, 0.25f, false);
		item.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Hot, 0.25f, false);
		item.data.sinceGrounded = 0f;
	}

	// Token: 0x06000CE3 RID: 3299 RVA: 0x00040501 File Offset: 0x0003E701
	private IEnumerator IHoldPlayer(Character item)
	{
		this.hitPlayers.Add(item);
		yield return new WaitForSeconds(1f);
		this.hitPlayers.Remove(item);
		yield break;
	}

	// Token: 0x06000CE4 RID: 3300 RVA: 0x00040517 File Offset: 0x0003E717
	private void Movement()
	{
	}

	// Token: 0x06000CE5 RID: 3301 RVA: 0x0004051C File Offset: 0x0003E71C
	private void TryCookItems()
	{
		if (!PhotonNetwork.IsMasterClient)
		{
			return;
		}
		for (int i = 0; i < Item.ALL_ACTIVE_ITEMS.Count; i++)
		{
			Item item = Item.ALL_ACTIVE_ITEMS[i];
			if (item.UnityObjectExists<Item>() && !this.OutsideBounds(item.Center()) && item.cooking.canBeCooked && this.GetItemCookAmount(item) > 0f && this.itemToCookTime.TryAdd(item, 0f))
			{
				Debug.Log("Lava started cooking: " + item.GetItemName(null));
				item.GetComponent<ItemCooking>().StartCookingVisuals();
			}
		}
		this.itemToRemoveList.Clear();
		this.itemToCookList.Clear();
		foreach (Item item2 in this.itemToCookTime.Keys)
		{
			if (item2 == null)
			{
				this.itemToRemoveList.Add(item2);
			}
			else if (this.OutsideBounds(item2.Center()))
			{
				this.itemToRemoveList.Add(item2);
				item2.GetComponent<ItemCooking>().CancelCookingVisuals();
			}
			else
			{
				this.itemToCookList.Add(item2);
			}
		}
		foreach (Item item3 in this.itemToCookList)
		{
			float num = this.GetItemCookAmount(item3) * Time.deltaTime;
			Dictionary<Item, float> dictionary = this.itemToCookTime;
			Item item4 = item3;
			dictionary[item4] += num;
			if (this.itemToCookTime[item3] >= 1f)
			{
				Debug.Log("Lava finished cooking: " + item3.GetItemName(null));
				item3.GetComponent<ItemCooking>().FinishCooking();
				this.itemToCookTime[item3] = 0f;
			}
		}
		foreach (Item item5 in this.itemToRemoveList)
		{
			this.itemToCookTime.Remove(item5);
		}
	}

	// Token: 0x06000CE6 RID: 3302 RVA: 0x00040768 File Offset: 0x0003E968
	private float GetItemCookAmount(Item item)
	{
		float num = item.Center().y - base.transform.position.y;
		float num2 = 1f - Mathf.Clamp01(num / this.height);
		if (num2 < 0.01f)
		{
			return 0f;
		}
		return num2 * 0.7f;
	}

	// Token: 0x04000BDC RID: 3036
	private List<Character> hitPlayers = new List<Character>();

	// Token: 0x04000BDD RID: 3037
	public float heatRate = 0.5f;

	// Token: 0x04000BDE RID: 3038
	public float heat = 0.02f;

	// Token: 0x04000BDF RID: 3039
	public float height = 10f;

	// Token: 0x04000BE0 RID: 3040
	private Bounds bounds;

	// Token: 0x04000BE1 RID: 3041
	private float counter;

	// Token: 0x04000BE2 RID: 3042
	public Dictionary<Item, float> itemToCookTime = new Dictionary<Item, float>();

	// Token: 0x04000BE3 RID: 3043
	private List<Item> itemToRemoveList = new List<Item>();

	// Token: 0x04000BE4 RID: 3044
	private List<Item> itemToCookList = new List<Item>();
}
