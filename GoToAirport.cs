using System;
using System.Collections;
using UnityEngine;
using Zorro.Core;

// Token: 0x02000156 RID: 342
public class GoToAirport : MonoBehaviour
{
	// Token: 0x060009C9 RID: 2505 RVA: 0x00030C54 File Offset: 0x0002EE54
	public void GoFromMainMenu()
	{
		RetrievableResourceSingleton<LoadingScreenHandler>.Instance.Load(LoadingScreen.LoadingScreenType.Plane, null, new IEnumerator[] { RetrievableResourceSingleton<LoadingScreenHandler>.Instance.LoadSceneProcess("Airport", false, true, 3f) });
	}
}
