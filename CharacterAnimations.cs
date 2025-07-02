using System;
using Photon.Pun;
using UnityEngine;
using Zorro.Core;

// Token: 0x02000009 RID: 9
public class CharacterAnimations : MonoBehaviour
{
	// Token: 0x060000CE RID: 206 RVA: 0x00006587 File Offset: 0x00004787
	private void Awake()
	{
		this.character = base.GetComponent<Character>();
	}

	// Token: 0x060000CF RID: 207 RVA: 0x00006598 File Offset: 0x00004798
	private void Start()
	{
		Character character = this.character;
		character.landAction = (Action<float>)Delegate.Combine(character.landAction, new Action<float>(this.Land));
		Character character2 = this.character;
		character2.startJumpAction = (Action)Delegate.Combine(character2.startJumpAction, new Action(this.StartJump));
		Character character3 = this.character;
		character3.jumpAction = (Action)Delegate.Combine(character3.jumpAction, new Action(this.Jump));
		Character character4 = this.character;
		character4.startClimbAction = (Action)Delegate.Combine(character4.startClimbAction, new Action(this.StartClimb));
	}

	// Token: 0x060000D0 RID: 208 RVA: 0x00006644 File Offset: 0x00004844
	private void Update()
	{
		Animator animator = this.character.refs.animator;
		animator.SetBool("Climb Surface", this.character.data.isClimbing);
		animator.SetBool("Climb Rope", this.character.data.isRopeClimbing);
		animator.SetFloat("Input X", this.character.input.movementInput.x, 0.125f, Time.deltaTime);
		animator.SetFloat("Input Y", this.character.input.movementInput.y, 0.125f, Time.deltaTime);
		animator.SetFloat("Throw Charge", this.character.refs.items.throwChargeLevel);
		animator.SetFloat("Throw", this.throwTime);
		if (Mathf.Abs(animator.GetFloat("Input X")) < 0.125f && Mathf.Abs(this.character.input.movementInput.x) < 0.125f)
		{
			animator.SetFloat("Input X", 0f);
		}
		if (Mathf.Abs(animator.GetFloat("Input Y")) < 0.125f && Mathf.Abs(this.character.input.movementInput.y) < 0.125f)
		{
			animator.SetFloat("Input Y", 0f);
		}
		animator.SetBool("Is Grounded", true);
		animator.SetFloat("Velocity Y", this.character.data.avarageVelocity.y);
		animator.SetFloat("Velocity Z", this.character.data.avarageVelocity.z);
		if (this.lookRef)
		{
			Vector3 cameraPos = this.character.GetCameraPos(0f);
			Vector3 lookDirection = this.character.data.lookDirection;
			Matrix4x4 matrix4x = Matrix4x4.TRS(cameraPos, Quaternion.LookRotation(lookDirection, Vector3.up), Vector3.one);
			this.lookRef.rotation = Quaternion.Euler(0f, matrix4x.rotation.eulerAngles.y, 0f);
			animator.SetFloat("Look Y", matrix4x.inverse.TransformDirection(this.lookRef.forward).y);
			animator.SetFloat("Look X", this.character.input.lookInput.x, 0.25f, Time.deltaTime);
		}
		if (this.character.data.sinceGrounded > 0.3f || this.character.data.avarageVelocity.y > 5f || this.character.data.isJumping || this.character.data.sinceClimb < 0.25f)
		{
			animator.SetBool("Is Grounded", false);
		}
		if (this.character.data.isSprinting)
		{
			animator.SetFloat("Sprint", 1f, 0.125f, Time.deltaTime);
		}
		if (!this.character.data.isSprinting)
		{
			animator.SetFloat("Sprint", 0f, 0.125f, Time.deltaTime);
		}
		animator.SetBool("Crouch", this.character.data.isCrouching);
		animator.SetBool("Reach", this.character.data.isReaching);
		animator.SetBool("Grab", this.character.data.grabJoint);
		animator.SetBool("Vine Hang", this.character.data.isVineClimbing);
		if (this.character.data.isVineClimbing && this.character.data.heldVine)
		{
			animator.SetBool("Vine Slide", this.character.refs.vineClimbing.Sliding());
			animator.SetInteger("Vine Type", this.character.data.heldVine.vineType);
		}
		animator.SetBool("Is Sliding", this.character.IsSliding());
		animator.SetBool("Climb Jump", this.character.data.sinceClimbJump < 0.3f);
		if (!this.character.data.isSprinting && animator.GetFloat("Sprint") < 0.75f)
		{
			animator.SetFloat("Sprint", 0f);
		}
		animator.SetBool("Charge Jump", this.character.data.chargingJump);
		animator.SetBool("Jump", this.character.data.isJumping);
		animator.SetFloat("Since Grounded", this.character.data.sinceGrounded, 0.25f, Time.deltaTime);
		animator.SetInteger("Reach Type", 0);
		animator.SetFloat("Myers Distance", this.character.data.myersDistance);
		this.character.data.myersDistance = 1000f;
		animator.SetBool("Hang", this.character.data.currentClimbHandle != null);
		animator.SetBool("Help", false);
		if (this.character.data.grabFriendDistance <= 3.5f && !this.character.data.isClimbing)
		{
			animator.SetBool("Help", true);
		}
		if (!animator.GetBool("Is Grounded"))
		{
			animator.SetInteger("Reach Type", 1);
		}
		if (this.character.data.isCrouching)
		{
			animator.SetInteger("Reach Type", 2);
		}
		this.HandleIK();
		this.SetAnimSpeed();
		Bodypart bodypart = this.character.GetBodypart(BodypartType.Hip);
		this.character.refs.animationPositionTransform.position = bodypart.transform.position;
		this.throwTime -= Time.deltaTime;
		if (this.throwTime <= 0f)
		{
			this.throwTime = 0f;
		}
		this.sinceEmoteStart += Time.deltaTime;
		if (this.emoting && (this.sinceEmoteStart > 2f || (this.sinceEmoteStart > 0.7f && (this.character.input.movementInput.magnitude > 0.1f || this.character.input.jumpWasPressed || this.character.data.sinceGrounded > 0.2f))))
		{
			this.character.refs.animator.SetBool("Emote", false);
			this.emoting = false;
		}
	}

