using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x0200000A RID: 10
public class CharacterData : MonoBehaviourPunCallbacks
{
	// Token: 0x060000DF RID: 223 RVA: 0x00007238 File Offset: 0x00005438
	public float GetTargetRagdollControll()
	{
		if (this.carrier)
		{
			return 1f;
		}
		if (this.fallSeconds > 0f)
		{
			return 0f;
		}
		if (this.passedOut)
		{
			return 0f;
		}
		if (this.fullyPassedOut)
		{
			return 0f;
		}
		if (this.dead)
		{
			return 0f;
		}
		float num = 1f;
		float num2 = 1f - this.passOutValue;
		return Mathf.Min(num, num2);
	}

	// Token: 0x1700000E RID: 14
	// (get) Token: 0x060000E0 RID: 224 RVA: 0x000072AD File Offset: 0x000054AD
	public bool fullyConscious
	{
		get
		{
			return !this.passedOut && !this.fullyPassedOut && !this.dead;
		}
	}

	// Token: 0x1700000F RID: 15
	// (get) Token: 0x060000E1 RID: 225 RVA: 0x000072CA File Offset: 0x000054CA
	public bool isClimbingAnything
	{
		get
		{
			return this.isClimbing || this.isRopeClimbing || this.isVineClimbing;
		}
	}

	// Token: 0x17000010 RID: 16
	// (get) Token: 0x060000E2 RID: 226 RVA: 0x000072E4 File Offset: 0x000054E4
	// (set) Token: 0x060000E3 RID: 227 RVA: 0x000072EC File Offset: 0x000054EC
	public float currentStamina
	{
		get
		{
			return this._stam;
		}
		set
		{
			if (this.character.infiniteStam)
			{
				return;
			}
			this._stam = value;
		}
	}

	// Token: 0x17000011 RID: 17
	// (get) Token: 0x060000E4 RID: 228 RVA: 0x00007303 File Offset: 0x00005503
	public float TotalStamina
	{
		get
		{
			return this.currentStamina + this.extraStamina;
		}
	}

	// Token: 0x060000E5 RID: 229 RVA: 0x00007312 File Offset: 0x00005512
	private void Awake()
	{
		this.character = base.GetComponent<Character>();
		this.SetBadgeStatus();
	}

	// Token: 0x060000E6 RID: 230 RVA: 0x00007328 File Offset: 0x00005528
	public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
	{
		base.OnPlayerEnteredRoom(newPlayer);
		if (!PhotonNetwork.IsMasterClient)
		{
			return;
		}
		PhotonView photonView = this.character.photonView;
		string text = "RPC_SyncOnJoin";
		object[] array = new object[17];
		array[0] = this.passedOut;
		array[1] = this.fullyPassedOut;
		array[2] = this.dead;
		array[3] = this.isSprinting;
		int num = 4;
		Item item = this.currentItem;
		array[num] = ((item != null) ? item.photonView : null);
		array[5] = this.isJumping;
		array[6] = this.isClimbing;
		array[7] = this.isRopeClimbing;
		array[8] = this.isVineClimbing;
		array[9] = this.vinePercent;
		array[10] = this.ropePercent;
		array[11] = this.isCrouching;
		array[12] = this.isReaching;
		int num2 = 13;
		JungleVine jungleVine = this.heldVine;
		array[num2] = ((jungleVine != null) ? jungleVine.photonView : null);
		int num3 = 14;
		Rope rope = this.heldRope;
		array[num3] = ((rope != null) ? rope.photonView : null);
		array[15] = this.sprintJump;
		array[16] = this.badgeStatus;
		photonView.RPC(text, newPlayer, array);
	}

	// Token: 0x060000E7 RID: 231 RVA: 0x00007468 File Offset: 0x00005668
	[PunRPC]
	public void RPC_SyncOnJoin(bool passedOut, bool fullyPassedOut, bool dead, bool isSprinting, PhotonView currentItem, bool isJumping, bool isClimbing, bool isRopeClimbing, bool isVineClimbing, float vinePercent, float ropePercent, bool isCrouching, bool isReaching, PhotonView heldVine, PhotonView heldRope, bool sprintJump, bool[] badgeStatus)
	{
		Debug.Log(string.Format("RPC_SyncOnJoin: {0}, {1}", passedOut, fullyPassedOut));
		this.passedOut = passedOut;
		this.fullyPassedOut = fullyPassedOut;
		this.dead = dead;
		this.isSprinting = isSprinting;
		this.currentItem = ((currentItem != null) ? currentItem.GetComponent<Item>() : null);
		this.isJumping = isJumping;
		this.isClimbing = isClimbing;
		this.isRopeClimbing = isRopeClimbing;
		this.isVineClimbing = isVineClimbing;
		this.vinePercent = vinePercent;
		this.ropePercent = ropePercent;
		this.isCrouching = isCrouching;
		this.isReaching = isReaching;
		this.heldVine = ((heldVine != null) ? heldVine.GetComponent<JungleVine>() : null);
		this.heldRope = ((heldRope != null) ? heldRope.GetComponent<Rope>() : null);
		this.sprintJump = sprintJump;
		this.badgeStatus = badgeStatus;
		if (this.character.refs.badgeUnlocker == null)
		{
			Debug.LogError("Badge unlocker not found...");
			return;
		}
		this.character.refs.badgeUnlocker.BadgeUnlockVisual();
	}

