using System;
using System.Collections;
using Photon.Pun;
using Unity.Collections;
using UnityEngine;
using Zorro.Core;

// Token: 0x02000121 RID: 289
public class RunManager : Singleton<RunManager>
{
	// Token: 0x06000880 RID: 2176 RVA: 0x0002D604 File Offset: 0x0002B804
	private IEnumerator Start()
	{
		this.runStarted = false;
		this.timeSinceRunStarted = 0f;
		while (!PhotonNetwork.InRoom || !Character.localCharacter || LoadingScreenHandler.loading)
		{
			yield return null;
		}
		Debug.Log("RUN STARTED");
		this.StartRun();
		yield break;
	}

	// Token: 0x06000881 RID: 2177 RVA: 0x0002D613 File Offset: 0x0002B813
	private void Update()
	{
		if (this.runStarted)
		{
			this.timeSinceRunStarted += Time.deltaTime;
		}
	}

	// Token: 0x06000882 RID: 2178 RVA: 0x0002D62F File Offset: 0x0002B82F
	public void StartRun()
	{
		this.runStarted = true;
		Singleton<AchievementManager>.Instance.InitRunBasedValues();
	}

	// Token: 0x040007FC RID: 2044
	[ReadOnly]
	public float timeSinceRunStarted;

	// Token: 0x040007FD RID: 2045
	private bool runStarted;

	// Token: 0x040007FE RID: 2046
	private float timerUpdateTick;
}
