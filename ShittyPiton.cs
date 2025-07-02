using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000272 RID: 626
public class ShittyPiton : MonoBehaviour
{
	// Token: 0x06000F2F RID: 3887 RVA: 0x0004CB44 File Offset: 0x0004AD44
	private void Start()
	{
		this.view = base.GetComponent<PhotonView>();
		this.handle = base.GetComponent<ClimbHandle>();
		ClimbHandle climbHandle = this.handle;
		climbHandle.onHangStart = (Action<Character>)Delegate.Combine(climbHandle.onHangStart, new Action<Character>(this.OnHang));
		ClimbHandle climbHandle2 = this.handle;
		climbHandle2.onHangStop = (Action)Delegate.Combine(climbHandle2.onHangStop, new Action(this.OnHangStop));
		this.totalSecondsOfHang = Random.Range(1f, 5f);
	}

	// Token: 0x06000F30 RID: 3888 RVA: 0x0004CBCC File Offset: 0x0004ADCC
	private void OnHangStop()
	{
		this.isHung = false;
	}

	// Token: 0x06000F31 RID: 3889 RVA: 0x0004CBD5 File Offset: 0x0004ADD5
	private void OnHang(Character character)
	{
		this.isHung = true;
	}

	// Token: 0x06000F32 RID: 3890 RVA: 0x0004CBE0 File Offset: 0x0004ADE0
	private void Update()
	{
		if (this.isBreaking)
		{
			if (this.isHung)
			{
				this.sinceCrack += Time.deltaTime;
			}
			if (this.sinceCrack > 1.5f)
			{
				this.Crack();
				this.sinceCrack = 0f;
			}
			this.crack.transform.localScale = Vector3.Lerp(this.crack.transform.localScale, Vector3.one * this.crackScale, Time.deltaTime * 15f);
			return;
		}
		if (!this.view.IsMine)
		{
			return;
		}
		if (this.isHung)
		{
			this.totalSecondsOfHang -= Time.deltaTime;
			if (this.totalSecondsOfHang < 0f)
			{
				this.view.RPC("RPCA_StartBreaking", RpcTarget.All, Array.Empty<object>());
			}
		}
	}

	// Token: 0x06000F33 RID: 3891 RVA: 0x0004CCBC File Offset: 0x0004AEBC
	private void Crack()
	{
		this.crackScale += 0.75f;
		this.cracksToBreak--;
		GamefeelHandler.instance.AddPerlinShakeProximity(base.transform.position, 2f + this.crackScale, 0.2f, 15f, 10f);
		if (this.cracksToBreak <= 0 && this.view.IsMine)
		{
			this.view.RPC("RPCA_Break", RpcTarget.All, Array.Empty<object>());
		}
	}

	// Token: 0x06000F34 RID: 3892 RVA: 0x0004CD48 File Offset: 0x0004AF48
	[PunRPC]
	private void RPCA_Break()
	{
		this.vfx.transform.SetParent(null);
		this.vfx.SetActive(true);
		this.crack.gameObject.AddComponent<RemoveAfterSeconds>().Config(true, 2f);
		this.crack.transform.SetParent(null);
		this.handle.Break();
	}

	// Token: 0x06000F35 RID: 3893 RVA: 0x0004CDA9 File Offset: 0x0004AFA9
	[PunRPC]
	private void RPCA_StartBreaking()
	{
		this.isBreaking = true;
		this.crack.SetActive(true);
		this.crack.transform.localScale *= 0f;
	}

	// Token: 0x04000E1A RID: 3610
	private ClimbHandle handle;

	// Token: 0x04000E1B RID: 3611
	private PhotonView view;

	// Token: 0x04000E1C RID: 3612
	private float totalSecondsOfHang;

	// Token: 0x04000E1D RID: 3613
	public GameObject crack;

	// Token: 0x04000E1E RID: 3614
	public GameObject vfx;

	// Token: 0x04000E1F RID: 3615
	private float crackScale;

	// Token: 0x04000E20 RID: 3616
	private int cracksToBreak = 4;

	// Token: 0x04000E21 RID: 3617
	private float sinceCrack = 10f;

	// Token: 0x04000E22 RID: 3618
	private bool isHung;

	// Token: 0x04000E23 RID: 3619
	private bool isBreaking;
}
