using System;
using System.Collections;
using System.Collections.Generic;
using Steamworks;
using UnityEngine;
using UnityEngine.InputSystem;
using Zorro.Core;
using Zorro.Core.CLI;

// Token: 0x02000036 RID: 54
[ConsoleClassCustomizer("Achievements")]
public class AchievementManager : Singleton<AchievementManager>
{
	// Token: 0x0600029F RID: 671 RVA: 0x000118C4 File Offset: 0x0000FAC4
	public void DebugGetAchievement()
	{
		this.ThrowAchievement(this.debugAchievement);
	}

	// Token: 0x17000029 RID: 41
	// (get) Token: 0x060002A0 RID: 672 RVA: 0x000118D2 File Offset: 0x0000FAD2
	// (set) Token: 0x060002A1 RID: 673 RVA: 0x000118DA File Offset: 0x0000FADA
	public bool gotStats { get; private set; }

	// Token: 0x060002A2 RID: 674 RVA: 0x000118E4 File Offset: 0x0000FAE4
	public void InitRunBasedValues()
	{
		this.runBasedValues = new Dictionary<RUNBASEDVALUETYPE, object>();
		this.runBasedAchievements = new List<AchievementManager.RunBasedAchievementData>
		{
			new AchievementManager.RunBasedAchievementData(ACHIEVEMENTTYPE.KnotTyingBadge, RUNBASEDVALUETYPE.RopePlaced, 100),
			new AchievementManager.RunBasedAchievementData(ACHIEVEMENTTYPE.ClutchBadge, RUNBASEDVALUETYPE.ScoutsResurrected, 3),
			new AchievementManager.RunBasedAchievementData(ACHIEVEMENTTYPE.PlundererBadge, RUNBASEDVALUETYPE.LuggageOpened, 15),
			new AchievementManager.RunBasedAchievementData(ACHIEVEMENTTYPE.FirstAidBadge, RUNBASEDVALUETYPE.FriendsHealedAmount, 20)
		};
		this.PrimeExistingAchievements();
		this.runBasedFruitsEaten.Clear();
		this.nonToxicMushroomsEaten.Clear();
		this.gourmandRequirementsEaten.Clear();
		this.achievementsEarnedThisRun.Clear();
		this.completedAscentsThisRun.Clear();
	}

	// Token: 0x060002A3 RID: 675 RVA: 0x00011983 File Offset: 0x0000FB83
	[ConsoleCommand]
	public static void ClearAchievements()
	{
		Singleton<AchievementManager>.Instance.ResetAllUserStats();
	}

	// Token: 0x060002A4 RID: 676 RVA: 0x0001198F File Offset: 0x0000FB8F
	[ContextMenu("RESET ALL DATA")]
	private void ResetAllUserStats()
	{
		SteamUserStats.ResetAllStats(true);
		this.StoreUserStats();
		this.InitRunBasedValues();
	}

	// Token: 0x060002A5 RID: 677 RVA: 0x000119A4 File Offset: 0x0000FBA4
	private void Start()
	{
		base.StartCoroutine(this.GetUserStatsRoutine());
		this.InitRunBasedValues();
		this.SubscribeToEvents();
	}

	// Token: 0x060002A6 RID: 678 RVA: 0x000119BF File Offset: 0x0000FBBF
	public override void OnDestroy()
	{
		base.OnDestroy();
		this.UnsubscribeFromEvents();
	}

	// Token: 0x060002A7 RID: 679 RVA: 0x000119CD File Offset: 0x0000FBCD
	private IEnumerator GetUserStatsRoutine()
	{
		while (SteamManager.Instance == null)
		{
			Debug.Log("Waiting for steam manager");
			yield return null;
		}
		while (!SteamManager.Initialized)
		{
			yield return null;
		}
		while (!this.gotStats)
		{
			SteamUserStats.RequestUserStats(SteamUser.GetSteamID());
			yield return new WaitForSeconds(2f);
		}
		yield break;
	}

	// Token: 0x060002A8 RID: 680 RVA: 0x000119DC File Offset: 0x0000FBDC
	private void StoreUserStats()
	{
		base.StartCoroutine(this.StoreUserStatsRoutine());
	}

