using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x020000E4 RID: 228
public class RespawnRandomScout : MonoBehaviour
{
	// Token: 0x060006F4 RID: 1780 RVA: 0x000247AC File Offset: 0x000229AC
	private void Start()
	{
		if (PhotonNetwork.IsMasterClient)
		{
			List<Character> list = new List<Character>();
			foreach (Character character in Character.AllCharacters)
			{
				if (character.data.dead || character.data.fullyPassedOut)
				{
					list.Add(character);
				}
			}
			list.RandomSelection((Character c) => 1).photonView.RPC("RPCA_ReviveAtPosition", RpcTarget.All, new object[]
			{
				base.transform.position,
				false
			});
		}
		Object.Destroy(base.gameObject);
	}
}
