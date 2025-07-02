using System;
using UnityEngine;

// Token: 0x0200004A RID: 74
public class BotBoar : MonoBehaviour
{
	// Token: 0x06000368 RID: 872 RVA: 0x00014B83 File Offset: 0x00012D83
	private void Awake()
	{
		this.bot = base.GetComponentInChildren<Bot>();
		this.character = base.GetComponent<Character>();
	}

	// Token: 0x06000369 RID: 873 RVA: 0x00014B9D File Offset: 0x00012D9D
	private void Start()
	{
	}

	// Token: 0x0600036A RID: 874 RVA: 0x00014B9F File Offset: 0x00012D9F
	public void ClearTarget()
	{
		this.bot.ClearTarget();
	}

	// Token: 0x0600036B RID: 875 RVA: 0x00014BAC File Offset: 0x00012DAC
	private void Update()
	{
		this.bot.navigator.SetAgentVelocity(this.character.GetBodypart(BodypartType.Torso).Rig.linearVelocity);
		if (this.bot.timeSprinting > 3f)
		{
			this.bot.IsSprinting = false;
		}
		if (this.flee)
		{
			Debug.Log("Fleeing");
			if (this.bot.TargetCharacter == null || this.outOfSightTime >= 4f)
			{
				this.flee = false;
				this.outOfSightTime = 0f;
				this.bot.ClearTarget();
				this.potentialTarget = null;
				return;
			}
			this.bot.FleeFromPoint(this.bot.TargetCharacter.Center);
			if (this.bot.CanSee(this.bot.TargetCharacter.Head, this.bot.Center, 20f, 360f))
			{
				Debug.DrawLine(this.bot.TargetCharacter.Head, this.bot.Center, Color.green);
				this.outOfSightTime = 0f;
				return;
			}
			Debug.DrawLine(this.bot.TargetCharacter.Head, this.bot.Center, Color.red);
			this.outOfSightTime += Time.deltaTime;
			return;
		}
		else
		{
			if (this.bot.TargetCharacter)
			{
				Debug.Log("Chasing");
				Vector3? vector = this.bot.DistanceToTargetCharacter;
				if (vector == null || vector.GetValueOrDefault().magnitude <= 4f)
				{
					if (this.bot.timeWithTarget <= 15f)
					{
						goto IL_01E6;
					}
					vector = this.bot.DistanceToTargetCharacter;
					if (vector == null || vector.GetValueOrDefault().magnitude <= 2f)
					{
						goto IL_01E6;
					}
				}
				this.bot.ClearTarget();
				IL_01E6:
				this.bot.Chase();
				this.bot.CanSeeTarget(20f, 120f);
				if (this.bot.timeSinceSawTarget > 5f)
				{
					this.bot.ClearTarget();
				}
				if (!this.bot.IsSprinting)
				{
					this.flee = true;
				}
				return;
			}
			if (this.potentialTarget != null)
			{
				Debug.Log("Looking at target");
				if (!this.bot.CanSee(this.bot.HeadPosition, this.potentialTarget.Center, 70f, 110f))
				{
					this.potentialTarget = null;
					this.timeLookingAtTarget = 0f;
					return;
				}
				this.bot.StandStill();
				this.bot.LookAtPoint(this.potentialTarget.Center, 3f);
				this.timeLookingAtTarget += Time.deltaTime;
				if (this.timeLookingAtTarget > 4f)
				{
					this.bot.TargetCharacter = this.potentialTarget;
					this.bot.IsSprinting = true;
					this.potentialTarget = null;
					this.timeLookingAtTarget = 0f;
				}
			}
			if (this.potentialTarget == null)
			{
				this.bot.Patrol();
				Rigidbody rigidbody = this.bot.LookForPlayerHead(this.bot.HeadPosition, 20f, 110f);
				this.potentialTarget = ((rigidbody != null) ? rigidbody.GetComponentInParent<Character>() : null);
			}
			return;
		}
	}

	// Token: 0x040003EF RID: 1007
	private Bot bot;

	// Token: 0x040003F0 RID: 1008
	private Rigidbody rig_g;

	// Token: 0x040003F1 RID: 1009
	private Character character;

	// Token: 0x040003F2 RID: 1010
	private Vector3 startPosition;

	// Token: 0x040003F3 RID: 1011
	public float timeSinceSawTarget;

	// Token: 0x040003F4 RID: 1012
	public Character potentialTarget;

	// Token: 0x040003F5 RID: 1013
	public float timeLookingAtTarget;

	// Token: 0x040003F6 RID: 1014
	public float timeSprinting;

	// Token: 0x040003F7 RID: 1015
	private bool flee;

	// Token: 0x040003F8 RID: 1016
	private float outOfSightTime;
}
