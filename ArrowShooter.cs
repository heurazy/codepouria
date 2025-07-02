using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Photon.Pun;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x0200003D RID: 61
public class ArrowShooter : MonoBehaviourPunCallbacks
{
	// Token: 0x060002F5 RID: 757 RVA: 0x00012F51 File Offset: 0x00011151
	private void Awake()
	{
		this.view = base.GetComponent<PhotonView>();
	}

	// Token: 0x060002F6 RID: 758 RVA: 0x00012F5F File Offset: 0x0001115F
	public override void OnJoinedRoom()
	{
		base.OnJoinedRoom();
		if (this.view.IsMine)
		{
			this.view.RPC("WarningArrows_RPC", RpcTarget.AllBuffered, new object[] { global::UnityEngine.Random.Range(1, 5) });
		}
	}

	// Token: 0x060002F7 RID: 759 RVA: 0x00012F9A File Offset: 0x0001119A
	private void Start()
	{
	}

	// Token: 0x060002F8 RID: 760 RVA: 0x00012F9C File Offset: 0x0001119C
	private void Update()
	{
		if (this.empty)
		{
			return;
		}
		if (!this.reloading)
		{
			RaycastHit raycastHit;
			if (Physics.SphereCast(base.transform.position, this.castRadius, base.transform.forward, out raycastHit, this.range))
			{
				bool flag = false;
				if (raycastHit.collider.gameObject.layer == LayerMask.NameToLayer("Character"))
				{
					this.targetCharacter = raycastHit.collider.gameObject.GetComponentInParent<Character>();
					this.target = raycastHit.collider.transform;
					this.hitTarget = raycastHit.point;
					flag = true;
				}
				if (!flag)
				{
					this.target = raycastHit.collider.transform;
					this.hitTarget = raycastHit.point;
				}
			}
			if (this.target != null)
			{
				this.moveAcumulator += Vector3.Distance(this.target.position, this.targetLastPosition);
				if (this.moveAcumulator > 0f)
				{
					this.moveAcumulator -= this.movementCooldown * Time.deltaTime;
				}
				this.targetLastPosition = this.target.position;
			}
			else
			{
				this.moveAcumulator = 0f;
			}
			if (this.moveAcumulator > this.movementThreshold)
			{
				this.view.RPC("FireArrow_RPC", RpcTarget.AllBuffered, Array.Empty<object>());
			}
		}
	}

	// Token: 0x060002F9 RID: 761 RVA: 0x000130FC File Offset: 0x000112FC
	public void testFire()
	{
		if (this.view.IsMine)
		{
			this.view.RPC("FireArrow_RPC", RpcTarget.AllBuffered, new object[] { base.transform.position + base.transform.forward });
		}
	}

