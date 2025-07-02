using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Photon.Pun;
using pworld.Scripts;
using pworld.Scripts.Extensions;
using UnityEngine;

// Token: 0x02000270 RID: 624
public class ShakyRock : MonoBehaviour
{
	// Token: 0x06000F1B RID: 3867 RVA: 0x0004C332 File Offset: 0x0004A532
	private void Awake()
	{
		this.meshCollider = base.GetComponent<MeshCollider>();
		this.rig = base.GetComponent<Rigidbody>();
		this.view = base.GetComponent<PhotonView>();
		this.rig.useGravity = false;
		this.rig.isKinematic = true;
	}

	// Token: 0x06000F1C RID: 3868 RVA: 0x0004C370 File Offset: 0x0004A570
	private void Start()
	{
	}

	// Token: 0x06000F1D RID: 3869 RVA: 0x0004C372 File Offset: 0x0004A572
	private void Update()
	{
	}

	// Token: 0x06000F1E RID: 3870 RVA: 0x0004C374 File Offset: 0x0004A574
	private void FixedUpdate()
	{
		if (this.isFinished)
		{
			return;
		}
		if (!this.isFalling)
		{
			return;
		}
		if (!this.once)
		{
			this.rig.AddForce(Vector3.back * this.pushOutForce, ForceMode.VelocityChange);
			this.once = true;
		}
		if ((from c in Physics.OverlapSphere(this.meshCollider.bounds.center, this.meshCollider.bounds.extents.magnitude / 2f)
			where c != this.meshCollider
			select c).ToList<Collider>().Count > 0)
		{
			return;
		}
		List<Collider> list = (from c in Physics.OverlapBox(this.meshCollider.bounds.center, this.meshCollider.bounds.extents, base.transform.rotation)
			where c != this.meshCollider
			select c).ToList<Collider>();
		Debug.Log(string.Format("Count: {0}", list.Count));
		foreach (Collider collider in list)
		{
			Vector3 vector;
			float num;
			if (Physics.ComputePenetration(this.meshCollider, this.meshCollider.transform.position, this.meshCollider.transform.rotation, collider, collider.transform.position, collider.transform.rotation, out vector, out num))
			{
				Debug.Log("colliding with " + collider.name);
				return;
			}
			Debug.Log("Not colliding with " + collider.name);
		}
		this.scaleOnChange = this.meshCollider.transform.lossyScale;
		this.positionOnChange = this.meshCollider.transform.position;
		this.rotationOnChange = this.meshCollider.transform.rotation;
		this.isFinished = true;
		this.rig.excludeLayers = 0;
	}

