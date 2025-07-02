using System;
using UnityEngine;
using Zorro.Core;
using Zorro.Core.CLI;

// Token: 0x0200003F RID: 63
public static class Ascents
{
	// Token: 0x1700002B RID: 43
	// (get) Token: 0x06000302 RID: 770 RVA: 0x00013513 File Offset: 0x00011713
	// (set) Token: 0x06000303 RID: 771 RVA: 0x0001351A File Offset: 0x0001171A
	public static int currentAscent
	{
		get
		{
			return Ascents._currentAscent;
		}
		set
		{
			Ascents._currentAscent = value;
			Debug.Log("Ascent set to " + value.ToString());
		}
	}

	// Token: 0x1700002C RID: 44
	// (get) Token: 0x06000304 RID: 772 RVA: 0x00013538 File Offset: 0x00011738
	public static float fallDamageMultiplier
	{
		get
		{
			if (Ascents.currentAscent < 1)
			{
				return 1f;
			}
			return 2f;
		}
	}

	// Token: 0x1700002D RID: 45
	// (get) Token: 0x06000305 RID: 773 RVA: 0x0001354D File Offset: 0x0001174D
	public static float hungerRateMultiplier
	{
		get
		{
			if (Ascents.currentAscent == -1)
			{
				return 0.7f;
			}
			if (Ascents.currentAscent >= 2)
			{
				return 1.6f;
			}
			return 1f;
		}
	}

	// Token: 0x1700002E RID: 46
	// (get) Token: 0x06000306 RID: 774 RVA: 0x00013570 File Offset: 0x00011770
	public static int itemWeightModifier
	{
		get
		{
			if (Ascents.currentAscent < 3)
			{
				return 0;
			}
			return 1;
		}
	}

	// Token: 0x1700002F RID: 47
	// (get) Token: 0x06000307 RID: 775 RVA: 0x0001357D File Offset: 0x0001177D
	public static bool shouldSpawnFlare
	{
		get
		{
			return Ascents.currentAscent < 4;
		}
	}

	// Token: 0x17000030 RID: 48
	// (get) Token: 0x06000308 RID: 776 RVA: 0x00013587 File Offset: 0x00011787
	public static bool isNightCold
	{
		get
		{
			return Ascents.currentAscent >= 5;
		}
	}

	// Token: 0x17000031 RID: 49
	// (get) Token: 0x06000309 RID: 777 RVA: 0x00013594 File Offset: 0x00011794
	public static float nightColdRate
	{
		get
		{
			return 0.005f;
		}
	}

	// Token: 0x17000032 RID: 50
	// (get) Token: 0x0600030A RID: 778 RVA: 0x0001359B File Offset: 0x0001179B
	public static bool canReviveDead
	{
		get
		{
			return Ascents.currentAscent < 7;
		}
	}

	// Token: 0x17000033 RID: 51
	// (get) Token: 0x0600030B RID: 779 RVA: 0x000135A5 File Offset: 0x000117A5
	public static float climbStaminaMultiplier
	{
		get
		{
			if (Ascents.currentAscent >= 6)
			{
				return 1.4f;
			}
			if (Ascents.currentAscent == -1)
			{
				return 0.7f;
			}
			return 1f;
		}
	}

	// Token: 0x0600030C RID: 780 RVA: 0x000135C8 File Offset: 0x000117C8
	[ConsoleCommand]
	public static void UnlockAll()
	{
		Singleton<AchievementManager>.Instance.SetSteamStat(STEAMSTATTYPE.MaxAscent, 7);
	}

	// Token: 0x0600030D RID: 781 RVA: 0x000135D8 File Offset: 0x000117D8
	[ConsoleCommand]
	public static void UnlockOne()
	{
		int num;
		if (Singleton<AchievementManager>.Instance.GetSteamStatInt(STEAMSTATTYPE.MaxAscent, out num))
		{
			Singleton<AchievementManager>.Instance.SetSteamStat(STEAMSTATTYPE.MaxAscent, num + 1);
		}
	}

	// Token: 0x0600030E RID: 782 RVA: 0x00013604 File Offset: 0x00011804
	[ConsoleCommand]
	public static void LockAll()
	{
		Singleton<AchievementManager>.Instance.SetSteamStat(STEAMSTATTYPE.MaxAscent, 0);
	}

	// Token: 0x040003B1 RID: 945
	internal static int _currentAscent;
}
