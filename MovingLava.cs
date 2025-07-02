using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000202 RID: 514
public class MovingLava : MonoBehaviour
{
	// Token: 0x06000D51 RID: 3409 RVA: 0x00043246 File Offset: 0x00041446
	private void Start()
	{
		this.view = base.GetComponent<PhotonView>();
	}

	// Token: 0x06000D52 RID: 3410 RVA: 0x00043254 File Offset: 0x00041454
	private void Update()
	{
		if (base.transform.position.y > 1150f)
		{
			return;
		}
		if (!this.timeToMove)
		{
			if (this.PlayersHaveMovedOn())
			{
				this.view.RPC("RPCA_StartLavaRise", RpcTarget.All, Array.Empty<object>());
			}
			return;
		}
		base.transform.position += Vector3.up * this.speed * Time.deltaTime;
		this.sinceSync += Time.deltaTime;
		if (this.sinceSync > 1f)
		{
			this.sinceSync = 0f;
			this.view.RPC("RPCA_SyncLavaHeight", RpcTarget.All, new object[] { base.transform.position.y });
		}
	}

	// Token: 0x06000D53 RID: 3411 RVA: 0x00043329 File Offset: 0x00041529
	[PunRPC]
	public void RPCA_SyncLavaHeight(float height)
	{
		base.transform.position = new Vector3(base.transform.position.x, height, base.transform.position.z);
	}

	// Token: 0x06000D54 RID: 3412 RVA: 0x0004335C File Offset: 0x0004155C
	[PunRPC]
	public void RPCA_StartLavaRise()
	{
		this.rockAnim.Play("RockDoor", 0, 0f);
		this.timeToMove = true;
		GamefeelHandler.instance.AddPerlinShake(3f, 2f, 10f);
		GamefeelHandler.instance.AddPerlinShake(15f, 0.3f, 15f);
	}

	// Token: 0x06000D55 RID: 3413 RVA: 0x000433B8 File Offset: 0x000415B8
	private bool PlayersHaveMovedOn()
	{
		if (Character.AllCharacters.Count == 0)
		{
			return false;
		}
		float num = 879f;
		for (int i = 0; i < Character.AllCharacters.Count; i++)
		{
			if (Character.AllCharacters[i].Center.y > num)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x04000C78 RID: 3192
	public float speed = 0.25f;

	// Token: 0x04000C79 RID: 3193
	public Animator rockAnim;

	// Token: 0x04000C7A RID: 3194
	private PhotonView view;

	// Token: 0x04000C7B RID: 3195
	private bool timeToMove;

	// Token: 0x04000C7C RID: 3196
	private float sinceSync;
}
