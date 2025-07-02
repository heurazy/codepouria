using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x020001CA RID: 458
public class Fog : MonoBehaviour
{
	// Token: 0x170000AD RID: 173
	// (get) Token: 0x06000C42 RID: 3138 RVA: 0x0003CFE1 File Offset: 0x0003B1E1
	private bool IsInFog
	{
		get
		{
			return Character.localCharacter.Center.y < base.transform.position.y;
		}
	}

	// Token: 0x06000C43 RID: 3139 RVA: 0x0003D004 File Offset: 0x0003B204
	private void Start()
	{
		this.view = base.GetComponent<PhotonView>();
	}

	// Token: 0x06000C44 RID: 3140 RVA: 0x0003D014 File Offset: 0x0003B214
	private void Update()
	{
		if (Character.localCharacter == null)
		{
			return;
		}
		if (this.stops == null)
		{
			Debug.LogError("Disabling fog movement: No stops were found");
			base.enabled = false;
			return;
		}
		this.Movement();
		this.MakePlayerCold();
		this.ApplyVisuals();
		if (this.view.IsMine)
		{
			this.Sync();
		}
		if (this.fogParticles == null)
		{
			return;
		}
		this.fogParticles.transform.position = Character.localCharacter.Center;
		if (this.IsInFog)
		{
			this.fogParticles.Play();
			Character.localCharacter.data.isInFog = true;
			return;
		}
		this.fogParticles.Stop();
		Character.localCharacter.data.isInFog = false;
	}

	// Token: 0x06000C45 RID: 3141 RVA: 0x0003D0D8 File Offset: 0x0003B2D8
	private void Sync()
	{
		this.syncCounter += Time.deltaTime;
		if (this.syncCounter > 5f)
		{
			this.syncCounter = 0f;
			this.view.RPC("RPCA_SyncFog", RpcTarget.Others, new object[] { this.fogHeight });
		}
	}

	// Token: 0x06000C46 RID: 3142 RVA: 0x0003D134 File Offset: 0x0003B334
	private void ApplyVisuals()
	{
		base.transform.position = new Vector3(Character.localCharacter.Center.x, this.fogHeight, Mathf.Clamp(Character.localCharacter.Center.z, -10000f, 870f));
		Shader.SetGlobalFloat(Fog.FogHeight, base.transform.position.y);
	}

	// Token: 0x06000C47 RID: 3143 RVA: 0x0003D19E File Offset: 0x0003B39E
	private void MakePlayerCold()
	{
		if (Character.localCharacter == null)
		{
			return;
		}
		if (this.IsInFog)
		{
			Character.localCharacter.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Cold, this.amount * Time.deltaTime, false);
		}
	}

	// Token: 0x06000C48 RID: 3144 RVA: 0x0003D1D9 File Offset: 0x0003B3D9
	private void Movement()
	{
		if (this.waiting)
		{
			this.Wait();
			return;
		}
		this.Move();
	}

	// Token: 0x06000C49 RID: 3145 RVA: 0x0003D1F0 File Offset: 0x0003B3F0
	private void Wait()
	{
		if (!this.view.IsMine)
		{
			return;
		}
		this.sinceStop += Time.deltaTime;
		if (this.TimeToMove() || this.PlayersHaveMovedOn())
		{
			this.view.RPC("RPCA_Resume", RpcTarget.All, Array.Empty<object>());
		}
	}

	// Token: 0x06000C4A RID: 3146 RVA: 0x0003D243 File Offset: 0x0003B443
	private bool TimeToMove()
	{
		return this.sinceStop > this.maxWaitTime && this.currentStop > 0;
	}

	// Token: 0x06000C4B RID: 3147 RVA: 0x0003D260 File Offset: 0x0003B460
	private bool PlayersHaveMovedOn()
	{
		if (Character.AllCharacters.Count == 0)
		{
			return false;
		}
		float num = this.StopHeight() + this.startMoveHeightThreshold;
		for (int i = 0; i < Character.AllCharacters.Count; i++)
		{
			if (Character.AllCharacters[i].Center.y < num)
			{
				return false;
			}
		}
		Debug.Log("Players have moved on");
		return true;
	}

	// Token: 0x06000C4C RID: 3148 RVA: 0x0003D2C3 File Offset: 0x0003B4C3
	[PunRPC]
	private void RPCA_Resume()
	{
		this.currentStop++;
		this.waiting = false;
		GUIManager.instance.TheFogRises();
	}

	// Token: 0x06000C4D RID: 3149 RVA: 0x0003D2E4 File Offset: 0x0003B4E4
	private void Move()
	{
		if (this.currentStop >= this.stops.Length)
		{
			return;
		}
		this.fogHeight += Time.deltaTime * this.fogSpeed;
		if (this.fogHeight > this.StopHeight())
		{
			this.Stop();
		}
	}

	// Token: 0x06000C4E RID: 3150 RVA: 0x0003D324 File Offset: 0x0003B524
	private void Stop()
	{
		this.sinceStop = 0f;
		this.waiting = true;
	}

	// Token: 0x06000C4F RID: 3151 RVA: 0x0003D338 File Offset: 0x0003B538
	private float StopHeight()
	{
		return this.stops[this.currentStop].transform.position.y;
	}

	// Token: 0x06000C50 RID: 3152 RVA: 0x0003D356 File Offset: 0x0003B556
	[PunRPC]
	public void RPCA_SyncFog(float setHeight)
	{
		this.fogHeight = setHeight;
	}

	// Token: 0x04000B35 RID: 2869
	public float fogHeight;

	// Token: 0x04000B36 RID: 2870
	public float fogSpeed = 0.4f;

	// Token: 0x04000B37 RID: 2871
	public float amount;

	// Token: 0x04000B38 RID: 2872
	private static readonly int FogHeight = Shader.PropertyToID("FogHeight");

	// Token: 0x04000B39 RID: 2873
	private Transform[] stops;

	// Token: 0x04000B3A RID: 2874
	private int currentStop;

	// Token: 0x04000B3B RID: 2875
	private float sinceStop;

	// Token: 0x04000B3C RID: 2876
	public float maxWaitTime = 180f;

	// Token: 0x04000B3D RID: 2877
	public float startMoveHeightThreshold = 60f;

	// Token: 0x04000B3E RID: 2878
	private bool waiting;

	// Token: 0x04000B3F RID: 2879
	private PhotonView view;

	// Token: 0x04000B40 RID: 2880
	public ParticleSystem fogParticles;

	// Token: 0x04000B41 RID: 2881
	private float syncCounter;
}
