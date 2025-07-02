using System;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x0200004E RID: 78
public class Navigator : MonoBehaviour
{
	// Token: 0x06000378 RID: 888 RVA: 0x000151B0 File Offset: 0x000133B0
	private void Awake()
	{
		this.agent = base.GetComponent<NavMeshAgent>();
		this.agent.updatePosition = false;
		this.agent.updateRotation = false;
		this.bot = base.GetComponentInParent<Bot>();
	}

	// Token: 0x06000379 RID: 889 RVA: 0x000151E2 File Offset: 0x000133E2
	private void Start()
	{
	}

	// Token: 0x0600037A RID: 890 RVA: 0x000151E4 File Offset: 0x000133E4
	public bool TryGetPointOnNavMeshCloseTo(Vector3 position, out NavMeshHit hit)
	{
		return NavMesh.SamplePosition(position, out hit, 2f, 1 << NavMesh.GetAreaFromName("Walkable"));
	}

	// Token: 0x0600037B RID: 891 RVA: 0x00015204 File Offset: 0x00013404
	private void Update()
	{
		this.agent.nextPosition = this.bot.Center;
		this.bot.navigationDirection_read = this.agent.desiredVelocity.normalized;
		if (this.agent.isOnNavMesh)
		{
			this.bot.remainingNavDistance = this.agent.remainingDistance;
		}
		if (this.lastReadTargetPosition == this.bot.targetPos_Set)
		{
			return;
		}
		if (this.agent.isOnNavMesh)
		{
			this.lastReadTargetPosition = this.bot.targetPos_Set;
			this.agent.SetDestination(this.lastReadTargetPosition);
		}
	}

	// Token: 0x0600037C RID: 892 RVA: 0x000152B1 File Offset: 0x000134B1
	public void SetAgentVelocity(Vector3 velocity)
	{
		this.agent.velocity = velocity;
	}

	// Token: 0x04000402 RID: 1026
	[HideInInspector]
	public NavMeshAgent agent;

	// Token: 0x04000403 RID: 1027
	private Bot bot;

	// Token: 0x04000404 RID: 1028
	private Vector3 lastReadTargetPosition;
}
