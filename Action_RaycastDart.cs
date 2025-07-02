using System;
using Peak.Afflictions;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x02000181 RID: 385
public class Action_RaycastDart : ItemAction
{
	// Token: 0x06000AB6 RID: 2742 RVA: 0x00034204 File Offset: 0x00032404
	public override void RunAction()
	{
		this.FireDart();
	}

	// Token: 0x06000AB7 RID: 2743 RVA: 0x0003420C File Offset: 0x0003240C
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(this.spawnTransform.position, this.dartCollisionSize);
	}

	// Token: 0x06000AB8 RID: 2744 RVA: 0x00034230 File Offset: 0x00032430
	private void FireDart()
	{
		if (this.shotSFX)
		{
			this.shotSFX.Play(base.transform.position);
		}
		Physics.Raycast(this.spawnTransform.position, MainCamera.instance.transform.forward, out this.lineHit, this.maxDistance, HelperFunctions.terrainMapMask, QueryTriggerInteraction.Ignore);
		if (!this.lineHit.collider)
		{
			this.lineHit.distance = this.maxDistance;
			this.lineHit.point = this.spawnTransform.position + MainCamera.instance.transform.forward * this.maxDistance;
		}
		this.sphereHits = Physics.SphereCastAll(this.spawnTransform.position, this.dartCollisionSize, MainCamera.instance.transform.forward, this.lineHit.distance, LayerMask.GetMask(new string[] { "Character" }), QueryTriggerInteraction.Ignore);
		foreach (RaycastHit raycastHit in this.sphereHits)
		{
			if (raycastHit.collider)
			{
				Character componentInParent = raycastHit.collider.GetComponentInParent<Character>();
				if (componentInParent)
				{
					Debug.Log("HIT");
					if (componentInParent != base.character)
					{
						this.DartImpact(componentInParent, this.spawnTransform.position, raycastHit.point);
						return;
					}
				}
			}
		}
		this.DartImpact(null, this.spawnTransform.position, this.lineHit.point);
	}

	// Token: 0x06000AB9 RID: 2745 RVA: 0x000343C8 File Offset: 0x000325C8
	private void DartImpact(Character hitCharacter, Vector3 origin, Vector3 endpoint)
	{
		if (hitCharacter)
		{
			base.photonView.RPC("RPC_DartImpact", RpcTarget.All, new object[]
			{
				hitCharacter.photonView.Owner,
				origin,
				endpoint
			});
			return;
		}
		base.photonView.RPC("RPC_DartImpact", RpcTarget.All, new object[] { null, origin, endpoint });
	}

	// Token: 0x06000ABA RID: 2746 RVA: 0x00034440 File Offset: 0x00032640
	[PunRPC]
	private void RPC_DartImpact(Photon.Realtime.Player hitPlayer, Vector3 origin, Vector3 endpoint)
	{
		if (hitPlayer != null && hitPlayer.IsLocal)
		{
			Debug.Log("I'M HIT");
			foreach (Affliction affliction in this.afflictionsOnHit)
			{
				Character.localCharacter.refs.afflictions.AddAffliction(affliction, false);
			}
		}
		Object.Instantiate<GameObject>(this.dartVFX, endpoint, Quaternion.identity);
		GamefeelHandler.instance.AddPerlinShakeProximity(endpoint, 5f, 0.2f, 15f, 10f);
	}

	// Token: 0x04000993 RID: 2451
	public float maxDistance;

	// Token: 0x04000994 RID: 2452
	public float dartCollisionSize;

	// Token: 0x04000995 RID: 2453
	[SerializeReference]
	public Affliction[] afflictionsOnHit;

	// Token: 0x04000996 RID: 2454
	public Transform spawnTransform;

	// Token: 0x04000997 RID: 2455
	public GameObject dartVFX;

	// Token: 0x04000998 RID: 2456
	private HelperFunctions.LayerType layerMaskType;

	// Token: 0x04000999 RID: 2457
	private RaycastHit lineHit;

	// Token: 0x0400099A RID: 2458
	private RaycastHit[] sphereHits;

	// Token: 0x0400099B RID: 2459
	public SFX_Instance shotSFX;
}
