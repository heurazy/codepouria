using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Zorro.Core;

// Token: 0x02000044 RID: 68
public abstract class BackpackVisuals : MonoBehaviour
{
	// Token: 0x06000335 RID: 821
	public abstract BackpackData GetBackpackData();

	// Token: 0x06000336 RID: 822 RVA: 0x00013DA4 File Offset: 0x00011FA4
	private void OnDestroy()
	{
		foreach (ValueTuple<GameObject, ushort> valueTuple in this.visualItems.Values)
		{
			PhotonNetwork.Destroy(valueTuple.Item1);
		}
	}

	// Token: 0x06000337 RID: 823 RVA: 0x00013E00 File Offset: 0x00012000
	public void RefreshVisuals()
	{
		if (!PhotonNetwork.IsMasterClient)
		{
			return;
		}
		BackpackData backpackData = this.GetBackpackData();
		if (backpackData == null)
		{
			return;
		}
		for (byte b = 0; b < 4; b += 1)
		{
			ItemSlot itemSlot = backpackData.itemSlots[(int)b];
			Optionable<ushort> optionable = (itemSlot.IsEmpty() ? Optionable<ushort>.None : Optionable<ushort>.Some(itemSlot.prefab.itemID));
			ValueTuple<GameObject, ushort> valueTuple;
			Optionable<ushort> optionable2 = (this.visualItems.TryGetValue(b, out valueTuple) ? Optionable<ushort>.Some(valueTuple.Item2) : Optionable<ushort>.None);
			if (optionable != optionable2)
			{
				if (optionable.IsSome && optionable2.IsSome)
				{
					Debug.LogError("Item Visuals Missmatch!");
				}
				else if (optionable.IsSome && optionable2.IsNone)
				{
					Debug.Log(string.Format("Spawning Backpack Visual for {0}", optionable.Value));
					GameObject gameObject = PhotonNetwork.Instantiate("0_Items/" + itemSlot.GetPrefabName(), new Vector3(0f, -500f, 0f), Quaternion.identity, 0, null);
					this.PutItemInBackpack(gameObject, b);
					gameObject.GetComponent<PhotonView>().RPC("SetItemInstanceDataRPC", RpcTarget.All, new object[] { itemSlot.data });
					this.visualItems.Add(b, new ValueTuple<GameObject, ushort>(gameObject, optionable.Value));
				}
				else if (optionable.IsNone || optionable2.IsSome)
				{
					Debug.Log(string.Format("Removing backpack visual for {0}", optionable2.Value));
					ValueTuple<GameObject, ushort> valueTuple2;
					if (!this.visualItems.TryGetValue(b, out valueTuple2))
					{
						Debug.LogError(string.Format("Failed to get spawned object from slotID {0}", b));
					}
					PhotonView component = valueTuple2.Item1.GetComponent<PhotonView>();
					Debug.Log(string.Format("Destroying photon view: {0}", component));
					PhotonNetwork.Destroy(component);
					this.visualItems.Remove(b);
				}
				else
				{
					Debug.LogError("Should be unreachable");
				}
			}
			else if (optionable.IsNone)
			{
				Debug.Log(string.Format("Not Spawning backpack visual for slot id: {0} because it's empty...", b));
			}
		}
	}

	// Token: 0x06000338 RID: 824
	protected abstract void PutItemInBackpack(GameObject visual, byte slotID);

	// Token: 0x06000339 RID: 825 RVA: 0x0001400E File Offset: 0x0001220E
	private void OnApplicationQuit()
	{
		this.m_shuttingDown = true;
	}

	// Token: 0x0600033A RID: 826 RVA: 0x00014018 File Offset: 0x00012218
	public void RemoveVisuals()
	{
		if (this.m_shuttingDown)
		{
			return;
		}
		foreach (ValueTuple<GameObject, ushort> valueTuple in this.visualItems.Values)
		{
			GameObject item = valueTuple.Item1;
			if (PhotonNetwork.IsMasterClient)
			{
				PhotonNetwork.Destroy(item);
			}
			else
			{
				item.gameObject.SetActive(false);
			}
		}
		if (PhotonNetwork.IsMasterClient)
		{
			this.visualItems.Clear();
		}
	}

	// Token: 0x0600033B RID: 827 RVA: 0x000140A4 File Offset: 0x000122A4
	public bool TryGetSpawnedItem(byte slotID, out Item item)
	{
		return this.spawnedVisualItems.TryGetValue(slotID, out item) && item != null;
	}

	// Token: 0x0600033C RID: 828 RVA: 0x000140BF File Offset: 0x000122BF
	public void SetSpawnedBackpackItem(byte slotID, Item item)
	{
		this.spawnedVisualItems[slotID] = item;
	}

	// Token: 0x040003C4 RID: 964
	public Transform[] backpackSlots;

	// Token: 0x040003C5 RID: 965
	private Dictionary<byte, ValueTuple<GameObject, ushort>> visualItems = new Dictionary<byte, ValueTuple<GameObject, ushort>>();

	// Token: 0x040003C6 RID: 966
	private Dictionary<byte, Item> spawnedVisualItems = new Dictionary<byte, Item>();

	// Token: 0x040003C7 RID: 967
	protected bool m_shuttingDown;
}
