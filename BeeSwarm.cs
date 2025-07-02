using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000006 RID: 6
public class BeeSwarm : MonoBehaviourPun
{
	// Token: 0x17000002 RID: 2
	// (get) Token: 0x06000026 RID: 38 RVA: 0x0000266C File Offset: 0x0000086C
	private bool canSeeHive
	{
		get
		{
			return this.currentHiveDistance <= this.maxHiveDistance;
		}
	}

	// Token: 0x06000027 RID: 39 RVA: 0x0000267F File Offset: 0x0000087F
	protected void Awake()
	{
		this.rb = base.GetComponent<Rigidbody>();
		this.lastSawBeehivePosition = base.transform.position;
	}

	// Token: 0x06000028 RID: 40 RVA: 0x0000269E File Offset: 0x0000089E
	public void SetBeehive(Beehive hive)
	{
		this.beehiveID = hive.instanceID;
		this.beehive = hive;
	}

	// Token: 0x06000029 RID: 41 RVA: 0x000026B4 File Offset: 0x000008B4
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(base.transform.position, this.defaultAggroDistance);
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(base.transform.position, this.hiveAggroDistance);
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(base.transform.position, this.deAggroDistance);
		Gizmos.color = Color.cyan;
		Gizmos.DrawWireSphere(base.transform.position, this.maxHiveDistance);
	}

	// Token: 0x0600002A RID: 42 RVA: 0x00002744 File Offset: 0x00000944
	private void Update()
	{
		if (this.dispersing)
		{
			return;
		}
		if (!PhotonNetwork.InRoom)
		{
			return;
		}
		if (base.photonView.IsMine)
		{
			bool flag = this.beehiveDangerTick > 0f;
			if (this.beesAngry != flag)
			{
				base.photonView.RPC("SetBeesAngryRPC", RpcTarget.AllBuffered, new object[] { flag });
			}
		}
		this.stingerField.statusAmountPerSecond = (this.beesAngry ? this.poisonOverTimeAngry : this.poisonOverTime);
		if (this.beehive == null)
		{
			this.TryGetBeehive();
		}
		this.UpdateAggro();
		if (!base.photonView.IsMine)
		{
			return;
		}
		if (this.beesAngry)
		{
			this.beehiveDangerTick = Mathf.Max(this.beehiveDangerTick - Time.deltaTime, 0f);
		}
	}

	// Token: 0x0600002B RID: 43 RVA: 0x00002813 File Offset: 0x00000A13
	[PunRPC]
	public void SetBeesAngryRPC(bool angry)
	{
		Debug.Log(string.Format("Setting bees angry: {0}", angry));
		this.beesAngry = angry;
	}

	// Token: 0x0600002C RID: 44 RVA: 0x00002831 File Offset: 0x00000A31
	private void FixedUpdate()
	{
		if (!PhotonNetwork.InRoom)
		{
			return;
		}
		this.UpdateBeehavior();
	}

	// Token: 0x0600002D RID: 45 RVA: 0x00002844 File Offset: 0x00000A44
	private void UpdateBeehavior()
	{
		if (!base.photonView.IsMine)
		{
			return;
		}
		this.currentHiveDistance = ((this.beehive == null) ? float.MaxValue : Vector3.Distance(this.beehive.transform.position, base.transform.position));
		this.currentLastSawHiveDistance = Vector3.Distance(this.lastSawBeehivePosition, base.transform.position);
		if (this.currentAggroCharacter == null)
		{
			this.rb.AddForce((this.lastSawBeehivePosition - base.transform.position).normalized * (this.movementForce * Time.fixedDeltaTime), ForceMode.Acceleration);
			this.UpdateDisperse();
			this.beeAngryLoop.volume = Mathf.Lerp(this.beeAngryLoop.volume, 0f, Time.deltaTime * 2f);
			this.beeIdleLoop.volume = Mathf.Lerp(this.beeIdleLoop.volume, 0.75f, Time.deltaTime * 2f);
			return;
		}
		float num = (this.beesAngry ? this.movementForceAngry : this.movementForce);
		this.rb.AddForce((this.currentAggroCharacter.Center - base.transform.position).normalized * (num * Time.fixedDeltaTime), ForceMode.Acceleration);
		this.beeAngryLoop.volume = Mathf.Lerp(this.beeAngryLoop.volume, 0.75f, Time.deltaTime * 2f);
		this.beeIdleLoop.volume = Mathf.Lerp(this.beeIdleLoop.volume, 0f, Time.deltaTime * 2f);
	}

	// Token: 0x0600002E RID: 46 RVA: 0x00002A08 File Offset: 0x00000C08
	private void UpdateDisperse()
	{
		if (this.currentAggroCharacter == null && !this.canSeeHive)
		{
			this.beeDispersalTick += Time.fixedDeltaTime;
			if (this.beeDispersalTick >= this.beesDispersalTime)
			{
				this.Disperse();
			}
			return;
		}
		this.beeDispersalTick = 0f;
	}

	// Token: 0x0600002F RID: 47 RVA: 0x00002A5D File Offset: 0x00000C5D
	private void GetAngry(float time)
	{
		this.beehiveDangerTick = time;
	}

	// Token: 0x06000030 RID: 48 RVA: 0x00002A66 File Offset: 0x00000C66
	public void HiveDestroyed(Vector3 atPosition)
	{
		if (Vector3.Distance(base.transform.position, atPosition) <= this.hiveAggroDistance)
		{
			this.hiveDestroyed = true;
			this.lastSawBeehivePosition = atPosition;
			this.GetAngry(this.beesAngerTimeHiveDestroyed);
		}
	}

	// Token: 0x06000031 RID: 49 RVA: 0x00002A9B File Offset: 0x00000C9B
	private void Disperse()
	{
		base.photonView.RPC("DisperseRPC", RpcTarget.All, Array.Empty<object>());
	}

	// Token: 0x06000032 RID: 50 RVA: 0x00002AB3 File Offset: 0x00000CB3
	[PunRPC]
	public void DisperseRPC()
	{
		this.dispersing = true;
		base.StartCoroutine(this.DisperseRoutine());
	}

	// Token: 0x06000033 RID: 51 RVA: 0x00002AC9 File Offset: 0x00000CC9
	private IEnumerator DisperseRoutine()
	{
		float tick = 0f;
		if (this.stingerField)
		{
			Object.Destroy(this.stingerField.gameObject);
		}
		while (tick < 1f)
		{
			ParticleSystem.EmissionModule emission = this.beeParticles.emission;
			emission.rateOverTimeMultiplier = Mathf.Max(emission.rateOverTimeMultiplier - Time.deltaTime, 0f);
			float num = Mathf.Max(this.beeForceField.gravity.constantMin - Time.deltaTime, 0f);
			float num2 = Mathf.Max(this.beeForceField.gravity.constantMax - Time.deltaTime, 0f);
			this.beeForceField.gravity = new ParticleSystem.MinMaxCurve(num, num2);
			tick += Time.deltaTime;
			yield return null;
		}
		while (tick < 4f)
		{
			tick += Time.deltaTime;
			yield return null;
		}
		if (base.photonView.IsMine)
		{
			PhotonNetwork.Destroy(base.gameObject);
		}
		yield break;
	}

	// Token: 0x06000034 RID: 52 RVA: 0x00002AD8 File Offset: 0x00000CD8
	private void UpdateAggro()
	{
		if (!base.photonView.IsMine)
		{
			return;
		}
		this.TryDeAggro();
		if (this.beehive != null && this.canSeeHive)
		{
			this.lastSawBeehivePosition = this.beehive.transform.position;
			if (this.beehive.item.holderCharacter != null)
			{
				this.beehiveDangerTick = this.beesAngerTimeHiveStolen;
				this.currentAggroCharacter = this.beehive.item.holderCharacter;
				return;
			}
		}
		if (this.currentAggroCharacter == null)
		{
			float num = float.MaxValue;
			Character character = null;
			if (this.beehive != null && this.currentLastSawHiveDistance > this.maxHiveDistance - this.hiveAggroDistance)
			{
				return;
			}
			float num2 = (this.beesAngry ? this.hiveAggroDistance : this.defaultAggroDistance);
			for (int i = 0; i < Character.AllCharacters.Count; i++)
			{
				Character character2 = Character.AllCharacters[i];
				float num3 = Vector3.Distance(character2.Center, base.transform.position);
				if (character2.data.fullyConscious && num3 < num2 && num3 < num)
				{
					num = num3;
					character = character2;
				}
			}
			this.currentAggroCharacter = character;
		}
	}

	// Token: 0x06000035 RID: 53 RVA: 0x00002C14 File Offset: 0x00000E14
	private void TryDeAggro()
	{
		if (this.currentAggroCharacter)
		{
			if (!this.currentAggroCharacter.data.fullyConscious)
			{
				this.currentAggroCharacter = null;
				return;
			}
			if (!this.hiveDestroyed && this.currentLastSawHiveDistance > this.maxHiveDistance)
			{
				this.currentAggroCharacter = null;
				return;
			}
			float num = Vector3.Distance(this.currentAggroCharacter.Center, base.transform.position);
			float num2 = (this.beesAngry ? this.hiveAggroDistance : this.deAggroDistance);
			if (num > num2)
			{
				this.currentAggroCharacter = null;
			}
		}
	}

	// Token: 0x06000036 RID: 54 RVA: 0x00002CA4 File Offset: 0x00000EA4
	private void TryGetBeehive()
	{
		Beehive beehive = Beehive.GetBeehive(this.beehiveID);
		if (beehive != null)
		{
			this.beehive = beehive;
			beehive.currentBees = this;
			Debug.Log(string.Format("Reattached to beehive #{0}", this.beehiveID));
		}
	}

	// Token: 0x0400000E RID: 14
	public int beehiveID;

	// Token: 0x0400000F RID: 15
	public Beehive beehive;

	// Token: 0x04000010 RID: 16
	[SerializeField]
	private float beehiveDangerTick;

	// Token: 0x04000011 RID: 17
	[SerializeField]
	private float beeDispersalTick;

	// Token: 0x04000012 RID: 18
	private float beehiveDangerTime;

	// Token: 0x04000013 RID: 19
	private Rigidbody rb;

	// Token: 0x04000014 RID: 20
	public Character currentAggroCharacter;

	// Token: 0x04000015 RID: 21
	public float defaultAggroDistance;

	// Token: 0x04000016 RID: 22
	public float hiveAggroDistance;

	// Token: 0x04000017 RID: 23
	public float deAggroDistance;

	// Token: 0x04000018 RID: 24
	public float maxHiveDistance;

	// Token: 0x04000019 RID: 25
	public float movementForce;

	// Token: 0x0400001A RID: 26
	public float movementForceAngry;

	// Token: 0x0400001B RID: 27
	public float beesAngerTimeHiveStolen = 8f;

	// Token: 0x0400001C RID: 28
	public float beesAngerTimeHiveDestroyed = 20f;

	// Token: 0x0400001D RID: 29
	public float beesDispersalTime = 6f;

	// Token: 0x0400001E RID: 30
	public float poisonOverTime;

	// Token: 0x0400001F RID: 31
	public float poisonOverTimeAngry;

	// Token: 0x04000020 RID: 32
	public StatusField stingerField;

	// Token: 0x04000021 RID: 33
	public ParticleSystem beeParticles;

	// Token: 0x04000022 RID: 34
	public ParticleSystemForceField beeForceField;

	// Token: 0x04000023 RID: 35
	private Vector3 lastSawBeehivePosition;

	// Token: 0x04000024 RID: 36
	private float currentHiveDistance;

	// Token: 0x04000025 RID: 37
	private float currentLastSawHiveDistance;

	// Token: 0x04000026 RID: 38
	public bool beesAngry;

	// Token: 0x04000027 RID: 39
	public AudioSource beeIdleLoop;

	// Token: 0x04000028 RID: 40
	public AudioSource beeAngryLoop;

	// Token: 0x04000029 RID: 41
	private bool hiveDestroyed;

	// Token: 0x0400002A RID: 42
	private bool dispersing;
}
