using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Photon.Pun;
using pworld.Scripts;
using pworld.Scripts.Extensions;
using Sirenix.Utilities;
using UnityEngine;

// Token: 0x0200026F RID: 623
public class ShakyIcicleIce2 : MonoBehaviour
{
	// Token: 0x170000BD RID: 189
	// (get) Token: 0x06000F0C RID: 3852 RVA: 0x0004B8BD File Offset: 0x00049ABD
	private bool IsLocalPlayerClimbing
	{
		get
		{
			return Character.localCharacter.data.isClimbing && Character.localCharacter.data.climbHit.collider == this.meshCollider;
		}
	}

	// Token: 0x170000BE RID: 190
	// (get) Token: 0x06000F0D RID: 3853 RVA: 0x0004B8F1 File Offset: 0x00049AF1
	private float DistanceToLocalPlayer
	{
		get
		{
			return Vector3.Distance(Character.localCharacter.Center, base.transform.position);
		}
	}

	// Token: 0x06000F0E RID: 3854 RVA: 0x0004B910 File Offset: 0x00049B10
	private void Awake()
	{
		this.source = base.GetComponent<AudioSource>();
		this.photonView = base.GetComponent<PhotonView>();
		this.meshCollider = base.GetComponent<MeshCollider>();
		this.startPeicesCount = this.fracturedRoot.transform.childCount;
		this.fracturedRoot.gameObject.SetActive(false);
		this.source.volume = 0f;
		this.source.Stop();
		if (Random.Range(0f, 1f) > this.fallChance)
		{
			base.enabled = false;
		}
	}

	// Token: 0x06000F0F RID: 3855 RVA: 0x0004B9A4 File Offset: 0x00049BA4
	private void Start()
	{
		this.fracturedRoot.gameObject.SetActive(true);
		this.stuckies = this.GetStuckPieces();
		this.fracturedRoot.gameObject.SetActive(false);
		this.fullMesh.gameObject.SetActive(true);
		if (this.fallOnStart)
		{
			this.Fall_Rpc();
		}
	}

	// Token: 0x06000F10 RID: 3856 RVA: 0x0004BA00 File Offset: 0x00049C00
	private void Update()
	{
		if (!this.photonView.IsMine)
		{
			return;
		}
		if (!this.isShaking && !this.isFalling)
		{
			if ((from p in (from p in PlayerHandler.GetAllPlayerCharacters()
					where p.data.isClimbing
					select p).ToList<Character>()
				where p.data.climbHit.collider == this.meshCollider
				select p).ToList<Character>().Count > 0)
			{
				this.photonView.RPC("ShakeRock_Rpc", RpcTarget.All, Array.Empty<object>());
			}
		}
		this.timeUntilShake -= Time.deltaTime;
		if (this.isShaking && this.IsLocalPlayerClimbing && this.timeUntilShake <= 0f)
		{
			GamefeelHandler.instance.AddPerlinShake(this.climbingScreenShake, 0.2f, 15f);
			Debug.Log("Clime shake");
			this.timeUntilShake = this.screenShakeTickTime;
		}
	}

	// Token: 0x06000F11 RID: 3857 RVA: 0x0004BAEC File Offset: 0x00049CEC
	private void FixedUpdate()
	{
		if (this.rig == null)
		{
			return;
		}
		this.lastLinearVelocity = this.rig.linearVelocity;
		this.lastAngularVelocity = this.rig.angularVelocity;
	}

	// Token: 0x06000F12 RID: 3858 RVA: 0x0004BB1F File Offset: 0x00049D1F
	public void OnDestroy()
	{
		Object.DestroyImmediate(this.stuckiesRoot);
		Object.DestroyImmediate(this.shardsRoot);
	}

	// Token: 0x06000F13 RID: 3859 RVA: 0x0004BB38 File Offset: 0x00049D38
	private void OnCollisionEnter(Collision other)
	{
		if (!this.isFalling)
		{
			return;
		}
		if ((float)this.fracturedRoot.transform.childCount < (float)this.startPeicesCount * this.maxFracturePercent)
		{
			return;
		}
		bool flag = false;
		HashSet<Collider> hashSet = new HashSet<Collider>();
		foreach (ContactPoint contactPoint in other.contacts)
		{
			Collider[] array = Physics.OverlapSphere(contactPoint.point, this.contactExplosionRadius);
			hashSet.AddRange(array);
		}
		foreach (Collider collider in hashSet)
		{
			if (collider.transform.parent != this.fracturedRoot)
			{
				if (this.shards.Contains(collider.gameObject))
				{
					this.rig.linearVelocity = this.lastLinearVelocity * this.collisionDamp;
					this.rig.angularVelocity = this.lastAngularVelocity;
				}
			}
			else
			{
				flag = true;
				if (this.shardsRoot == null)
				{
					this.shardsRoot = new GameObject("ShardsRoot");
					this.shardsRoot.transform.position = collider.transform.position;
				}
				collider.gameObject.AddComponent<Rigidbody>().mass = this.fracturedMass;
				collider.transform.parent = this.shardsRoot.transform;
				this.shards.Add(collider.gameObject);
			}
		}
		if (flag)
		{
			this.rig.linearVelocity = this.lastLinearVelocity * this.collisionDamp;
			this.rig.angularVelocity = this.lastAngularVelocity;
		}
	}

