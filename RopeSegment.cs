using System;
using Photon.Pun;
using UnityEngine;
using Zorro.Core;

// Token: 0x0200011A RID: 282
public class RopeSegment : MonoBehaviour, IInteractible
{
	// Token: 0x06000843 RID: 2115 RVA: 0x0002C1B8 File Offset: 0x0002A3B8
	private void Awake()
	{
		this.rope = base.GetComponentInParent<Rope>();
	}

	// Token: 0x06000844 RID: 2116 RVA: 0x0002C1C6 File Offset: 0x0002A3C6
	private void Update()
	{
		this.angle = this.GetAngle();
	}

	// Token: 0x06000845 RID: 2117 RVA: 0x0002C1D4 File Offset: 0x0002A3D4
	public Vector3 Center()
	{
		return base.transform.position;
	}

	// Token: 0x06000846 RID: 2118 RVA: 0x0002C1E1 File Offset: 0x0002A3E1
	public string GetInteractionText()
	{
		return "Grab";
	}

	// Token: 0x06000847 RID: 2119 RVA: 0x0002C1E8 File Offset: 0x0002A3E8
	public string GetName()
	{
		return this.displayName;
	}

	// Token: 0x06000848 RID: 2120 RVA: 0x0002C1F0 File Offset: 0x0002A3F0
	public Transform GetTransform()
	{
		return base.transform;
	}

	// Token: 0x06000849 RID: 2121 RVA: 0x0002C1F8 File Offset: 0x0002A3F8
	public void Interact(Character interactor)
	{
		interactor.refs.items.EquipSlot(Optionable<byte>.None);
		int num = base.transform.GetSiblingIndex() - 2;
		Debug.Log(string.Format("Grabbing Rope: {0} with index {1}", base.gameObject.name, num));
		interactor.GetComponent<PhotonView>().RPC("GrabRopeRpc", RpcTarget.All, new object[]
		{
			this.rope.GetComponentInParent<PhotonView>(),
			num
		});
	}

	// Token: 0x0600084A RID: 2122 RVA: 0x0002C278 File Offset: 0x0002A478
	public bool IsInteractible(Character interactor)
	{
		float num = this.GetAngle();
		bool flag = num < interactor.refs.ropeHandling.maxRopeAngle * 0.6f || 180f - num < interactor.refs.ropeHandling.maxRopeAngle * 0.6f;
		flag = flag && this.rope.isClimbable;
		if (interactor.data.isRopeClimbing)
		{
			flag = flag && interactor.data.heldRope != this.rope;
		}
		return flag;
	}

	// Token: 0x0600084B RID: 2123 RVA: 0x0002C305 File Offset: 0x0002A505
	public float GetAngle()
	{
		return Vector3.Angle(Vector3.up, base.transform.up);
	}

	// Token: 0x0600084C RID: 2124 RVA: 0x0002C31C File Offset: 0x0002A51C
	internal Vector3 GetClimbNormal(Vector3 charPos)
	{
		Vector3 vector = charPos - base.transform.position;
		vector = Vector3.ProjectOnPlane(vector, base.transform.up);
		return base.transform.InverseTransformDirection(vector);
	}

	// Token: 0x0600084D RID: 2125 RVA: 0x0002C35C File Offset: 0x0002A55C
	internal void Tie(Vector3 tiePos)
	{
		RopeSegment.<>c__DisplayClass13_0 CS$<>8__locals1 = new RopeSegment.<>c__DisplayClass13_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.joint = base.gameObject.AddComponent<ConfigurableJoint>();
		CS$<>8__locals1.joint.xMotion = ConfigurableJointMotion.Locked;
		CS$<>8__locals1.joint.yMotion = ConfigurableJointMotion.Locked;
		CS$<>8__locals1.joint.zMotion = ConfigurableJointMotion.Locked;
		CS$<>8__locals1.joint.anchor = Vector3.zero;
		base.StartCoroutine(CS$<>8__locals1.<Tie>g__ITighten|0(tiePos));
	}

	// Token: 0x0600084E RID: 2126 RVA: 0x0002C3C9 File Offset: 0x0002A5C9
	public void HoverEnter()
	{
	}

	// Token: 0x0600084F RID: 2127 RVA: 0x0002C3CB File Offset: 0x0002A5CB
	public void HoverExit()
	{
	}

	// Token: 0x040007BE RID: 1982
	public Rope rope;

	// Token: 0x040007BF RID: 1983
	public float angle;

	// Token: 0x040007C0 RID: 1984
	public string displayName;
}
