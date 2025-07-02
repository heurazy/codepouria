using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Zorro.Core;

// Token: 0x0200008F RID: 143
public class GameUtils : MonoBehaviourPunCallbacks
{
	// Token: 0x060004F1 RID: 1265 RVA: 0x0001C770 File Offset: 0x0001A970
	private void Awake()
	{
		GameUtils.instance = this;
		this.photonView = base.GetComponent<PhotonView>();
	}

	// Token: 0x060004F2 RID: 1266 RVA: 0x0001C784 File Offset: 0x0001A984
	public void StartFeed(int giverID, int receiverID, ushort itemID, float totalItemTime)
	{
		this.feedData.Add(new FeedData
		{
			giverID = giverID,
			receiverID = receiverID,
			itemID = itemID,
			totalItemTime = totalItemTime
		});
		Action onUpdatedFeedData = this.OnUpdatedFeedData;
		if (onUpdatedFeedData == null)
		{
			return;
		}
		onUpdatedFeedData();
	}

	// Token: 0x060004F3 RID: 1267 RVA: 0x0001C7C4 File Offset: 0x0001A9C4
	public List<FeedData> GetFeedDataForReceiver(int receiverID)
	{
		return this.feedData.FindAll((FeedData x) => x.receiverID == receiverID);
	}

	// Token: 0x060004F4 RID: 1268 RVA: 0x0001C7F8 File Offset: 0x0001A9F8
	public void EndFeed(int giverID)
	{
		for (int i = this.feedData.Count - 1; i >= 0; i--)
		{
			if (this.feedData[i].giverID == giverID)
			{
				this.feedData.RemoveAt(i);
			}
		}
		Action onUpdatedFeedData = this.OnUpdatedFeedData;
		if (onUpdatedFeedData == null)
		{
			return;
		}
		onUpdatedFeedData();
	}

	// Token: 0x060004F5 RID: 1269 RVA: 0x0001C84D File Offset: 0x0001AA4D
	private void FixedUpdate()
	{
		this.UpdateCollisionIgnores();
	}

	// Token: 0x060004F6 RID: 1270 RVA: 0x0001C858 File Offset: 0x0001AA58
	private void UpdateCollisionIgnores()
	{
		for (int i = this.ignoredCollidersCache.Count - 1; i >= 0; i--)
		{
			this.ignoredCollidersCache[i].time -= Time.fixedDeltaTime;
			if (this.ignoredCollidersCache[i].time <= 0f)
			{
				if (this.ignoredCollidersCache[i].colliderA != null && this.ignoredCollidersCache[i].colliderB != null)
				{
					Physics.IgnoreCollision(this.ignoredCollidersCache[i].colliderA, this.ignoredCollidersCache[i].colliderB, false);
				}
				this.ignoredCollidersCache.RemoveAt(i);
			}
		}
	}

	// Token: 0x060004F7 RID: 1271 RVA: 0x0001C920 File Offset: 0x0001AB20
	public void IgnoreCollisions(GameObject object1, GameObject object2, float time)
	{
		Collider[] componentsInChildren = object1.GetComponentsInChildren<Collider>();
		Collider[] componentsInChildren2 = object2.GetComponentsInChildren<Collider>();
		this.IgnoreCollisions(componentsInChildren, componentsInChildren2, time);
	}

	// Token: 0x060004F8 RID: 1272 RVA: 0x0001C944 File Offset: 0x0001AB44
	public void IgnoreCollisions(Character c, Item item)
	{
	}

	// Token: 0x060004F9 RID: 1273 RVA: 0x0001C948 File Offset: 0x0001AB48
	public void IgnoreCollisions(Collider[] collidersA, Collider[] collidersB, float time)
	{
		foreach (Collider collider in collidersA)
		{
			foreach (Collider collider2 in collidersB)
			{
				Physics.IgnoreCollision(collider, collider2);
				this.ignoredCollidersCache.Add(new GameUtils.IgnoredCollidersEntry(collider, collider2, time));
			}
		}
	}

	// Token: 0x060004FA RID: 1274 RVA: 0x0001C99F File Offset: 0x0001AB9F
	public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
	{
		base.OnPlayerEnteredRoom(newPlayer);
		if (PhotonNetwork.IsMasterClient)
		{
			GameHandler.GetService<PersistentPlayerDataService>().SyncToPlayer(newPlayer);
			this.photonView.RPC("RPC_SyncAscent", newPlayer, new object[] { Ascents.currentAscent });
		}
	}

	// Token: 0x060004FB RID: 1275 RVA: 0x0001C9DE File Offset: 0x0001ABDE
	internal void SyncAscentAll(int ascent)
	{
		if (!PhotonNetwork.IsMasterClient)
		{
			return;
		}
		this.photonView.RPC("RPC_SyncAscent", RpcTarget.All, new object[] { ascent });
	}

	// Token: 0x060004FC RID: 1276 RVA: 0x0001CA08 File Offset: 0x0001AC08
	[PunRPC]
	internal void RPC_SyncAscent(int ascent)
	{
		Ascents.currentAscent = ascent;
	}

	// Token: 0x060004FD RID: 1277 RVA: 0x0001CA10 File Offset: 0x0001AC10
	internal void ThrowBingBongAchievement()
	{
		this.photonView.RPC("ThrowBingBongAchievementRpc", RpcTarget.All, Array.Empty<object>());
	}

	// Token: 0x060004FE RID: 1278 RVA: 0x0001CA28 File Offset: 0x0001AC28
	[PunRPC]
	private void ThrowBingBongAchievementRpc()
	{
		Singleton<AchievementManager>.Instance.ThrowAchievement(ACHIEVEMENTTYPE.BingBongBadge);
	}

