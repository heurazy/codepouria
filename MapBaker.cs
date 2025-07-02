using System;
using UnityEngine;
using Zorro.Core;
using Zorro.Core.Editor;

// Token: 0x020000EF RID: 239
[CreateAssetMenu(menuName = "Peak/MapBaker")]
public class MapBaker : SingletonAsset<MapBaker>
{
	// Token: 0x06000729 RID: 1833 RVA: 0x00025DAF File Offset: 0x00023FAF
	public void GenerateMaps()
	{
	}

	// Token: 0x0600072A RID: 1834 RVA: 0x00025DB1 File Offset: 0x00023FB1
	private void GenerateMap(int i)
	{
	}

	// Token: 0x0600072B RID: 1835 RVA: 0x00025DB3 File Offset: 0x00023FB3
	public string GetLevel(int levelIndex)
	{
		if (this.AllLevels.Length == 0)
		{
			Debug.LogError("No levels found, using WilIsland...");
			return "";
		}
		levelIndex %= this.AllLevels.Length;
		return PathUtil.WithoutExtensions(PathUtil.GetFileName(this.AllLevels[levelIndex].ScenePath));
	}

	// Token: 0x040006C0 RID: 1728
	public int DesiredAmountOfLevels = 2;

	// Token: 0x040006C1 RID: 1729
	public SceneReference[] AllLevels;
}
