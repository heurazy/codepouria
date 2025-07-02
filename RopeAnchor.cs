using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000114 RID: 276
public class RopeAnchor : MonoBehaviour
{
	// Token: 0x0600081A RID: 2074 RVA: 0x0002B19D File Offset: 0x0002939D
	private void Awake()
	{
		this.photonView = base.GetComponent<PhotonView>();
	}

	// Token: 0x1700006B RID: 107
	// (get) Token: 0x0600081B RID: 2075 RVA: 0x0002B1AB File Offset: 0x000293AB
	// (set) Token: 0x0600081C RID: 2076 RVA: 0x0002B1B3 File Offset: 0x000293B3
	public bool Ghost
	{
		get
		{
			return this.isGhost;
		}
		set
		{
			this.isGhost = value;
			this.HideAll();
			if (this.isGhost)
			{
				this.ghostPart.SetActive(true);
				return;
			}
			this.normalPart.SetActive(true);
		}
	}

	// Token: 0x0600081D RID: 2077 RVA: 0x0002B1E3 File Offset: 0x000293E3
	private void HideAll()
	{
		this.ghostPart.SetActive(false);
		this.normalPart.SetActive(false);
	}

	// Token: 0x04000793 RID: 1939
	public GameObject ghostPart;

	// Token: 0x04000794 RID: 1940
	public GameObject normalPart;

	// Token: 0x04000795 RID: 1941
	public Transform anchorPoint;

	// Token: 0x04000796 RID: 1942
	private bool isGhost;

	// Token: 0x04000797 RID: 1943
	public PhotonView photonView;
}
