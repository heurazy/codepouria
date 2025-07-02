using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000120 RID: 288
public class TempleEntranceRope : RopeAnchorWithRope
{
	// Token: 0x06000877 RID: 2167 RVA: 0x0002D375 File Offset: 0x0002B575
	public override void Awake()
	{
		base.Awake();
		this.doorStartingPosition = this.doorRb.transform.position;
	}

	// Token: 0x06000878 RID: 2168 RVA: 0x0002D393 File Offset: 0x0002B593
	public void Update()
	{
		if (!PhotonNetwork.InRoom)
		{
			return;
		}
		this.UpdateWeight();
	}

	// Token: 0x06000879 RID: 2169 RVA: 0x0002D3A3 File Offset: 0x0002B5A3
	public void FixedUpdate()
	{
		this.UpdateDoorOpen();
	}

	// Token: 0x0600087A RID: 2170 RVA: 0x0002D3AB File Offset: 0x0002B5AB
	[PunRPC]
	private void SetWeightRPC(float weight)
	{
		Debug.Log(string.Format("Received weight RPC. {0}", weight));
		this.currentWeight = weight;
		if (this.currentWeight > this.lockWeight)
		{
			this.lockedOpen = true;
		}
	}

	// Token: 0x0600087B RID: 2171 RVA: 0x0002D3DE File Offset: 0x0002B5DE
	private void UpdateDescent()
	{
		float num = this.currentWeight / this.weightPerSegment;
	}

	// Token: 0x0600087C RID: 2172 RVA: 0x0002D3F0 File Offset: 0x0002B5F0
	private void UpdateDoorOpen()
	{
		float num = Mathf.Min(this.doorHeightPerWeight * this.currentWeight, this.maxDoorHeight);
		this.currentDoorTarget = this.doorStartingPosition + Vector3.up * num;
		Vector3 vector = this.currentDoorTarget - this.doorRb.transform.position;
		if (vector.y > 0f)
		{
			Vector3 vector2 = Vector3.ClampMagnitude(Vector3.Lerp(this.doorRb.position, this.currentDoorTarget, this.doorLerpSpeedUp * Time.fixedDeltaTime) - this.doorRb.position, this.maxDoorMoveSpeedUp * Time.fixedDeltaTime);
			this.doorRb.MovePosition(this.doorRb.position + vector2);
			return;
		}
		if (vector.y < 0f)
		{
			this.doorRb.MovePosition(Vector3.MoveTowards(this.doorRb.position, this.currentDoorTarget, this.doorMoveSpeedDown * Time.fixedDeltaTime));
		}
	}

	// Token: 0x0600087D RID: 2173 RVA: 0x0002D4F6 File Offset: 0x0002B6F6
	public override void OnJoinedRoom()
	{
		base.OnJoinedRoom();
		if (base.photonView.IsMine)
		{
			base.photonView.RPC("SetWeightRPC", RpcTarget.All, new object[] { this.currentWeight });
		}
	}

	// Token: 0x0600087E RID: 2174 RVA: 0x0002D530 File Offset: 0x0002B730
	private void UpdateWeight()
	{
		if (!base.photonView.IsMine || !this.rope)
		{
			return;
		}
		float num = 0f;
		foreach (Character character in this.rope.charactersClimbing)
		{
			num += this.baseScoutWeight;
			num += character.refs.afflictions.GetCurrentStatus(CharacterAfflictions.STATUSTYPE.Weight);
		}
		if ((!this.lockedOpen || num > this.currentWeight) && this.currentWeight != num)
		{
			base.photonView.RPC("SetWeightRPC", RpcTarget.All, new object[] { num });
		}
	}

	// Token: 0x040007EF RID: 2031
	public float baseScoutWeight;

	// Token: 0x040007F0 RID: 2032
	public float weightPerSegment;

	// Token: 0x040007F1 RID: 2033
	public float currentWeight;

	// Token: 0x040007F2 RID: 2034
	[Header("Weight at which the door will lock open.")]
	public float lockWeight;

	// Token: 0x040007F3 RID: 2035
	public Rigidbody doorRb;

	// Token: 0x040007F4 RID: 2036
	public float doorHeightPerWeight;

	// Token: 0x040007F5 RID: 2037
	public float maxDoorHeight;

	// Token: 0x040007F6 RID: 2038
	public float doorLerpSpeedUp;

	// Token: 0x040007F7 RID: 2039
	public float maxDoorMoveSpeedUp;

	// Token: 0x040007F8 RID: 2040
	public float doorMoveSpeedDown;

	// Token: 0x040007F9 RID: 2041
	private Vector3 doorStartingPosition;

	// Token: 0x040007FA RID: 2042
	private Vector3 currentDoorTarget;

	// Token: 0x040007FB RID: 2043
	private bool lockedOpen;
}
