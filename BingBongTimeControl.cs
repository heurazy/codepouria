using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000195 RID: 405
public class BingBongTimeControl : MonoBehaviour
{
	// Token: 0x06000B16 RID: 2838 RVA: 0x00036F7B File Offset: 0x0003517B
	private void Start()
	{
		this.view = base.GetComponent<PhotonView>();
	}

	// Token: 0x06000B17 RID: 2839 RVA: 0x00036F8C File Offset: 0x0003518C
	private void Update()
	{
		this.syncCounter += Time.unscaledDeltaTime;
		if (Input.GetKeyDown(KeyCode.R))
		{
			this.currentTimeScale = 1f;
		}
		if (Input.GetKeyDown(KeyCode.F))
		{
			this.currentTimeScale = 0f;
		}
		if (Input.GetKeyDown(KeyCode.Mouse0))
		{
			this.currentTimeScale += Mathf.Clamp(0.1f, this.currentTimeScale * 0.3f, 0.5f);
		}
		if (Input.GetKeyDown(KeyCode.Mouse1))
		{
			this.currentTimeScale -= Mathf.Clamp(0.1f, this.currentTimeScale * 0.3f, 0.5f);
		}
		this.currentTimeScale = Mathf.Clamp(this.currentTimeScale, 0.02f, 10f);
		if (Time.timeScale != this.currentTimeScale)
		{
			this.bingBongPowers.SetTip(string.Format("Time Scale: {0:P0}", this.currentTimeScale), 1);
			if (this.syncCounter > 0.1f)
			{
				this.view.RPC("RPCA_SyncTime", RpcTarget.All, new object[] { this.currentTimeScale });
			}
		}
	}

	// Token: 0x06000B18 RID: 2840 RVA: 0x000370B6 File Offset: 0x000352B6
	[PunRPC]
	public void RPCA_SyncTime(float newTime)
	{
		Time.timeScale = newTime;
	}

	// Token: 0x06000B19 RID: 2841 RVA: 0x000370BE File Offset: 0x000352BE
	private void OnDestroy()
	{
		Time.timeScale = 1f;
	}

	// Token: 0x06000B1A RID: 2842 RVA: 0x000370CA File Offset: 0x000352CA
	private void OnEnable()
	{
		this.bingBongPowers = base.GetComponent<BingBongPowers>();
		this.bingBongPowers.SetTexts("TIME", this.descr);
	}

	// Token: 0x04000A20 RID: 2592
	private PhotonView view;

	// Token: 0x04000A21 RID: 2593
	public float currentTimeScale = 1f;

	// Token: 0x04000A22 RID: 2594
	private float syncCounter;

	// Token: 0x04000A23 RID: 2595
	private BingBongPowers bingBongPowers;

	// Token: 0x04000A24 RID: 2596
	private string descr = "Reset time: [R]\n\nFreeze: [F]\n\nFaster: [LMB]\n\nSlower: [RMB]";
}
