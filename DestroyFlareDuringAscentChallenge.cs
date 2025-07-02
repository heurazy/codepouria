using System;
using System.Collections;
using Photon.Pun;

// Token: 0x02000073 RID: 115
public class DestroyFlareDuringAscentChallenge : MonoBehaviourPun
{
	// Token: 0x0600041F RID: 1055 RVA: 0x00017D34 File Offset: 0x00015F34
	private IEnumerator Start()
	{
		while (!PhotonNetwork.InRoom)
		{
			yield return null;
		}
		if (!Ascents.shouldSpawnFlare)
		{
			PhotonNetwork.Destroy(base.gameObject);
		}
		yield break;
	}
}