	// Token: 0x060000E8 RID: 232 RVA: 0x00007574 File Offset: 0x00005774
	internal void SetBadgeStatus()
	{
		if (!this.character.IsLocal)
		{
			return;
		}
		this.badgeStatus = new bool[GUIManager.instance.mainBadgeManager.badgeData.Length];
		for (int i = 0; i < this.badgeStatus.Length; i++)
		{
			this.badgeStatus[i] = !GUIManager.instance.mainBadgeManager.badgeData[i].IsLocked;
		}
		base.photonView.RPC("SyncBadgeStatus", RpcTarget.All, new object[] { this.badgeStatus });
	}

	// Token: 0x060000E9 RID: 233 RVA: 0x000075FF File Offset: 0x000057FF
	[PunRPC]
	private void SyncBadgeStatus(bool[] statusArray)
	{
		this.badgeStatus = statusArray;
		this.character.refs.badgeUnlocker.BadgeUnlockVisual();
	}

	// Token: 0x060000EA RID: 234 RVA: 0x0000761D File Offset: 0x0000581D
	internal bool GetBadgeStatus(int index)
	{
		return index >= 0 && index < this.badgeStatus.Length && this.badgeStatus[index];
	}

	// Token: 0x17000012 RID: 18
	// (get) Token: 0x060000EB RID: 235 RVA: 0x00007638 File Offset: 0x00005838
	public bool usingWheel
	{
		get
		{
			return this.usingEmoteWheel || this.usingBackpackWheel;
		}
	}

	// Token: 0x0400006F RID: 111
	public float currentRagdollControll;

	// Token: 0x04000070 RID: 112
	[Range(0f, 1f)]
	public float passOutValue;

	// Token: 0x04000071 RID: 113
	public bool passedOut;

	// Token: 0x04000072 RID: 114
	public bool fullyPassedOut;

	// Token: 0x04000073 RID: 115
	public float deathTimer;

	// Token: 0x04000074 RID: 116
	public float sinceDied;

	// Token: 0x04000075 RID: 117
	public bool dead;

	// Token: 0x04000076 RID: 118
	public bool isGrounded;

	// Token: 0x04000077 RID: 119
	public float sinceGrounded;

	// Token: 0x04000078 RID: 120
	public Vector3 groundPos;

	// Token: 0x04000079 RID: 121
	public Vector3 groundNormal;

	// Token: 0x0400007A RID: 122
	public float targetHeadHeight;

	// Token: 0x0400007B RID: 123
	public float targetHipHeight;

	// Token: 0x0400007C RID: 124
	public Vector3 worldMovementInput;

	// Token: 0x0400007D RID: 125
	public Vector3 worldMovementInput_Grounded;

	// Token: 0x0400007E RID: 126
	public Vector3 worldMovementInput_Lerp;

	// Token: 0x0400007F RID: 127
	public Vector2 lookValues;

	// Token: 0x04000080 RID: 128
	public Vector3 lookDirection;

	// Token: 0x04000081 RID: 129
	public Vector3 lookDirection_Flat;

	// Token: 0x04000082 RID: 130
	public Vector3 lookDirection_Right;

	// Token: 0x04000083 RID: 131
	public Vector3 lookDirection_Up;

	// Token: 0x04000084 RID: 132
	public bool isSprinting;

	// Token: 0x04000085 RID: 133
	public Item currentItem;

	// Token: 0x04000086 RID: 134
	public Vector3 avarageVelocity;

	// Token: 0x04000087 RID: 135
	public Vector3 avarageLastFrameVelocity;

	// Token: 0x04000088 RID: 136
	public float sinceJump;

	// Token: 0x04000089 RID: 137
	public float sinceClimb;

	// Token: 0x0400008A RID: 138
	public float currentHeadHeight;

	// Token: 0x0400008B RID: 139
	public bool isJumping;

	// Token: 0x0400008C RID: 140
	public float groundedFor;

	// Token: 0x0400008D RID: 141
	public float lastGroundedHeight;

	// Token: 0x0400008E RID: 142
	public bool chargingJump;

	// Token: 0x0400008F RID: 143
	public bool isClimbing;

	// Token: 0x04000090 RID: 144
	public bool isRopeClimbing;

	// Token: 0x04000091 RID: 145
	public bool isVineClimbing;

	// Token: 0x04000092 RID: 146
	public Vector3 climbPos;

	// Token: 0x04000093 RID: 147
	public Vector3 climbNormal;

	// Token: 0x04000094 RID: 148
	public float spectateZoom;

