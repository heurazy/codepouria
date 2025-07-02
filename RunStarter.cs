using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;
using Zorro.Core;

// Token: 0x02000122 RID: 290
public class RunStarter : MonoBehaviour
{
	// Token: 0x06000884 RID: 2180 RVA: 0x0002D64A File Offset: 0x0002B84A
	private IEnumerator Start()
	{
		while (!PhotonNetwork.InRoom || !Character.localCharacter || LoadingScreenHandler.loading)
		{
			yield return null;
		}
		Debug.Log("RUN STARTED");
		this.StartRun();
		yield break;
	}

	// Token: 0x06000885 RID: 2181 RVA: 0x0002D659 File Offset: 0x0002B859
	private void StartRun()
	{
		Singleton<RunManager>.Instance.StartRun();
	}
}