	// Token: 0x060002A9 RID: 681 RVA: 0x000119EB File Offset: 0x0000FBEB
	private IEnumerator StoreUserStatsRoutine()
	{
		while (!SteamManager.Initialized)
		{
			yield return null;
		}
		SteamUserStats.StoreStats();
		yield break;
	}

	// Token: 0x060002AA RID: 682 RVA: 0x000119F3 File Offset: 0x0000FBF3
	public void Update()
	{
		bool wasPressedThisFrame = Keyboard.current.lKey.wasPressedThisFrame;
	}

	// Token: 0x060002AB RID: 683 RVA: 0x00011A08 File Offset: 0x0000FC08
	public int GetMaxAscent()
	{
		if (this.useDebugAscent)
		{
			return this.debugAscent;
		}
		int num;
		if (Singleton<AchievementManager>.Instance.GetSteamStatInt(STEAMSTATTYPE.MaxAscent, out num))
		{
			return num;
		}
		return 0;
	}

	// Token: 0x060002AC RID: 684 RVA: 0x00011A38 File Offset: 0x0000FC38
	public bool IsAchievementUnlocked(ACHIEVEMENTTYPE achievementType)
	{
		if (!SteamManager.Initialized)
		{
			return false;
		}
		bool flag;
		SteamUserStats.GetAchievement(achievementType.ToString(), out flag);
		return flag;
	}

	// Token: 0x060002AD RID: 685 RVA: 0x00011A64 File Offset: 0x0000FC64
	private void CheckRunBasedAchievement(RUNBASEDVALUETYPE type)
	{
		foreach (AchievementManager.RunBasedAchievementData runBasedAchievementData in this.runBasedAchievements)
		{
			if (runBasedAchievementData.valueType == type && runBasedAchievementData.IsAchieved())
			{
				this.ThrowAchievement(runBasedAchievementData.achievementType);
			}
		}
	}

	// Token: 0x060002AE RID: 686 RVA: 0x00011AD0 File Offset: 0x0000FCD0
	private void PrimeExistingAchievements()
	{
		this.steamAchievementsPreviouslyUnlocked.Clear();
		foreach (object obj in Enum.GetValues(typeof(ACHIEVEMENTTYPE)))
		{
			ACHIEVEMENTTYPE achievementtype = (ACHIEVEMENTTYPE)obj;
			bool flag;
			if (SteamUserStats.GetAchievement(achievementtype.ToString(), out flag) && flag)
			{
				this.steamAchievementsPreviouslyUnlocked.Add(achievementtype);
			}
		}
	}

	// Token: 0x060002AF RID: 687 RVA: 0x00011B5C File Offset: 0x0000FD5C
	private void CheckNewAchievements()
	{
		foreach (object obj in Enum.GetValues(typeof(ACHIEVEMENTTYPE)))
		{
			ACHIEVEMENTTYPE achievementtype = (ACHIEVEMENTTYPE)obj;
			bool flag;
			if (!this.steamAchievementsPreviouslyUnlocked.Contains(achievementtype) && SteamUserStats.GetAchievement(achievementtype.ToString(), out flag) && flag)
			{
				Debug.Log("EARNED ACHIEVEMENT: " + achievementtype.ToString());
				if (!this.achievementsEarnedThisRun.Contains(achievementtype))
				{
					this.achievementsEarnedThisRun.Add(achievementtype);
				}
				this.steamAchievementsPreviouslyUnlocked.Add(achievementtype);
			}
		}
	}

	// Token: 0x060002B0 RID: 688 RVA: 0x00011C20 File Offset: 0x0000FE20
	public void SetSteamStat(STEAMSTATTYPE steamStatType, int value)
	{
		if (!SteamManager.Initialized)
		{
			return;
		}
		SteamUserStats.SetStat(steamStatType.ToString(), value);
		this.StoreUserStats();
		this.CheckNewAchievements();
	}

	// Token: 0x060002B1 RID: 689 RVA: 0x00011C4A File Offset: 0x0000FE4A
	public bool GetSteamStatInt(STEAMSTATTYPE steamStatType, out int value)
	{
		if (!SteamManager.Initialized)
		{
			value = -1;
			return false;
		}
		return SteamUserStats.GetStat(steamStatType.ToString(), out value);
	}

