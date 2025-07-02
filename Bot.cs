using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x02000049 RID: 73
public class Bot : MonoBehaviour
{
	// Token: 0x17000035 RID: 53
	// (get) Token: 0x0600034D RID: 845 RVA: 0x0001458C File Offset: 0x0001278C
	// (set) Token: 0x0600034E RID: 846 RVA: 0x00014594 File Offset: 0x00012794
	public Vector3 LookDirection
	{
		get
		{
			return this.lookDirection;
		}
		set
		{
			this.lookDirection = value;
		}
	}

	// Token: 0x17000036 RID: 54
	// (get) Token: 0x0600034F RID: 847 RVA: 0x0001459D File Offset: 0x0001279D
	// (set) Token: 0x06000350 RID: 848 RVA: 0x000145A5 File Offset: 0x000127A5
	public Vector2 MovementInput
	{
		get
		{
			return this.movementInput;
		}
		set
		{
			this.movementInput = value;
		}
	}

	// Token: 0x17000037 RID: 55
	// (get) Token: 0x06000351 RID: 849 RVA: 0x000145AE File Offset: 0x000127AE
	// (set) Token: 0x06000352 RID: 850 RVA: 0x000145B6 File Offset: 0x000127B6
	public bool IsSprinting
	{
		get
		{
			return this.isSprinting;
		}
		set
		{
			this.isSprinting = value;
		}
	}

	// Token: 0x17000038 RID: 56
	// (get) Token: 0x06000353 RID: 851 RVA: 0x000145BF File Offset: 0x000127BF
	public Vector3 Center
	{
		get
		{
			return this.centerTransform.position;
		}
	}

	// Token: 0x17000039 RID: 57
	// (get) Token: 0x06000354 RID: 852 RVA: 0x000145CC File Offset: 0x000127CC
	// (set) Token: 0x06000355 RID: 853 RVA: 0x000145D4 File Offset: 0x000127D4
	[CanBeNull]
	public Character TargetCharacter
	{
		get
		{
			return this.targetCharacter;
		}
		set
		{
			this.targetCharacter = value;
		}
	}

	// Token: 0x1700003A RID: 58
	// (get) Token: 0x06000356 RID: 854 RVA: 0x000145E0 File Offset: 0x000127E0
	public Vector3? DistanceToTargetCharacter
	{
		get
		{
			if (!(this.TargetCharacter == null))
			{
				return null;
			}
			return new Vector3?(this.TargetCharacter.Center - this.Center);
		}
	}

	// Token: 0x1700003B RID: 59
	// (get) Token: 0x06000357 RID: 855 RVA: 0x00014620 File Offset: 0x00012820
	public Vector3 HeadPosition
	{
		get
		{
			return this.Center + Vector3.up;
		}
	}

	// Token: 0x06000358 RID: 856 RVA: 0x00014632 File Offset: 0x00012832
	private void Awake()
	{
		this.navigator = base.GetComponentInChildren<Navigator>();
	}

	// Token: 0x06000359 RID: 857 RVA: 0x00014640 File Offset: 0x00012840
	private void Update()
	{
		this.timeSprinting = (this.IsSprinting ? (this.timeSprinting + Time.deltaTime) : 0f);
		this.timeSincePatrolEnded += Time.deltaTime;
		if (this.targetCharacter != null)
		{
			this.timeWithTarget += Time.deltaTime;
			this.timeWithoutTarget = 0f;
		}
		else
		{
			this.timeWithoutTarget += 0f;
			this.timeWithTarget = 0f;
		}
		if (this.timeSincePatrolEnded > 0.2f)
		{
			this.patrolHit = null;
		}
		Debug.DrawLine(this.Center, this.targetPos_Set, Color.cyan);
		Debug.DrawLine(this.Center, this.Center + this.navigationDirection_read, Color.blue);
		Debug.DrawLine(this.Center + Vector3.up, this.Center + Vector3.up + this.lookDirection, Color.yellow);
	}

	// Token: 0x0600035A RID: 858 RVA: 0x0001474F File Offset: 0x0001294F
	private void Start()
	{
		this.LookDirection = base.transform.forward;
	}

	// Token: 0x0600035B RID: 859 RVA: 0x00014762 File Offset: 0x00012962
	public void ClearTarget()
	{
		this.targetCharacter = null;
	}

	// Token: 0x0600035C RID: 860 RVA: 0x0001476C File Offset: 0x0001296C
	public bool CanSee(Vector3 from, Vector3 to, float maxDistance = 70f, float maxAngle = 110f)
	{
		return Vector3.Distance(from, to) <= maxDistance && Vector3.Angle(this.lookDirection, to - from) <= maxAngle && !HelperFunctions.LineCheck(from, to, HelperFunctions.LayerType.TerrainMap, 0f, QueryTriggerInteraction.Ignore).transform;
	}