	// Token: 0x04000095 RID: 149
	public bool isBlind;

	// Token: 0x04000096 RID: 150
	private float _stam;

	// Token: 0x04000097 RID: 151
	[FormerlySerializedAs("lastFrameStamina")]
	public float lastFrameTotalStamina;

	// Token: 0x04000098 RID: 152
	public float staminaDelta;

	// Token: 0x04000099 RID: 153
	public Rope heldRope;

	// Token: 0x0400009A RID: 154
	public JungleVine heldVine;

	// Token: 0x0400009B RID: 155
	public float vinePercent;

	// Token: 0x0400009C RID: 156
	public float ropePercent;

	// Token: 0x0400009D RID: 157
	public Vector3 ropeClimbNormal;

	// Token: 0x0400009E RID: 158
	public Vector3 ropeClimbWorldNormal;

	// Token: 0x0400009F RID: 159
	public Vector3 ropeClimbWorldUp;

	// Token: 0x040000A0 RID: 160
	public float sinceUseStamina;

	// Token: 0x040000A1 RID: 161
	public bool isCrouching;

	// Token: 0x040000A2 RID: 162
	public bool isReaching;

	// Token: 0x040000A3 RID: 163
	public FixedJoint grabJoint;

	// Token: 0x040000A4 RID: 164
	public float sincePressClimb = 10f;

	// Token: 0x040000A5 RID: 165
	public float sincePressReach = 10f;

	// Token: 0x040000A6 RID: 166
	public float lastConsumedItem;

	// Token: 0x040000A7 RID: 167
	public float sinceHeldItem;

	// Token: 0x040000A8 RID: 168
	public float lastAddedStatusAmount;

	// Token: 0x040000A9 RID: 169
	public bool isInFog;

	// Token: 0x040000AA RID: 170
	public bool[] badgeStatus;

	// Token: 0x040000AB RID: 171
	public float overrideIKForSeconds;

	// Token: 0x040000AC RID: 172
	public float extraStamina;

	// Token: 0x040000AD RID: 173
	public float outOfStaminaFor;

	// Token: 0x040000AE RID: 174
	public float staminaMod;

	// Token: 0x040000AF RID: 175
	public float sinceClimbJump;

	// Token: 0x040000B0 RID: 176
	public int climbingSpikeCount;

	// Token: 0x040000B1 RID: 177
	public float grabFriendDistance;

	// Token: 0x040000B2 RID: 178
	public float sinceFallSlide;

	// Token: 0x040000B3 RID: 179
	public ClimbHandle currentClimbHandle;

	// Token: 0x040000B4 RID: 180
	public float sinceClimbHandle;

	// Token: 0x040000B5 RID: 181
	public float sinceGrabFriend;

	// Token: 0x040000B6 RID: 182
	public bool usingEmoteWheel;

	// Token: 0x040000B7 RID: 183
	public bool usingBackpackWheel;

	// Token: 0x040000B8 RID: 184
	public float fallSeconds;

	// Token: 0x040000B9 RID: 185
	public float sinceAddedCold = 10f;

	// Token: 0x040000BA RID: 186
	public float sinceStartClimb;

	// Token: 0x040000BB RID: 187
	public Character carriedPlayer;

	// Token: 0x040000BC RID: 188
	public Character carrier;

	// Token: 0x040000BD RID: 189
	public bool sprintJump;

	// Token: 0x040000BE RID: 190
	public int jumpsRemaining = 1;

	// Token: 0x040000BF RID: 191
	public ClimbModifierSurface climbMod;

	// Token: 0x040000C0 RID: 192
	public float slippy;

	// Token: 0x040000C1 RID: 193
	public RaycastHit climbHit;

	// Token: 0x040000C2 RID: 194
	internal Character grabbedPlayer;

	// Token: 0x040000C3 RID: 195
	internal Character grabbingPlayer;

	// Token: 0x040000C4 RID: 196
	public Transform spawnPoint;

	// Token: 0x040000C5 RID: 197
	private Character character;

	// Token: 0x040000C6 RID: 198
	public bool isKinecmatic;

	// Token: 0x040000C7 RID: 199
	public bool isCarried;

	// Token: 0x040000C8 RID: 200
	public float sinceLetGoOfFriend;

	// Token: 0x040000C9 RID: 201
	public float sinceStandOnPlayer;

	// Token: 0x040000CA RID: 202
	public float sincePalJump = 10f;

	// Token: 0x040000CB RID: 203
	public Character lastStoodOnPlayer;

	// Token: 0x040000CC RID: 204
	public float myersDistance;

	// Token: 0x040000CD RID: 205
	public float sinceItemAttach = 10f;

	// Token: 0x040000CE RID: 206
	public float sinceCanClimb = 10f;

	// Token: 0x040000CF RID: 207
	public bool hasClimbedSinceGrounded;

	// Token: 0x040000D0 RID: 208
	public float passedOutOnTheBeach;

	// Token: 0x040000D1 RID: 209
	public float sinceDead;
}