	// Token: 0x06000F1F RID: 3871 RVA: 0x0004C594 File Offset: 0x0004A794
	private void OnCollisionEnter(Collision other)
	{
		if (this.isShaking || this.isFalling || this.isFinished)
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

	// Token: 0x06000F20 RID: 3872 RVA: 0x0004C5F8 File Offset: 0x0004A7F8
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

	// Token: 0x06000F21 RID: 3873 RVA: 0x0004C678 File Offset: 0x0004A878
	private void OnDrawGizmosSelected()
	{
		if (!this.drawGizmos)
		{
			return;
		}
		if (this.isFinished)
		{
			Gizmos.DrawWireMesh(this.meshCollider.sharedMesh, this.positionOnChange, this.rotationOnChange, this.scaleOnChange);
		}
		this.meshCollider = base.GetComponent<MeshCollider>();
		List<Collider> list = (from c in Physics.OverlapSphere(this.meshCollider.bounds.center, this.meshCollider.bounds.extents.magnitude / 2f)
			where c != this.meshCollider
			select c).ToList<Collider>();
		Gizmos.color = ((list.Count > 0) ? Color.red : Color.green);
		Gizmos.DrawWireSphere(this.meshCollider.bounds.center, this.meshCollider.bounds.extents.magnitude / 2f);
		if (list.Count > 0)
		{
			return;
		}
		List<Collider> list2 = (from c in Physics.OverlapBox(this.meshCollider.bounds.center, this.meshCollider.bounds.extents, base.transform.rotation)
			where c != this.meshCollider
			select c).ToList<Collider>();
		Gizmos.color = ((list2.Count > 0) ? Color.red : Color.green);
		Gizmos.DrawWireCube(this.meshCollider.bounds.center, this.meshCollider.bounds.size);
		foreach (Collider collider in list2)
		{
			Vector3 vector;
			float num;
			if (Physics.ComputePenetration(this.meshCollider, this.meshCollider.transform.position, this.meshCollider.transform.rotation, collider, collider.transform.position, collider.transform.rotation, out vector, out num))
			{
				Gizmos.color = Color.red;
				Gizmos.DrawWireMesh(this.meshCollider.sharedMesh, this.meshCollider.transform.position, this.meshCollider.transform.rotation, this.meshCollider.transform.lossyScale);
				return;
			}
			Debug.Log("Not colliding with " + collider.name);
		}
		Gizmos.color = Color.green;
		Gizmos.DrawWireMesh(this.meshCollider.sharedMesh, this.meshCollider.transform.position, this.meshCollider.transform.rotation, this.meshCollider.transform.lossyScale);
	}

	// Token: 0x06000F22 RID: 3874 RVA: 0x0004C934 File Offset: 0x0004AB34
	private void Go2()
	{
		GamefeelHandler.instance.AddPerlinShake(this.shakeAmount, 0.2f, 15f);
	}

	// Token: 0x06000F23 RID: 3875 RVA: 0x0004C950 File Offset: 0x0004AB50
	[PunRPC]
	private void ShakeRock()
	{
		Debug.Log("start shake rock");
		this.isShaking = true;
		base.StartCoroutine(this.<ShakeRock>g__RockShake|28_0());
	}

	// Token: 0x06000F24 RID: 3876 RVA: 0x0004C970 File Offset: 0x0004AB70
	private void Go()
	{
		this.isFalling = true;
		this.rig.useGravity = true;
		this.rig.isKinematic = false;
	}

	// Token: 0x06000F2A RID: 3882 RVA: 0x0004CA21 File Offset: 0x0004AC21
	[CompilerGenerated]
	private IEnumerator <ShakeRock>g__RockShake|28_0()
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
		this.rig.AddForce(Vector3.back * this.pushOutForce, ForceMode.VelocityChange);
		yield break;
	}

	// Token: 0x04000DFF RID: 3583
	public float fallTime = 5f;

	// Token: 0x04000E00 RID: 3584
	public float amount = 1f;

	// Token: 0x04000E01 RID: 3585
	public float shakeScale = 15f;

	// Token: 0x04000E02 RID: 3586
	public Transform mesh;

	// Token: 0x04000E03 RID: 3587
	public float shakeAmount = 10f;

	// Token: 0x04000E04 RID: 3588
	public bool drawGizmos;

	// Token: 0x04000E05 RID: 3589
	public float pushOutForce = 10f;

	// Token: 0x04000E06 RID: 3590
	private bool isFalling;

	// Token: 0x04000E07 RID: 3591
	private bool isFinished;

	// Token: 0x04000E08 RID: 3592
	private bool isShaking;

	// Token: 0x04000E09 RID: 3593
	private MeshCollider meshCollider;

	// Token: 0x04000E0A RID: 3594
	private Transform model;

	// Token: 0x04000E0B RID: 3595
	private bool once;

	// Token: 0x04000E0C RID: 3596
	private Vector3 positionOnChange;

	// Token: 0x04000E0D RID: 3597
	private Rigidbody rig;

	// Token: 0x04000E0E RID: 3598
	private Quaternion rotationOnChange;

	// Token: 0x04000E0F RID: 3599
	private Vector3 scaleOnChange;

	// Token: 0x04000E10 RID: 3600
	private float tickTime;

	// Token: 0x04000E11 RID: 3601
	private Vector3 velocity = Vector3.zero;

	// Token: 0x04000E12 RID: 3602
	private PhotonView view;
}
