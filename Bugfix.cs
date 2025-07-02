using System;
using Peak.Afflictions;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200019A RID: 410
public class Bugfix : MonoBehaviour, IInteractible
{
	// Token: 0x06000B45 RID: 2885 RVA: 0x00037B5A File Offset: 0x00035D5A
	private void Start()
	{
		base.transform.localScale = Vector3.zero;
		this.photonView = base.GetComponent<PhotonView>();
	}

	// Token: 0x06000B46 RID: 2886 RVA: 0x00037B78 File Offset: 0x00035D78
	private void LateUpdate()
	{
		this.counter += Time.deltaTime;
		this.lifeTime += Time.deltaTime;
		if (this.targetCharacter && !this.targetCharacter.data.dead)
		{
			if (this.targetCharacter.IsLocal && this.counter > 29f)
			{
				this.targetCharacter.refs.afflictions.AddAffliction(new Affliction_PreventPoisonHealing(30f), false);
				if (this.totalStatusApplied < this.maxStatus || this.targetCharacter.refs.afflictions.GetCurrentStatus(CharacterAfflictions.STATUSTYPE.Poison) < 0.5f)
				{
					this.targetCharacter.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Poison, 0.05f, false);
					this.totalStatusApplied += 0.05f;
				}
				this.counter = 0f;
			}
			Vector3 vector = this.leg.TransformPoint(this.localPos);
			base.transform.position = vector;
			Quaternion quaternion = Quaternion.LookRotation(this.leg.TransformDirection(this.forward), this.leg.TransformDirection(this.up));
			base.transform.rotation = quaternion;
			base.transform.localScale = Vector3.LerpUnclamped(Vector3.zero, Vector3.one, this.lifeTime / 300f);
			return;
		}
		if (this.photonView.IsMine)
		{
			PhotonNetwork.Destroy(base.gameObject);
		}
	}

	// Token: 0x06000B47 RID: 2887 RVA: 0x00037D04 File Offset: 0x00035F04
	[PunRPC]
	public void AttachBug(int targetID)
	{
		PhotonView photonView = PhotonView.Find(targetID);
		this.targetCharacter = photonView.GetComponent<Character>();
		Rigidbody bodypartRig = this.targetCharacter.GetBodypartRig(BodypartType.Knee_R);
		this.leg = bodypartRig.transform;
		this.localPos = new Vector3(-0.27054f, 0f, -0.17134f);
		Vector3 vector = bodypartRig.transform.TransformPoint(this.localPos);
		Vector3 vector2 = new Vector3(0f, 55f, 0f);
		Quaternion quaternion = bodypartRig.transform.rotation * Quaternion.Euler(vector2);
		base.transform.position = vector;
		base.transform.rotation = quaternion;
		this.forward = this.leg.InverseTransformDirection(base.transform.forward);
		this.up = this.leg.InverseTransformDirection(base.transform.up);
	}

	// Token: 0x06000B48 RID: 2888 RVA: 0x00037DEC File Offset: 0x00035FEC
	public bool IsInteractible(Character interactor)
	{
		return Vector3.Angle(base.transform.position - MainCamera.instance.transform.position, MainCamera.instance.transform.forward) <= 2f + this.lifeTime / 60f;
	}

	// Token: 0x06000B49 RID: 2889 RVA: 0x00037E43 File Offset: 0x00036043
	public void Interact(Character interactor)
	{
		GameUtils.instance.InstantiateAndGrab(this.bugItem, interactor);
		this.photonView.RPC("RPCA_Remove", RpcTarget.All, Array.Empty<object>());
	}

	// Token: 0x06000B4A RID: 2890 RVA: 0x00037E6C File Offset: 0x0003606C
	[PunRPC]
	public void RPCA_Remove()
	{
		if (this.photonView.IsMine)
		{
			PhotonNetwork.Destroy(base.gameObject);
		}
	}

	// Token: 0x06000B4B RID: 2891 RVA: 0x00037E86 File Offset: 0x00036086
	public void HoverEnter()
	{
	}

	// Token: 0x06000B4C RID: 2892 RVA: 0x00037E88 File Offset: 0x00036088
	public void HoverExit()
	{
	}

	// Token: 0x06000B4D RID: 2893 RVA: 0x00037E8A File Offset: 0x0003608A
	public Vector3 Center()
	{
		return base.transform.position;
	}

	// Token: 0x06000B4E RID: 2894 RVA: 0x00037E97 File Offset: 0x00036097
	public Transform GetTransform()
	{
		return base.transform;
	}

	// Token: 0x06000B4F RID: 2895 RVA: 0x00037E9F File Offset: 0x0003609F
	public string GetInteractionText()
	{
		return "Remove tick";
	}

	// Token: 0x06000B50 RID: 2896 RVA: 0x00037EA6 File Offset: 0x000360A6
	public string GetName()
	{
		return "Tick";
	}

	// Token: 0x04000A5A RID: 2650
	public Item bugItem;

	// Token: 0x04000A5B RID: 2651
	private Transform leg;

	// Token: 0x04000A5C RID: 2652
	private Vector3 localPos;

	// Token: 0x04000A5D RID: 2653
	private Vector3 forward;

	// Token: 0x04000A5E RID: 2654
	private Vector3 up;

	// Token: 0x04000A5F RID: 2655
	public float maxStatus = 0.5f;

	// Token: 0x04000A60 RID: 2656
	private float totalStatusApplied;

	// Token: 0x04000A61 RID: 2657
	private float lifeTime;

	// Token: 0x04000A62 RID: 2658
	private PhotonView photonView;

	// Token: 0x04000A63 RID: 2659
	private Character targetCharacter;

	// Token: 0x04000A64 RID: 2660
	private float counter;
}