	// Token: 0x06000F14 RID: 3860 RVA: 0x0004BD00 File Offset: 0x00049F00
	private void OnDrawGizmosSelected()
	{
		if (!this.drawGizmos)
		{
			return;
		}
		this.meshCollider = base.GetComponent<MeshCollider>();
		this.rig = base.GetComponent<Rigidbody>();
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(base.transform.position, this.startShakeDistance);
		if (this.isFalling)
		{
			return;
		}
		foreach (Collider collider in this.GetStuckPieces())
		{
			Gizmos.color = Color.red;
			Gizmos.DrawWireMesh(collider.GetComponent<MeshCollider>().sharedMesh, collider.transform.position, collider.transform.rotation);
		}
	}

	// Token: 0x06000F15 RID: 3861 RVA: 0x0004BDC8 File Offset: 0x00049FC8
	[PunRPC]
	private void ShakeRock_Rpc()
	{
		Debug.Log("start shake rock");
		this.isShaking = true;
		this.source.Play();
		this.source.volume = 0.7f;
		if (this.DistanceToLocalPlayer < this.startShakeDistance)
		{
			Debug.Log(string.Format("start shake {0}", this.startShakeAmount));
			GamefeelHandler.instance.AddPerlinShake(this.startShakeAmount, 0.2f, 15f);
		}
		base.StartCoroutine(this.<ShakeRock_Rpc>g__RockShake|42_0());
	}

	// Token: 0x06000F16 RID: 3862 RVA: 0x0004BE50 File Offset: 0x0004A050
	[PunRPC]
	private void Fall_Rpc()
	{
		if (Character.localCharacter.data.isClimbing && Character.localCharacter.data.climbHit.collider == this.meshCollider)
		{
			Character.localCharacter.refs.climbing.StopClimbing();
		}
		this.popSound.Play(base.transform.position);
		if (this.DistanceToLocalPlayer < this.startShakeDistance)
		{
			Debug.Log(string.Format("fall shake {0}", this.startShakeAmount));
			GamefeelHandler.instance.AddPerlinShake(this.startShakeAmount, 0.2f, 15f);
		}
		this.fracturedRoot.gameObject.SetActive(true);
		this.fullMesh.gameObject.SetActive(false);
		this.rig = base.gameObject.AddComponent<Rigidbody>();
		this.rig.mass = 1000f;
		this.rig.useGravity = true;
		this.rig.isKinematic = false;
		this.meshCollider.enabled = false;
		Object.DestroyImmediate(this.meshCollider);
		foreach (Collider collider in this.stuckies)
		{
			if (this.stuckiesRoot == null)
			{
				this.stuckiesRoot = new GameObject("StuckiesRoot");
				this.stuckiesRoot.transform.position = collider.transform.position;
			}
			collider.transform.parent = this.stuckiesRoot.transform;
			collider.enabled = true;
		}
		this.startPeicesCount = this.fracturedRoot.transform.childCount;
		Debug.Log("Falling");
		this.isFalling = true;
	}

	// Token: 0x06000F17 RID: 3863 RVA: 0x0004C02C File Offset: 0x0004A22C
	private List<Collider> GetStuckPieces()
	{
		List<MeshCollider> piecsColliders = this.fracturedRoot.GetComponentsInChildren<MeshCollider>().ToList<MeshCollider>();
		List<Collider> list = (from c in (from c in (from c in Physics.OverlapBox(this.meshCollider.bounds.center, this.meshCollider.bounds.extents, base.transform.rotation)
					where c != this.meshCollider
					select c).ToList<Collider>()
				where c.gameObject.IsInLayer(HelperFunctions.LayerType.TerrainMap.ToLayerMask())
				select c).ToList<Collider>()
			where !piecsColliders.Contains(c)
			select c).ToList<Collider>();
		HashSet<Collider> hashSet = new HashSet<Collider>();
		foreach (Collider collider in list)
		{
			foreach (MeshCollider meshCollider in piecsColliders)
			{
				Vector3 vector;
				float num;
				if (Physics.ComputePenetration(meshCollider, meshCollider.transform.position, meshCollider.transform.rotation, collider, collider.transform.position, collider.transform.rotation, out vector, out num))
				{
					hashSet.Add(meshCollider);
				}
			}
		}
		HashSet<Collider> hashSet2 = new HashSet<Collider>();
		foreach (MeshCollider meshCollider2 in piecsColliders)
		{
			using (HashSet<Collider>.Enumerator enumerator3 = hashSet.GetEnumerator())
			{
				while (enumerator3.MoveNext())
				{
					if (enumerator3.Current.transform.position.y < meshCollider2.transform.position.y)
					{
						hashSet2.Add(meshCollider2);
					}
				}
			}
		}
		hashSet.AddRange(hashSet2);
		return hashSet.ToList<Collider>();
	}