	// Token: 0x060000D1 RID: 209 RVA: 0x00006CFC File Offset: 0x00004EFC
	private void SetAnimSpeed()
	{
		if (this.character.data.carrier)
		{
			this.character.refs.animator.speed = 1f;
			return;
		}
		if (this.character.data.dead || this.character.data.fullyPassedOut)
		{
			this.character.refs.animator.speed = 0f;
			return;
		}
		if (this.character.data.isClimbing && this.character.data.sinceClimbJump > 0.5f)
		{
			this.character.refs.animator.speed = this.character.data.staminaMod;
			return;
		}
		this.character.refs.animator.speed = 1f;
	}

	// Token: 0x060000D2 RID: 210 RVA: 0x00006DE3 File Offset: 0x00004FE3
	private bool ReachIK()
	{
		return !this.character.data.isCrouching && this.character.data.isReaching && this.character.data.sinceGrabFriend > 0.5f;
	}

	// Token: 0x060000D3 RID: 211 RVA: 0x00006E24 File Offset: 0x00005024
	private void HandleIK()
	{
		if (!this.character.refs.ikRight)
		{
			return;
		}
		if (this.ReachIK())
		{
			this.character.refs.ikRig.weight = 1f;
			this.character.refs.ikRight.weight = 1f;
			this.character.refs.ikLeft.weight = 0f;
			return;
		}
		if (this.character.data.currentItem && this.character.data.overrideIKForSeconds <= 0f)
		{
			this.character.refs.ikRig.weight = 1f;
			this.character.refs.ikRight.weight = 1f;
			this.character.refs.ikLeft.weight = 1f;
			return;
		}
		this.character.refs.ikRig.weight = 0f;
	}

