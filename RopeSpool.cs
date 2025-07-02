using System;
using Photon.Pun;
using pworld.Scripts.Extensions;
using UnityEngine;

// Token: 0x0200011C RID: 284
public class RopeSpool : ItemComponent
{
	// Token: 0x17000070 RID: 112
	// (get) Token: 0x0600085D RID: 2141 RVA: 0x0002C883 File Offset: 0x0002AA83
	public bool IsOutOfRope
	{
		get
		{
			return this.ropeFuel <= 2f;
		}
	}

	// Token: 0x17000071 RID: 113
	// (get) Token: 0x0600085E RID: 2142 RVA: 0x0002C895 File Offset: 0x0002AA95
	// (set) Token: 0x0600085F RID: 2143 RVA: 0x0002C8B0 File Offset: 0x0002AAB0
	public float RopeFuel
	{
		get
		{
			return base.GetData<FloatItemData>(DataEntryKey.Fuel, new Func<FloatItemData>(this.DefaultFuel)).Value;
		}
		set
		{
			base.GetData<FloatItemData>(DataEntryKey.Fuel, new Func<FloatItemData>(this.DefaultFuel)).Value = value;
			this.ropeFuel = value;
			if (this.ropeFuel <= 2f)
			{
				this.photonView.RPC("Consume", RpcTarget.All, Array.Empty<object>());
			}
			this.item.SetUseRemainingPercentage(this.ropeFuel / this.ropeStartFuel);
		}
	}

	// Token: 0x06000860 RID: 2144 RVA: 0x0002C919 File Offset: 0x0002AB19
	private FloatItemData DefaultFuel()
	{
		return new FloatItemData
		{
			Value = this.ropeStartFuel
		};
	}

	// Token: 0x17000072 RID: 114
	// (get) Token: 0x06000861 RID: 2145 RVA: 0x0002C92C File Offset: 0x0002AB2C
	// (set) Token: 0x06000862 RID: 2146 RVA: 0x0002C934 File Offset: 0x0002AB34
	public float Segments
	{
		get
		{
			return this.segments;
		}
		set
		{
			this.segments = value;
		}
	}

	// Token: 0x06000863 RID: 2147 RVA: 0x0002C93D File Offset: 0x0002AB3D
	public override void Awake()
	{
		base.Awake();
		this.ropeTier = base.GetComponent<RopeTier>();
		this.rig = base.GetComponent<Rigidbody>();
	}

	// Token: 0x06000864 RID: 2148 RVA: 0x0002C960 File Offset: 0x0002AB60
	private void OnDestroy()
	{
		if (this.item.itemState == ItemState.Held && this.photonView.IsMine)
		{
			this.ClearRope();
		}
		if (!this.photonView.IsMine)
		{
			return;
		}
		this.ropeFuel = this.RopeFuel;
		this.item.SetUseRemainingPercentage(this.ropeFuel / this.ropeStartFuel);
	}

	// Token: 0x06000865 RID: 2149 RVA: 0x0002C9C0 File Offset: 0x0002ABC0
	private void Update()
	{
		if (this.item.itemState != ItemState.Held || this.IsOutOfRope)
		{
			return;
		}
		if (!this.photonView.IsMine)
		{
			return;
		}
		if (this.ropeInstance == null && !this.IsOutOfRope)
		{
			this.ropeInstance = PhotonNetwork.Instantiate(this.ropePrefab.name, this.ropeBase.position, this.ropeBase.rotation, 0, null);
			this.rope = this.ropeInstance.GetComponent<Rope>();
			this.rope.photonView.RPC("AttachToSpool_Rpc", RpcTarget.AllBuffered, new object[] { this.photonView });
			this.Segments = 0f;
			this.segsVel = 0f;
			this.scroll = 0f;
			this.rope.Segments = this.Segments;
		}
		this.item.SetUseRemainingPercentage(((this.ropeFuel - this.rope.Segments) / this.ropeStartFuel).Clamp01());
		this.scroll = this.item.holderCharacter.input.scrollInput;
		if (this.ropeTier.LookingToPlaceAnchor)
		{
			this.scroll = 0f;
			this.segsVel = 0f;
		}
	}

	// Token: 0x06000866 RID: 2150 RVA: 0x0002CB0C File Offset: 0x0002AD0C
	private void FixedUpdate()
	{
		this.segsVel = Mathf.Lerp(this.segsVel, this.scroll, Time.fixedDeltaTime * 4f);
		this.segsVel = Mathf.Clamp(this.segsVel, -1f, 5f);
		if (this.photonView.IsMine && this.rope != null)
		{
			this.Segments += this.segsVel * Time.fixedDeltaTime * 25f;
			this.Segments = Mathf.Clamp(this.Segments, this.minSegments, Mathf.Min(this.ropeFuel, (float)Rope.MaxSegments));
			float num = this.Segments - this.rope.Segments;
			this.ropeSpoolTf.transform.localEulerAngles += new Vector3(0f, 0f, num * -50f);
			this.rope.Segments = this.Segments;
		}
	}

	// Token: 0x06000867 RID: 2151 RVA: 0x0002CC14 File Offset: 0x0002AE14
	public void ClearRope()
	{
		Debug.Log(string.Format("ClearRope{0}", this.ropeInstance));
		if (this.ropeInstance != null)
		{
			Debug.Log("Destroy rope");
			PhotonNetwork.Destroy(this.rope.view);
		}
		this.rope = null;
	}

	// Token: 0x06000868 RID: 2152 RVA: 0x0002CC68 File Offset: 0x0002AE68
	public override void OnInstanceDataSet()
	{
		if (base.HasData(DataEntryKey.Fuel))
		{
			Debug.Log("HasData");
			this.ropeFuel = base.GetData<FloatItemData>(DataEntryKey.Fuel).Value;
			Debug.Log(string.Format("ropeFuel {0}", this.ropeFuel));
		}
	}

	// Token: 0x040007CD RID: 1997
	public float segments;

	// Token: 0x040007CE RID: 1998
	public float minSegments = 3.5f;

	// Token: 0x040007CF RID: 1999
	public float ropeStartFuel = 60f;

	// Token: 0x040007D0 RID: 2000
	private float ropeFuel = 60f;

	// Token: 0x040007D1 RID: 2001
	public GameObject ropePrefab;

	// Token: 0x040007D2 RID: 2002
	public Transform ropeBase;

	// Token: 0x040007D3 RID: 2003
	public Transform ropeStart;

	// Token: 0x040007D4 RID: 2004
	public Transform ropeSpoolTf;

	// Token: 0x040007D5 RID: 2005
	public GameObject ropeInstance;

	// Token: 0x040007D6 RID: 2006
	public Rigidbody rig;

	// Token: 0x040007D7 RID: 2007
	public Rope rope;

	// Token: 0x040007D8 RID: 2008
	private float scroll;

	// Token: 0x040007D9 RID: 2009
	private float segsVel;

	// Token: 0x040007DA RID: 2010
	private RopeTier ropeTier;

	// Token: 0x040007DB RID: 2011
	public bool isAntiRope;
}