	// Token: 0x06000F1A RID: 3866 RVA: 0x0004C323 File Offset: 0x0004A523
	[CompilerGenerated]
	private IEnumerator <ShakeRock_Rpc>g__RockShake|42_0()
	{
		Debug.Log("Start shaking");
		float duration = 0f;
		while (duration < this.fallTime)
		{
			Vector3 vector = Vector2.zero;
			vector.x += Perlin.Noise(Time.time * this.shakeScale, 0f, 0f) - 0.5f;
			vector.y += Perlin.Noise(0f, Time.time * this.shakeScale, 0f) - 0.5f;
			vector.z += Perlin.Noise(0f, 0f, Time.time * this.shakeScale) - 0.5f;
			vector *= this.amount * Time.deltaTime;
			duration += Time.deltaTime;
			this.fullMesh.localPosition = vector;
			yield return null;
		}
		Debug.Log("Done shaking");
		this.isShaking = false;
		this.fullMesh.localPosition = 0.ToVec();
		this.source.volume = 0f;
		this.source.Stop();
		if (this.photonView.IsMine)
		{
			this.photonView.RPC("Fall_Rpc", RpcTarget.All, Array.Empty<object>());
		}
		yield break;
	}

	// Token: 0x04000DE0 RID: 3552
	public float fallChance = 0.5f;

	// Token: 0x04000DE1 RID: 3553
	public float contactExplosionRadius = 0.2f;

	// Token: 0x04000DE2 RID: 3554
	public float maxFracturePercent = 0.5f;

	// Token: 0x04000DE3 RID: 3555
	public float fracturedMass = 1f;

	// Token: 0x04000DE4 RID: 3556
	public float collisionDamp;

	// Token: 0x04000DE5 RID: 3557
	public float shakeScale = 30f;

	// Token: 0x04000DE6 RID: 3558
	public float fallTime = 5f;

	// Token: 0x04000DE7 RID: 3559
	public float amount = 1f;

	// Token: 0x04000DE8 RID: 3560
	public float startShakeDistance = 10f;

	// Token: 0x04000DE9 RID: 3561
	public float startShakeAmount = 400f;

	// Token: 0x04000DEA RID: 3562
	public float climbingScreenShake = 240f;

	// Token: 0x04000DEB RID: 3563
	public float screenShakeTickTime = 0.2f;

	// Token: 0x04000DEC RID: 3564
	public bool isFalling;

	// Token: 0x04000DED RID: 3565
	public bool isShaking;

	// Token: 0x04000DEE RID: 3566
	public bool fallOnStart;

	// Token: 0x04000DEF RID: 3567
	public Transform fullMesh;

	// Token: 0x04000DF0 RID: 3568
	public Transform fracturedRoot;

	// Token: 0x04000DF1 RID: 3569
	public SFX_Instance popSound;

	// Token: 0x04000DF2 RID: 3570
	private readonly List<GameObject> shards = new List<GameObject>();

	// Token: 0x04000DF3 RID: 3571
	private Vector3 lastAngularVelocity;

	// Token: 0x04000DF4 RID: 3572
	private Vector3 lastLinearVelocity;

	// Token: 0x04000DF5 RID: 3573
	private MeshCollider meshCollider;

	// Token: 0x04000DF6 RID: 3574
	private PhotonView photonView;

	// Token: 0x04000DF7 RID: 3575
	private Rigidbody rig;

	// Token: 0x04000DF8 RID: 3576
	private GameObject shardsRoot;

	// Token: 0x04000DF9 RID: 3577
	private AudioSource source;

	// Token: 0x04000DFA RID: 3578
	private int startPeicesCount;

	// Token: 0x04000DFB RID: 3579
	private List<Collider> stuckies = new List<Collider>();

	// Token: 0x04000DFC RID: 3580
	private GameObject stuckiesRoot;

	// Token: 0x04000DFD RID: 3581
	private float timeUntilShake;

	// Token: 0x04000DFE RID: 3582
	public bool drawGizmos;
}