	// Token: 0x060002B2 RID: 690 RVA: 0x00011C6C File Offset: 0x0000FE6C
	public int GetTotalPagesSeen()
	{
		if (!SteamManager.Initialized)
		{
			return 0;
		}
		int num = 0;
		for (int i = 0; i < 8; i++)
		{
			int num2;
			SteamUserStats.GetStat("ReadGuidebookPage_" + i.ToString(), out num2);
			if (num2 != 1)
			{
				return num;
			}
			num++;
		}
		return num;
	}

	// Token: 0x060002B3 RID: 691 RVA: 0x00011CB8 File Offset: 0x0000FEB8
	public bool SeenGuidebookPage(int index)
	{
		int num;
		SteamUserStats.GetStat("ReadGuidebookPage_" + index.ToString(), out num);
		return num == 1;
	}

	// Token: 0x060002B4 RID: 692 RVA: 0x00011CE4 File Offset: 0x0000FEE4
	public void TriggerSeenGuidebookPage(int index)
	{
		SteamUserStats.SetStat("ReadGuidebookPage_" + index.ToString(), 1);
		this.SetSteamStat(STEAMSTATTYPE.TotalPagesRead, this.GetTotalPagesSeen());
		this.StoreUserStats();
		Debug.Log("Saw page " + index.ToString());
	}

	// Token: 0x060002B5 RID: 693 RVA: 0x00011D34 File Offset: 0x0000FF34
	public void IncrementSteamStat(STEAMSTATTYPE steamStatType, int value)
	{
		try
		{
			if (SteamManager.Initialized)
			{
				int num;
				SteamUserStats.GetStat(steamStatType.ToString(), out num);
				num += value;
				SteamUserStats.SetStat(steamStatType.ToString(), num);
				this.StoreUserStats();
				this.CheckNewAchievements();
			}
		}
		catch (Exception ex)
		{
			Debug.LogError(ex);
		}
	}

	// Token: 0x060002B6 RID: 694 RVA: 0x00011D9C File Offset: 0x0000FF9C
	[ConsoleCommand]
	internal static void Grant(ACHIEVEMENTTYPE type)
	{
		Singleton<AchievementManager>.Instance.ThrowAchievement(type);
	}

	// Token: 0x060002B7 RID: 695 RVA: 0x00011DAC File Offset: 0x0000FFAC
	internal void ThrowAchievement(ACHIEVEMENTTYPE type)
	{
		try
		{
			if (SteamManager.Initialized)
			{
				bool flag;
				SteamUserStats.GetAchievement(type.ToString(), out flag);
				if (!flag && !this.achievementsEarnedThisRun.Contains(type))
				{
					this.achievementsEarnedThisRun.Add(type);
					Debug.Log("Throwing achievement: " + type.ToString());
					SteamUserStats.SetAchievement(type.ToString());
				}
				this.StoreUserStats();
			}
		}
		catch (Exception ex)
		{
			Debug.LogError(ex);
		}
	}

	// Token: 0x060002B8 RID: 696 RVA: 0x00011E44 File Offset: 0x00010044
	public void SetRunBasedInt(RUNBASEDVALUETYPE type, int value)
	{
		this.runBasedValues[type] = value;
		this.CheckRunBasedAchievement(type);
	}

	// Token: 0x060002B9 RID: 697 RVA: 0x00011E60 File Offset: 0x00010060
	public int GetRunBasedInt(RUNBASEDVALUETYPE type)
	{
		if (!this.runBasedValues.ContainsKey(type))
		{
			this.SetRunBasedInt(type, 0);
		}
		try
		{
			return (int)this.runBasedValues[type];
		}
		catch
		{
			Debug.LogError(string.Format("Tried to retrieve run based int {0} that is not an int.", type));
		}
		return 0;
	}

	// Token: 0x060002BA RID: 698 RVA: 0x00011EC4 File Offset: 0x000100C4
	public void AddToRunBasedInt(RUNBASEDVALUETYPE type, int valueToAdd)
	{
		int runBasedInt = this.GetRunBasedInt(type);
		this.SetRunBasedInt(type, runBasedInt + valueToAdd);
	}