	// Token: 0x060004FF RID: 1279 RVA: 0x0001CA36 File Offset: 0x0001AC36
	internal void IncrementPermanentItemsPlaced()
	{
		this.photonView.RPC("IncrementPermanentItemsPlacedRpc", RpcTarget.All, Array.Empty<object>());
	}

	// Token: 0x06000500 RID: 1280 RVA: 0x0001CA4E File Offset: 0x0001AC4E
	[PunRPC]
	private void IncrementPermanentItemsPlacedRpc()
	{
		Singleton<AchievementManager>.Instance.AddToRunBasedInt(RUNBASEDVALUETYPE.PermanentItemsPlaced, 1);
	}

	// Token: 0x06000501 RID: 1281 RVA: 0x0001CA5C File Offset: 0x0001AC5C
	internal void IncrementFriendHealing(int amt, Photon.Realtime.Player target)
	{
		this.photonView.RPC("IncrementFriendHealingRpc", target, new object[] { amt });
	}

	// Token: 0x06000502 RID: 1282 RVA: 0x0001CA7E File Offset: 0x0001AC7E
	[PunRPC]
	private void IncrementFriendHealingRpc(int amt)
	{
		Singleton<AchievementManager>.Instance.AddToRunBasedInt(RUNBASEDVALUETYPE.FriendsHealedAmount, amt);
	}

	// Token: 0x06000503 RID: 1283 RVA: 0x0001CA8C File Offset: 0x0001AC8C
	internal void IncrementFriendPoisonHealing(int amt, Photon.Realtime.Player target)
	{
		this.photonView.RPC("IncrementPoisonHealedStat", target, new object[] { amt });
	}

	// Token: 0x06000504 RID: 1284 RVA: 0x0001CAAE File Offset: 0x0001ACAE
	[PunRPC]
	protected void IncrementPoisonHealedStat(int amt)
	{
		Singleton<AchievementManager>.Instance.IncrementSteamStat(STEAMSTATTYPE.PoisonHealed, amt);
	}

	// Token: 0x06000505 RID: 1285 RVA: 0x0001CABC File Offset: 0x0001ACBC
	internal void ThrowEmergencyPreparednessAchievement(Photon.Realtime.Player target)
	{
		this.photonView.RPC("ThrowEmergencyPreparednessAchievementRpc", target, Array.Empty<object>());
	}

	// Token: 0x06000506 RID: 1286 RVA: 0x0001CAD4 File Offset: 0x0001ACD4
	[PunRPC]
	private void ThrowEmergencyPreparednessAchievementRpc()
	{
		Singleton<AchievementManager>.Instance.ThrowAchievement(ACHIEVEMENTTYPE.EmergencyPreparednessBadge);
	}

	// Token: 0x06000507 RID: 1287 RVA: 0x0001CAE4 File Offset: 0x0001ACE4
	[PunRPC]
	private void InstantiateAndGrabRPC(string itemPrefabName, PhotonView characterView)
	{
		if (!PhotonNetwork.IsMasterClient)
		{
			return;
		}
		Character component = characterView.GetComponent<Character>();
		Bodypart bodypart = component.GetBodypart(BodypartType.Hip);
		PhotonNetwork.InstantiateItemRoom(itemPrefabName, bodypart.transform.position + bodypart.transform.forward * 0.5f, Quaternion.identity).GetComponent<Item>().Interact(component);
	}

	// Token: 0x06000508 RID: 1288 RVA: 0x0001CB43 File Offset: 0x0001AD43
	public void InstantiateAndGrab(Item item, Character character)
	{
		this.photonView.RPC("InstantiateAndGrabRPC", RpcTarget.MasterClient, new object[]
		{
			item.gameObject.name,
			character.photonView
		});
	}

	// Token: 0x06000509 RID: 1289 RVA: 0x0001CB74 File Offset: 0x0001AD74
	[ContextMenu("Debug All Items")]
	private void DebugAllItems()
	{
		string text = "";
		foreach (KeyValuePair<ushort, Item> keyValuePair in SingletonAsset<ItemDatabase>.Instance.itemLookup)
		{
			text = text + keyValuePair.Value.UIData.itemName + "\n";
		}
		Debug.Log(text);
		text = "";
		foreach (KeyValuePair<ushort, Item> keyValuePair2 in SingletonAsset<ItemDatabase>.Instance.itemLookup)
		{
			text = text + keyValuePair2.Value.gameObject.name + "\n";
		}
		Debug.Log(text);
	}

	// Token: 0x04000527 RID: 1319
	public static GameUtils instance;

	// Token: 0x04000528 RID: 1320
	[SerializeField]
	public List<FeedData> feedData = new List<FeedData>();

	// Token: 0x04000529 RID: 1321
	public Action OnUpdatedFeedData;

	// Token: 0x0400052A RID: 1322
	internal new PhotonView photonView;

	// Token: 0x0400052B RID: 1323
	private List<GameUtils.IgnoredCollidersEntry> ignoredCollidersCache = new List<GameUtils.IgnoredCollidersEntry>();

	// Token: 0x02000315 RID: 789
	private class IgnoredCollidersEntry
	{
		// Token: 0x060012C0 RID: 4800 RVA: 0x0005AD80 File Offset: 0x00058F80
		public IgnoredCollidersEntry(Collider A, Collider B, float time)
		{
			this.colliderA = A;
			this.colliderB = B;
			this.time = time;
		}

		// Token: 0x0400114D RID: 4429
		public Collider colliderA;

		// Token: 0x0400114E RID: 4430
		public Collider colliderB;

		// Token: 0x0400114F RID: 4431
		public float time;
	}
}
