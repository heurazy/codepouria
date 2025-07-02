using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000190 RID: 400
[DefaultExecutionOrder(1000002)]
public class BingBongForceAbilities : MonoBehaviour
{
	// Token: 0x06000AEF RID: 2799 RVA: 0x000361A9 File Offset: 0x000343A9
	private void Start()
	{
		this.view = base.GetComponent<PhotonView>();
		if (this.physicsType == BingBongPhysics.PhysicsType.ForcePush_Gentle || this.physicsType == BingBongPhysics.PhysicsType.ForcePush)
		{
			this.DoEffect();
		}
	}

	// Token: 0x06000AF0 RID: 2800 RVA: 0x000361CF File Offset: 0x000343CF
	[PunRPC]
	public void RPCA_BingBongInitObj(int bingbongID)
	{
		this.bingbong = PhotonView.Find(bingbongID).transform;
	}

	// Token: 0x06000AF1 RID: 2801 RVA: 0x000361E2 File Offset: 0x000343E2
	private void LateUpdate()
	{
		base.transform.position = this.bingbong.position;
		base.transform.rotation = this.bingbong.rotation;
	}

	// Token: 0x06000AF2 RID: 2802 RVA: 0x00036210 File Offset: 0x00034410
	private void Update()
	{
		this.removeAfterSeconds -= Time.deltaTime;
		this.effectTime -= Time.deltaTime;
		if (this.view.IsMine && this.removeAfterSeconds <= 0f)
		{
			PhotonNetwork.Destroy(base.gameObject);
			return;
		}
	}

	// Token: 0x06000AF3 RID: 2803 RVA: 0x00036267 File Offset: 0x00034467
	private void FixedUpdate()
	{
		if (this.effectTime > 0f && this.physicsType != BingBongPhysics.PhysicsType.ForcePush_Gentle && this.physicsType != BingBongPhysics.PhysicsType.ForcePush)
		{
			this.DoEffect();
		}
	}

	// Token: 0x06000AF4 RID: 2804 RVA: 0x00036290 File Offset: 0x00034490
	private void DoEffect()
	{
		foreach (Character character in this.GetTargets())
		{
			character.refs.movement.ApplyExtraDrag(this.drag, true);
			character.AddForce(this.GetForceDirection(character.Center) * this.force, 1f, 1f);
			character.data.sinceGrounded = Mathf.Clamp(character.data.sinceGrounded, 0f, 0.25f);
			if (this.fallAmount > 0.01f && character.IsLocal)
			{
				character.Fall(this.fallAmount);
			}
		}
	}

	// Token: 0x06000AF5 RID: 2805 RVA: 0x00036368 File Offset: 0x00034568
	private Vector3 GetForceDirection(Vector3 playerPos)
	{
		if (this.physicsType == BingBongPhysics.PhysicsType.Blow)
		{
			return this.bingbong.forward;
		}
		if (this.physicsType == BingBongPhysics.PhysicsType.Suck)
		{
			return -this.bingbong.forward;
		}
		if (this.physicsType == BingBongPhysics.PhysicsType.ForcePush)
		{
			return this.bingbong.forward;
		}
		if (this.physicsType == BingBongPhysics.PhysicsType.ForcePush_Gentle)
		{
			return this.bingbong.forward;
		}
		if (this.physicsType == BingBongPhysics.PhysicsType.ForceGrab)
		{
			return this.TargetPos() - playerPos;
		}
		return Vector3.zero;
	}

	// Token: 0x06000AF6 RID: 2806 RVA: 0x000363E8 File Offset: 0x000345E8
	private List<Character> GetTargets()
	{
		Vector3 vector = this.TargetPos();
		float num = 5f;
		List<Character> list = new List<Character>();
		foreach (Character character in Character.AllCharacters)
		{
			if (Vector3.Distance(vector, character.Center) < num)
			{
				list.Add(character);
			}
		}
		return list;
	}

	// Token: 0x06000AF7 RID: 2807 RVA: 0x00036460 File Offset: 0x00034660
	private Vector3 TargetPos()
	{
		return base.transform.TransformPoint(Vector3.forward * 5f);
	}

	// Token: 0x040009FC RID: 2556
	private PhotonView view;

	// Token: 0x040009FD RID: 2557
	private Transform bingbong;

	// Token: 0x040009FE RID: 2558
	public BingBongPhysics.PhysicsType physicsType;

	// Token: 0x040009FF RID: 2559
	public float force;

	// Token: 0x04000A00 RID: 2560
	public float drag;

	// Token: 0x04000A01 RID: 2561
	public float fallAmount;

	// Token: 0x04000A02 RID: 2562
	public float removeAfterSeconds = 2f;

	// Token: 0x04000A03 RID: 2563
	public float effectTime = 2f;
}