	// Token: 0x060000D4 RID: 212 RVA: 0x00006F37 File Offset: 0x00005137
	private void Land(float sinceGrounded)
	{
	}

	// Token: 0x060000D5 RID: 213 RVA: 0x00006F39 File Offset: 0x00005139
	private void Jump()
	{
	}

	// Token: 0x060000D6 RID: 214 RVA: 0x00006F3B File Offset: 0x0000513B
	private void StartJump()
	{
	}

	// Token: 0x060000D7 RID: 215 RVA: 0x00006F3D File Offset: 0x0000513D
	private void StartClimb()
	{
	}

	// Token: 0x060000D8 RID: 216 RVA: 0x00006F3F File Offset: 0x0000513F
	public void PlaySpecificAnimation(string animationName)
	{
		if (this.character.refs.animator == null)
		{
			return;
		}
		this.character.refs.animator.Play(animationName, 0, 0f);
	}

	// Token: 0x060000D9 RID: 217 RVA: 0x00006F76 File Offset: 0x00005176
	public void PrepIK()
	{
	}

	// Token: 0x060000DA RID: 218 RVA: 0x00006F78 File Offset: 0x00005178
	public void ConfigureIK()
	{
		if (this.character.refs.IKHandTargetLeft == null)
		{
			return;
		}
		if (this.character.data.currentItem)
		{
			this.character.refs.IKHandTargetLeft.position = this.character.refs.items.GetItemPosLeft(this.character.data.currentItem);
			this.character.refs.IKHandTargetRight.position = this.character.refs.items.GetItemPosRight(this.character.data.currentItem);
			this.character.refs.IKHandTargetRight.rotation = this.character.refs.items.GetItemRotRight(this.character.data.currentItem);
			this.character.refs.IKHandTargetLeft.rotation = this.character.refs.items.GetItemRotLeft(this.character.data.currentItem);
			return;
		}
		if (this.ReachIK())
		{
			this.character.refs.IKHandTargetRight.position = this.character.refs.animationHeadTransform.position + this.character.refs.animationLookTransform.TransformDirection(new Vector3(0.15f, -0.1f, 1.5f));
			this.character.refs.IKHandTargetRight.localEulerAngles = new Vector3(this.ReachHandPos.x, this.ReachHandPos.y, this.ReachHandPos.z + this.character.data.lookValues.y);
		}
	}

	// Token: 0x060000DB RID: 219 RVA: 0x00007155 File Offset: 0x00005355
	internal void PlayEmote(string emoteName)
	{
		this.character.refs.view.RPC("RPCA_PlayRemove", RpcTarget.All, new object[] { emoteName });
	}

	// Token: 0x060000DC RID: 220 RVA: 0x0000717C File Offset: 0x0000537C
	[PunRPC]
	private void RPCA_PlayRemove(string emoteName)
	{
		if (emoteName == "A_Scout_Emote_Flex")
		{
			this.character.Fall(3f);
			return;
		}
		this.character.refs.animator.SetBool("Emote", true);
		this.character.refs.animator.Play(emoteName, 0, 0f);
		this.sinceEmoteStart = 0f;
		this.emoting = true;
	}

	// Token: 0x060000DD RID: 221 RVA: 0x000071F0 File Offset: 0x000053F0
	internal void SetBool(string boolKey, bool boolValue)
	{
		this.character.refs.animator.SetBool(boolKey, boolValue);
	}

	// Token: 0x04000069 RID: 105
	private Character character;

	// Token: 0x0400006A RID: 106
	public Transform lookRef;

	// Token: 0x0400006B RID: 107
	[HideInInspector]
	public float throwTime;

	// Token: 0x0400006C RID: 108
	private Vector3 ReachHandPos = new Vector3(-30f, -90f, -70f);

	// Token: 0x0400006D RID: 109
	private bool emoting;

	// Token: 0x0400006E RID: 110
	private float sinceEmoteStart = 10f;
}
