using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Photon.Pun;
using pworld.Scripts;
using pworld.Scripts.Extensions;
using UnityEngine;

// Token: 0x0200026E RID: 622
public class ShakyIcicleIce : MonoBehaviour
{
	// Token: 0x06000EF4 RID: 3828 RVA: 0x0004AFF4 File Offset: 0x000491F4
	private void Awake()
	{
		this.meshCollider = base.GetComponent<MeshCollider>();
		this.rig = base.GetComponent<Rigidbody>();
		this.view = base.GetComponent<PhotonView>();
		this.fractureRoot.gameObject.SetActive(false);
		this.rig.useGravity = false;
		this.rig.isKinematic = true;
	}

	// Token: 0x06000EF5 RID: 3829 RVA: 0x0004B04E File Offset: 0x0004924E
	private void Start()
	{
	}

	// Token: 0x06000EF6 RID: 3830 RVA: 0x0004B050 File Offset: 0x00049250
	private void Update()
	{
	}

	// Token: 0x06000EF7 RID: 3831 RVA: 0x0004B054 File Offset: 0x00049254
	private void SetIgnoreColliders()
	{
		this.ignoreColliders = new HashSet<Collider>();
		HashSet<Collider> hashSet = (from c in Physics.OverlapBox(this.meshCollider.bounds.center, this.meshCollider.bounds.extents, base.transform.rotation)
			where c != this.meshCollider
			select c).ToHashSet<Collider>();
		Vector3 vector = base.transform.TransformVector(this.innerCheck);
		Vector3 vector2 = base.transform.position + -base.transform.up * vector.y;
		Debug.Log(string.Format("Count: {0}", hashSet.Count));
		foreach (Collider collider in hashSet)
		{
			Vector3 vector3;
			float num;
			if (Physics.ComputePenetration(this.meshCollider, this.meshCollider.transform.position, this.meshCollider.transform.rotation, collider, collider.transform.position, collider.transform.rotation, out vector3, out num))
			{
				this.ignoreColliders.Add(collider);
			}
			else if ((from c in Physics.OverlapBox(vector2, vector, base.transform.rotation)
				where c != this.meshCollider
				select c).ToList<Collider>().Count > 0)
			{
				this.ignoreColliders.Add(collider);
			}
		}
	}

	// Token: 0x06000EF8 RID: 3832 RVA: 0x0004B1EC File Offset: 0x000493EC
	private bool CheckInTheClear()
	{
		HashSet<Collider> hashSet = (from c in Physics.OverlapBox(this.meshCollider.bounds.center, this.meshCollider.bounds.extents, base.transform.rotation)
			where c != this.meshCollider
			select c).ToHashSet<Collider>();
		if (hashSet.Count == 0 || !hashSet.Any((Collider c) => this.ignoreColliders.Contains(c)))
		{
			this.scaleOnChange = this.meshCollider.transform.lossyScale;
			this.positionOnChange = this.meshCollider.transform.position;
			this.rotationOnChange = this.meshCollider.transform.rotation;
			return true;
		}
		return false;
	}

	// Token: 0x06000EF9 RID: 3833 RVA: 0x0004B2A8 File Offset: 0x000494A8
	private void FixedUpdate()
	{
		if (!this.isFalling)
		{
			return;
		}
		if (this.isFractured)
		{
			return;
		}
		if (!this.once)
		{
			this.once = true;
		}
		if (!this.isInTheClear)
		{
			this.isInTheClear = this.CheckInTheClear();
			if (this.isInTheClear)
			{
				this.ignoreColliders.Clear();
				this.rig.excludeLayers = 0;
			}
		}
		Vector3 vector;
		Vector3 vector2;
		List<Collider> list;
		if (this.CheckBoundingBox(out vector, out vector2, out list))
		{
			this.isFractured = true;
			this.mesh.gameObject.SetActive(false);
			Object.Destroy(this.meshCollider);
			Object.Destroy(base.GetComponent<MeshRenderer>());
			this.fractureRoot.gameObject.SetActive(true);
			Object.Destroy(this.rig);
		}
	}