	// Token: 0x060002BB RID: 699 RVA: 0x00011EE3 File Offset: 0x000100E3
	public void SetRunBasedFloat(RUNBASEDVALUETYPE type, float value)
	{
		this.runBasedValues[type] = value;
		this.CheckRunBasedAchievement(type);
	}

	// Token: 0x060002BC RID: 700 RVA: 0x00011F00 File Offset: 0x00010100
	public float GetRunBasedFloat(RUNBASEDVALUETYPE type)
	{
		if (!this.runBasedValues.ContainsKey(type))
		{
			this.SetRunBasedFloat(type, 0f);
		}
		try
		{
			return Convert.ToSingle(this.runBasedValues[type]);
		}
		catch (Exception ex)
		{
			Debug.LogError(string.Format("Tried to retrieve run based float {0} that is not a float.\n{1}", type, ex.ToString()));
		}
		return 0f;
	}

	// Token: 0x060002BD RID: 701 RVA: 0x00011F74 File Offset: 0x00010174
	public void AddToRunBasedFloat(RUNBASEDVALUETYPE type, float valueToAdd)
	{
		float runBasedFloat = this.GetRunBasedFloat(type);
		this.SetRunBasedFloat(type, runBasedFloat + valueToAdd);
	}

	// Token: 0x060002BE RID: 702 RVA: 0x00011F94 File Offset: 0x00010194
	private void SubscribeToEvents()
	{
		GlobalEvents.OnItemRequested = (Action<Item, Character>)Delegate.Combine(GlobalEvents.OnItemRequested, new Action<Item, Character>(this.TestRequestedItem));
		GlobalEvents.OnItemConsumed = (Action<Item, Character>)Delegate.Combine(GlobalEvents.OnItemConsumed, new Action<Item, Character>(this.TestItemConsumed));
		GlobalEvents.OnRespawnChestOpened = (Action<RespawnChest, Character>)Delegate.Combine(GlobalEvents.OnRespawnChestOpened, new Action<RespawnChest, Character>(this.TestRespawnChestOpened));
		GlobalEvents.OnLuggageOpened = (Action<Luggage, Character>)Delegate.Combine(GlobalEvents.OnLuggageOpened, new Action<Luggage, Character>(this.TestLuggageOpened));
		GlobalEvents.OnLocalCharacterWonRun = (Action)Delegate.Combine(GlobalEvents.OnLocalCharacterWonRun, new Action(this.TestWonRun));
		GlobalEvents.OnCharacterPassedOut = (Action<Character>)Delegate.Combine(GlobalEvents.OnCharacterPassedOut, new Action<Character>(this.TestCharacterPassedOut));
		GlobalEvents.OnSomeoneWonRun = (Action)Delegate.Combine(GlobalEvents.OnSomeoneWonRun, new Action(this.TestSomeoneWonRun));
		Callback<UserStatsReceived_t>.Create(new Callback<UserStatsReceived_t>.DispatchDelegate(this.OnUserStatsRecieved));
	}

	// Token: 0x060002BF RID: 703 RVA: 0x00012094 File Offset: 0x00010294
	private void UnsubscribeFromEvents()
	{
		GlobalEvents.OnItemRequested = (Action<Item, Character>)Delegate.Remove(GlobalEvents.OnItemRequested, new Action<Item, Character>(this.TestRequestedItem));
		GlobalEvents.OnItemConsumed = (Action<Item, Character>)Delegate.Remove(GlobalEvents.OnItemConsumed, new Action<Item, Character>(this.TestItemConsumed));
		GlobalEvents.OnRespawnChestOpened = (Action<RespawnChest, Character>)Delegate.Remove(GlobalEvents.OnRespawnChestOpened, new Action<RespawnChest, Character>(this.TestRespawnChestOpened));
		GlobalEvents.OnLuggageOpened = (Action<Luggage, Character>)Delegate.Remove(GlobalEvents.OnLuggageOpened, new Action<Luggage, Character>(this.TestLuggageOpened));
		GlobalEvents.OnLocalCharacterWonRun = (Action)Delegate.Remove(GlobalEvents.OnLocalCharacterWonRun, new Action(this.TestWonRun));
		GlobalEvents.OnCharacterPassedOut = (Action<Character>)Delegate.Remove(GlobalEvents.OnCharacterPassedOut, new Action<Character>(this.TestCharacterPassedOut));
		GlobalEvents.OnSomeoneWonRun = (Action)Delegate.Remove(GlobalEvents.OnSomeoneWonRun, new Action(this.TestSomeoneWonRun));
	}