	// Token: 0x060002FA RID: 762 RVA: 0x00013150 File Offset: 0x00011350
	[PunRPC]
	public void FireArrow_RPC()
	{
		this.firedParticles.Play();
		Vector3 vector = this.hitTarget - base.transform.position;
		Vector3 vector2 = base.transform.position + vector * 0.5f;
		ParticleSystem particleSystem = Object.Instantiate<ParticleSystem>(this.trailParticles, vector2, Quaternion.identity);
		particleSystem.shape.radius = Vector3.Distance(this.hitTarget, base.transform.position) / 2f;
		particleSystem.transform.rotation = Quaternion.LookRotation(vector, base.transform.up);
		if (this.targetCharacter != null)
		{
			this.targetCharacter.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Injury, (float)this.damagePips * 0.025f, false);
		}
		Arrow arrow = Object.Instantiate<Arrow>(this.arrowPrefab, this.hitTarget, Quaternion.identity);
		arrow.transform.rotation = quaternion.LookRotation(vector, Vector3.up);
		arrow.transform.parent = this.target;
		arrow.stuckArrow(true);
		Rigidbody rigidbody;
		if (this.target.gameObject.TryGetComponent<Rigidbody>(out rigidbody))
		{
			rigidbody.AddForce(vector.normalized * this.force, ForceMode.Impulse);
		}
		this.arrows.Add(arrow);
		this.checkMaxArrows();
		base.StartCoroutine(this.<FireArrow_RPC>g__Reload|27_0());
	}

	// Token: 0x060002FB RID: 763 RVA: 0x000132C8 File Offset: 0x000114C8
	[PunRPC]
	public void WarningArrows_RPC(int count)
	{
		for (int i = 0; i < count; i++)
		{
			Vector3 vector = base.transform.up * global::UnityEngine.Random.Range(-1f, 1f) + base.transform.right * global::UnityEngine.Random.Range(-1f, 1f);
			RaycastHit raycastHit;
			if (Physics.Raycast(base.transform.position + vector, base.transform.forward, out raycastHit, this.range))
			{
				MonoBehaviour.print(raycastHit.collider.gameObject.name);
				Arrow arrow = Object.Instantiate<Arrow>(this.arrowPrefab, raycastHit.point, Quaternion.identity);
				arrow.stuckArrow(true);
				arrow.transform.rotation = quaternion.LookRotation(raycastHit.point - base.transform.position, Vector3.up);
				arrow.transform.Rotate(new Vector3((float)global::UnityEngine.Random.Range(-10, 10), (float)global::UnityEngine.Random.Range(-10, 10), (float)global::UnityEngine.Random.Range(-10, 10)));
				arrow.transform.parent = raycastHit.transform;
			}
		}
	}

	// Token: 0x060002FC RID: 764 RVA: 0x00013407 File Offset: 0x00011607
	public void checkMaxArrows()
	{
		if (this.arrows.Count >= this.maxArrows)
		{
			this.emptyParticles.Play();
			this.empty = true;
		}
	}

	// Token: 0x060002FD RID: 765 RVA: 0x00013430 File Offset: 0x00011630
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(base.transform.position + base.transform.forward * this.range, this.castRadius);
		Gizmos.DrawLine(base.transform.position, base.transform.position + base.transform.forward * this.range);
		Gizmos.DrawRay(base.transform.position, this.hitTarget - base.transform.position);
	}

	// Token: 0x060002FF RID: 767 RVA: 0x000134EF File Offset: 0x000116EF
	[CompilerGenerated]
	private IEnumerator <FireArrow_RPC>g__Reload|27_0()
	{
		this.target = null;
		this.moveAcumulator = 0f;
		this.targetCharacter = null;
		this.reloading = true;
		yield return new WaitForSeconds(this.reloadTime);
		this.reloading = false;
		yield break;
	}

	// Token: 0x0400039A RID: 922
	[FormerlySerializedAs("damage")]
	public int damagePips;

	// Token: 0x0400039B RID: 923
	public float force;

	// Token: 0x0400039C RID: 924
	public float range;

	// Token: 0x0400039D RID: 925
	public float castRadius;

	// Token: 0x0400039E RID: 926
	public float movementThreshold;

	// Token: 0x0400039F RID: 927
	public float movementCooldown;

	// Token: 0x040003A0 RID: 928
	public Arrow arrowPrefab;

	// Token: 0x040003A1 RID: 929
	public List<Arrow> arrows = new List<Arrow>();

	// Token: 0x040003A2 RID: 930
	public int maxArrows = 100;

	// Token: 0x040003A3 RID: 931
	private PhotonView view;

	// Token: 0x040003A4 RID: 932
	public float reloadTime;

	// Token: 0x040003A5 RID: 933
	private bool reloading;

	// Token: 0x040003A6 RID: 934
	public Transform target;

	// Token: 0x040003A7 RID: 935
	private Vector3 hitTarget;

	// Token: 0x040003A8 RID: 936
	private Vector3 targetLastPosition;

	// Token: 0x040003A9 RID: 937
	private float moveAcumulator;

	// Token: 0x040003AA RID: 938
	public Character targetCharacter;

	// Token: 0x040003AB RID: 939
	public ParticleSystem trailParticles;

	// Token: 0x040003AC RID: 940
	public ParticleSystem firedParticles;

	// Token: 0x040003AD RID: 941
	public ParticleSystem emptyParticles;

	// Token: 0x040003AE RID: 942
	public bool empty;

	// Token: 0x040003AF RID: 943
	private bool initialized;
}
