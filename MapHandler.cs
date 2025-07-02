using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Zorro.Core;
using Zorro.Core.CLI;
using Zorro.PhotonUtility;

// Token: 0x020000F0 RID: 240
public class MapHandler : Singleton<MapHandler>
{
	// Token: 0x0600072D RID: 1837 RVA: 0x00025E00 File Offset: 0x00024000
	protected override void Awake()
	{
		base.Awake();
		this.debugCommandHandle = CustomCommands<CustomCommandType>.RegisterListener<SyncMapHandlerDebugCommandPackage>(new Action<SyncMapHandlerDebugCommandPackage>(this.OnPackageHandle));
	}

	// Token: 0x0600072E RID: 1838 RVA: 0x00025E1F File Offset: 0x0002401F
	public override void OnDestroy()
	{
		base.OnDestroy();
		CustomCommands<CustomCommandType>.UnregisterListener(this.debugCommandHandle);
	}

	// Token: 0x0600072F RID: 1839 RVA: 0x00025E32 File Offset: 0x00024032
	private IEnumerator Start()
	{
		yield return null;
		for (int i = 1; i < this.segments.Length; i++)
		{
			this.segments[i].segmentParent.SetActive(false);
			if (this.segments[i].segmentCampfire != null)
			{
				this.segments[i].segmentCampfire.SetActive(false);
			}
			Debug.Log(string.Format("Disabling segment: {0} with parent: {1}", i, this.segments[i].segmentParent.name));
		}
		this.segments[0].wallNext.SetActive(true);
		yield break;
	}