	// Token: 0x06000EFA RID: 3834 RVA: 0x0004B368 File Offset: 0x00049568
	private void OnCollisionEnter(Collision other)
	{
		if (this.isShaking || this.isFalling)
		{
			return;
		}
		Character componentInParent = other.gameObject.GetComponentInParent<Character>();
		if (!componentInParent)
		{
			return;
		}
		if (!componentInParent.IsLocal)
		{
			return;
		}
		Debug.Log("Before Shake rock");
		this.view.RPC("ShakeRock", RpcTarget.All, Array.Empty<object>());
	}

	// Token: 0x06000EFB RID: 3835 RVA: 0x0004B3C4 File Offset: 0x000495C4
	private void OnCollisionStay(Collision other)
	{
		if (!this.isShaking)
		{
			return;
		}
		Character componentInParent = other.gameObject.GetComponentInParent<Character>();
		if (!componentInParent)
		{
			return;
		}
		if (!componentInParent.IsLocal)
		{
			return;
		}
		this.tickTime += Time.deltaTime;
		if ((double)this.tickTime > 0.1)
		{
			this.tickTime = 0f;
			GamefeelHandler.instance.AddPerlinShake(this.shakeAmount, 0.2f, 15f);
		}
	}

	// Token: 0x06000EFC RID: 3836 RVA: 0x0004B444 File Offset: 0x00049644
	private bool CheckInnerBox(out Vector3 halfExtends, out Vector3 innerCheckPosition)
	{
		halfExtends = base.transform.TransformVector(this.innerCheck);
		innerCheckPosition = base.transform.position + -base.transform.up * halfExtends.y;
		return (from c in (from c in Physics.OverlapBox(innerCheckPosition, halfExtends, base.transform.rotation)
				where c != this.meshCollider
				select c).ToList<Collider>()
			where !this.ignoreColliders.Contains(c)
			select c).ToList<Collider>().Count > 0;
	}

	// Token: 0x06000EFD RID: 3837 RVA: 0x0004B4EC File Offset: 0x000496EC
	public bool CheckBoundingBox(out Vector3 halfExtends, out Vector3 position, out List<Collider> colliders)
	{
		halfExtends = this.meshCollider.bounds.extents;
		position = this.meshCollider.bounds.center;
		colliders = (from c in Physics.OverlapBox(position, halfExtends, base.transform.rotation)
			where c != this.meshCollider
			select c).ToList<Collider>();
		colliders = colliders.Where((Collider c) => !this.ignoreColliders.Contains(c)).ToList<Collider>();
		return colliders.Count > 0;
	}

