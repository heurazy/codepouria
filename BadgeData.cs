using System;
using UnityEngine;
using Zorro.Core;

// Token: 0x0200014B RID: 331
[CreateAssetMenu(fileName = "BadgeData", menuName = "Scriptable Objects/BadgeData")]
public class BadgeData : ScriptableObject
{
	// Token: 0x1700007B RID: 123
	// (get) Token: 0x0600097C RID: 2428 RVA: 0x0002FDAE File Offset: 0x0002DFAE
	public bool IsLocked
	{
		get
		{
			return !Singleton<AchievementManager>.Instance.IsAchievementUnlocked(this.linkedAchievement);
		}
	}

	// Token: 0x04000860 RID: 2144
	public Texture icon;

	// Token: 0x04000861 RID: 2145
	public string displayName;

	// Token: 0x04000862 RID: 2146
	public string description;

	// Token: 0x04000863 RID: 2147
	public ACHIEVEMENTTYPE linkedAchievement;

	// Token: 0x04000864 RID: 2148
	public bool testLocked;

	// Token: 0x04000865 RID: 2149
	public int visualID;
}
