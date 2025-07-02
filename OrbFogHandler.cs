using System;
using System.Collections;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Zorro.Core;

// Token: 0x02000209 RID: 521
public class OrbFogHandler : Singleton<OrbFogHandler>, IInRoomCallbacks
{
	// Token: 0x06000D6E RID: 3438 RVA: 0x00043DD5 File Offset: 0x00041FD5
	protected override void Awake()
	{
		base.Awake();
		this.photonView = base.GetComponent<PhotonView>();
	}

	// Token: 0x06000D6F RID: 3439 RVA: 0x00043DE9 File Offset: 0x00041FE9
	private void Start()
	{
		this.sphere = base.GetComponentInChildren<FogSphere>();
		this.origins = base.transform.root.GetComponentsInChildren<FogSphereOrigin>();
		this.InitNewSphere(this.origins[this.currentID]);
	}

	// Token: 0x06000D70 RID: 3440 RVA: 0x00043E20 File Offset: 0x00042020
	private void OnEnable()
	{
		PhotonNetwork.AddCallbackTarget(this);
	}

	// Token: 0x06000D71 RID: 3441 RVA: 0x00043E28 File Offset: 0x00042028
	private void OnDisable()
	{
		PhotonNetwork.RemoveCallbackTarget(this);
	}

	// Token: 0x06000D72 RID: 3442 RVA: 0x00043E30 File Offset: 0x00042030
	private void Update()
	{
		this.sphere != null;
		if (!this.hasArrived)
		{
			if (this.isMoving)
			{
				this.Move();
			}
			else
			{
				this.WaitToMove();
			}
		}
		if (PhotonNetwork.IsMasterClient)
		{
			this.Sync();
		}
		this.ApplyMeshEffects();
		float num = Mathf.Lerp(1f, 5f, this.dispelFogAmount);
		this.currentCloseFog = Mathf.Lerp(this.currentCloseFog, num, Time.deltaTime * 1f);
		Shader.SetGlobalFloat("CloseDistanceMod", this.currentCloseFog);
	}

	// Token: 0x06000D73 RID: 3443 RVA: 0x00043EC0 File Offset: 0x000420C0
	private void Sync()
	{
		this.syncCounter += Time.deltaTime;
		if (this.syncCounter > 5f)
		{
			this.syncCounter = 0f;
			this.photonView.RPC("RPCA_SyncFog", RpcTarget.Others, new object[] { this.currentSize, this.isMoving });
		}
	}

	// Token: 0x06000D74 RID: 3444 RVA: 0x00043F2A File Offset: 0x0004212A
	[PunRPC]
	public void RPCA_SyncFog(float s, bool moving)
	{
		this.currentSize = s;
		this.isMoving = moving;
	}

	// Token: 0x06000D75 RID: 3445 RVA: 0x00043F3A File Offset: 0x0004213A
	public IEnumerator WaitForFogCatchUp()
	{
		this.isMoving = true;
		while (this.currentSize > 30f && this.isMoving && !this.hasArrived)
		{
			this.currentSize = Mathf.Lerp(this.currentSize, 29.5f, Time.deltaTime);
			this.currentSize = Mathf.MoveTowards(this.currentSize, 29.5f, Time.deltaTime);
			Debug.Log("Waitng for fog to catch up...");
			yield return null;
		}
		yield break;
	}

	// Token: 0x06000D76 RID: 3446 RVA: 0x00043F49 File Offset: 0x00042149
	public IEnumerator WaitForReveal()
	{
		float c = 0f;
		float t = 5f;
		this.sphere.ENABLE = 1f;
		while (c < t)
		{
			c += Time.deltaTime;
			this.sphere.REVEAL_AMOUNT = this.fogRevealCurve.Evaluate(c / t);
			this.sphere.ENABLE = this.fogFadeCurve.Evaluate(c / t);
			yield return null;
		}
		this.sphere.REVEAL_AMOUNT = 1f;
		this.sphere.ENABLE = 0f;
		yield break;
	}

	// Token: 0x06000D77 RID: 3447 RVA: 0x00043F58 File Offset: 0x00042158
	public IEnumerator DisableFog()
	{
		float c = 0f;
		float t = 1f;
		while (c < t)
		{
			c += Time.deltaTime;
			this.sphere.ENABLE = 1f - c / t;
			yield return null;
		}
		this.sphere.ENABLE = 0f;
		this.sphere.REVEAL_AMOUNT = 0f;
		yield break;
	}

	// Token: 0x06000D78 RID: 3448 RVA: 0x00043F68 File Offset: 0x00042168
	private void Move()
	{
		this.sphere.REVEAL_AMOUNT = 0f;
		this.sphere.ENABLE = Mathf.MoveTowards(this.sphere.ENABLE, 1f, Time.deltaTime * 0.1f);
		this.currentSize -= this.speed * Time.deltaTime;
		if (this.currentSize <= 30f)
		{
			this.Stop();
		}
	}

	// Token: 0x06000D79 RID: 3449 RVA: 0x00043FDC File Offset: 0x000421DC
	private void Stop()
	{
		this.hasArrived = true;
		this.isMoving = false;
	}

	// Token: 0x06000D7A RID: 3450 RVA: 0x00043FEC File Offset: 0x000421EC
	private void WaitToMove()
	{
		this.currentWaitTime += Time.deltaTime;
		if ((this.PlayersHaveMovedOn() || this.TimeToMove()) && PhotonNetwork.IsMasterClient)
		{
			this.photonView.RPC("StartMovingRPC", RpcTarget.All, Array.Empty<object>());
		}
	}

