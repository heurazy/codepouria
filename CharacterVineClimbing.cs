using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x020001A7 RID: 423
public class CharacterVineClimbing : MonoBehaviour
{
	// Token: 0x06000BBF RID: 3007 RVA: 0x0003AD40 File Offset: 0x00038F40
	private void Awake()
	{
		this.view = base.GetComponent<PhotonView>();
	}

	// Token: 0x06000BC0 RID: 3008 RVA: 0x0003AD4E File Offset: 0x00038F4E
	private void Start()
	{
		this.character = base.GetComponent<Character>();
	}

	// Token: 0x06000BC1 RID: 3009 RVA: 0x0003AD5C File Offset: 0x00038F5C
	private void Update()
	{
		if (this.character.data.isVineClimbing)
		{
			float num = this.character.data.heldVine.LengthFactor();
			float num2 = 0.005f;
			if (this.Sliding())
			{
				this.character.data.vinePercent += num * 2f * Time.deltaTime * this.attachVel;
			}
			else
			{
				float sign = this.character.data.heldVine.GetSign(this.character.data.lookDirection_Flat, this.character.data.vinePercent);
				float num3 = this.character.data.vinePercent + num * this.climbSpeed * this.climbSpeedMod * Time.deltaTime * sign * this.character.input.movementInput.y;
				num3 = Mathf.Clamp(num3, 0.01f, 0.99f);
				this.character.data.vinePercent = num3;
				if (Mathf.Abs(this.character.input.movementInput.y) > 0.01f)
				{
					num2 = this.staminaUsage;
				}
			}
			this.character.data.vinePercent = Mathf.Clamp01(this.character.data.vinePercent);
			if (this.character.IsLocal && (this.character.input.jumpWasPressed || !this.character.UseStamina(num2 * Time.deltaTime, true) || this.character.data.currentRagdollControll < 0.5f))
			{
				this.view.RPC("StopVineClimbingRpc", RpcTarget.All, Array.Empty<object>());
			}
			this.syncC += Time.deltaTime;
			if (this.syncC > 0.25f)
			{
				this.syncC = 0f;
				this.view.RPC("RPCA_SyncVineClimb", RpcTarget.Others, new object[]
				{
					this.character.data.vinePercent,
					this.attachVel
				});
			}
		}
	}

	// Token: 0x06000BC2 RID: 3010 RVA: 0x0003AF7E File Offset: 0x0003917E
	[PunRPC]
	private void RPCA_SyncVineClimb(float p, float vel)
	{
		this.character.data.vinePercent = p;
		this.attachVel = vel;
	}

	// Token: 0x06000BC3 RID: 3011 RVA: 0x0003AF98 File Offset: 0x00039198
	private float SlideAngleMult()
	{
		Vector3 dir = this.character.data.heldVine.GetDir(this.character.data.lookDirection_Flat * this.character.input.movementInput.y, this.character.data.vinePercent);
		float num = Mathf.InverseLerp(0.5f, -0.5f, dir.y);
		return Mathf.Lerp(1f, 4f, num);
	}

	// Token: 0x06000BC4 RID: 3012 RVA: 0x0003B01C File Offset: 0x0003921C
	private void FixedUpdate()
	{
		if (this.Sliding())
		{
			if (this.character.data.vinePercent > 0.99f || this.character.data.vinePercent < 0.01f)
			{
				this.attachVel *= 0f;
			}
			this.attachVel *= 0.99f;
		}
		else
		{
			this.attachVel *= 0.95f;
		}
		if (this.character.data.isVineClimbing)
		{
			this.Climbing();
		}
	}

	// Token: 0x06000BC5 RID: 3013 RVA: 0x0003B0AF File Offset: 0x000392AF
	public bool Sliding()
	{
		return Mathf.Abs(this.attachVel) > 3f;
	}

	// Token: 0x06000BC6 RID: 3014 RVA: 0x0003B0C3 File Offset: 0x000392C3
	[PunRPC]
	private void StopVineClimbingRpc()
	{
		this.character.data.isVineClimbing = false;
		this.character.data.isJumping = false;
		this.character.data.sinceGrounded = 0f;
	}

	// Token: 0x06000BC7 RID: 3015 RVA: 0x0003B0FC File Offset: 0x000392FC
	private void Climbing()
	{
		this.character.AddForce(this.ClimbForce(), 1f, 1f);
	}

	// Token: 0x06000BC8 RID: 3016 RVA: 0x0003B119 File Offset: 0x00039319
	private Vector3 ClimbForce()
	{
		return (this.GetPosition() - this.character.TorsoPos()) * this.climbForce;
	}

	// Token: 0x06000BC9 RID: 3017 RVA: 0x0003B13C File Offset: 0x0003933C
	private Vector3 GetPosition()
	{
		return this.character.data.heldVine.GetPosition(this.character.data.vinePercent) + Vector3.down * 1f;
	}

	// Token: 0x06000BCA RID: 3018 RVA: 0x0003B178 File Offset: 0x00039378
	[PunRPC]
	public void GrabVineRpc(PhotonView ropeView, int segmentIndex)
	{
		JungleVine component = ropeView.GetComponent<JungleVine>();
		if (component == null)
		{
			Debug.LogError("Failed to get rope from network object");
			return;
		}
		Debug.Log("Start Rope Climbing!");
		this.character.data.isRopeClimbing = false;
		this.character.data.isClimbing = false;
		this.character.data.isVineClimbing = true;
		this.character.data.heldVine = component;
		this.character.data.vinePercent = component.GetPercentFromSegmentIndex(segmentIndex);
		this.attachVel = component.GetVineVel(this.character.data.avarageVelocity, this.character.data.vinePercent);
	}

	// Token: 0x04000A9A RID: 2714
	private Character character;

	// Token: 0x04000A9B RID: 2715
	public float climbForce;

	// Token: 0x04000A9C RID: 2716
	public float climbSpeed;

	// Token: 0x04000A9D RID: 2717
	public float climbSpeedMod = 1f;

	// Token: 0x04000A9E RID: 2718
	public float climbDrag = 0.85f;

	// Token: 0x04000A9F RID: 2719
	public float staminaUsage;

	// Token: 0x04000AA0 RID: 2720
	private PhotonView view;

	// Token: 0x04000AA1 RID: 2721
	private float attachVel;

	// Token: 0x04000AA2 RID: 2722
	private float syncC;
}
