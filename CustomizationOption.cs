using System;
using UnityEngine;
using Zorro.Core;

// Token: 0x0200006F RID: 111
[CreateAssetMenu(fileName = "CustomizationOption", menuName = "Scriptable Objects/CustomizationOption")]
public class CustomizationOption : ScriptableObject
{
	// Token: 0x17000042 RID: 66
	// (get) Token: 0x0600040C RID: 1036 RVA: 0x00017A23 File Offset: 0x00015C23
	private bool IsSkin
	{
		get
		{
			return this.type == Customization.Type.Skin;
		}
	}

	// Token: 0x17000043 RID: 67
	// (get) Token: 0x0600040D RID: 1037 RVA: 0x00017A2E File Offset: 0x00015C2E
	private bool IsFit
	{
		get
		{
			return this.type == Customization.Type.Fit;
		}
	}

	// Token: 0x17000044 RID: 68
	// (get) Token: 0x0600040E RID: 1038 RVA: 0x00017A3A File Offset: 0x00015C3A
	public Material fitPantsMaterial
	{
		get
		{
			if (this.fitMaterialOverridePants != null)
			{
				return this.fitMaterialOverridePants;
			}
			return this.fitMaterial;
		}
	}

	// Token: 0x17000045 RID: 69
	// (get) Token: 0x0600040F RID: 1039 RVA: 0x00017A57 File Offset: 0x00015C57
	public Material fitHatMaterial
	{
		get
		{
			if (this.fitMaterialOverrideHat != null)
			{
				return this.fitMaterialOverrideHat;
			}
			return this.fitMaterial;
		}
	}

	// Token: 0x17000046 RID: 70
	// (get) Token: 0x06000410 RID: 1040 RVA: 0x00017A74 File Offset: 0x00015C74
	public bool IsLocked
	{
		get
		{
			return this.requiredAchievement != ACHIEVEMENTTYPE.NONE && !Singleton<AchievementManager>.Instance.IsAchievementUnlocked(this.requiredAchievement);
		}
	}

	// Token: 0x0400045D RID: 1117
	public Customization.Type type;

	// Token: 0x0400045E RID: 1118
	public Texture texture;

	// Token: 0x0400045F RID: 1119
	public ACHIEVEMENTTYPE requiredAchievement;

	// Token: 0x04000460 RID: 1120
	public bool testLocked;

	// Token: 0x04000461 RID: 1121
	[ColorUsage(true, false)]
	public Color color;

	// Token: 0x04000462 RID: 1122
	public Mesh fitMesh;

	// Token: 0x04000463 RID: 1123
	public Material fitMaterial;

	// Token: 0x04000464 RID: 1124
	public Material fitMaterialShoes;

	// Token: 0x04000465 RID: 1125
	public Material fitMaterialOverridePants;

	// Token: 0x04000466 RID: 1126
	public Material fitMaterialOverrideHat;

	// Token: 0x04000467 RID: 1127
	public bool isSkirt;
}
