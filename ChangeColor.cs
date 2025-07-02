using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000211 RID: 529
[RequireComponent(typeof(Renderer))]
[RequireComponent(typeof(PhotonView))]
public class ChangeColor : MonoBehaviour
{
	// Token: 0x06000DAB RID: 3499 RVA: 0x00044E40 File Offset: 0x00043040
	private void Start()
	{
		this.photonView = base.GetComponent<PhotonView>();
		if (this.photonView.IsMine)
		{
			Color color = Random.ColorHSV();
			this.photonView.RPC("ChangeColour", RpcTarget.AllBuffered, new object[]
			{
				new Vector3(color.r, color.g, color.b)
			});
		}
	}

	// Token: 0x06000DAC RID: 3500 RVA: 0x00044EA2 File Offset: 0x000430A2
	[PunRPC]
	private void ChangeColour(Vector3 randomColor)
	{
		base.GetComponent<Renderer>().material.SetColor("_Color", new Color(randomColor.x, randomColor.y, randomColor.z));
	}

	// Token: 0x04000CC2 RID: 3266
	private PhotonView photonView;
}
