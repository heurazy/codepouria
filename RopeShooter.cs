using System;
using Photon.Pun;
using pworld.Scripts.Extensions;
using UnityEngine;
using Zorro.Core;

// Token: 0x0200011B RID: 283
public class RopeShooter : ItemComponent
{
	// Token: 0x1700006E RID: 110
	// (get) Token: 0x06000851 RID: 2129 RVA: 0x0002C3D5 File Offset: 0x0002A5D5
	// (set) Token: 0x06000852 RID: 2130 RVA: 0x0002C3F0 File Offset: 0x0002A5F0
	private int Ammo
	{
		get
		{
			return base.GetData<IntItemData>(DataEntryKey.PetterItemUses, new Func<IntItemData>(this.GetNew)).Value;
		}
		set
		{
			base.GetData<IntItemData>(DataEntryKey.PetterItemUses, new Func<IntItemData>(this.GetNew)).Value = value;
			this.item.SetUseRemainingPercentage((float)value / (float)this.startAmmo);
		}
	}

	// Token: 0x06000853 RID: 2131 RVA: 0x0002C421 File Offset: 0x0002A621
	private IntItemData GetNew()
	{
		Debug.Log(string.Format("GetNew startAmmo: {0}", this.startAmmo));
		return new IntItemData
		{
			Value = this.startAmmo
		};
	}

	// Token: 0x06000854 RID: 2132 RVA: 0x0002C44E File Offset: 0x0002A64E
	public override void Awake()
	{
		base.Awake();
		Item item = this.item;
		item.OnPrimaryFinishedCast = (Action)Delegate.Combine(item.OnPrimaryFinishedCast, new Action(this.OnPrimaryFinishedCast));
	}

	// Token: 0x06000855 RID: 2133 RVA: 0x0002C47D File Offset: 0x0002A67D
	private void OnDestroy()
	{
		Item item = this.item;
		item.OnPrimaryFinishedCast = (Action)Delegate.Remove(item.OnPrimaryFinishedCast, new Action(this.OnPrimaryFinishedCast));
	}

	// Token: 0x06000856 RID: 2134 RVA: 0x0002C4A8 File Offset: 0x0002A6A8
	public void Update()
	{
		RaycastHit raycastHit;
		this.item.overrideUsability = Optionable<bool>.Some(this.WillAttach(out raycastHit));
	}

	// Token: 0x1700006F RID: 111
	// (get) Token: 0x06000857 RID: 2135 RVA: 0x0002C4CD File Offset: 0x0002A6CD
	public bool HasAmmo
	{
		get
		{
			return this.Ammo >= 1;
		}
	}

