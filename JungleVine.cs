using System;
using Photon.Pun;
using pworld.Scripts.Extensions;
using UnityEngine;
using Zorro.Core;

// Token: 0x020001E2 RID: 482
public class JungleVine : CustomSpawnCondition, IInteractible
{
	// Token: 0x06000CA8 RID: 3240 RVA: 0x0003F270 File Offset: 0x0003D470
	private void Awake()
	{
		this.totalLength = 0f;
		this.photonView = base.GetComponent<PhotonView>();
		if (this.colliderRoot == null)
		{
			this.colliderRoot = new GameObject("ColliderRoot").transform;
			this.colliderRoot.parent = base.transform;
			this.colliderRoot.localPosition = Vector3.zero;
			this.colliderRoot.localRotation = Quaternion.identity;
		}
	}

	// Token: 0x06000CA9 RID: 3241 RVA: 0x0003F2E8 File Offset: 0x0003D4E8
	private void Start()
	{
		this.SetRendererBounds();
	}

	// Token: 0x06000CAA RID: 3242 RVA: 0x0003F2F0 File Offset: 0x0003D4F0
	public void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawSphere(base.transform.position + base.transform.forward * this.maxDist, 0.5f);
		MeshRenderer componentInChildren = base.GetComponentInChildren<MeshRenderer>();
		Gizmos.DrawWireCube(componentInChildren.bounds.center, componentInChildren.bounds.size);
	}

	// Token: 0x06000CAB RID: 3243 RVA: 0x0003F35F File Offset: 0x0003D55F
	public bool IsInteractible(Character interactor)
	{
		return this.colliderType == JungleVine.ColliderType.Capsule;
	}

	// Token: 0x06000CAC RID: 3244 RVA: 0x0003F36C File Offset: 0x0003D56C
	public void Interact(Character interactor)
	{
		interactor.refs.items.EquipSlot(Optionable<byte>.None);
		int closestChild = this.GetClosestChild(interactor.Center);
		Debug.Log(string.Format("Grabbing Vine with index: {0}", closestChild));
		interactor.GetComponent<PhotonView>().RPC("GrabVineRpc", RpcTarget.All, new object[]
		{
			base.GetComponent<PhotonView>(),
			closestChild
		});
	}

	// Token: 0x06000CAD RID: 3245 RVA: 0x0003F3D9 File Offset: 0x0003D5D9
	public void HoverEnter()
	{
	}

	// Token: 0x06000CAE RID: 3246 RVA: 0x0003F3DB File Offset: 0x0003D5DB
	public void HoverExit()
	{
	}

	// Token: 0x06000CAF RID: 3247 RVA: 0x0003F3DD File Offset: 0x0003D5DD
	public Vector3 Center()
	{
		return base.transform.position;
	}

	// Token: 0x06000CB0 RID: 3248 RVA: 0x0003F3EA File Offset: 0x0003D5EA
	public Transform GetTransform()
	{
		return base.transform;
	}

	// Token: 0x06000CB1 RID: 3249 RVA: 0x0003F3F2 File Offset: 0x0003D5F2
	public string GetInteractionText()
	{
		return "Grab";
	}

	// Token: 0x06000CB2 RID: 3250 RVA: 0x0003F3F9 File Offset: 0x0003D5F9
	public string GetName()
	{
		return this.displayName;
	}

	// Token: 0x06000CB3 RID: 3251 RVA: 0x0003F404 File Offset: 0x0003D604
	public override bool CheckCondition(PropSpawner.SpawnData data)
	{
		Vector3 vector = data.normal;
		vector.y *= this.normalYMult;
		vector = vector.normalized;
		RaycastHit raycastHit = HelperFunctions.LineCheck(base.transform.position, base.transform.position + vector * this.maxDist, HelperFunctions.LayerType.TerrainMap, 0f, QueryTriggerInteraction.Ignore);
		if (!raycastHit.transform)
		{
			return false;
		}
		if (raycastHit.distance < this.minDist)
		{
			return false;
		}
		if (Mathf.Abs(raycastHit.point.y - base.transform.position.y) > this.maxHeightDifference)
		{
			return false;
		}
		bool flag = this.ConfigVine(raycastHit.point);
		BreakableBridge breakableBridge;
		if (flag && base.TryGetComponent<BreakableBridge>(out breakableBridge))
		{
			breakableBridge.AddCollisionModifiers();
		}
		return flag;
	}

	// Token: 0x06000CB4 RID: 3252 RVA: 0x0003F4D4 File Offset: 0x0003D6D4
	public static bool CheckVinePath(Vector3 from, Vector3 to, float hang, out Vector3 mid)
	{
		mid = Vector3.Lerp(from, to, 0.5f);
		mid.y += hang;
		for (int i = 0; i < 50; i++)
		{
			float num = (float)i / 49f;
			Vector3 vector = BezierCurve.QuadraticBezier(from, mid, to, num);
			if (i < 49 && HelperFunctions.LineCheck(from, vector, HelperFunctions.LayerType.TerrainMap, 0f, QueryTriggerInteraction.Ignore).transform)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06000CB5 RID: 3253 RVA: 0x0003F54B File Offset: 0x0003D74B
	[PunRPC]
	public void ForceBuildVine_RPC(Vector3 from, Vector3 to, float hang, Vector3 mid)
	{
		this.ForceBuildVine(from, to, hang, mid);
	}

	// Token: 0x06000CB6 RID: 3254 RVA: 0x0003F558 File Offset: 0x0003D758
	public void ForceBuildVine(Vector3 from, Vector3 to, float hang, Vector3 mid)
	{
		if (this.colliderRoot == null)
		{
			Debug.Log("colliderRoot was null, creating new one for " + base.gameObject.name);
			this.colliderRoot = new GameObject("ColliderRoot").transform;
			this.colliderRoot.parent = base.transform;
			this.colliderRoot.localPosition = Vector3.zero;
			this.colliderRoot.localRotation = Quaternion.identity;
		}
		this.colliderRoot.KillAllChildren(true, false, false);
		float num = Vector3.Distance(from, to) / this.meshLength;
		Renderer componentInChildren = base.GetComponentInChildren<Renderer>();
		componentInChildren.material.SetFloat("_Hang", hang);
		if (this.hangCenter != null)
		{
			this.hangCenter.position = BezierCurve.QuadraticBezier(from, mid, to, 0.5f);
		}
		Vector3 vector = from;
		for (int i = 0; i < 50; i++)
		{
			float num2 = (float)i / 49f;
			Vector3 vector2 = BezierCurve.QuadraticBezier(from, mid, to, num2);
			GameObject gameObject = new GameObject("Collider");
			gameObject.transform.parent = this.colliderRoot;
			if (this.colliderType == JungleVine.ColliderType.Capsule)
			{
				CapsuleCollider capsuleCollider = gameObject.AddComponent<CapsuleCollider>();
				capsuleCollider.radius = 0.25f;
				capsuleCollider.height = Vector3.Distance(vector, vector2) + 0.5f;
				capsuleCollider.isTrigger = true;
			}
			else
			{
				gameObject.AddComponent<BoxCollider>().size = new Vector3(this.boxShape.x * num, Vector3.Distance(vector, vector2) + 0.5f, this.boxShape.y);
			}
			gameObject.transform.rotation = HelperFunctions.GetRotationWithUp(Vector3.down, vector2 - vector);
			gameObject.transform.position = vector2 - Vector3.down * this.colliderOffset;
			gameObject.gameObject.layer = 21;
			vector = vector2;
		}
		Transform transform = base.transform.Find("Mesh");
		transform.transform.rotation = Quaternion.LookRotation(to - from);
		transform.transform.localScale = Vector3.one * num;
		if (num < 0.5f)
		{
			componentInChildren.material.SetFloat("_LengthScale", num * 2f);
		}
		Debug.Log("Vine built, calling onFinish");
	}

	// Token: 0x06000CB7 RID: 3255 RVA: 0x0003F7A0 File Offset: 0x0003D9A0
	public bool ConfigVine(Vector3 to)
	{
		base.GetComponentsInChildren<Collider>().KillAllGameObjects(true);
		float num = Random.Range(-this.maxDown, -this.minDown);
		Vector3 position = base.transform.position;
		Vector3 vector;
		if (!JungleVine.CheckVinePath(position, to, num, out vector))
		{
			return false;
		}
		this.ForceBuildVine(position, to, num, vector);
		return true;
	}

	// Token: 0x06000CB8 RID: 3256 RVA: 0x0003F7F2 File Offset: 0x0003D9F2
	public void ConnectDebug()
	{
		this.ConfigVine(this.connectTo.transform.position);
	}

	// Token: 0x06000CB9 RID: 3257 RVA: 0x0003F80C File Offset: 0x0003DA0C
	public void SetRendererBounds()
	{
		Vector3 vector = base.transform.position;
		Vector3 vector2 = base.transform.position;
		Debug.Log(string.Format("ColliderRoot, isNull:{0}, child count:", this.colliderRoot == null));
		if (this.colliderRoot != null)
		{
			for (int i = 0; i < Mathf.Min(this.colliderRoot.transform.childCount, this.segments); i++)
			{
				Vector3 position = this.colliderRoot.transform.GetChild(i).transform.position;
				this.totalLength += Vector3.Distance(vector, position);
				vector = position;
				if (position.y < vector2.y)
				{
					vector2 = position;
				}
			}
		}
		Renderer componentInChildren = base.GetComponentInChildren<Renderer>();
		Bounds localBounds = componentInChildren.localBounds;
		localBounds.Encapsulate(componentInChildren.transform.InverseTransformPoint(vector2));
		componentInChildren.localBounds = localBounds;
	}

	// Token: 0x06000CBA RID: 3258 RVA: 0x0003F8F9 File Offset: 0x0003DAF9
	public float LengthFactor()
	{
		return 1f / this.totalLength;
	}

	// Token: 0x06000CBB RID: 3259 RVA: 0x0003F907 File Offset: 0x0003DB07
	public float GetPercentFromSegmentIndex(int segmentIndex)
	{
		return (float)segmentIndex / ((float)this.segments - 1f);
	}

	// Token: 0x06000CBC RID: 3260 RVA: 0x0003F919 File Offset: 0x0003DB19
	public int GetIndexFromPercentage(float percent)
	{
		return Mathf.RoundToInt(Mathf.Lerp(0f, (float)(this.segments - 1), percent));
	}

	// Token: 0x06000CBD RID: 3261 RVA: 0x0003F934 File Offset: 0x0003DB34
	internal Vector3 GetDir(Vector3 lookDirection_Flat, float percent)
	{
		int indexFromPercentage = this.GetIndexFromPercentage(percent);
		Vector3 vector = this.colliderRoot.transform.GetChild(indexFromPercentage).up;
		if (Vector3.Angle(lookDirection_Flat, vector) > 90f)
		{
			vector *= -1f;
		}
		return vector;
	}

	// Token: 0x06000CBE RID: 3262 RVA: 0x0003F97C File Offset: 0x0003DB7C
	public float GetVineVel(Vector3 vel, float percent)
	{
		Vector3 up = this.colliderRoot.transform.GetChild(this.GetIndexFromPercentage(percent)).up;
		Vector3 dir = this.GetDir(vel, percent);
		float num = 1f;
		if (Vector3.Angle(vel, up) > 90f)
		{
			num = -1f;
		}
		vel = Vector3.Project(vel, up);
		return num * vel.magnitude * Mathf.InverseLerp(0f, -0.5f, dir.y);
	}

	// Token: 0x06000CBF RID: 3263 RVA: 0x0003F9F4 File Offset: 0x0003DBF4
	public float GetSign(Vector3 dir, float percent)
	{
		Vector3 up = this.colliderRoot.transform.GetChild(this.GetIndexFromPercentage(percent)).up;
		float num = 1f;
		if (Vector3.Angle(dir, up) > 90f)
		{
			num = -1f;
		}
		return num;
	}

	// Token: 0x06000CC0 RID: 3264 RVA: 0x0003FA3C File Offset: 0x0003DC3C
	public Vector3 GetUp(float percent)
	{
		Vector3 vector = this.colliderRoot.transform.GetChild(this.GetIndexFromPercentage(percent)).up;
		if (Vector3.Angle(Vector3.up, vector) > 90f)
		{
			vector *= -1f;
		}
		return vector;
	}

	// Token: 0x06000CC1 RID: 3265 RVA: 0x0003FA88 File Offset: 0x0003DC88
	public float UpMult(float percent)
	{
		Vector3 up = this.colliderRoot.transform.GetChild(this.GetIndexFromPercentage(percent)).up;
		return (float)((Vector3.Angle(Vector3.up, up) < 90f) ? 1 : (-1));
	}

	// Token: 0x06000CC2 RID: 3266 RVA: 0x0003FACC File Offset: 0x0003DCCC
	public Vector3 GetPosition(float percent)
	{
		percent = Mathf.Clamp01(percent);
		float num = percent * (float)(this.segments - 1);
		int num2 = Mathf.FloorToInt(num);
		int num3 = num2;
		if (num2 == 0)
		{
			num2 = 1;
		}
		if (percent < 1f)
		{
			num3 = num2 + 1;
		}
		float num4 = num - (float)num2;
		num2 = Mathf.Clamp(num2, 0, this.colliderRoot.transform.childCount - 1);
		num3 = Mathf.Clamp(num3, num2, this.colliderRoot.transform.childCount - 1);
		return Vector3.Lerp(this.colliderRoot.transform.GetChild(num2).position, this.colliderRoot.transform.GetChild(num3).position, num4);
	}

	// Token: 0x06000CC3 RID: 3267 RVA: 0x0003FB70 File Offset: 0x0003DD70
	private int GetClosestChild(Vector3 center)
	{
		float num = 100000f;
		int num2 = -1;
		for (int i = 0; i < this.colliderRoot.transform.childCount; i++)
		{
			float num3 = Vector3.Distance(center, this.colliderRoot.transform.GetChild(i).position);
			if (num3 < num)
			{
				num = num3;
				num2 = i;
			}
		}
		return num2;
	}

	// Token: 0x04000BB0 RID: 2992
	public float minDist = 25f;

	// Token: 0x04000BB1 RID: 2993
	public float maxDist = 50f;

	// Token: 0x04000BB2 RID: 2994
	public float maxHeightDifference = 100f;

	// Token: 0x04000BB3 RID: 2995
	public float normalYMult = 1f;

	// Token: 0x04000BB4 RID: 2996
	public float minDown = 5f;

	// Token: 0x04000BB5 RID: 2997
	public float maxDown = 30f;

	// Token: 0x04000BB6 RID: 2998
	public float meshLength = 40f;

	// Token: 0x04000BB7 RID: 2999
	public int vineType;

	// Token: 0x04000BB8 RID: 3000
	public float colliderOffset;

	// Token: 0x04000BB9 RID: 3001
	public JungleVine.ColliderType colliderType;

	// Token: 0x04000BBA RID: 3002
	public Transform colliderTransform;

	// Token: 0x04000BBB RID: 3003
	public Vector2 boxShape = Vector2.one;

	// Token: 0x04000BBC RID: 3004
	public PhotonView photonView;

	// Token: 0x04000BBD RID: 3005
	public Transform connectTo;

	// Token: 0x04000BBE RID: 3006
	private readonly int segments = 50;

	// Token: 0x04000BBF RID: 3007
	private float totalLength;

	// Token: 0x04000BC0 RID: 3008
	public Transform hangCenter;

	// Token: 0x04000BC1 RID: 3009
	public string displayName;

	// Token: 0x04000BC2 RID: 3010
	public Transform colliderRoot;

	// Token: 0x0200038D RID: 909
	public enum ColliderType
	{
		// Token: 0x0400131E RID: 4894
		Capsule,
		// Token: 0x0400131F RID: 4895
		Box
	}
}