	// Token: 0x06000D7B RID: 3451 RVA: 0x00044038 File Offset: 0x00042238
	private bool TimeToMove()
	{
		return Ascents.currentAscent >= 0 && this.currentWaitTime > this.maxWaitTime && this.currentID > 0;
	}

	// Token: 0x06000D7C RID: 3452 RVA: 0x00044060 File Offset: 0x00042260
	private bool PlayersHaveMovedOn()
	{
		if (Character.AllCharacters.Count == 0)
		{
			return false;
		}
		if (Ascents.currentAscent < 0)
		{
			return false;
		}
		for (int i = 0; i < Character.AllCharacters.Count; i++)
		{
			if (Character.AllCharacters[i].Center.y < this.currentStartHeight || Character.AllCharacters[i].Center.z < this.currentStartForward)
			{
				return false;
			}
		}
		Debug.Log("Players have moved on");
		return true;
	}

	// Token: 0x06000D7D RID: 3453 RVA: 0x000440E1 File Offset: 0x000422E1
	private void ApplyMeshEffects()
	{
		this.sphere.currentSize = this.currentSize;
	}

	// Token: 0x06000D7E RID: 3454 RVA: 0x000440F4 File Offset: 0x000422F4
	public void InitNewSphere(FogSphereOrigin newOrigin)
	{
		this.sphere.fogPoint = newOrigin.transform.position;
		this.currentSize = newOrigin.size;
		this.currentStartHeight = newOrigin.moveOnHeight;
		this.currentStartForward = newOrigin.moveOnForward;
	}

	// Token: 0x06000D7F RID: 3455 RVA: 0x00044130 File Offset: 0x00042330
	[PunRPC]
	public void StartMovingRPC()
	{
		this.currentWaitTime = 0f;
		this.hasArrived = false;
		this.isMoving = true;
		GUIManager.instance.TheFogRises();
	}

	// Token: 0x06000D80 RID: 3456 RVA: 0x00044158 File Offset: 0x00042358
	public void SetFogOrigin(int id)
	{
		this.currentID = id;
		if (this.currentID < this.origins.Length)
		{
			this.hasArrived = false;
			this.sphere.gameObject.SetActive(true);
			this.InitNewSphere(this.origins[this.currentID]);
			return;
		}
		this.hasArrived = true;
		Debug.Log("Last section, disabling fog sphere");
		this.sphere.gameObject.SetActive(false);
	}

	// Token: 0x06000D81 RID: 3457 RVA: 0x000441CC File Offset: 0x000423CC
	public void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
	{
		OrbFogHandler.<>c__DisplayClass36_0 CS$<>8__locals1 = new OrbFogHandler.<>c__DisplayClass36_0();
		CS$<>8__locals1.newPlayer = newPlayer;
		CS$<>8__locals1.<>4__this = this;
		this.photonView.RPC("RPCA_SyncFog", CS$<>8__locals1.newPlayer, new object[] { this.currentSize, this.isMoving });
		if (PhotonNetwork.IsMasterClient)
		{
			base.StartCoroutine(CS$<>8__locals1.<OnPlayerEnteredRoom>g__KillLateJoinedPlayer|0());
		}
	}

	// Token: 0x06000D82 RID: 3458 RVA: 0x00044239 File Offset: 0x00042439
	[PunRPC]
	public void RPC_KillIfNotReconnect(PhotonView character)
	{
		if (RoomProperties.me.IsLocallyReconnecting())
		{
			return;
		}
		character.RPC("RPCA_Die", RpcTarget.All, new object[] { Vector3.zero });
	}

	// Token: 0x06000D83 RID: 3459 RVA: 0x00044267 File Offset: 0x00042467
	public void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
	{
	}

	// Token: 0x06000D84 RID: 3460 RVA: 0x00044269 File Offset: 0x00042469
	public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
	{
	}

	// Token: 0x06000D85 RID: 3461 RVA: 0x0004426B File Offset: 0x0004246B
	public void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps)
	{
	}

	// Token: 0x06000D86 RID: 3462 RVA: 0x0004426D File Offset: 0x0004246D
	public void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
	{
	}

	// Token: 0x04000C8B RID: 3211
	public float speed = 0.3f;

	// Token: 0x04000C8C RID: 3212
	public float maxWaitTime = 500f;

	// Token: 0x04000C8D RID: 3213
	public float currentWaitTime;

	// Token: 0x04000C8E RID: 3214
	public bool hasArrived;

	// Token: 0x04000C8F RID: 3215
	public bool isMoving;

	// Token: 0x04000C90 RID: 3216
	public float currentSize;

	// Token: 0x04000C91 RID: 3217
	public float currentStartHeight;

	// Token: 0x04000C92 RID: 3218
	public float currentStartForward;

	// Token: 0x04000C93 RID: 3219
	public float dispelFogAmount;

	// Token: 0x04000C94 RID: 3220
	private FogSphere sphere;

	// Token: 0x04000C95 RID: 3221
	private FogSphereOrigin[] origins;

	// Token: 0x04000C96 RID: 3222
	private int currentID;

	// Token: 0x04000C97 RID: 3223
	private float syncCounter;

	// Token: 0x04000C98 RID: 3224
	private PhotonView photonView;

	// Token: 0x04000C99 RID: 3225
	public AnimationCurve fogRevealCurve;

	// Token: 0x04000C9A RID: 3226
	public AnimationCurve fogFadeCurve;

	// Token: 0x04000C9B RID: 3227
	public float currentCloseFog = 1f;
}