	// Token: 0x060002C0 RID: 704 RVA: 0x00012181 File Offset: 0x00010381
	private void OnUserStatsRecieved(UserStatsReceived_t result)
	{
		if (result.m_eResult != EResult.k_EResultFail)
		{
			this.gotStats = true;
		}
	}

	// Token: 0x060002C1 RID: 705 RVA: 0x00012193 File Offset: 0x00010393
	private void TestRequestedItem(Item item, Character character)
	{
		if (character.IsLocal && item.itemTags.HasFlag(Item.ItemTags.Mystical))
		{
			this.ThrowAchievement(ACHIEVEMENTTYPE.EsotericaBadge);
		}
	}

	// Token: 0x060002C2 RID: 706 RVA: 0x000121C0 File Offset: 0x000103C0
	private void TestItemConsumed(Item item, Character character)
	{
		if (character.IsLocal)
		{
			if (item.itemTags.HasFlag(Item.ItemTags.Berry))
			{
				this.AddToRunBasedFruitsEaten(item.itemID);
			}
			if (item.itemTags.HasFlag(Item.ItemTags.PackagedFood))
			{
				this.AddToRunBasedInt(RUNBASEDVALUETYPE.PackagedFoodEaten, 1);
			}
			if (item.itemTags.HasFlag(Item.ItemTags.Mushroom) && item.GetComponent<Action_InflictPoison>() == null)
			{
				this.AddToNonToxicMushroomsEaten(item.itemID);
			}
			if (item.itemTags.HasFlag(Item.ItemTags.GourmandRequirement))
			{
				this.AddToGourmandRequirementsEaten(item.itemID);
			}
		}
	}

	// Token: 0x060002C3 RID: 707 RVA: 0x00012274 File Offset: 0x00010474
	private void TestRespawnChestOpened(RespawnChest chest, Character opener)
	{
		if (opener.IsLocal)
		{
			foreach (Character character in Character.AllCharacters)
			{
				if (character.data.dead || character.data.fullyPassedOut)
				{
					this.AddToRunBasedInt(RUNBASEDVALUETYPE.ScoutsResurrected, 1);
				}
			}
		}
	}

	// Token: 0x060002C4 RID: 708 RVA: 0x000122EC File Offset: 0x000104EC
	private void TestLuggageOpened(Luggage luggage, Character opener)
	{
		if (opener.IsLocal)
		{
			this.AddToRunBasedInt(RUNBASEDVALUETYPE.LuggageOpened, 1);
		}
	}

	// Token: 0x060002C5 RID: 709 RVA: 0x00012300 File Offset: 0x00010500
	private void TestWonRun()
	{
		this.ThrowAchievement(ACHIEVEMENTTYPE.PeakBadge);
		this.IncrementSteamStat(STEAMSTATTYPE.TimesPeaked, 1);
		if (Character.AllCharacters.Count == 1)
		{
			this.ThrowAchievement(ACHIEVEMENTTYPE.LoneWolfBadge);
		}
		if (this.GetRunBasedFloat(RUNBASEDVALUETYPE.FallDamageTaken) == 0f)
		{
			this.ThrowAchievement(ACHIEVEMENTTYPE.BalloonBadge);
		}
		int num = Mathf.FloorToInt(Singleton<RunManager>.Instance.timeSinceRunStarted);
		if ((float)num <= 3600f)
		{
			this.ThrowAchievement(ACHIEVEMENTTYPE.SpeedClimberBadge);
		}
		int num2;
		this.GetSteamStatInt(STEAMSTATTYPE.BestTime, out num2);
		if (num < num2)
		{
			this.SetSteamStat(STEAMSTATTYPE.BestTime, num);
		}
		if (this.GetRunBasedFloat(RUNBASEDVALUETYPE.PackagedFoodEaten) == 0f)
		{
			this.ThrowAchievement(ACHIEVEMENTTYPE.NaturalistBadge);
		}
		if (this.GetRunBasedInt(RUNBASEDVALUETYPE.TimesPassedOut) == 0)
		{
			this.ThrowAchievement(ACHIEVEMENTTYPE.SurvivalistBadge);
		}
		if (this.GetRunBasedInt(RUNBASEDVALUETYPE.PermanentItemsPlaced) == 0)
		{
			this.ThrowAchievement(ACHIEVEMENTTYPE.LeaveNoTraceBadge);
		}
		if (this.HasBingBong(Character.localCharacter))
		{
			GameUtils.instance.ThrowBingBongAchievement();
		}
		if (this.gourmandRequirementsEaten.Count >= 4)
		{
			this.ThrowAchievement(ACHIEVEMENTTYPE.GourmandBadge);
		}
	}

