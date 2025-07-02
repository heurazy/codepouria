using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200019B RID: 411
public class Bugfixer : MonoBehaviour
{
	// Token: 0x06000B52 RID: 2898 RVA: 0x00037EC0 File Offset: 0x000360C0
	private void Start()
	{
	}

	// Token: 0x06000B53 RID: 2899 RVA: 0x00037EC4 File Offset: 0x000360C4
	private void Update()
	{
		if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Period))
		{
			Character target = this.GetTarget();
			if (target != null)
			{
				PhotonNetwork.Instantiate("BugfixOnYou", Vector3.zero, Quaternion.identity, 0, null).GetComponent<PhotonView>().RPC("AttachBug", RpcTarget.All, new object[] { target.photonView.ViewID });
			}
		}
	}

	// Token: 0x06000B54 RID: 2900 RVA: 0x00037F38 File Offset: 0x00036138
	private Character GetTarget()
	{
		if (this.useLocalCharacter)
		{
			return Character.localCharacter;
		}
		Character character = null;
		float num = float.MaxValue;
		foreach (Character character2 in Character.AllCharacters)
		{
			float num2 = Vector3.Angle(MainCamera.instance.transform.forward, character2.Center - MainCamera.instance.transform.position);
			if (num2 < num)
			{
				num = num2;
				character = character2;
			}
		}
		return character;
	}

	// Token: 0x04000A65 RID: 2661
	public bool useLocalCharacter;
}