	// Token: 0x0600035D RID: 861 RVA: 0x000147C0 File Offset: 0x000129C0
	public Rigidbody LookForPlayerHead(Vector3 searcherHeadPos, float maxRange = 70f, float maxAngle = 110f)
	{
		using (IEnumerator<Character> enumerator = (from character in Object.FindObjectsByType<Character>(FindObjectsSortMode.None)
			where !character.isBot
			select character).GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				Character character2 = enumerator.Current;
				if (character2 == null)
				{
					Debug.Log("No player found");
					return null;
				}
				if (Vector3.Distance(this.Center, character2.TorsoPos()) > maxRange)
				{
					return null;
				}
				if (Vector3.Angle(character2.TorsoPos() - this.Center, this.lookDirection) > maxAngle)
				{
					return null;
				}
				Bodypart bodypart = character2.GetBodypart(BodypartType.Head);
				Debug.DrawLine(searcherHeadPos, bodypart.Rig.position, Color.red);
				if (HelperFunctions.LineCheck(searcherHeadPos, bodypart.Rig.position, HelperFunctions.LayerType.TerrainMap, 0f, QueryTriggerInteraction.Ignore).transform)
				{
					return null;
				}
				Debug.Log("Found player head", bodypart.Rig);
				return bodypart.Rig;
			}
		}
		return null;
	}

	// Token: 0x0600035E RID: 862 RVA: 0x000148F0 File Offset: 0x00012AF0
	public void Patrol()
	{
		this.timeSincePatrolEnded = 0f;
		NavMeshHit navMeshHit;
		if ((this.patrolHit == null || this.remainingNavDistance < 1f) && this.navigator.TryGetPointOnNavMeshCloseTo(PatrolBoss.me.GetPoint(), out navMeshHit))
		{
			this.patrolHit = new NavMeshHit?(navMeshHit);
			this.targetPos_Set = this.patrolHit.Value.position;
		}
		this.MoveForward();
		this.LookInDirection(this.navigationDirection_read, 3f);
	}

	// Token: 0x0600035F RID: 863 RVA: 0x00014977 File Offset: 0x00012B77
	public void RotateThenMove(Vector3 dir, float rotationSpeed = 3f)
	{
		if (HelperFunctions.FlatAngle(dir, this.lookDirection) < 5f)
		{
			this.MoveForward();
		}
		else
		{
			this.StandStill();
		}
		this.LookInDirection(dir, rotationSpeed);
	}

	// Token: 0x06000360 RID: 864 RVA: 0x000149A2 File Offset: 0x00012BA2
	public void StandStill()
	{
		this.MovementInput = new Vector2(0f, 0f);
	}

	// Token: 0x06000361 RID: 865 RVA: 0x000149B9 File Offset: 0x00012BB9
	public void MoveForward()
	{
		this.MovementInput = new Vector2(0f, 1f);
	}

	// Token: 0x06000362 RID: 866 RVA: 0x000149D0 File Offset: 0x00012BD0
	public void Chase()
	{
		if (this.TargetCharacter == null)
		{
			this.StandStill();
			Debug.Log("No target character");
			return;
		}
		this.targetPos_Set = this.TargetCharacter.Center;
		this.MoveForward();
		this.LookInDirection(this.navigationDirection_read, 3f);
	}

	// Token: 0x06000363 RID: 867 RVA: 0x00014A24 File Offset: 0x00012C24
	public void LookAtPoint(Vector3 point, float rotationSpeed = 3f)
	{
		this.LookInDirection((point - this.Center).normalized, rotationSpeed);
	}

	// Token: 0x06000364 RID: 868 RVA: 0x00014A4C File Offset: 0x00012C4C
	public void LookInDirection(Vector3 direction, float rotationSpeed = 3f)
	{
		this.LookDirection = Vector3.RotateTowards(this.LookDirection, direction, Time.deltaTime * rotationSpeed, 0f);
	}

	// Token: 0x06000365 RID: 869 RVA: 0x00014A6C File Offset: 0x00012C6C
	public bool CanSeeTarget(float maxDistance = 20f, float maxAngle = 120f)
	{
		if (this.TargetCharacter != null && this.CanSee(this.HeadPosition, this.TargetCharacter.Center, maxDistance, maxAngle))
		{
			this.timeSinceSawTarget = 0f;
			return true;
		}
		this.timeSinceSawTarget += Time.deltaTime;
		return false;
	}

	// Token: 0x06000366 RID: 870 RVA: 0x00014ABC File Offset: 0x00012CBC
	public void FleeFromPoint(Vector3 point)
	{
		if (this.fleePoint == null || this.remainingNavDistance < 2f)
		{
			Vector3 normalized = (this.Center - point).normalized;
			NavMeshHit navMeshHit;
			if (this.navigator.TryGetPointOnNavMeshCloseTo(this.Center + normalized * 6f, out navMeshHit))
			{
				this.fleePoint = new NavMeshHit?(navMeshHit);
				this.targetPos_Set = this.fleePoint.Value.position;
			}
		}
		this.MoveForward();
		this.LookInDirection(this.navigationDirection_read, 3f);
	}

	// Token: 0x040003DE RID: 990
	public Vector3 targetPos_Set;

	// Token: 0x040003DF RID: 991
	public Vector3 navigationDirection_read;

	// Token: 0x040003E0 RID: 992
	public bool targetIsReachable;

	// Token: 0x040003E1 RID: 993
	public float remainingNavDistance;

	// Token: 0x040003E2 RID: 994
	public float timeSincePatrolEnded;

	// Token: 0x040003E3 RID: 995
	public float timeWithTarget;

	// Token: 0x040003E4 RID: 996
	public float timeWithoutTarget;

	// Token: 0x040003E5 RID: 997
	public Navigator navigator;

	// Token: 0x040003E6 RID: 998
	public Transform centerTransform;

	// Token: 0x040003E7 RID: 999
	public float timeSinceSawTarget;

	// Token: 0x040003E8 RID: 1000
	public float timeSprinting;

	// Token: 0x040003E9 RID: 1001
	private bool isSprinting;

	// Token: 0x040003EA RID: 1002
	private Vector3 lookDirection;

	// Token: 0x040003EB RID: 1003
	private Vector2 movementInput;

	// Token: 0x040003EC RID: 1004
	private Character targetCharacter;

	// Token: 0x040003ED RID: 1005
	private NavMeshHit? fleePoint;

	// Token: 0x040003EE RID: 1006
	private NavMeshHit? patrolHit = new NavMeshHit?(default(NavMeshHit));
}