	// Token: 0x06000EFE RID: 3838 RVA: 0x0004B584 File Offset: 0x00049784
	public bool ConvexMeshCollision(List<Collider> colliders)
	{
		foreach (Collider collider in colliders)
		{
			Vector3 vector;
			float num;
			if (Physics.ComputePenetration(this.meshCollider, this.meshCollider.transform.position, this.meshCollider.transform.rotation, collider, collider.transform.position, collider.transform.rotation, out vector, out num))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000EFF RID: 3839 RVA: 0x0004B61C File Offset: 0x0004981C
	private void OnDrawGizmosSelected()
	{
		if (!this.drawGizmos || this.isFractured)
		{
			return;
		}
		this.meshCollider = base.GetComponent<MeshCollider>();
		if (this.isInTheClear)
		{
			Gizmos.DrawWireMesh(this.meshCollider.sharedMesh, this.positionOnChange, this.rotationOnChange, this.scaleOnChange);
		}
		foreach (Collider collider in this.ignoreColliders)
		{
			Debug.DrawLine(base.transform.position, collider.bounds.center);
		}
		this.CheckInTheClear();
		Vector3 vector;
		Vector3 vector2;
		Gizmos.color = (this.CheckInnerBox(out vector, out vector2) ? Color.red : Color.green);
		Gizmos.DrawCube(vector2, vector * 2f);
		Vector3 vector3;
		Vector3 vector4;
		List<Collider> list;
		Gizmos.color = (this.CheckBoundingBox(out vector3, out vector4, out list) ? Color.red : Color.green);
		Gizmos.DrawWireCube(vector4, vector3 * 2f);
		Gizmos.color = (this.ConvexMeshCollision(list) ? Color.red : Color.green);
		Gizmos.DrawWireMesh(this.meshCollider.sharedMesh, this.meshCollider.transform.position, this.meshCollider.transform.rotation, this.meshCollider.transform.lossyScale);
	}

	// Token: 0x06000F00 RID: 3840 RVA: 0x0004B794 File Offset: 0x00049994
	[PunRPC]
	private void ShakeRock()
	{
		Debug.Log("start shake rock");
		this.isShaking = true;
		base.StartCoroutine(this.<ShakeRock>g__RockShake|36_0());
	}

	// Token: 0x06000F01 RID: 3841 RVA: 0x0004B7B4 File Offset: 0x000499B4
	private void Go()
	{
		this.isFalling = true;
		this.rig.useGravity = true;
		this.rig.isKinematic = false;
	}

	// Token: 0x06000F0B RID: 3851 RVA: 0x0004B8AE File Offset: 0x00049AAE
	[CompilerGenerated]
	private IEnumerator <ShakeRock>g__RockShake|36_0()
	{
		Debug.Log("Start shaking");
		float duration = 0f;
		Debug.Log(string.Format("duration: {0}, fallTime: {1}", duration, this.fallTime));
		while (duration < this.fallTime)
		{
			Debug.Log(string.Format("duration: {0}, fallTime: {1}", duration, this.fallTime));
			Vector3 vector = Vector2.zero;
			vector.x += Perlin.Noise(Time.time * this.shakeScale, 0f, 0f) - 0.5f;
			vector.y += Perlin.Noise(0f, Time.time * this.shakeScale, 0f) - 0.5f;
			vector.z += Perlin.Noise(0f, 0f, Time.time * this.shakeScale) - 0.5f;
			vector *= this.amount * Time.deltaTime;
			duration += Time.deltaTime;
			Debug.Log(string.Format("offset: {0}", vector));
			this.mesh.localPosition = vector;
			yield return null;
		}
		Debug.Log("Done shaking");
		this.isShaking = false;
		this.mesh.localPosition = 0.ToVec();
		this.isFalling = true;
		this.rig.useGravity = true;
		this.rig.isKinematic = false;
		yield break;
	}

	// Token: 0x04000DC8 RID: 3528
	public float fallTime = 5f;

	// Token: 0x04000DC9 RID: 3529
	public float amount = 1f;

	// Token: 0x04000DCA RID: 3530
	public float shakeScale = 15f;

	// Token: 0x04000DCB RID: 3531
	public Transform mesh;

	// Token: 0x04000DCC RID: 3532
	public float shakeAmount = 10f;

	// Token: 0x04000DCD RID: 3533
	public bool drawGizmos;

	// Token: 0x04000DCE RID: 3534
	public float pushOutForce = 10f;

	// Token: 0x04000DCF RID: 3535
	private bool isFalling;

	// Token: 0x04000DD0 RID: 3536
	private bool isInTheClear;

	// Token: 0x04000DD1 RID: 3537
	private bool isShaking;

	// Token: 0x04000DD2 RID: 3538
	private MeshCollider meshCollider;

	// Token: 0x04000DD3 RID: 3539
	private Transform model;

	// Token: 0x04000DD4 RID: 3540
	private bool once;

	// Token: 0x04000DD5 RID: 3541
	private Vector3 positionOnChange;

	// Token: 0x04000DD6 RID: 3542
	private Rigidbody rig;

	// Token: 0x04000DD7 RID: 3543
	private Quaternion rotationOnChange;

	// Token: 0x04000DD8 RID: 3544
	private Vector3 scaleOnChange;

	// Token: 0x04000DD9 RID: 3545
	private float tickTime;

	// Token: 0x04000DDA RID: 3546
	private Vector3 velocity = Vector3.zero;

	// Token: 0x04000DDB RID: 3547
	private PhotonView view;

	// Token: 0x04000DDC RID: 3548
	public Vector3 innerCheck;

	// Token: 0x04000DDD RID: 3549
	private HashSet<Collider> ignoreColliders = new HashSet<Collider>();

	// Token: 0x04000DDE RID: 3550
	public Transform fractureRoot;

	// Token: 0x04000DDF RID: 3551
	private bool isFractured;
}
