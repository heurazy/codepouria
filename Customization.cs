using System;
using System.Collections.Generic;
using UnityEngine;
using Zorro.Core;

// Token: 0x0200006E RID: 110
public class Customization : Singleton<Customization>
{
	// Token: 0x06000408 RID: 1032 RVA: 0x000178B4 File Offset: 0x00015AB4
	public bool TryGetUnlockedCosmetic(BadgeData badge, out CustomizationOption cosmetic)
	{
		cosmetic = null;
		foreach (object obj in Enum.GetValues(typeof(Customization.Type)))
		{
			Customization.Type type = (Customization.Type)obj;
			foreach (CustomizationOption customizationOption in this.GetList(type))
			{
				if (!(customizationOption == null) && customizationOption.requiredAchievement != ACHIEVEMENTTYPE.NONE && customizationOption.requiredAchievement == badge.linkedAchievement)
				{
					cosmetic = customizationOption;
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06000409 RID: 1033 RVA: 0x00017960 File Offset: 0x00015B60
	public CustomizationOption[] GetList(Customization.Type type)
	{
		if (type <= Customization.Type.Eyes)
		{
			if (type == Customization.Type.Skin)
			{
				return this.skins;
			}
			if (type == Customization.Type.Accessory)
			{
				return this.accessories;
			}
			if (type == Customization.Type.Eyes)
			{
				return this.eyes;
			}
		}
		else
		{
			if (type == Customization.Type.Mouth)
			{
				return this.mouths;
			}
			if (type == Customization.Type.Fit)
			{
				return this.fits;
			}
			if (type == Customization.Type.Hat)
			{
				return this.hats;
			}
		}
		return this.skins;
	}

	// Token: 0x0600040A RID: 1034 RVA: 0x000179C4 File Offset: 0x00015BC4
	public int GetRandomUnlockedIndex(Customization.Type type)
	{
		CustomizationOption[] list = this.GetList(type);
		List<int> list2 = new List<int>();
		for (int i = 0; i < list.Length; i++)
		{
			if (!list[i].IsLocked)
			{
				list2.Add(i);
			}
		}
		if (list2.Count <= 0)
		{
			return 0;
		}
		return list2[Random.Range(0, list2.Count)];
	}

	// Token: 0x04000457 RID: 1111
	public CustomizationOption[] skins;

	// Token: 0x04000458 RID: 1112
	public CustomizationOption[] accessories;

	// Token: 0x04000459 RID: 1113
	public CustomizationOption[] eyes;

	// Token: 0x0400045A RID: 1114
	public CustomizationOption[] mouths;

	// Token: 0x0400045B RID: 1115
	public CustomizationOption[] fits;

	// Token: 0x0400045C RID: 1116
	public CustomizationOption[] hats;

	// Token: 0x02000302 RID: 770
	public enum Type
	{
		// Token: 0x040010FF RID: 4351
		Skin,
		// Token: 0x04001100 RID: 4352
		Accessory = 10,
		// Token: 0x04001101 RID: 4353
		Eyes = 20,
		// Token: 0x04001102 RID: 4354
		Mouth = 30,
		// Token: 0x04001103 RID: 4355
		Fit = 40,
		// Token: 0x04001104 RID: 4356
		Hat = 50
	}
}