	// Token: 0x060002C6 RID: 710 RVA: 0x000123E8 File Offset: 0x000105E8
	private bool HasBingBong(Character character)
	{
		if (character.data.currentItem && character.data.currentItem.itemTags.HasFlag(Item.ItemTags.BingBong))
		{
			return true;
		}
		foreach (ItemSlot itemSlot in character.player.itemSlots)
		{
			if (itemSlot != null && itemSlot.prefab != null && itemSlot.prefab.itemTags.HasFlag(Item.ItemTags.BingBong))
			{
				return true;
			}
		}
		BackpackData backpackData;
		if (!character.player.backpackSlot.IsEmpty() && character.player.backpackSlot.data.TryGetDataEntry<BackpackData>(DataEntryKey.BackpackData, out backpackData))
		{
			foreach (ItemSlot itemSlot2 in backpackData.itemSlots)
			{
				if (itemSlot2 != null && itemSlot2.prefab != null && itemSlot2.prefab.itemTags.HasFlag(Item.ItemTags.BingBong))
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x060002C7 RID: 711 RVA: 0x000124F6 File Offset: 0x000106F6
	private void TestCharacterPassedOut(Character character)
	{
		if (character.IsLocal)
		{
			this.AddToRunBasedInt(RUNBASEDVALUETYPE.TimesPassedOut, 1);
		}
	}

	// Token: 0x060002C8 RID: 712 RVA: 0x00012508 File Offset: 0x00010708
	private void TestSomeoneWonRun()
	{
		if (Character.localCharacter.refs.stats.lost)
		{
			Debug.Log("YOU TRIED");
			this.ThrowAchievement(ACHIEVEMENTTYPE.TriedYourBestBadge);
		}
		int i;
		if (this.GetSteamStatInt(STEAMSTATTYPE.MaxAscent, out i) && Ascents.currentAscent >= i)
		{
			while (i <= Ascents.currentAscent)
			{
				Debug.Log("Completed Ascent: " + i.ToString());
				this.completedAscentsThisRun.Add(i);
				i++;
			}
			this.SetSteamStat(STEAMSTATTYPE.MaxAscent, Ascents.currentAscent + 1);
		}
	}

	// Token: 0x060002C9 RID: 713 RVA: 0x0001258F File Offset: 0x0001078F
	private void AddToRunBasedFruitsEaten(ushort value)
	{
		if (!this.runBasedFruitsEaten.Contains(value))
		{
			this.runBasedFruitsEaten.Add(value);
			if (this.runBasedFruitsEaten.Count >= 5)
			{
				this.ThrowAchievement(ACHIEVEMENTTYPE.ForagingBadge);
			}
		}
	}

	// Token: 0x060002CA RID: 714 RVA: 0x000125C1 File Offset: 0x000107C1
	private void AddToNonToxicMushroomsEaten(ushort value)
	{
		if (!this.nonToxicMushroomsEaten.Contains(value))
		{
			this.nonToxicMushroomsEaten.Add(value);
			if (this.nonToxicMushroomsEaten.Count >= 4)
			{
				this.ThrowAchievement(ACHIEVEMENTTYPE.MycologyBadge);
			}
		}
	}

	// Token: 0x060002CB RID: 715 RVA: 0x000125F3 File Offset: 0x000107F3
	private void AddToGourmandRequirementsEaten(ushort value)
	{
		if (!this.gourmandRequirementsEaten.Contains(value))
		{
			this.gourmandRequirementsEaten.Add(value);
		}
	}

	// Token: 0x060002CC RID: 716 RVA: 0x00012610 File Offset: 0x00010810
	internal void RecordMaxHeight(int meters)
	{
		if (meters < 25)
		{
			return;
		}
		int runBasedInt = this.GetRunBasedInt(RUNBASEDVALUETYPE.MaxHeightReached);
		if (meters >= runBasedInt + 5)
		{
			this.IncrementSteamStat(STEAMSTATTYPE.HeightClimbed, meters - runBasedInt);
			this.SetRunBasedInt(RUNBASEDVALUETYPE.MaxHeightReached, meters);
		}
	}

	// Token: 0x0400035B RID: 859
	public List<AchievementData> allAchievements;

	// Token: 0x0400035C RID: 860
	public Dictionary<RUNBASEDVALUETYPE, object> runBasedValues = new Dictionary<RUNBASEDVALUETYPE, object>();

	// Token: 0x0400035D RID: 861
	internal List<int> completedAscentsThisRun = new List<int>();

	// Token: 0x0400035E RID: 862
	[SerializeField]
	private ACHIEVEMENTTYPE debugAchievement;

	// Token: 0x04000360 RID: 864
	private List<AchievementManager.RunBasedAchievementData> runBasedAchievements = new List<AchievementManager.RunBasedAchievementData>();

	// Token: 0x04000361 RID: 865
	internal List<ACHIEVEMENTTYPE> achievementsEarnedThisRun = new List<ACHIEVEMENTTYPE>();

	// Token: 0x04000362 RID: 866
	public bool useDebugAscent;

	// Token: 0x04000363 RID: 867
	public int debugAscent;

	// Token: 0x04000364 RID: 868
	private List<ACHIEVEMENTTYPE> steamAchievementsPreviouslyUnlocked = new List<ACHIEVEMENTTYPE>();

	// Token: 0x04000365 RID: 869
	public const int TOTAL_GUIDEBOOK_PAGES = 8;

	// Token: 0x04000366 RID: 870
	public const string STEAMSTAT_GUIDEBOOK_PREFIX = "ReadGuidebookPage_";

	// Token: 0x04000367 RID: 871
	private const float ONE_HOUR_IN_SECONDS = 3600f;

	// Token: 0x04000368 RID: 872
	private const int FRUITSNEEDEDFORACHIEVEMENT = 5;

	// Token: 0x04000369 RID: 873
	private const int MUSHROOMSNEEDEDFORACHIEVEMENT = 4;

	// Token: 0x0400036A RID: 874
	private List<ushort> runBasedFruitsEaten = new List<ushort>();

	// Token: 0x0400036B RID: 875
	private List<ushort> nonToxicMushroomsEaten = new List<ushort>();

	// Token: 0x0400036C RID: 876
	private List<ushort> gourmandRequirementsEaten = new List<ushort>();

	// Token: 0x020002F4 RID: 756
	private class RunBasedAchievementData
	{
		// Token: 0x06001274 RID: 4724 RVA: 0x0005A3EF File Offset: 0x000585EF
		public RunBasedAchievementData(ACHIEVEMENTTYPE achievementType, RUNBASEDVALUETYPE valueType, int requiredValue)
		{
			this.achievementType = achievementType;
			this.valueType = valueType;
			this.requiredValue = requiredValue;
		}

		// Token: 0x06001275 RID: 4725 RVA: 0x0005A40C File Offset: 0x0005860C
		public bool IsAchieved()
		{
			try
			{
				return Singleton<AchievementManager>.Instance.GetRunBasedFloat(this.valueType) >= (float)this.requiredValue;
			}
			catch (Exception ex)
			{
				Debug.LogError(ex);
			}
			return false;
		}

		// Token: 0x040010DC RID: 4316
		public ACHIEVEMENTTYPE achievementType;

		// Token: 0x040010DD RID: 4317
		public RUNBASEDVALUETYPE valueType;

		// Token: 0x040010DE RID: 4318
		public int requiredValue;
	}
}