	// Token: 0x06000858 RID: 2136 RVA: 0x0002C4DC File Offset: 0x0002A6DC
	private void OnPrimaryFinishedCast()
	{
		RaycastHit raycastHit;
		if (!this.WillAttach(out raycastHit))
		{
			return;
		}
		Debug.Log("OnPrimaryFinishedCast");
		if (!this.HasAmmo)
		{
			this.fumesVFX.Play();
			Debug.Log(string.Format("totalUses < 1,  {0}", this.item.totalUses));
			for (int i = 0; i < this.emptySound.Length; i++)
			{
				this.emptySound[i].Play(base.transform.position);
			}
			return;
		}
		RaycastHit raycastHit2;
		if (!Camera.main.ForwardRay<Camera>().Raycast(out raycastHit2, HelperFunctions.LayerType.TerrainMap.ToLayerMask(), 0f))
		{
			return;
		}
		Quaternion identity = Quaternion.identity;
		if (Vector3.Angle(raycastHit2.normal, Vector3.up) < 45f)
		{
			Debug.Log("Angle is less than 45");
			ExtQuaternion.FromUpAndRightPrioUp(base.transform.forward, raycastHit2.normal);
		}
		else
		{
			Debug.Log("Angle is more than 45");
			ExtQuaternion.FromUpAndRightPrioUp(Vector3.down, -Camera.main.transform.forward);
		}
		GameObject gameObject = PhotonNetwork.Instantiate(this.ropeAnchorWithRopePref.name, this.spawnPoint.position, ExtQuaternion.FromUpAndRightPrioUp(base.transform.forward, raycastHit2.normal), 0, null);
		float num = Vector3.Distance(this.spawnPoint.position, raycastHit2.point) * 0.01f;
		this.gunshotVFX.Play();
		for (int j = 0; j < this.shotSound.Length; j++)
		{
			this.shotSound[j].Play(base.transform.position);
		}
		GamefeelHandler.instance.AddPerlinShakeProximity(this.gunshotVFX.transform.position, this.screenshakeIntensity, 0.3f, 15f, 10f);
		this.hideOnFire.SetActive(this.HasAmmo);
		int ammo = this.Ammo;
		this.Ammo = ammo - 1;
		this.photonView.RPC("Sync_Rpc", RpcTarget.AllBuffered, new object[] { this.HasAmmo });
		gameObject.GetComponent<RopeAnchorProjectile>().photonView.RPC("GetShot", RpcTarget.AllBuffered, new object[]
		{
			raycastHit2.point,
			num,
			this.length,
			-Camera.main.transform.forward
		});
		if (this.photonView.IsMine)
		{
			Singleton<AchievementManager>.Instance.AddToRunBasedFloat(RUNBASEDVALUETYPE.RopePlaced, Rope.GetLengthInMeters(this.length));
			GameUtils.instance.IncrementPermanentItemsPlaced();
		}
	}

	// Token: 0x06000859 RID: 2137 RVA: 0x0002C77B File Offset: 0x0002A97B
	[PunRPC]
	private void Sync_Rpc(bool show)
	{
		Debug.Log(string.Format("Sync_Rpc: {0}", show));
		this.hideOnFire.SetActive(show);
	}

	// Token: 0x0600085A RID: 2138 RVA: 0x0002C7A0 File Offset: 0x0002A9A0
	public bool WillAttach(out RaycastHit hit)
	{
		hit = default(RaycastHit);
		return Character.localCharacter.data.isGrounded && this.HasAmmo && Physics.Raycast(MainCamera.instance.transform.position, MainCamera.instance.transform.forward, out hit, this.maxLength, HelperFunctions.LayerType.TerrainMap.ToLayerMask(), QueryTriggerInteraction.UseGlobal);
	}

	// Token: 0x0600085B RID: 2139 RVA: 0x0002C80C File Offset: 0x0002AA0C
	public override void OnInstanceDataSet()
	{
		this.hideOnFire.SetActive(this.HasAmmo);
		Debug.Log(string.Format(" OnInstanceDataSet item.totalUses: {0}", this.Ammo));
		this.item.SetUseRemainingPercentage((float)this.Ammo / (float)this.startAmmo);
	}

	// Token: 0x040007C1 RID: 1985
	public ParticleSystem gunshotVFX;

	// Token: 0x040007C2 RID: 1986
	public ParticleSystem fumesVFX;

	// Token: 0x040007C3 RID: 1987
	public bool cantReFire;

	// Token: 0x040007C4 RID: 1988
	public Transform spawnPoint;

	// Token: 0x040007C5 RID: 1989
	public float length;

	// Token: 0x040007C6 RID: 1990
	public GameObject ropeAnchorWithRopePref;

	// Token: 0x040007C7 RID: 1991
	public GameObject hideOnFire;

	// Token: 0x040007C8 RID: 1992
	public float screenshakeIntensity = 30f;

	// Token: 0x040007C9 RID: 1993
	public int startAmmo = 1;

	// Token: 0x040007CA RID: 1994
	public SFX_Instance[] shotSound;

	// Token: 0x040007CB RID: 1995
	public SFX_Instance[] emptySound;

	// Token: 0x040007CC RID: 1996
	public float maxLength = 30f;
}
