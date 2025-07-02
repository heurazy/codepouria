using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x020001A4 RID: 420
public class CharacterGrabbing : MonoBehaviour
{
	// Token: 0x06000B96 RID: 2966 RVA: 0x00039E66 File Offset: 0x00038066
	private void Start()
	{
		this.character = base.GetComponent<Character>();
		Bodypart bodypart = this.character.GetBodypart(BodypartType.Hand_R);
		bodypart.collisionStayAction = (Action<Collision>)Delegate.Combine(bodypart.collisionStayAction, new Action<Collision>(this.GrabAction));
	}

	// Token: 0x06000B97 RID: 2967 RVA: 0x00039EA4 File Offset: 0x000380A4
	private void GrabAction(Collision collision)
	{
		if (!this.character.photonView.IsMine)
		{
			return;
		}
		if (this.character.data.grabJoint)
		{
			return;
		}
		if (!this.character.data.isReaching)
		{
			return;
		}
		if (this.character.data.sinceLetGoOfFriend < 0.35f)
		{
			return;
		}
		if (!collision.rigidbody)
		{
			return;
		}
		Character componentInParent = collision.transform.GetComponentInParent<Character>();
		if (!componentInParent)
		{
			return;
		}
		if (componentInParent == this.character)
		{
			return;
		}
		BodypartType partType = componentInParent.GetPartType(collision.rigidbody);
		if (partType == (BodypartType)(-1))
		{
			return;
		}
		this.character.photonView.RPC("RPCA_GrabAttach", RpcTarget.All, new object[]
		{
			componentInParent.photonView,
			(int)partType,
			collision.rigidbody.transform.InverseTransformPoint(this.character.GetBodypart(BodypartType.Hand_R).Rig.transform.position)
		});
	}

	// Token: 0x06000B98 RID: 2968 RVA: 0x00039FB0 File Offset: 0x000381B0
	[PunRPC]
	public void RPCA_GrabAttach(PhotonView view, int bodyPartID, Vector3 relativePos)
	{
		Character component = view.GetComponent<Character>();
		if (!component)
		{
			return;
		}
		Rigidbody rig = component.GetBodypart((BodypartType)bodyPartID).Rig;
		Rigidbody rig2 = this.character.GetBodypart(BodypartType.Hand_R).Rig;
		rig2.transform.position = rig.transform.TransformPoint(relativePos);
		this.character.data.grabJoint = rig2.gameObject.AddComponent<FixedJoint>();
		this.character.data.grabJoint.connectedBody = rig;
		this.character.data.grabbedPlayer = component;
		component.data.grabbingPlayer = this.character;
		Debug.Log(string.Format("Grab Attaching {0} to {1}", component, rig));
	}

	// Token: 0x06000B99 RID: 2969 RVA: 0x0003A06C File Offset: 0x0003826C
	[PunRPC]
	public void RPCA_GrabUnattach()
	{
		if (this.character.data.grabbedPlayer)
		{
			this.character.data.grabbedPlayer.data.grabbingPlayer = null;
		}
		this.character.data.grabbedPlayer = null;
		Object.Destroy(this.character.data.grabJoint);
		this.character.data.sinceLetGoOfFriend = 0f;
		Debug.Log("Grab unattaching");
	}

	// Token: 0x06000B9A RID: 2970 RVA: 0x0003A0F0 File Offset: 0x000382F0
	private void Update()
	{
		if (!this.character.refs.view.IsMine)
		{
			return;
		}
		if (this.character.data.grabbingPlayer && this.character.input.jumpWasPressed && !this.character.data.grabbingPlayer.isBot)
		{
			this.character.data.grabbingPlayer.refs.view.RPC("RPCA_GrabUnattach", RpcTarget.All, Array.Empty<object>());
		}
		if (!this.CanGrab())
		{
			if (this.character.data.grabJoint || this.character.data.isReaching)
			{
				this.character.refs.view.RPC("RPCA_StopReaching", RpcTarget.All, Array.Empty<object>());
			}
			return;
		}
		if (this.character.data.sincePressReach < 0.2f)
		{
			if (!this.character.data.isReaching)
			{
				this.character.refs.view.RPC("RPCA_StartReaching", RpcTarget.All, Array.Empty<object>());
			}
		}
		else if (this.character.data.isReaching)
		{
			this.character.refs.view.RPC("RPCA_StopReaching", RpcTarget.All, Array.Empty<object>());
		}
		if (this.character.data.grabJoint)
		{
			if (this.character.data.grabbedPlayer)
			{
				this.character.data.grabbedPlayer.LimitFalling();
			}
			if (!this.character.data.isReaching)
			{
				this.character.refs.view.RPC("RPCA_GrabUnattach", RpcTarget.All, Array.Empty<object>());
			}
		}
	}

	// Token: 0x06000B9B RID: 2971 RVA: 0x0003A2C3 File Offset: 0x000384C3
	private void FixedUpdate()
	{
		this.character.data.grabFriendDistance = 1000f;
		if (this.character.data.isReaching)
		{
			this.Reach();
		}
	}

	// Token: 0x06000B9C RID: 2972 RVA: 0x0003A2F2 File Offset: 0x000384F2
	[PunRPC]
	private void RPCA_StopReaching()
	{
		this.character.data.isReaching = false;
		if (this.character.data.grabJoint)
		{
			Object.Destroy(this.character.data.grabJoint);
		}
	}

	// Token: 0x06000B9D RID: 2973 RVA: 0x0003A331 File Offset: 0x00038531
	[PunRPC]
	private void RPCA_StartGrabbing()
	{
		this.character.data.isReaching = false;
	}

	// Token: 0x06000B9E RID: 2974 RVA: 0x0003A344 File Offset: 0x00038544
	[PunRPC]
	private void RPCA_StartReaching()
	{
		this.character.data.isReaching = true;
	}

	// Token: 0x06000B9F RID: 2975 RVA: 0x0003A358 File Offset: 0x00038558
	private void Reach()
	{
		foreach (Character character in Character.AllCharacters)
		{
			float num = Vector3.Distance(this.character.Center, character.Center);
			if (num <= 4f && Vector3.Angle(this.character.data.lookDirection, character.Center - this.character.Center) <= 60f && character.data.isClimbing && character.Center.y <= this.character.Center.y)
			{
				if (num < this.character.data.grabFriendDistance)
				{
					this.character.data.grabFriendDistance = num;
					this.character.data.sinceGrabFriend = 0f;
				}
				if (this.character.refs.view.IsMine)
				{
					GUIManager.instance.Grasp();
				}
				if (character.refs.view.IsMine)
				{
					character.DragTowards(this.character.Center, 50f);
					character.LimitFalling();
					GUIManager.instance.Grasp();
				}
			}
		}
	}

	// Token: 0x06000BA0 RID: 2976 RVA: 0x0003A4D0 File Offset: 0x000386D0
	private bool CanGrab()
	{
		return !(this.character.data.currentItem != null) && Time.time - this.character.data.lastConsumedItem >= 0.5f && !this.character.data.isClimbing && !this.character.data.isRopeClimbing && !this.character.data.isVineClimbing;
	}

	// Token: 0x06000BA1 RID: 2977 RVA: 0x0003A554 File Offset: 0x00038754
	internal void Throw(Vector3 force, float fallSeconds)
	{
		this.character.data.grabbedPlayer.RPCA_Fall(1f);
		this.character.data.grabbedPlayer.AddForce(force, 0.7f, 1f);
		this.RPCA_GrabUnattach();
	}

	// Token: 0x04000A8F RID: 2703
	private Character character;
}