	// Token: 0x06000730 RID: 1840 RVA: 0x00025E44 File Offset: 0x00024044
	private void Update()
	{
		if (PhotonNetwork.InRoom && PhotonNetwork.IsMasterClient && !this.hasSpawnedInitialSpawners)
		{
			Spawner[] componentsInChildren = this.segments[0].segmentParent.GetComponentsInChildren<Spawner>();
			this.hasSpawnedInitialSpawners = true;
			foreach (Spawner spawner in componentsInChildren)
			{
				this.viewsToDestoryIfNotAlreadyWhenSwitchingSegments.AddRange(spawner.TrySpawnItems());
			}
		}
		bool flag = true;
		List<global::Player> allPlayers = PlayerHandler.GetAllPlayers();
		using (List<global::Player>.Enumerator enumerator = allPlayers.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (!enumerator.Current.hasClosedEndScreen)
				{
					flag = false;
					break;
				}
			}
		}
		EndScreenStatus endScreenStatus;
		if (flag && allPlayers.Count > 0 && !GameHandler.TryGetStatus<EndScreenStatus>(out endScreenStatus) && !this.hasEnded)
		{
			bool flag2 = Character.localCharacter.refs.stats.won || Character.localCharacter.refs.stats.somebodyElseWon;
			this.hasEnded = true;
			if (flag2)
			{
				GameHandler.AddStatus<EndScreenStatus>(new EndScreenStatus());
				Singleton<PeakHandler>.Instance.EndScreenComplete();
			}
			else
			{
				Debug.LogError("Everyone has closed end screen.. Loading airport");
				Singleton<GameOverHandler>.Instance.LoadAirport();
			}
		}
		bool flag3 = false;
		using (List<global::Player>.Enumerator enumerator = allPlayers.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.doneWithCutscene)
				{
					flag3 = true;
					break;
				}
			}
		}
		if (flag3 && allPlayers.Count > 0 && !this.hasCutsceneEnded)
		{
			this.hasCutsceneEnded = true;
			Debug.Log("Everyone is done with cutscene, loading airport");
			GameHandler.AddStatus<SceneSwitchingStatus>(new SceneSwitchingStatus());
			RetrievableResourceSingleton<LoadingScreenHandler>.Instance.Load(LoadingScreen.LoadingScreenType.Basic, null, new IEnumerator[] { RetrievableResourceSingleton<LoadingScreenHandler>.Instance.LoadSceneProcess("Airport", true, true, 1f) });
		}
	}

	// Token: 0x06000731 RID: 1841 RVA: 0x00026018 File Offset: 0x00024218
	public void GoToSegment(Segment s)
	{
		if ((int)s <= this.currentSegment)
		{
			Debug.LogError(string.Format("Trying to transition to segment already passed: {0}", s));
			return;
		}
		base.StartCoroutine(this.<GoToSegment>g__ShowNextSegmentCoroutine|14_0());
	}

	// Token: 0x06000732 RID: 1842 RVA: 0x00026046 File Offset: 0x00024246
	[ConsoleCommand]
	public static void JumpToSegment(Segment segment)
	{
		MapHandler.JumpToSegmentLogic(segment, (from player in PlayerHandler.GetAllPlayers()
			select player.photonView.Owner.ActorNumber).ToHashSet<int>(), true);
	}

	// Token: 0x06000733 RID: 1843 RVA: 0x00026080 File Offset: 0x00024280
	private static void JumpToSegmentLogic(Segment segment, HashSet<int> playersToTeleport, bool sendToEveryone)
	{
		Singleton<MapHandler>.Instance.currentSegment = (int)segment;
		Debug.Log(string.Format("Jumping to segment: {0}", segment));
		foreach (MapHandler.MapSegment mapSegment in Singleton<MapHandler>.Instance.segments)
		{
			mapSegment.segmentParent.SetActive(false);
			if (mapSegment.segmentCampfire)
			{
				mapSegment.segmentCampfire.SetActive(false);
			}
			if (mapSegment.wallNext)
			{
				mapSegment.wallNext.gameObject.SetActive(false);
			}
			if (mapSegment.wallPrevious)
			{
				mapSegment.wallPrevious.gameObject.SetActive(false);
			}
		}
		int num = (int)segment;
		if (segment == Segment.TheKiln)
		{
			num--;
		}
		else if (segment == Segment.Peak)
		{
			num -= 2;
		}
		MapHandler.MapSegment mapSegment2 = Singleton<MapHandler>.Instance.segments[num];
		mapSegment2.segmentParent.SetActive(true);
		if (mapSegment2.segmentCampfire)
		{
			mapSegment2.segmentCampfire.SetActive(true);
		}
		if (mapSegment2.wallNext)
		{
			mapSegment2.wallNext.gameObject.SetActive(true);
		}
		if (mapSegment2.wallPrevious)
		{
			mapSegment2.wallPrevious.gameObject.SetActive(true);
		}
		Vector3 vector = mapSegment2.reconnectSpawnPos.position;
		if (segment == Segment.TheKiln)
		{
			vector = Singleton<MapHandler>.Instance.respawnTheKiln.position;
		}
		else if (segment == Segment.Peak)
		{
			vector = Singleton<MapHandler>.Instance.respawnThePeak.position;
		}
		if (num > 0)
		{
			MapHandler.MapSegment mapSegment3 = Singleton<MapHandler>.Instance.segments[num - 1];
			if (mapSegment3.segmentCampfire != null)
			{
				mapSegment3.segmentCampfire.SetActive(true);
			}
		}
		if (PhotonNetwork.IsMasterClient)
		{
			Spawner[] componentsInChildren = mapSegment2.segmentParent.GetComponentsInChildren<Spawner>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].TrySpawnItems();
			}
		}
		Singleton<OrbFogHandler>.Instance.SetFogOrigin(num);
		if (PhotonNetwork.IsMasterClient)
		{
			Debug.Log(string.Format("Teleporting all players to {0} campfire..", segment));
			foreach (Character character in PlayerHandler.GetAllPlayerCharacters())
			{
				if (playersToTeleport.Contains(character.photonView.Owner.ActorNumber))
				{
					character.photonView.RPC("WarpPlayerRPC", RpcTarget.All, new object[] { vector, false });
				}
			}
		}
		if (sendToEveryone)
		{
			CustomCommands<CustomCommandType>.SendPackage(new SyncMapHandlerDebugCommandPackage(segment, Array.Empty<int>()), ReceiverGroup.Others);
		}
	}

	// Token: 0x06000734 RID: 1844 RVA: 0x00026314 File Offset: 0x00024514
	private void OnPackageHandle(SyncMapHandlerDebugCommandPackage p)
	{
		MapHandler.JumpToSegmentLogic(p.Segment, p.PlayerToTeleport.ToHashSet<int>(), false);
	}

	// Token: 0x06000735 RID: 1845 RVA: 0x0002632D File Offset: 0x0002452D
	public Segment GetCurrentSegment()
	{
		return (Segment)this.currentSegment;
	}

	// Token: 0x06000737 RID: 1847 RVA: 0x00026349 File Offset: 0x00024549
	[CompilerGenerated]
	private IEnumerator <GoToSegment>g__ShowNextSegmentCoroutine|14_0()
	{
		MapHandler.<>c__DisplayClass14_0 CS$<>8__locals1 = new MapHandler.<>c__DisplayClass14_0();
		CS$<>8__locals1.startSegment = this.currentSegment;
		this.currentSegment++;
		OrbFogHandler orbFogHandler = Singleton<OrbFogHandler>.Instance;
		yield return orbFogHandler.WaitForFogCatchUp();
		yield return new WaitForSecondsRealtime(1f);
		this.segments[CS$<>8__locals1.startSegment].segmentParent.SetActive(false);
		yield return null;
		this.segments[this.currentSegment].segmentParent.SetActive(true);
		EnablingSubstep[] array = (from substep in this.segments[this.currentSegment].segmentParent.GetComponentsInChildren<EnablingSubstep>()
			where substep.gameObject.activeSelf
			select substep).ToArray<EnablingSubstep>();
		EnablingSubstep[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i].gameObject.SetActive(false);
		}
		if (this.segments[CS$<>8__locals1.startSegment].wallNext)
		{
			this.segments[CS$<>8__locals1.startSegment].wallNext.SetActive(false);
		}
		if (this.segments[CS$<>8__locals1.startSegment].wallPrevious)
		{
			this.segments[CS$<>8__locals1.startSegment].wallPrevious.SetActive(false);
		}
		if (this.segments[this.currentSegment].wallNext)
		{
			this.segments[this.currentSegment].wallNext.SetActive(true);
		}
		if (this.segments[this.currentSegment].wallPrevious)
		{
			this.segments[this.currentSegment].wallPrevious.SetActive(true);
		}
		this.segments[this.currentSegment].segmentParent.SetActive(true);
		foreach (EnablingSubstep substep2 in array)
		{
			yield return new WaitForSeconds(0.15f);
			substep2.gameObject.SetActive(true);
			Debug.Log(string.Format("Enabling substep: {0}", substep2));
			substep2 = null;
		}
		EnablingSubstep[] array3 = null;
		if (PhotonNetwork.IsMasterClient)
		{
			foreach (PhotonView photonView in this.viewsToDestoryIfNotAlreadyWhenSwitchingSegments)
			{
				if (photonView != null && Vector3.Distance(photonView.transform.position, MainCamera.instance.transform.position) > 50f)
				{
					PhotonNetwork.Destroy(photonView);
				}
			}
			this.viewsToDestoryIfNotAlreadyWhenSwitchingSegments.Clear();
		}
		if (this.segments[this.currentSegment].segmentCampfire != null)
		{
			this.segments[this.currentSegment].segmentCampfire.SetActive(true);
		}
		if (this.segments.WithinRange(CS$<>8__locals1.startSegment - 1) && this.segments[CS$<>8__locals1.startSegment - 1].segmentCampfire != null)
		{
			this.segments[CS$<>8__locals1.startSegment - 1].segmentCampfire.SetActive(false);
		}
		if (PhotonNetwork.IsMasterClient)
		{
			foreach (Spawner spawner in this.segments[this.currentSegment].segmentParent.GetComponentsInChildren<Spawner>())
			{
				this.viewsToDestoryIfNotAlreadyWhenSwitchingSegments.AddRange(spawner.TrySpawnItems());
			}
		}
		yield return new WaitForSeconds(0.5f);
		base.StartCoroutine(CS$<>8__locals1.<GoToSegment>g__ShowTitleText|2());
		yield return orbFogHandler.WaitForReveal();
		orbFogHandler.SetFogOrigin(this.currentSegment);
		yield break;
	}

	// Token: 0x040006C2 RID: 1730
	public MapHandler.MapSegment[] segments;

	// Token: 0x040006C3 RID: 1731
	public Transform respawnTheKiln;

	// Token: 0x040006C4 RID: 1732
	public Transform respawnThePeak;

	// Token: 0x040006C5 RID: 1733
	private int currentSegment;

	// Token: 0x040006C6 RID: 1734
	private bool hasSpawnedInitialSpawners;

	// Token: 0x040006C7 RID: 1735
	private ListenerHandle debugCommandHandle;

	// Token: 0x040006C8 RID: 1736
	private bool hasEnded;

	// Token: 0x040006C9 RID: 1737
	private bool hasCutsceneEnded;

	// Token: 0x040006CA RID: 1738
	private List<PhotonView> viewsToDestoryIfNotAlreadyWhenSwitchingSegments = new List<PhotonView>();

	// Token: 0x02000337 RID: 823
	[Serializable]
	public class MapSegment
	{
		// Token: 0x040011DA RID: 4570
		public GameObject segmentParent;

		// Token: 0x040011DB RID: 4571
		public GameObject segmentCampfire;

		// Token: 0x040011DC RID: 4572
		public GameObject wallNext;

		// Token: 0x040011DD RID: 4573
		public GameObject wallPrevious;

		// Token: 0x040011DE RID: 4574
		public Transform reconnectSpawnPos;
	}
}
