using System;
using UnityEngine;

// Token: 0x02000062 RID: 98
public static class GameBooter
{
	// Token: 0x060003D5 RID: 981 RVA: 0x00016B91 File Offset: 0x00014D91
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
	public static void Initialize()
	{
		GameBooter.AutoBoot();
	}

	// Token: 0x060003D6 RID: 982 RVA: 0x00016B98 File Offset: 0x00014D98
	public static void AutoBoot()
	{
		GameObject gameObject = new GameObject("Game");
		gameObject.AddComponent<GameHandler>().Initialize();
		gameObject.AddComponent<UIInputHandler>().Initialize();
	}
}
